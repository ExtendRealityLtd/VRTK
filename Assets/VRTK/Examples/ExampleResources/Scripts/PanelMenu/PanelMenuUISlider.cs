namespace VRTK.Examples.PanelMenu
{
    // Panel Menu UI Slider

    using UnityEngine;
    using UnityEngine.UI;
    using VRTK;

    /// <summary>
    ///  Demo component for example scene.
    /// </summary>
    /// <example>
    /// See the demo scene for a complete example: [ 040_Controls_Panel_Menu ] 
    /// </example>
    public class PanelMenuUISlider : MonoBehaviour
    {
        #region Variables

        private Slider slider;

        #endregion Variables

        #region Unity Methods

        private void Start()
        {
            slider = GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogWarning("The PanelMenuUISlider could not automatically find the UI Slider component.");
                return;
            }

            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeLeft += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeLeft);
            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeRight += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeRight);
        }

        #endregion Unity Methods

        #region UI Events

        private void OnPanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
        {
            slider.value -= 1;
            SendMessageToInteractableObject(e.interactableObject);
        }

        private void OnPanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
        {
            slider.value += 1;
            SendMessageToInteractableObject(e.interactableObject);
        }

        private void SendMessageToInteractableObject(GameObject interactableObject)
        {
            interactableObject.SendMessage("UpdateSliderValue", slider.value);
        }

        #endregion UI Events
    }
}