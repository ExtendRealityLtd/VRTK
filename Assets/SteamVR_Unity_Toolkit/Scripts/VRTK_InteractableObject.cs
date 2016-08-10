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
            Rotator_Track,
            Child_Of_Controller,
            Climbable
        }

        public enum AllowedController
        {
            Both,
            Left_Only,
            Right_Only
        }

        public enum ControllerHideMode
        {
            Default,
            OverrideHide,
            OverrideDontHide,
        }

        [Header("Touch Interactions", order = 1)]
        public bool highlightOnTouch = false;
        public Color touchHighlightColor = Color.clear;
        public Vector2 rumbleOnTouch = Vector2.zero;
        public AllowedController allowedTouchControllers = AllowedController.Both;
        public ControllerHideMode hideControllerOnTouch = ControllerHideMode.Default;

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
        public ControllerHideMode hideControllerOnGrab = ControllerHideMode.Default;

        [Header("Grab Mechanics", order = 3)]
        public GrabAttachType grabAttachMechanic = GrabAttachType.Fixed_Joint;
        public float detachThreshold = 500f;
        public float springJointStrength = 500f;
        public float springJointDamper = 50f;
        public float throwMultiplier = 1f;
        public float onGrabCollisionDelay = 0f;

        [Header("Use Interactions", order = 4)]
        public bool isUsable = false;
        public bool useOnlyIfGrabbed = false;
        public bool holdButtonToUse = true;
        public bool pointerActivatesUseAction = false;
        public Vector2 rumbleOnUse = Vector2.zero;
        public AllowedController allowedUseControllers = AllowedController.Both;
        public ControllerHideMode hideControllerOnUse = ControllerHideMode.Default;

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
        private Dictionary<string, Color[]> originalObjectColours;

        private Transform grabbedSnapHandle;
        private Transform trackPoint;
        private bool customTrackPoint = false;
        private Transform originalControllerAttachPoint;

        private Transform previousParent;
        private bool previousKinematicState;
        private bool previousIsGrabbable;
        private bool forcedDropped;

        public bool CheckHideMode(bool defaultMode, ControllerHideMode overrideMode)
        {
            switch (overrideMode)
            {
                case VRTK_InteractableObject.ControllerHideMode.OverrideDontHide:
                    return false;
                case VRTK_InteractableObject.ControllerHideMode.OverrideHide:
                    return true;
            }
            // default: do not change
            return defaultMode;
        }

        public virtual void OnInteractableObjectTouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectTouched != null)
            {
                InteractableObjectTouched(this, e);
            }
        }

        public virtual void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUntouched != null)
            {
                InteractableObjectUntouched(this, e);
            }
        }

        public virtual void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectGrabbed != null)
            {
                InteractableObjectGrabbed(this, e);
            }
        }

        public virtual void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUngrabbed != null)
            {
                InteractableObjectUngrabbed(this, e);
            }
        }

        public virtual void OnInteractableObjectUsed(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUsed != null)
            {
                InteractableObjectUsed(this, e);
            }
        }

        public virtual void OnInteractableObjectUnused(InteractableObjectEventArgs e)
        {
            if (InteractableObjectUnused != null)
            {
                InteractableObjectUnused(this, e);
            }
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
            StopUsingOnControllerChange(previousTouchingObject);
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
            StopUsingOnControllerChange(previousGrabbingObject);
        }

        private void StopUsingOnControllerChange(GameObject previousController)
        {
            var usingObject = previousController.GetComponent<VRTK_InteractUse>();
            if (usingObject)
            {
                if (holdButtonToUse)
                {
                    usingObject.ForceStopUsing();
                }
                else
                {
                    usingObject.ForceResetUsing();
                }
            }
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
                if (GetComponent<Rigidbody>())
                {
                    GetComponent<Rigidbody>().detectCollisions = false;
                }
                foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                {
                    rb.detectCollisions = false;
                }
                Invoke("UnpauseCollisions", onGrabCollisionDelay);
            }
        }

        public bool AttachIsTrackObject()
        {
            return (grabAttachMechanic == GrabAttachType.Track_Object || grabAttachMechanic == GrabAttachType.Rotator_Track);
        }

        public bool AttachIsClimbObject()
        {
            return (grabAttachMechanic == GrabAttachType.Climbable);
        }

        public bool AttachIsStaticObject()
        {
            return AttachIsClimbObject(); // only one at the moment
        }

        public void ZeroVelocity()
        {
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }

        public void SaveCurrentState()
        {
            if (grabbingObject == null)
            {
                previousParent = transform.parent;

                if (rb)
                {
                    previousKinematicState = rb.isKinematic;
                }
            }
        }

        public void ToggleKinematic(bool state)
        {
            if (rb)
            {
                rb.isKinematic = state;
            }
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

            var controllerHand = VRTK_DeviceFinder.GetControllerHandType(controllerCheck.ToString().Replace("_Only", ""));
            return (VRTK_DeviceFinder.IsControllerOfHand(actualController, controllerHand));
        }

        public void ForceStopInteracting()
        {
            if (touchingObject != null && touchingObject.activeInHierarchy)
            {
                touchingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                forcedDropped = true;
            }

            if (grabbingObject != null && grabbingObject.activeInHierarchy)
            {
                grabbingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
                forcedDropped = true;
            }

            if (usingObject != null && usingObject.activeInHierarchy)
            {
                usingObject.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                usingObject.GetComponent<VRTK_InteractUse>().ForceStopUsing();
                forcedDropped = true;
            }
        }

        public void SetGrabbedSnapHandle(Transform handle)
        {
            grabbedSnapHandle = handle;
        }

        public void RegisterTeleporters()
        {
            foreach (var teleporter in FindObjectsOfType<VRTK_BasicTeleport>())
            {
                teleporter.Teleported += new TeleportEventHandler(OnTeleported);
            }
        }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (!AttachIsStaticObject())
            {
                // If there is no rigid body, add one and set it to 'kinematic'.
                if (!rb)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
                rb.maxAngularVelocity = float.MaxValue;
            }
            forcedDropped = false;
        }

        protected virtual void Start()
        {
            originalObjectColours = StoreOriginalColors();
        }

        protected virtual void Update()
        {
            if (!gameObject.activeInHierarchy)
            {
                ForceStopInteracting();
            }

            if (AttachIsTrackObject())
            {
                CheckBreakDistance();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (trackPoint)
            {
                switch (grabAttachMechanic)
                {
                    case GrabAttachType.Rotator_Track:
                        FixedUpdateRotatorTrack();
                        break;
                    case GrabAttachType.Track_Object:
                        FixedUpdateTrackObject();
                        break;
                }
            }
        }

        protected virtual void OnEnable()
        {
            RegisterTeleporters();
            if (forcedDropped)
            {
                LoadPreviousState();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var teleporter in FindObjectsOfType<VRTK_BasicTeleport>())
            {
                teleporter.Teleported -= new TeleportEventHandler(OnTeleported);
            }
            ForceStopInteracting();
        }

        protected virtual void OnJointBreak(float force)
        {
            ForceReleaseGrab();
        }

        protected virtual void LoadPreviousState()
        {
            if (gameObject.activeInHierarchy)
            {
                transform.parent = previousParent;
                forcedDropped = false;
            }
            if (rb)
            {
                rb.isKinematic = previousKinematicState;
            }
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
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().detectCollisions = true;
            }
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.detectCollisions = true;
            }
        }

        private Renderer[] GetRendererArray()
        {
            return (GetComponents<Renderer>().Length > 0 ? GetComponents<Renderer>() : GetComponentsInChildren<Renderer>());
        }

        private Dictionary<string, Color[]> StoreOriginalColors()
        {
            var colors = new Dictionary<string, Color[]>();
            foreach (Renderer renderer in GetRendererArray())
            {
                colors[renderer.gameObject.name] = new Color[renderer.materials.Length];

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    if (material.HasProperty("_Color"))
                    {
                        colors[renderer.gameObject.name][i] = material.color;
                    }
                }
            }
            return colors;
        }

        private Dictionary<string, Color[]> BuildHighlightColorArray(Color color)
        {
            var colors = new Dictionary<string, Color[]>();
            foreach (Renderer renderer in GetRendererArray())
            {
                colors[renderer.gameObject.name] = new Color[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    if (material.HasProperty("_Color"))
                    {
                        colors[renderer.gameObject.name][i] = color;
                    }
                }
            }
            return colors;
        }

        private void ChangeColor(Dictionary<string, Color[]> colors)
        {
            foreach (Renderer renderer in GetRendererArray())
            {
                if (!colors.ContainsKey(renderer.gameObject.name))
                {
                    continue;
                }

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    if (material.HasProperty("_Color"))
                    {
                        material.color = colors[renderer.gameObject.name][i];
                    }
                }
            }
        }

        private void CheckBreakDistance()
        {
            if (trackPoint)
            {
                float distance = Vector3.Distance(trackPoint.position, transform.position);
                if (distance > (detachThreshold / 1000))
                {
                    ForceReleaseGrab();
                }
            }
        }

        private void SetTrackPoint(GameObject point)
        {
            var controllerPoint = point.transform;
            var grabScript = point.GetComponent<VRTK_InteractGrab>();

            if (grabScript && grabScript.controllerAttachPoint)
            {
                controllerPoint = grabScript.controllerAttachPoint.transform;
            }

            if (AttachIsTrackObject() && precisionSnap)
            {
                trackPoint = new GameObject(string.Format("[{0}]TrackObject_PrecisionSnap_AttachPoint", gameObject.name)).transform;
                trackPoint.parent = point.transform;
                customTrackPoint = true;
                if (grabAttachMechanic == GrabAttachType.Track_Object)
                {
                    trackPoint.position = transform.position;
                    trackPoint.rotation = transform.rotation;
                }
                else
                {
                    trackPoint.position = controllerPoint.position;
                    trackPoint.rotation = controllerPoint.rotation;
                }
            }
            else
            {
                trackPoint = controllerPoint;
                customTrackPoint = false;
            }

            originalControllerAttachPoint = new GameObject(string.Format("[{0}]Original_Controller_AttachPoint", grabbingObject.name)).transform;
            originalControllerAttachPoint.parent = transform;
            originalControllerAttachPoint.position = trackPoint.position;
            originalControllerAttachPoint.rotation = trackPoint.rotation;
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
            if (originalControllerAttachPoint)
            {
                Destroy(originalControllerAttachPoint.gameObject);
            }
        }

        private void FixedUpdateRotatorTrack()
        {
            var rotateForce = trackPoint.position - originalControllerAttachPoint.position;
            rb.AddForceAtPosition(rotateForce, originalControllerAttachPoint.position, ForceMode.VelocityChange);
        }

        private void FixedUpdateTrackObject()
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
                rotationDelta = trackPoint.rotation * Quaternion.Inverse(transform.rotation);
                positionDelta = trackPoint.position - transform.position;
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

        private void OnTeleported(object sender, DestinationMarkerEventArgs e)
        {
            if (AttachIsTrackObject() && trackPoint)
            {
                transform.position = grabbingObject.transform.position;
            }
        }
    }
}
