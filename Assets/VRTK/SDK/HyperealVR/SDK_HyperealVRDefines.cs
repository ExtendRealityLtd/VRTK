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
        public const string ScriptDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix +
            "SDK_HYPEREALVR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix +
            "HYPEREALVR_PLUGIN_1_0_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion10OrNewer()
        {
            return true;
        }
    }
}
