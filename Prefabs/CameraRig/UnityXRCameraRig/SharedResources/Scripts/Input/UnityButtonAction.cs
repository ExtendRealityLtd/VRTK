namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;

    /// <summary>
    /// Listens for the specified key state and emits the appropriate action.
    /// </summary>
    public class UnityButtonAction : BooleanAction
    {
        /// <summary>
        /// The <see cref="UnityEngine.KeyCode"/> to listen for state changes on.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public KeyCode KeyCode { get; set; }

        protected virtual void Update()
        {
            Receive(Input.GetKey(KeyCode));
        }
    }
}