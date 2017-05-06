namespace VRTK.Examples
{
    using System;
    using UnityEngine;

    public class FireExtinguisher_Base : VRTK_InteractableObject
    {
        public Animation leverAnimation;
        public FireExtinguisher_Sprayer sprayer;

        private VRTK_ControllerEvents controllerEvents;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
        }

        public override void StopUsing(GameObject previousUsingObject)
        {
            base.StopUsing(previousUsingObject);
            controllerEvents = null;
        }

        protected override void Update()
        {
            base.Update();
            if (controllerEvents)
            {
                float power = controllerEvents.GetTriggerAxis();
                Spray(power);
                VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(controllerEvents.gameObject), power * 0.25f, 0.1f, 0.01f);
            }
            else
            {
                Spray(0f);
            }
        }

        private void Spray(float power)
        {
            SetHandleFrame(power);
            sprayer.Spray(power);
        }

        private void SetHandleFrame(float frame)
        {
            leverAnimation["FireExtinguisherLever"].speed = 0;
            leverAnimation["FireExtinguisherLever"].time = frame;
            leverAnimation.Play("FireExtinguisherLever");
        }
    }
}