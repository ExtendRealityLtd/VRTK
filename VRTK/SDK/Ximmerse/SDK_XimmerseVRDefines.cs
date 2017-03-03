// XimmerseVR Defines|SDK_XimmerseVR|001
namespace VRTK
{
    /// <summary>
    /// Handles all the scripting define symbols for the XimmerseVR SDK.
    /// </summary>
    public static class SDK_XimmerseVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the XimmerseVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_XIMMERSEVR";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        private static bool IsXimmerseVRAvailable()
        {
            return typeof(SDK_XimmerseVRDefines).Assembly.GetType("Ximmerse.InputSystem.XDevicePlugin") != null;
        }
    }
}