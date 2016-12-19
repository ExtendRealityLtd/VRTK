// Interact Haptics|Interactions|30070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Haptics script is attached along side the Interactable Object script and provides controller haptics on touch, grab and use of the object.
    /// </summary>
    public class VRTK_InteractHaptics : MonoBehaviour
    {
        [Header("Haptics On Touch")]
        [Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
        [Range(0, 1)]
        public float strengthOnTouch = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on touch.")]
        public float durationOnTouch = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on touch.")]
        public float intervalOnTouch = minInterval;

        [Header("Haptics On Grab")]
        [Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
        [Range(0, 1)]
        public float strengthOnGrab = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on grab.")]
        public float durationOnGrab = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on grab.")]
        public float intervalOnGrab = minInterval;

        [Header("Haptics On Use")]
        [Tooltip("Denotes how strong the rumble in the controller will be on use.")]
        [Range(0, 1)]
        public float strengthOnUse = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on use.")]
        public float durationOnUse = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on use.")]
        public float intervalOnUse = minInterval;

        private const float minInterval = 0.05f;

        /// <summary>
        /// The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.
        /// </summary>
        /// <param name="controllerActions">The controller to activate the haptic feedback on.</param>
        public void HapticsOnTouch(VRTK_ControllerActions controllerActions)
        {
            if (strengthOnTouch > 0 && durationOnTouch > 0f)
            {
                TriggerHapticPulse(controllerActions, strengthOnTouch, durationOnTouch, intervalOnTouch);
            }
        }

        /// <summary>
        /// The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.
        /// </summary>
        /// <param name="controllerActions">The controller to activate the haptic feedback on.</param>
        public void HapticsOnGrab(VRTK_ControllerActions controllerActions)
        {
            if (strengthOnGrab > 0 && durationOnGrab > 0f)
            {
                TriggerHapticPulse(controllerActions, strengthOnGrab, durationOnGrab, intervalOnGrab);
            }
        }

        /// <summary>
        /// The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.
        /// </summary>
        /// <param name="controllerActions">The controller to activate the haptic feedback on.</param>
        public void HapticsOnUse(VRTK_ControllerActions controllerActions)
        {
            if (strengthOnUse > 0 && durationOnUse > 0f)
            {
                TriggerHapticPulse(controllerActions, strengthOnUse, durationOnUse, intervalOnUse);
            }
        }

        private void TriggerHapticPulse(VRTK_ControllerActions controllerActions, float strength, float duration, float interval)
        {
            if (controllerActions)
            {
                controllerActions.TriggerHapticPulse(strength, duration, (interval >= minInterval ? interval : minInterval));
            }
        }
    }
}