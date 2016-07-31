﻿//====================================================================================
//
// Purpose: Provide basic laser pointer to VR Controller
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The VRTK_ControllerEvents script must also be attached to the Controller
//
// Press the default 'Grip' button on the controller to activate the beam
// Released the default 'Grip' button on the controller to deactivate the beam
//
// This script is an implementation of the VRTK_WorldPointer.
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    public class VRTK_SimplePointer : VRTK_WorldPointer
    {
        public float pointerThickness = 0.002f;
        public float pointerLength = 100f;
        public bool showPointerTip = true;
        public GameObject customPointerCursor;
        [Tooltip("Check this if you don't want to use the beam material for the custom pointer curso.")]
        public bool keepCustomPointerMaterial = false;
        public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

        private GameObject pointerHolder;
        private GameObject pointer;
        private GameObject pointerTip;
        private Vector3 pointerTipScale = new Vector3(0.05f, 0.05f, 0.05f);

        private Quaternion customPointerOriginalRotation;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            InitPointer();
        }

        protected override void Update()
        {
            base.Update();
            if (pointer.gameObject.activeSelf)
            {
                Ray pointerRaycast = new Ray(transform.position, transform.forward);
                RaycastHit pointerCollidedWith;
                var rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith, pointerLength, ~layersToIgnore);
                var pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
                SetPointerTransform(pointerBeamLength, pointerThickness);
            }
        }

        protected override void InitPointer()
        {
            pointerHolder = new GameObject(string.Format("[{0}]WorldPointer_SimplePointer_Holder", gameObject.name));
            Utilities.SetPlayerObject(pointerHolder, VRTK_PlayerObject.ObjectTypes.Pointer);
            pointerHolder.transform.parent = transform;
            pointerHolder.transform.localPosition = Vector3.zero;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.name = string.Format("[{0}]WorldPointer_SimplePointer_Pointer", gameObject.name);
            Utilities.SetPlayerObject(pointer, VRTK_PlayerObject.ObjectTypes.Pointer);
            pointer.transform.parent = pointerHolder.transform;

            pointer.GetComponent<BoxCollider>().isTrigger = true;
            pointer.AddComponent<Rigidbody>().isKinematic = true;
            pointer.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (customPointerCursor == null)
            {
                pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointerTip.transform.localScale = pointerTipScale;
            }
            else
            {
                pointerTip = Instantiate(customPointerCursor);
                pointerTip.transform.localPosition = Vector3.zero;
                customPointerOriginalRotation = pointerTip.transform.rotation;
            }

            pointerTip.transform.name = string.Format("[{0}]WorldPointer_SimplePointer_PointerTip", gameObject.name);
            Utilities.SetPlayerObject(pointerTip, VRTK_PlayerObject.ObjectTypes.Pointer);
            pointerTip.transform.parent = pointerHolder.transform;

            pointerTip.GetComponent<Collider>().isTrigger = true;
            pointerTip.AddComponent<Rigidbody>().isKinematic = true;
            pointerTip.layer = LayerMask.NameToLayer("Ignore Raycast");

            base.InitPointer();

            SetPointerTransform(pointerLength, pointerThickness);
            TogglePointer(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (pointerHolder != null)
            {
                Destroy(pointerHolder);
            }
        }

        protected override void SetPointerMaterial()
        {
            base.SetPointerMaterial();
            pointer.GetComponent<Renderer>().material = pointerMaterial;

            if (keepCustomPointerMaterial)
                return;

            Renderer tipRenderer = pointerTip.GetComponent<Renderer>();
            if(tipRenderer)
                tipRenderer.material = pointerMaterial;
        }

        protected override void TogglePointer(bool state)
        {
            state = (pointerVisibility == pointerVisibilityStates.Always_On ? true : state);
            base.TogglePointer(state);
            pointer.gameObject.SetActive(state);

            var tipState = (showPointerTip ? state : false);
            pointerTip.gameObject.SetActive(tipState);

            Renderer pointerRenderer = pointer.GetComponent<Renderer>();

            if (pointerRenderer && pointerVisibility == pointerVisibilityStates.Always_Off)
            {
                pointerRenderer.enabled = false;
            }
            else if(!pointerRenderer.enabled && pointerVisibility != pointerVisibilityStates.Always_Off)
            {
                pointerRenderer.enabled = true;
            }
        }

        private void SetPointerTransform(float setLength, float setThicknes)
        {
            //if the additional decimal isn't added then the beam position glitches
            var beamPosition = setLength / (2 + 0.00001f);

            pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
            pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);

            if (customPointerCursor == null)
                pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));
            else
            {
                pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength);                
            }

            pointerHolder.transform.localRotation = Quaternion.identity;

            if(customPointerCursor!=null)
                pointerTip.transform.rotation = customPointerOriginalRotation;

            base.SetPlayAreaCursorTransform(pointerTip.transform.position);
        }

        private float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
        {
            var actualLength = pointerLength;

            //reset if beam not hitting or hitting new target
            if (!hasRayHit || (pointerContactTarget && pointerContactTarget != collidedWith.transform))
            {
                if (pointerContactTarget != null)
                {
                    base.PointerOut();
                }

                pointerContactDistance = 0f;
                pointerContactTarget = null;
                destinationPosition = Vector3.zero;

                UpdatePointerMaterial(pointerMissColor);
            }

            //check if beam has hit a new target
            if (hasRayHit)
            {
                pointerContactDistance = collidedWith.distance;
                pointerContactTarget = collidedWith.transform;
                destinationPosition = pointerTip.transform.position;

                UpdatePointerMaterial(pointerHitColor);

                PointerIn();
            }
            else
                pointerTip.SetActive(false);

            //adjust beam length if something is blocking it
            if (hasRayHit && pointerContactDistance < pointerLength)
            {
                actualLength = pointerContactDistance;
            }

            return actualLength;
        }

        protected override void PointerIn()
        {
            base.PointerIn();

            if(showPointerTip)
                pointerTip.SetActive(true);
        }

        public Vector3 GetPointedDirection()
        {
            if (pointer != null)
                return pointer.transform.forward;
            else
                return Vector3.zero;
        }
    }
}