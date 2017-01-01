// Simulator Headset|SDK_Simulator|002
namespace VRTK
{
#if VRTK_SDK_SIM
    using UnityEngine;

    /// <summary>
    /// The Sim Headset SDK script  provides dummy functions for the headset.
    /// </summary>
    public class SDK_SimHeadset : SDK_BaseHeadset
    {
        private Transform camera;

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
    }
#else
    public class SDK_SimHeadset : SDK_FallbackHeadset
    {
    }
#endif
}