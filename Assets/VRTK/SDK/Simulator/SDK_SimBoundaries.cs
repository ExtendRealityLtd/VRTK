// Simulator Boundaries|SDK_Simulator|004
namespace VRTK
{
#if VRTK_SDK_SIM
    using UnityEngine;

    /// <summary>
    /// The Sim Boundaries SDK script provides dummy functions for the play area bounderies.
    /// </summary>
    public class SDK_SimBoundaries : SDK_BaseBoundaries
    {
        private Transform area;

        /// <summary>
        /// The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.
        /// </summary>
        public override void InitBoundaries()
        {
        }

        /// <summary>
        /// The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the play area in the scene.</returns>
        public override Transform GetPlayArea()
        {
            if (area == null)
            {
                GameObject simPlayer = SDK_InputSimulator.FindInScene();
                if (simPlayer)
                {
                    area = simPlayer.transform;
                }
            }

            return area;
        }

        /// <summary>
        /// The GetPlayAreaVertices method returns the points of the play area boundaries.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            if (area)
            {
                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(1, 0, 1);
                vertices[1] = new Vector3(-1, 0, 1);
                vertices[2] = new Vector3(1, 0, -1);
                vertices[3] = new Vector3(-1, 0, -1);
                return vertices;
            }
            return null;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return 0.1f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <param name="playArea">The GameObject containing the play area representation.</param>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return true;
        }
    }
#else
    public class SDK_SimBoundaries : SDK_FallbackBoundaries
    {
    }
#endif
}