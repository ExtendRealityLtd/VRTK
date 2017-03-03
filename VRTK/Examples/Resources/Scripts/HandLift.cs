namespace VRTK.Examples
{
    using UnityEngine;

    public class HandLift : VRTK_InteractableObject
    {
        [Header("Hand Lift Options", order = 4)]
        public float speed = 0.1f;
        public Transform handleTop;
        public Transform ropeTop;
        public Transform ropeBottom;
        public GameObject rope;
        public GameObject handle;

        private bool isMoving = false;
        private bool isMovingUp = true;

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);

            isMoving = true;
        }

        protected override void Update()
        {
            base.Update();

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

                if((!isMovingUp && handle.transform.position.y <= ropeBottom.position.y) || 
                    (isMovingUp && handle.transform.position.y >= handleTop.position.y))
                {
                    isMoving = false;
                    isMovingUp = !isMovingUp;
                }
            }
        }
    }
}