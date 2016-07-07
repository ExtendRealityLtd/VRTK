//=====================================================================================
//
// Purpose: Provide a mechanism for determining if a game world object is interactable
//
// This script should be attached to any object that needs touch, use or grab
//
// An optional highlight color can be set to change the object's appearance if it is
// invoked.
//
//=====================================================================================
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public struct InteractableObjectEventArgs
    {
        public GameObject interactingObject;
    }

    public delegate void InteractableObjectEventHandler(object sender, InteractableObjectEventArgs e);

    public class VRTK_InteractableObject : MonoBehaviour
    {
        public enum GrabAttachType
        {
            Fixed_Joint,
            Spring_Joint,
            Track_Object,
            Child_Of_Controller
        }

        public enum AllowedController
        {
            Both,
            Left_Only,
            Right_Only
        }

        [Header("Touch Interactions", order = 1)]
        public bool highlightOnTouch = false;
        public Color touchHighlightColor = Color.clear;
        public Vector2 rumbleOnTouch = Vector2.zero;
        public AllowedController allowedTouchControllers = AllowedController.Both;

        [Header("Grab Interactions", order = 2)]
        public bool isGrabbable = false;
        public bool isDroppable = true;
        public bool isSwappable = true;
        public bool holdButtonToGrab = true;
        public Vector2 rumbleOnGrab = Vector2.zero;
        public AllowedController allowedGrabControllers = AllowedController.Both;
        public bool precisionSnap;
        public Transform rightSnapHandle;
        public Transform leftSnapHandle;

        [Header("Grab Mechanics", order = 3)]
        public GrabAttachType grabAttachMechanic = GrabAttachType.Fixed_Joint;
        public float detachThreshold = 500f;
        public float springJointStrength = 500f;
        public float springJointDamper = 50f;
        public float throwMultiplier = 1f;
        public float onGrabCollisionDelay = 0f;

        [Header("Use Interactions", order = 4)]
        public bool isUsable = false;
        public bool holdButtonToUse = true;
        public bool pointerActivatesUseAction = false;
        public Vector2 rumbleOnUse = Vector2.zero;
        public AllowedController allowedUseControllers = AllowedController.Both;

        public event InteractableObjectEventHandler InteractableObjectTouched;
        public event InteractableObjectEventHandler InteractableObjectUntouched;
        public event InteractableObjectEventHandler InteractableObjectGrabbed;
        public event InteractableObjectEventHandler InteractableObjectUngrabbed;
        public event InteractableObjectEventHandler InteractableObjectUsed;
        public event InteractableObjectEventHandler InteractableObjectUnused;

        protected Rigidbody rb;
        protected GameObject touchingObject = null;
        protected GameObject grabbingObject = null;
        protected GameObject usingObject = null;

        private int usingState = 0;
        private Dictionary<string, Color> originalObjectColours;

        private Transform grabbedSnapHandle;
        private Transform trackPoint;
        private bool customTrackPoint = false;

        private Transform previousParent;
        private bool previousKinematicState;
        private bool previousIsGrabbable;

        public virtual void OnInteractableObjectTouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectTouched != null)
                InteractableObjectTouched(this, e);
        }

        public virtual void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUntouched != null)
                InteractableObjectUntouched(this, e);
        }

        public virtual void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectGrabbed != null)
                InteractableObjectGrabbed(this, e);
        }

        public virtual void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUngrabbed != null)
                InteractableObjectUngrabbed(this, e);
        }

        public virtual void OnInteractableObjectUsed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUsed != null)
                InteractableObjectUsed(this, e);
        }

        public virtual void OnInteractableObjectUnused(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUnused != null)
                InteractableObjectUnused(this, e);
        }

        public InteractableObjectEventArgs SetInteractableObjectEvent(GameObject interactingObject)
        {
            InteractableObjectEventArgs e;
            e.interactingObject = interactingObject;
            return e;
        }

        public bool IsTouched()
        {
            return (touchingObject != null);
        }

        public bool IsGrabbed()
        {
            return (grabbingObject != null);
        }

        public bool IsUsing()
        {
            return (usingObject != null);
        }

        public virtual void StartTouching(GameObject currentTouchingObject)
        {
            OnInteractableObjectTouched(SetInteractableObjectEvent(currentTouchingObject));
            touchingObject = currentTouchingObject;
        }

        public virtual void StopTouching(GameObject previousTouchingObject)
        {
            OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingObject));
            touchingObject = null;
        }

        public virtual void Grabbed(GameObject currentGrabbingObject)
        {
            OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingObject));
            ForceReleaseGrab();
            RemoveTrackPoint();
            grabbingObject = currentGrabbingObject;
            SetTrackPoint(grabbingObject);
            if (!isSwappable)
            {
                previousIsGrabbable = isGrabbable;
                isGrabbable = false;
            }
        }

        public virtual void Ungrabbed(GameObject previousGrabbingObject)
        {
            OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingObject));
            RemoveTrackPoint();
            grabbedSnapHandle = null;
            grabbingObject = null;
            LoadPreviousState();
        }

        public virtual void StartUsing(GameObject currentUsingObject)
        {
            OnInteractableObjectUsed(SetInteractableObjectEvent(currentUsingObject));
            usingObject = currentUsingObject;
        }

        public virtual void StopUsing(GameObject previousUsingObject)
        {
            OnInteractableObjectUnused(SetInteractableObjectEvent(previousUsingObject));
            usingObject = null;
        }

        public virtual void ToggleHighlight(bool toggle)
        {
            ToggleHighlight(toggle, Color.clear);
        }

        public virtual void ToggleHighlight(bool toggle, Color globalHighlightColor)
        {
            if (highlightOnTouch)
            {
                if (toggle && !IsGrabbed() && !IsUsing())
                {
                    Color color = (touchHighlightColor != Color.clear ? touchHighlightColor : globalHighlightColor);
                    if (color != Color.clear)
                    {
                        var colorArray = BuildHighlightColorArray(color);
                        ChangeColor(colorArray);
                    }
                }
                else
                {
                    if (originalObjectColours == null)
                    {
                        Debug.LogError("VRTK_InteractableObject has not had the Start() method called, if you are inheriting this class then call base.Start() in your Start() method.");
                        return;
                    }
                    ChangeColor(originalObjectColours);
                }
            }
        }

        public int UsingState
        {
            get { return usingState; }
            set { usingState = value; }
        }

        public void PauseCollisions()
        {
            if (onGrabCollisionDelay > 0f)
            {
                if (this.GetComponent<Rigidbody>())
                {
                    this.GetComponent<Rigidbody>().detectCollisions = false;
                }
                foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
                {
                    rb.detectCollisions = false;
                }
                Invoke("UnpauseCollisions", onGrabCollisionDelay);
            }
        }

        public bool AttachIsTrackObject()
        {
            return (grabAttachMechanic == GrabAttachType.Track_Object);
        }

        public void ZeroVelocity()
        {
            if (this.GetComponent<Rigidbody>())
            {
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }

        public void SaveCurrentState()
        {
            if (grabbingObject == null)
            {
                previousParent = this.transform.parent;
                previousKinematicState = rb.isKinematic;
            }
        }

        public void ToggleKinematic(bool state)
        {
            rb.isKinematic = state;
        }

        public GameObject GetGrabbingObject()
        {
            return grabbingObject;
        }

        public bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)
        {
            if (controllerCheck == AllowedController.Both)
            {
                return true;
            }

            var controllerHand = DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("_Only", ""));
            return (DeviceFinder.IsControllerOfHand(actualController, controllerHand));
        }

        public void ForceStopInteracting()
        {
            if (touchingObject != null)
            {
                touchingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            }

            if (grabbingObject != null)
            {
                grabbingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }

            if (usingObject != null)
            {
                usingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                usingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
            }
        }

        public void SetGrabbedSnapHandle(Transform handle)
        {
            grabbedSnapHandle = handle;
        }

        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();

            // If there is no rigid body, add one and set it to 'kinematic'.
            if (!rb)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }
            rb.maxAngularVelocity = float.MaxValue;
        }

        protected virtual void Start()
        {
            originalObjectColours = StoreOriginalColors();
        }

        protected virtual void Update()
        {
            if (!this.gameObject.activeInHierarchy)
            {
                ForceStopInteracting();
            }

            if (grabAttachMechanic == GrabAttachType.Track_Object)
            {
                CheckBreakDistance();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (grabAttachMechanic == GrabAttachType.Track_Object)
            {
                FixedUpdateTrackedObject();
            }
        }

        protected virtual void OnDisable()
        {
            ForceStopInteracting();
        }

        protected virtual void OnJointBreak(float force)
        {
            ForceReleaseGrab();
        }

        protected virtual void LoadPreviousState()
        {
            if (this.gameObject.activeInHierarchy)
            {
                this.transform.parent = previousParent;
            }
            rb.isKinematic = previousKinematicState;
            if (!isSwappable)
            {
                isGrabbable = previousIsGrabbable;
            }
        }

        private void ForceReleaseGrab()
        {
            if (grabbingObject)
            {
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
        }

        private void UnpauseCollisions()
        {
            if (this.GetComponent<Rigidbody>())
            {
                this.GetComponent<Rigidbody>().detectCollisions = true;
            }
            foreach (Rigidbody rb in this.GetComponentsInChildren<Rigidbody>())
            {
                rb.detectCollisions = true;
            }
        }

        private Renderer[] GetRendererArray()
        {
            return (GetComponents<Renderer>().Length > 0 ? GetComponents<Renderer>() : GetComponentsInChildren<Renderer>());
        }

        private Dictionary<string, Color> StoreOriginalColors()
        {
            Dictionary<string, Color> colors = new Dictionary<string, Color>();
            foreach (Renderer renderer in GetRendererArray())
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    colors[renderer.gameObject.name] = renderer.material.color;
                }
            }
            return colors;
        }

        private Dictionary<string, Color> BuildHighlightColorArray(Color color)
        {
            Dictionary<string, Color> colors = new Dictionary<string, Color>();
            foreach (Renderer renderer in GetRendererArray())
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    colors[renderer.gameObject.name] = color;
                }
            }
            return colors;
        }

        private void ChangeColor(Dictionary<string, Color> colors)
        {
            foreach (Renderer renderer in GetRendererArray())
            {
                if (renderer.material.HasProperty("_Color") && colors.ContainsKey(renderer.gameObject.name))
                {
                    renderer.material.color = colors[renderer.gameObject.name];
                }
            }
        }

        private void CheckBreakDistance()
        {
            if (trackPoint)
            {
                float distance = Vector3.Distance(trackPoint.position, this.transform.position);
                if (distance > (detachThreshold / 1000))
                {
                    ForceReleaseGrab();
                }
            }
        }

        private void SetTrackPoint(GameObject point)
        {
            Transform controllerPoint = point.transform;

            if (point.GetComponent<VRTK_InteractGrab>() && point.GetComponent<VRTK_InteractGrab>().controllerAttachPoint)
            {
                controllerPoint = point.GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform;
            }

            if (grabAttachMechanic == GrabAttachType.Track_Object && precisionSnap)
            {
                trackPoint = new GameObject(string.Format("[{0}]TrackObject_PrecisionSnap_AttachPoint", this.gameObject.name)).transform;
                trackPoint.parent = point.transform;
                trackPoint.position = this.transform.position;
                trackPoint.rotation = this.transform.rotation;
                customTrackPoint = true;
            }
            else
            {
                trackPoint = controllerPoint;
                customTrackPoint = false;
            }
        }

        private void RemoveTrackPoint()
        {
            if (customTrackPoint && trackPoint)
            {
                Destroy(trackPoint.gameObject);
            }
            else
            {
                trackPoint = null;
            }
        }

        private void FixedUpdateTrackedObject()
        {
            if (trackPoint)
            {
                float maxDistanceDelta = 10f;

                Quaternion rotationDelta;
                Vector3 positionDelta;

                float angle;
                Vector3 axis;

                if (grabbedSnapHandle != null)
                {
                    rotationDelta = trackPoint.rotation * Quaternion.Inverse(grabbedSnapHandle.rotation);
                    positionDelta = trackPoint.position - grabbedSnapHandle.position;
                }
                else
                {
                    rotationDelta = trackPoint.rotation * Quaternion.Inverse(this.transform.rotation);
                    positionDelta = trackPoint.position - this.transform.position;
                }

                rotationDelta.ToAngleAxis(out angle, out axis);

                angle = (angle > 180 ? angle -= 360 : angle);

                if (angle != 0)
                {
                    Vector3 angularTarget = angle * axis;
                    rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, maxDistanceDelta);
                }

                Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
                rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxDistanceDelta);
            }
        }
    }
}