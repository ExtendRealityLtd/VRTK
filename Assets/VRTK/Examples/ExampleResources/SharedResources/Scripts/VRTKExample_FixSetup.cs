#if UNITY_EDITOR
namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEditor;

    [ExecuteInEditMode]
    public class VRTKExample_FixSetup : MonoBehaviour
    {
        public virtual void ApplyFixes()
        {
            FixOculus();
        }

        protected virtual void Awake()
        {
            ApplyFixes();
        }

        protected virtual void FixOculus()
        {
#if VRTK_DEFINE_SDK_OCULUS
            string oculusPath = "[VRTK_SDKManager]/[VRTK_SDKSetups]/Oculus";
            GameObject oculusSDK = GameObject.Find(oculusPath);
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

            GameObject ovrCameraRig = PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Oculus/VR/Prefabs/OVRCameraRig.prefab", typeof(GameObject))) as GameObject;
            if (ovrCameraRig != null)
            {
                ovrCameraRig.transform.SetParent(oculusSDK.transform);
                ovrCameraRig.SetActive(false);
                oculusSetup.actualBoundaries = ovrCameraRig;
                oculusSetup.actualHeadset = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
                oculusSetup.actualLeftController = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/LeftHandAnchor");
                oculusSetup.actualRightController = GameObject.Find(oculusPath + "/OVRCameraRig/TrackingSpace/RightHandAnchor");
                OVRManager ovrManager = ovrCameraRig.GetComponent<OVRManager>();
                ovrManager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                Debug.Log("Successfully repaired Oculus OVRCameraRig prefab");
            }

            GameObject ovrAvatar = PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Oculus/Avatar/Content/Prefabs/LocalAvatar.prefab", typeof(GameObject))) as GameObject;
            if (ovrAvatar == null)
            {
                //legacy location
                ovrAvatar = PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath("Assets/OvrAvatar/Content/Prefabs/LocalAvatar.prefab", typeof(GameObject))) as GameObject;
            }
            if (ovrAvatar != null)
            {
                ovrAvatar.transform.SetParent(oculusSDK.transform);
                ovrAvatar.SetActive(false);
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