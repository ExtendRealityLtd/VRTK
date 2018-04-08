// Interact Haptics|Interactables|35020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerReference">The reference to the controller to perform haptics on.</param>
    public struct InteractHapticsEventArgs
    {
        public VRTK_ControllerReference controllerReference;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="InteractHapticsEventArgs"/></param>
    public delegate void InteractHapticsEventHandler(object sender, InteractHapticsEventArgs e);

    /// <summary>
    /// Provides controller haptics upon interaction with the specified Interactable Object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Object To Affect` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_InteractHaptics` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Object To Affect` parameter of this script.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/VRTK_InteractHaptics")]
    public class VRTK_InteractHaptics : VRTK_InteractableListener
    {
        [Header("Haptics On Near Touch Settings")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on near touch.")]
        public AudioClip clipOnNearTouch;
        [Tooltip("Denotes how strong the rumble in the controller will be on near touch.")]
        [Range(0, 1)]
        public float strengthOnNearTouch = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on near touch.")]
        public float durationOnNearTouch = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on near touch.")]
        public float intervalOnNearTouch = minInterval;
        [Tooltip("If this is checked then the rumble will be cancelled when the controller is no longer near touching.")]
        public bool cancelOnNearUntouch = true;

        [Header("Haptics On Touch Settings")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on touch.")]
        public AudioClip clipOnTouch;
        [Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
        [Range(0, 1)]
        public float strengthOnTouch = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on touch.")]
        public float durationOnTouch = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on touch.")]
        public float intervalOnTouch = minInterval;
        [Tooltip("If this is checked then the rumble will be cancelled when the controller is no longer touching.")]
        public bool cancelOnUntouch = true;

        [Header("Haptics On Grab Settings")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on grab.")]
        public AudioClip clipOnGrab;
        [Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
        [Range(0, 1)]
        public float strengthOnGrab = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on grab.")]
        public float durationOnGrab = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on grab.")]
        public float intervalOnGrab = minInterval;
        [Tooltip("If this is checked then the rumble will be cancelled when the controller is no longer grabbing.")]
        public bool cancelOnUngrab = true;

        [Header("Haptics On Use Settings")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on use.")]
        public AudioClip clipOnUse;
        [Tooltip("Denotes how strong the rumble in the controller will be on use.")]
        [Range(0, 1)]
        public float strengthOnUse = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on use.")]
        public float durationOnUse = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on use.")]
        public float intervalOnUse = minInterval;
        [Tooltip("If this is checked then the rumble will be cancelled when the controller is no longer using.")]
        public bool cancelOnUnuse = true;

        [Header("Custom Settings")]

        [Tooltip("The Interactable Object to initiate the haptics from. If this is left blank, then the Interactable Object will need to be on the current or a parent GameObject.")]
        public VRTK_InteractableObject objectToAffect;

        /// <summary>
        /// Emitted when the haptics are from a near touch.
        /// </summary>
        public event InteractHapticsEventHandler InteractHapticsNearTouched;
        /// <summary>
        /// Emitted when the haptics are from a touch.
        /// </summary>
        public event InteractHapticsEventHandler InteractHapticsTouched;
        /// <summary>
        /// Emitted when the haptics are from a grab.
        /// </summary>
        public event InteractHapticsEventHandler InteractHapticsGrabbed;
        /// <summary>
        /// Emitted when the haptics are from a use.
        /// </summary>
        public event InteractHapticsEventHandler InteractHapticsUsed;

        protected const float minInterval = 0.05f;

        public virtual void OnInteractHapticsNearTouched(InteractHapticsEventArgs e)
        {
            if (InteractHapticsNearTouched != null)
            {
                InteractHapticsNearTouched(this, e);
            }
        }

        public virtual void OnInteractHapticsTouched(InteractHapticsEventArgs e)
        {
            if (InteractHapticsTouched != null)
            {
                InteractHapticsTouched(this, e);
            }
        }

        public virtual void OnInteractHapticsGrabbed(InteractHapticsEventArgs e)
        {
            if (InteractHapticsGrabbed != null)
            {
                InteractHapticsGrabbed(this, e);
            }
        }

        public virtual void OnInteractHapticsUsed(InteractHapticsEventArgs e)
        {
            if (InteractHapticsUsed != null)
            {
                InteractHapticsUsed(this, e);
            }
        }

        /// <summary>
        /// The CancelHaptics method cancels any existing haptic feedback on the given controller.
        /// </summary>
        /// <param name="controllerReference"></param>
        public virtual void CancelHaptics(VRTK_ControllerReference controllerReference)
        {
            VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
        }

        /// <summary>
        /// The HapticsOnNearTouch method triggers the haptic feedback on the given controller for the settings associated with near touch.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        public virtual void HapticsOnNearTouch(VRTK_ControllerReference controllerReference)
        {
            if (clipOnNearTouch != null)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnNearTouch);
            }
            else if (strengthOnNearTouch > 0 && durationOnNearTouch > 0f)
            {
                TriggerHapticPulse(controllerReference, strengthOnNearTouch, durationOnNearTouch, intervalOnNearTouch);
            }
            else
            {
                VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
            }
            OnInteractHapticsNearTouched(SetEventPayload(controllerReference));
        }

        /// <summary>
        /// The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        public virtual void HapticsOnTouch(VRTK_ControllerReference controllerReference)
        {
            if (clipOnTouch != null)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnTouch);
            }
            else if (strengthOnTouch > 0 && durationOnTouch > 0f)
            {
                TriggerHapticPulse(controllerReference, strengthOnTouch, durationOnTouch, intervalOnTouch);
            }
            else
            {
                VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
            }
            OnInteractHapticsTouched(SetEventPayload(controllerReference));
        }

        /// <summary>
        /// The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        public virtual void HapticsOnGrab(VRTK_ControllerReference controllerReference)
        {
            if (clipOnGrab != null)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnGrab);
            }
            else if (strengthOnGrab > 0 && durationOnGrab > 0f)
            {
                TriggerHapticPulse(controllerReference, strengthOnGrab, durationOnGrab, intervalOnGrab);
            }
            else
            {
                VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
            }
            OnInteractHapticsGrabbed(SetEventPayload(controllerReference));
        }

        /// <summary>
        /// The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        public virtual void HapticsOnUse(VRTK_ControllerReference controllerReference)
        {
            if (clipOnUse != null)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnUse);
            }
            else if (strengthOnUse > 0 && durationOnUse > 0f)
            {
                TriggerHapticPulse(controllerReference, strengthOnUse, durationOnUse, intervalOnUse);
            }
            else
            {
                VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
            }
            OnInteractHapticsUsed(SetEventPayload(controllerReference));
        }

        protected virtual void OnEnable()
        {
            EnableListeners();
        }

        protected virtual void OnDisable()
        {
            DisableListeners();
        }

        protected override bool SetupListeners(bool throwError)
        {
            objectToAffect = (objectToAffect != null ? objectToAffect : GetComponentInParent<VRTK_InteractableObject>());
            if (objectToAffect != null)
            {
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, CancelNearTouchHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, CancelTouchHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, CancelGrabHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, CancelUseHaptics);

                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHaptics);
                objectToAffect.SubscribeToInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHaptics);
                return true;
            }
            else if (throwError)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractHaptics", "VRTK_InteractableObject", "the same or parent"));
            }
            return false;
        }

        protected override void TearDownListeners()
        {
            if (objectToAffect != null)
            {
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearUntouch, CancelNearTouchHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Untouch, CancelTouchHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Ungrab, CancelGrabHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Unuse, CancelUseHaptics);

                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.NearTouch, NearTouchHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Touch, TouchHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Grab, GrabHaptics);
                objectToAffect.UnsubscribeFromInteractionEvent(VRTK_InteractableObject.InteractionType.Use, UseHaptics);
            }
        }

        protected virtual void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float interval)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength, duration, (interval >= minInterval ? interval : minInterval));
        }

        protected virtual InteractHapticsEventArgs SetEventPayload(VRTK_ControllerReference givenControllerReference)
        {
            InteractHapticsEventArgs e;
            e.controllerReference = givenControllerReference;
            return e;
        }

        protected virtual void NearTouchHaptics(object sender, InteractableObjectEventArgs e)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(e.interactingObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                HapticsOnNearTouch(controllerReference);
            }
        }

        protected virtual void TouchHaptics(object sender, InteractableObjectEventArgs e)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(e.interactingObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                HapticsOnTouch(controllerReference);
            }
        }

        protected virtual void GrabHaptics(object sender, InteractableObjectEventArgs e)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(e.interactingObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                HapticsOnGrab(controllerReference);
            }
        }

        protected virtual void UseHaptics(object sender, InteractableObjectEventArgs e)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(e.interactingObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                HapticsOnUse(controllerReference);
            }
        }

        protected virtual void CancelOn(GameObject givenObject)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(givenObject);
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                CancelHaptics(controllerReference);
            }
        }

        protected virtual void CancelNearTouchHaptics(object sender, InteractableObjectEventArgs e)
        {
            if (cancelOnNearUntouch)
            {
                CancelOn(e.interactingObject);
            }
        }

        protected virtual void CancelTouchHaptics(object sender, InteractableObjectEventArgs e)
        {
            if (cancelOnUntouch)
            {
                CancelOn(e.interactingObject);
            }
        }

        protected virtual void CancelGrabHaptics(object sender, InteractableObjectEventArgs e)
        {
            if (cancelOnUngrab)
            {
                CancelOn(e.interactingObject);
            }
        }

        protected virtual void CancelUseHaptics(object sender, InteractableObjectEventArgs e)
        {
            if (cancelOnUnuse)
            {
                CancelOn(e.interactingObject);
            }
        }
    }
}