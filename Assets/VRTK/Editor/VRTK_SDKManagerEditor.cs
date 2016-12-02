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
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Get actual inspector
            VRTK_SDKManager sdkManager = (VRTK_SDKManager)target;

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("persistOnLoad"));

            sdkManager.systemSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("systemSDK"), sdkManager.systemSDK);
            sdkManager.boundariesSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("boundariesSDK"), sdkManager.boundariesSDK);
            sdkManager.headsetSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("headsetSDK"), sdkManager.headsetSDK);
            sdkManager.controllerSDK = (VRTK_SDKManager.SupportedSDKs)EditorGUILayout.EnumPopup(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("controllerSDK"), sdkManager.controllerSDK);

            CheckSDKUsage(sdkManager.systemSDK, sdkManager.headsetSDK, sdkManager.controllerSDK, sdkManager.boundariesSDK);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualBoundaries"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualHeadset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualRightController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasRightController"));

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void CheckSDKUsage(VRTK_SDKManager.SupportedSDKs system, VRTK_SDKManager.SupportedSDKs headset, VRTK_SDKManager.SupportedSDKs controller, VRTK_SDKManager.SupportedSDKs boundaries)
        {
            var message = "SDK has been selected but is not currently installed.";

            if (system == VRTK_SDKManager.SupportedSDKs.SteamVR || headset == VRTK_SDKManager.SupportedSDKs.SteamVR || controller == VRTK_SDKManager.SupportedSDKs.SteamVR || boundaries == VRTK_SDKManager.SupportedSDKs.SteamVR)
            {
                if (CheckSDKInstalled("SteamVR " + message, "SteamVR"))
                {
                    AddScriptingDefineSymbol("VRTK_SDK_STEAMVR");
                }
                else
                {
                    RemoveScriptingDefineSymbol("VRTK_SDK_STEAMVR");
                }
            }
        }

        private bool CheckSDKInstalled(string message, string checkType)
        {
            if (!TypeExists(checkType))
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                return false;
            }
            return true;
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