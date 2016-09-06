using UnityEngine;
using VRTK;

//=============================================================================
//
// Purpose: Add animations and sound effects to interactable game world objects
//
// This script should be attached to the main object, providing individual
// targets for each animation or sound (generally other game objects attached
// to the main game object).
//
// The duration of the animation, set for both starting and stopping the "use",
// affects each animation target set. There are different animation
// target transforms that can be set, if an animation target transform is not
// defined (null) the related animation will be disabled.
//
// Supported animation effects played if starting/stopping use are:
// - position offset: shift a transform by the given offset
// - spin: make a transform rotate around an axis with the given speed
// - scaling: shift a transform to the given value (optional pulse effect)
// - light: set a light intensity to the given value (optional pulse effect)
// "Idle" means the object is not in use, "use" means the opposite.
//
// Also sound effects can be triggered by events:
// - on start using (once)
// - on using (continuous loop)
// - on object hit while in use (once)
// - on stop using (once)
// If no audio source is provided no sound effect is triggered.
//
// Rumble effects (if not zero) can be triggered by events:
// - on using (continuous loop)
// - on object hit while in use (once)
//
// This script adds also the possibility to hide the pointer when the object is
// grabbed, or when it is grabbed and in use.
//=============================================================================
public class AnimatedInteractableObject : VRTK_InteractableObject
{
    public enum PointerDisableMode
    {
        DontDisable,
        OnGrab,
        OnUse,
    }

    [Tooltip("Disable the controller pointer on grab or on use")]
    public PointerDisableMode pointerDisableMode = PointerDisableMode.DontDisable;

    [Header("Animation")]
    [Tooltip("Duration of the activation/deactivation animation")]
    public float animationDuration = 1.0f;

    [Header("Position offset")]
    [Tooltip("Target transform for animation\nIf not set it will be disabled")]
    public Transform offsetTransform;
    public Vector3 idleOffset = Vector3.zero;
    public Vector3 useOffset = Vector3.zero;

    [Header("Spin")]
    [Tooltip("Target transform for animation\nIf not set it will be disabled")]
    public Transform spinTransform;
    public float idleSpinSpeed = 0f;
    public float useSpinSpeed = 360f;
    public Vector3 spinAxis = Vector3.forward;


    [Header("Scaling")]
    [Tooltip("Target transform for animation\nIf not set it will be disabled")]
    public Transform scaleTransform;
    public Vector3 idleScale = Vector3.one;
    public Vector3 useScale = Vector3.one;
    public Vector3 pulseScaleMagnitude = Vector3.zero;
    public float pulseScaleFrequency = 1.0f;

    [Header("Light")]
    [Tooltip("Target light for animation\nIf not set it will be disabled")]
    public Light controlledLight;
    public float idleLightIntensity = 0f;
    public float useLightIntensity = 0f;
    public float pulseLightMagnitude = 0f;
    public float pulseLightFrequency = 1.0f;

    [Header("Sound effects")]
    [Tooltip("Target sound source for events\nIf not set sounds will be disabled")]
    public AudioSource soundSource;
    public AudioClip soundStartUsing;
    public AudioClip soundStopUsing;
    public AudioClip soundUsing;
    public AudioClip soundHit;

    [Header("Rumble effects")]
    [Tooltip("Continuous rumble effect while in use")]
    [Range(0, 3999)]
    public ushort rumbleUsing = 0;
    [Tooltip("Rumble effect strength when\nan object is hit (while in use)")]
    [Range(0, 3999)]
    public ushort rumbleHit = 0;


    // Time at which the animation starts
    protected float animStartTime = 0.0f;

    // Internal variables used for animation (they could be used by derived scripts)

    protected Vector3 initPos = Vector3.zero;
    protected Vector3 endOffset = Vector3.zero;
    protected Vector3 startOffset = Vector3.zero;

    protected float currSpinSpeed = 0f;
    protected float endSpinSpeed = 0f;
    protected float startSpinSpeed = 0f;

    protected Vector3 endScale = Vector3.one;
    protected Vector3 startScale = Vector3.one;

    protected float startLightIntensity = 0f;
    protected float endLightIntensity = 0f;
    protected float currLightIntensity = 0f;

