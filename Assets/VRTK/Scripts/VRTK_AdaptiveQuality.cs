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
    using System.Text;
    using UnityEngine;
    using UnityEngine.VR;

    /// <summary>
    /// Adaptive Quality dynamically changes rendering settings to maintain VR framerate while maximizing GPU utilization.
    /// </summary>
    /// <remarks>
    ///   > **Only Compatible With Unity 5.4 and above**
    ///
    /// The Adaptive Quality script is attached to the `eye` object within the `[CameraRig]` prefab.
    /// <para>&#160;</para>
    /// There are two goals:
    /// <list type="bullet">
    /// <item> <description>Reduce the chances of dropping frames and reprojecting</description> </item>
    /// <item> <description>Increase quality when there are idle GPU cycles</description> </item>
    /// </list>
    /// <para />
    /// This script currently changes the following to reach these goals:
    /// <list type="bullet">
    /// <item> <description>Rendering resolution and viewport (aka Dynamic Resolution)</description> </item>
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

        [Tooltip("Toggles whether the render quality is dynamically adjusted to maintain VR framerate.\n\n"
                 + "If unchecked, the renderer will render at the recommended resolution provided by the current `VRDevice`.")]
        public bool active = true;

        [Tooltip("Toggles whether to show the debug overlay.\n\n"
                 + "Each square represents a different level on the quality scale. Levels increase from left to right,"
                 + " and the first green box that is lit above represents the recommended render target resolution provided by"
                 + " the current `VRDevice`. The yellow boxes represent resolutions below the recommended render target resolution.\n"
                 + "The currently lit box becomes red whenever the user is likely seeing reprojection in the HMD since the"
                 + " application isn't maintaining VR framerate. If lit, the box all the way on the left is almost always lit red because"
                 + " it represents the lowest render scale with reprojection on.")]
        public bool drawDebugVisualization;

        [Tooltip("Toggles whether to allow keyboard shortcuts to control this script.\n\n"
                 + "* The supported shortcuts are:\n"
                 + " * `Shift+F1`: Toggle debug visualization on/off\n"
                 + " * `Shift+F2`: Toggle usage of override render scale on/off\n"
                 + " * `Shift+F3`: Decrease override render scale level\n"
                 + " * `Shift+F4`: Increase override render scale level")]
        public bool respondsToKeyboardShortcuts = true;

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
        public bool respondsToCommandLineArguments = true;

        [Tooltip("The MSAA level to use.")]
        [Header("Quality")]
        [Range(0, 8)]
        public int msaaLevel = 4;

        [Tooltip("The minimum allowed render scale.")]
        [Header("Render Scale")]
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

        [Tooltip("Toggles whether to override the used render scale level.")]
        [Header("Override")]
        [NonSerialized]
        public bool overrideRenderScale;
        [Tooltip("The render scale level to override the current one with.")]
        [NonSerialized]
        public int overrideRenderScaleLevel;

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
        /// A render scale of 1.0 represents the recommended render target resolution provided by the current `VRDevice`.
        /// </remarks>
        public float currentRenderScale
        {
            get { return VRSettings.renderScale * VRSettings.renderViewportScale; }
        }

        /// <summary>
        /// The recommended render target resolution provided by the current `VRDevice`.
        /// </summary>
        public Vector2 defaultRenderTargetResolution
        {
            get { return RenderTargetResolutionForRenderScale(allRenderScales[defaultRenderScaleLevel]); }
        }

        /// <summary>
        /// The current render target resolution.
        /// </summary>
        public Vector2 currentRenderTargetResolution
        {
            get { return RenderTargetResolutionForRenderScale(currentRenderScale); }
        }

        #endregion

        #region Private fields

        /// <summary>
        /// The frame duration in milliseconds to fallback to if the current `VRDevice` specifies no refresh rate.
        /// </summary>
        private const float DefaultFrameDurationInMilliseconds = 1000f / 90f;

        private readonly List<float> allRenderScales = new List<float>();
        private int defaultRenderScaleLevel;
        private int currentRenderScaleLevel;
        private float previousMinimumRenderScale;
        private float previousMaximumRenderScale;
        private float previousRenderScaleFillRateStepSizeInPercent;

        private int lastRenderScaleChangeFrameCount;
        private readonly float[] frameTimeRingBuffer = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                                         0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        private int frameTimeRingBufferIndex;
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
            var stringBuilder = new StringBuilder("Adaptive Quality:\n");

            for (int index = 0; index < allRenderScales.Count; index++)
            {
                float renderScale = allRenderScales[index];
                var renderTargetResolution = RenderTargetResolutionForRenderScale(renderScale);

                stringBuilder.Append(index);
                stringBuilder.Append(". ");
                stringBuilder.Append(" ");
                stringBuilder.Append((int)renderTargetResolution.x);
                stringBuilder.Append("x");
                stringBuilder.Append((int)renderTargetResolution.y);
                stringBuilder.Append(" ");
                stringBuilder.Append(renderScale);

                if (index == 0)
                {
                    stringBuilder.Append(" (Interleaved reprojection hint)");
                }
                else if (index == defaultRenderScaleLevel)
                {
                    stringBuilder.Append(" (Default)");
                }

                if (index != allRenderScales.Count - 1)
                {
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }

        #region MonoBehaviour methods

        private void OnEnable()
        {
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
        }

#if UNITY_5_4_1 || UNITY_5_5_OR_NEWER
        private void LateUpdate()
        {
            UpdateRenderScale();
        }
#endif

        private void OnPreCull()
        {
#if !(UNITY_5_4_1 || UNITY_5_5_OR_NEWER)
            UpdateRenderScale();
#endif
            UpdateMSAALevel();
        }

        #endregion

        private void HandleCommandLineArguments()
        {
            if (!respondsToCommandLineArguments)
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
                        active = false;
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
                        overrideRenderScale = true;
                        overrideRenderScaleLevel = int.Parse(nextArgument);
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
            if (!respondsToKeyboardShortcuts || !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                return;
            }

            // Toggle debug visualization on Shift+F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                drawDebugVisualization = !drawDebugVisualization;
                CreateOrDestroyDebugVisualization();
            }

            // Toggle usage of override render scale on Shift+F2
            if (Input.GetKeyDown(KeyCode.F2))
            {
                overrideRenderScale = !overrideRenderScale;
            }

            // Decrease override render scale level on Shift+F3
            if (Input.GetKeyDown(KeyCode.F3))
            {
                overrideRenderScaleLevel = Mathf.Clamp(overrideRenderScaleLevel - 1, 0, allRenderScales.Count - 1);
            }

            // Increase override render scale level on Shift+F4
            if (Input.GetKeyDown(KeyCode.F4))
            {
                overrideRenderScaleLevel = Mathf.Clamp(overrideRenderScaleLevel + 1, 0, allRenderScales.Count - 1);
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

            // Figure out default level for debug visualization
            defaultRenderScaleLevel = Mathf.Clamp(
                allRenderScales.FindIndex(renderScale => renderScale >= 1.0f),
                1,
                allRenderScales.Count - 1);
            currentRenderScaleLevel = defaultRenderScaleLevel;

            overrideRenderScaleLevel = Mathf.Clamp(overrideRenderScaleLevel, 0, allRenderScales.Count - 1);
            currentRenderScaleLevel = Mathf.Clamp(currentRenderScaleLevel, 0, allRenderScales.Count - 1);
        }

        private void UpdateRenderScale()
        {
            if (!active)
            {
                SetRenderScale(1.0f, 1.0f);
                return;
            }

            // Add latest timing to ring buffer
            frameTimeRingBufferIndex = (frameTimeRingBufferIndex + 1) % frameTimeRingBuffer.Length;
            frameTimeRingBuffer[frameTimeRingBufferIndex] = VRStats.gpuTimeLastFrame;

            // Rendering in low resolution means adaptive quality needs to scale back the render scale target to free up gpu cycles
            bool renderInLowResolution = VRTK_SDK_Bridge.ShouldAppRenderWithLowResources();

            // Thresholds
            float thresholdModifier = renderInLowResolution
                                      ? singleFrameDurationInMilliseconds * 0.75f
                                      : singleFrameDurationInMilliseconds;
            float lowThresholdInMilliseconds = 0.7f * thresholdModifier;
            float extrapolationThresholdInMilliseconds = 0.85f * thresholdModifier;
            float highThresholdInMilliseconds = 0.9f * thresholdModifier;

            // Get latest 3 frames
            float frameMinus0 = frameTimeRingBuffer[(frameTimeRingBufferIndex - 0 + frameTimeRingBuffer.Length) % frameTimeRingBuffer.Length];
            float frameMinus1 = frameTimeRingBuffer[(frameTimeRingBufferIndex - 1 + frameTimeRingBuffer.Length) % frameTimeRingBuffer.Length];
            float frameMinus2 = frameTimeRingBuffer[(frameTimeRingBufferIndex - 2 + frameTimeRingBuffer.Length) % frameTimeRingBuffer.Length];

            int previousLevel = currentRenderScaleLevel;

            // Always drop 2 levels except when dropping from level 2
            int dropTargetLevel = previousLevel == 2 ? 1 : previousLevel - 2;
            int newLevel = Mathf.Clamp(dropTargetLevel, 0, allRenderScales.Count - 1);

            // Ignore frame timings if overriding
            if (overrideRenderScale)
            {
                currentRenderScaleLevel = overrideRenderScaleLevel;
            }
            // Rapidly reduce quality 2 levels if last frame was critical
            else if (Time.frameCount >= lastRenderScaleChangeFrameCount + 2 + 1
                && frameMinus0 > highThresholdInMilliseconds
                && newLevel != previousLevel)
            {
                currentRenderScaleLevel = newLevel;
                lastRenderScaleChangeFrameCount = Time.frameCount;
            }
            // Rapidly reduce quality 2 levels if last 3 frames are expensive
            else if (Time.frameCount >= lastRenderScaleChangeFrameCount + 2 + 3
                && frameMinus0 > highThresholdInMilliseconds
                && frameMinus1 > highThresholdInMilliseconds
                && frameMinus2 > highThresholdInMilliseconds
                && newLevel != previousLevel)
            {
                currentRenderScaleLevel = newLevel;
                lastRenderScaleChangeFrameCount = Time.frameCount;
            }
            // Predict next frame's cost using linear extrapolation: max(frame-1 to frame+1, frame-2 to frame+1)
            else if (Time.frameCount >= lastRenderScaleChangeFrameCount + 2 + 2
                     && frameMinus0 > extrapolationThresholdInMilliseconds)
            {
                float frameDelta = frameMinus0 - frameMinus1;

                // Use frame-2 if it's available
                if (Time.frameCount >= lastRenderScaleChangeFrameCount + 2 + 3)
                {
                    float frameDelta2 = (frameMinus0 - frameMinus2) * 0.5f;
                    frameDelta = Mathf.Max(frameDelta, frameDelta2);
                }

                if (frameMinus0 + frameDelta > highThresholdInMilliseconds
                    && newLevel != previousLevel)
                {
                    currentRenderScaleLevel = newLevel;
                    lastRenderScaleChangeFrameCount = Time.frameCount;
                }
            }
            else
            {
                // Increase quality 1 level if last 3 frames are cheap
                newLevel = Mathf.Clamp(previousLevel + 1, 0, allRenderScales.Count - 1);
                if (Time.frameCount >= lastRenderScaleChangeFrameCount + 2 + 3
                    && frameMinus0 < lowThresholdInMilliseconds
                    && frameMinus1 < lowThresholdInMilliseconds
                    && frameMinus2 < lowThresholdInMilliseconds
                    && newLevel != previousLevel)
                {
                    currentRenderScaleLevel = newLevel;
                    lastRenderScaleChangeFrameCount = Time.frameCount;
                }
            }

            // Force on interleaved reprojection for level 0 which is just a replica of level 1 with reprojection enabled
            float additionalViewportScale = 1.0f;
            if (!hmdDisplayIsOnDesktop)
            {
                if (currentRenderScaleLevel == 0)
                {
                    if (interleavedReprojectionEnabled && frameMinus0 < singleFrameDurationInMilliseconds * 0.85f)
                    {
                        interleavedReprojectionEnabled = false;
                    }
                    else if (frameMinus0 > singleFrameDurationInMilliseconds * 0.925f)
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
            // Not running in direct mode! Interleaved reprojection not supported, so scale down the viewport
            else if (currentRenderScaleLevel == 0)
            {
                additionalViewportScale = 0.8f;
            }

            float newRenderScale = allRenderScales[allRenderScales.Count - 1];
            float newRenderViewportScale = allRenderScales[currentRenderScaleLevel] / newRenderScale * additionalViewportScale;

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
                debugVisualizationQuad.transform.parent = transform;
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

            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.LevelsCount, allRenderScales.Count);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.DefaultLevel, defaultRenderScaleLevel);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.CurrentLevel, currentRenderScaleLevel);
            debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.LastFrameIsInBudget, lastFrameIsInBudget);
        }

        #endregion

        #region Private helper classes

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

        private static class ShaderPropertyIDs
        {
            public static readonly int LevelsCount = Shader.PropertyToID("_LevelsCount");
            public static readonly int DefaultLevel = Shader.PropertyToID("_DefaultLevel");
            public static readonly int CurrentLevel = Shader.PropertyToID("_CurrentLevel");
            public static readonly int LastFrameIsInBudget = Shader.PropertyToID("_LastFrameIsInBudget");
        }

        #endregion
    }
}
#endif