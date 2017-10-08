namespace VRTK.Examples
{
    using UnityEngine;
    using VRTK.Controllables.PhysicsBased;

    public class ButtonReactor : MonoBehaviour
    {
        protected VRTK_Button buttonEvents;

        protected virtual void OnEnable()
        {
            buttonEvents = GetComponent<VRTK_Button>();
            if (buttonEvents != null)
            {
                buttonEvents.MaxLimitReached += MaxLimitReached;
            }
        }

        protected virtual void OnDisable()
        {
            if (buttonEvents != null)
            {
                buttonEvents.MaxLimitReached -= MaxLimitReached;
            }
        }

        private void MaxLimitReached(object sender, Controllables.ControllableEventArgs e)
        {
            VRTK_Button senderButton = sender as VRTK_Button;
            VRTK_Logger.Info(senderButton.name + " was pushed");
        }
    }
}