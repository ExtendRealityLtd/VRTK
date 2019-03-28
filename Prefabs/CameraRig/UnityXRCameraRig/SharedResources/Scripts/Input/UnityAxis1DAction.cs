namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified axis and emits the appropriate action.
    /// </summary>
    public class UnityAxis1DAction : FloatAction
    {
        /// <summary>
        /// The named axis to listen for state changes on.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string AxisName { get; set; }

        protected virtual void Update()
        {
            Receive(Input.GetAxis(AxisName));
        }
    }
}