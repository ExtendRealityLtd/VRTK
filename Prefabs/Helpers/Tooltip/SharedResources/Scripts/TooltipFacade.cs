namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the TooltipFacade prefab.
    /// </summary>
    public class TooltipFacade : MonoBehaviour
    {
        #region Tooltip Settings
        [Header("Tooltip Settings"), Tooltip("The object that the tooltip will face towards."), SerializeField]
        private GameObject facingSource;
        /// <summary>
        /// The object that the tooltip will face towards.
        /// </summary>
        public GameObject FacingSource
        {
            get
            {
                return facingSource;
            }
            set
            {
                facingSource = value;
            }
        }

        [Tooltip("The target to draw the tooltip line to."), SerializeField]
        private GameObject lineTarget;
        /// <summary>
        /// The target to draw the tooltip line to.
        /// </summary>
        public GameObject LineTarget
        {
            get
            {
                return lineTarget;
            }
            set
            {
                lineTarget = value;
                internalSetup.SetLine(lineTarget);
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected TooltipInternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.SetLine(LineTarget);
        }
    }
}