// Interactive Haptics|Interactions|30101
namespace VRTK
{
    using UnityEngine;
    using VRTK.Controllables;

    /// <summary>
    /// The VRTK_InteractiveHaptics script is attached on the same GameObject as an a VRTK_BaseControllable subclass. This includes artifical and physics based interactions. VRTK_InteractiveHaptics provides customizable haptic feedback curves for more realistic interactions over the fixed range of the VRTK_BaseControllable object.
    /// </summary>
    /// <remarks>
    ///  * Add this script to the same game object on which a subclass of `VRTK_BaseControllable` resides. This script uses the `VRTK_BaseControllable` subclass to determine the range over which it will operate.
    ///  * Optionally specify a `VRTK_InteractableObject`. This will be taken from the parent object at runtime unless it is specified. This object will be used to determine which controller is interacting with the object while using the `While Grabbing` `InteractionType`.
    ///  </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractiveHaptics")]
    public class VRTK_InteractiveHaptics : MonoBehaviour
    {
        /// <summary>
        /// The types of interaction that can be performed with Interactive Haptics.
        /// </summary>
        public enum InteractionType
        {
            WhileGrabbing,
            WhileTouching
        }

        [Header("Haptic Response")]

        [Tooltip("The type of interaction on which to apply this curve. If multiple interactions are needed, add multiple InteractiveHaptics components.")]
        public InteractionType interactionType;

        [Tooltip("Denotes the curve in which the normalized input will be evaluated. The output of the curve is the strength of the haptic feedback from 0 to 1.")]
        public AnimationCurve strengthCurve;

        [Tooltip("Denotes the curve in which the normalized input will be evaluated. The output of the curve is the interval between fastestPulseInterval and slowestPulseInterval represented by the curve.")]
        public AnimationCurve pulseIntervalCurve;
        
        [Tooltip("The fastest possible pulse interval. Maps to the upper boundary (1) of the pulse interval curve. This value should be the lower of the two as a lower number represents less time and more frequent intervals.")]
        public float fastestPulseInterval;

        [Tooltip("The slowest possible pulse interval. Maps to the lower boundary (0) of the pulse interval curve. This value should be the higher of the two as a higher number represents more time and less frequent intervals.")]
        public float slowestPulseInterval;

        [Tooltip("The minimum change in the normalized value provided by the input class to effect haptic feedback. Since it's nearly impossible to hold an object completely still, this threshold helps prevent minor variations in the normalized value from triggering a pulse.")]
        public float sensitivityThreshold = 0.01f;

        [Header("Custom Settings")]

        [Tooltip("The VRTK_BaseControllable object to affect. If left blank, then the VRTK_BaseControllable subclass will need to be on the current GameObject")]
        public VRTK_BaseControllable objectToAffect;

        [Tooltip("The Interactable Object to initiate the haptics from. If this is left blank, then the Interactable Object will need to be on the parent GameObject.")]
        public VRTK_InteractableObject interactableObject;

        private VRTK_ControllerReference controller;
        private float lastPulseTime;
        private float lastNormalizedValue;

        protected virtual void OnEnable()
        {
            objectToAffect = (objectToAffect != null ? objectToAffect : GetComponent<VRTK_BaseControllable>());

            if(objectToAffect != null)
            {
                objectToAffect.ValueChanged += Interact;
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractiveHaptics", "VRTK_InteractableObject", "the same"));
            }
        }

        protected virtual void OnDisable()
        {
            if(objectToAffect != null)
            {
                objectToAffect.ValueChanged -= Interact;
            }

            if (interactableObject != null)
            {
                VRTK_InteractableObject.InteractionType start, stop;

                if (interactionType == InteractionType.WhileGrabbing)
                {
                    stop = VRTK_InteractableObject.InteractionType.Ungrab;
                    start = VRTK_InteractableObject.InteractionType.Grab;
                }
                else
                {
                    stop = VRTK_InteractableObject.InteractionType.Untouch;
                    start = VRTK_InteractableObject.InteractionType.Touch;
                }

                interactableObject.UnsubscribeFromInteractionEvent(stop, StopHaptics);
                interactableObject.UnsubscribeFromInteractionEvent(start, StartHaptics);
            }
        }

        private void Start()
        {
            if(interactableObject == null)
            {
                interactableObject = GetComponentInParent<VRTK_InteractableObject>();
            }
            
            if (interactableObject != null)
            {
                VRTK_InteractableObject.InteractionType start, stop;

                if (interactionType == InteractionType.WhileGrabbing)
                {
                    stop = VRTK_InteractableObject.InteractionType.Ungrab;
                    start = VRTK_InteractableObject.InteractionType.Grab;
                }
                else
                {
                    stop = VRTK_InteractableObject.InteractionType.Untouch;
                    start = VRTK_InteractableObject.InteractionType.Touch;
                }

                interactableObject.SubscribeToInteractionEvent(stop, StopHaptics);
                interactableObject.SubscribeToInteractionEvent(start, StartHaptics);
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractiveHaptics", "VRTK_InteractableObject", "the parent"));
            }
        }

        protected virtual void StopHaptics(object sender, InteractableObjectEventArgs e)
        {
            controller = null;
        }

        protected virtual void StartHaptics(object sender, InteractableObjectEventArgs e)
        {
            controller = VRTK_ControllerReference.GetControllerReference(e.interactingObject);
        }

        private void Interact(object sender, ControllableEventArgs e)
        {
            VRTK_ControllerReference controllerReference = null;

            if (controller != null)
            {
                controllerReference = controller;
            }
            else if (e.interactingTouchScript != null)
            {
                controllerReference = VRTK_ControllerReference.GetControllerReference(e.interactingTouchScript.gameObject);
            }

            if (controllerReference == null)
            {
                return;
            }

            float normalizedValueDelta = Mathf.Abs(lastNormalizedValue - e.normalizedValue);

            if(normalizedValueDelta >= sensitivityThreshold)
            {
                lastNormalizedValue = e.normalizedValue;

                float pulseStrength = strengthCurve.Evaluate(e.normalizedValue);                
                float pulseInterval = slowestPulseInterval + ((fastestPulseInterval - slowestPulseInterval) * pulseIntervalCurve.Evaluate(e.normalizedValue));

                if(Time.time - lastPulseTime >= pulseInterval)
                {
                    VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, pulseStrength);
                    lastPulseTime = Time.time;
                }
            }
        }
    }
}