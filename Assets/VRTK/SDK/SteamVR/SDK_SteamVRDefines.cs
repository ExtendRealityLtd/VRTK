// SteamVR Defines|SDK_SteamVR|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the SteamVR SDK.
    /// </summary>
    public static class SDK_SteamVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the SteamVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_STEAMVR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_0_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion120OrNewer()
        {
            Type eventClass = typeof(SDK_SteamVRDefines).Assembly.GetType("SteamVR_Events");
            return eventClass != null && eventClass.GetMethod("System", BindingFlags.Public | BindingFlags.Static) != null;
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_1_1_OR_OLDER", BuildTargetGroupName)]
        private static bool IsPluginVersion111OrOlder()
        {
            Type utilsClass = typeof(SDK_SteamVRDefines).Assembly.GetType("SteamVR_Utils");
            if (utilsClass == null)
            {
                return false;
            }

            Type eventClass = utilsClass.GetNestedType("Event");
            return eventClass != null && eventClass.GetMethod("Listen", BindingFlags.Public | BindingFlags.Static) != null;
        }
    }
}