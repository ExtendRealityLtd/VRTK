namespace VRTK
{
    using UnityEngine;

    public abstract class SDK_InterfaceBoundaries : ScriptableObject
    {
        public abstract Transform GetPlayArea();
        public abstract Vector3[] GetPlayAreaVertices(GameObject playArea);
        public abstract float GetPlayAreaBorderThickness(GameObject playArea);
        public abstract bool IsPlayAreaSizeCalibrated(GameObject playArea);
    }
}