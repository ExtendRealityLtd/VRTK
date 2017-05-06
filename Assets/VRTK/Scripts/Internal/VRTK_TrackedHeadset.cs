namespace VRTK
{
    using UnityEngine;

    public class VRTK_TrackedHeadset : MonoBehaviour
    {
        protected virtual void FixedUpdate()
        {
            VRTK_SDK_Bridge.HeadsetProcessFixedUpdate();
        }

        protected virtual void Update()
        {
            VRTK_SDK_Bridge.HeadsetProcessUpdate();
        }
    }
}