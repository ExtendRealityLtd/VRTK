// Controller Haptics|Interactions|30030
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A collection of static methods for calling haptic functions on a given controller.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > There is no requirement to add this script to a GameObject as all of the public methods are static and can be called directly e.g. `VRTK_ControllerHaptics.TriggerHapticPulse(ref, 1f)`.
    /// </remarks>
    public class VRTK_ControllerHaptics : MonoBehaviour
    {
        protected static VRTK_ControllerHaptics instance;
        protected Dictionary<VRTK_ControllerReference, Coroutine> hapticLoopCoroutines = new Dictionary<VRTK_ControllerReference, Coroutine>();

        /// <summary>
        /// The TriggerHapticPulse/2 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalTriggerHapticPulse(controllerReference, strength);
            }
        }

        /// <summary>
        /// The TriggerHapticPulse/4 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float pulseInterval)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalTriggerHapticPulse(controllerReference, strength, duration, pulseInterval);
            }
        }

        /// <summary>
        /// The TriggerHapticPulse/2 method calls a haptic pulse based on a given audio clip.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to activate the haptic feedback on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalTriggerHapticPulse(controllerReference, clip);
            }
        }

        /// <summary>
        /// The CancelHapticPulse method cancels the existing running haptic pulse on the given controller index.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to cancel the haptic feedback on.</param>
        public static void CancelHapticPulse(VRTK_ControllerReference controllerReference)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalCancelHapticPulse(controllerReference);
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            hapticLoopCoroutines.Clear();
        }

        protected static void SetupInstance()
        {
            if (instance == null && VRTK_SDKManager.ValidInstance())
            {
                instance = VRTK_SDKManager.instance.gameObject.AddComponent<VRTK_ControllerHaptics>();
            }
        }

        protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength)
        {
            InternalCancelHapticPulse(controllerReference);
            float hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
            VRTK_SDK_Bridge.HapticPulse(controllerReference, hapticPulseStrength);
        }

        protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float pulseInterval)
        {
            InternalCancelHapticPulse(controllerReference);
            float hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
            SDK_ControllerHapticModifiers hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
            Coroutine hapticLoop = StartCoroutine(SimpleHapticPulseRoutine(controllerReference, duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
            VRTK_SharedMethods.AddDictionaryValue(hapticLoopCoroutines, controllerReference, hapticLoop);
        }

        protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            InternalCancelHapticPulse(controllerReference);
            if (!VRTK_SDK_Bridge.HapticPulse(controllerReference, clip))
            {
                //If the SDK Bridge doesn't support audio clips then defer to a local version
                Coroutine hapticLoop = StartCoroutine(AudioClipHapticsRoutine(controllerReference, clip));
                VRTK_SharedMethods.AddDictionaryValue(hapticLoopCoroutines, controllerReference, hapticLoop);
            }
        }

        protected virtual void InternalCancelHapticPulse(VRTK_ControllerReference controllerReference)
        {
            Coroutine currentHapticLoopRoutine = VRTK_SharedMethods.GetDictionaryValue(hapticLoopCoroutines, controllerReference);
            if (currentHapticLoopRoutine != null)
            {
                StopCoroutine(currentHapticLoopRoutine);
                hapticLoopCoroutines.Remove(controllerReference);
            }
        }

        protected virtual IEnumerator SimpleHapticPulseRoutine(VRTK_ControllerReference controllerReference, float duration, float hapticPulseStrength, float pulseInterval)
        {
            if (pulseInterval <= 0)
            {
                yield break;
            }

            while (duration > 0)
            {
                VRTK_SDK_Bridge.HapticPulse(controllerReference, hapticPulseStrength);
                yield return new WaitForSeconds(pulseInterval);
                duration -= pulseInterval;
            }
        }

        protected virtual IEnumerator AudioClipHapticsRoutine(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            SDK_ControllerHapticModifiers hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
            float hapticScalar = hapticModifiers.maxHapticVibration;
            float[] audioData = new float[hapticModifiers.hapticsBufferSize];
            int sampleOffset = -hapticModifiers.hapticsBufferSize;
            float startTime = Time.time;
            float length = clip.length / 1;
            float endTime = startTime + length;
            float sampleRate = clip.samples;
            while (Time.time <= endTime)
            {
                float lerpVal = (Time.time - startTime) / length;
                int sampleIndex = (int)(sampleRate * lerpVal);
                if (sampleIndex >= sampleOffset + hapticModifiers.hapticsBufferSize)
                {
                    clip.GetData(audioData, sampleIndex);
                    sampleOffset = sampleIndex;
                }
                float currentSample = Mathf.Abs(audioData[sampleIndex - sampleOffset]);
                ushort hapticStrength = (ushort)(hapticScalar * currentSample);
                VRTK_SDK_Bridge.HapticPulse(controllerReference, hapticStrength);
                yield return null;
            }
        }
    }
}