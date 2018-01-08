// Desktop Camera|Prefabs|0040
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Allows rendering a separate camera that is shown on the desktop only, without changing what's seen in VR headsets.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/DesktopCamera/DesktopCamera` prefab in the scene.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Prefabs/VRTK_DesktopCamera")]
    public class VRTK_DesktopCamera : MonoBehaviour
    {
        [Header("Desktop Camera")]

        [Tooltip("The camera to use for the desktop view. If left blank the camera on the game object this script is attached to or any of its children will be used.")]
        public Camera desktopCamera;
        [Tooltip("The follow script to use for following the headset. If left blank the follow script on the game object this script is attached to or any of its children will be used.")]
        public VRTK_ObjectFollow followScript;

        [Header("Headset Image")]

        [Tooltip("The optional image to render the headset's view into. Can be left blank.")]
        public RawImage headsetImage;
        [Tooltip("The optional render texture to render the headset's view into. Can be left blank. If this is blank and `headsetImage` is set a default render texture will be created.")]
        public RenderTexture headsetRenderTexture;

        protected Camera headsetCameraCopy;
        protected VRTK_TransformFollow headsetCameraTransformFollow;

        protected virtual void OnEnable()
        {
            desktopCamera = desktopCamera == null ? GetComponentInChildren<Camera>() : desktopCamera;
            followScript = followScript == null ? GetComponentInChildren<VRTK_ObjectFollow>() : followScript;

            if (desktopCamera == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_DesktopCamera", "Camera", "desktopCamera", "the same", " or any child of it"));
                return;
            }

            if (followScript == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_DesktopCamera", "VRTK_ObjectFollow", "followScript", "the same", " or any child of it"));
                return;
            }

            headsetCameraTransformFollow = gameObject.AddComponent<VRTK_TransformFollow>();
            headsetCameraTransformFollow.moment = VRTK_TransformFollow.FollowMoment.OnLateUpdate;

            if (VRTK_SDKManager.SubscribeLoadedSetupChanged(LoadedSetupChanged) && VRTK_SDKManager.GetLoadedSDKSetup() != null)
            {
                ConfigureForCurrentSDKSetup();
            }
        }

        protected virtual void OnDisable()
        {
            VRTK_SDKManager.UnsubscribeLoadedSetupChanged(LoadedSetupChanged);
            Destroy(headsetCameraTransformFollow);
            if (headsetCameraCopy != null)
            {
                Destroy(headsetCameraCopy.gameObject);
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            ConfigureForCurrentSDKSetup();
        }

        protected virtual void ConfigureForCurrentSDKSetup()
        {
            if (headsetCameraCopy != null)
            {
                Destroy(headsetCameraCopy.gameObject);
            }

            headsetCameraTransformFollow.enabled = false;
            followScript.enabled = false;

            if (VRTK_SDKManager.GetLoadedSDKSetup() == null)
            {
                return;
            }

            Camera headsetCamera = VRTK_DeviceFinder.HeadsetCamera().GetComponent<Camera>();

            desktopCamera.depth = headsetCamera.depth + 1;
            desktopCamera.stereoTargetEye = StereoTargetEyeMask.None;

            followScript.gameObjectToFollow = headsetCamera.gameObject;
            followScript.gameObjectToChange = desktopCamera.gameObject;
            followScript.Follow();
            followScript.enabled = true;

            if (headsetImage == null)
            {
                return;
            }

            if (headsetRenderTexture == null)
            {
                headsetRenderTexture = new RenderTexture(
                    (int)headsetImage.rectTransform.rect.width,
                    (int)headsetImage.rectTransform.rect.height,
                    24,
                    RenderTextureFormat.ARGB32)
                {
                    name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Headset RenderTexture")
                };
            }

            headsetCameraCopy = Instantiate(headsetCamera, transform);
            headsetCameraCopy.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Headset Camera Copy");
            headsetCameraCopy.targetTexture = headsetRenderTexture;

            foreach (Transform child in headsetCameraCopy.transform)
            {
                Destroy(child.gameObject);
            }

            IEnumerable<Component> componentsToDestroy = headsetCameraCopy
                .GetComponents<Component>()
                .Where(component => component != headsetCameraCopy && !(component is Transform));
            foreach (Component component in componentsToDestroy)
            {
                Destroy(component);
            }

            headsetCameraTransformFollow.gameObjectToFollow = headsetCamera.gameObject;
            headsetCameraTransformFollow.gameObjectToChange = headsetCameraCopy.gameObject;
            headsetCameraTransformFollow.Follow();
            headsetCameraTransformFollow.enabled = true;

            headsetImage.texture = headsetRenderTexture;
            headsetImage.SetNativeSize();
        }
    }
}
