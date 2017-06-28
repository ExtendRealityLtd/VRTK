// Interact Haptics|Interactions|30100
namespace VRTK
{
    using UnityEngine;
    using System;

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
    /// The Interact Haptics script is attached on the same GameObject as an Interactable Object script and provides controller haptics on touch, grab and use of the object.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractHaptics")]
    public class VRTK_InteractHaptics : MonoBehaviour
    {
        [Header("Haptics On Touch")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on touch.")]
        public AudioClip clipOnTouch;
        [Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
        [Range(0, 1)]
        public float strengthOnTouch = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on touch.")]
        public float durationOnTouch = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on touch.")]
        public float intervalOnTouch = minInterval;

        [Header("Haptics On Grab")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on grab.")]
        public AudioClip clipOnGrab;
        [Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
        [Range(0, 1)]
        public float strengthOnGrab = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on grab.")]
        public float durationOnGrab = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on grab.")]
        public float intervalOnGrab = minInterval;

        [Header("Haptics On Use")]

        [Tooltip("Denotes the audio clip to use to rumble the controller on use.")]
        public AudioClip clipOnUse;
        [Tooltip("Denotes how strong the rumble in the controller will be on use.")]
        [Range(0, 1)]
        public float strengthOnUse = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on use.")]
        public float durationOnUse = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on use.")]
        public float intervalOnUse = minInterval;

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
        /// The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        [Obsolete("`VRTK_InteractHaptics.HapticsOnTouch(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnTouch(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public virtual void HapticsOnTouch(uint controllerIndex)
        {
            HapticsOnTouch(VRTK_ControllerReference.GetControllerReference(controllerIndex));
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
            OnInteractHapticsTouched(SetEventPayload(controllerReference));
        }

        /// <summary>
        /// The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        [Obsolete("`VRTK_InteractHaptics.HapticsOnGrab(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnGrab(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public virtual void HapticsOnGrab(uint controllerIndex)
        {
            HapticsOnGrab(VRTK_ControllerReference.GetControllerReference(controllerIndex));
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
            OnInteractHapticsGrabbed(SetEventPayload(controllerReference));
        }

        /// <summary>
        /// The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        [Obsolete("`VRTK_InteractHaptics.HapticsOnUse(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnUse(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public virtual void HapticsOnUse(uint controllerIndex)
        {
            HapticsOnUse(VRTK_ControllerReference.GetControllerReference(controllerIndex));
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
            OnInteractHapticsUsed(SetEventPayload(controllerReference));
        }

        protected virtual void OnEnable()
        {
            if (!GetComponent<VRTK_InteractableObject>())
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractHaptics", "VRTK_InteractableObject", "the same"));
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
    }
}