// Interact Near Touch|Interactors|30030
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Determines if a GameObject can initiate a near touch with an Interactable Object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractTouch` - The touch component to determine the actual interacting GameObject that will deal with the near touch interaction. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_InteractNearTouch` script on either:
    ///    * The Interact Touch GameObject.
    ///    * Any other scene GameObject and provide a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter of this script.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactors/VRTK_InteractNearTouch")]
    public class VRTK_InteractNearTouch : MonoBehaviour
    {
        [Tooltip("The radius of the auto generated collider if a `Custom Collider Container` is not supplied.")]
        public float colliderRadius = 0.2f;
        [Tooltip("An optional GameObject that contains the compound colliders to represent the near touching object. If this is empty then the collider will be auto generated at runtime.")]
        public GameObject customColliderContainer;
        [Tooltip("The Interact Touch script to associate the near touches with. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;

        protected GameObject neartouchColliderContainer;
        protected List<GameObject> nearTouchedObjects = new List<GameObject>();
        protected VRTK_InteractNearTouchCollider interactNearTouchColliderScript;

        /// <summary>
        /// Emitted when a valid object is near touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerNearTouchInteractableObject;
        /// <summary>
        /// Emitted when a valid object is no longer being near touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerNearUntouchInteractableObject;

        public virtual void OnControllerNearTouchInteractableObject(ObjectInteractEventArgs e)
        {
            VRTK_SharedMethods.AddListValue(nearTouchedObjects, e.target, true);

            if (ControllerNearTouchInteractableObject != null)
            {
                ControllerNearTouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerNearUntouchInteractableObject(ObjectInteractEventArgs e)
        {
            nearTouchedObjects.Remove(e.target);

            if (ControllerNearUntouchInteractableObject != null)
            {
                ControllerNearUntouchInteractableObject(this, e);
            }
        }

        /// <summary>
        /// The GetNearTouchedObjects method returns all of the GameObjects that are currently being near touched.
        /// </summary>
        /// <returns>A list of GameObjects that are being near touched.</returns>
        public virtual List<GameObject> GetNearTouchedObjects()
        {
            return nearTouchedObjects;
        }

        /// <summary>
        /// The ForceNearTouch method will attempt to force the controller to near touch the given GameObject.
        /// </summary>
        /// <param name="obj">The GameObject to attempt to force near touch.</param>
        public virtual void ForceNearTouch(GameObject obj)
        {
            Collider objCollider = (obj != null ? obj.GetComponentInChildren<Collider>() : null);
            if (objCollider != null)
            {
                interactNearTouchColliderScript.StartNearTouch(objCollider);
            }
        }

        /// <summary>
        /// The ForceStopNearTouching method will stop the Interact Touch GameObject from near touching an Interactable Object even if the Interact Touch GameObject is physically touching the Interactable Object still.
        /// </summary>
        /// <param name="obj">An optional GameObject to only include in the force stop. If this is null then all near touched GameObjects will be force stopped.</param>
        public virtual void ForceStopNearTouching(GameObject obj = null)
        {
            if (obj != null)
            {
                Collider objCollider = (obj != null ? obj.GetComponentInChildren<Collider>() : null);
                if (objCollider != null)
                {
                    interactNearTouchColliderScript.EndNearTouch(objCollider);
                }
            }
            else
            {
                for (int i = 0; i < nearTouchedObjects.Count; i++)
                {
                    OnControllerNearUntouchInteractableObject(interactTouch.SetControllerInteractEvent(nearTouchedObjects[i]));
                }
            }
        }

        protected virtual void OnEnable()
        {
            nearTouchedObjects.Clear();
            interactTouch = (interactTouch != null ? interactTouch : GetComponentInParent<VRTK_InteractTouch>());
            if (interactTouch != null)
            {
                CreateNearTouchCollider();
                interactTouch.ControllerStartTouchInteractableObject += ControllerStartTouchInteractableObject;
                interactTouch.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
            }
        }

        protected virtual void OnDisable()
        {
            Destroy(neartouchColliderContainer);
            if (interactTouch != null)
            {
                interactTouch.ControllerStartTouchInteractableObject -= ControllerStartTouchInteractableObject;
                interactTouch.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
            }
        }

        protected virtual void ControllerStartTouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            ForceStopNearTouching(e.target);
        }

        protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (interactNearTouchColliderScript.GetNearTouchedObjects().Contains(e.target))
            {
                ForceNearTouch(e.target);
            }
        }

        protected virtual void CreateNearTouchCollider()
        {
            if (customColliderContainer == null)
            {
                neartouchColliderContainer = new GameObject();
                neartouchColliderContainer.transform.SetParent(interactTouch.transform);
                neartouchColliderContainer.transform.localPosition = Vector3.zero;
                neartouchColliderContainer.transform.localRotation = Quaternion.identity;
                neartouchColliderContainer.transform.localScale = interactTouch.transform.localScale;
            }
            else
            {
                neartouchColliderContainer = Instantiate(customColliderContainer);
                neartouchColliderContainer.transform.SetParent(interactTouch.transform);
                neartouchColliderContainer.transform.localPosition = customColliderContainer.transform.localPosition;
                neartouchColliderContainer.transform.localRotation = customColliderContainer.transform.localRotation;
                neartouchColliderContainer.transform.localScale = customColliderContainer.transform.localScale;
            }

            neartouchColliderContainer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Controller", "NearTouch", "CollidersContainer");

            Rigidbody attachedRigidbody = neartouchColliderContainer.GetComponentInChildren<Rigidbody>();
            if (attachedRigidbody == null)
            {
                attachedRigidbody = neartouchColliderContainer.AddComponent<Rigidbody>();
            }

            attachedRigidbody.isKinematic = true;
            attachedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Collider attachedCollider = neartouchColliderContainer.GetComponentInChildren<Collider>();
            if (attachedCollider == null)
            {
                SphereCollider attachedSphereCollider = neartouchColliderContainer.AddComponent<SphereCollider>();
                attachedSphereCollider.isTrigger = true;
                attachedSphereCollider.radius = colliderRadius;
            }
            else
            {
                attachedCollider.isTrigger = true;
            }

            interactNearTouchColliderScript = neartouchColliderContainer.AddComponent<VRTK_InteractNearTouchCollider>();
            interactNearTouchColliderScript.SetInteractNearTouch(this);

            neartouchColliderContainer.SetActive(true);
        }
    }

    public class VRTK_InteractNearTouchCollider : MonoBehaviour
    {
        protected VRTK_InteractNearTouch interactNearTouch;
        protected List<GameObject> nearTouchedObjects = new List<GameObject>();

        public virtual void SetInteractNearTouch(VRTK_InteractNearTouch givenInteractNearTouch)
        {
            interactNearTouch = givenInteractNearTouch;
        }

        public virtual List<GameObject> GetNearTouchedObjects()
        {
            return nearTouchedObjects;
        }

        public virtual void StartNearTouch(Collider collider)
        {
            VRTK_InteractableObject checkObject = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && validObject(checkObject))
            {
                if (checkObject != null)
                {
                    checkObject.StartNearTouching(interactNearTouch);
                }
                interactNearTouch.OnControllerNearTouchInteractableObject(interactNearTouch.interactTouch.SetControllerInteractEvent(collider.gameObject));
            }
        }

        public virtual void EndNearTouch(Collider collider)
        {
            VRTK_InteractableObject checkObject = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && validObject(checkObject))
            {
                if (checkObject != null)
                {
                    checkObject.StopNearTouching(interactNearTouch);
                }
                interactNearTouch.OnControllerNearUntouchInteractableObject(interactNearTouch.interactTouch.SetControllerInteractEvent(collider.gameObject));
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            StartNearTouch(collider);
            VRTK_SharedMethods.AddListValue(nearTouchedObjects, collider.gameObject, true);
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            EndNearTouch(collider);
            nearTouchedObjects.Remove(collider.gameObject);
        }

        protected virtual void OnEnable()
        {
            nearTouchedObjects.Clear();
        }

        protected virtual bool validObject(VRTK_InteractableObject checkObject)
        {
            return (checkObject == null || checkObject.IsValidInteractableController(interactNearTouch.interactTouch.gameObject, checkObject.allowedNearTouchControllers));
        }
    }
}