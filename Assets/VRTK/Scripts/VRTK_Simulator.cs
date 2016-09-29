﻿// Simulating Headset Movement|Scripts|0230
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// To test a scene it is often necessary to use the headset to move to a location. This increases turn-around times and can become cumbersome. The simulator allows navigating through the scene using the keyboard instead, without the need to put on the headset. One can then move around (also through walls) while looking at the monitor and still use the controllers to interact.
    /// </summary>
    /// <remarks>
    /// The Simulator script is attached to the `[CameraRig]` prefab. Supported movements are: forward, backward, strafe left, strafe right, turn left, turn right, up, down.
    /// </remarks>
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

        [Tooltip("Per default the keys on the left-hand side of the keyboard are used (WASD). They can be individually set as needed. The reset key brings the camera to its initial location.")]
        public Keys keys;
        [Tooltip("Typically the simulator should be turned off when not testing anymore. This option will do this automatically when outside the editor.")]
        public bool onlyInEditor = true;
        [Tooltip("Depending on the scale of the world the step size can be defined to increase or decrease movement speed.")]
        public float stepSize = 0.05f;
        [Tooltip("An optional game object marking the position and rotation at which the camera should be initially placed.")]
        public Transform camStart;

        private Transform cam;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Start()
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

        private void Update()
        {
            Vector3 movDir = Vector3.zero;
            Vector3 rotDir = Vector3.zero;

            if (Input.GetKey(keys.forward))
            {
                movDir = overwriteY(cam.forward, 0);
            }
            else if (Input.GetKey(keys.backward))
            {
                movDir = overwriteY(-cam.forward, 0);
            }
            else if (Input.GetKey(keys.strafeLeft))
            {
                movDir = overwriteY(-cam.right, 0);
            }
            else if (Input.GetKey(keys.strafeRight))
            {
                movDir = overwriteY(cam.right, 0);
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
                rotDir = new Vector3(0, -1, 0);
            }
            else if (Input.GetKey(keys.right))
            {
                rotDir = new Vector3(0, 1, 0);
            }
            else if (Input.GetKey(keys.reset))
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
            transform.Translate(movDir * stepSize, Space.World);
            transform.Rotate(rotDir);
        }

        private Vector3 overwriteY(Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }
    }
}