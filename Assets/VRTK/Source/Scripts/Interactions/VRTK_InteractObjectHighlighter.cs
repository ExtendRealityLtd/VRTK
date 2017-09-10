// Interact Object Highlighter|Interactions|30045
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="affectedObject">The GameObject that is being highlighted.</param>
    /// /// <param name="affectingObject">The GameObject is initiating the highlight via an interaction.</param>
    public struct InteractObjectHighlighterEventArgs
    {
        public VRTK_InteractableObject.InteractionType interactionType;
        public Color highlightColor;
        public VRTK_InteractableObject affectedObject;
        public GameObject affectingObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractObjectHighlighterEventArgs"/></param>
    public delegate void InteractObjectHighlighterEventHandler(object sender, InteractObjectHighlighterEventArgs e);

    /// <summary>
    /// The Interact Object Hightlighter script is attached to an Interactable Object component to provide highlighting to the object on various interaction stages.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractObjectHighlighter")]
    public class VRTK_InteractObjectHighlighter : MonoBehaviour
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

        [Tooltip("The Interactable Object to affect the highlighter of. If this is left blank, then the Interactable Object will need to be on the current or a parent GameObject.")]
        public VRTK_InteractableObject objectToAffect;

        protected Color currentColour = Color.clear;

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
        /// The GetCurrentHighlightColor returns the colour that the object is currently being highlighted to.
        /// </summary>
        /// <returns>The Color that the Interactable Object is being highlighted to.</returns>
        public virtual Color GetCurrentHighlightColor()
        {
            return currentColour;
        }

        protected virtual void OnEnable()
        {
            objectToAffect = (objectToAffect != null ? objectToAffect : GetComponentInParent<VRTK_InteractableObject>());

            if (objectToAffect != null)
            {
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHighlightObject);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, TouchUnHighlightObject);

                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractObjectHighlighter", "VRTK_InteractableObject", "the same or parent"));
            }
        }

        protected virtual void OnDisable()
        {
            if (objectToAffect != null)
            {
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHighlightObject);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, NearTouchUnHighlightObject);

                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHighlightObject);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, TouchUnHighlightObject);

                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHighlightObject);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, GrabUnHighlightObject);

                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHighlightObject);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, UseUnHighlightObject);
            }
        }

        protected virtual InteractObjectHighlighterEventArgs SetEventArgs(VRTK_InteractableObject.InteractionType interactionType, GameObject affectingObject)
        {
            InteractObjectHighlighterEventArgs e;
            e.interactionType = interactionType;
            e.affectedObject = objectToAffect;
            e.affectingObject = affectingObject;
            e.highlightColor = currentColour;
            return e;
        }

        protected virtual void NearTouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            Highlight(interactableObject, nearTouchHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
        }

        protected virtual void NearTouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (!interactableObject.IsTouched())
            {
                Unhighlight(interactableObject);
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearUntouch, e.interactingObject));
            }
        }

        protected virtual void TouchHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            Highlight(interactableObject, touchHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
        }

        protected virtual void TouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsNearTouched())
            {
                Highlight(sender as VRTK_InteractableObject, nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight(interactableObject);
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Untouch, e.interactingObject));
            }
        }

        protected virtual void GrabHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (!interactableObject.IsUsing())
            {
                Highlight(interactableObject, grabHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Grab, e.interactingObject));
            }
        }

        protected virtual void GrabUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsTouched())
            {
                Highlight(interactableObject, touchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
            }
            else if (interactableObject.IsNearTouched())
            {
                Highlight(sender as VRTK_InteractableObject, nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight(interactableObject);
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Ungrab, e.interactingObject));
            }
        }

        protected virtual void UseHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            Highlight(interactableObject, useHighlight);
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Use, e.interactingObject));
        }

        protected virtual void UseUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject interactableObject = sender as VRTK_InteractableObject;
            if (interactableObject.IsGrabbed())
            {
                Highlight(sender as VRTK_InteractableObject, grabHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Grab, e.interactingObject));
            }
            else if (interactableObject.IsTouched())
            {
                Highlight(interactableObject, touchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Touch, e.interactingObject));
            }
            else if (interactableObject.IsNearTouched())
            {
                Highlight(sender as VRTK_InteractableObject, nearTouchHighlight);
                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.NearTouch, e.interactingObject));
            }
            else
            {
                Unhighlight(interactableObject);
                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.Unuse, e.interactingObject));
            }
        }

        protected virtual void Highlight(VRTK_InteractableObject interactableObject, Color highlightColor)
        {
            if (interactableObject != null)
            {
                interactableObject.Highlight(highlightColor);
                currentColour = highlightColor;
            }
        }

        protected virtual void Unhighlight(VRTK_InteractableObject interactableObject)
        {
            if (interactableObject != null)
            {
                interactableObject.Unhighlight();
            }
        }
    }
}