// Controller Tracked Collider|Interactors|30060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Provides a controller collider collection that follows the controller rigidbody via the physics system.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractTouch` - An Interact Touch script to determine which controller rigidbody to follow.
    ///
    /// **Optional Components:**
    ///  * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied in the same object hierarchy as the Interact Touch script if one is not provided via the `Controller Events` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ControllerTrackedCollider` script on any active scene GameObject except the Script Alias objects.
    ///  * Assign the controller to track by applying an Interact Touch to the relevant Script Alias and then providing that reference to the `Interact Touch` parameter on this script.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactors/VRTK_ControllerTrackedCollider")]
    public class VRTK_ControllerTrackedCollider : VRTK_SDKControllerReady
    {
        [Header("Tracked Controller Settings")]

        [Tooltip("The Interact Touch script to relate the tracked collider to.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("The maximum distance the collider object can be from the controller before it automatically snaps back to the same position.")]
        public float maxResnapDistance = 0.5f;
        [Tooltip("The button to press to activate the colliders on the tracked collider set. If `Undefined` then it will always be active.")]
        public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        [Header("Custom Settings")]

        [Tooltip("An optional Controller Events to use for listening to the button events. If this is left blank then it will attempt to be retrieved from the same controller as the `Interact Touch` parameter.")]
        public VRTK_ControllerEvents controllerEvents;

        protected VRTK_TrackedController trackedController;
        protected VRTK_ControllerReference controllerReference;
        protected Rigidbody trackedRigidbody;
        protected bool createRigidbody;
        protected Collider[] trackedColliders = new Collider[0];
        protected GameObject customColliderContainer;
        protected bool createColliders;
        protected VRTK_RigidbodyFollow rigidbodyFollow;
        protected bool createRigidbodyFollow;
        protected VRTK_ControllerEvents.ButtonAlias subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        /// <summary>
        /// The ToggleColliders method toggles the collision state of the tracked colliders.
        /// </summary>
        /// <param name="state">If `true` then the tracked colliders will be able to affect other Rigidbodies.</param>
        public virtual void ToggleColliders(bool state)
        {
            if (!state && activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                return;
            }

            for (int i = 0; i < trackedColliders.Length; i++)
            {
                trackedColliders[i].isTrigger = !state;
            }
        }

        /// <summary>
        /// The TrackedColliders method returns an array of the tracked colliders.
        /// </summary>
        /// <returns>A Collider array of the tracked colliders.</returns>
        public virtual Collider[] TrackedColliders()
        {
            return trackedColliders;
        }

        protected override void OnEnable()
        {
            if (interactTouch == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_PARAMETER, "VRTK_ControllerTrackedCollider", "VRTK_InteractTouch", "Interact Touch"));
            }
            else
            {
                VRTK_SharedMethods.AddDictionaryValue(VRTK_ObjectCache.registeredTrackedColliderToInteractTouches, interactTouch, this);
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ManageActivationListeners(false);
            Cleanup(false);
            VRTK_ObjectCache.registeredTrackedColliderToInteractTouches.Remove(interactTouch);
        }

        protected override void ControllerReady(VRTK_ControllerReference passedControllerReference)
        {
            if (sdkManager != null && sdkManager.loadedSetup != null && gameObject.activeInHierarchy && VRTK_ControllerReference.IsValid(passedControllerReference))
            {
                Cleanup(true);
                controllerReference = passedControllerReference;
                controllerEvents = (controllerEvents == null ? controllerReference.scriptAlias.GetComponentInChildren<VRTK_ControllerEvents>() : controllerEvents);
                ManageActivationListeners(true);
                SetupRigidbody();
                SetupColliders();
                SetupFollower();
                ToggleColliders(activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined);
            }
        }

        protected virtual void Cleanup(bool immediate)
        {
            if (createRigidbody)
            {
                DestroyObject(trackedRigidbody, immediate);
            }

            trackedColliders = new Collider[0];
            if (createColliders)
            {
                DestroyObject(customColliderContainer, immediate);
            }

            if (createRigidbodyFollow)
            {
                DestroyObject(rigidbodyFollow, immediate);
            }
        }

        protected virtual void DestroyObject(Object toDestroy, bool immediate)
        {
            if (immediate)
            {
                DestroyImmediate(toDestroy);
            }
            else
            {
                Destroy(toDestroy);
            }
        }

        protected virtual void ManageActivationListeners(bool state)
        {
            if (controllerEvents != null)
            {
                if (subscribedActivationButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || activationButton != subscribedActivationButton))
                {
                    controllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, true, DoActivationPress);
                    controllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, false, DoActivationRelease);
                    subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                }

                if (state && activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    controllerEvents.SubscribeToButtonAliasEvent(activationButton, true, DoActivationPress);
                    controllerEvents.SubscribeToButtonAliasEvent(activationButton, false, DoActivationRelease);
                    subscribedActivationButton = activationButton;
                }
            }
        }

        protected virtual void DoActivationPress(object sender, ControllerInteractionEventArgs e)
        {
            ToggleColliders(true);
        }

        protected virtual void DoActivationRelease(object sender, ControllerInteractionEventArgs e)
        {
            ToggleColliders(false);
        }

        protected virtual void SetupRigidbody()
        {
            createRigidbody = false;
            trackedRigidbody = GetComponent<Rigidbody>();
            if (trackedRigidbody == null)
            {
                createRigidbody = true;
                trackedRigidbody = gameObject.AddComponent<Rigidbody>();
                trackedRigidbody.useGravity = false;
                trackedRigidbody.drag = 0f;
                trackedRigidbody.angularDrag = 0f;
            }
        }

        protected virtual void SetupColliders()
        {
            createColliders = false;
            Collider[] foundColliders = VRTK_SharedMethods.GetCollidersInGameObjects(new GameObject[] { gameObject }, true, true);
            if (foundColliders.Length == 0)
            {
                Object defaultColliderPrefab = Resources.Load(VRTK_SDK_Bridge.GetControllerDefaultColliderPath(controllerReference.hand));
                if (defaultColliderPrefab == null)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "default collider prefab", "Controller SDK"));
                    return;
                }
                createColliders = true;
                customColliderContainer = Instantiate(defaultColliderPrefab) as GameObject;
                customColliderContainer.transform.SetParent(transform);
                customColliderContainer.transform.localPosition = Vector3.zero;
                customColliderContainer.transform.localRotation = Quaternion.identity;
                customColliderContainer.transform.localScale = Vector3.one;
                customColliderContainer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Controller", "TrackedCollidersContainer");
                foundColliders = VRTK_SharedMethods.GetCollidersInGameObjects(new GameObject[] { gameObject }, true, true);
            }
            trackedColliders = foundColliders;

            Collider[] touchColliders = interactTouch.ControllerColliders();

            for (int touchColliderIndex = 0; touchColliderIndex < touchColliders.Length; touchColliderIndex++)
            {
                for (int trackedColliderIndex = 0; trackedColliderIndex < trackedColliders.Length; trackedColliderIndex++)
                {
                    Physics.IgnoreCollision(touchColliders[touchColliderIndex], trackedColliders[trackedColliderIndex], true);
                }
            }
        }

        protected virtual void SetupFollower()
        {
            createRigidbodyFollow = false;
            rigidbodyFollow = GetComponent<VRTK_RigidbodyFollow>();
            if (rigidbodyFollow == null)
            {
                createRigidbodyFollow = true;
                rigidbodyFollow = gameObject.AddComponent<VRTK_RigidbodyFollow>();
            }
            rigidbodyFollow.gameObjectToFollow = interactTouch.gameObject;
            rigidbodyFollow.trackMaxDistance = maxResnapDistance;
            rigidbodyFollow.movementOption = VRTK_RigidbodyFollow.MovementOption.Track;
        }
    }
}