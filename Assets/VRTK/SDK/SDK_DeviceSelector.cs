using UnityEngine;
using UnityEngine.VR;

namespace VRTK
{
    /// <summary>
    /// Activate only the node with the active device name (OpenVR, Oculus, GearVR, [default]).
    /// </summary>
    /// <remarks>
    /// If running on an Android device with GearVR use "GearVR" instead of "Oculus".
    /// </remarks>
    public class SDK_DeviceSelector : MonoBehaviour
    {
#if (UNITY_5_4_OR_NEWER)
        [SerializeField]
        [Tooltip("Name of the child object selected when no VR device is loaded")]
        string defaultChildObjectName = "[default]";
#if UNITY_EDITOR
        [Header("Editor only")]
        [SerializeField]
        [Tooltip("Force a device for debugging (only for Unity Editor)")]
        bool forceDevice = false;
        [SerializeField]
        [Tooltip("Name of the device forced for debugging (only for Unity Editor)")]
        string forcedDeviceName;
#endif
#else
        [Header("Warning! Automatic switching available only with Unity 5.4 and above")]
        [Tooltip("Type here the name of the child object to be enabled (the other children will be disabled)")]
        [SerializeField]
        string deviceChildObjectName;
#endif

        // Use this for initialization
        void OnEnable()
        {
#if (UNITY_5_4_OR_NEWER)
            string device = VRSettings.loadedDeviceName;
#if UNITY_EDITOR
            if(forceDevice) device = forcedDeviceName;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            if (device == "Oculus") device = "GearVR";
#endif
            if (device.Length == 0) device = defaultChildObjectName;
#else
            string device = deviceChildObjectName;
#endif
            Transform activeChild = transform.Find(device);
            if (activeChild == null) return;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                child.SetActive(child.name == device);
            }
        }
    }
}