    // Current rumble effect strength.
    // Hold the value of one of the "Rumble effects" set in the Inspector
    private ushort currRumble = 0;


    // Store the state of the pointer before changing it
    private bool controllerHadPointerEnabled = false;

    protected override void Start()
    {
        base.Start();
        animStartTime = Time.time;

        if (offsetTransform != null)
        {
            initPos = offsetTransform.localPosition;
            offsetTransform.localPosition += idleOffset;
            startOffset = idleOffset;
            endOffset = idleOffset;
        }
        if (scaleTransform != null)
        {
            scaleTransform.localScale = idleScale;
            startScale = idleScale;
            endScale = idleScale;
        }
        if (spinTransform != null)
        {
            startSpinSpeed = idleSpinSpeed;
            endSpinSpeed = idleSpinSpeed;
            currSpinSpeed = idleSpinSpeed;
        }
        if (controlledLight != null)
        {
            startLightIntensity = idleLightIntensity;
            endLightIntensity = idleLightIntensity;
            currLightIntensity = idleLightIntensity;
        }
    }



    protected virtual void TriggerAnimStartUsing()
    {
        animStartTime = Time.time;
        if (offsetTransform != null)
        {
            startOffset = offsetTransform.localPosition - initPos;
            endOffset = useOffset;
        }
        if (spinTransform != null)
        {
            startSpinSpeed = currSpinSpeed;
            endSpinSpeed = useSpinSpeed;
        }
        if (scaleTransform != null)
        {
            startScale = scaleTransform.localScale;
            endScale = useScale;
        }
        if (controlledLight != null)
        {
            startLightIntensity = controlledLight.intensity;
            endLightIntensity = useLightIntensity;
        }
    }

    protected virtual void TriggerAnimStopUsing()
    {
        animStartTime = Time.time;
        if (offsetTransform != null)
        {
            startOffset = offsetTransform.localPosition - initPos;
            endOffset = idleOffset;
        }
        if (spinTransform != null)
        {
            startSpinSpeed = currSpinSpeed;
            endSpinSpeed = idleSpinSpeed;
        }
        if (scaleTransform != null)
        {
            startScale = scaleTransform.localScale;
            endScale = idleScale;
        }
        if (controlledLight != null)
        {
            startLightIntensity = controlledLight.intensity;
            endLightIntensity = idleLightIntensity;
        }
    }

    protected void TriggeSoundStartUsing()
    {
        if (soundSource != null && soundStartUsing != null)
        {
            soundSource.Stop();
            soundSource.loop = false;
            soundSource.clip = soundStartUsing;
            soundSource.Play();
        }
    }

