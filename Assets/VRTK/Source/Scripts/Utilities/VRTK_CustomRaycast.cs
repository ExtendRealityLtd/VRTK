// Custom Raycast|Utilities|90045
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// A Custom Raycast allows to specify custom options for a Physics.Raycast.
    /// </summary>
    /// <remarks>
    /// A number of other scripts can utilise a Custom Raycast to further customise the raycasts that the scripts use internally.
    ///
    /// For example, the VRTK_BodyPhysics script can be set to ignore trigger colliders when casting to see if it should teleport up or down to the nearest floor.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_CustomRaycast")]
    public class VRTK_CustomRaycast : MonoBehaviour
    {
        [Tooltip("The layers to ignore when raycasting.")]
        public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;
        [Tooltip("Determines whether the ray will interact with trigger colliders.")]
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal;

        /// <summary>
        /// The Raycast method is used to generate a raycast either from the given CustomRaycast object or a default Physics.Raycast.
        /// </summary>
        /// <param name="customCast">The optional object with customised cast parameters.</param>
        /// <param name="ray">The Ray to cast with.</param>
        /// <param name="hitData">The raycast hit data.</param>
        /// <param name="ignoreLayers">A layermask of layers to ignore from the raycast.</param>
        /// <param name="length">The maximum length of the raycast.</param>
        /// <param name="affectTriggers">Determines the trigger interaction level of the cast.</param>
        /// <returns>Returns true if the raycast successfully collides with a valid object.</returns>
        public static bool Raycast(VRTK_CustomRaycast customCast, Ray ray, out RaycastHit hitData, LayerMask ignoreLayers, float length = Mathf.Infinity, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)
        {
            if (customCast != null)
            {
                return customCast.CustomRaycast(ray, out hitData, length);
            }
            else
            {
                return Physics.Raycast(ray, out hitData, length, ~ignoreLayers, affectTriggers);
            }
        }

        /// <summary>
        /// The Linecast method is used to generate a linecast either from the given CustomRaycast object or a default Physics.Linecast.
        /// </summary>
        /// <param name="customCast">The optional object with customised cast parameters.</param>
        /// <param name="startPosition">The world position to start the linecast from.</param>
        /// <param name="endPosition">The world position to end the linecast at.</param>
        /// <param name="hitData">The linecast hit data.</param>
        /// <param name="ignoreLayers">A layermask of layers to ignore from the linecast.</param>
        /// <param name="affectTriggers">Determines the trigger interaction level of the cast.</param>
        /// <returns>Returns true if the linecast successfully collides with a valid object.</returns>
        public static bool Linecast(VRTK_CustomRaycast customCast, Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData, LayerMask ignoreLayers, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)
        {
            if (customCast != null)
            {
                return customCast.CustomLinecast(startPosition, endPosition, out hitData);
            }
            else
            {
                return Physics.Linecast(startPosition, endPosition, out hitData, ~ignoreLayers, affectTriggers);
            }
        }

        /// <summary>
        /// The CapsuleCast method is used to generate a linecast either from the given CustomRaycast object or a default Physics.Linecast.
        /// </summary>
        /// <param name="customCast">The optional object with customised cast parameters.</param>
        /// <param name="point1">The center of the sphere at the start of the capsule.</param>
        /// <param name="point2">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="direction">The direction into which to sweep the capsule.</param>
        /// <param name="maxDistance">The max length of the sweep.</param>
        /// <param name="hitData">The linecast hit data.</param>
        /// <param name="ignoreLayers">A layermask of layers to ignore from the linecast.</param>
        /// <param name="affectTriggers">Determines the trigger interaction level of the cast.</param>
        /// <returns>Returns true if the linecast successfully collides with a valid object.</returns>
        public static bool CapsuleCast(VRTK_CustomRaycast customCast, Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, out RaycastHit hitData, LayerMask ignoreLayers, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)
        {
            if (customCast != null)
            {
                return customCast.CustomCapsuleCast(point1, point2, radius, direction, maxDistance, out hitData);
            }
            else
            {
                return Physics.CapsuleCast(point1, point2, radius, direction, out hitData, maxDistance, ~ignoreLayers, affectTriggers);
            }
        }

        /// <summary>
        /// The CustomRaycast method is used to generate a raycast based on the options defined in the CustomRaycast object.
        /// </summary>
        /// <param name="ray">The Ray to cast with.</param>
        /// <param name="hitData">The raycast hit data.</param>
        /// <param name="length">The maximum length of the raycast.</param>
        /// <returns>Returns true if the raycast successfully collides with a valid object.</returns>
        public virtual bool CustomRaycast(Ray ray, out RaycastHit hitData, float length = Mathf.Infinity)
        {
            return Physics.Raycast(ray, out hitData, length, ~layersToIgnore, triggerInteraction);
        }

        /// <summary>
        /// The CustomLinecast method is used to generate a linecast based on the options defined in the CustomRaycast object.
        /// </summary>
        /// <param name="startPosition">The world position to start the linecast from.</param>
        /// <param name="endPosition">The world position to end the linecast at.</param>
        /// <param name="hitData">The linecast hit data.</param>
        /// <returns>Returns true if the line successfully collides with a valid object.</returns>
        public virtual bool CustomLinecast(Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData)
        {
            return Physics.Linecast(startPosition, endPosition, out hitData, ~layersToIgnore, triggerInteraction);
        }

        /// <summary>
        /// The CustomCapsuleCast method is used to generate a capsulecast based on the options defined in the CustomRaycast object.
        /// </summary>
        /// <param name="point1">The center of the sphere at the start of the capsule.</param>
        /// <param name="point2">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="direction">The direction into which to sweep the capsule.</param>
        /// <param name="maxDistance">The max length of the sweep.</param>
        /// <param name="hitData">The capsulecast hit data.</param>
        /// <returns>Returns true if the capsule successfully collides with a valid object.</returns>
        public virtual bool CustomCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, out RaycastHit hitData)
        {
            return Physics.CapsuleCast(point1, point2, radius, direction, out hitData, maxDistance, ~layersToIgnore, triggerInteraction);
        }
    }
}