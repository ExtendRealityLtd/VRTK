// Simulator Boundaries|SDK_Simulator|004
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Sim Boundaries SDK script provides dummy functions for the play area boundaries.
    /// </summary>
    [SDK_Description(typeof(SDK_SimSystem))]
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
        /// <returns>A Vector3 array of the points in the scene that represent the play area boundaries.</returns>
        public override Vector3[] GetPlayAreaVertices()
        {
            float inner = 0.9f;
            float outer = 1f;

            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(inner, 0f, -inner);
            vertices[1] = new Vector3(-inner, 0f, -inner);
            vertices[2] = new Vector3(-inner, 0f, inner);
            vertices[3] = new Vector3(inner, 0f, inner);

            vertices[4] = new Vector3(outer, 0f, -outer);
            vertices[5] = new Vector3(-outer, 0f, -outer);
            vertices[6] = new Vector3(-outer, 0f, outer);
            vertices[7] = new Vector3(outer, 0f, outer);

            return vertices;
        }

        /// <summary>
        /// The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.
        /// </summary>
        /// <returns>The thickness of the drawn border.</returns>
        public override float GetPlayAreaBorderThickness()
        {
            return 0.1f;
        }

        /// <summary>
        /// The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.
        /// </summary>
        /// <returns>Returns true if the play area size has been auto calibrated and set by external sensors.</returns>
        public override bool IsPlayAreaSizeCalibrated()
        {
            return true;
        }

        /// <summary>
        /// The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.
        /// </summary>
        /// <returns>Returns true if the drawn border is being displayed.</returns>
        public override bool GetDrawAtRuntime()
        {
            return false;
        }

        /// <summary>
        /// The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.
        /// </summary>
        /// <param name="value">The state of whether the drawn border should be displayed or not.</param>
        public override void SetDrawAtRuntime(bool value)
        {
        }
    }
}