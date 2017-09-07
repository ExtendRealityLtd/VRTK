namespace VRTK
{
    /// <summary>
    /// Handles all the scripting define symbols for the Windows Immersive Mixed Reality SDK.
    /// </summary>
    public static class SDK_WindowsMRDefines
    {
        /// <summary>
        /// The scripting define symbol for the Immersive Mixed Reality SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_WINDOWSMR";

        private const string BuildTargetGroupName = "WSA";
        
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "WSA")]
        private static bool IsXRSettingsEnabled()
        {
            //TODO : Check somehow for XR Settings
            return true;
        }
    }
}
