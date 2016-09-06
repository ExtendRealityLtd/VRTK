// Bezier Pointer|Scripts|0050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Bezier Pointer emits a curved line (made out of game objects) from the end of the controller to a point on a ground surface (at any height). It is more useful than the Simple Laser Pointer for traversing objects of various heights as the end point can be curved on top of objects that are not visible to the user.
    /// </summary>
    /// <remarks>
    /// The laser beam is activated by default by pressing the `Touchpad` on the controller. The event it is listening for is the `AliasPointer` events so the pointer toggle button can be set by changing the `Pointer Toggle` button on the `VRTK_ControllerEvents` script parameters.
    ///
    /// The Bezier Pointer script can be attached to a Controller object within the `[CameraRig]` prefab and the Controller object also requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for enabling and disabling the beam. It is also possible to attach the Bezier Pointer script to another object (like the `[CameraRig]/Camera (head)`) to enable other objects to project the beam. The controller parameter must be entered with the desired controller to toggle the beam if this is the case.
    ///
    ///   > The bezier curve generation code is in another script located at `VRTK/Scripts/Helper/CurveGenerator.cs` and was heavily inspired by the tutorial and code from [Catlike Coding](http://catlikecoding.com/unity/tutorials/curves-and-splines/).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/009_Controller_BezierPointer` is used in conjunction with the Height Adjust Teleporter shows how it is possible to traverse different height objects using the curved pointer without needing to see the top of the object.
    ///
    /// `VRTK/Examples/012_Controller_PointerWithAreaCollision` shows how a Bezier Pointer with the Play Area Cursor and Collision Detection enabled can be used to traverse a game area but not allow teleporting into areas where the walls or other objects would fall into the play area space enabling the user to enter walls.
    ///
    /// `VRTK/Examples/036_Controller_CustomCompoundPointer' shows how to display an object (a teleport beam) only if the teleport location is valid, and can create an animated trail along the tracer curve.
    /// </example>
    public class VRTK_BezierPointer : VRTK_WorldPointer
    {
        [Tooltip("The length of the projected forward pointer beam, this is basically the distance able to point from the controller position.")]
        public float pointerLength = 10f;
        [Tooltip("The number of items to render in the beam bezier curve. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.")]
        public int pointerDensity = 10;
        [Tooltip("A cursor is displayed on the ground at the location the beam ends at, it is useful to see what height the beam end location is, however it can be turned off by toggling this.")]
        public bool showPointerCursor = true;
        [Tooltip("The size of the ground pointer cursor. This number also affects the size of the objects in the bezier curve beam. The larger the radius, the larger the objects will be.")]
        public float pointerCursorRadius = 0.5f;
        [Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
        public float beamCurveOffset = 1f;
        [Tooltip("A custom Game Object can be applied here to use instead of the default sphere for the beam tracer. The custom Game Object will match the rotation of the controller.")]
        public GameObject customPointerTracer;
        [Tooltip("A custom Game Object can be applied here to use instead of the default flat cylinder for the pointer cursor.")]
        public GameObject customPointerCursor;
        [Tooltip("The layers to ignore when raycasting.")]
        public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;
        [Tooltip("A custom Game Object can be applied here to appear only if the teleport is allowed (its material will not be changed ).")]
        public GameObject validTeleportLocationObject = null;
        [Tooltip("Rescale each pointer tracer element according to the length of the Bezier curve.")]
        public bool rescalePointerTracer = false;

        private GameObject projectedBeamContainer;
        private GameObject projectedBeamForward;
        private GameObject projectedBeamJoint;
        private GameObject projectedBeamDown;

        private GameObject pointerCursor;
        private GameObject curvedBeamContainer;
        private CurveGenerator curvedBeam;

        private GameObject validTeleportLocationInstance = null;
        private Material customPointerMaterial;
        private Material beamTraceMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();
            InitProjectedBeams();
            InitPointer();
            TogglePointer(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (projectedBeamDown != null)
            {
                Destroy(projectedBeamDown);
            }
            if (pointerCursor != null)
            {
                Destroy(pointerCursor);
            }
            if (curvedBeam != null)
            {
                Destroy(curvedBeam);
            }
            if (projectedBeamContainer != null)
            {
                Destroy(projectedBeamContainer);
            }
            if (curvedBeamContainer != null)
            {
                Destroy(curvedBeamContainer);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (projectedBeamForward.gameObject.activeSelf)
            {
                ProjectForwardBeam();
                ProjectDownBeam();
                DisplayCurvedBeam();
                SetPointerCursor();
            }
        }

        protected override void InitPointer()
        {
            if (customPointerTracer != null)
            {
                var renderer = customPointerTracer.GetComponentInChildren<MeshRenderer>();
                if (renderer)
                {
                    beamTraceMaterial = Material.Instantiate(renderer.sharedMaterial);
                }
            }
            if (customPointerCursor)
            {
                Renderer renderer = customPointerCursor.GetComponentInChildren<MeshRenderer>();
                if (renderer != null)
                {
                    customPointerMaterial = Material.Instantiate(renderer.sharedMaterial);
                }
                pointerCursor = GameObject.Instantiate(customPointerCursor);
                foreach (Renderer mr in pointerCursor.GetComponentsInChildren<Renderer>())
                {
                    mr.material = customPointerMaterial;
                }
                if (validTeleportLocationObject != null)
                {
                    validTeleportLocationInstance = Instantiate(validTeleportLocationObject);
                    validTeleportLocationInstance.name = string.Format("[{0}]WorldPointer_BezierPointer_TeleportBeam", gameObject.name);
                    validTeleportLocationInstance.transform.parent = pointerCursor.transform;
                    validTeleportLocationInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
                    validTeleportLocationInstance.SetActive(false);
                }
            }
            else
            {
                pointerCursor = CreateCursor();
            }
            pointerCursor.name = string.Format("[{0}]WorldPointer_BezierPointer_PointerCursor", gameObject.name);
            Utilities.SetPlayerObject(pointerCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
            pointerCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
            pointerCursor.SetActive(false);

            curvedBeamContainer = new GameObject(string.Format("[{0}]WorldPointer_BezierPointer_CurvedBeamContainer", gameObject.name));
            Utilities.SetPlayerObject(curvedBeamContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
            curvedBeamContainer.SetActive(false);
            curvedBeam = curvedBeamContainer.gameObject.AddComponent<CurveGenerator>();
            curvedBeam.transform.parent = null;
            curvedBeam.Create(pointerDensity, pointerCursorRadius, customPointerTracer, rescalePointerTracer);
            base.InitPointer();
        }

        protected override void SetPointerMaterial()
        {
            if (customPointerMaterial != null)
            {
                customPointerMaterial.color = pointerMaterial.color;
            }
            if (beamTraceMaterial != null)
            {
                beamTraceMaterial.color = pointerMaterial.color;
                if (beamTraceMaterial.HasProperty("_EmissionColor"))
                {
                    beamTraceMaterial.SetColor("_EmissionColor", pointerMaterial.color);
                }
            }

            if (customPointerCursor == null)
            {
                if (pointerCursor.GetComponent<Renderer>())
                {
                    pointerCursor.GetComponent<Renderer>().material = pointerMaterial;
                }

                foreach (Renderer mr in pointerCursor.GetComponentsInChildren<Renderer>())
                {
                    mr.material = pointerMaterial;
                }
            }
            base.SetPointerMaterial();
        }

        protected override void TogglePointer(bool state)
        {
            state = (pointerVisibility == pointerVisibilityStates.Always_On ? true : state);

            if (projectedBeamForward)
            {
                projectedBeamForward.gameObject.SetActive(state);
            }
            if (projectedBeamJoint)
            {
                projectedBeamJoint.gameObject.SetActive(state);
            }
            if (projectedBeamDown)
            {
                projectedBeamDown.SetActive(state);
            }
        }

        protected override void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            base.DisablePointerBeam(sender, e);
            TogglePointerCursor(false);
            curvedBeam.TogglePoints(false);
        }

        private GameObject CreateCursor()
        {
            var cursorYOffset = 0.02f;
            var cursor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cursor.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            cursor.GetComponent<MeshRenderer>().receiveShadows = false;
            cursor.transform.localScale = new Vector3(pointerCursorRadius, cursorYOffset, pointerCursorRadius);
            Destroy(cursor.GetComponent<CapsuleCollider>());
            return cursor;
        }

        private void TogglePointerCursor(bool state)
        {
            var pointerCursorState = (showPointerCursor && state ? showPointerCursor : false);
            var playAreaCursorState = (showPlayAreaCursor && state ? showPlayAreaCursor : false);
            pointerCursor.gameObject.SetActive(pointerCursorState);
            base.TogglePointer(playAreaCursorState);
        }

        private void InitProjectedBeams()
        {
            projectedBeamContainer = new GameObject(string.Format("[{0}]WorldPointer_BezierPointer_ProjectedBeamContainer", gameObject.name));
            Utilities.SetPlayerObject(projectedBeamContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
            projectedBeamContainer.transform.parent = transform;
            projectedBeamContainer.transform.localPosition = Vector3.zero;

            projectedBeamForward = new GameObject(string.Format("[{0}]WorldPointer_BezierPointer_ProjectedBeamForward", gameObject.name));
            Utilities.SetPlayerObject(projectedBeamForward, VRTK_PlayerObject.ObjectTypes.Pointer);
            projectedBeamForward.transform.parent = projectedBeamContainer.transform;

            projectedBeamJoint = new GameObject(string.Format("[{0}]WorldPointer_BezierPointer_ProjectedBeamJoint", gameObject.name));
            Utilities.SetPlayerObject(projectedBeamJoint, VRTK_PlayerObject.ObjectTypes.Pointer);
            projectedBeamJoint.transform.parent = projectedBeamContainer.transform;
            projectedBeamJoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            projectedBeamDown = new GameObject(string.Format("[{0}]WorldPointer_BezierPointer_ProjectedBeamDown", gameObject.name));
            Utilities.SetPlayerObject(projectedBeamDown, VRTK_PlayerObject.ObjectTypes.Pointer);
        }

        private float GetForwardBeamLength()
        {
            var actualLength = pointerLength;
            Ray pointerRaycast = new Ray(transform.position, transform.forward);
            RaycastHit collidedWith;
            var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, pointerLength, ~layersToIgnore);

            //reset if beam not hitting or hitting new target
            if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
            {
                pointerContactDistance = 0f;
            }

            //check if beam has hit a new target
            if (hasRayHit)
            {
                pointerContactDistance = collidedWith.distance;
            }

            //adjust beam length if something is blocking it
            if (hasRayHit && pointerContactDistance < pointerLength)
            {
                actualLength = pointerContactDistance;
            }

            return actualLength;
        }

        private void ProjectForwardBeam()
        {
            var setThicknes = 0.01f;
            var setLength = GetForwardBeamLength();
            //if the additional decimal isn't added then the beam position glitches
            var beamPosition = setLength / (2 + 0.00001f);

            projectedBeamForward.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
            projectedBeamForward.transform.localPosition = new Vector3(0f, 0f, beamPosition);
            projectedBeamJoint.transform.localPosition = new Vector3(0f, 0f, setLength - (projectedBeamJoint.transform.localScale.z / 2.0f));
            projectedBeamContainer.transform.localRotation = Quaternion.identity;
        }

        private void ProjectDownBeam()
        {
            projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y, projectedBeamJoint.transform.position.z);

            Ray projectedBeamDownRaycast = new Ray(projectedBeamDown.transform.position, Vector3.down);
            RaycastHit collidedWith;

            var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity, ~layersToIgnore);

            if (!downRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
            {
                if (pointerContactTarget != null)
                {
                    base.PointerOut();
                }
                pointerContactTarget = null;
                destinationPosition = Vector3.zero;
            }

            if (downRayHit)
            {
                projectedBeamDown.transform.position = new Vector3(projectedBeamJoint.transform.position.x, projectedBeamJoint.transform.position.y - collidedWith.distance, projectedBeamJoint.transform.position.z);
                projectedBeamDown.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                pointerContactTarget = collidedWith.transform;
                destinationPosition = projectedBeamDown.transform.position;

                base.PointerIn();
            }
        }

        private void SetPointerCursor()
        {
            if (pointerContactTarget != null)
            {
                TogglePointerCursor(true);
                pointerCursor.transform.position = projectedBeamDown.transform.position;
                base.SetPlayAreaCursorTransform(pointerCursor.transform.position);
                UpdatePointerMaterial(pointerHitColor);
                if (validTeleportLocationInstance != null)
                {
                    validTeleportLocationInstance.SetActive(ValidDestination(pointerContactTarget, destinationPosition));
                }
            }
            else
            {
                TogglePointerCursor(false);
                UpdatePointerMaterial(pointerMissColor);
            }
        }

        private void DisplayCurvedBeam()
        {
            Vector3[] beamPoints = new Vector3[]
            {
                transform.position,
                projectedBeamJoint.transform.position + new Vector3(0f, beamCurveOffset, 0f),
                projectedBeamDown.transform.position,
                projectedBeamDown.transform.position,
            };
            curvedBeam.SetPoints(beamPoints, beamTraceMaterial ?? pointerMaterial);
            if (pointerVisibility != pointerVisibilityStates.Always_Off)
            {
                curvedBeam.TogglePoints(true);
            }
        }
    }
}