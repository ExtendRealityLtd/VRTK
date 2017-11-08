﻿// Object Touch Auto Interact|Interactables|35050
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Allows for Interact Grab or Interact Use interactions to automatically happen upon touching an Interactable Object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Interactable Object` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ObjectTouchAutoInteract` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Interactable Object` parameter of this script.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/VRTK_ObjectTouchAutoInteract")]
    public class VRTK_ObjectTouchAutoInteract : MonoBehaviour
    {
        /// <summary>
        /// Situation when auto interaction can occur.
        /// </summary>
        public enum AutoInteractions
        {
            /// <summary>
            /// Auto interaction can never occur on touch.
            /// </summary>
            Never,
            /// <summary>
            /// Auto interaction will occur on touch even if the specified interaction button is not held down.
            /// </summary>
            NoButtonHeld,
            /// <summary>
            /// Auto interaction will only occur on touch if the specified interaction button is held down.
            /// </summary>
            ButtonHeld
        }

        [Header("Auto Grab")]

        [Tooltip("Determines when a grab on touch should occur.")]
        public AutoInteractions grabOnTouchWhen = AutoInteractions.Never;
        [Tooltip("After being ungrabbed, another auto grab on touch can only occur after this time.")]
        public float regrabDelay = 0.1f;
        [Tooltip("If this is checked then the grab on touch check will happen every frame and not only on the first touch of the Interactable Object.")]
        public bool continuousGrabCheck = false;

        [Header("Auto Use")]

        [Tooltip("Determines when a use on touch should occur.")]
        public AutoInteractions useOnTouchWhen = AutoInteractions.Never;
        [Tooltip("After being unused, another auto use on touch can only occur after this time.")]
        public float reuseDelay = 0.1f;
        [Tooltip("If this is checked then the use on touch check will happen every frame and not only on the first touch of the Interactable Object.")]
        public bool continuousUseCheck = false;

        [Header("Custom Settings")]

        [Tooltip("The Interactable Object that the auto interaction will occur on. If this is blank then the script must be on the same GameObject as the Interactable Object script.")]
        public VRTK_InteractableObject interactableObject;

        protected float regrabTimer;
        protected float reuseTimer;
        protected List<GameObject> touchers;

        protected virtual void OnEnable()
        {
            regrabTimer = 0f;
            reuseTimer = 0f;
            touchers = new List<GameObject>();

            interactableObject = (interactableObject != null ? interactableObject : GetComponent<VRTK_InteractableObject>());

            if (interactableObject != null)
            {
                interactableObject.InteractableObjectTouched += InteractableObjectTouched;
                interactableObject.InteractableObjectUntouched += InteractableObjectUntouched;
                interactableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
                interactableObject.InteractableObjectUnused += InteractableObjectUnused;
            }
        }

        protected virtual void OnDisable()
        {
            if (interactableObject != null)
            {
                interactableObject.InteractableObjectTouched -= InteractableObjectTouched;
                interactableObject.InteractableObjectUntouched -= InteractableObjectUntouched;
                interactableObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
                interactableObject.InteractableObjectUnused -= InteractableObjectUnused;
            }
        }

        protected virtual void Update()
        {
            if (interactableObject != null && (continuousGrabCheck || continuousUseCheck))
            {
                for (int i = 0; i < touchers.Count; i++)
                {
                    if (continuousGrabCheck)
                    {
                        CheckGrab(touchers[i]);
                    }
                    if (continuousUseCheck)
                    {
                        CheckUse(touchers[i]);
                    }
                }
            }
        }

        protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            ManageTouchers(e.interactingObject, true);
            CheckGrab(e.interactingObject);
            CheckUse(e.interactingObject);
        }

        protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            ManageTouchers(e.interactingObject, false);
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            regrabTimer = regrabDelay + Time.time;
        }


        protected virtual void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
        {
            reuseTimer = reuseDelay + Time.time;
        }

        protected virtual void ManageTouchers(GameObject interactingObject, bool add)
        {
            if (add && !touchers.Contains(interactingObject))
            {
                touchers.Add(interactingObject);
            }
            else if (!add && touchers.Contains(interactingObject))
            {
                touchers.Remove(interactingObject);
            }
        }

        protected virtual void CheckGrab(GameObject interactingObject)
        {
            if (grabOnTouchWhen != AutoInteractions.Never && regrabTimer < Time.time)
            {
                VRTK_InteractGrab interactGrabScript = interactingObject.GetComponentInChildren<VRTK_InteractGrab>();
                if (interactGrabScript != null && (grabOnTouchWhen == AutoInteractions.NoButtonHeld || (grabOnTouchWhen == AutoInteractions.ButtonHeld && interactGrabScript.IsGrabButtonPressed())))
                {
                    interactGrabScript.AttemptGrab();
                }
            }
        }

        protected virtual void CheckUse(GameObject interactingObject)
        {
            if (useOnTouchWhen != AutoInteractions.Never && reuseTimer < Time.time)
            {
                VRTK_InteractUse interactUseScript = interactingObject.GetComponentInChildren<VRTK_InteractUse>();
                if (interactUseScript != null && (useOnTouchWhen == AutoInteractions.NoButtonHeld || (useOnTouchWhen == AutoInteractions.ButtonHeld && interactUseScript.IsUseButtonPressed())))
                {
                    if (!interactableObject.holdButtonToUse && interactableObject.IsUsing())
                    {
                        interactableObject.ForceStopInteracting();
                    }
                    else
                    {
                        interactUseScript.AttemptUse();
                    }
                }
            }
        }
    }
}