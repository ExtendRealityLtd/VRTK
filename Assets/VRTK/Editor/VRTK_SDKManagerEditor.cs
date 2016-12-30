﻿namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [CustomEditor(typeof(VRTK_SDKManager))]
    public class VRTK_SDKManagerEditor : Editor
    {
        private SDK_BaseHeadset previousHeadsetSDK;
        private SDK_BaseController previousControllerSDK;
        private SDK_BaseBoundaries previousBoundariesSDK;

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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelAliasLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelAliasRightController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasRightController"));

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space();
            if (GUILayout.Button("Auto Populate Linked Objects"))
            {
                AutoPopulate(sdkManager);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private SDK_BaseHeadset GetHeadsetSDK(VRTK_SDKManager sdkManager)
        {
            return sdkManager.GetHeadsetSDK();
        }

        private SDK_BaseController GetControllerSDK(VRTK_SDKManager sdkManager)
        {
            return sdkManager.GetControllerSDK();
        }

        private SDK_BaseBoundaries GetBoundariesSDK(VRTK_SDKManager sdkManager)
        {
            return sdkManager.GetBoundariesSDK();
        }

        private void AutoPopulate(VRTK_SDKManager sdkManager)
        {
            var boundariesSDK = GetBoundariesSDK(sdkManager);
            var headsetSDK = GetHeadsetSDK(sdkManager);
            var controllerSDK = GetControllerSDK(sdkManager);

            var forceSaveScene = false;

            if (boundariesSDK && (!sdkManager.actualBoundaries || !previousBoundariesSDK || boundariesSDK.GetType() != previousBoundariesSDK.GetType()))
            {
                var playareaTransform = boundariesSDK.GetPlayArea();
                sdkManager.actualBoundaries = (playareaTransform ? playareaTransform.gameObject : null);
                previousBoundariesSDK = boundariesSDK;
                forceSaveScene = true;
            }

            if (headsetSDK && (!sdkManager.actualHeadset || !previousHeadsetSDK || headsetSDK.GetType() != previousHeadsetSDK.GetType()))
            {
                var headsetTransform = headsetSDK.GetHeadset();
                sdkManager.actualHeadset = (headsetTransform ? headsetTransform.gameObject : null);
                previousHeadsetSDK = headsetSDK;
                forceSaveScene = true;
            }

            var setPreviousControllerSDK = false;

            if (controllerSDK && (!sdkManager.actualLeftController || !previousControllerSDK || controllerSDK.GetType() != previousControllerSDK.GetType()))
            {
                var controllerLeft = controllerSDK.GetControllerLeftHand(true);
                sdkManager.actualLeftController = (controllerLeft ? controllerLeft : null);
                setPreviousControllerSDK = true;
            }

            if (controllerSDK && (!sdkManager.actualRightController || !previousControllerSDK || controllerSDK.GetType() != previousControllerSDK.GetType()))
            {
                var controllerRight = controllerSDK.GetControllerRightHand(true);
                sdkManager.actualRightController = (controllerRight ? controllerRight : null);
                setPreviousControllerSDK = true;
            }

            if (controllerSDK && (!sdkManager.modelAliasLeftController || !previousControllerSDK || controllerSDK.GetType() != previousControllerSDK.GetType()))
            {
                var controllerLeft = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Left);
                sdkManager.modelAliasLeftController = (controllerLeft ? controllerLeft : null);
                setPreviousControllerSDK = true;
            }

            if (controllerSDK && (!sdkManager.modelAliasRightController || !previousControllerSDK || controllerSDK.GetType() != previousControllerSDK.GetType()))
            {
                var controllerRight = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Right);
                sdkManager.modelAliasRightController = (controllerRight ? controllerRight : null);
                setPreviousControllerSDK = true;
            }

            if (setPreviousControllerSDK)
            {
                previousControllerSDK = controllerSDK;
                forceSaveScene = true;
            }

            if (forceSaveScene)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        private void CheckSDKUsage(VRTK_SDKManager.SupportedSDKs system, VRTK_SDKManager.SupportedSDKs headset, VRTK_SDKManager.SupportedSDKs controller, VRTK_SDKManager.SupportedSDKs boundaries)
        {
            ProcessSDK(VRTK_SDKManager.SupportedSDKs.SteamVR, "VRTK_SDK_STEAMVR", "SteamVR", "SteamVR", system, headset, controller, boundaries);
            ProcessSDK(VRTK_SDKManager.SupportedSDKs.OculusVR, "VRTK_SDK_OCULUSVR", "OculusVR", "OVRInput", system, headset, controller, boundaries);
            ProcessSDK(VRTK_SDKManager.SupportedSDKs.Simulator, "VRTK_SDK_SIM", "Simulator", "VRTK_SDKManager", system, headset, controller, boundaries);
        }

        private void ProcessSDK(VRTK_SDKManager.SupportedSDKs sdk, string defineSymbol, string name, string checkType, VRTK_SDKManager.SupportedSDKs system, VRTK_SDKManager.SupportedSDKs headset, VRTK_SDKManager.SupportedSDKs controller, VRTK_SDKManager.SupportedSDKs boundaries)
        {
            var message = "SDK has been selected but is not currently installed.";

            if (!CheckSDKInstalled(name + message, checkType, false) || (system != sdk && headset != sdk && controller != sdk && boundaries != sdk))
            {
                RemoveScriptingDefineSymbol(defineSymbol);
            }

            if (system == sdk || headset == sdk || controller == sdk || boundaries == sdk)
            {
                if (CheckSDKInstalled(name + message, checkType, true))
                {
                    AddScriptingDefineSymbol(defineSymbol);
                }
            }

            CheckAvatarSupport(sdk);
        }

        private void CheckAvatarSupport(VRTK_SDKManager.SupportedSDKs sdk)
        {
            switch (sdk)
            {
                case VRTK_SDKManager.SupportedSDKs.OculusVR:
                    var defineSymbol = "VRTK_SDK_OCULUSVR_AVATAR";
                    if (TypeExists("OvrAvatar"))
                    {
                        AddScriptingDefineSymbol(defineSymbol);
                    }
                    else
                    {
                        RemoveScriptingDefineSymbol(defineSymbol);
                    }
                    break;
            }
        }

        private bool CheckSDKInstalled(string message, string checkType, bool showMessage)
        {
            if (!TypeExists(checkType))
            {
                if (showMessage)
                {
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                }
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