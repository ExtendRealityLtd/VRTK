namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class VRTK_Haptics : MonoBehaviour
    {
        protected Dictionary<uint, Coroutine> hapticLoopCoroutines = new Dictionary<uint, Coroutine>();

        public virtual void TriggerHapticPulse(uint controllerIndex, float strength)
        {
            CancelCurrentHapticPulse(controllerIndex);
            var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
            VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
        }

        public virtual void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
        {
            CancelCurrentHapticPulse(controllerIndex);
            var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
            var hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
            Coroutine hapticLoop = StartCoroutine(HapticPulse(controllerIndex, duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
            if (!hapticLoopCoroutines.ContainsKey(controllerIndex))
            {
                hapticLoopCoroutines.Add(controllerIndex, hapticLoop);
            }
        }

        public virtual void CancelHapticPulse(uint controllerIndex)
        {
            CancelCurrentHapticPulse(controllerIndex);
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            hapticLoopCoroutines.Clear();
        }

        protected virtual void CancelCurrentHapticPulse(uint controllerIndex)
        {
            if (hapticLoopCoroutines.ContainsKey(controllerIndex) && hapticLoopCoroutines[controllerIndex] != null)
            {
                StopCoroutine(hapticLoopCoroutines[controllerIndex]);
                hapticLoopCoroutines.Remove(controllerIndex);
            }
        }

        protected virtual IEnumerator HapticPulse(uint controllerIndex, float duration, float hapticPulseStrength, float pulseInterval)
        {
            if (pulseInterval <= 0)
            {
                yield break;
            }

            while (duration > 0)
            {
                VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
                yield return new WaitForSeconds(pulseInterval);
                duration -= pulseInterval;
            }
        }
    }
}