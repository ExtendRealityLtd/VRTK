namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified axes and emits the appropriate action.
    /// </summary>
    public class UnityAxis2DAction : Vector2Action
    {
        /// <summary>
        /// The named x axis to listen for state changes on.
        /// </summary>
        [Tooltip("The named x axis to listen for state changes on.")]
        public string xAxisName;
        /// <summary>
        /// The named y axis to listen for state changes on.
        /// </summary>
        [Tooltip("The named y axis to listen for state changes on.")]
        public string yAxisName;

        protected virtual void Update()
        {
            Receive(new Vector2(Input.GetAxis(xAxisName), Input.GetAxis(yAxisName)));
        }
    }
}