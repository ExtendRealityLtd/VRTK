// OculusVR Defines|SDK_OculusVR|001
namespace VRTK
{
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
        private static bool IsOculusVRAvailable()
        {
            return typeof(SDK_OculusVRDefines).Assembly.GetType("OVRInput") != null;
        }

        [SDK_ScriptingDefineSymbolPredicate(AvatarScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsOculusVRAvatarAvailable()
        {
            return IsOculusVRAvailable() && typeof(SDK_OculusVRDefines).Assembly.GetType("OVRAvatar") != null;
        }
    }
}