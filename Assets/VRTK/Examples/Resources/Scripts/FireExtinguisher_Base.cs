namespace VRTK.Examples
{
    using System;
    using UnityEngine;

    public class FireExtinguisher_Base : VRTK_InteractableObject
    {
        public Animation leverAnimation;
        public FireExtinguisher_Sprayer sprayer;

        private VRTK_ControllerEvents controllerEvents;
        private VRTK_ControllerActions controllerActions;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
            controllerActions = usingObject.GetComponent<VRTK_ControllerActions>();
        }

        public override void StopUsing(GameObject previousUsingObject)
        {
            base.StopUsing(previousUsingObject);
            controllerEvents = null;
            controllerActions = null;
        }

        protected override void Update()
        {
            base.Update();
            if (controllerEvents)
            {
                var power = controllerEvents.GetTriggerAxis();
                Spray(power);
                controllerActions.TriggerHapticPulse(Convert.ToUInt16(1000f * power), 0.1f, 0.01f);
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