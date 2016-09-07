using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_HeadsetFade))]
public class VRTK_HeadsetFade_UnityEvents : MonoBehaviour
{
    private VRTK_HeadsetFade hf;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<HeadsetFadeEventArgs> { };
    public UnityObjectEvent OnHeadsetFadeStart;
    public UnityObjectEvent OnHeadsetFadeComplete;
    public UnityObjectEvent OnHeadsetUnfadeStart;
    public UnityObjectEvent OnHeadsetUnfadeComplete;

    private void SetHeadsetFade()
    {
        if (hf == null)
        {
            hf = GetComponent<VRTK_HeadsetFade>();
        }
    }

    private void OnEnable()
    {
        SetHeadsetFade();
        if (hf == null)
        {
            Debug.LogError("The VRTK_HeadsetFade_UnityEvents script requires to be attached to a GameObject that contains a VRTK_HeadsetFade script");
            return;
        }

        hf.HeadsetFadeStart += HeadsetFadeStart;
        hf.HeadsetFadeComplete += HeadsetFadeComplete;
        hf.HeadsetUnfadeStart += HeadsetUnfadeStart;
        hf.HeadsetUnfadeComplete += HeadsetUnfadeComplete;
    }

    private void HeadsetFadeStart(object o, HeadsetFadeEventArgs e)
    {
        OnHeadsetFadeStart.Invoke(e);
    }

    private void HeadsetFadeComplete(object o, HeadsetFadeEventArgs e)
    {
        OnHeadsetFadeComplete.Invoke(e);
    }

    private void HeadsetUnfadeStart(object o, HeadsetFadeEventArgs e)
    {
        OnHeadsetUnfadeStart.Invoke(e);
    }

    private void HeadsetUnfadeComplete(object o, HeadsetFadeEventArgs e)
    {
        OnHeadsetUnfadeComplete.Invoke(e);
    }

    private void OnDisable()
    {
        if (hf == null)
        {
            return;
        }

        hf.HeadsetFadeStart -= HeadsetFadeStart;
        hf.HeadsetFadeComplete -= HeadsetFadeComplete;
        hf.HeadsetUnfadeStart -= HeadsetUnfadeStart;
        hf.HeadsetUnfadeComplete -= HeadsetUnfadeComplete;
    }
}