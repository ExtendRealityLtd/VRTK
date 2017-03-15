// OculusVR Defines|SDK_OculusVR|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the OculusVR and Avatar SDKs.
    /// </summary>
    public static class SDK_OculusVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the OculusVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUSVR";
        /// <summary>
        /// The scripting define symbol for the OculusVR Avatar SDK.
        /// </summary>
        public const string AvatarScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUSVR_AVATAR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUSVR_UTILITIES_1_12_0_OR_NEWER", BuildTargetGroupName)]
        private static bool IsUtilitiesVersion1120OrNewer()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion >= new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "OCULUSVR_UTILITIES_1_11_0_OR_OLDER", BuildTargetGroupName)]
        private static bool IsUtilitiesVersion1110OrOlder()
        {
            Version wrapperVersion = GetOculusWrapperVersion();
            return wrapperVersion != null && wrapperVersion < new Version(1, 12, 0);
        }

        [SDK_ScriptingDefineSymbolPredicate(AvatarScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsAvatarAvailable()
        {
            return (IsUtilitiesVersion1120OrNewer() || IsUtilitiesVersion1110OrOlder())
                   && typeof(SDK_OculusVRDefines).Assembly.GetType("OvrAvatar") != null;
        }

        private static Version GetOculusWrapperVersion()
        {
            Type pluginClass = typeof(SDK_OculusVRDefines).Assembly.GetType("OVRPlugin");
            if (pluginClass == null)
            {
                return null;
            }

            FieldInfo versionField = pluginClass.GetField("wrapperVersion", BindingFlags.Public | BindingFlags.Static);
            if (versionField == null)
            {
                return null;
            }

            var version = (Version)versionField.GetValue(null);
            return version;
        }

        private static Version GetOculusRuntimeVersion()
        {
            Type pluginClass = typeof(SDK_OculusVRDefines).Assembly.GetType("OVRPlugin");
            if (pluginClass == null)
            {
                return null;
            }

            PropertyInfo versionProperty = pluginClass.GetProperty("version", BindingFlags.Public | BindingFlags.Static);
            if (versionProperty == null)
            {
                return null;
            }

            var version = (Version)versionProperty.GetGetMethod().Invoke(null, null);
            return version;
        }
    }
}