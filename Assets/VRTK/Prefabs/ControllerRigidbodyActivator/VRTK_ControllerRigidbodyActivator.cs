// Controller Rigidbody Activator|Prefabs|0050
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
    /// Provides a simple trigger collider volume that when a controller enters will enable the rigidbody on the controller.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/ControllerRigidbodyActivator/ControllerRigidbodyActivator` prefab in the scene at the location where the controller rigidbody should be automatically activated.
    ///  * The prefab contains a default sphere collider to determine ths collision, this collider component can be customised in the inspector or can be replaced with another collider component (set to `Is Trigger`).
    ///
    ///   > If the prefab is placed as a child of the target Interactable Object then the collider volume on the prefab will trigger collisions on the Interactable Object.
    /// </remarks>
    public class VRTK_ControllerRigidbodyActivator : MonoBehaviour
    {
        [Tooltip("If this is checked then the Collider will have it's Rigidbody toggled on and off during a collision.")]
        public bool isEnabled = true;
        [Tooltip("If this is checked then the Rigidbody Activator will activate the rigidbody and colliders on the Interact Touch script.")]
        public bool activateInteractTouch = true;
        [Tooltip("If this is checked then the Rigidbody Activator will activate the rigidbody and colliders on the Controller Tracked Collider script.")]
        public bool activateTrackedCollider = false;

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
            if (isEnabled || !state)
            {
                if (activateTrackedCollider)
                {
                    VRTK_ControllerTrackedCollider trackedCollider = collider.GetComponentInParent<VRTK_ControllerTrackedCollider>();
                    if (trackedCollider != null)
                    {
                        trackedCollider.ToggleColliders(state);
                        EmitEvent(state, trackedCollider.interactTouch);
                    }
                }

                if (activateInteractTouch)
                {
                    VRTK_InteractTouch touch = collider.GetComponentInParent<VRTK_InteractTouch>();
                    if (touch != null)
                    {
                        touch.ToggleControllerRigidBody(state, state);
                        EmitEvent(state, touch);
                    }
                }
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