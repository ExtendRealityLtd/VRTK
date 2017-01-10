// Simulator Headset|SDK_Simulator|002
namespace VRTK
{
#if VRTK_SDK_SIM
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Sim Headset SDK script  provides dummy functions for the headset.
    /// </summary>
    public class SDK_SimHeadset : SDK_BaseHeadset
    {
        private Transform camera;
        private Vector3 lastPos;
        private Vector3 lastRot;
        private List<Vector3> posList;
        private List<Vector3> rotList;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            posList.Add((camera.position - lastPos) / Time.deltaTime);
            if (posList.Count > 10)
            {
                posList.RemoveAt(0);
            }
            rotList.Add((Quaternion.FromToRotation(lastRot, camera.rotation.eulerAngles)).eulerAngles / Time.deltaTime);
            if (rotList.Count > 10)
            {
                rotList.RemoveAt(0);
            }
            lastPos = camera.position;
            lastRot = camera.rotation.eulerAngles;
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
                    camera = simPlayer.transform.FindChild("Camera");
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
            if (camera == null)
            {
                GameObject simPlayer = SDK_InputSimulator.FindInScene();
                if (simPlayer)
                {
                    camera = simPlayer.transform.FindChild("Camera");
                }
            }

            return camera;
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

        }

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public override bool HasHeadsetFade(Transform obj)
        {
            return false;
        }

        /// <summary>
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public override void AddHeadsetFade(Transform camera)
        {

        }

        private void Awake()
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();

            var headset = GetHeadset();
            lastPos = headset.position;
            lastRot = headset.rotation.eulerAngles;
        }
    }
#else
    public class SDK_SimHeadset : SDK_FallbackHeadset
    {
    }
#endif
}