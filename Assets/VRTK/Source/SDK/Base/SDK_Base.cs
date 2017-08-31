// SDK Base|SDK_Base|001
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Abstract superclass that defines that a particular class is an SDK.
    /// </summary>
    /// <remarks>
    /// This is an abstract class to mark all different SDK endpoints with. This is used to allow for type safety when talking about 'an SDK' instead of one of the different endpoints (System, Boundaries, Headset, Controller).
    /// </remarks>
    public abstract class SDK_Base : ScriptableObject
    {
        /// <summary>
        /// This method is called just before loading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public virtual void OnBeforeSetupLoad(VRTK_SDKSetup setup)
        {

        }

        /// <summary>
        /// This method is called just after loading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public virtual void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {

        }

        /// <summary>
        /// This method is called just before unloading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public virtual void OnBeforeSetupUnload(VRTK_SDKSetup setup)
        {

        }

        /// <summary>
        /// This method is called just after unloading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public virtual void OnAfterSetupUnload(VRTK_SDKSetup setup)
        {

        }
    }
}