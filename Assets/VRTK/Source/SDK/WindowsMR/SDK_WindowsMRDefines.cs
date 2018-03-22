// WindowsMR Defines|SDK_WindowsMR|001
namespace VRTK
{
    using System;

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

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "WINDOWSMR_CONTROLLER_VISUALIZATION", BuildTargetGroupName)]
        private static bool HasControllerVisualization()
        {
            Type controllerVisualizerClass = VRTK_SharedMethods.GetTypeUnknownAssembly("VRTK.WindowsMixedReality.MotionControllerVisualizer");

            return controllerVisualizerClass != null;
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        private static bool IsXRSettingsEnabled()
        {
            //TODO : Check somehow for XR Settings
            return true;
        }
    }
}
