namespace VRTK
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "AudioMaterialData", menuName = "VRTK/AudioMaterialData")]

	public class VRTK_AudioMaterialData : ScriptableObject
	{
		[TooltipAttribute("AudioClip to use for hit sounds.")]
		public AudioClip hitAudioClip;
		[TooltipAttribute("AudioClip to use for hit slide.")]
		public AudioClip slideAudioClip;
		[TooltipAttribute("Minimum random pitch to use.")]
		[RangeAttribute(.5f, 2f)]
		public float minPitch;
		[TooltipAttribute("Maximum random pitch to use.")]
		[RangeAttribute(.5f, 2f)]
		public float maxPitch;
	}
}