    protected void TriggerSoundStopUsing()
    {
        if (soundSource != null)
        {
            soundSource.Stop();
            soundSource.loop = false;
            if (soundStopUsing != null)
            {
                soundSource.clip = soundStopUsing;
                soundSource.Play();
            }
        }
    }

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        TriggerAnimStartUsing();
        TriggeSoundStartUsing();
        if (pointerDisableMode == PointerDisableMode.OnUse && grabbingObject != null)
        {
            // disable teleport (save its state to restore it when ungrabbed)
            VRTK_WorldPointer worldPointer = grabbingObject.GetComponent<VRTK_WorldPointer>();
            if (worldPointer != null && worldPointer.enabled)
            {
                controllerHadPointerEnabled = true;
                worldPointer.enabled = false;
            }
        }
    }

    public override void StopUsing(GameObject usingObject)
    {
        bool enablePointer = controllerHadPointerEnabled && (pointerDisableMode == PointerDisableMode.OnUse);
        if (enablePointer && grabbingObject != null)
        {
            // restore teleport if it was enabled
            VRTK_WorldPointer worldPointer = grabbingObject.GetComponent<VRTK_WorldPointer>();
            if (worldPointer != null)
            {
                worldPointer.enabled = true;
            }
        }
        base.StopUsing(usingObject);
        TriggerAnimStopUsing();
        TriggerSoundStopUsing();
    }

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        base.Grabbed(currentGrabbingObject);
        if (pointerDisableMode==PointerDisableMode.OnGrab && grabbingObject != null)
        {
            // disable teleport (save its state to restore it when ungrabbed)
            VRTK_WorldPointer worldPointer = grabbingObject.GetComponent<VRTK_WorldPointer>();
            if (worldPointer != null && worldPointer.enabled)
            {
                controllerHadPointerEnabled = true;
                worldPointer.enabled = false;
            }
        }

    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
        if (useOnlyIfGrabbed)
        {
            TriggerAnimStopUsing();
            TriggerSoundStopUsing();
        }
        bool enablePointer = controllerHadPointerEnabled && (pointerDisableMode == PointerDisableMode.OnGrab) || (pointerDisableMode == PointerDisableMode.OnUse && useOnlyIfGrabbed);
        if (enablePointer && previousGrabbingObject != null)
        {
            // restore teleport if it was enabled
            VRTK.VRTK_WorldPointer worldPointer = previousGrabbingObject.GetComponent<VRTK.VRTK_WorldPointer>();
            if (worldPointer != null) worldPointer.enabled = true;
        }
    }

    protected void Rumble(ushort strength)
    {
        if (grabbingObject != null)
        {
            grabbingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse(strength);
        }
        currRumble = strength;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        TriggerCollisionEffect(collision.gameObject);
    }

    protected virtual void TriggerCollisionEffect(GameObject colliding_obj)
    {
        if (!IsUsing())
        {
            return;
        }

        Rumble(rumbleHit);

        if (soundSource != null && soundHit != null && soundSource.clip == soundUsing)
        {
            soundSource.Stop();
            soundSource.loop = false;
            soundSource.clip = soundHit;
            soundSource.Play();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        float t = 1.0f;
        if (animationDuration > 0.0)
        {
            t = (Time.time - animStartTime) / animationDuration;
        }

        if (offsetTransform != null)
        {
            Vector3 posOffset = Vector3.Lerp(startOffset, endOffset, t);
            offsetTransform.localPosition = initPos + posOffset;
        }

        if (spinTransform != null)
        {
            if (currSpinSpeed != endSpinSpeed)
            {
                currSpinSpeed = Mathf.Lerp(startSpinSpeed, endSpinSpeed, t);
            }
            if (currSpinSpeed != 0) spinTransform.Rotate(spinAxis, currSpinSpeed * Time.deltaTime);
        }

        if (scaleTransform != null)
        {
            if (startScale == Vector3.zero && endScale == Vector3.zero)
            {
                // avoid zero scaling
                scaleTransform.localScale = Vector3.one * 0.0001f;
            }
            else
            {
                Vector3 newScale = Vector3.Lerp(startScale, endScale, t);
                if (pulseScaleMagnitude != Vector3.zero && IsUsing())
                {
                    newScale = newScale + pulseScaleMagnitude * Mathf.Sin(Time.time * 2.0f * Mathf.PI * pulseScaleFrequency);
                }
                scaleTransform.localScale = newScale;
            }
        }

        if (controlledLight != null)
        {
            if (currLightIntensity != endLightIntensity)
            {
                currLightIntensity = Mathf.Lerp(startLightIntensity, endLightIntensity, t);
            }

            float light_intensity = currLightIntensity;
            if (pulseLightMagnitude != 0f && currLightIntensity != idleLightIntensity)
            {
                light_intensity = currLightIntensity + pulseLightMagnitude * Mathf.Sin(Time.time * 2.0f * Mathf.PI * pulseLightFrequency);
            }
            controlledLight.intensity = light_intensity;
        }

        if (soundSource != null)
        {
            if (!IsGrabbed() || !IsUsing())
            {
                if (soundSource.isPlaying && soundSource.clip != soundStopUsing) soundSource.Stop();
            }
            else if (soundUsing != null)
            {
                if (!soundSource.isPlaying)
                {
                    soundSource.clip = soundUsing;
                    soundSource.loop = true;
                    soundSource.Play();
                }
            }
        }

        if (IsUsing() && (rumbleUsing > 0 || currRumble == rumbleUsing))
        {
            Rumble(rumbleUsing);
        }
    }
}
