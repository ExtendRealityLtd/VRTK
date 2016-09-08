namespace VRTK.Examples.Utilities
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class Add_HeadsetCollisionFade : MonoBehaviour
    {
        private bool initalised = false;
#if UNITY_EDITOR
        private void Update()
        {
            if (!initalised)
            {
                var headset = VRTK_DeviceFinder.HeadsetTransform();
                if (!headset.GetComponent<VRTK_HeadsetCollisionFade>())
                {
                    headset.gameObject.AddComponent<VRTK_HeadsetCollisionFade>();
                }
                initalised = true;
            }
        }
#endif
    }
}