// Controller Rigidbody Activator|Prefabs|0033
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The object that touching the activator.</param>
    public struct ControllerRigidbodyActivatorEventArgs
    {
        public VRTK_InteractTouch touchingObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerRigidbodyActivatorEventArgs"/></param>
    public delegate void ControllerRigidbodyActivatorEventHandler(object sender, ControllerRigidbodyActivatorEventArgs e);

    /// <summary>
    /// This adds a simple trigger collider volume that when a controller enters will enable the rigidbody on the controller.
    /// </summary>
    /// <remarks>
    /// The prefab game object should be placed in the scene where another interactable game object (such as a button control) is located to turn the controller rigidbody on at the appropriate time for interaction with the control without needing to manually activate by pressing the grab.
    /// 
    /// If the prefab is placed as a child of the target interactable game object then the collider volume on the prefab will trigger collisions on the interactable object.
    ///
    /// The sphere collider on the prefab can have the radius adjusted to determine how close the controller needs to be to the object before the rigidbody is activated.
    ///
    /// It's also possible to replace the sphere trigger collider with an alternative trigger collider for customised collision detection.
    /// </remarks>
    public class VRTK_ControllerRigidbodyActivator : MonoBehaviour
    {
        [Tooltip("If this is checked then the collider will have it's rigidbody toggled on and off during a collision.")]
        public bool isEnabled = true;

        /// <summary>
        /// Emitted when the controller rigidbody is turned on.
        /// </summary>
        public event ControllerRigidbodyActivatorEventHandler ControllerRigidbodyOn;
        /// <summary>
        /// Emitted when the controller rigidbody is turned off.
        /// </summary>
        public event ControllerRigidbodyActivatorEventHandler ControllerRigidbodyOff;

        public virtual void OnControllerRigidbodyOn(ControllerRigidbodyActivatorEventArgs e)
        {
            if (ControllerRigidbodyOn != null)
            {
                ControllerRigidbodyOn(this, e);
            }
        }

        public virtual void OnControllerRigidbodyOff(ControllerRigidbodyActivatorEventArgs e)
        {
            if (ControllerRigidbodyOff != null)
            {
                ControllerRigidbodyOff(this, e);
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            ToggleRigidbody(collider, true);
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            ToggleRigidbody(collider, false);
        }

        protected virtual void ToggleRigidbody(Collider collider, bool state)
        {
            VRTK_InteractTouch touch = collider.GetComponentInParent<VRTK_InteractTouch>();
            if (touch != null && (isEnabled || !state))
            {
                touch.ToggleControllerRigidBody(state, state);
                EmitEvent(state, touch);
            }
        }

        protected virtual void EmitEvent(bool state, VRTK_InteractTouch touch)
        {
            ControllerRigidbodyActivatorEventArgs e;
            e.touchingObject = touch;
            if (state)
            {
                OnControllerRigidbodyOn(e);
            }
            else
            {
                OnControllerRigidbodyOff(e);
            }
        }
    }
}