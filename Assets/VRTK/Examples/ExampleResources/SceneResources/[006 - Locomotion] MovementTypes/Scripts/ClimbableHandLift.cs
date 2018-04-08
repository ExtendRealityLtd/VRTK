namespace VRTK.Examples
{
    using UnityEngine;

    public class ClimbableHandLift : MonoBehaviour
    {
        public VRTK_InteractableObject interactableObject;
        public float speed = 0.1f;
        public Transform handleTop;
        public Transform ropeTop;
        public Transform ropeBottom;
        public GameObject rope;
        public GameObject handle;

        public bool isMoving = false;
        protected bool isMovingUp = true;

        protected virtual void OnEnable()
        {
            interactableObject = (interactableObject == null ? GetComponent<VRTK_InteractableObject>() : interactableObject);
            if (interactableObject != null)
            {
                interactableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
            }
        }

        protected virtual void OnDisable()
        {
            if (interactableObject != null)
            {
                interactableObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
            }
        }

        protected virtual void Update()
        {
            if (isMoving)
            {
                Vector3 movePosition = (isMovingUp ? Vector3.up : Vector3.down) * speed * Time.deltaTime;

                handle.transform.position += movePosition;

                Vector3 scale = rope.transform.localScale;
                scale.y = (ropeTop.position.y - handle.transform.position.y) / 2.0f;

                Vector3 midpoint = ropeTop.transform.position;
                midpoint.y -= scale.y;

                rope.transform.localScale = scale;
                rope.transform.position = midpoint;

                if ((!isMovingUp && handle.transform.position.y <= ropeBottom.position.y) ||
                    (isMovingUp && handle.transform.position.y >= handleTop.position.y))
                {
                    isMoving = false;
                    isMovingUp = !isMovingUp;
                }
            }
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            isMoving = true;
        }
    }
}