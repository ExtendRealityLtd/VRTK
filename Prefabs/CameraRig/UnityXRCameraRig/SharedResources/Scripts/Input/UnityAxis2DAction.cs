namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified axes and emits the appropriate action.
    /// </summary>
    public class UnityAxis2DAction : Vector2Action
    {
        /// <summary>
        /// The named x axis to listen for state changes on.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string XAxisName { get; set; }
        /// <summary>
        /// The named y axis to listen for state changes on.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string YAxisName { get; set; }

        protected virtual void Update()
        {
            Receive(new Vector2(Input.GetAxis(XAxisName), Input.GetAxis(YAxisName)));
        }
    }
}