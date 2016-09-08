namespace VRTK.Examples
{
    using UnityEngine;

    public class RC_Car : MonoBehaviour
    {
        public float maxAcceleration = 3f;
        public float jumpPower = 10f;

        private float acceleration = 0.05f;
        private float movementSpeed = 0f;
        private float rotationSpeed = 180f;
        private bool isJumping = false;
        private Vector2 touchAxis;
        private float triggerAxis;
        private Rigidbody rb;
        private Vector3 defaultPosition;
        private Quaternion defaultRotation;

        public void SetTouchAxis(Vector2 data)
        {
            touchAxis = data;
        }

        public void SetTriggerAxis(float data)
        {
            triggerAxis = data;
        }

        public void Reset()
        {
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            if (isJumping)
            {
                touchAxis.x = 0f;
            }
            CalculateSpeed();
            Move();
            Turn();
            Jump();
        }

        private void CalculateSpeed()
        {
            if (touchAxis.y != 0f)
            {
                movementSpeed += (acceleration * touchAxis.y);
                movementSpeed = Mathf.Clamp(movementSpeed, -maxAcceleration, maxAcceleration);
            }
            else
            {
                Decelerate();
            }
        }

        private void Decelerate()
        {
            if (movementSpeed > 0)
            {
                movementSpeed -= Mathf.Lerp(acceleration, maxAcceleration, 0f);
            }
            else if (movementSpeed < 0)
            {
                movementSpeed += Mathf.Lerp(acceleration, -maxAcceleration, 0f);
            }
            else
            {
                movementSpeed = 0;
            }
        }

        private void Move()
        {
            Vector3 movement = transform.forward * movementSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }

        private void Turn()
        {
            float turn = touchAxis.x * rotationSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        private void Jump()
        {
            if (!isJumping && triggerAxis > 0)
            {
                float jumpHeight = (triggerAxis * jumpPower);
                rb.AddRelativeForce(Vector3.up * jumpHeight);
                triggerAxis = 0f;
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            isJumping = false;
        }

        private void OnTriggerExit(Collider collider)
        {
            isJumping = true;
        }
    }
}