namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK.Controllables;

    public class ToggleGameObject : MonoBehaviour
    {
        public VRTK_BaseControllable controllable;
        public Text displayText;
        public Text descriptionText;
        public GameObject toggleObject;
        public string onText = "On";
        public string offText = "Off";
        public string description = "";

        protected VRTK_InteractableObject io;

        protected virtual void OnEnable()
        {
            if (controllable != null)
            {
                controllable.MaxLimitReached += MaxLimitReached;
                controllable.MinLimitReached += MinLimitReached;
            }
            Invoke("SetupIOListeners", 0.1f);
        }

        protected virtual void OnDisable()
        {
            if (controllable != null)
            {
                controllable.MaxLimitReached -= MaxLimitReached;
                controllable.MinLimitReached -= MinLimitReached;
            }

            if (io != null)
            {
                io.InteractableObjectTouched -= InteractableObjectTouched;
            }
        }

        protected virtual void SetupIOListeners()
        {
            io = controllable.GetComponentInParent<VRTK_InteractableObject>();
            if (io != null)
            {
                io.InteractableObjectTouched += InteractableObjectTouched;
            }
        }

        protected virtual void MinLimitReached(object sender, ControllableEventArgs e)
        {
            ToggleObject(false);
            UpdateText(offText);
        }

        protected virtual void MaxLimitReached(object sender, ControllableEventArgs e)
        {
            ToggleObject(true);
            UpdateText(onText);
        }

        protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            if (descriptionText != null)
            {
                descriptionText.text = description;
            }
        }

        protected virtual void ToggleObject(bool state)
        {
            if (toggleObject != null)
            {
                toggleObject.SetActive(state);
            }
        }

        protected virtual void UpdateText(string text)
        {
            if (displayText != null)
            {
                displayText.text = text;
            }
        }
    }
}