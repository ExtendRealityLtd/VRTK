// Oculus Defines|SDK_Oculus|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the Oculus and Avatar SDKs.
    /// </summary>
    public static class SDK_OculusDefines
    {
        /// <summary>
        /// The scripting define symbol for the Oculus SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUS";
        /// <summary>
        /// The scripting define symbol for the Oculus Avatar SDK.
        /// </summary>
        public const string AvatarScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUS_AVATAR";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_12_0_OR_NEWER", "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_12_0_OR_NEWER", "Android")]
        private static bool IsUtilitiesVersion1120OrNewer()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion >= new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_11_0_OR_OLDER", "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUS_UTILITIES_1_11_0_OR_OLDER", "Android")]
        private static bool IsUtilitiesVersion1110OrOlder()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion < new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(AvatarScriptingDefineSymbol, "Standalone")]
        [SDK_ScriptingDefineSymbolPredicate(AvatarScriptingDefineSymbol, "Android")]
        private static bool IsAvatarAvailable()
        {
            return (IsUtilitiesVersion1120OrNewer() || IsUtilitiesVersion1110OrOlder())
                   && VRTK_SharedMethods.GetTypeUnknownAssembly("OvrAvatar") != null;
        }

        private static Version GetOculusWrapperVersion()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("OVRPlugin");
            if (pluginClass == null)
            {
                return null;
            }

            FieldInfo versionField = pluginClass.GetField("wrapperVersion", BindingFlags.Public | BindingFlags.Static);
            if (versionField == null)
            {
                return null;
            }

            return (Version)versionField.GetValue(null);
        }
    }
}