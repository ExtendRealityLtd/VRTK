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
            Version version = GetVersion();
            return version != null && version >= new Version(1, 2, 2);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_1_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion121OrNewer()
        {
            Version version = GetVersion();
            return version != null && version >= new Version(1, 2, 1);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_0", BuildTargetGroupName)]
        private static bool IsPluginVersion120()
        {
            Version version = GetVersion();
            return version != null && version == new Version(1, 2, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_1_1_OR_OLDER", BuildTargetGroupName)]
        private static bool IsPluginVersion111OrOlder()
        {
            Version version = GetVersion();
            return version != null && version <= new Version(1, 1, 1);
        }

        private static Version GetVersion()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_Update");
            if (pluginClass == null)
            {
                return null;
            }

            FieldInfo versionField = pluginClass.GetField("currentVersion", BindingFlags.NonPublic | BindingFlags.Static);
            if (versionField == null)
            {
                return null;
            }

            string versionString = (string)versionField.GetValue(null);
            if (versionString == null)
            {
                return null;
            }

            return new Version(versionString);
        }
    }
}