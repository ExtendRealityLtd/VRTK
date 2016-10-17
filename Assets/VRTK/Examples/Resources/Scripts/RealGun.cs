namespace VRTK.Examples
{
    using UnityEngine;

    public class RealGun : VRTK_InteractableObject
    {
        public float bulletSpeed = 200f;
        public float bulletLife = 5f;

        private GameObject bullet;
        private GameObject trigger;
        private RealGun_Slide slide;
        private RealGun_SafetySwitch safetySwitch;

        private Rigidbody slideRigidbody;
        private Collider slideCollider;
        private Rigidbody safetySwitchRigidbody;
        private Collider safetySwitchCollider;

        private VRTK_ControllerEvents controllerEvents;
        private VRTK_ControllerActions controllerActions;

        private float minTriggerRotation = -10f;
        private float maxTriggerRotation = 45f;

        private void ToggleCollision(Rigidbody objRB, Collider objCol, bool state)
        {
            objRB.isKinematic = state;
            objCol.isTrigger = state;
        }

        private void ToggleSlide(bool state)
        {
            if (!state)
            {
                slide.ForceStopInteracting();
            }
            slide.enabled = state;
            slide.isGrabbable = state;
            ToggleCollision(slideRigidbody, slideCollider, state);
        }

        private void ToggleSafetySwitch(bool state)
        {
            if (!state)
            {
                safetySwitch.ForceStopInteracting();
            }
            ToggleCollision(safetySwitchRigidbody, safetySwitchCollider, state);
        }

        public override void Grabbed(GameObject currentGrabbingObject)
        {
            base.Grabbed(currentGrabbingObject);

            controllerEvents = currentGrabbingObject.GetComponent<VRTK_ControllerEvents>();
            controllerActions = currentGrabbingObject.GetComponent<VRTK_ControllerActions>();

            ToggleSlide(true);
            ToggleSafetySwitch(true);

            //Limit hands grabbing when picked up
            if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject) == VRTK_DeviceFinder.ControllerHand.Left)
            {
                allowedTouchControllers = AllowedController.Left_Only;
                allowedUseControllers = AllowedController.Left_Only;
                slide.allowedGrabControllers = AllowedController.Right_Only;
                safetySwitch.allowedGrabControllers = AllowedController.Right_Only;
            }
            else if (VRTK_DeviceFinder.GetControllerHand(currentGrabbingObject) == VRTK_DeviceFinder.ControllerHand.Right)
            {
                allowedTouchControllers = AllowedController.Right_Only;
                allowedUseControllers = AllowedController.Right_Only;
                slide.allowedGrabControllers = AllowedController.Left_Only;
                safetySwitch.allowedGrabControllers = AllowedController.Left_Only;
            }
        }

        public override void Ungrabbed(GameObject previousGrabbingObject)
        {
            base.Ungrabbed(previousGrabbingObject);

            ToggleSlide(false);
            ToggleSafetySwitch(false);

            //Unlimit hands
            allowedTouchControllers = AllowedController.Both;
            allowedUseControllers = AllowedController.Both;
            slide.allowedGrabControllers = AllowedController.Both;
            safetySwitch.allowedGrabControllers = AllowedController.Both;

            controllerEvents = null;
            controllerActions = null;
        }

        public override void StartUsing(GameObject currentUsingObject)
        {
            base.StartUsing(currentUsingObject);
            if (safetySwitch.safetyOff)
            {
                slide.Fire();
                FireBullet();
                controllerActions.TriggerHapticPulse(2500, 0.2f, 0.01f);
            }
            else
            {
                controllerActions.TriggerHapticPulse(300, 0.1f, 0.01f);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            bullet = transform.Find("Bullet").gameObject;
            bullet.SetActive(false);

            trigger = transform.FindChild("TriggerHolder").gameObject;

            slide = transform.FindChild("Slide").GetComponent<RealGun_Slide>();
            slideRigidbody = slide.GetComponent<Rigidbody>();
            slideCollider = slide.GetComponent<Collider>();

            safetySwitch = transform.FindChild("SafetySwitch").GetComponent<RealGun_SafetySwitch>();
            safetySwitchRigidbody = safetySwitch.GetComponent<Rigidbody>();
            safetySwitchCollider = safetySwitch.GetComponent<Collider>();
        }

        protected override void Update()
        {
            base.Update();
            if (controllerEvents)
            {
                var pressure = (maxTriggerRotation * controllerEvents.GetTriggerAxis()) - minTriggerRotation;
                trigger.transform.localEulerAngles = new Vector3(0f, pressure, 0f);
            }
            else
            {
                trigger.transform.localEulerAngles = new Vector3(0f, minTriggerRotation, 0f);
            }
        }

        private void FireBullet()
        {
            GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
            bulletClone.SetActive(true);
            Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
            rb.AddForce(bullet.transform.forward * bulletSpeed);
            Destroy(bulletClone, bulletLife);
        }
    }
}