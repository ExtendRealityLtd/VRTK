namespace VRTK
{
    using UnityEngine;

    public class VRTK_Simulator : MonoBehaviour
    {
        [System.Serializable]
        public class Keys
        {
            public KeyCode forward = KeyCode.W;
            public KeyCode backward = KeyCode.S;
            public KeyCode strafeLeft = KeyCode.A;
            public KeyCode strafeRight = KeyCode.D;
            public KeyCode left = KeyCode.Q;
            public KeyCode right = KeyCode.E;
            public KeyCode up = KeyCode.Y;
            public KeyCode down = KeyCode.C;
            public KeyCode reset = KeyCode.X;
        }
        public Keys keys;
        [Tooltip("Will deactivate the script if run in a build outside the editor.")]
        public bool onlyInEditor = true;
        public float stepSize = 0.05f;
        [Tooltip("An optional game object marking the position and rotation at which the camera should be initially placed.")]
        public Transform camStart;

        private Transform cam;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        void Start()
        {
            // don't run in builds outside the editor
            if (onlyInEditor && !Application.isEditor)
            {
                enabled = false;
                return;
            }

            cam = GetComponentInChildren<Camera>().transform;
            if (!cam)
            {
                Debug.LogWarning("Could not find camera. Simulator deactivated.");
                enabled = false;
                return;
            }

            if (camStart && camStart.gameObject.activeInHierarchy)
            {
                transform.position = camStart.position;
                transform.rotation = camStart.rotation;
            }

            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        void Update()
        {
            Vector3 movDir = Vector3.zero;
            Vector3 rotDir = Vector3.zero;

            if (Input.GetKey(keys.forward))
            {
                movDir = overwriteY(cam.forward, 0);
            }
            else if (Input.GetKey(keys.backward))
            {
                movDir = overwriteY(cam.forward, 0) * -1;
            }
            else if (Input.GetKey(keys.strafeLeft))
            {
                movDir = overwriteY(cam.right, 0);
            }
            else if (Input.GetKey(keys.strafeRight))
            {
                movDir = overwriteY(cam.right, 0) * -1;
            }
            else if (Input.GetKey(keys.up))
            {
                movDir = new Vector3(0, 1, 0);
            }
            else if (Input.GetKey(keys.down))
            {
                movDir = new Vector3(0, -1, 0);
            }
            else if (Input.GetKey(keys.left))
            {
                rotDir = new Vector3(0, 1, 0);
            }
            else if (Input.GetKey(keys.right))
            {
                rotDir = new Vector3(0, -1, 0);
            }
            else if (Input.GetKey(keys.reset))
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
            transform.Translate(movDir * stepSize);
            transform.Rotate(rotDir);
        }

        private Vector3 overwriteY(Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }
    }
}