namespace VRTK
{
    using UnityEngine;

    public interface SDK_InterfaceHeadset
    {
        Transform GetHeadset();
        Transform GetHeadsetCamera();
        void HeadsetFade(Color color, float duration, bool fadeOverlay = false);
        bool HasHeadsetFade(Transform obj);
        void AddHeadsetFade(Transform camera);
    }
}