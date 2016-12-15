// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The SDK Manager script provides configuration of supported SDKs
    /// </summary>
    [ExecuteInEditMode]
    public class VRTK_SDKManager : MonoBehaviour
    {
        /// <summary>
        /// The supported SDKs
        /// </summary>
        public enum SupportedSDKs
        {
            None,
            SteamVR
        }

        /// <summary>
        /// The singleton instance to access the SDK Manager variables from.
        /// </summary>
        public static VRTK_SDKManager instance = null;

        [Header("SDK Selection")]

        [Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
        public bool persistOnLoad = false;

        [Tooltip("The SDK to use to deal with all system actions.")]
        public SupportedSDKs systemSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise room scale boundaries.")]
        public SupportedSDKs boundariesSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the VR headset.")]
        public SupportedSDKs headsetSDK = SupportedSDKs.None;
        [Tooltip("The SDK to use to utilise the input devices.")]
        public SupportedSDKs controllerSDK = SupportedSDKs.None;

        [Header("Linked Objects")]

        [Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
        public GameObject actualBoundaries;
        [Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
        public GameObject actualHeadset;
        [Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
        public GameObject actualLeftController;
        [Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
        public GameObject actualRightController;

        [Header("Controller Aliases")]

        [Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
        public GameObject modelAliasLeftController;
        [Tooltip("A reference to the GameObject that models for the Right Hand Controller")]
        public GameObject modelAliasRightController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
        public GameObject scriptAliasLeftController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
        public GameObject scriptAliasRightController;

        private void Awake()
        {
            CreateInstance();
            if (!VRTK_SharedMethods.IsEditTime())
            {
                SetupControllers();
            }
        }

        private void SetupControllers()
        {
            if (!actualLeftController.GetComponent<VRTK_TrackedController>())
            {
                actualLeftController.AddComponent<VRTK_TrackedController>();
            }

            if (!actualRightController.GetComponent<VRTK_TrackedController>())
            {
                actualRightController.AddComponent<VRTK_TrackedController>();
            }

            if (scriptAliasLeftController && !scriptAliasLeftController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasLeftController.AddComponent<VRTK_ControllerTracker>();
            }

            if (scriptAliasRightController && !scriptAliasRightController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasRightController.AddComponent<VRTK_ControllerTracker>();
            }
        }

        private void CreateInstance()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (persistOnLoad && !VRTK_SharedMethods.IsEditTime())
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}