namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK.Controllables;

    public class TogglePointerInteraction : MonoBehaviour
    {
        public enum OptionType
        {
            InteractWithObjects,
            GrabToPointerTip
        }

        public OptionType optionType = OptionType.InteractWithObjects;
        public VRTK_Pointer[] pointers = new VRTK_Pointer[0];
        public VRTK_BaseControllable controllable;
        public Text displayText;
        public string maxText;
        public string minText;


        protected virtual void OnEnable()
        {
            controllable = (controllable == null ? GetComponent<VRTK_BaseControllable>() : controllable);
            if (controllable != null)
            {
                controllable.MaxLimitReached += MaxLimitReached;
                controllable.MinLimitReached += MinLimitReached;
            }
        }

        protected virtual void OnDisable()
        {
            if (controllable != null)
            {
                controllable.MaxLimitReached -= MaxLimitReached;
                controllable.MinLimitReached -= MinLimitReached;
            }
        }

        protected virtual void MaxLimitReached(object sender, ControllableEventArgs e)
        {
            SetOption(true, maxText);
        }

        protected virtual void MinLimitReached(object sender, ControllableEventArgs e)
        {
            SetOption(false, minText);
        }

        protected virtual void SetOption(bool value, string text)
        {
            if (displayText != null)
            {
                displayText.text = text;
            }

            foreach (VRTK_Pointer pointer in pointers)
            {
                pointer.enabled = false;
                pointer.pointerRenderer.enabled = false;
                switch (optionType)
                {
                    case OptionType.InteractWithObjects:
                        pointer.interactWithObjects = value;
                        break;
                    case OptionType.GrabToPointerTip:
                        pointer.grabToPointerTip = value;
                        break;
                }
                pointer.pointerRenderer.enabled = true;
                pointer.enabled = true;
            }
        }
    }
}