// GearVR Defines|SDK_GearVR|001
namespace VRTK
{
    /// <summary>
    /// Handles all the scripting define symbols for the GearVR SDK.
    /// </summary>
    public static class SDK_GearVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the GearVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_GEARVR";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        private static bool IsGearVRAvailable()
        {
            // Native support for Oculus by Unity
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }
    }
}