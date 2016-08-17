namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;
    public class VRTK_IndependentRadialMenuController : RadialMenuController
    {
        #region Variables
        public VRTK_InteractableObject eventsManager;
        public bool addMenuCollider = true;
        [Range (0, 10)]
        public float colliderRadiusMultiplier = 1.2f;
        public bool hideAfterExecution = true;

        [Range(-10, 10)]
        public float offsetMultiplier = 1.1f;
        public GameObject rotateTowards;

        private List<GameObject> interactingObjects; // Objects (controllers) that are either colliding with the menu or clicking the menu
        private List<GameObject> collidingObjects; // Just objects that are currently colliding with the menu or its parent

        private SphereCollider menuCollider;
        private Coroutine disableCoroutine;
        private Vector3 desiredColliderCenter;
        private Quaternion initialRotation;

        private bool isClicked = false;
        private bool waitingToDisableCollider = false;
        private int counter = 2;
        #endregion Variables

        #region Init and Unity Methods
        public void UpdateEventsManager()
        {
            VRTK_InteractableObject newEventsManager = transform.GetComponentInParent<VRTK_InteractableObject>();
            if (newEventsManager == null)
            {
                Debug.LogError("The radial menu must be a child of an interactable object or be set in the inspector!");
                return;
            }
            else if (newEventsManager != eventsManager) // Changed managers
            {
                if (eventsManager != null)
                { // Unsubscribe from the old events
                    OnDisable ();
                }

                eventsManager = newEventsManager;

                // Subscribe to new events
                OnEnable ();

                Object.Destroy (menuCollider);

                // Reset to initial state
                Initialize();
            }
        }

        protected override void Initialize()
        {
            if (eventsManager == null)
            {
                initialRotation = transform.localRotation;
                UpdateEventsManager ();
                return; // If all goes well in updateEventsManager, it will then call Initialize again, skipping this if statement
            }

            // Reset variables
            interactingObjects = new List<GameObject>();
            collidingObjects = new List<GameObject>();
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
                disableCoroutine = null;
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
                transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0)); // Rotation can mess up the calculations below

                SphereCollider collider = eventsManager.gameObject.AddComponent<SphereCollider>();

                // All of the transformVector's are to account for the scaling of the radial menu's 'panel' and the scaling of the eventsManager parent object
                collider.radius = (transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2) * colliderRadiusMultiplier * eventsManager.transform.InverseTransformVector(transform.GetChild (0).TransformVector(Vector3.one)).x;
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

        protected override void OnEnable ()
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
                Initialize ();
            }
        }

        protected override void OnDisable ()
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
        #endregion Init

        #region Event Listeners
        protected virtual void ObjectClicked(object sender, InteractableObjectEventArgs e)
        {
            base.DoClickButton(sender);
            isClicked = true;

            if (hideAfterExecution && !menu.executeOnUnclick)
            {
                ImmediatelyHideMenu(e);
            }
        }

        protected virtual void ObjectUnClicked(object sender, InteractableObjectEventArgs e)
        {
            base.DoUnClickButton(sender);
            isClicked = false;

            if ((hideAfterExecution || (collidingObjects.Count == 0 && menu.hideOnRelease)) && menu.executeOnUnclick)
            {
                ImmediatelyHideMenu(e);
            }
        }

        protected virtual void ObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            base.DoShowMenu(CalculateAngle(e.interactingObject), sender);
            collidingObjects.Add(e.interactingObject);

            interactingObjects.Add(e.interactingObject);
            if (addMenuCollider && menuCollider != null)
            {
                SetColliderState(true, e);
                if (disableCoroutine != null)
                {
                    StopCoroutine(disableCoroutine);
                }
            }
        }

        protected virtual void ObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            collidingObjects.Remove(e.interactingObject);
            if (((!menu.executeOnUnclick || !isClicked) && menu.hideOnRelease) || (Object)sender == this)
            {
                base.DoHideMenu(hideAfterExecution ,sender);

                interactingObjects.Remove(e.interactingObject);
                if (addMenuCollider && menuCollider != null)
                {
                    // In case there's any gap between the normal collider and the menuCollider, delay a bit. Cancelled if collider is re-entered
                    disableCoroutine = StartCoroutine(DelayedSetColliderEnabled(false, 0.25f, e));
                }
            }
        }

        protected override void AttemptHapticPulse (ushort strength)
        {
            if (interactingObjects.Count > 0)
            {
                SteamVR_Controller.Input ((int)interactingObjects[0].GetComponent<SteamVR_TrackedObject> ().index).TriggerHapticPulse (strength);
            }
        }
        #endregion Event Listeners

        #region Helpers
        protected float CalculateAngle(GameObject interactingObject)
        {
            Vector3 controllerPosition = interactingObject.transform.position;

            Vector3 toController = controllerPosition - transform.position;
            Vector3 projection = transform.position + Vector3.ProjectOnPlane(toController, transform.forward);

            float angle = 0;
            angle = Utilities.AngleSigned(transform.right * -1, projection - transform.position, transform.forward);

            // Ensure angle is positive
            if (angle < 0)
            {
                angle += 360.0f;
            }

            return angle;
        }

        private void ImmediatelyHideMenu(InteractableObjectEventArgs e)
        {
            ObjectUntouched(this, e);
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            SetColliderState(false, e); // Don't want to wait for this
        }

        private void SetColliderState(bool state, InteractableObjectEventArgs e)
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
                    foreach (var collider in colliders)
                    {
                        if (collider != menuCollider)
                        {
                            foreach(var controllerCollider in controllerColliders)
                            {
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

        IEnumerator DelayedSetColliderEnabled(bool enabled, float delay, InteractableObjectEventArgs e)
        {
            yield return new WaitForSeconds(delay);

            SetColliderState(enabled, e);

            StopCoroutine("delayedSetColliderEnabled");
        }
        #endregion Helpers

        #region Unity Methods
        private void Awake()
        {
            menu = GetComponent<RadialMenu> ();
        }

        private void Start()
        {
            Initialize ();
        }

        private void Update()
        {
            if (rotateTowards == null) // Backup
            {
                rotateTowards = GameObject.Find ("Camera (eye)");
                if (rotateTowards == null)
                {
                    Debug.LogWarning ("The IndependentRadialMenu could not automatically find an object to rotate towards.");
                }
            }

            if (menu.isShown) 
            {
                if (interactingObjects.Count > 0) // There's not really an event for the controller moving, so just update the position every frame
                {
                    base.DoChangeAngle (CalculateAngle (interactingObjects[0]), this);
                }

                if (rotateTowards != null)
                {
                    transform.rotation = Quaternion.LookRotation ((rotateTowards.transform.position - transform.position) * -1, Vector3.up) * initialRotation; // Face the target, but maintain initial rotation
                }
            }
        }

        private void FixedUpdate()
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
        #endregion Unity Methods
    }
}
