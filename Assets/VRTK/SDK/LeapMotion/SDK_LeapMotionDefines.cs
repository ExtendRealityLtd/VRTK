// OculusVR Defines|SDK_LeapMotion|001
namespace VRTK
{
    /// <summary>
    /// Handles all the scripting define symbols for the LeapMotion
    /// </summary>
    public static class SDK_LeapMotionDefines
    {
        /// <summary>
        /// The scripting define symbol for the OculusVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_LEAPMOTION";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsLeapMotionAvailable()
        {
            return typeof(SDK_LeapMotionDefines).Assembly.GetType("Leap.Unity.LeapDeviceInfo") != null;
        }
    }
}