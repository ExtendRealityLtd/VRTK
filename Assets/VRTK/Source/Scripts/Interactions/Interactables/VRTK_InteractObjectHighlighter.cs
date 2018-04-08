// Interact Object Highlighter|Interactables|35040
namespace VRTK
{
    using UnityEngine;
    using Highlighters;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactionType">The type of interaction occuring on the object to monitor.</param>
    /// <param name="highlightColor">The colour being provided to highlight the affected object with.</param>
    /// <param name="affectingObject">The GameObject is initiating the highlight via an interaction.</param>
    /// <param name="objectToMonitor">The Interactable Object that is being interacted with.</param>
    /// <param name="affectedObject">The GameObject that is being highlighted.</param>
    public struct InteractObjectHighlighterEventArgs
    {
        public VRTK_InteractableObject.InteractionType interactionType;
        public Color highlightColor;
        public GameObject affectingObject;
        public VRTK_InteractableObject objectToMonitor;
        public GameObject affectedObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractObjectHighlighterEventArgs"/></param>
    public delegate void InteractObjectHighlighterEventHandler(object sender, InteractObjectHighlighterEventArgs e);

    /// <summary>
    /// Enable highlighting of an Interactable Object base on interaction type.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Object To Monitor` parameter.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BaseHighlighter` - The highlighter to use when highligting the Object. If one is not already injected in the `Object Highlighter` parameter then the component on the same GameObject will be used.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_InteractObjectHighlighter` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Object To Affect` parameter of this script.
    /// </remarks>

    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/VRTK_InteractObjectHighlighter")]
    public class VRTK_InteractObjectHighlighter : VRTK_InteractableListener
    {
        [Header("Object Interaction Settings")]

        [Tooltip("The colour to highlight the object on the near touch interaction.")]
        public Color nearTouchHighlight = Color.clear;
        [Tooltip("The colour to highlight the object on the touch interaction.")]
        public Color touchHighlight = Color.clear;
        [Tooltip("The colour to highlight the object on the grab interaction.")]
        public Color grabHighlight = Color.clear;
        [Tooltip("The colour to highlight the object on the use interaction.")]
        public Color useHighlight = Color.clear;

        [Header("Custom Settings")]

        [Tooltip("The Interactable Object to monitor the interactions on. If this is left blank, then the Interactable Object will need to be on the current or a parent GameObject.")]
        public VRTK_InteractableObject objectToMonitor;
        [Tooltip("The GameObject to highlight.")]
        public GameObject objectToHighlight;
        [Tooltip("An optional Highlighter to use when highlighting the specified Object. If this is left blank, then the first active highlighter on the same GameObject will be used, if one isn't found then a Material Color Swap Highlighter will be created at runtime.")]
        public VRTK_BaseHighlighter objectHighlighter;

        [Header("Obsolete Settings")]

