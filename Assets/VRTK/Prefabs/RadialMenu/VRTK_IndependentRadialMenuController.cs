// Independent Radial Menu|Prefabs|0120
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Allows the RadialMenu to be anchored to any object, not just a controller.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/RadialMenu/RadialMenu` prefab as a child of the GameObject to associate the Radial Menu with.
    ///  * Position and scale the menu by adjusting the transform of the `RadialMenu` empty.
    ///  * Replace `VRTK_RadialMenuController` with `VRTK_IndependentRadialMenuController` that is located on the `RadialMenu/RadialMenuUI/Panel` GameObject.
    ///  * Ensure the parent object has the `VRTK_InteractableObject` script.
    ///  * Verify that `Is Usable` and `Hold Button to Use` are both checked on the `VRTK_InteractableObject`.
    ///  * Attach `VRTK_InteractTouch` and `VRTK_InteractUse` scripts to the objects that will activate the Radial Menu (e.g. the Controllers).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.
    /// </example>
    public class VRTK_IndependentRadialMenuController : VRTK_RadialMenuController
    {
        [Tooltip("If the RadialMenu is the child of an object with VRTK_InteractableObject attached, this will be automatically obtained. It can also be manually set.")]
        public VRTK_InteractableObject eventsManager;
        [Tooltip("Whether or not the script should dynamically add a SphereCollider to surround the menu.")]
        public bool addMenuCollider = true;
        [Tooltip("This times the size of the RadialMenu is the size of the collider.")]
        [Range(0, 10)]
        public float colliderRadiusMultiplier = 1.2f;
        [Tooltip("If true, after a button is clicked, the RadialMenu will hide.")]
        public bool hideAfterExecution = true;
        [Tooltip("How far away from the object the menu should be placed, relative to the size of the RadialMenu.")]
        [Range(-10, 10)]
        public float offsetMultiplier = 1.1f;
        [Tooltip("The object the RadialMenu should face towards. If left empty, it will automatically try to find the Headset Camera.")]
        public GameObject rotateTowards;

        protected List<GameObject> interactingObjects = new List<GameObject>(); // Objects (controllers) that are either colliding with the menu or clicking the menu
        protected HashSet<GameObject> collidingObjects = new HashSet<GameObject>(); // Just objects that are currently colliding with the menu or its parent
        protected SphereCollider menuCollider;
        protected Coroutine delayedSetColliderEnabledRoutine;
        protected Vector3 desiredColliderCenter;
        protected Quaternion initialRotation;
        protected bool isClicked = false;
        protected bool waitingToDisableCollider = false;
        protected int counter = 2;

        /// <summary>
        /// The UpdateEventsManager method is used to update the events within the menu controller.
        /// </summary>
        public virtual void UpdateEventsManager()
        {
            VRTK_InteractableObject newEventsManager = transform.GetComponentInParent<VRTK_InteractableObject>();
            if (newEventsManager == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_IndependentRadialMenuController", "VRTK_InteractableObject", "eventsManager", "the parent"));
                return;
            }
            else if (newEventsManager != eventsManager) // Changed managers
            {
                if (eventsManager != null)
                { // Unsubscribe from the old events
                    OnDisable();
                }

                eventsManager = newEventsManager;

                // Subscribe to new events
                OnEnable();

                Destroy(menuCollider);

                // Reset to initial state
                Initialize();
            }
        }

        protected override void Initialize()
        {
            if (eventsManager == null)
            {
                initialRotation = transform.localRotation;
                UpdateEventsManager();
                return; // If all goes well in updateEventsManager, it will then call Initialize again, skipping this if statement
            }

            // Reset variables
            interactingObjects.Clear();
            collidingObjects.Clear();
            if (delayedSetColliderEnabledRoutine != null)
            {
                StopCoroutine(delayedSetColliderEnabledRoutine);
            }
            isClicked = false;
            waitingToDisableCollider = false;
            counter = 2;

            if (transform.childCount == 0) // This means things haven't been properly initialized yet, will cause problems.
            {
                return;
            }

            float radius = (transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2) * offsetMultiplier;
            transform.localPosition = new Vector3(0, 0, radius);

            if (addMenuCollider)
            {
                gameObject.SetActive(false); // Just be sure it doesn't briefly flash
                transform.localScale = Vector3.one; // If this were left at zero it would ruin the transformations below

                Quaternion startingRot = transform.rotation;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // Rotation can mess up the calculations below

                SphereCollider collider = eventsManager.gameObject.AddComponent<SphereCollider>();

                // All of the transformVector's are to account for the scaling of the radial menu's 'panel' and the scaling of the eventsManager parent object
                collider.radius = (transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2) * colliderRadiusMultiplier * eventsManager.transform.InverseTransformVector(transform.GetChild(0).TransformVector(Vector3.one)).x;
                collider.center = eventsManager.transform.InverseTransformVector(transform.position - eventsManager.transform.position);

                collider.isTrigger = true;
                collider.enabled = false; // Want this to only activate when the menu is showing

                menuCollider = collider;
                desiredColliderCenter = collider.center;

                transform.rotation = startingRot;
            }

            if (!menu.isShown)
            {
                transform.localScale = Vector3.zero;
            }
            gameObject.SetActive(true);
        }

        protected override void Awake()
        {
            menu = GetComponent<VRTK_RadialMenu>();
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Start()
        {
            Initialize();
        }

        protected override void OnEnable()
        {
            if (eventsManager != null)
            {
                eventsManager.InteractableObjectUsed += ObjectClicked;
                eventsManager.InteractableObjectUnused += ObjectUnClicked;
                eventsManager.InteractableObjectTouched += ObjectTouched;
                eventsManager.InteractableObjectUntouched += ObjectUntouched;

                menu.FireHapticPulse += AttemptHapticPulse;
            }
            else
            {
                Initialize();
            }
        }

        protected override void OnDisable()
        {
            if (eventsManager != null)
            {
                eventsManager.InteractableObjectUsed -= ObjectClicked;
                eventsManager.InteractableObjectUnused -= ObjectUnClicked;
                eventsManager.InteractableObjectTouched -= ObjectTouched;
                eventsManager.InteractableObjectUntouched -= ObjectUntouched;

                menu.FireHapticPulse -= AttemptHapticPulse;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (rotateTowards == null) // Backup
            {
                Transform headset = VRTK_DeviceFinder.HeadsetTransform();
                if (headset)
                {
                    rotateTowards = headset.gameObject;
                }
                else
                {
                    VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION, "IndependentRadialMenu", "an object", "rotate towards"));
                }
            }

            if (menu.isShown)
            {
                if (interactingObjects.Count > 0) // There's not really an event for the controller moving, so just update the position every frame
                {
                    DoChangeAngle(CalculateAngle(interactingObjects[0]), this);
                }

                if (rotateTowards != null)
                {
                    transform.rotation = Quaternion.LookRotation((rotateTowards.transform.position - transform.position) * -1, Vector3.up) * initialRotation; // Face the target, but maintain initial rotation
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (waitingToDisableCollider)
            {
                if (counter == 0)
                {
                    menuCollider.enabled = false;
                    waitingToDisableCollider = false;

                    counter = 2;
                }
                else
                {
                    counter--;
                }
            }
        }

        protected override void AttemptHapticPulse(float strength)
        {
            if (interactingObjects.Count > 0)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(interactingObjects[0]), strength);
            }
        }

        protected virtual void ObjectClicked(object sender, InteractableObjectEventArgs e)
        {
            DoClickButton(sender);
            isClicked = true;

            if (hideAfterExecution && !menu.executeOnUnclick)
            {
                ImmediatelyHideMenu(e);
            }
        }

        protected virtual void ObjectUnClicked(object sender, InteractableObjectEventArgs e)
        {
            DoUnClickButton(sender);
            isClicked = false;

            if ((hideAfterExecution || (collidingObjects.Count == 0 && menu.hideOnRelease)) && menu.executeOnUnclick)
            {
                ImmediatelyHideMenu(e);
            }
        }

        protected virtual void ObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            DoShowMenu(CalculateAngle(e.interactingObject), sender);
            collidingObjects.Add(e.interactingObject);
            VRTK_SharedMethods.AddListValue(interactingObjects, e.interactingObject, true);
            if (addMenuCollider && menuCollider != null)
            {
                SetColliderState(true, e);
                if (delayedSetColliderEnabledRoutine != null)
                {
                    StopCoroutine(delayedSetColliderEnabledRoutine);
                }
            }
        }

        protected virtual void ObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            collidingObjects.Remove(e.interactingObject);
            if (((!menu.executeOnUnclick || !isClicked) && menu.hideOnRelease) || (Object)sender == this)
            {
                DoHideMenu(hideAfterExecution, sender);
                interactingObjects.Remove(e.interactingObject);
                if (addMenuCollider && menuCollider != null)
                {
                    // In case there's any gap between the normal collider and the menuCollider, delay a bit. Cancelled if collider is re-entered
                    delayedSetColliderEnabledRoutine = StartCoroutine(DelayedSetColliderEnabled(false, 0.25f, e));
                }
            }
        }

        protected virtual TouchAngleDeflection CalculateAngle(GameObject interactingObject)
        {
            Vector3 controllerPosition = interactingObject.transform.position;

            Vector3 toController = controllerPosition - transform.position;
            Vector3 projection = transform.position + Vector3.ProjectOnPlane(toController, transform.forward);

            float angle = 0;
            angle = AngleSigned(transform.right * -1, projection - transform.position, transform.forward);

            // Ensure angle is positive
            if (angle < 0)
            {
                angle += 360.0f;
            }

            return new TouchAngleDeflection(angle, 1);
        }

        protected virtual float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        protected virtual void ImmediatelyHideMenu(InteractableObjectEventArgs e)
        {
            ObjectUntouched(this, e);
            if (delayedSetColliderEnabledRoutine != null)
            {
                StopCoroutine(delayedSetColliderEnabledRoutine);
            }
            SetColliderState(false, e); // Don't want to wait for this
        }

        protected virtual void SetColliderState(bool state, InteractableObjectEventArgs e)
        {
            if (addMenuCollider && menuCollider != null)
            {
                if (state)
                {
                    menuCollider.enabled = true;
                    menuCollider.center = desiredColliderCenter;
                }
                else
                {
                    bool should = true;
                    Collider[] colliders = eventsManager.GetComponents<Collider>();
                    Collider[] controllerColliders = e.interactingObject.GetComponent<VRTK_InteractTouch>().ControllerColliders();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Collider collider = colliders[i];
                        if (collider != menuCollider)
                        {
                            for (int j = 0; j < controllerColliders.Length; j++)
                            {
                                Collider controllerCollider = controllerColliders[j];
                                if (controllerCollider.bounds.Intersects(collider.bounds))
                                {
                                    should = false;
                                }
                            }
                        }
                    }

                    if (should)
                    {
                        menuCollider.center = new Vector3(100000000.0f, 100000000.0f, 100000000.0f); // This needs to be done to get OnTriggerExit() to fire, unfortunately
                        waitingToDisableCollider = true; // Need to give other things time to realize that they're not colliding with this anymore, so do it a couple FixedUpdates
                    }
                    else
                    {
                        menuCollider.enabled = false;
                    }
                }
            }
        }

        protected virtual IEnumerator DelayedSetColliderEnabled(bool enabled, float delay, InteractableObjectEventArgs e)
        {
            yield return new WaitForSeconds(delay);

            SetColliderState(enabled, e);
        }
    }
}