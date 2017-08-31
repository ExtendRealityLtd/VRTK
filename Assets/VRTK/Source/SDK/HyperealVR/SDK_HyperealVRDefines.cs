// HyperealVR Defines|SDK_HyperealVR|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the Hypereal SDK.
    /// </summary>
    public static class SDK_HyperealVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the Hypereal SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix +
            "SDK_HYPEREALVR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsHyperealVRAvailable()
        {
            return VRTK_SharedMethods.GetTypeUnknownAssembly("Hypereal.HyperealApi") != null;
        }
    }
}
