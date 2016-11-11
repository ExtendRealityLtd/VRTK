

namespace VRTK
{
    using UnityEngine;

    public class SDK_GearVR : SDK_Default
    {
        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            // to do: implement fade effect
        }

        public override bool HasHeadsetFade(GameObject obj)
        {
            // to do: implement fade effect
            return false;
        }

        public override void AddHeadsetFade(Transform camera)
        {
            // to do: implement fade effect
        }
    }
}