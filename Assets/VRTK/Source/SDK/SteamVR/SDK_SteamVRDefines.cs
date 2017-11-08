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
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_2_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion122OrNewer()
        {
            Type controllerManagerClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_ControllerManager");
            if (controllerManagerClass == null)
            {
                return false;
            }

            return controllerManagerClass.GetMethod("SetUniqueObject", BindingFlags.NonPublic | BindingFlags.Instance) != null;
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_1_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion121OrNewer()
        {
            Type eventClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_Events");
            if (eventClass == null)
            {
                return false;
            }

            MethodInfo systemMethod = eventClass.GetMethod("System", BindingFlags.Public | BindingFlags.Static);
            if (systemMethod == null)
            {
                return false;
            }

            ParameterInfo[] systemMethodParameters = systemMethod.GetParameters();
            if (systemMethodParameters.Length != 1)
            {
                return false;
            }

            return systemMethodParameters[0].ParameterType == VRTK_SharedMethods.GetTypeUnknownAssembly("Valve.VR.EVREventType");
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_0", BuildTargetGroupName)]
        private static bool IsPluginVersion120()
        {
            Type eventClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_Events");
            if (eventClass == null)
            {
                return false;
            }

            MethodInfo systemMethod = eventClass.GetMethod("System", BindingFlags.Public | BindingFlags.Static);
            if (systemMethod == null)
            {
                return false;
            }

            ParameterInfo[] systemMethodParameters = systemMethod.GetParameters();
            if (systemMethodParameters.Length != 1)
            {
                return false;
            }

            return systemMethodParameters[0].ParameterType == typeof(string);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_1_1_OR_OLDER", BuildTargetGroupName)]
        private static bool IsPluginVersion111OrOlder()
        {
            Type utilsClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_Utils");
            if (utilsClass == null)
            {
                return false;
            }

            Type eventClass = utilsClass.GetNestedType("Event");
            if (eventClass == null)
            {
                return false;
            }

            return eventClass.GetMethod("Listen", BindingFlags.Public | BindingFlags.Static) != null;
        }
    }
}