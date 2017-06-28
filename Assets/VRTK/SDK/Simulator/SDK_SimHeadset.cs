// Simulator Headset|SDK_Simulator|002
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Sim Headset SDK script  provides dummy functions for the headset.
    /// </summary>
    [SDK_Description(typeof(SDK_SimSystem))]
    public class SDK_SimHeadset : SDK_BaseHeadset
    {
        private Transform camera;
        private Vector3 lastPos = new Vector3();
        private Quaternion lastRot = new Quaternion();
        private List<Vector3> posList = new List<Vector3>();
        private List<Vector3> rotList = new List<Vector3>();
        private float magnitude;
        private Vector3 axis;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            if (camera != null)
            {
                posList.Add((camera.position - lastPos) / Time.deltaTime);
                if (posList.Count > 4)
                {
                    posList.RemoveAt(0);
                }
                Quaternion deltaRotation = camera.rotation * Quaternion.Inverse(lastRot);
                deltaRotation.ToAngleAxis(out magnitude, out axis);
                rotList.Add((axis * magnitude));
                if (rotList.Count > 4)
                {
                    rotList.RemoveAt(0);
                }
                lastPos = camera.position;
                lastRot = camera.rotation;
            }
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public override Transform GetHeadset()
        {
            if (camera == null)
            {
                GameObject simPlayer = SDK_InputSimulator.FindInScene();
                if (simPlayer)
                {
                    camera = simPlayer.transform.Find("Neck/Camera");
                }
            }

            return camera;
        }

        /// <summary>
        /// The GetHeadsetCamera/0 method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public override Transform GetHeadsetCamera()
        {
            return GetHeadset();
        }

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public override Vector3 GetHeadsetVelocity()
        {
            Vector3 velocity = Vector3.zero;
            foreach (Vector3 vel in posList)
            {
                velocity += vel;
            }
            velocity /= posList.Count;
            return velocity;
        }

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public override Vector3 GetHeadsetAngularVelocity()
        {
            Vector3 angularVelocity = Vector3.zero;
            foreach (Vector3 vel in rotList)
            {
                angularVelocity += vel;
            }
            angularVelocity /= rotList.Count;
            return angularVelocity;
        }

        /// <summary>
        /// The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.
        /// </summary>
        /// <param name="color">The colour to fade to.</param>
        /// <param name="duration">The amount of time the fade should take to reach the given colour.</param>
        /// <param name="fadeOverlay">Determines whether to use an overlay on the fade.</param>
        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            VRTK_ScreenFade.Start(color, duration);
        }

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public override bool HasHeadsetFade(Transform obj)
        {
            return obj.GetComponentInChildren<VRTK_ScreenFade>() != null;
        }

        /// <summary>
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public override void AddHeadsetFade(Transform camera)
        {
            if (camera != null && camera.GetComponent<VRTK_ScreenFade>() == null)
            {
                camera.gameObject.AddComponent<VRTK_ScreenFade>();
            }
        }

        private void Awake()
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();

            var headset = GetHeadset();
            if (headset != null)
            {
                lastPos = headset.position;
                lastRot = headset.rotation;
            }
        }
    }
}