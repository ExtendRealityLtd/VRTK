// Interact Haptics|Interactions|30070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Haptics script is attached on the same GameObject as an Interactable Object script and provides controller haptics on touch, grab and use of the object.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractHaptics")]
    public class VRTK_InteractHaptics : MonoBehaviour
    {
        [Header("Haptics On Touch")]
        [Tooltip("Optionally select an AudioClip to use as touch haptic.")]
        public AudioClip audioClipTouch;
        [Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
        [Range(0, 1)]
        public float strengthOnTouch = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on touch.")]
        public float durationOnTouch = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on touch.")]
        public float intervalOnTouch = minInterval;

        [Header("Haptics On Grab")]
        [Tooltip("Optionally select an AudioClip to use as grab haptic.")]
        public AudioClip audioClipGrab;
        [Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
        [Range(0, 1)]
        public float strengthOnGrab = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on grab.")]
        public float durationOnGrab = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on grab.")]
        public float intervalOnGrab = minInterval;

        [Header("Haptics On Use")]
        [Tooltip("Optionally select an AudioClip to use touch as use haptic.")]
        public AudioClip audioClipUse;
        [Tooltip("Denotes how strong the rumble in the controller will be on use.")]
        [Range(0, 1)]
        public float strengthOnUse = 0;
        [Tooltip("Denotes how long the rumble in the controller will last on use.")]
        public float durationOnUse = 0f;
        [Tooltip("Denotes interval betweens rumble in the controller on use.")]
        public float intervalOnUse = minInterval;

        protected const float minInterval = 0.05f;

        /// <summary>
        /// The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        public virtual void HapticsOnTouch(uint controllerIndex)
        {
            if (audioClipTouch && strengthOnTouch > 0)
            {
                TriggerHapticAudio(controllerIndex, audioClipTouch, strengthOnTouch);
            }
            else if (strengthOnTouch > 0 && durationOnTouch > 0f)
            {
                TriggerHapticPulse(controllerIndex, strengthOnTouch, durationOnTouch, intervalOnTouch);
            }
        }

        /// <summary>
        /// The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        public virtual void HapticsOnGrab(uint controllerIndex)
        {
            if (audioClipGrab && strengthOnGrab > 0)
            {
                TriggerHapticAudio(controllerIndex, audioClipGrab, strengthOnGrab);
            }
            else if (strengthOnGrab > 0 && durationOnGrab > 0f)
            {
                TriggerHapticPulse(controllerIndex, strengthOnGrab, durationOnGrab, intervalOnGrab);
            }
        }

        /// <summary>
        /// The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        public virtual void HapticsOnUse(uint controllerIndex)
        {
            if (audioClipUse && strengthOnUse > 0)
            {
                TriggerHapticAudio(controllerIndex, audioClipUse, strengthOnUse);
            }
            else if (strengthOnUse > 0 && durationOnUse > 0f)
            {
                TriggerHapticPulse(controllerIndex, strengthOnUse, durationOnUse, intervalOnUse);
            }
        }

        protected virtual void OnEnable()
        {
            if (!GetComponent<VRTK_InteractableObject>())
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractHaptics", "VRTK_InteractableObject", "the same"));
            }
        }

        protected virtual void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float interval)
        {
            VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength, duration, (interval >= minInterval ? interval : minInterval));
        }

        private void TriggerHapticAudio(uint controllerIndex, AudioClip clip, float strength)
        {
            VRTK_SharedMethods.TriggerHapticAudio(controllerIndex, clip, strength);
        }
    }
}