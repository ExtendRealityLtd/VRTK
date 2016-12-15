namespace VRTK
{
    using UnityEngine;

    public interface SDK_InterfaceBoundaries
    {
        Transform GetPlayArea();
        Vector3[] GetPlayAreaVertices(GameObject playArea);
        float GetPlayAreaBorderThickness(GameObject playArea);
        bool IsPlayAreaSizeCalibrated(GameObject playArea);
    }
}