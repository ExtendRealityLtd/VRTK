namespace VRTK
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRSettings = UnityEngine.VR.VRSettings;
#endif
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_SDKSetup))]
    public class VRTK_SDKSetupEditor : Editor
    {
        private const string SDKNotInstalledDescription = " (not installed)";
        private const string SDKNotFoundAnymoreDescription = " (not found)";
        private bool? isDetailedSDKSelectionFoldOut;

        public override void OnInspectorGUI()
        {
            VRTK_SDKSetup setup = (VRTK_SDKSetup)target;

            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("SDK Selection", false);

                Func<VRTK_SDKInfo, ReadOnlyCollection<VRTK_SDKInfo>, string> sdkNameSelector = (info, installedInfos)
                    => info.description.prettyName + (installedInfos.Contains(info) ? "" : SDKNotInstalledDescription);
                string[] availableSystemSDKNames = VRTK_SDKManager.AvailableSystemSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.InstalledSystemSDKInfos)).ToArray();
                string[] availableBoundariesSDKNames = VRTK_SDKManager.AvailableBoundariesSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.InstalledBoundariesSDKInfos)).ToArray();
                string[] availableHeadsetSDKNames = VRTK_SDKManager.AvailableHeadsetSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.InstalledHeadsetSDKInfos)).ToArray();
                string[] availableControllerSDKNames = VRTK_SDKManager.AvailableControllerSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.InstalledControllerSDKInfos)).ToArray();
                string[] allAvailableSDKNames = availableSystemSDKNames
                    .Concat(availableBoundariesSDKNames)
                    .Concat(availableHeadsetSDKNames)
                    .Concat(availableControllerSDKNames)
                    .Distinct()
                    .ToArray();

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();

                    List<GUIContent> quickSelectOptions = allAvailableSDKNames
                        .Select(sdkName => new GUIContent(sdkName))
                        .ToList();
                    int quicklySelectedSDKIndex = 0;

                    if (AreAllSDKsTheSame())
                    {
                        quicklySelectedSDKIndex = allAvailableSDKNames
                            .ToList()
                            .FindIndex(availableSDKName => availableSDKName.Replace(SDKNotInstalledDescription, "")
                                                           == setup.systemSDKInfo.description.prettyName);
                    }
                    else
                    {
                        quickSelectOptions.Insert(0, new GUIContent("Mixed..."));
                    }

                    quicklySelectedSDKIndex = EditorGUILayout.Popup(
                        new GUIContent("Quick Select", "Quickly select one of the SDKs into all slots."),
                        quicklySelectedSDKIndex,
                        quickSelectOptions.ToArray());
                    if (!AreAllSDKsTheSame())
                    {
                        quicklySelectedSDKIndex--;
                    }

                    if (EditorGUI.EndChangeCheck() && (AreAllSDKsTheSame() || quicklySelectedSDKIndex != -1))
                    {
                        string quicklySelectedSDKName = allAvailableSDKNames[quicklySelectedSDKIndex].Replace(SDKNotInstalledDescription, "");

                        Func<VRTK_SDKInfo, bool> predicate = info => info.description.prettyName == quicklySelectedSDKName;
                        VRTK_SDKInfo newSystemSDKInfo = VRTK_SDKManager.AvailableSystemSDKInfos.FirstOrDefault(predicate);
                        VRTK_SDKInfo newBoundariesSDKInfo = VRTK_SDKManager.AvailableBoundariesSDKInfos.FirstOrDefault(predicate);
                        VRTK_SDKInfo newHeadsetSDKInfo = VRTK_SDKManager.AvailableHeadsetSDKInfos.FirstOrDefault(predicate);
                        VRTK_SDKInfo newControllerSDKInfo = VRTK_SDKManager.AvailableControllerSDKInfos.FirstOrDefault(predicate);

                        Undo.RecordObject(setup, "SDK Change (Quick Select)");
                        if (newSystemSDKInfo != null)
                        {
                            setup.systemSDKInfo = newSystemSDKInfo;
                        }
                        if (newBoundariesSDKInfo != null)
                        {
                            setup.boundariesSDKInfo = newBoundariesSDKInfo;
                        }
                        if (newHeadsetSDKInfo != null)
                        {
                            setup.headsetSDKInfo = newHeadsetSDKInfo;
                        }
                        if (newControllerSDKInfo != null)
                        {
                            setup.controllerSDKInfo = newControllerSDKInfo;
                        }

                        UpdateDetailedSDKSelectionFoldOut();
                    }

                    if (AreAllSDKsTheSame())
                    {
                        VRTK_SDKInfo selectedInfo = new[]
                        {
                            setup.systemSDKInfo,
                            setup.boundariesSDKInfo,
                            setup.headsetSDKInfo,
                            setup.controllerSDKInfo,
                        }.First(info => info != null);
                        DrawVRDeviceNameLabel(selectedInfo, "System, Boundaries, Headset and Controller", 0);
                    }
                }

                EditorGUI.indentLevel++;

                if (!isDetailedSDKSelectionFoldOut.HasValue)
                {
                    UpdateDetailedSDKSelectionFoldOut();
                }
                isDetailedSDKSelectionFoldOut = EditorGUI.Foldout(
                    EditorGUILayout.GetControlRect(),
                    isDetailedSDKSelectionFoldOut.Value,
                    "Detailed Selection",
                    true
                );
                if (isDetailedSDKSelectionFoldOut.Value)
                {
                    EditorGUI.BeginChangeCheck();

                    DrawAndHandleSDKSelection<SDK_BaseSystem>("The SDK to use to deal with all system actions.", 1);
                    DrawAndHandleSDKSelection<SDK_BaseBoundaries>("The SDK to use to utilize room scale boundaries.", 1);
                    DrawAndHandleSDKSelection<SDK_BaseHeadset>("The SDK to use to utilize the VR headset.", 1);
                    DrawAndHandleSDKSelection<SDK_BaseController>("The SDK to use to utilize the input devices.", 1);

                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateDetailedSDKSelectionFoldOut();
                    }
                }

                EditorGUI.indentLevel--;

                string errorDescriptions = string.Join("\n", setup.GetSimplifiedErrorDescriptions());
                if (!string.IsNullOrEmpty(errorDescriptions))
                {
                    EditorGUILayout.HelpBox(errorDescriptions, MessageType.Error);
                }

                if (allAvailableSDKNames.Length != availableSystemSDKNames.Length
                    || allAvailableSDKNames.Length != availableBoundariesSDKNames.Length
                    || allAvailableSDKNames.Length != availableHeadsetSDKNames.Length
                    || allAvailableSDKNames.Length != availableControllerSDKNames.Length)
                {
                    EditorGUILayout.HelpBox("Only endpoints that are supported by the selected SDK are changed by Quick Select, the others are left untouched."
                                            + "\n\nSome of the available SDK implementations are only available for a subset of SDK endpoints. Quick Select"
                                            + " shows SDKs that provide an implementation for *any* of the different SDK endpoints in VRTK"
                                            + " (System, Boundaries, Headset, Controller).", MessageType.Info);
                }
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("Object References", false);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    bool autoPopulate = EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("autoPopulateObjectReferences", "Auto Populate"),
                                                               setup.autoPopulateObjectReferences,
                                                               GUILayout.ExpandWidth(false));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.FindProperty("autoPopulateObjectReferences").boolValue = autoPopulate;
                        serializedObject.ApplyModifiedProperties();
                        setup.PopulateObjectReferences(false);
                    }

                    const string populateNowDescription = "Populate Now";
                    GUIContent populateNowGUIContent = new GUIContent(populateNowDescription, "Set the SDK object references to the objects of the selected SDKs.");
                    if (GUILayout.Button(populateNowGUIContent, GUILayout.MaxHeight(GUI.skin.label.CalcSize(populateNowGUIContent).y)))
                    {
                        Undo.RecordObject(setup, populateNowDescription);
                        setup.PopulateObjectReferences(true);
                    }
                }

                if (setup.autoPopulateObjectReferences)
                {
                    EditorGUILayout.HelpBox("The SDK Setup is configured to automatically populate object references so the following fields are disabled."
                                            + " Uncheck `Auto Populate` above to enable customization of the fields below.", MessageType.Info);
                }

                using (new EditorGUI.DisabledGroupScope(setup.autoPopulateObjectReferences))
                {
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        VRTK_EditorUtilities.AddHeader("Actual Objects", false);
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("actualBoundaries"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("actualBoundaries", "Boundaries")
                        );
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("actualHeadset"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("actualHeadset", "Headset")
                        );
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("actualLeftController"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("actualLeftController", "Left Controller")
                        );
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("actualRightController"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("actualRightController", "Right Controller")
                        );
                    }

                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        VRTK_EditorUtilities.AddHeader("Model Aliases", false);
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("modelAliasLeftController"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("modelAliasLeftController", "Left Controller")
                        );
                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("modelAliasRightController"),
                            VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKSetup>("modelAliasRightController", "Right Controller")
                        );
                    }
                }

                EditorGUILayout.HelpBox(
                    "The game object this SDK Setup is attached to will be set inactive automatically to allow for SDK loading and switching.",
                    MessageType.Info
                );

                IEnumerable<GameObject> referencedObjects = new[]
                {
                    setup.actualBoundaries,
                    setup.actualHeadset,
                    setup.actualLeftController,
                    setup.actualRightController,
                    setup.modelAliasLeftController,
                    setup.modelAliasRightController
                }.Where(referencedObject => referencedObject != null);
                if (referencedObjects.Any(referencedObject => !referencedObject.transform.IsChildOf(setup.transform)))
                {
                    EditorGUILayout.HelpBox(
                        "There is at least one referenced object that is neither a child of, deep child (child of a child) of nor attached to the game object this SDK Setup is attached to.",
                        MessageType.Error
                    );
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Handle undo

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            UpdateDetailedSDKSelectionFoldOut();
        }

        #endregion

        /// <summary>
        /// Draws a popup menu and handles the selection for an SDK info.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="description">The description of the SDK base.</param>
        private void DrawAndHandleSDKSelection<BaseType>(string description, int indentLevelToRemove) where BaseType : SDK_Base
        {
            Type baseType = typeof(BaseType);
            Type sdkManagerType = typeof(VRTK_SDKManager);
            string baseName = baseType.Name.Remove(0, typeof(SDK_Base).Name.Length);

            ReadOnlyCollection<VRTK_SDKInfo> availableSDKInfos = (ReadOnlyCollection<VRTK_SDKInfo>)sdkManagerType
                .GetProperty(string.Format("Available{0}SDKInfos", baseName), BindingFlags.Public | BindingFlags.Static)
                .GetGetMethod()
                .Invoke(null, null);
            ReadOnlyCollection<VRTK_SDKInfo> installedSDKInfos = (ReadOnlyCollection<VRTK_SDKInfo>)sdkManagerType
                .GetProperty(string.Format("Installed{0}SDKInfos", baseName), BindingFlags.Public | BindingFlags.Static)
                .GetGetMethod()
                .Invoke(null, null);

            PropertyInfo sdkInfoPropertyInfo = typeof(VRTK_SDKSetup).GetProperty(string.Format("{0}SDKInfo", baseName.ToLowerInvariant()));
            VRTK_SDKSetup sdkSetup = (VRTK_SDKSetup)target;
            VRTK_SDKInfo selectedSDKInfo = (VRTK_SDKInfo)sdkInfoPropertyInfo.GetGetMethod().Invoke(sdkSetup, null);

            List<string> availableSDKNames = availableSDKInfos.Select(info => info.description.prettyName + (installedSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToList();
            int selectedSDKIndex = availableSDKInfos.IndexOf(selectedSDKInfo);
            if (selectedSDKInfo.originalTypeNameWhenFallbackIsUsed != null)
            {
                availableSDKNames.Add(selectedSDKInfo.originalTypeNameWhenFallbackIsUsed + SDKNotFoundAnymoreDescription);
                selectedSDKIndex = availableSDKNames.Count - 1;
            }

            GUIContent[] availableSDKGUIContents = availableSDKNames.Select(availableSDKName => new GUIContent(availableSDKName)).ToArray();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                int newSelectedSDKIndex = EditorGUILayout.Popup(
                    new GUIContent(baseName, description),
                    selectedSDKIndex,
                    availableSDKGUIContents
                );
                VRTK_SDKInfo newSelectedSDKInfo = selectedSDKInfo.originalTypeNameWhenFallbackIsUsed != null
                                                  && newSelectedSDKIndex == availableSDKNames.Count - 1
                                                      ? selectedSDKInfo
                                                      : availableSDKInfos[newSelectedSDKIndex];
                if (EditorGUI.EndChangeCheck() && newSelectedSDKInfo != selectedSDKInfo)
                {
                    Undo.RecordObject(sdkSetup, string.Format("{0} SDK Change", baseName));
                    sdkInfoPropertyInfo.GetSetMethod().Invoke(sdkSetup, new object[] { newSelectedSDKInfo });
                }

                DrawVRDeviceNameLabel(selectedSDKInfo, baseName, indentLevelToRemove);
            }
        }

        private static void DrawVRDeviceNameLabel(VRTK_SDKInfo info, string sdkBaseName, int indentLevelToRemove)
        {
            float maxVRDeviceNameTextWidth = new[]
                {
                    VRTK_SDKManager.AvailableSystemSDKInfos,
                    VRTK_SDKManager.AvailableBoundariesSDKInfos,
                    VRTK_SDKManager.AvailableHeadsetSDKInfos,
                    VRTK_SDKManager.AvailableControllerSDKInfos
                }
                .SelectMany(infos => infos.Select(sdkInfo => sdkInfo.description.vrDeviceName))
                .Concat(XRSettings.supportedDevices)
                .Concat(new[] { "None" })
                .Distinct()
                .Select(deviceName => GUI.skin.label.CalcSize(new GUIContent(deviceName)).x)
                .Max();

            EditorGUI.indentLevel -= indentLevelToRemove;
            EditorGUILayout.LabelField(
                new GUIContent(info.description.vrDeviceName,
                               string.Format("The VR Device used by the {0} {1} SDK.", info.description.prettyName, sdkBaseName)),
                new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.MiddleRight
                },
                GUILayout.Width(maxVRDeviceNameTextWidth)
            );
            EditorGUI.indentLevel += indentLevelToRemove;
        }

        private bool AreAllSDKsTheSame()
        {
            VRTK_SDKSetup setup = (VRTK_SDKSetup)target;
            VRTK_SDKInfo[] infos =
            {
                setup.systemSDKInfo,
                setup.boundariesSDKInfo,
                setup.headsetSDKInfo,
                setup.controllerSDKInfo,
            };

            if (infos.Any(info => info.originalTypeNameWhenFallbackIsUsed != null))
            {
                return false;
            }

            return infos.Select(info => info.description.prettyName)
                        .Distinct()
                        .Count() == 1;
        }

        private void UpdateDetailedSDKSelectionFoldOut()
        {
            isDetailedSDKSelectionFoldOut = !AreAllSDKsTheSame();
        }

        private sealed class SDKSetupGameObjectsDisabler : AssetModificationProcessor
        {
            /*
             * Used to make sure the warning is actually seen in case the console is automatically cleared
             * because of 'Clear on Play' when playing the scene in the Editor.
             */
            private const string PreferencesKey = "VRTK.SDKSetupGameObjectsDisabler.InfoMessage";

            [InitializeOnLoadMethod]
            private static void ListenToPlayModeChanges()
            {
#if UNITY_2017_2_OR_NEWER
                EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
#else
                EditorApplication.playmodeStateChanged += () =>
#endif
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                    {
                        FixOpenAndUnsavedScenes();
                    }

                    if (EditorApplication.isPlaying
#if UNITY_5_6_OR_NEWER
                        && SessionState.GetBool(PreferencesKey, false)
#else
                        && EditorPrefs.HasKey(PreferencesKey)
#endif
                    )
                    {
#if UNITY_5_6_OR_NEWER
                        SessionState.EraseBool(
#else
                        EditorPrefs.DeleteKey(
#endif
                            PreferencesKey
                        );
                    }
                };
            }

            private static string[] OnWillSaveAssets(string[] paths)
            {
                FixOpenAndUnsavedScenes();
                return paths;
            }

            private static void FixOpenAndUnsavedScenes()
            {

                List<VRTK_SDKSetup> setups = Enumerable.Range(0, EditorSceneManager.loadedSceneCount)
                                                       .SelectMany(sceneIndex => SceneManager.GetSceneAt(sceneIndex).GetRootGameObjects())
                                                       .SelectMany(rootObject => rootObject.GetComponentsInChildren<VRTK_SDKManager>())
                                                       .Select(manager => manager.setups.Where(setup => setup != null).ToArray())
                                                       .Where(sdkSetups => sdkSetups.Length > 1)
                                                       .SelectMany(sdkSetups => sdkSetups)
                                                       .Where(setup => setup.gameObject.activeSelf)
                                                       .ToList();
                if (setups.Count == 0)
                {
                    return;
                }

                setups.ForEach(setup => setup.gameObject.SetActive(false));

                string infoMessage = string.Format(
                    "The following game objects have been set inactive to allow for SDK loading and switching using the SDK Setups on them:\n{0}",
                    string.Join(", ", setups.Select(setup => setup.name).ToArray()));
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
#if UNITY_5_6_OR_NEWER
                    SessionState.SetString(
#else
                    EditorPrefs.SetString(
#endif
                        PreferencesKey,
                        infoMessage
                    );
                }
                else
                {
                    VRTK_Logger.Info(infoMessage);
                }
            }
        }
    }
}