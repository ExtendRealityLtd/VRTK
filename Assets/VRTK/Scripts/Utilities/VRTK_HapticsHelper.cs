// Haptics Helper|Utilities|90025

namespace VRTK
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The haptics helper offers a collection of extension methods to make it easy to play haptic feedback sourced from audio clips, animation curves, and arbitrary float functions.  Due to the way extension methods work, you call the methods in VRTK_HapticsHelper as if they belong to VRTK_ControllerActions.
    /// </summary>
    public static class VRTK_HapticsHelper
    {
        static ushort maxHapticStrength = 3999;
        static int bufferSize = 8192;
        static int sampleCount = 1024;

        /// <summary>
        /// Play a provided audioclip as a series of haptic pulses.  Note:  If the clip is compressed, it must be set to Decompress on Load in import settings.
        /// </summary>
        /// <param name="controller">The controller to play the haptic sequence on</param>
        /// <param name="clip">The audioclip to convert to haptic pulses.</param>
        /// <param name="strength">The strength to play the haptic sequence at.  At 1, the audio will scale from 0 to maxHapticPulse.</param>
        /// <param name="timeScale">The timescale to play at.  At 1, haptic sequence will play in realtime.  At 2, will play at double speed.</param>
        public static void PlayHaptics(this VRTK_ControllerActions controller, AudioClip clip, float strength = 1, float timeScale = 1)
        {
            controller.StartCoroutine(PlayHapticsRoutine(controller, clip, strength, timeScale));
        }

        /// <summary>
        /// Play the provided audiosource's output as a series of haptic pulses.  Stops when the audiosource stops playing.
        /// </summary>
        /// <param name="controller">The controller to play the haptic sequence on</param>
        /// <param name="sound">The audiosource to use as the source of haptic data</param>
        /// <param name="strength">The strength to play the haptic sequence at.  At 1, the audio will scale from 0 to maxHapticPulse.</param>
        public static void PlayHaptics(this VRTK_ControllerActions controller, AudioSource sound, float strength = 1)
        {
            controller.StartCoroutine(PlayHapticsRoutine(controller, sound, strength));
        }

        /// <summary>
        /// Play a provided animation curve as a series of haptic pulses.
        /// </summary>
        /// <param name="controller">The controller to play the haptic sequence on.</param>
        /// <param name="curve">The animation curve to convert to haptic pulses.  Absolute values of the curve will be used.</param>
        /// <param name="strength">The strength to play the haptic sequence at.  At 1, 1 on the animation curve will be maxHapticStrength.</param>
        /// <param name="duration">The duration of the haptic sequence in seconds.</param>
        public static void PlayHaptics(this VRTK_ControllerActions controller, AnimationCurve curve, float strength = 1, float duration = 1)
        {
            controller.StartCoroutine(PlayHapticsRoutine(controller, curve, strength, duration));
        }

        /// <summary>
        /// Play a provided function as a series of haptic pulses.  Function must have signature of float func(float).
        /// </summary>
        /// <param name="controller">The controller to play the haptic sequence on.</param>
        /// <param name="function">The function to convert to haptic pulses.  The function should take input in the range from 0 to 1.</param>
        /// <param name="strength">The strength to play the haptic sequence at.  At 1, a value of 1 returned by the function will be maxHapticStrength.</param>
        /// <param name="duration">The duration of the haptic sequence in seconds.</param>
        public static void PlayHaptics(this VRTK_ControllerActions controller, System.Func<float, float> function, float strength = 1, float duration = 1)
        {
            controller.StartCoroutine(PlayHapticsRoutine(controller, function, strength, duration));
        }

        static IEnumerator PlayHapticsRoutine(VRTK_ControllerActions controller, AudioClip clip, float strength, float timeScale)
        {
            float hapticScalar = maxHapticStrength * strength;
            float[] audioData = new float[bufferSize];
            int sampleOffset = -bufferSize;
            float startTime = Time.time;
            float length = clip.length / timeScale;
            float endTime = startTime + length;
            float sampleRate = clip.samples;
            while (Time.time <= endTime) {
                var lerpVal = (Time.time - startTime) / length;
                int sampleIndex = (int)(sampleRate * lerpVal);
                if (sampleIndex >= sampleOffset + bufferSize) {
                    clip.GetData(audioData, sampleIndex);
                    sampleOffset = sampleIndex;
                }
                var currentSample = Mathf.Abs(audioData[sampleIndex - sampleOffset]);
                var hapticStrength = (ushort)(hapticScalar * currentSample);
                controller.TriggerHapticPulse(hapticStrength);
                yield return null;
            }
        }

        static IEnumerator PlayHapticsRoutine(VRTK_ControllerActions controller, AudioSource sound, float strength) {
            float hapticScalar = maxHapticStrength * strength;
            float[] bufferL = new float[sampleCount];
            float[] bufferR = new float[sampleCount];
            while (sound.isPlaying) {
                float sum = 0;
                sound.GetOutputData(bufferL, 0);
                sound.GetOutputData(bufferR, 1);
                for (int i=0;i<sampleCount;i++) {
                    sum += bufferL[i] * bufferL[i];
                    sum += bufferR[i] * bufferR[i];
                }
                var rms = Mathf.Sqrt(sum / (sampleCount * 2));
                var hapticStrength = (ushort)(hapticScalar * rms);
                controller.TriggerHapticPulse(hapticStrength);
                yield return null;
            }
        }

        static IEnumerator PlayHapticsRoutine(VRTK_ControllerActions controller, AnimationCurve curve, float strength, float duration)
        {
            float hapticScalar = maxHapticStrength * strength;
            float startTime = Time.time;
            float endTime = startTime + duration;
            while (Time.time <= endTime) {
                var lerpVal = (Time.time - startTime) / duration;
                var currentSample = Mathf.Abs(curve.Evaluate(lerpVal));
                var hapticStrength = (ushort)(hapticScalar * currentSample);
                controller.TriggerHapticPulse(hapticStrength);
                yield return null;
            }
        }

        static IEnumerator PlayHapticsRoutine(VRTK_ControllerActions controller, System.Func<float, float> function, float strength, float duration)
        {
            float hapticScalar = maxHapticStrength * strength;
            float startTime = Time.time;
            float endTime = startTime + duration;
            while (Time.time <= endTime) {
                var lerpVal = (Time.time - startTime) / duration;
                var currentSample = function(lerpVal);
                var hapticStrength = (ushort)(hapticScalar * currentSample);
                controller.TriggerHapticPulse(hapticStrength);
                yield return null;
            }
        }
    }
}