namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_PlayerClimbStamina : VRTK_PlayerClimb
    {
        public enum ClimbingControllers
        {
            Both,
            Left_Only,
            Right_Only,
            None
        }

        public float minReach = 0.05f;
        public float maxReach = 0.8f;
        public float maxStamina = 100f;
        public float staminaRecoveryRate = 20f;
        public float staminaDrainRate = 40f; 
        
        private ClimbingControllers climbingControllers = ClimbingControllers.None;
        private GameObject climbingObjectL;
        private GameObject climbingObjectR;
        private GameObject secondaryHold;
        private Vector3 secondaryHoldStartingPosition;
        private float detachThreshold = 100f;
        private float climbingObjectDistance;
        private float climbingObjectDifficulty = 1.0f;
        private float currentStamina;

        // Accessor for current stamina. Want to be able to set it in case of stamina regen items or penalties
        public float CurrentStamina
        {
            get
            {
                return currentStamina;
            }

            set
            {
                currentStamina = value;
            }
        }

        // Call base class awake and set current stamina to the max value 
        protected override void Awake()
        {
            base.Awake();
            CurrentStamina = maxStamina;
        }

        // Call base class OnEnable and intialize grab listeners
        protected override void OnEnable()
        {
            base.OnEnable();
            InitGrabEventListeners(VRTK_SDK_Bridge.GetControllerLeftHand(), true);
            InitGrabEventListeners(VRTK_SDK_Bridge.GetControllerRightHand(), true);
        }

        // Call base class OnDisable, clean up, and remove grab event listeners
        protected override void OnDisable()
        {
            base.OnDisable();
            climbingObjectL = null;
            climbingObjectR = null;
            climbingControllers = ClimbingControllers.None;
            InitGrabEventListeners(VRTK_SDK_Bridge.GetControllerLeftHand(), false);
            InitGrabEventListeners(VRTK_SDK_Bridge.GetControllerRightHand(), false);
        }

        // Initialize Grab Event listeners
        private void InitGrabEventListeners(GameObject controller, bool state)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController)
                {
                    if (state)
                    {
                        grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                    }
                    else
                    {
                        grabbingController.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                    }
                }
            }
        }

        // When an object is grabbed, check which hand, and set previously grabbed object to secondary hold if one is still being held
        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.controllerIndex == VRTK_DeviceFinder.GetControllerIndex(VRTK_SDK_Bridge.GetControllerLeftHand()))
            {
                climbingObjectL = e.target;
                if (climbingControllers == ClimbingControllers.Right_Only)
                {
                    secondaryHold = climbingObjectR;
                    secondaryHoldStartingPosition = secondaryHold.GetComponent<VRTK_InteractableObject>().GetGrabbingObject().transform.position;
                    climbingControllers = ClimbingControllers.Both;                    
                }
                else
                {
                    secondaryHold = null;
                    climbingControllers = ClimbingControllers.Left_Only;
                }
            }
            else if (e.controllerIndex == VRTK_DeviceFinder.GetControllerIndex(VRTK_SDK_Bridge.GetControllerRightHand()))
            {
                climbingObjectR = e.target;
                if (climbingControllers == ClimbingControllers.Left_Only)
                {
                    secondaryHold = climbingObjectL;
                    secondaryHoldStartingPosition = secondaryHold.GetComponent<VRTK_InteractableObject>().GetGrabbingObject().transform.position;
                    climbingControllers = ClimbingControllers.Both;                    
                }
                else
                {
                    secondaryHold = null;                    
                    climbingControllers = ClimbingControllers.Right_Only;
                }
            }
        }

        // Clear climbing object and secondary hold. Set ClimbingControllers to approriate value
        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            secondaryHold = null;
            if (e.controllerIndex == VRTK_DeviceFinder.GetControllerIndex(VRTK_SDK_Bridge.GetControllerLeftHand()))
            {                
                climbingObjectL = null;
                if (climbingControllers == ClimbingControllers.Both)
                {
                    climbingControllers = ClimbingControllers.Right_Only;
                    secondaryHold = null;
                }
                else
                {
                    climbingControllers = ClimbingControllers.None;
                }
            }
            else if (e.controllerIndex == VRTK_DeviceFinder.GetControllerIndex(VRTK_SDK_Bridge.GetControllerRightHand()))
            {
                climbingObjectR = null;
                if (climbingControllers == ClimbingControllers.Both)
                {
                    climbingControllers = ClimbingControllers.Left_Only;
                    secondaryHold = null;
                }
                else
                {
                    climbingControllers = ClimbingControllers.None;
                }
            }
        }

        // Update base class, then make updates based on current climbing hands 
        protected override void Update()
        {
            base.Update();

            if (climbingControllers == ClimbingControllers.Both) 
            {
                // Regen Stamina     
                UpdateBothHands();
            }
            else if (climbingControllers == ClimbingControllers.Left_Only) 
            {
                // Remove Stamina
                UpdateSingleHand(climbingObjectL);               
            }
            else if (climbingControllers == ClimbingControllers.Right_Only)
            {
                // Remove Stamina
                UpdateSingleHand(climbingObjectR);
            }
            else if (climbingControllers == ClimbingControllers.None) 
            {
                // Regen Stamina
                UpdateNoHands();
            }
        }

        // Update both hands
        protected void UpdateBothHands()
        {
            if (secondaryHold)
            {
                float distance = Vector3.Distance(secondaryHold.GetComponent<VRTK_InteractableObject>().GetGrabbingObject().transform.position, secondaryHoldStartingPosition);
                if (distance > (detachThreshold / 1000))
                {
                    secondaryHold.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
                    secondaryHold = null;
                }
            }
            CurrentStamina = Mathf.Min(CurrentStamina + (Time.deltaTime * staminaRecoveryRate), maxStamina);
        }

        // Update single hand
        protected void UpdateSingleHand(GameObject grabbedObject)
        {
            climbingObjectDistance = GetNormalizedClimbDistance(grabbedObject.GetComponent<VRTK_InteractableObject>().GetGrabbingObject());
            CurrentStamina = Mathf.Max(CurrentStamina - (Time.deltaTime * staminaDrainRate * climbingObjectDistance * climbingObjectDistance * climbingObjectDifficulty), 0.0f);
            if (CurrentStamina == 0f)
            {               
                grabbedObject.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
            }
        }

        // Update when nothhing is currently being grabbed
        protected void UpdateNoHands()
        {
            CurrentStamina = Mathf.Min(CurrentStamina + 2 * (Time.deltaTime * staminaRecoveryRate), maxStamina);
        }

        // Get a normalized climb value between 0 and 1 based on reach distances and current hand distance from head.
        protected float GetNormalizedClimbDistance(GameObject climbingObject)
        {
            float normalizedDistance = Mathf.Pow(climbingObject.transform.position.x - headCamera.position.x, 2) + Mathf.Pow(climbingObject.transform.position.z - headCamera.position.z, 2);
            normalizedDistance = Mathf.Clamp01((normalizedDistance - minReach * minReach) / (maxReach * maxReach - minReach * minReach));
            return normalizedDistance;
        }
    }
}