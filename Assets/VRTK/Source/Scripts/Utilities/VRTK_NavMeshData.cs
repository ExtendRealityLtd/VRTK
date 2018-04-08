// Nav Mesh Data|Utilities|90090
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Nav Mesh Data script allows custom nav mesh information to be provided to the teleporter script.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_NavMeshData")]
    public class VRTK_NavMeshData : MonoBehaviour
    {
        [Tooltip("The max distance given point can be outside the nav mesh to be considered valid.")]
        public float distanceLimit = 0.1f;
        [Tooltip("The parts of the navmesh that are considered valid")]
        public int validAreas = -1;
    }
}