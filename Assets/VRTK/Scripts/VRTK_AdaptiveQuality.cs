// Adaptive Quality|Scripts|0240

// Adapted from The Lab Renderer's ValveCamera.cs, available at
// https://github.com/ValveSoftware/the_lab_renderer/blob/ae64c48a8ccbe5406aba1e39b160d4f2f7156c2c/Assets/TheLabRenderer/Scripts/ValveCamera.cs
// For The Lab Renderer's license see THIRD_PARTY_NOTICES.
// **Only Compatible With Unity 5.4 and above**

#if (UNITY_5_4_OR_NEWER)
namespace VRTK
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEngine.VR;

    /// <summary>
    /// Adaptive Quality dynamically changes rendering settings to maintain VR framerate while maximizing GPU utilization.
    /// </summary>
    /// <remarks>
    ///   > **Only Compatible With Unity 5.4 and above**
    ///
    /// The Adaptive Quality script is attached to the `[CameraRig]` game object.
    ///
    /// There are two goals:
    /// <list type="bullet">
    /// <item> <description>Reduce the chances of dropping frames and reprojecting</description> </item>
    /// <item> <description>Increase quality when there are idle GPU cycles</description> </item>
    /// </list>
    /// <para />
    /// This script currently changes the following to reach these goals:
    /// <list type="bullet">
    /// <item> <description>Rendering resolution and viewport size (aka Dynamic Resolution)</description> </item>
    /// </list>
    /// <para />
    /// In the future it could be changed to also change the following:
    /// <list type="bullet">
    /// <item> <description>MSAA level</description> </item>
    /// <item> <description>Fixed Foveated Rendering</description> </item>
    /// <item> <description>Radial Density Masking</description> </item>
    /// <item> <description>(Non-fixed) Foveated Rendering (once HMDs support eye tracking)</description> </item>
    /// </list>
    /// <para />
    /// Some shaders, especially Image Effects, need to be modified to work with the changed render scale. To fix them
    /// pass `1.0f / VRSettings.renderViewportScale` into the shader and scale all incoming UV values with it in the vertex
    /// program. Do this by using `Material.SetFloat` to set the value in the script that configures the shader.
    /// <para />
    /// In more detail:
    /// <list type="bullet">
    /// <item> <description>In the `.shader` file: Add a new runtime-set property value `float _InverseOfRenderViewportScale`
    /// and add `vertexInput.texcoord *= _InverseOfRenderViewportScale` to the start of the vertex program</description> </item>
    /// <item> <description>In the `.cs` file: Before using the material (eg. `Graphics.Blit`) add
    /// `material.SetFloat("_InverseOfRenderViewportScale", 1.0f / VRSettings.renderViewportScale)`</description> </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/039_CameraRig_AdaptiveQuality` displays the frames per second in the centre of the headset view.
    /// The debug visualization of this script is displayed near the top edge of the headset view.
    /// Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres.
    /// Eventually when lots of spheres are present the FPS will drop and demonstrate the script.
    /// </example>
    public sealed class VRTK_AdaptiveQuality : MonoBehaviour
    {
        #region Public fields

        [Tooltip("Toggles whether to show the debug overlay.\n\n"
                 + "Each square represents a different level on the quality scale. Levels increase from left to right,"
                 + " the first green box that is lit above represents the recommended render target resolution provided by the"
                 + " current `VRDevice`, the box that is lit below in cyan represents the current resolution and the filled box"
                 + " represents the current viewport scale. The yellow boxes represent resolutions below the recommended render target resolution.\n"
                 + "The currently lit box becomes red whenever the user is likely seeing reprojection in the HMD since the"
                 + " application isn't maintaining VR framerate. If lit, the box all the way on the left is almost always lit"
                 + " red because it represents the lowest render scale with reprojection on.")]
        public bool drawDebugVisualization;

        [Tooltip("Toggles whether to allow keyboard shortcuts to control this script.\n\n"
                 + "* The supported shortcuts are:\n"
                 + " * `Shift+F1`: Toggle debug visualization on/off\n"
                 + " * `Shift+F2`: Toggle usage of override render scale on/off\n"
                 + " * `Shift+F3`: Decrease override render scale level\n"
                 + " * `Shift+F4`: Increase override render scale level")]
        public bool allowKeyboardShortcuts = true;

        [Tooltip("Toggles whether to allow command line arguments to control this script at startup of the standalone build.\n\n"
                 + "* The supported command line arguments all begin with '-' and are:\n"
                 + " * `-noaq`: Disable adaptive quality\n"
                 + " * `-aqminscale X`: Set minimum render scale to X\n"
                 + " * `-aqmaxscale X`: Set maximum render scale to X\n"
                 + " * `-aqmaxres X`: Set maximum render target dimension to X\n"
                 + " * `-aqfillratestep X`: Set render scale fill rate step size in percent to X (X from 1 to 100)\n"
                 + " * `-aqoverride X`: Set override render scale level to X\n"
                 + " * `-vrdebug`: Enable debug visualization\n"
                 + " * `-msaa X`: Set MSAA level to X")]
        public bool allowCommandLineArguments = true;

        [Tooltip("The MSAA level to use.")]
        [Header("Quality")]
        [Range(0, 8)]
        public int msaaLevel = 4;

        [Tooltip("Toggles whether the render viewport scale is dynamically adjusted to maintain VR framerate.\n\n"
                 + "If unchecked, the renderer will render at the recommended resolution provided by the current `VRDevice`.")]
        public bool scaleRenderViewport = true;
        [Tooltip("The minimum allowed render scale.")]
        [Range(0.01f, 5)]
        public float minimumRenderScale = 0.8f;
        [Tooltip("The maximum allowed render scale.")]
        public float maximumRenderScale = 1.4f;
        [Tooltip("The maximum allowed render target dimension.\n\n"
                 + "This puts an upper limit on the size of the render target regardless of the maximum render scale.")]
        public int maximumRenderTargetDimension = 4096;
        [Tooltip("The fill rate step size in percent by which the render scale levels will be calculated.")]
        [Range(1, 100)]
        public int renderScaleFillRateStepSizeInPercent = 15;
        [Tooltip("Toggles whether the render target resolution is dynamically adjusted to maintain VR framerate.\n\n"
                 + "If unchecked, the renderer will use the maximum target resolution specified by `maximumRenderScale`.")]
        public bool scaleRenderTargetResolution = false;

        [Tooltip("Toggles whether to override the used render viewport scale level.")]
        [Header("Override")]
        [NonSerialized]
        public bool overrideRenderViewportScale;
        [Tooltip("The render viewport scale level to override the current one with.")]
        [NonSerialized]
        public int overrideRenderViewportScaleLevel;

        #endregion

        #region Public readonly fields & properties

        /// <summary>
        /// All the calculated render scales.
        /// </summary>
        /// <remarks>
        /// The elements of this collection are to be interpreted as modifiers to the recommended render target
        /// resolution provided by the current `VRDevice`.
        /// </remarks>
        public readonly ReadOnlyCollection<float> renderScales;

        /// <summary>
        /// The current render scale.
        /// </summary>
        /// <remarks>
        /// A render scale of `1.0` represents the recommended render target resolution provided by the current `VRDevice`.
        /// </remarks>
        public static float CurrentRenderScale
        {
            get { return VRSettings.renderScale * VRSettings.renderViewportScale; }
        }

        /// <summary>
        /// The recommended render target resolution provided by the current `VRDevice`.
        /// </summary>
        public Vector2 defaultRenderTargetResolution
        {
            get { return RenderTargetResolutionForRenderScale(allRenderScales[defaultRenderViewportScaleLevel]); }
        }

        /// <summary>
        /// The current render target resolution.
        /// </summary>
        public Vector2 currentRenderTargetResolution
        {
            get { return RenderTargetResolutionForRenderScale(CurrentRenderScale); }
        }

        #endregion

        #region Private fields

        /// <summary>
        /// The frame duration in milliseconds to fallback to if the current `VRDevice` specifies no refresh rate.
        /// </summary>
        private const float DefaultFrameDurationInMilliseconds = 1000f / 90f;

        private readonly AdaptiveSetting<int> renderViewportScaleSetting = new AdaptiveSetting<int>(0, 30, 10);
        private readonly AdaptiveSetting<int> renderScaleSetting = new AdaptiveSetting<int>(0, 180, 90);

        private readonly List<float> allRenderScales = new List<float>();
        private int defaultRenderViewportScaleLevel;
        private float previousMinimumRenderScale;
        private float previousMaximumRenderScale;
        private float previousRenderScaleFillRateStepSizeInPercent;

        private readonly Timing timing = new Timing();
        private int lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount;

        private bool interleavedReprojectionEnabled;
        private bool hmdDisplayIsOnDesktop;
        private float singleFrameDurationInMilliseconds;

        private GameObject debugVisualizationQuad;
        private Material debugVisualizationQuadMaterial;

        #endregion

        public VRTK_AdaptiveQuality()
        {
            renderScales = allRenderScales.AsReadOnly();
        }

        /// <summary>
        /// Calculates and returns the render target resolution for a given render scale.
        /// </summary>
        /// <param name="renderScale">
        /// The render scale to calculate the render target resolution with.
        /// </param>
        /// <returns>
        /// The render target resolution for `renderScale`.
        /// </returns>
        public static Vector2 RenderTargetResolutionForRenderScale(float renderScale)
        {
            return new Vector2((int)(VRSettings.eyeTextureWidth / VRSettings.renderScale * renderScale),
                               (int)(VRSettings.eyeTextureHeight / VRSettings.renderScale * renderScale));
        }

        /// <summary>
        /// Calculates and returns the biggest allowed maximum render scale to be used for `maximumRenderScale`
        /// given the current `maximumRenderTargetDimension`.
        /// </summary>
        /// <returns>
        /// The biggest allowed maximum render scale.
        /// </returns>
        public float BiggestAllowedMaximumRenderScale()
        {
            if (VRSettings.eyeTextureWidth == 0 || VRSettings.eyeTextureHeight == 0)
            {
                return maximumRenderScale;
            }

            float maximumHorizontalRenderScale = maximumRenderTargetDimension * VRSettings.renderScale
                                                 / VRSettings.eyeTextureWidth;
            float maximumVerticalRenderScale = maximumRenderTargetDimension * VRSettings.renderScale
                                               / VRSettings.eyeTextureHeight;
            return Mathf.Min(maximumHorizontalRenderScale, maximumVerticalRenderScale);
        }

        /// <summary>
        /// A summary of this script by listing all the calculated render scales with their
        /// corresponding render target resolution.
        /// </summary>
        /// <returns>
        /// The summary.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder("Adaptive Quality\n");
            stringBuilder.AppendLine("Render Scale:");
            stringBuilder.AppendLine("Level - Resolution - Multiplier");

            for (int index = 0; index < allRenderScales.Count; index++)
            {
                float renderScale = allRenderScales[index];
                var renderTargetResolution = RenderTargetResolutionForRenderScale(renderScale);

                stringBuilder.AppendFormat(
                    "{0, 3} {1, 5}x{2, -5} {3, -8}",
                    index,
                    (int)renderTargetResolution.x,
                    (int)renderTargetResolution.y,
                    renderScale);

                if (index == 0)
                {
                    stringBuilder.Append(" (Interleaved reprojection hint)");
                }
                else if (index == defaultRenderViewportScaleLevel)
                {
                    stringBuilder.Append(" (Default)");
                }

                if (index == renderViewportScaleSetting.currentValue)
                {
                    stringBuilder.Append(" (Current Viewport)");
                }

                if (index == renderScaleSetting.currentValue)
                {
                    stringBuilder.Append(" (Current Target Resolution)");
                }

                if (index != allRenderScales.Count - 1)
                {
                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }

        #region MonoBehaviour methods

        private void OnEnable()
        {
            Camera.onPreCull += OnCameraPreCull;

            hmdDisplayIsOnDesktop = VRTK_SDK_Bridge.IsDisplayOnDesktop();
            singleFrameDurationInMilliseconds = VRDevice.refreshRate > 0.0f
                                                ? 1000.0f / VRDevice.refreshRate
                                                : DefaultFrameDurationInMilliseconds;

            HandleCommandLineArguments();

            if (!Application.isEditor)
            {
                OnValidate();
            }

            CreateOrDestroyDebugVisualization();
        }

        private void OnDisable()
        {
            Camera.onPreCull -= OnCameraPreCull;

            CreateOrDestroyDebugVisualization();
            SetRenderScale(1.0f, 1.0f);
        }

        private void OnValidate()
        {
            minimumRenderScale = Mathf.Max(0.01f, minimumRenderScale);
            maximumRenderScale = Mathf.Max(minimumRenderScale, maximumRenderScale);
            maximumRenderTargetDimension = Mathf.Max(2, maximumRenderTargetDimension);
            renderScaleFillRateStepSizeInPercent = Mathf.Max(1, renderScaleFillRateStepSizeInPercent);
            msaaLevel = msaaLevel == 1 ? 0 : Mathf.Clamp(Mathf.ClosestPowerOfTwo(msaaLevel), 0, 8);

            CreateOrDestroyDebugVisualization();
        }

        private void Update()
        {
            HandleKeyPresses();
            UpdateRenderScaleLevels();
            UpdateDebugVisualization();

            timing.SaveCurrentFrameTiming();
        }

#if UNITY_5_4_1 || UNITY_5_4_2 || UNITY_5_5_OR_NEWER
        private void LateUpdate()
        {
            UpdateRenderScale();
        }
#endif

        private void OnCameraPreCull(Camera camera)
        {
            if (camera.transform != VRTK_SDK_Bridge.GetHeadsetCamera())
            {
                return;
            }

#if !(UNITY_5_4_1 || UNITY_5_4_2 || UNITY_5_5_OR_NEWER)
            UpdateRenderScale();
#endif
            UpdateMSAALevel();
        }

        #endregion

        private void HandleCommandLineArguments()
        {
            if (!allowCommandLineArguments)
            {
                return;
            }

            var commandLineArguments = Environment.GetCommandLineArgs();

            for (int index = 0; index < commandLineArguments.Length; index++)
            {
                string argument = commandLineArguments[index];
                string nextArgument = index + 1 < commandLineArguments.Length ? commandLineArguments[index + 1] : "";

                switch (argument)
                {
                    case CommandLineArguments.Disable:
                        scaleRenderViewport = false;
                        break;
                    case CommandLineArguments.MinimumRenderScale:
                        minimumRenderScale = float.Parse(nextArgument);
                        break;
                    case CommandLineArguments.MaximumRenderScale:
                        maximumRenderScale = float.Parse(nextArgument);
                        break;
                    case CommandLineArguments.MaximumRenderTargetDimension:
                        maximumRenderTargetDimension = int.Parse(nextArgument);
                        break;
                    case CommandLineArguments.RenderScaleFillRateStepSizeInPercent:
                        renderScaleFillRateStepSizeInPercent = int.Parse(nextArgument);
                        break;
                    case CommandLineArguments.OverrideRenderScaleLevel:
                        overrideRenderViewportScale = true;
                        overrideRenderViewportScaleLevel = int.Parse(nextArgument);
                        break;
                    case CommandLineArguments.DrawDebugVisualization:
                        drawDebugVisualization = true;
                        CreateOrDestroyDebugVisualization();
                        break;
                    case CommandLineArguments.MSAALevel:
                        msaaLevel = int.Parse(nextArgument);
                        break;
                }
            }
        }

        private void HandleKeyPresses()
        {
            if (!allowKeyboardShortcuts || !KeyboardShortcuts.Modifiers.Any(Input.GetKey))
            {
                return;
            }

            if (Input.GetKeyDown(KeyboardShortcuts.ToggleDrawDebugVisualization))
            {
                drawDebugVisualization = !drawDebugVisualization;
                CreateOrDestroyDebugVisualization();
            }
            else if (Input.GetKeyDown(KeyboardShortcuts.ToggleOverrideRenderScale))
            {
                overrideRenderViewportScale = !overrideRenderViewportScale;
            }
            else if (Input.GetKeyDown(KeyboardShortcuts.DecreaseOverrideRenderScaleLevel))
            {
                overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel - 1);
            }
            else if (Input.GetKeyDown(KeyboardShortcuts.IncreaseOverrideRenderScaleLevel))
            {
                overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel + 1);
            }
        }

        private void UpdateMSAALevel()
        {
            if (QualitySettings.antiAliasing != msaaLevel)
            {
                QualitySettings.antiAliasing = msaaLevel;
            }
        }

        #region Render scale methods

        private void UpdateRenderScaleLevels()
        {
            if (Mathf.Abs(previousMinimumRenderScale - minimumRenderScale) <= float.Epsilon
                && Mathf.Abs(previousMaximumRenderScale - maximumRenderScale) <= float.Epsilon
                && Mathf.Abs(previousRenderScaleFillRateStepSizeInPercent - renderScaleFillRateStepSizeInPercent) <= float.Epsilon)
            {
                return;
            }

            previousMinimumRenderScale = minimumRenderScale;
            previousMaximumRenderScale = maximumRenderScale;
            previousRenderScaleFillRateStepSizeInPercent = renderScaleFillRateStepSizeInPercent;

            allRenderScales.Clear();

            // Respect maximumRenderTargetDimension
            float allowedMaximumRenderScale = BiggestAllowedMaximumRenderScale();
            float renderScaleToAdd = Mathf.Min(minimumRenderScale, allowedMaximumRenderScale);

            // Add min scale as the reprojection scale
            allRenderScales.Add(renderScaleToAdd);

            // Add all entries
            while (renderScaleToAdd <= maximumRenderScale)
            {
                allRenderScales.Add(renderScaleToAdd);
                renderScaleToAdd =
                    Mathf.Sqrt((renderScaleFillRateStepSizeInPercent + 100) / 100.0f * renderScaleToAdd * renderScaleToAdd);

                if (renderScaleToAdd > allowedMaximumRenderScale)
                {
                    // Too large
                    break;
                }
            }

            // Figure out default render viewport scale level for debug visualization
            defaultRenderViewportScaleLevel = Mathf.Clamp(
                allRenderScales.FindIndex(renderScale => renderScale >= 1.0f),
                1,
                allRenderScales.Count - 1);
            renderViewportScaleSetting.currentValue = defaultRenderViewportScaleLevel;
            renderScaleSetting.currentValue = defaultRenderViewportScaleLevel;
            overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel);
        }

        private void UpdateRenderScale()
        {
            if (!scaleRenderViewport)
            {
                renderViewportScaleSetting.currentValue = defaultRenderViewportScaleLevel;
                renderScaleSetting.currentValue = defaultRenderViewportScaleLevel;
                SetRenderScale(1.0f, 1.0f);

                return;
            }

            // Rendering in low resolution means adaptive quality needs to scale back the render scale target to free up gpu cycles
            bool renderInLowResolution = VRTK_SDK_Bridge.ShouldAppRenderWithLowResources();

            // Thresholds
            float allowedSingleFrameDurationInMilliseconds = renderInLowResolution
                                                             ? singleFrameDurationInMilliseconds * 0.75f
                                                             : singleFrameDurationInMilliseconds;
            float lowThresholdInMilliseconds = 0.7f * allowedSingleFrameDurationInMilliseconds;
            float extrapolationThresholdInMilliseconds = 0.85f * allowedSingleFrameDurationInMilliseconds;
            float highThresholdInMilliseconds = 0.9f * allowedSingleFrameDurationInMilliseconds;

            int newRenderViewportScaleLevel = renderViewportScaleSetting.currentValue;

            // Rapidly reduce render viewport scale level if cost of last 1 or 3 frames, or the predicted next frame's cost are expensive
            if (timing.WasFrameTimingBad(
                    1,
                    highThresholdInMilliseconds,
                    renderViewportScaleSetting.lastChangeFrameCount,
                    renderViewportScaleSetting.decreaseFrameCost)
                || timing.WasFrameTimingBad(
                    3,
                    highThresholdInMilliseconds,
                    renderViewportScaleSetting.lastChangeFrameCount,
                    renderViewportScaleSetting.decreaseFrameCost)
                || timing.WillFrameTimingBeBad(
                    extrapolationThresholdInMilliseconds,
                    highThresholdInMilliseconds,
                    renderViewportScaleSetting.lastChangeFrameCount,
                    renderViewportScaleSetting.decreaseFrameCost))
            {
                // Always drop 2 levels except when dropping from level 2 (level 0 is for reprojection)
                newRenderViewportScaleLevel = ClampRenderScaleLevel(renderViewportScaleSetting.currentValue == 2
                                                                    ? 1
                                                                    : renderViewportScaleSetting.currentValue - 2);
            }
            // Rapidly increase render viewport scale level if last 12 frames are cheap
            else if (timing.WasFrameTimingGood(
                    12,
                    lowThresholdInMilliseconds,
                    renderViewportScaleSetting.lastChangeFrameCount - renderViewportScaleSetting.increaseFrameCost,
                    renderViewportScaleSetting.increaseFrameCost))
            {
                newRenderViewportScaleLevel = ClampRenderScaleLevel(renderViewportScaleSetting.currentValue + 2);
            }
            // Slowly increase render viewport scale level if last 6 frames are cheap
            else if (timing.WasFrameTimingGood(
                6,
                lowThresholdInMilliseconds,
                renderViewportScaleSetting.lastChangeFrameCount,
                renderViewportScaleSetting.increaseFrameCost))
            {
                // Only increase by 1 level to prevent frame drops caused by adjusting
                newRenderViewportScaleLevel = ClampRenderScaleLevel(renderViewportScaleSetting.currentValue + 1);
            }

            // Apply and remember when render viewport scale level changed
            if (newRenderViewportScaleLevel != renderViewportScaleSetting.currentValue)
            {
                if (renderViewportScaleSetting.currentValue >= renderScaleSetting.currentValue
                    && newRenderViewportScaleLevel < renderScaleSetting.currentValue)
                {
                    lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount = Time.frameCount;
                }

                renderViewportScaleSetting.currentValue = newRenderViewportScaleLevel;
            }

            // Ignore frame timings if overriding
            if (overrideRenderViewportScale)
            {
                renderViewportScaleSetting.currentValue = overrideRenderViewportScaleLevel;
            }

            // Force on interleaved reprojection for level 0 which is just a replica of level 1 with reprojection enabled
            float additionalViewportScale = 1.0f;
            if (!hmdDisplayIsOnDesktop)
            {
                if (renderViewportScaleSetting.currentValue == 0)
                {
                    if (interleavedReprojectionEnabled && timing.GetFrameTiming(0) < singleFrameDurationInMilliseconds * 0.85f)
                    {
                        interleavedReprojectionEnabled = false;
                    }
                    else if (timing.GetFrameTiming(0) > singleFrameDurationInMilliseconds * 0.925f)
                    {
                        interleavedReprojectionEnabled = true;
                    }
                }
                else
                {
                    interleavedReprojectionEnabled = false;
                }

                VRTK_SDK_Bridge.ForceInterleavedReprojectionOn(interleavedReprojectionEnabled);
            }
            // Not running in direct mode! Interleaved reprojection not supported, so scale down the viewport some more
            else if (renderViewportScaleSetting.currentValue == 0)
            {
                additionalViewportScale = 0.8f;
            }

            int newRenderScaleLevel = renderScaleSetting.currentValue;
            int levelInBetween = (renderViewportScaleSetting.currentValue - renderScaleSetting.currentValue) / 2;

            // Increase render scale level to the level in between
            if (renderScaleSetting.currentValue < renderViewportScaleSetting.currentValue
                && Time.frameCount >= renderScaleSetting.lastChangeFrameCount + renderScaleSetting.increaseFrameCost)
            {
                newRenderScaleLevel = ClampRenderScaleLevel(renderScaleSetting.currentValue + Mathf.Max(1, levelInBetween));
            }
            // Decrease render scale level
            else if (renderScaleSetting.currentValue > renderViewportScaleSetting.currentValue
                     && Time.frameCount >= renderScaleSetting.lastChangeFrameCount + renderScaleSetting.decreaseFrameCost
                     && Time.frameCount >= lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount + renderViewportScaleSetting.increaseFrameCost)
            {
                // Slowly decrease render scale level to level in between if last 6 frames are cheap, otherwise rapidly
                newRenderScaleLevel = timing.WasFrameTimingGood(6, lowThresholdInMilliseconds, 0, 0)
                                      ? ClampRenderScaleLevel(renderScaleSetting.currentValue + Mathf.Min(-1, levelInBetween))
                                      : renderViewportScaleSetting.currentValue;
            }

            // Apply and remember when render scale level changed
            renderScaleSetting.currentValue = newRenderScaleLevel;

            if (!scaleRenderTargetResolution)
            {
                renderScaleSetting.currentValue = allRenderScales.Count - 1;
            }

            float newRenderScale = allRenderScales[renderScaleSetting.currentValue];
            float newRenderViewportScale = allRenderScales[Mathf.Min(renderViewportScaleSetting.currentValue, renderScaleSetting.currentValue)]
                                           / newRenderScale * additionalViewportScale;

            SetRenderScale(newRenderScale, newRenderViewportScale);
        }

        private static void SetRenderScale(float renderScale, float renderViewportScale)
        {
            if (Mathf.Abs(VRSettings.renderScale - renderScale) > float.Epsilon)
            {
                VRSettings.renderScale = renderScale;
            }
            if (Mathf.Abs(VRSettings.renderViewportScale - renderViewportScale) > float.Epsilon)
            {
                VRSettings.renderViewportScale = renderViewportScale;
            }
        }

        private int ClampRenderScaleLevel(int renderScaleLevel)
        {
            return Mathf.Clamp(renderScaleLevel, 0, allRenderScales.Count - 1);
        }

        #endregion

        #region Debug visualization methods

        private void CreateOrDestroyDebugVisualization()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (enabled && drawDebugVisualization && debugVisualizationQuad == null)
            {
                var mesh = new Mesh
                {
                    vertices =
                        new[]
                        {
                            new Vector3(-0.5f, 0.9f, 1.0f),
                            new Vector3(-0.5f, 1.0f, 1.0f),
                            new Vector3(0.5f, 1.0f, 1.0f),
                            new Vector3(0.5f, 0.9f, 1.0f)
                        },
                    uv =
                        new[]
                        {
                            new Vector2(0.0f, 0.0f),
                            new Vector2(0.0f, 1.0f),
                            new Vector2(1.0f, 1.0f),
                            new Vector2(1.0f, 0.0f)
                        },
                    triangles = new[] { 0, 1, 2, 0, 2, 3 }
                };
                mesh.Optimize();
                mesh.UploadMeshData(true);

                debugVisualizationQuad = new GameObject("AdaptiveQualityDebugVisualizationQuad");
                debugVisualizationQuad.transform.parent = VRTK_DeviceFinder.HeadsetTransform();
                debugVisualizationQuad.transform.localPosition = Vector3.forward;
                debugVisualizationQuad.transform.localRotation = Quaternion.identity;
                debugVisualizationQuad.AddComponent<MeshFilter>().mesh = mesh;

                debugVisualizationQuadMaterial = Resources.Load<Material>("AdaptiveQualityDebugVisualization");
                debugVisualizationQuad.AddComponent<MeshRenderer>().material = debugVisualizationQuadMaterial;
            }
            else if ((!enabled || !drawDebugVisualization) && debugVisualizationQuad != null)
            {
                Destroy(debugVisualizationQuad);

                debugVisualizationQuad = null;
                debugVisualizationQuadMaterial = null;
            }
        }

        private void UpdateDebugVisualization()
        {
            if (!drawDebugVisualization || debugVisualizationQuadMaterial == null)
            {
                return;
            }

            int lastFrameIsInBudget = interleavedReprojectionEnabled || VRStats.gpuTimeLastFrame > singleFrameDurationInMilliseconds
                                      ? 0
                                      : 1;

            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.RenderScaleLevelsCount, allRenderScales.Count);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.DefaultRenderViewportScaleLevel, defaultRenderViewportScaleLevel);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.CurrentRenderViewportScaleLevel, renderViewportScaleSetting.currentValue);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.CurrentRenderScaleLevel, renderScaleSetting.currentValue);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.LastFrameIsInBudget, lastFrameIsInBudget);
        }

        #endregion

        #region Private helper classes

        private sealed class AdaptiveSetting<T>
        {
            public T currentValue
            {
                get { return _currentValue; }
                set
                {
                    if (!EqualityComparer<T>.Default.Equals(value, _currentValue))
                    {
                        lastChangeFrameCount = Time.frameCount;
                    }

                    previousValue = _currentValue;
                    _currentValue = value;
                }
            }
            public T previousValue { get; private set; }
            public int lastChangeFrameCount { get; private set; }

            public readonly int increaseFrameCost;
            public readonly int decreaseFrameCost;

            private T _currentValue;

            public AdaptiveSetting(T currentValue, int increaseFrameCost, int decreaseFrameCost)
            {
                previousValue = currentValue;
                this.currentValue = currentValue;

                this.increaseFrameCost = increaseFrameCost;
                this.decreaseFrameCost = decreaseFrameCost;
            }
        }

        private static class CommandLineArguments
        {
            public const string Disable = "-noaq";

            public const string MinimumRenderScale = "-aqminscale";
            public const string MaximumRenderScale = "-aqmaxscale";
            public const string MaximumRenderTargetDimension = "-aqmaxres";
            public const string RenderScaleFillRateStepSizeInPercent = "-aqfillratestep";

            public const string OverrideRenderScaleLevel = "-aqoverride";

            public const string DrawDebugVisualization = "-vrdebug";

            public const string MSAALevel = "-msaa";
        }

        private static class KeyboardShortcuts
        {
            public static readonly KeyCode[] Modifiers = { KeyCode.LeftShift, KeyCode.RightShift };
            public const KeyCode ToggleDrawDebugVisualization = KeyCode.F1;
            public const KeyCode ToggleOverrideRenderScale = KeyCode.F2;
            public const KeyCode DecreaseOverrideRenderScaleLevel = KeyCode.F3;
            public const KeyCode IncreaseOverrideRenderScaleLevel = KeyCode.F4;
        }

        private static class ShaderPropertyIDs
        {
            public static readonly int RenderScaleLevelsCount = Shader.PropertyToID("_RenderScaleLevelsCount");
            public static readonly int DefaultRenderViewportScaleLevel = Shader.PropertyToID("_DefaultRenderViewportScaleLevel");
            public static readonly int CurrentRenderViewportScaleLevel = Shader.PropertyToID("_CurrentRenderViewportScaleLevel");
            public static readonly int CurrentRenderScaleLevel = Shader.PropertyToID("_CurrentRenderScaleLevel");
            public static readonly int LastFrameIsInBudget = Shader.PropertyToID("_LastFrameIsInBudget");
        }

        private sealed class Timing
        {
            private readonly float[] buffer = new float[12];
            private int bufferIndex;

            public void SaveCurrentFrameTiming()
            {
                bufferIndex = (bufferIndex + 1) % buffer.Length;
                buffer[bufferIndex] = VRStats.gpuTimeLastFrame;
            }

            public float GetFrameTiming(int framesAgo)
            {
                return buffer[(bufferIndex - framesAgo + buffer.Length) % buffer.Length];
            }

            public bool WasFrameTimingBad(int framesAgo, float thresholdInMilliseconds, int lastChangeFrameCount, int changeFrameCost)
            {
                if (!AreFramesAvailable(framesAgo, lastChangeFrameCount, changeFrameCost))
                {
                    // Too early to know
                    return false;
                }

                for (int frame = 0; frame < framesAgo; frame++)
                {
                    if (GetFrameTiming(frame) <= thresholdInMilliseconds)
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool WasFrameTimingGood(int framesAgo, float thresholdInMilliseconds, int lastChangeFrameCount, int changeFrameCost)
            {
                if (!AreFramesAvailable(framesAgo, lastChangeFrameCount, changeFrameCost))
                {
                    // Too early to know
                    return false;
                }

                for (int frame = 0; frame < framesAgo; frame++)
                {
                    if (GetFrameTiming(frame) > thresholdInMilliseconds)
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool WillFrameTimingBeBad(float extrapolationThresholdInMilliseconds, float thresholdInMilliseconds,
                                             int lastChangeFrameCount, int changeFrameCost)
            {
                if (!AreFramesAvailable(2, lastChangeFrameCount, changeFrameCost))
                {
                    // Too early to know
                    return false;
                }

                // Predict next frame's cost using linear extrapolation: max(frame-1 to frame+1, frame-2 to frame+1)
                float frameMinus0Timing = GetFrameTiming(0);
                if (frameMinus0Timing <= extrapolationThresholdInMilliseconds)
                {
                    return false;
                }

                float delta = frameMinus0Timing - GetFrameTiming(1);

                if (!AreFramesAvailable(3, lastChangeFrameCount, changeFrameCost))
                {
                    delta = Mathf.Max(delta, (frameMinus0Timing - GetFrameTiming(2)) / 2f);
                }

                return frameMinus0Timing + delta > thresholdInMilliseconds;
            }

            private static bool AreFramesAvailable(int framesAgo, int lastChangeFrameCount, int changeFrameCost)
            {
                return Time.frameCount >= framesAgo + lastChangeFrameCount + changeFrameCost;
            }
        }

        #endregion
    }
}
#endif
