// Frames Per Second Canvas|Prefabs|0010
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This canvas adds a frames per second text element to the headset. To use the prefab it must be placed into the scene then the headset camera needs attaching to the canvas:
    /// </summary>
    /// <remarks>
    ///   * Select `FramesPerSecondCanvas` object from the scene objects
    ///   * Find the `Canvas` component
    ///   * Set the `Render Camera` parameter to the camera used by the VR Headset (e.g. SteamVR: [CameraRig]-> Camera(Head) -> Camera(eye)])
    ///
    /// This script is pretty much a copy and paste from the script at: http://talesfromtherift.com/vr-fps-counter/ So all credit to Peter Koch for his work. Twitter: @peterept
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/018_CameraRig_FramesPerSecondCounter` displays the frames per second in the centre of the headset view. Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres. Eventually when lots of spheres are present the FPS will drop and demonstrate the prefab.
    /// </example>
    public class VRTK_FramesPerSecondViewer : MonoBehaviour
    {
        [Tooltip("Toggles whether the FPS text is visible.")]
        public bool displayFPS = true;
        [Tooltip("The frames per second deemed acceptable that is used as the benchmark to change the FPS text colour.")]
        public int targetFPS = 90;
        [Tooltip("The size of the font the FPS is displayed in.")]
        public int fontSize = 32;
        [Tooltip("The position of the FPS text within the headset view.")]
        public Vector3 position = Vector3.zero;
        [Tooltip("The colour of the FPS text when the frames per second are within reasonable limits of the Target FPS.")]
        public Color goodColor = Color.green;
        [Tooltip("The colour of the FPS text when the frames per second are falling short of reasonable limits of the Target FPS.")]
        public Color warnColor = Color.yellow;
        [Tooltip("The colour of the FPS text when the frames per second are at an unreasonable level of the Target FPS.")]
        public Color badColor = Color.red;

        protected const float updateInterval = 0.5f;
        protected int framesCount;
        protected float framesTime;
        protected Canvas canvas;
        protected Text text;
        protected VRTK_SDKManager sdkManager;

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged += LoadedSetupChanged;
            }
            InitCanvas();
        }

        protected virtual void OnDisable()
        {
            if (sdkManager != null && !gameObject.activeSelf)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }
        }

        protected virtual void Update()
        {
            framesCount++;
            framesTime += Time.unscaledDeltaTime;

            if (framesTime > updateInterval)
            {
                if (text != null)
                {
                    if (displayFPS)
                    {
                        float fps = framesCount / framesTime;
                        text.text = string.Format("{0:F2} FPS", fps);
                        text.color = (fps > (targetFPS - 5) ? goodColor :
                                     (fps > (targetFPS - 30) ? warnColor :
                                      badColor));
                    }
                    else
                    {
                        text.text = "";
                    }
                }
                framesCount = 0;
                framesTime = 0;
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            SetCanvasCamera();
        }

        protected virtual void InitCanvas()
        {
            canvas = transform.GetComponentInParent<Canvas>();
            text = GetComponent<Text>();

            if (canvas != null)
            {
                canvas.planeDistance = 0.5f;
            }

            if (text != null)
            {
                text.fontSize = fontSize;
                text.transform.localPosition = position;
            }
            SetCanvasCamera();
        }

        protected virtual void SetCanvasCamera()
        {
            Transform sdkCamera = VRTK_DeviceFinder.HeadsetCamera();
            if (sdkCamera != null)
            {
                canvas.worldCamera = sdkCamera.GetComponent<Camera>();
            }
        }
    }
}