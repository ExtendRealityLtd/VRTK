namespace VRTK.Examples
{
    using UnityEngine;

    public class Zipline : VRTK_InteractableObject
    {
        [Header("Zipline Options", order = 4)]
        public float downStartSpeed = 0.2f;
        public float acceleration = 1.0f;
        public float upSpeed = 1.0f;
        public Transform handleEndPosition;
        public Transform handleStartPosition;
        public GameObject handle;

        private bool isMoving = false;
        private bool isMovingDown = true;

        private float currentSpeed;

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);
            isMoving = true;
        }

        protected override void Awake()
        {
            base.Awake();
            currentSpeed = downStartSpeed;
        }

        protected override void Update()
        {
            base.Update();

            if (isMoving)
            {
                Vector3 moveAmount;
                
                if(isMovingDown)
                {
                    currentSpeed += acceleration * Time.deltaTime;
                    moveAmount = Vector3.down * currentSpeed * Time.deltaTime;
                }
                else
                {
                    moveAmount = Vector3.up * upSpeed * Time.deltaTime;
                }

                handle.transform.localPosition += moveAmount;

                if((isMovingDown && handle.transform.localPosition.y <= handleEndPosition.localPosition.y) || 
                    (!isMovingDown && handle.transform.localPosition.y >= handleStartPosition.localPosition.y))
                {
                    isMoving = false;
                    isMovingDown = !isMovingDown;
                    currentSpeed = downStartSpeed;
                }
            }
        }
    }
}