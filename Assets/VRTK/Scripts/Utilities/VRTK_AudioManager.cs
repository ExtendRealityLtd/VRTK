namespace VRTK
{
	using UnityEngine;
	using System.Collections.Generic;

	public class VRTK_AudioManager : MonoBehaviour
	{
		[TooltipAttribute("Fallback audio to play when something without AudioMaterial is hit (optional).")]
		public VRTK_AudioMaterialData fallbackAudioData;
		[TooltipAttribute("Global volume multiplyer for all hit audio.")]
		[RangeAttribute(.2f, 5f)]
		public float globalHitVolume = 1;
		[TooltipAttribute("Global volume multiplyer for all slide audio.")]
		[RangeAttribute(.2f, 5f)]
		public float globalSlideVolume = 1;
		[TooltipAttribute("The distance from the player where sounds should stop playing.")]
		public float stopAudioDistance = 25;
		[TooltipAttribute("Size of the audio pool. Recommended is twice the size of the maximum simultaneous sounds you expect.")]
		public int audioPoolSize = 8;
		[TooltipAttribute("Using Audio Mixing will halve the needed AudioSources, but very slightly add some computing.")]
		public bool useAudioMixing = true;
		[TooltipAttribute("The sample rate to use for all AudioClips (Only with Audio Mixing).")]
		public int sampleRate = 44100;
		[TooltipAttribute("Use Autio Haptics feature with hit/slide sounds.")]
		public bool useAudioHaptics = true;

		public static VRTK_AudioManager instance;

		private List<GameObject> audioPool;
		private static GameObject poolParent;

		private void Awake()
		{
			if(instance == null)
			{
				instance = this;
			}

			poolParent = new GameObject("VRTK_AudioPool");
			audioPool = new List<GameObject>();
			for(int i = 0; i < audioPoolSize; i++)
			{
				GameObject go = new GameObject("VRTK_AudioPoolObject");
				AudioSource audioSource = go.AddComponent<AudioSource>();
				audioSource.spatialBlend = 1;
				audioPool.Add(go);
				go.transform.SetParent(poolParent.transform);
			}
			CheckAssets();
		}

		public static void PlayAtPoint(AudioClip clip, Vector3 pos, float volume, float pitch)
		{
			GameObject source = instance.audioPool[0];
			if(source)
			{
				instance.audioPool.RemoveAt(0);
				instance.audioPool.Insert(instance.audioPool.Count - 1, source);
				SetSourceData(source, clip, pos, volume * instance.globalHitVolume, pitch, false);
			}
		}

		public static AudioSource PlayLoopAtPoint(AudioClip clip, Transform trans, float volume, float pitch)
		{
			GameObject source = instance.audioPool[0];
			if(source)
			{
				instance.audioPool.RemoveAt(0);
				source.transform.SetParent(trans);
				return SetSourceData(source, clip, trans.position, volume * instance.globalSlideVolume, pitch, true);
			}
			return null;
		}

		public static void StopLoop(AudioSource audioSource)
		{
			audioSource.Stop();
			audioSource.transform.SetParent(poolParent.transform);
			instance.audioPool.Add(audioSource.gameObject);
		}

		private static AudioSource SetSourceData(GameObject source, AudioClip clip, Vector3 pos, float volume, float pitch, bool loop)
		{
			AudioSource audioSource = source.GetComponent<AudioSource>();
			source.transform.position = pos;
			audioSource.pitch = pitch;
			audioSource.volume = volume;
			audioSource.clip = clip;
			audioSource.loop = loop;
			audioSource.Play();
			return audioSource;
		}

		private static void CheckAssets()
		{
			if(instance.useAudioMixing)
			{
				VRTK_AudioMaterialData[] materials = (VRTK_AudioMaterialData[])(Resources.FindObjectsOfTypeAll(typeof(VRTK_AudioMaterialData)));
				foreach (VRTK_AudioMaterialData item in materials)
				{
					if(item.hitAudioClip.channels != 1)
					{
						Debug.LogWarning("The clip " + item.hitAudioClip + " has more channels then one, please set it to mono in the import settings.");
					}
					if(item.slideAudioClip.channels != 1)
					{
						Debug.LogWarning("The clip " + item.slideAudioClip + " has more channels then one, please set it to mono in the import settings.");
					}
					if(item.hitAudioClip.frequency != instance.sampleRate)
					{
						Debug.LogWarning("The clip " + item.hitAudioClip + " frequency does not match the choosen sample rate:" + instance.sampleRate + ".");
					}
					if(item.slideAudioClip.frequency != instance.sampleRate)
					{
						Debug.LogWarning("The clip " + item.slideAudioClip + " frequency does not match the choosen sample rate:" + instance.sampleRate + ".");
					}
				}
			}
		}
	}
}
