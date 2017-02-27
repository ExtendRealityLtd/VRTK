// Bezier Pointer Renderer|PointerRenderers|10030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Bezier Pointer Renderer emits a curved line (made out of game objects) from the end of the attached object to a point on a ground surface (at any height).
    /// </summary>
    /// <remarks>
    /// It is more useful than the Simple Pointer Renderer for traversing objects of various heights as the end point can be curved on top of objects that are not visible to the user.
    ///
    ///   > The bezier curve generation code is in another script located at `VRTK/Scripts/Internal/VRTK_CurveGenerator.cs` and was heavily inspired by the tutorial and code from [Catlike Coding](http://catlikecoding.com/unity/tutorials/curves-and-splines/).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/009_Controller_BezierPointer` is used in conjunction with the Height Adjust Teleporter shows how it is possible to traverse different height objects using the curved pointer without needing to see the top of the object.
    ///
    /// `VRTK/Examples/036_Controller_CustomCompoundPointer' shows how to display an object (a teleport beam) only if the teleport location is valid, and can create an animated trail along the tracer curve.
    /// </example>
    public class VRTK_BezierPointerRenderer : VRTK_BasePointerRenderer
    {
        [Header("Bezier Pointer Appearance Settings")]

        [Tooltip("The maximum length of the projected forward beam.")]
        public float maximumLength = 10f;
        [Tooltip("The number of items to render in the bezier curve tracer beam. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.")]
        public int tracerDensity = 10;
        [Tooltip("The size of the ground cursor. This number also affects the size of the objects in the bezier curve tracer beam. The larger the radius, the larger the objects will be.")]
        public float cursorRadius = 0.5f;

        [Header("Bezier Pointer Render Settings")]

        [Tooltip("The maximum angle in degrees of the origin before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.")]
        [Range(1, 100)]
        public float heightLimitAngle = 100f;
        [Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
        public float curveOffset = 1f;
        [Tooltip("Rescale each tracer element according to the length of the Bezier curve.")]
        public bool rescaleTracer = false;
        [Tooltip("The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
        public bool cursorMatchTargetRotation = false;
        [Tooltip("The number of points along the bezier curve to check for an early beam collision. Useful if the bezier curve is appearing to clip through teleport locations. 0 won't make any checks and it will be capped at `Pointer Density`. The higher the number, the more CPU intensive the checks become.")]
        public int collisionCheckFrequency = 0;

        [Header("Bezier Pointer Custom Appearance Settings")]

        [Tooltip("A custom game object to use as the appearance for the pointer tracer. If this is empty then a collection of Sphere primitives will be created and used.")]
        public GameObject customTracer;
        [Tooltip("A custom game object to use as the appearance for the pointer cursor. If this is empty then a Cylinder primitive will be created and used.")]
        public GameObject customCursor;
        [Tooltip("A custom game object can be applied here to appear only if the location is valid.")]
        public GameObject validLocationObject = null;
        [Tooltip("A custom game object can be applied here to appear only if the location is invalid.")]
        public GameObject invalidLocationObject = null;

        protected VRTK_CurveGenerator actualTracer;
        protected GameObject actualContainer;
        protected GameObject actualCursor;
        protected GameObject actualValidLocationObject = null;
        protected GameObject actualInvalidLocationObject = null;
        protected Vector3 fixedForwardBeamForward;

        /// <summary>
        /// The UpdateRenderer method is used to run an Update routine on the pointer.
        /// </summary>
        public override void UpdateRenderer()
        {
            if ((controllingPointer && controllingPointer.IsPointerActive()) || IsVisible())
            {
                Vector3 jointPosition = ProjectForwardBeam();
                Vector3 downPosition = ProjectDownBeam(jointPosition);
                AdjustForEarlyCollisions(jointPosition, downPosition);
                MakeRenderersVisible();
            }
            base.UpdateRenderer();
        }

        protected override void ToggleRenderer(bool pointerState, bool actualState)
        {
            TogglePointerCursor(pointerState, actualState);
            TogglePointerTracer(pointerState, actualState);
            if (actualTracer != null && actualState && tracerVisibility != VisibilityStates.AlwaysOn)
            {
                ToggleRendererVisibility(actualTracer.gameObject, false);
                AddVisibleRenderer(actualTracer.gameObject);
            }
        }

        protected override void CreatePointerObjects()
        {
            actualContainer = new GameObject(string.Format("[{0}]BezierPointerRenderer_Container", gameObject.name));
            VRTK_PlayerObject.SetPlayerObject(actualContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
            actualContainer.SetActive(false);

            CreateTracer();
            CreateCursor();
            Toggle(false, false);
            if (controllingPointer)
            {
                controllingPointer.ResetActivationTimer(true);
                controllingPointer.ResetSelectionTimer(true);
            }
        }

        protected override void DestroyPointerObjects()
        {
            if (actualCursor != null)
            {
                Destroy(actualCursor);
            }
            if (actualTracer != null)
            {
                Destroy(actualTracer);
            }
            if (actualContainer != null)
            {
                Destroy(actualContainer);
            }
        }

        protected override void UpdateObjectInteractor()
        {
            base.UpdateObjectInteractor();
            //if the object interactor is too far from the pointer tip then set it to the pointer tip position to prevent glitching.
            if (objectInteractor && actualCursor && Vector3.Distance(objectInteractor.transform.position, actualCursor.transform.position) > 0f)
            {
                objectInteractor.transform.position = actualCursor.transform.position;
            }
        }

        protected override void ChangeMaterial(Color givenColor)
        {
            base.ChangeMaterial(givenColor);
            ChangeMaterialColor(actualCursor, givenColor);
        }

        protected virtual void CreateTracer()
        {
            actualTracer = actualContainer.gameObject.AddComponent<VRTK_CurveGenerator>();
            actualTracer.transform.SetParent(null);
            actualTracer.Create(tracerDensity, cursorRadius, customTracer, rescaleTracer);
        }

        protected virtual GameObject CreateCursorObject()
        {
            float cursorYOffset = 0.02f;
            GameObject createdCursor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            MeshRenderer createdCursorRenderer = createdCursor.GetComponent<MeshRenderer>();

            createdCursor.transform.localScale = new Vector3(cursorRadius, cursorYOffset, cursorRadius);
            createdCursorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            createdCursorRenderer.receiveShadows = false;
            createdCursorRenderer.material = defaultMaterial;
            Destroy(createdCursor.GetComponent<CapsuleCollider>());
            return createdCursor;
        }

        protected virtual void CreateCursorLocations()
        {
            if (validLocationObject != null)
            {
                actualValidLocationObject = Instantiate(validLocationObject);
                actualValidLocationObject.name = string.Format("[{0}]BezierPointerRenderer_ValidLocation", gameObject.name);
                actualValidLocationObject.transform.SetParent(actualCursor.transform);
                actualValidLocationObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                actualValidLocationObject.SetActive(false);
            }

            if (invalidLocationObject != null)
            {
                actualInvalidLocationObject = Instantiate(invalidLocationObject);
                actualInvalidLocationObject.name = string.Format("[{0}]BezierPointerRenderer_InvalidLocation", gameObject.name);
                actualInvalidLocationObject.transform.SetParent(actualCursor.transform);
                actualInvalidLocationObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                actualInvalidLocationObject.SetActive(false);
            }
        }

        protected virtual void CreateCursor()
        {
            actualCursor = (customCursor ? Instantiate(customCursor) : CreateCursorObject());
            CreateCursorLocations();
            actualCursor.name = string.Format("[{0}]BezierPointerRenderer_Cursor", gameObject.name);
            VRTK_PlayerObject.SetPlayerObject(actualCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
            actualCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
            actualCursor.SetActive(false);
        }

        protected virtual Vector3 ProjectForwardBeam()
        {
            Transform origin = GetOrigin();
            float attachedRotation = Vector3.Dot(Vector3.up, origin.forward.normalized);
            float calculatedLength = maximumLength;
            Vector3 useForward = origin.forward;
            if ((attachedRotation * 100f) > heightLimitAngle)
            {
                useForward = new Vector3(useForward.x, fixedForwardBeamForward.y, useForward.z);
                var controllerRotationOffset = 1f - (attachedRotation - (heightLimitAngle / 100f));
                calculatedLength = (maximumLength * controllerRotationOffset) * controllerRotationOffset;
            }
            else
            {
                fixedForwardBeamForward = origin.forward;
            }

            var actualLength = calculatedLength;
            Ray pointerRaycast = new Ray(origin.position, useForward);

            RaycastHit collidedWith;
            var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, calculatedLength, ~layersToIgnore);

            float contactDistance = 0f;
            //reset if beam not hitting or hitting new target
            if (!hasRayHit || (destinationHit.collider && destinationHit.collider != collidedWith.collider))
            {
                contactDistance = 0f;
            }

            //check if beam has hit a new target
            if (hasRayHit)
            {
                contactDistance = collidedWith.distance;
            }

            //adjust beam length if something is blocking it
            if (hasRayHit && contactDistance < calculatedLength)
            {
                actualLength = contactDistance;
            }

            //Use BEAM_ADJUST_OFFSET to move point back and up a bit to prevent beam clipping at collision point
            return (pointerRaycast.GetPoint(actualLength - BEAM_ADJUST_OFFSET) + (Vector3.up * BEAM_ADJUST_OFFSET));
        }

        protected virtual Vector3 ProjectDownBeam(Vector3 jointPosition)
        {
            Vector3 downPosition = Vector3.zero;
            Ray projectedBeamDownRaycast = new Ray(jointPosition, Vector3.down);
            RaycastHit collidedWith;

            var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity, ~layersToIgnore);

            if (!downRayHit || (destinationHit.collider && destinationHit.collider != collidedWith.collider))
            {
                if (destinationHit.collider != null)
                {
                    PointerExit(destinationHit);
                }
                destinationHit = new RaycastHit();
                downPosition = projectedBeamDownRaycast.GetPoint(0f);
            }

            if (downRayHit)
            {
                downPosition = projectedBeamDownRaycast.GetPoint(collidedWith.distance);
                PointerEnter(collidedWith);
                destinationHit = collidedWith;
            }
            return downPosition;
        }

        protected virtual void AdjustForEarlyCollisions(Vector3 jointPosition, Vector3 downPosition)
        {
            Vector3 newDownPosition = downPosition;
            Vector3 newJointPosition = jointPosition;

            if (collisionCheckFrequency > 0 && actualTracer != null)
            {
                collisionCheckFrequency = Mathf.Clamp(collisionCheckFrequency, 0, tracerDensity);
                Vector3[] beamPoints = new Vector3[]
                {
                    GetOrigin().position,
                    jointPosition + new Vector3(0f, curveOffset, 0f),
                    downPosition,
                    downPosition,
                };

                Vector3[] checkPoints = actualTracer.GetPoints(beamPoints);
                int checkFrequency = tracerDensity / collisionCheckFrequency;

                for (int i = 0; i < tracerDensity - checkFrequency; i += checkFrequency)
                {
                    var currentPoint = checkPoints[i];
                    var nextPoint = (i + checkFrequency < checkPoints.Length ? checkPoints[i + checkFrequency] : checkPoints[checkPoints.Length - 1]);
                    var nextPointDirection = (nextPoint - currentPoint).normalized;
                    var nextPointDistance = Vector3.Distance(currentPoint, nextPoint);

                    Ray checkCollisionRay = new Ray(currentPoint, nextPointDirection);
                    RaycastHit checkCollisionHit;

                    if (Physics.Raycast(checkCollisionRay, out checkCollisionHit, nextPointDistance, ~layersToIgnore))
                    {
                        var collisionPoint = checkCollisionRay.GetPoint(checkCollisionHit.distance);
                        Ray downwardCheckRay = new Ray(collisionPoint + (Vector3.up * 0.01f), Vector3.down);
                        RaycastHit downwardCheckHit;

                        if (Physics.Raycast(downwardCheckRay, out downwardCheckHit, float.PositiveInfinity, ~layersToIgnore))
                        {
                            destinationHit = downwardCheckHit;
                            newDownPosition = downwardCheckRay.GetPoint(downwardCheckHit.distance); ;
                            newJointPosition = (newDownPosition.y < jointPosition.y ? new Vector3(newDownPosition.x, jointPosition.y, newDownPosition.z) : jointPosition);
                            break;
                        }
                    }
                }
            }

            DisplayCurvedBeam(newJointPosition, newDownPosition);
            SetPointerCursor();
        }

        protected virtual void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
        {
            if (actualTracer != null)
            {
                Vector3[] beamPoints = new Vector3[]
                {
                GetOrigin(false).position,
                jointPosition + new Vector3(0f, curveOffset, 0f),
                downPosition,
                downPosition,
                };
                var tracerMaterial = (customTracer ? null : defaultMaterial);
                actualTracer.SetPoints(beamPoints, tracerMaterial, currentColor);
                if (tracerVisibility == VisibilityStates.AlwaysOff)
                {
                    TogglePointerTracer(false, false);
                }
                else if (controllingPointer)
                {
                    TogglePointerTracer(controllingPointer.IsPointerActive(), controllingPointer.IsPointerActive());
                }
            }
        }

        protected virtual void TogglePointerCursor(bool pointerState, bool actualState)
        {
            ToggleElement(actualCursor, pointerState, actualState, cursorVisibility, ref cursorVisible);
        }

        protected virtual void TogglePointerTracer(bool pointerState, bool actualState)
        {
            tracerVisible = (tracerVisibility == VisibilityStates.AlwaysOn ? true : pointerState);
            if (actualTracer != null)
            {
                actualTracer.TogglePoints(tracerVisible);
            }
        }

        protected virtual void SetPointerCursor()
        {
            if (controllingPointer && destinationHit.transform)
            {
                TogglePointerCursor(controllingPointer.IsPointerActive(), controllingPointer.IsPointerActive());
                actualCursor.transform.position = destinationHit.point;
                if (cursorMatchTargetRotation)
                {
                    actualCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, destinationHit.normal);
                }
                base.UpdateDependencies(actualCursor.transform.position);

                ChangeColor(validCollisionColor);
                if (actualValidLocationObject)
                {
                    actualValidLocationObject.SetActive(ValidDestination() && IsValidCollision());
                }
                if (actualInvalidLocationObject)
                {
                    actualInvalidLocationObject.SetActive(!ValidDestination() || !IsValidCollision());
                }
            }
            else
            {
                TogglePointerCursor(false, false);
                ChangeColor(invalidCollisionColor);
            }
        }
    }
}