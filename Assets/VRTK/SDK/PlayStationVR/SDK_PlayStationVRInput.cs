// VR Simulator|Prefabs|0005

using System;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    ///     The `PlayStationVRCameraRig` prefab is a Camera Rig set up for VRTK to allow the use of PlayStationVR and
    ///     PlayStation Move input inside your game .
    /// </summary>
    public partial class SDK_PlayStationVRInput : MonoBehaviour
    {
        #region Private fields

        private Transform rightHand;
        private Transform leftHand;
        private Transform myCamera;
        private Transform trackedDevices;
        private SDK_PlayStationMoveController rightController;
        private SDK_PlayStationMoveController leftController;
        private static GameObject cachedCameraRig;
        private GameObject hintCanvas;
        #endregion

        /// <summary>
        ///     The FindInScene method is used to find the `PlayStationVRCameraRig` GameObject within the current scene.
        /// </summary>
        /// <returns>
        ///     Returns the found `PlayStationVRCameraRig` GameObject if it is found. If it is not found then it prints a debug
        ///     log error.
        /// </returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null)
            {
                cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_PlayStationVRInput>();
                if (!cachedCameraRig)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(
                                          VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE,
                                          "PlayStationVRCameraRig", "SDK_PlayStationVR",
                                          ". check that the `VRTK/PlayStationVR/PlayStationVRCameraRig` prefab has been added to the scene."));
                }
            }
            return cachedCameraRig;
        }

        private void Start()
        {
            SDK_PlayStationMoveController[] controllers = FindInScene()
                .GetComponentsInChildren<SDK_PlayStationMoveController>(true);
            myCamera = transform.GetComponentInChildren<Camera>().transform;
            foreach (SDK_PlayStationMoveController controller in controllers)
            {
                switch (controller.ControllerType)
                {
                    case SDK_PlayStationMoveController.Controller.Primary:
                        rightHand = controller.transform;
                        rightController = controller;
                        break;
                    case SDK_PlayStationMoveController.Controller.Secondary:
                        leftHand = controller.transform;
                        leftController = controller;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            trackedDevices = rightHand.parent;
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
#if UNITY_EDITOR
            SetUpEditorSimulator();
#endif
#if UNITY_PS4
            InitDeviceTracking();
#endif
        }



        private void Update()
        {
#if UNITY_EDITOR
            UpdateSimulator();
#endif
        }



    }
}