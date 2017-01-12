namespace VRTK
{
    using UnityEngine;

    public class VRTK_TrackedHeadset : MonoBehaviour
    {
        private void Update()
        {
            VRTK_SDK_Bridge.HeadsetProcessUpdate();
        }
    }
}