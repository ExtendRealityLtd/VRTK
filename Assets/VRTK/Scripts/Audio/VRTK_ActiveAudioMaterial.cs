namespace VRTK
{
	using UnityEngine;

	public class VRTK_ActiveAudioMaterial : VRTK_PassiveAudioMaterial
	{
		protected VRTK_AudioManager audioManager;
		protected AudioSource slidingSource;
		protected Rigidbody rb;
		protected Transform camRig;

		protected virtual void Awake()
		{
			audioManager = VRTK_AudioManager.instance;
			rb = GetComponent<Rigidbody>();
			enabled = false;
		}

		protected virtual void Start()
		{
			VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
		}

		protected virtual void OnEnable()
		{
			if(camRig == null)
			{
				camRig = VRTK_DeviceFinder.PlayAreaTransform();
			}
		}

		protected virtual void Update()
		{
			if(rb.velocity.sqrMagnitude < 0.005f)
			{
				OnCollisionExit(null);
			}
			if(Vector3.Distance(camRig.position, transform.position) > audioManager.stopAudioDistance)
			{
				OnCollisionExit(null);
			}
		}

		protected virtual void OnCollisionEnter(Collision other)
		{
			VRTK_AudioMaterialData otherClip = null;
			VRTK_TerrainAudioMaterial audioTerrMat = other.gameObject.GetComponent<VRTK_TerrainAudioMaterial>();

			if(audioMaterialData == null || audioMaterialData.hitAudioClip == null)
			{
				return;
			}

			if(audioTerrMat != null)
			{
				otherClip = audioTerrMat.GetAudioMaterialData(other.contacts[0].point);
			}
			else
			{
				VRTK_PassiveAudioMaterial audioMat = other.gameObject.GetComponent<VRTK_PassiveAudioMaterial>();
				if(audioMat == null)
				{
					if(audioManager.fallbackAudioData != null)
					{
						otherClip = audioManager.fallbackAudioData;
					}
				}
				else
				{
					if(audioMat is VRTK_ActiveAudioMaterial)
					{
						if(gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
						{
							otherClip = audioMat.audioMaterialData;
						}
						else
						{
							return;
						}
					}
					else
					{
						otherClip = audioMat.audioMaterialData;
					}
				}
			}
			PlaySound(otherClip, other);
			if(slidingSource == null)
			{
				PlaySlideSound();
			}
		}

		protected virtual void OnCollisionExit(Collision other)
		{
			if(slidingSource != null)
			{
				VRTK_AudioManager.StopLoop(slidingSource);
				enabled = false;
				slidingSource = null;
			}
		}

		protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
		}

		protected virtual void PlaySlideSound()
		{
			slidingSource = VRTK_AudioManager.PlayLoopAtPoint(audioMaterialData.slideAudioClip, transform, 0.4f, 1);
			enabled = true;
		}

		protected virtual void PlaySound(VRTK_AudioMaterialData otherMat, Collision other)
		{
			float volume = Mathf.Abs(Mathf.Log10(other.relativeVelocity.sqrMagnitude));
			if(audioManager.useAudioMixing)
			{
				if(otherMat != null && otherMat.hitAudioClip != null)
				{
					float pitchMin = (audioMaterialData.minPitch + otherMat.minPitch) / 2;
					float pitchMax = (audioMaterialData.maxPitch + otherMat.maxPitch) / 2;
					float pitch = Random.Range(pitchMin, pitchMax);
					AudioClip mixedClip = MixAudioClips(audioMaterialData.hitAudioClip, otherMat.hitAudioClip);
					VRTK_AudioManager.PlayAtPoint(mixedClip, other.contacts[0].point, volume, pitch);
				}
				else
				{
					float pitch = Random.Range(audioMaterialData.minPitch, audioMaterialData.maxPitch);
					VRTK_AudioManager.PlayAtPoint(audioMaterialData.hitAudioClip, other.contacts[0].point, volume, pitch);
				}
			}
			else
			{
				if(otherMat != null && otherMat.hitAudioClip != null)
				{
					float pitch2 = Random.Range(otherMat.minPitch, otherMat.maxPitch);
					VRTK_AudioManager.PlayAtPoint(otherMat.hitAudioClip, other.contacts[0].point, volume, pitch2);
				}
				float pitch = Random.Range(audioMaterialData.minPitch, audioMaterialData.maxPitch);
				VRTK_AudioManager.PlayAtPoint(audioMaterialData.hitAudioClip, other.contacts[0].point, volume, pitch);
			}
		}

		protected virtual AudioClip MixAudioClips(AudioClip clip1, AudioClip clip2)
		{
			float[] data1;
			float[] data2;
			float[] mixedData;

			if(clip1 == clip2)
			{
				return clip1;
			}

			if(clip1.length < clip2.length)
			{
				data1 = new float[clip2.samples];
				clip2.GetData(data1, 0);
				data2 = new float[clip1.samples];
				clip2.GetData(data1, 0);
				mixedData = new float[clip2.samples];
			}
			else
			{
				data1 = new float[clip1.samples];
				clip1.GetData(data1, 0);
				data2 = new float[clip2.samples];
				clip2.GetData(data2, 0);
				mixedData = new float[clip1.samples];
			}

			for(int i = 0; i < data1.Length; i++)
			{
				if(data2.Length > i)
				{
					mixedData[i] = (data1[i] + data2[i]);
				}
				else
				{
					mixedData[i] = data1[i] / 2;
				}
			}

			AudioClip newClip = AudioClip.Create("MixedClip", mixedData.Length, 1, audioManager.sampleRate, false);
			newClip.SetData(mixedData, 0);

			return newClip;
		}
	}
}
