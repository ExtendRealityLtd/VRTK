// Base Touchpad Control Action|TouchpadControlActions|25000
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Touchpad Control Action script is an abstract class that all touchpad control action scripts inherit.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseTouchpadControlAction : MonoBehaviour
    {
        public enum AxisDescriptions
        {
            XAxis,
            YAxis
        }

        [Tooltip("A helper parameter to easily identify which axis this Touchpad Control Action is for.")]
        public AxisDescriptions axisDescription;

        /// <summary>
        /// The ProcessFixedUpdate method is run for every FixedUpdate on the Touchpad Control script.
        /// </summary>
        /// <param name="controlledGameObject">The GameObject that is going to be affected.</param>
        /// <param name="directionDevice">The device that is used for the direction.</param>
        /// <param name="axisDirection">The axis that is being affected from the touchpad.</param>
        /// <param name="axis">The value of the current touchpad touch point based across the axis direction.</param>
        /// <param name="deadzone">The value of the deadzone based across the axis direction.</param>
        /// <param name="currentlyFalling">Whether the controlled GameObject is currently falling.</param>
        /// <param name="modifierActive">Whether the modifier button is pressed.</param>
        public abstract void ProcessFixedUpdate(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive);
    }
}