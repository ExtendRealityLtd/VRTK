namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_SDKManager))]
    public class VRTK_SDKManagerEditor : Editor
    {
        private static readonly Dictionary<BuildTargetGroup, bool> isBuildTargetActiveSymbolsFoldOut;
        private ReorderableList setupsList;

        static VRTK_SDKManagerEditor()
        {
            BuildTargetGroup[] targetGroups = VRTK_SharedMethods.GetValidBuildTargetGroups();
            isBuildTargetActiveSymbolsFoldOut = new Dictionary<BuildTargetGroup, bool>(targetGroups.Length);

            foreach (BuildTargetGroup targetGroup in targetGroups)
            {
                isBuildTargetActiveSymbolsFoldOut[targetGroup] = true;
            }
        }

        protected virtual void OnEnable()
        {
            VRTK_SDKManager sdkManager = (VRTK_SDKManager)target;

            setupsList = new ReorderableList(serializedObject, serializedObject.FindProperty("setups"))
            {
                headerHeight = 2,
                drawElementCallback = (rect, index, active, focused) =>
                {
                    SerializedProperty serializedProperty = setupsList.serializedProperty;
                    if (serializedProperty.arraySize <= index)
                    {
                        return;
                    }

                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    Color previousColor = GUI.color;
                    if (IsSDKSetupNeverUsed(index))
                    {
                        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, 0.5f);
                    }

                    GUIContent unloadButtonGUIContent = new GUIContent("Unload", "Unload this SDK Setup.");
                    GUIContent loadButtonGUIContent = new GUIContent("Load", "Try to load this SDK Setup.");
                    float buttonGUIContentWidth = Mathf.Max(
                        GUI.skin.button.CalcSize(unloadButtonGUIContent).x,
                        GUI.skin.button.CalcSize(loadButtonGUIContent).x
                    );

                    VRTK_SDKSetup setup = (VRTK_SDKSetup)serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
                    if (EditorApplication.isPlaying && setup != null)
                    {
                        rect.width -= buttonGUIContentWidth + ReorderableList.Defaults.padding;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rect,
                                            serializedProperty.GetArrayElementAtIndex(index),
                                            GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        setup = (VRTK_SDKSetup)serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
                        if (setup != null)
                        {
                            int indexOfExistingDuplicateSetup = Enumerable
                                .Range(0, serializedProperty.arraySize)
                                .Except(new[] { index })
                                .Where(i => (VRTK_SDKSetup)serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == setup)
                                .DefaultIfEmpty(-1)
                                .First();
                            if (indexOfExistingDuplicateSetup != -1)
                            {
                                serializedProperty.GetArrayElementAtIndex(indexOfExistingDuplicateSetup).objectReferenceValue = null;

                                setupsList.index = indexOfExistingDuplicateSetup;
                                ReorderableList.defaultBehaviours.DoRemoveButton(setupsList);
                                setupsList.index = index;
                            }
                        }
                        sdkManager.ManageVRSettings(false);
                    }

                    GUI.color = previousColor;

                    if (EditorApplication.isPlaying && setup != null)
                    {
                        rect.x += rect.width + ReorderableList.Defaults.padding;
                        rect.width = buttonGUIContentWidth;

                        if (sdkManager.loadedSetup == setup)
                        {
                            if (GUI.Button(rect, unloadButtonGUIContent))
                            {
                                sdkManager.UnloadSDKSetup();
                            }
                        }
                        else
                        {
                            if (GUI.Button(rect, loadButtonGUIContent))
                            {
                                sdkManager.TryLoadSDKSetup(index, true, sdkManager.setups);
                            }
                        }
                    }
                },
                onAddCallback = list =>
                {
                    SerializedProperty serializedProperty = list.serializedProperty;
                    int index = serializedProperty.arraySize;
                    serializedProperty.arraySize++;
                    list.index = index;

                    SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
                    element.objectReferenceValue = null;

                    sdkManager.ManageVRSettings(false);
                },
                onRemoveCallback = list =>
                {
                    int index = list.index;
                    VRTK_SDKSetup sdkSetup = sdkManager.setups[index];
                    bool isLoaded = sdkManager.loadedSetup == sdkSetup;

                    if (isLoaded)
                    {
                        sdkManager.UnloadSDKSetup();
                    }

                    if (sdkSetup != null)
                    {
                        list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
                    }

                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    sdkManager.ManageVRSettings(false);
                },
                onReorderCallback = list => sdkManager.ManageVRSettings(false)
            };

            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            setupsList = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VRTK_SDKManager sdkManager = (VRTK_SDKManager)target;
            const string manageNowButtonText = "Manage Now";

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("Scripting Define Symbols", false);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    bool autoManage = EditorGUILayout.Toggle(
                        VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("autoManageScriptDefines", "Auto Manage"),
                        sdkManager.autoManageScriptDefines,
                        GUILayout.ExpandWidth(false)
                    );
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.FindProperty("autoManageScriptDefines").boolValue = autoManage;
                        serializedObject.ApplyModifiedProperties();
                        sdkManager.ManageScriptingDefineSymbols(false, true);
                    }

                    using (new EditorGUI.DisabledGroupScope(sdkManager.autoManageScriptDefines))
                    {
                        GUIContent manageNowGUIContent = new GUIContent(
                            manageNowButtonText,
                            "Manage the scripting define symbols defined by the installed SDKs."
                            + (sdkManager.autoManageScriptDefines
                                   ? "\n\nThis button is disabled because the SDK Manager is set up to manage the scripting define symbols automatically."
                                     + " Disable the checkbox on the left to allow managing them manually instead."
                                   : "")
                        );
                        if (GUILayout.Button(manageNowGUIContent, GUILayout.MaxHeight(GUI.skin.label.CalcSize(manageNowGUIContent).y)))
                        {
                            sdkManager.ManageScriptingDefineSymbols(true, true);
                        }
                    }
                }

                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    VRTK_EditorUtilities.AddHeader("Active Symbols Without SDK Classes", false);

                    VRTK_SDKInfo[] availableSDKInfos = VRTK_SDKManager
                        .AvailableSystemSDKInfos
                        .Concat(VRTK_SDKManager.AvailableBoundariesSDKInfos)
                        .Concat(VRTK_SDKManager.AvailableHeadsetSDKInfos)
                        .Concat(VRTK_SDKManager.AvailableControllerSDKInfos)
                        .ToArray();
                    HashSet<string> sdkSymbols = new HashSet<string>(availableSDKInfos.Select(info => info.description.symbol));
                    IGrouping<BuildTargetGroup, VRTK_SDKManager.ScriptingDefineSymbolPredicateInfo>[] availableGroupedPredicateInfos = VRTK_SDKManager
                        .AvailableScriptingDefineSymbolPredicateInfos
                        .GroupBy(info => info.attribute.buildTargetGroup)
                        .ToArray();
                    foreach (IGrouping<BuildTargetGroup, VRTK_SDKManager.ScriptingDefineSymbolPredicateInfo> grouping in availableGroupedPredicateInfos)
                    {
                        VRTK_SDKManager.ScriptingDefineSymbolPredicateInfo[] possibleActiveInfos = grouping
                            .Where(info => !sdkSymbols.Contains(info.attribute.symbol)
                                           && grouping.Except(new[] { info })
                                                      .All(predicateInfo => !(predicateInfo.methodInfo == info.methodInfo
                                                                              && sdkSymbols.Contains(predicateInfo.attribute.symbol))))
                            .OrderBy(info => info.attribute.symbol)
                            .ToArray();
                        if (possibleActiveInfos.Length == 0)
                        {
                            continue;
                        }

                        EditorGUI.indentLevel++;

                        BuildTargetGroup targetGroup = grouping.Key;
                        isBuildTargetActiveSymbolsFoldOut[targetGroup] = EditorGUI.Foldout(
                            EditorGUILayout.GetControlRect(),
                            isBuildTargetActiveSymbolsFoldOut[targetGroup],
                            targetGroup.ToString(),
                            true
                        );

                        if (isBuildTargetActiveSymbolsFoldOut[targetGroup])
                        {
                            foreach (VRTK_SDKManager.ScriptingDefineSymbolPredicateInfo predicateInfo in possibleActiveInfos)
                            {
                                int symbolIndex = sdkManager
                                    .activeScriptingDefineSymbolsWithoutSDKClasses
                                    .FindIndex(attribute => attribute.symbol == predicateInfo.attribute.symbol);
                                string symbolLabel = predicateInfo.attribute.symbol.Remove(
                                    0,
                                    SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix.Length
                                );

                                if (!(bool)predicateInfo.methodInfo.Invoke(null, null))
                                {
                                    symbolLabel += " (not installed)";
                                }

                                EditorGUI.BeginChangeCheck();
                                bool isSymbolActive = EditorGUILayout.ToggleLeft(symbolLabel, symbolIndex != -1);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(sdkManager, "Active Symbol Change");
                                    if (isSymbolActive)
                                    {
                                        sdkManager.activeScriptingDefineSymbolsWithoutSDKClasses.Add(predicateInfo.attribute);
                                    }
                                    else
                                    {
                                        sdkManager.activeScriptingDefineSymbolsWithoutSDKClasses.RemoveAt(symbolIndex);
                                    }
                                    sdkManager.ManageScriptingDefineSymbols(false, true);
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }

                VRTK_EditorUtilities.DrawUsingDestructiveStyle(GUI.skin.button, style =>
                {
                    GUIContent clearSymbolsGUIContent = new GUIContent(
                        "Remove All Symbols",
                        "Remove all scripting define symbols of VRTK. This is handy if you removed the SDK files from your project but still have"
                        + " the symbols defined which results in compile errors."
                        + "\nIf you have the above checkbox enabled the symbols will be managed automatically after clearing them. Otherwise hit the"
                        + " '" + manageNowButtonText + "' button to add the symbols for the currently installed SDKs again."
                    );

                    if (GUILayout.Button(clearSymbolsGUIContent, style))
                    {
                        BuildTargetGroup[] targetGroups = VRTK_SharedMethods.GetValidBuildTargetGroups();

                        foreach (BuildTargetGroup targetGroup in targetGroups)
                        {
                            string[] currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                                                                    .Split(';')
                                                                    .Distinct()
                                                                    .OrderBy(symbol => symbol, StringComparer.Ordinal)
                                                                    .ToArray();
                            string[] newSymbols = currentSymbols
                                .Where(symbol => !symbol.StartsWith(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix, StringComparison.Ordinal))
                                .ToArray();

                            if (currentSymbols.SequenceEqual(newSymbols))
                            {
                                continue;
                            }

                            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", newSymbols));

                            string[] removedSymbols = currentSymbols.Except(newSymbols).ToArray();
                            if (removedSymbols.Length > 0)
                            {
                                VRTK_Logger.Info(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_REMOVED, targetGroup, string.Join(", ", removedSymbols)));
                            }
                        }
                    }
                });
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("Script Aliases", false);
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("scriptAliasLeftController"),
                    VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("scriptAliasLeftController", "Left Controller")
                );
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("scriptAliasRightController"),
                    VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("scriptAliasRightController", "Right Controller")
                );
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("Setups", false);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    bool autoManage = EditorGUILayout.Toggle(
                        VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("autoManageVRSettings"),
                        sdkManager.autoManageVRSettings,
                        GUILayout.ExpandWidth(false)
                    );
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.FindProperty("autoManageVRSettings").boolValue = autoManage;
                        serializedObject.ApplyModifiedProperties();
                        sdkManager.ManageVRSettings(false);
                    }

                    using (new EditorGUI.DisabledGroupScope(sdkManager.autoManageVRSettings))
                    {
                        GUIContent manageNowGUIContent = new GUIContent(
                            manageNowButtonText,
                            "Manage the VR settings of the Player Settings to allow for all the installed SDKs."
                            + (sdkManager.autoManageVRSettings
                                   ? "\n\nThis button is disabled because the SDK Manager is set up to manage the VR Settings automatically."
                                     + " Disable the checkbox on the left to allow managing them manually instead."
                                   : "")
                        );
                        if (GUILayout.Button(manageNowGUIContent, GUILayout.MaxHeight(GUI.skin.label.CalcSize(manageNowGUIContent).y)))
                        {
                            sdkManager.ManageVRSettings(true);
                        }
                    }
                }

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("autoLoadSetup"),
                    VRTK_EditorUtilities.BuildGUIContent<VRTK_SDKManager>("autoLoadSetup", "Auto Load")
                );

                setupsList.DoLayoutList();

                GUIContent autoPopulateGUIContent = new GUIContent("Auto Populate", "Automatically populates the list of SDK Setups with Setups in the scene.");
                if (GUILayout.Button(autoPopulateGUIContent))
                {
                    SerializedProperty serializedProperty = setupsList.serializedProperty;
                    serializedProperty.ClearArray();
                    VRTK_SDKSetup[] setups = sdkManager.GetComponentsInChildren<VRTK_SDKSetup>(true)
                                                       .Concat(VRTK_SharedMethods.FindEvenInactiveComponents<VRTK_SDKSetup>())
                                                       .Distinct()
                                                       .ToArray();

                    for (int index = 0; index < setups.Length; index++)
                    {
                        VRTK_SDKSetup setup = setups[index];
                        serializedProperty.InsertArrayElementAtIndex(index);
                        serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = setup;
                    }
                }

                if (sdkManager.setups.Length > 1)
                {
                    EditorGUILayout.HelpBox("Duplicated setups are removed automatically.", MessageType.Info);
                }

                if (Enumerable.Range(0, sdkManager.setups.Length).Any(IsSDKSetupNeverUsed))
                {
                    EditorGUILayout.HelpBox("Gray setups will never be loaded because either the SDK Setup isn't valid or there"
                                        + " is a valid Setup before it that uses any non-VR SDK.",
                                        MessageType.Warning);
                }
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                VRTK_EditorUtilities.AddHeader("Target Platform Group Exclusions", false);
                SerializedProperty excludeTargetGroups = serializedObject.FindProperty("excludeTargetGroups");
                excludeTargetGroups.arraySize = EditorGUILayout.IntField("Size", excludeTargetGroups.arraySize);
                for (int i = 0; i < excludeTargetGroups.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(excludeTargetGroups.GetArrayElementAtIndex(i));
                }
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("persistOnLoad"));

            serializedObject.ApplyModifiedProperties();
        }

        private void UndoRedoPerformed()
        {
            VRTK_SDKManager sdkManager = (VRTK_SDKManager)target;
            sdkManager.ManageVRSettings(false);
            sdkManager.ManageScriptingDefineSymbols(false, false);
        }

        private bool IsSDKSetupNeverUsed(int sdkSetupIndex)
        {
            VRTK_SDKSetup[] setups = ((VRTK_SDKManager)target).setups;
            VRTK_SDKSetup setup = setups[sdkSetupIndex];

            return setup == null
                   || !setup.isValid
                   || Array.FindIndex(
                       setups,
                       0,
                       sdkSetupIndex,
                       sdkSetup => sdkSetup != null
                                   && sdkSetup.isValid
                                   && sdkSetup.usedVRDeviceNames.Contains("None")
                   ) != -1;
        }
    }
}