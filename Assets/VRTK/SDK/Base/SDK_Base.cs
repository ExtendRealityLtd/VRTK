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
    }
}