// Panel Menu Item Controller|Prefabs|0071
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactableObject">The GameObject for the interactable object the PanelMenu is attached to.</param>
    public struct PanelMenuItemControllerEventArgs
    {
        public GameObject interactableObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PanelMenuItemControllerEventArgs"/></param>
    public delegate void PanelMenuItemControllerEventHandler(object sender, PanelMenuItemControllerEventArgs e);

    /// <summary>
    /// Purpose: panel item controller class that intercepts the controller events sent from a [PanelMenuController] and passes them onto additional custom event subscriber scripts, which then carry out the required custom UI actions.
    /// </summary>
    /// <remarks>
    /// This script should be attached to a VRTK_InteractableObject > [PanelMenuController] > [panel items container] > child GameObject (See the [PanelMenuController] class for more details on setup structure.).
    /// To show / hide a UI panel, you must first pick up the VRTK_InteractableObject and then by pressing the touchpad top/bottom/left/right you can open/close the child UI panel that has been assigned via the Unity Editor panel.
    /// </remarks>
    /// <example>
    /// `040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.
    /// </example>
    public class PanelMenuItemController : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// Emitted when the panel menu item is showing.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemShowing;
        /// <summary>
        /// Emitted when the panel menu item is hiding.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemHiding;
        /// <summary>
        /// Emitted when the panel menu item is open and the user swipes left on the controller touchpad.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeLeft;
        /// <summary>
        /// Emitted when the panel menu item is open and the user swipes right on the controller touchpad.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeRight;
        /// <summary>
        /// Emitted when the panel menu item is open and the user swipes top on the controller touchpad.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeTop;
        /// <summary>
        /// Emitted when the panel menu item is open and the user swipes bottom on the controller touchpad.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeBottom;
        /// <summary>
        /// Emitted when the panel menu item is open and the user presses the trigger of the controller holding the interactable object.
        /// </summary>
        public event PanelMenuItemControllerEventHandler PanelMenuItemTriggerPressed;

        #endregion Variables

        #region Receive PanelMenuController Actions

        public virtual void Show(GameObject interactableObject)
        {
            gameObject.SetActive(true);
            OnPanelMenuItemShowing(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void Hide(GameObject interactableObject)
        {
            gameObject.SetActive(false);
            OnPanelMenuItemHiding(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void SwipeLeft(GameObject interactableObject)
        {
            OnPanelMenuItemSwipeLeft(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void SwipeRight(GameObject interactableObject)
        {
            OnPanelMenuItemSwipeRight(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void SwipeTop(GameObject interactableObject)
        {
            OnPanelMenuItemSwipeTop(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void SwipeBottom(GameObject interactableObject)
        {
            OnPanelMenuItemSwipeBottom(SetPanelMenuItemEvent(interactableObject));
        }

        public virtual void TriggerPressed(GameObject interactableObject)
        {
            OnPanelMenuItemTriggerPressed(SetPanelMenuItemEvent(interactableObject));
        }

        #endregion Receive PanelMenuController Actions

        #region Send Subscriber Actions

        public virtual void OnPanelMenuItemShowing(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemShowing != null)
            {
                PanelMenuItemShowing(this, e);
            }
        }
        public virtual void OnPanelMenuItemHiding(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemHiding != null)
            {
                PanelMenuItemHiding(this, e);
            }
        }
        public virtual void OnPanelMenuItemSwipeLeft(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemSwipeLeft != null)
            {
                PanelMenuItemSwipeLeft(this, e);
            }
        }

        public virtual void OnPanelMenuItemSwipeRight(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemSwipeRight != null)
            {
                PanelMenuItemSwipeRight(this, e);
            }
        }

        public virtual void OnPanelMenuItemSwipeTop(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemSwipeTop != null)
            {
                PanelMenuItemSwipeTop(this, e);
            }
        }

        public virtual void OnPanelMenuItemSwipeBottom(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemSwipeBottom != null)
            {
                PanelMenuItemSwipeBottom(this, e);
            }
        }

        private void OnPanelMenuItemTriggerPressed(PanelMenuItemControllerEventArgs e)
        {
            if (PanelMenuItemTriggerPressed != null)
            {
                PanelMenuItemTriggerPressed(this, e);
            }
        }

        public PanelMenuItemControllerEventArgs SetPanelMenuItemEvent(GameObject interactableObject)
        {
            PanelMenuItemControllerEventArgs e;
            e.interactableObject = interactableObject;
            return e;
        }

        #endregion Send Subscriber Actions
    }
}