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
        [Tooltip("The pointer cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
        public bool pointerCursorMatchTargetRotation = false;
        [Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
        public float beamCurveOffset = 1f;
        [Tooltip("The maximum angle in degrees of the controller before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.")]
        [Range(1, 100)]
        public float beamHeightLimitAngle = 100f;
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

        private GameObject pointerCursor;
        private GameObject curvedBeamContainer;
        private CurveGenerator curvedBeam;
        private GameObject validTeleportLocationInstance = null;
        private Material customPointerMaterial;
        private Material beamTraceMaterial;
        private bool beamActive = false;
        private Vector3 fixedForwardBeamForward;
        private Vector3 contactNormal;
        private const float BEAM_ADJUST_OFFSET = 0.00001f;

        protected override void OnEnable()
        {
            base.OnEnable();
            beamActive = false;
            InitPointer();
            TogglePointer(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            beamActive = false;
            if (pointerCursor != null)
            {
                Destroy(pointerCursor);
            }
            if (curvedBeam != null)
            {
                Destroy(curvedBeam);
            }
            if (curvedBeamContainer != null)
            {
                Destroy(curvedBeamContainer);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (beamActive)
            {
                var jointPosition = ProjectForwardBeam();
                var downPosition = ProjectDownBeam(jointPosition);
                DisplayCurvedBeam(jointPosition, downPosition);
                SetPointerCursor(downPosition);
            }
        }

        protected override void InitPointer()
        {
            if (customPointerTracer != null)
            {
                var renderer = customPointerTracer.GetComponentInChildren<MeshRenderer>();
                if (renderer)
                {
                    beamTraceMaterial = Instantiate(renderer.sharedMaterial);
                }
            }

            if (customPointerCursor)
            {
                var customPointerCursorRenderer = customPointerCursor.GetComponentInChildren<MeshRenderer>();

                if (customPointerCursorRenderer != null)
                {
                    customPointerMaterial = Instantiate(customPointerCursorRenderer.sharedMaterial);
                }

                pointerCursor = Instantiate(customPointerCursor);

                foreach (var pointerCursorRenderer in pointerCursor.GetComponentsInChildren<Renderer>())
                {
                    pointerCursorRenderer.material = customPointerMaterial;
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
                foreach (var pointerCursorRenderers in pointerCursor.GetComponentsInChildren<Renderer>())
                {
                    pointerCursorRenderers.material = pointerMaterial;
                }
            }
            base.SetPointerMaterial();
        }

        protected override void TogglePointer(bool state)
        {
            state = (pointerVisibility == pointerVisibilityStates.Always_On ? true : state);
            beamActive = state;
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

        private Vector3 ProjectForwardBeam()
        {
            var controllerRotation = Vector3.Dot(Vector3.up, controller.transform.forward.normalized);
            var calculatedLength = pointerLength;
            var useForward = transform.forward;
            if ((controllerRotation * 100f) > beamHeightLimitAngle)
            {
                useForward = new Vector3(transform.forward.x, fixedForwardBeamForward.y, transform.forward.z);
                var controllerRotationOffset = 1f - (controllerRotation - (beamHeightLimitAngle / 100f));
                calculatedLength = (pointerLength * controllerRotationOffset) * controllerRotationOffset;
            }
            else
            {
                fixedForwardBeamForward = transform.forward;
            }

            var actualLength = calculatedLength;
            Ray pointerRaycast = new Ray(transform.position, useForward);

            RaycastHit collidedWith;
            var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, calculatedLength, ~layersToIgnore);

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
            if (hasRayHit && pointerContactDistance < calculatedLength)
            {
                actualLength = pointerContactDistance;
            }

            //Use BEAM_ADJUST_OFFSET to move point back and up a bit to prevent beam clipping at collision point
            return (pointerRaycast.GetPoint(actualLength - BEAM_ADJUST_OFFSET) + (Vector3.up * BEAM_ADJUST_OFFSET));
        }

        private Vector3 ProjectDownBeam(Vector3 jointPosition)
        {
            Ray projectedBeamDownRaycast = new Ray(jointPosition, Vector3.down);
            RaycastHit collidedWith;

            var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity, ~layersToIgnore);

            if (!downRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
            {
                if (pointerContactTarget != null)
                {
                    base.PointerOut();
                }
                pointerContactTarget = null;
                contactNormal = Vector3.zero;
                destinationPosition = projectedBeamDownRaycast.GetPoint(0f);
            }

            if (downRayHit)
            {
                pointerContactTarget = collidedWith.transform;
                contactNormal = collidedWith.normal;
                destinationPosition = projectedBeamDownRaycast.GetPoint(collidedWith.distance);
                base.PointerIn();
            }
            return destinationPosition;
        }

        private void SetPointerCursor(Vector3 downPosition)
        {
            if (pointerContactTarget != null)
            {
                TogglePointerCursor(true);
                pointerCursor.transform.position = downPosition;
                if (pointerCursorMatchTargetRotation)
                {
                    pointerCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, contactNormal);
                }
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

        private void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
        {
            Vector3[] beamPoints = new Vector3[]
            {
                transform.position,
                jointPosition + new Vector3(0f, beamCurveOffset, 0f),
                downPosition,
                downPosition,
            };
            curvedBeam.SetPoints(beamPoints, beamTraceMaterial ?? pointerMaterial);
            if (pointerVisibility != pointerVisibilityStates.Always_Off)
            {
                curvedBeam.TogglePoints(true);
            }
        }
    }
}