namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK.Controllables.ArtificialBased;

    [System.Serializable]
    public class ToggleGameObjectSliderOptions
    {
        public GameObject option;
        public string description;
    }

    public class ToggleGameObjectSlider : MonoBehaviour
    {
        public VRTK_ArtificialSlider slider;
        public Text descriptionText;
        public ToggleGameObjectSliderOptions[] options;

        protected virtual void OnEnable()
        {
            if (slider != null)
            {
                slider.ValueChanged += ValueChanged;
            }
            ToggleOption(0);
        }

        protected virtual void OnDisable()
        {
            if (slider != null)
            {
                slider.ValueChanged -= ValueChanged;
            }
        }

        protected virtual void ValueChanged(object sender, Controllables.ControllableEventArgs e)
        {
            ToggleOption(Mathf.RoundToInt(e.value));
        }

        protected virtual void ToggleOption(int index)
        {
            foreach (ToggleGameObjectSliderOptions currentOption in options)
            {
                currentOption.option.SetActive(false);
            }

            ToggleGameObjectSliderOptions updateOption = options[index];
            updateOption.option.SetActive(true);
            descriptionText.text = updateOption.description;
        }
    }
}