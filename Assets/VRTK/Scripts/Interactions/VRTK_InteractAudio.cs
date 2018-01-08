namespace VRTK
{
	using UnityEngine;
	using System.Collections;
	using VRTK.GrabAttachMechanics;

	public class VRTK_InteractAudio : VRTK_ActiveAudioMaterial
	{
		VRTK_ControllerReference controller;

		protected override void Awake()
		{
			base.Awake();
			VRTK_InteractableObject io = GetComponent<VRTK_InteractableObject>();
			if(io.grabAttachMechanicScript is VRTK_ChildOfControllerGrabAttach)
			{
				Debug.LogWarning("Using ChildOfController as grab mechanic is currently not supported.\n Please use FixedJoint or Tracked instead.");
			}
			if(io.disableWhenIdle)
			{
				io.disableWhenIdle = false;
				Debug.LogWarning("disableWhenIdle can not be used with Physics Audio, setting it to false for the session.");
			}
			controller = VRTK_ControllerReference.GetControllerReference(gameObject);
		}

		protected override void PlaySound(VRTK_AudioMaterialData otherMat, Collision other)
		{
			base.PlaySound(otherMat, other);
			if(audioManager.useAudioHaptics)
			{
				VRTK_ControllerHaptics.TriggerHapticPulse(controller, audioMaterialData.hitAudioClip);
			}
		}

		protected override void PlaySlideSound()
		{
			base.PlaySlideSound();
			StartCoroutine("LoopSlideHaptics", slidingSource.clip);
		}

		protected override void OnCollisionExit(Collision other)
		{
			base.OnCollisionExit(other);
			StopCoroutine("LoopSlideHaptics");
		}

		private IEnumerator LoopSlideHaptics(AudioClip clip)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(controller, clip);
			yield return new WaitForSeconds(clip.length);
		}
	}
}
