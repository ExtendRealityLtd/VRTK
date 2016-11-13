// Simple Pointer|Scripts|0040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Simple Pointer emits a coloured beam from the end of the controller to simulate a laser beam. It can be useful for pointing to objects within a scene and it can also determine the object it is pointing at and the distance the object is from the controller the beam is being emitted from.
    /// </summary>
    /// <remarks>
    /// The laser beam is activated by default by pressing the `Touchpad` on the controller. The event it is listening for is the `AliasPointer` events so the pointer toggle button can be set by changing the `Pointer Toggle` button on the `VRTK_ControllerEvents` script parameters.
    ///
    /// The Simple Pointer script can be attached to a Controller object within the `[CameraRig]` prefab and the Controller object also requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for enabling and disabling the beam. It is also possible to attach the Simple Pointer script to another object (like the `[CameraRig]/Camera (head)`) to enable other objects to project the beam. The controller parameter must be entered with the desired controller to toggle the beam if this is the case.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/003_Controller_SimplePointer` shows the simple pointer in action and code examples of how the events are utilised and listened to can be viewed in the script `VRTK/Examples/Resources/Scripts/VRTK_ControllerPointerEvents_ListenerExample.cs`
    /// </example>
    public class VRTK_SimplePointer : VRTK_WorldPointer
    {
        [Header("Simple Pointer Settings", order = 3)]
        [Tooltip("The thickness and length of the beam can also be set on the script as well as the ability to toggle the sphere beam tip that is displayed at the end of the beam (to represent a cursor).")]
        public float pointerThickness = 0.002f;
        [Tooltip("The distance the beam will project before stopping.")]
        public float pointerLength = 100f;
        [Tooltip("Toggle whether the cursor is shown on the end of the pointer beam.")]
        public bool showPointerTip = true;
        [Header("Custom Appearance Settings", order = 4)]
        [Tooltip("A custom Game Object can be applied here to use instead of the default sphere for the pointer cursor.")]
        public GameObject customPointerCursor;
        [Tooltip("Rotate the pointer cursor to match the normal of the target surface (or the pointer direction if no target was hit).")]
        public bool pointerCursorMatchTargetNormal = false;
        [Tooltip("Rescale the pointer cursor proportionally to the distance from this game object (useful when used as a gaze pointer).")]
        public bool pointerCursorRescaledAlongDistance = false;

        private GameObject pointerHolder;
        private GameObject pointer;
        private GameObject pointerTip;
        private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);
        private Vector3 pointerCursorOriginalScale = Vector3.one;

        protected override void OnEnable()
        {
            base.OnEnable();
            InitPointer();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (pointerHolder != null)
            {
                Destroy(pointerHolder);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (pointer.gameObject.activeSelf)
            {
                Ray pointerRaycast = new Ray(GetOriginPosition(), GetOriginForward());
                RaycastHit pointerCollidedWith;
                var rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith, pointerLength, ~layersToIgnore);
                var pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
                SetPointerTransform(pointerBeamLength, pointerThickness);
                if (rayHit)
                {
                    if (pointerCursorMatchTargetNormal)
                    {
                        pointerTip.transform.forward = -pointerCollidedWith.normal;
                    }
                    if (pointerCursorRescaledAlongDistance)
                    {
                        float collisionDistance = Vector3.Distance(pointerCollidedWith.point, GetOriginPosition());
                        pointerTip.transform.localScale = pointerCursorOriginalScale * collisionDistance;
                    }
                }
                else
                {
                    if (pointerCursorMatchTargetNormal)
                    {
                        pointerTip.transform.forward = GetOriginForward();
                    }
                    if (pointerCursorRescaledAlongDistance)
                    {
                        pointerTip.transform.localScale = pointerCursorOriginalScale * pointerBeamLength;
                    }
                }
            }
        }

        protected override void UpdateObjectInteractor()
        {
            base.UpdateObjectInteractor();
            //if the object interactor is too far from the pointer tip then set it to the pointer tip position to prevent glitching.
            if (Vector3.Distance(objectInteractor.transform.position, pointerTip.transform.position) > 0)
            {
                objectInteractor.transform.position = pointerTip.transform.position;
            }
        }

        protected override void InitPointer()
        {
            pointerHolder = new GameObject(string.Format("[{0}]WorldPointer_SimplePointer_Holder", gameObject.name));
            pointerHolder.transform.parent = transform;
            pointerHolder.transform.localPosition = Vector3.zero;
            Utilities.SetPlayerObject(pointerHolder, VRTK_PlayerObject.ObjectTypes.Pointer);

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.name = string.Format("[{0}]WorldPointer_SimplePointer_Pointer", gameObject.name);
            pointer.transform.parent = pointerHolder.transform;
            pointer.GetComponent<BoxCollider>().isTrigger = true;
            pointer.AddComponent<Rigidbody>().isKinematic = true;
            pointer.layer = LayerMask.NameToLayer("Ignore Raycast");

            var pointerRenderer = pointer.GetComponent<MeshRenderer>();
            pointerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pointerRenderer.receiveShadows = false;
            pointerRenderer.material = pointerMaterial;

            Utilities.SetPlayerObject(pointer, VRTK_PlayerObject.ObjectTypes.Pointer);

            if (customPointerCursor)
            {
                pointerTip = Instantiate(customPointerCursor);
            }
            else
            {
                pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointerTip.transform.localScale = pointerTipScale;

                var pointerTipRenderer = pointerTip.GetComponent<MeshRenderer>();
                pointerTipRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                pointerTipRenderer.receiveShadows = false;
                pointerTipRenderer.material = pointerMaterial;
            }

            pointerCursorOriginalScale = pointerTip.transform.localScale;
            pointerTip.transform.name = string.Format("[{0}]WorldPointer_SimplePointer_PointerTip", gameObject.name);
            pointerTip.transform.parent = pointerHolder.transform;
            pointerTip.GetComponent<Collider>().isTrigger = true;
            pointerTip.AddComponent<Rigidbody>().isKinematic = true;
            pointerTip.layer = LayerMask.NameToLayer("Ignore Raycast");
            Utilities.SetPlayerObject(pointerTip, VRTK_PlayerObject.ObjectTypes.Pointer);

            base.InitPointer();

            if (showPointerTip && objectInteractor)
            {
                objectInteractor.transform.localScale = pointerTip.transform.localScale * 1.05f;
            }

            SetPointerTransform(pointerLength, pointerThickness);
            TogglePointer(false);
        }

        protected override void SetPointerMaterial(Color color)
        {
            base.SetPointerMaterial(color);

            base.ChangeMaterialColor(pointer, color);
            base.ChangeMaterialColor(pointerTip, color);
        }

        protected override void TogglePointer(bool state)
        {
            state = (pointerVisibility == pointerVisibilityStates.Always_On ? true : state);
            base.TogglePointer(state);
            pointer.gameObject.SetActive(state);

            var tipState = (showPointerTip ? state : false);
            pointerTip.gameObject.SetActive(tipState);

            if (pointer.GetComponent<Renderer>() && pointerVisibility == pointerVisibilityStates.Always_Off)
            {
                pointer.GetComponent<Renderer>().enabled = false;
            }
        }

        private void SetPointerTransform(float setLength, float setThicknes)
        {
            //if the additional decimal isn't added then the beam position glitches
            var beamPosition = setLength / (2 + 0.00001f);

            pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
            pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);
            pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));

            pointerHolder.transform.localPosition = GetOriginLocalPosition();
            pointerHolder.transform.localRotation = GetOriginLocalRotation();
            base.UpdateDependencies(pointerTip.transform.position);
        }

        private float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
        {
            var actualLength = pointerLength;

            //reset if beam not hitting or hitting new collider
            if (!hasRayHit || (pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != collidedWith.collider))
            {
                if (pointerContactRaycastHit.collider != null)
                {
                    base.PointerOut();
                }

                pointerContactDistance = 0f;
                pointerContactTarget = null;
                pointerContactRaycastHit = new RaycastHit();
                destinationPosition = Vector3.zero;

                UpdatePointerMaterial(pointerMissColor);
            }

            //check if beam has hit a new target
            if (hasRayHit)
            {
                pointerContactDistance = collidedWith.distance;
                pointerContactTarget = collidedWith.transform;
                pointerContactRaycastHit = collidedWith;
                destinationPosition = pointerTip.transform.position;

                UpdatePointerMaterial(pointerHitColor);

                base.PointerIn();
            }

            //adjust beam length if something is blocking it
            if (hasRayHit && pointerContactDistance < pointerLength)
            {
                actualLength = pointerContactDistance;
            }

            return OverrideBeamLength(actualLength);
        }
    }
}