using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    ///     The PlayStation Move Controller SDK script provides functions to map the move controllers buttons and get the
    ///     controllers position.
    /// </summary>
    public partial class SDK_PlayStationMoveController : MonoBehaviour
    {
        #region Controller enum

        public enum Controller
        {
            Primary,
            Secondary
        }

        #endregion

        #region PlayStationKeys enum

        public enum PlayStationKeys
        {
            Trigger = 330 + 4,
            Middle = 330 + 5,
            Start = 330 + 7,
            Triangle = 330 + 3,
            Circle = 330 + 1,
            Cross = 330 + 0,
            Square = 330 + 2
        }

        #endregion

        [HideInInspector] public bool ActiveController = true;
        public Controller ControllerType;
        [HideInInspector] public bool IsTracked;
        private Vector3 lastPos;
        private Vector3 lastRot;
        private List<Vector3> posList;
        private List<Vector3> rotList;
        private Transform velocityTracker;

        private Dictionary<PlayStationKeys, int> keyMappings = new Dictionary<PlayStationKeys, int>
        {
            {PlayStationKeys.Trigger, (int) PlayStationKeys.Trigger},
            {PlayStationKeys.Middle, (int) PlayStationKeys.Middle},
            {PlayStationKeys.Start, (int) PlayStationKeys.Start},
            {PlayStationKeys.Triangle, (int) PlayStationKeys.Triangle},
            {PlayStationKeys.Circle, (int) PlayStationKeys.Circle},
            {PlayStationKeys.Cross, (int) PlayStationKeys.Cross},
            {PlayStationKeys.Square, (int) PlayStationKeys.Square}
        };

   

        public Vector3 GetVelocity()
        {
            CreateVelocityTracker();
            if (velocityTracker == null)
            {
                velocityTracker = transform.GetComponentInChildren<VRTK_InteractGrab>().controllerAttachPoint.transform;
            }

            Vector3 velocity = Vector3.zero;
            foreach (Vector3 vel in posList)
            {
                velocity += vel;
            }
            velocity /= posList.Count;

            return velocityTracker.InverseTransformDirection(velocity * (velocity.magnitude / 3));
        }

        public Vector3 GetAngularVelocity()
        {
            CreateVelocityTracker();
            if (velocityTracker == null)
            {
                velocityTracker = transform.GetComponentInChildren<VRTK_InteractGrab>().controllerAttachPoint.transform;
            }
            Vector3 angularVelocity = Vector3.zero;
            foreach (Vector3 vel in rotList)
            {
                angularVelocity += vel;
            }
            angularVelocity /= rotList.Count;
            angularVelocity *= .0001f;


            return velocityTracker.InverseTransformDirection(angularVelocity);
        }


        /// <summary>
        ///     Set the key mapping to be different than the default
        /// </summary>
        /// <param name="mapping"> new key mapping</param>
        public void SetKeyMappings(Dictionary<PlayStationKeys, KeyCode> mapping)
        {
            keyMappings = new Dictionary<PlayStationKeys, int>();
            foreach (KeyValuePair<PlayStationKeys, KeyCode> inputKey in mapping)
            {
                if (!keyMappings.ContainsKey(inputKey.Key))
                {
                    keyMappings.Add(inputKey.Key, (int) inputKey.Value);
                }
            }
        }

        /// <summary>
        ///     change a specific key to a different input code
        /// </summary>
        /// <param name="key"> PlayStation Key to change</param>
        /// <param name="inputKey"> input to replace it with </param>
        public void SetKey(PlayStationKeys key, KeyCode inputKey)
        {
            keyMappings[key] = (int) inputKey;
        }

        /// <summary>
        ///     Adds the stick id to the playstation key and returns a UnityEngine KeyCode
        /// </summary>
        /// <param name="key"> The key Playstation key that you want returned for this controller </param>
        /// <returns></returns>
        public KeyCode GetControllerKey(PlayStationKeys key)
        {
            return (KeyCode) keyMappings[key] + GetStickId();
        }

        /// <summary>
        ///     Get the Stick KeyCode ID for this controller. Add this to the PlayStationKey to get the UnityEngine KeyCode
        /// </summary>
        /// <returns> Controller KeyCode</returns>
        public int GetStickId()
        {
#if UNITY_EDITOR
            return 0;
#endif
            switch (ControllerType)
            {
                case Controller.Primary:
                    return 5 * 20;
                case Controller.Secondary:
                    return 6 * 20;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateVelocityTracker()
        {
            if (velocityTracker == null)
            {
                return;
            }
            VRTK_InteractGrab grabController = transform.GetComponentInChildren<VRTK_InteractGrab>();
            velocityTracker = new GameObject("Velocity Tracker").transform;
            velocityTracker.parent = grabController.transform;
            velocityTracker.localEulerAngles = Vector3.zero;
            velocityTracker.position = grabController.controllerAttachPoint.position;
        }

        private void Awake()
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;

            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
#if UNITY_PS4
            UpdateMoveTransforms();
#endif
            posList.Add((transform.position - lastPos) / Time.deltaTime);
            if (posList.Count > 10)
            {
                posList.RemoveAt(0);
            }
            rotList.Add(Quaternion.FromToRotation(lastRot, transform.rotation.eulerAngles).eulerAngles
                        / Time.deltaTime);
            if (rotList.Count > 10)
            {
                rotList.RemoveAt(0);
            }
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
        }
    }
}