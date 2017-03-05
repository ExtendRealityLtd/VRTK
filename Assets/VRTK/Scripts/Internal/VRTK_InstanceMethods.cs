namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class VRTK_InstanceMethods : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance to access the InstanceMethods variables from.
        /// </summary>
        public static VRTK_InstanceMethods instance = null;

        private Dictionary<uint, Coroutine> hapticLoopCoroutines = new Dictionary<uint, Coroutine>();

        public virtual void TriggerHapticPulse(uint controllerIndex, float strength)
        {
            if (enabled)
            {
                CancelHapticPulse(controllerIndex);
                var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
                VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
            }
        }

        public virtual void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
        {
            if (enabled)
            {
                CancelHapticPulse(controllerIndex);
                var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
                var hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
                Coroutine hapticLoop = StartCoroutine(HapticPulse(controllerIndex, duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
                if (!hapticLoopCoroutines.ContainsKey(controllerIndex))
                {
                    hapticLoopCoroutines.Add(controllerIndex, hapticLoop);
                }
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.persistOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void CancelHapticPulse(uint controllerIndex)
        {
            if (hapticLoopCoroutines.ContainsKey(controllerIndex) && hapticLoopCoroutines[controllerIndex] != null)
            {
                StopCoroutine(hapticLoopCoroutines[controllerIndex]);
            }
        }

        private IEnumerator HapticPulse(uint controllerIndex, float duration, float hapticPulseStrength, float pulseInterval)
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