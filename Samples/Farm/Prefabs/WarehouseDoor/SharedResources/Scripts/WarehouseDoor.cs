namespace VRTK.Examples.Prefabs.WarehouseDoor
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Data.Type;

    /// <summary>
    /// Controls the motion of the warehouse door.
    /// </summary>
    public class WarehouseDoor : MonoBehaviour
    {
        /// <summary>
        /// The door to control.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Door { get; set; }
        /// <summary>
        /// The minimum and maximum height of the door.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public FloatRange HeightLimits { get; set; }

        /// <summary>
        /// The step to increase or decrease the height of the door by.
        /// </summary>
        public float HeightStep { get; set; }

        protected virtual void Update()
        {
            if (!HeightStep.ApproxEquals(0f))
            {
                float newHeight = Mathf.Clamp(Door.transform.localScale.y + HeightStep, HeightLimits.minimum, HeightLimits.maximum);
                Door.transform.localScale = new Vector3(1f, newHeight, 1f);
            }
        }
    }
}