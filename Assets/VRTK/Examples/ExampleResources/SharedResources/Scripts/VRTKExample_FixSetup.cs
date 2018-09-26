#if UNITY_EDITOR
namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEditor;

    [ExecuteInEditMode]
    public class VRTKExample_FixSetup : MonoBehaviour
    {
        public bool forceOculusFloorLevel = true;
        protected bool trackingLevelFloor = false;

        public virtual void ApplyFixes()
        {
            FixOculus();
        }

        protected virtual void Awake()
        {

            if (Application.isEditor && !Application.isPlaying)
            {
                ApplyFixes();
            }
        }

        protected virtual void Update()
        {
            FixTrackingType();
        }

        protected virtual void FixTrackingType()
        {
#if VRTK_DEFINE_SDK_OCULUS
            if (forceOculusFloorLevel && !trackingLevelFloor)
            {
                GameObject overManagerGO = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/Oculus/OVRCameraRig");
                if (overManagerGO != null)
                {
                    OVRManager ovrManager = overManagerGO.GetComponent<OVRManager>();
                    if (ovrManager != null)
                    {
                        ovrManager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                        trackingLevelFloor = true;
                        Debug.Log("Forced Oculus Tracking to Floor Level");
                    }
                }
            }
#endif
        }

        protected virtual void FixOculus()
        {
#if VRTK_DEFINE_SDK_OCULUS
            string oculusPath = "[VRTK_SDKManager]/[VRTK_SDKSetups]/Oculus";
            GameObject oculusSDK = GameObject.Find(oculusPath);

            if (oculusSDK == null || oculusSDK.GetComponentInChildren<OVRManager>() != null)
            {
                Debug.Log("No Oculus Repaired Required");
                return;
            }

            GameObject currentRig = GameObject.Find(oculusPath + "/OVRCameraRig");
            GameObject currentAvatar = GameObject.Find(oculusPath + "/LocalAvatar");
            VRTK_SDKSetup oculusSetup = oculusSDK.GetComponent<VRTK_SDKSetup>();

            if (currentRig != null)
            {
                DestroyImmediate(currentRig);
            }
            if (currentAvatar != null)
            {
                DestroyImmediate(currentAvatar);
            }

            GameObject ovrCameraRig = null;
            GameObject ovrCameraRigToInstantiate = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Oculus/VR/Prefabs/OVRCameraRig.prefab", typeof(GameObject));
            if (ovrCameraRigToInstantiate != null)
            {
                ovrCameraRig = Instantiate(ovrCameraRigToInstantiate) as GameObject;
            }
            if (ovrCameraRig != null)
            {
                ovrCameraRig.name = ovrCameraRig.name.Replace("(Clone)", "");
                ovrCameraRig.transform.SetParent(oculusSDK.transform);
                ovrCameraRig.SetActive(true);
                oculusSetup.actualBoundaries = ovrCameraRig;
                oculusSetup.actualHeadset = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
                oculusSetup.actualLeftController = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/LeftHandAnchor");
                oculusSetup.actualRightController = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/RightHandAnchor");
                Debug.Log("Successfully repaired Oculus OVRCameraRig prefab");
            }

            GameObject ovrAvatar = null;
            GameObject ovrAvatarToInstantiate = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Oculus/Avatar/Content/Prefabs/LocalAvatar.prefab", typeof(GameObject));
            if (ovrAvatarToInstantiate != null)
            {
                ovrAvatar = Instantiate(ovrAvatarToInstantiate) as GameObject;
            }

            if (ovrAvatar == null)
            {
                //legacy location
                ovrAvatarToInstantiate = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/OvrAvatar/Content/Prefabs/LocalAvatar.prefab", typeof(GameObject));
                if (ovrAvatarToInstantiate != null)
                {
                    ovrAvatar = Instantiate(ovrAvatarToInstantiate) as GameObject;
                }
            }
            if (ovrAvatar != null)
            {
                OvrAvatar avatarScript = ovrAvatar.GetComponent<OvrAvatar>();
                avatarScript.StartWithControllers = true;
                ovrAvatar.name = ovrAvatar.name.Replace("(Clone)", "");
                ovrAvatar.transform.SetParent(oculusSDK.transform);
                ovrAvatar.SetActive(true);
                oculusSetup.modelAliasLeftController = GameObject.Find(oculusPath + "/LocalAvatar/controller_left");
                oculusSetup.modelAliasRightController = GameObject.Find(oculusPath + "/LocalAvatar/controller_right");
                GameObject.Find(oculusPath + "/LocalAvatar/hand_left").SetActive(false);
                GameObject.Find(oculusPath + "/LocalAvatar/hand_right").SetActive(false);
                VRTK_TransformFollow transformFollow = ovrAvatar.AddComponent<VRTK_TransformFollow>();
                transformFollow.gameObjectToFollow = ovrCameraRig;
                Debug.Log("Successfully repaired Oculus LocalAvatar prefab");
            }
#endif
        }
    }
}
#endif