        [System.Obsolete("`objectToAffect` has been replaced with `objectToHighlight`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public VRTK_InteractableObject objectToAffect;

        protected Color currentColour = Color.clear;
        protected VRTK_BaseHighlighter baseHighlighter;
        protected bool createBaseHighlighter;
        protected GameObject currentAffectingObject;

        /// <summary>
        /// Emitted when the object is highlighted
        /// </summary>
        public event InteractObjectHighlighterEventHandler InteractObjectHighlighterHighlighted;
        /// <summary>
        /// Emitted when the object is unhighlighted
        /// </summary>
        public event InteractObjectHighlighterEventHandler InteractObjectHighlighterUnhighlighted;

        public virtual void OnInteractObjectHighlighterHighlighted(InteractObjectHighlighterEventArgs e)
        {
            if (InteractObjectHighlighterHighlighted != null)
            {
                InteractObjectHighlighterHighlighted(this, e);
            }
        }

        public virtual void OnInteractObjectHighlighterUnhighlighted(InteractObjectHighlighterEventArgs e)
        {
            if (InteractObjectHighlighterUnhighlighted != null)
            {
                InteractObjectHighlighterUnhighlighted(this, e);
            }
        }

        /// <summary>
        /// The ResetHighlighter method is used to reset the currently attached highlighter.
        /// </summary>
        public virtual void ResetHighlighter()
        {
            if (baseHighlighter != null)
            {
                baseHighlighter.ResetHighlighter();
            }
        }

        /// <summary>
        /// The Highlight method turns on the highlighter with the given Color.
        /// </summary>
        /// <param name="highlightColor">The colour to apply to the highlighter.</param>
        public virtual void Highlight(Color highlightColor)
        {
            InitialiseHighlighter(highlightColor);
            if (baseHighlighter != null && highlightColor != Color.clear)
            {
                baseHighlighter.Highlight(highlightColor);
            }
            else
            {
                Unhighlight();
            }
        }

        /// <summary>
        /// The Unhighlight method turns off the highlighter.
        /// </summary>
        public virtual void Unhighlight()
        {
            if (baseHighlighter != null)
            {
                baseHighlighter.Unhighlight();
            }
        }

        /// <summary>
        /// The GetCurrentHighlightColor returns the colour that the Interactable Object is currently being highlighted to.
        /// </summary>
        /// <returns>The Color that the Interactable Object is being highlighted to.</returns>
        public virtual Color GetCurrentHighlightColor()
        {
            return currentColour;
        }

        public virtual GameObject GetAffectingObject()
        {
            return currentAffectingObject;
        }

        protected virtual void OnEnable()
        {
#pragma warning disable 0618
            objectToMonitor = (objectToMonitor == null ? objectToAffect : objectToMonitor);
            objectToHighlight = (objectToHighlight == null && objectToAffect != null ? objectToAffect.gameObject : objectToHighlight);
#pragma warning restore 0618

            objectToHighlight = (objectToHighlight != null ? objectToHighlight : gameObject);
            if (GetValidHighlighter() != baseHighlighter)
            {
                baseHighlighter = null;
            }
            EnableListeners();
        }

        protected virtual void OnDisable()
        {
            if (createBaseHighlighter)
            {
                Destroy(baseHighlighter);
            }
            DisableListeners();
        }

        protected override bool SetupListeners(bool throwError)
        {
            objectToMonitor = (objectToMonitor != null ? objectToMonitor : GetComponentInParent<VRTK_InteractableObject>());
            if (objectToMonitor != null)
            {
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, TouchUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToMonitor.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
                return true;
            }
            else if (throwError)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractObjectHighlighter", "VRTK_InteractableObject", "the same or parent"));
            }
            return false;
        }

        protected override void TearDownListeners()
        {
            if (objectToMonitor != null)
            {
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, TouchUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToMonitor.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
            }
        }

        protected virtual InteractObjectHighlighterEventArgs SetEventArgs(VRTK_InteractableObject.InteractionType interactionType, GameObject affectingObject)
        {
            currentAffectingObject = affectingObject;
            InteractObjectHighlighterEventArgs e;
            e.interactionType = interactionType;
            e.highlightColor = currentColour;
            e.affectingObject = affectingObject;
            e.objectToMonitor = objectToMonitor;
            e.affectedObject = objectToHighlight;
            return e;
        }

        protected virtual void NearTouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            Highlight(nearTouchHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
        }

        protected virtual void NearTouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (!interactableObject.IsTouched())
            {
                Unhighlight();
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearUntouch, e.interactingObject));
            }
        }

        protected virtual void TouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            Highlight(touchHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
        }

        protected virtual void TouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsNearTouched())
            {
                Highlight(nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight();
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Untouch, e.interactingObject));
            }
        }

        protected virtual void GrabHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (!interactableObject.IsUsing())
            {
                Highlight(grabHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Grab, e.interactingObject));
            }
        }

        protected virtual void GrabUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsTouched())
            {
                Highlight(touchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
            }
            else if (interactableObject.IsNearTouched())
            {
                Highlight(nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight();
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Ungrab, e.interactingObject));
            }
        }

        protected virtual void UseHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            Highlight(useHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Use, e.interactingObject));
        }

        protected virtual void UseUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsGrabbed())
            {
                Highlight(grabHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Grab, e.interactingObject));
            }
            else if (interactableObject.IsTouched())
            {
                Highlight(touchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
            }
            else if (interactableObject.IsNearTouched())
            {
                Highlight(nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight();
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Unuse, e.interactingObject));
            }
        }

        protected virtual void InitialiseHighlighter(Color highlightColor)
        {
            if (baseHighlighter == null && highlightColor != Color.clear)
            {
                createBaseHighlighter = false;
                baseHighlighter = GetValidHighlighter();
                if (baseHighlighter == null)
                {
                    createBaseHighlighter = true;
                    baseHighlighter = objectToHighlight.AddComponent<VRTK_MaterialColorSwapHighlighter>();
                }
                baseHighlighter.Initialise(highlightColor, objectToHighlight);
            }
        }

        protected virtual VRTK_BaseHighlighter GetValidHighlighter()
        {
            return (objectHighlighter != null ? objectHighlighter : VRTK_BaseHighlighter.GetActiveHighlighter(objectToHighlight));
        }
    }
}