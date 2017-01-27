// Object Transform Follow|Utilities|90060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// A simple script that when attached to a GameObject will follow the position, scale and rotation of the given Transform.
    /// </summary>
    public class VRTK_ObjectFollow : MonoBehaviour
    {
        [Tooltip("A transform of an object to follow the position, scale and rotation of.")]
        public Transform objectToFollow;
        [Tooltip("Follow the position of the given object.")]
        public bool followPosition = true;
        [Tooltip("Follow the rotation of the given object.")]
        public bool followRotation = true;
        [Tooltip("Follow the scale of the given object.")]
        public bool followScale = true;

        protected virtual void Update()
        {
            if (objectToFollow != null)
            {
                if (followScale)
                {
                    transform.localScale = objectToFollow.localScale;
                }

                if (followRotation)
                {
                    transform.rotation = objectToFollow.rotation;
                }

                if (followPosition)
                {
                    transform.position = objectToFollow.position;
                }
            }
            else
            {
                Debug.LogError("No Object To Follow defined!");
            }
        }
    }
}