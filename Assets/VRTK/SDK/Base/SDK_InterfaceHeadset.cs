namespace VRTK
{
    using UnityEngine;

    public abstract class SDK_InterfaceHeadset : ScriptableObject
    {
        public abstract Transform GetHeadset();
        public abstract Transform GetHeadsetCamera();
        public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);
        public abstract bool HasHeadsetFade(Transform obj);
        public abstract void AddHeadsetFade(Transform camera);
    }
}