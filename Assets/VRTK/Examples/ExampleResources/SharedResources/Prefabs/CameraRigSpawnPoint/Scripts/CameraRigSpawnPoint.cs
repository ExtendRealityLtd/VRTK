namespace VRTK.Examples
{
    using UnityEngine;

    public class CameraRigSpawnPoint : MonoBehaviour
    {
        public GameObject boundary;
        public GameObject player;

        protected virtual void Awake()
        {
            Destroy(boundary);
            Destroy(player);
        }
    }
}