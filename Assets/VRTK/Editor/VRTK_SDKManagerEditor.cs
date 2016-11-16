namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [CustomEditor(typeof(VRTK_SDKManager))]
    public class VRTK_SDKManagerEditor : Editor
    {
        private VRTK_SDKManager.SupportedSDKs currentSystemSDK = VRTK_SDKManager.SupportedSDKs.None;
        private VRTK_SDKManager.SupportedSDKs currentHeadsetSDK = VRTK_SDKManager.SupportedSDKs.None;
        private VRTK_SDKManager.SupportedSDKs currentControllerSDK = VRTK_SDKManager.SupportedSDKs.None;
        private VRTK_SDKManager.SupportedSDKs currentBoundariesSDK = VRTK_SDKManager.SupportedSDKs.None;

        private Dictionary<VRTK_SDKManager.SupportedSDKs, string> systemSDKDefines = new Dictionary<VRTK_SDKManager.SupportedSDKs, string>
        {
            {VRTK_SDKManager.SupportedSDKs.None, ""},
            {VRTK_SDKManager.SupportedSDKs.SteamVR, "VRTK_SDK_SYSTEM_STEAMVR"}
        };

        private Dictionary<VRTK_SDKManager.SupportedSDKs, string> headsetSDKDefines = new Dictionary<VRTK_SDKManager.SupportedSDKs, string>
        {
            {VRTK_SDKManager.SupportedSDKs.None, ""},
            {VRTK_SDKManager.SupportedSDKs.SteamVR, "VRTK_SDK_HEADSET_STEAMVR"}
        };

        private Dictionary<VRTK_SDKManager.SupportedSDKs, string> controllerSDKDefines = new Dictionary<VRTK_SDKManager.SupportedSDKs, string>
        {
            {VRTK_SDKManager.SupportedSDKs.None, ""},
            {VRTK_SDKManager.SupportedSDKs.SteamVR, "VRTK_SDK_CONTROLLER_STEAMVR"}
        };

        private Dictionary<VRTK_SDKManager.SupportedSDKs, string> boundariesSDKDefines = new Dictionary<VRTK_SDKManager.SupportedSDKs, string>
        {
            {VRTK_SDKManager.SupportedSDKs.None, ""},
            {VRTK_SDKManager.SupportedSDKs.SteamVR, "VRTK_SDK_BOUNDARIES_STEAMVR"}
        };

        public override void OnInspectorGUI()
        {
            //Get actual inspector
            VRTK_SDKManager sdkManager = (VRTK_SDKManager)target;

            //Deal with selected System SDK
            sdkManager.systemSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("systemSDK"), sdkManager.systemSDK);
            if (currentSystemSDK != sdkManager.systemSDK)
            {
                RemoveScriptingDefineSymbol(systemSDKDefines[currentSystemSDK]);
                currentSystemSDK = sdkManager.systemSDK;
                AddScriptingDefineSymbol(systemSDKDefines[currentSystemSDK]);
            }

            //Deal with selected Headset SDK
            sdkManager.headsetSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("headsetSDK"), sdkManager.headsetSDK);
            if (currentHeadsetSDK != sdkManager.headsetSDK)
            {
                RemoveScriptingDefineSymbol(headsetSDKDefines[currentHeadsetSDK]);
                currentHeadsetSDK = sdkManager.headsetSDK;
                AddScriptingDefineSymbol(headsetSDKDefines[currentHeadsetSDK]);
            }

            //Deal with selected Controller SDK
            sdkManager.controllerSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("controllerSDK"), sdkManager.controllerSDK);
            if (currentControllerSDK != sdkManager.controllerSDK)
            {
                RemoveScriptingDefineSymbol(controllerSDKDefines[currentControllerSDK]);
                currentControllerSDK = sdkManager.controllerSDK;
                AddScriptingDefineSymbol(controllerSDKDefines[currentControllerSDK]);
            }

            //Deal with selected Boundaries SDK
            sdkManager.boundariesSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("boundariesSDK"), sdkManager.boundariesSDK);
            if (currentBoundariesSDK != sdkManager.boundariesSDK)
            {
                RemoveScriptingDefineSymbol(boundariesSDKDefines[currentBoundariesSDK]);
                currentBoundariesSDK = sdkManager.boundariesSDK;
                AddScriptingDefineSymbol(boundariesSDKDefines[currentBoundariesSDK]);
            }

            CheckSDKUsage(sdkManager.systemSDK, sdkManager.headsetSDK, sdkManager.controllerSDK, sdkManager.boundariesSDK);
        }

        private void CheckSDKUsage(VRTK_SDKManager.SupportedSDKs system, VRTK_SDKManager.SupportedSDKs headset, VRTK_SDKManager.SupportedSDKs controller, VRTK_SDKManager.SupportedSDKs boundaries)
        {
            var message = "SDK has been selected but is not currently installed.";
            if(system == VRTK_SDKManager.SupportedSDKs.SteamVR || headset == VRTK_SDKManager.SupportedSDKs.SteamVR || controller == VRTK_SDKManager.SupportedSDKs.SteamVR || boundaries == VRTK_SDKManager.SupportedSDKs.SteamVR)
            {
                CheckSDKInstalled("SteamVR " + message, "SteamVR");
            }
        }

        private void CheckSDKInstalled(string message, string checkType)
        {
            if(!TypeExists(checkType))
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
        }

        private void AddScriptingDefineSymbol(string define)
        {
            if (define == "")
            {
                return;
            }
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            List<string> definesList = new List<string>(scriptingDefineSymbols.Split(';'));
            if (!definesList.Contains(define))
            {
                definesList.Add(define);
                Debug.Log("Scripting Define Symbol Added To [Project Settings->Player]: " + define);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", definesList.ToArray()));
        }

        private void RemoveScriptingDefineSymbol(string define)
        {
            if (define == "")
            {
                return;
            }
            string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            List<string> definesList = new List<string>(scriptingDefineSymbols.Split(';'));
            if (definesList.Contains(define))
            {
                definesList.Remove(define);
                Debug.Log("Scripting Define Symbol Removed from [Project Settings->Player]: " + define);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", definesList.ToArray()));
        }

        private static bool TypeExists(string className)
        {
            var foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.Name == className
                             select type).FirstOrDefault();

            return foundType != null;
        }
    }
}