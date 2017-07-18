﻿// Controller Rigidbody Activator|Prefabs|0033
namespace VRTK
{
    using UnityEngine;

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

        protected virtual void OnTriggerEnter(Collider collider)
        {
            ToggleRigidbody(collider, true);
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            ToggleRigidbody(collider, false);
        }

        private void ToggleRigidbody(Collider collider, bool state)
        {
            var touch = collider.GetComponentInParent<VRTK_InteractTouch>();
            if (touch && (isEnabled || !state))
            {
                touch.ToggleControllerRigidBody(state, state);
            }
        }
    }
}