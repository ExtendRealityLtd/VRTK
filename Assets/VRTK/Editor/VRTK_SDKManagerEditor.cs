namespace VRTK
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_SDKManager))]
    public class VRTK_SDKManagerEditor : Editor
    {
        private const string SDKNotInstalledDescription = " (not installed)";
        private const string SDKNotFoundAnymoreDescription = " (not found)";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var sdkManager = (VRTK_SDKManager)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("persistOnLoad"));

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoPopulateObjectReferences"), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                sdkManager.PopulateObjectReferences(false);
            }

            const string populateNowDescription = "Populate Now";
            var populateNowGUIContent = new GUIContent(populateNowDescription, "Set the SDK object references to the objects of the selected SDKs.");
            if (GUILayout.Button(populateNowGUIContent, GUILayout.MaxHeight(GUI.skin.label.CalcSize(populateNowGUIContent).y)))
            {
                Undo.RecordObject(sdkManager, populateNowDescription);
                sdkManager.PopulateObjectReferences(true);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoManageScriptDefines"), GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                sdkManager.ManageScriptingDefineSymbols(false, true);
            }

            EditorGUI.BeginDisabledGroup(sdkManager.autoManageScriptDefines);
            const string manageNowDescription = "Manage Now";
            var manageNowGUIContent = new GUIContent(
                manageNowDescription,
                "Manage the scripting define symbols defined by the installed SDKs."
                + (sdkManager.autoManageScriptDefines
                   ? "\n\nThis button is disabled because the SDK Manager is set up to manage the scripting define symbols automatically."
                     + " Disable the checkbox on the left to allow managing them manually instead."
                   : "")
            );
            if (GUILayout.Button(manageNowGUIContent, GUILayout.MaxHeight(GUI.skin.label.CalcSize(manageNowGUIContent).y)))
            {
                Undo.RecordObject(sdkManager, manageNowDescription);
                sdkManager.ManageScriptingDefineSymbols(true, true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            var clearSymbolsGUIContent = new GUIContent(
                "Clear All Scripting Define Symbols",
                "Remove all scripting define symbols of VRTK. This is handy if you removed the SDK files from your project but still have"
                + " the symbols defined which results in compile errors."
                + "\nIf you have the above checkbox enabled the symbols will be managed automatically after clearing them. Otherwise hit the"
                + " '" + manageNowDescription + "' button to add the symbols for the currently installed SDKs again."
            );

            if (GUILayout.Button(clearSymbolsGUIContent, VRTK_EditorUtilities.CreateStyle(GUI.skin.button, Color.white, Color.red)))
            {
                //get valid BuildTargetGroups
                BuildTargetGroup[] targetGroups = Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>().Where(group =>
                {
                    if (group == BuildTargetGroup.Unknown)
                    {
                        return false;
                    }

                    string targetGroupName = Enum.GetName(typeof(BuildTargetGroup), group);
                    FieldInfo targetGroupFieldInfo = typeof(BuildTargetGroup).GetField(targetGroupName, BindingFlags.Public | BindingFlags.Static);

                    return targetGroupFieldInfo != null && targetGroupFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0;
                }).ToArray();

                foreach (BuildTargetGroup targetGroup in targetGroups)
                {
                    IEnumerable<string> nonSDKSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                        .Split(';')
                        .Where(symbol => !symbol.StartsWith(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix, StringComparison.Ordinal));
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", nonSDKSymbols.ToArray()));
                }
            }

            EditorGUILayout.BeginVertical("Box");
            VRTK_EditorUtilities.AddHeader("SDK Selection", false);

            EditorGUILayout.BeginVertical("Box");
            VRTK_EditorUtilities.AddHeader(ObjectNames.NicifyVariableName("activeScriptingDefineSymbolsWithoutSDKClasses"), false);

            Func<VRTK_SDKInfo, string> symbolSelector = info => info.description.symbol;
            var sdkSymbols = new HashSet<string>(
                VRTK_SDKManager.AvailableSystemSDKInfos.Select(symbolSelector)
                               .Concat(VRTK_SDKManager.AvailableBoundariesSDKInfos.Select(symbolSelector))
                               .Concat(VRTK_SDKManager.AvailableHeadsetSDKInfos.Select(symbolSelector))
                               .Concat(VRTK_SDKManager.AvailableControllerSDKInfos.Select(symbolSelector))
            );
            foreach (VRTK_SDKManager.ScriptingDefineSymbolPredicateInfo info in VRTK_SDKManager.AvailableScriptingDefineSymbolPredicateInfos)
            {
                string symbol = info.attribute.symbol;
                if (sdkSymbols.Contains(symbol)
                    || VRTK_SDKManager.AvailableScriptingDefineSymbolPredicateInfos
                                      .Except(new[] { info })
                                      .Any(predicateInfo => predicateInfo.methodInfo == info.methodInfo))
                {
                    continue;
                }

                int index = sdkManager.activeScriptingDefineSymbolsWithoutSDKClasses.FindIndex(attribute => attribute.symbol == symbol);
                string label = symbol.Remove(0, SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix.Length);

                EditorGUI.BeginChangeCheck();
                bool newValue = EditorGUILayout.ToggleLeft(label, index != -1);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sdkManager, "Active Symbol Change");
                    if (newValue)
                    {
                        sdkManager.activeScriptingDefineSymbolsWithoutSDKClasses.Add(info.attribute);
                    }
                    else
                    {
                        sdkManager.activeScriptingDefineSymbolsWithoutSDKClasses.RemoveAt(index);
                    }
                    sdkManager.ManageScriptingDefineSymbols(false, true);
                }
            }

            EditorGUILayout.EndVertical();

            HandleSDKSelection<SDK_BaseSystem>("The SDK to use to deal with all system actions.");
            HandleSDKSelection<SDK_BaseBoundaries>("The SDK to use to utilize room scale boundaries.");
            HandleSDKSelection<SDK_BaseHeadset>("The SDK to use to utilize the VR headset.");
            HandleSDKSelection<SDK_BaseController>("The SDK to use to utilize the input devices.");

            string sdkErrorDescriptions = string.Join("\n", sdkManager.GetSimplifiedSDKErrorDescriptions());
            if (!string.IsNullOrEmpty(sdkErrorDescriptions))
            {
                EditorGUILayout.HelpBox(sdkErrorDescriptions, MessageType.Error);
            }

            EditorGUILayout.Space();

            string[] availableSystemSDKNames = VRTK_SDKManager.AvailableSystemSDKInfos.Select(info => info.description.prettyName + (VRTK_SDKManager.InstalledSystemSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToArray();
            string[] availableBoundariesSDKNames = VRTK_SDKManager.AvailableBoundariesSDKInfos.Select(info => info.description.prettyName + (VRTK_SDKManager.InstalledBoundariesSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToArray();
            string[] availableHeadsetSDKNames = VRTK_SDKManager.AvailableHeadsetSDKInfos.Select(info => info.description.prettyName + (VRTK_SDKManager.InstalledHeadsetSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToArray();
            string[] availableControllerSDKNames = VRTK_SDKManager.AvailableControllerSDKInfos.Select(info => info.description.prettyName + (VRTK_SDKManager.InstalledControllerSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToArray();

            Func<string, GUIContent> guiContentCreator = sdkName => new GUIContent(sdkName);
            GUIContent[] availableSDKGUIContents = availableSystemSDKNames
                .Intersect(availableBoundariesSDKNames)
                .Intersect(availableHeadsetSDKNames)
                .Intersect(availableControllerSDKNames)
                .Select(guiContentCreator)
                .ToArray();

            EditorGUI.BeginChangeCheck();
            int quicklySelectedSDKIndex = EditorGUILayout.Popup(new GUIContent("Quick select SDK", "Quickly select one of the SDKs into all slots."), 0, availableSDKGUIContents);
            if (EditorGUI.EndChangeCheck())
            {
                string quicklySelectedSDKName = availableSDKGUIContents[quicklySelectedSDKIndex].text.Replace(SDKNotInstalledDescription, "");
                Func<VRTK_SDKInfo, bool> predicate = info => info.description.prettyName == quicklySelectedSDKName;

                Undo.RecordObject(sdkManager, "SDK Change (Quick Select)");
                sdkManager.systemSDKInfo = VRTK_SDKManager.AvailableSystemSDKInfos.First(predicate);
                sdkManager.boundariesSDKInfo = VRTK_SDKManager.AvailableBoundariesSDKInfos.First(predicate);
                sdkManager.headsetSDKInfo = VRTK_SDKManager.AvailableHeadsetSDKInfos.First(predicate);
                sdkManager.controllerSDKInfo = VRTK_SDKManager.AvailableControllerSDKInfos.First(predicate);
            }

            GUIContent[] availableSystemSDKGUIContents = availableSystemSDKNames.Select(guiContentCreator).ToArray();
            GUIContent[] availableBoundariesSDKGUIContents = availableBoundariesSDKNames.Select(guiContentCreator).ToArray();
            GUIContent[] availableHeadsetSDKGUIContents = availableHeadsetSDKNames.Select(guiContentCreator).ToArray();
            GUIContent[] availableControllerSDKGUIContents = availableControllerSDKNames.Select(guiContentCreator).ToArray();
            if (availableSDKGUIContents.Length != availableSystemSDKGUIContents.Length
                || availableSDKGUIContents.Length != availableBoundariesSDKGUIContents.Length
                || availableSDKGUIContents.Length != availableHeadsetSDKGUIContents.Length
                || availableSDKGUIContents.Length != availableControllerSDKGUIContents.Length)
            {
                EditorGUILayout.HelpBox("Some of the available SDK implementations are only available for a subset of SDK endpoints. Quick Select only shows SDKs that provide an implementation for *all* the different SDK endpoints in VRTK (System, Boundaries, Headset, Controller).", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            if (sdkManager.autoPopulateObjectReferences)
            {
                EditorGUILayout.HelpBox("The SDK Manager is configured to auto populate object references so some of the following fields are disabled. Uncheck `Auto Populate Object References` above to enable customization of the below fields.", MessageType.Info);
            }

            VRTK_EditorUtilities.AddHeader("Linked Objects", false);

            EditorGUI.BeginDisabledGroup(sdkManager.autoPopulateObjectReferences);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualBoundaries"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualHeadset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actualRightController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelAliasLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelAliasRightController"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasLeftController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptAliasRightController"));

            EditorGUILayout.EndVertical();

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
            //make sure to trigger populating the object references in case an SDK change was undone
            var sdkManager = (VRTK_SDKManager)target;
            VRTK_SDKInfo systemSDKInfo = sdkManager.systemSDKInfo;
            VRTK_SDKInfo boundariesSDKInfo = sdkManager.boundariesSDKInfo;
            VRTK_SDKInfo headsetSDKInfo = sdkManager.headsetSDKInfo;
            VRTK_SDKInfo controllerSDKInfo = sdkManager.controllerSDKInfo;

            bool populatesObjectReferences = sdkManager.autoPopulateObjectReferences;
            sdkManager.autoPopulateObjectReferences = false;

            sdkManager.systemSDKInfo = null;
            sdkManager.boundariesSDKInfo = null;
            sdkManager.headsetSDKInfo = null;
            sdkManager.controllerSDKInfo = null;

            sdkManager.systemSDKInfo = systemSDKInfo;
            sdkManager.boundariesSDKInfo = boundariesSDKInfo;
            sdkManager.headsetSDKInfo = headsetSDKInfo;
            sdkManager.controllerSDKInfo = controllerSDKInfo;

            sdkManager.autoPopulateObjectReferences = populatesObjectReferences;

            if (!sdkManager.ManageScriptingDefineSymbols(false, false))
            {
                sdkManager.PopulateObjectReferences(false);
            }
        }

        #endregion

        /// <summary>
        /// Draws a popup menu and handles the selection for an SDK info.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="description">The description of the SDK base.</param>
        private void HandleSDKSelection<BaseType>(string description) where BaseType : SDK_Base
        {
            Type baseType = typeof(BaseType);
            Type sdkManagerType = typeof(VRTK_SDKManager);
            string baseName = baseType.Name.Remove(0, typeof(SDK_Base).Name.Length);

            var availableSDKInfos = (ReadOnlyCollection<VRTK_SDKInfo>)sdkManagerType
                .GetProperty(string.Format("Available{0}SDKInfos", baseName), BindingFlags.Public | BindingFlags.Static)
                .GetGetMethod()
                .Invoke(null, null);
            var installedSDKInfos = (ReadOnlyCollection<VRTK_SDKInfo>)sdkManagerType
                .GetProperty(string.Format("Installed{0}SDKInfos", baseName), BindingFlags.Public | BindingFlags.Static)
                .GetGetMethod()
                .Invoke(null, null);

            PropertyInfo sdkInfoPropertyInfo = sdkManagerType.GetProperty(string.Format("{0}SDKInfo", baseName.ToLowerInvariant()));
            var sdkManager = (VRTK_SDKManager)target;
            var selectedSDKInfo = (VRTK_SDKInfo)sdkInfoPropertyInfo.GetGetMethod().Invoke(sdkManager, null);

            List<string> availableSDKNames = availableSDKInfos.Select(info => info.description.prettyName + (installedSDKInfos.Contains(info) ? "" : SDKNotInstalledDescription)).ToList();
            int selectedSDKIndex = availableSDKInfos.IndexOf(selectedSDKInfo);
            if (selectedSDKInfo.originalTypeNameWhenFallbackIsUsed != null)
            {
                availableSDKNames.Add(selectedSDKInfo.originalTypeNameWhenFallbackIsUsed + SDKNotFoundAnymoreDescription);
                selectedSDKIndex = availableSDKNames.Count - 1;
            }

            GUIContent[] availableSDKGUIContents = availableSDKNames.Select(availableSDKName => new GUIContent(availableSDKName)).ToArray();

            EditorGUI.BeginChangeCheck();
            int newSelectedSDKIndex = EditorGUILayout.Popup(new GUIContent(string.Format("{0} SDK", baseName), description), selectedSDKIndex, availableSDKGUIContents);
            VRTK_SDKInfo newSelectedSDKInfo = selectedSDKInfo.originalTypeNameWhenFallbackIsUsed != null && newSelectedSDKIndex == availableSDKNames.Count - 1
                ? selectedSDKInfo
                : availableSDKInfos[newSelectedSDKIndex];
            if (EditorGUI.EndChangeCheck() && newSelectedSDKInfo != selectedSDKInfo)
            {
                Undo.RecordObject(sdkManager, string.Format("{0} SDK Change", baseName));
                sdkInfoPropertyInfo.GetSetMethod().Invoke(sdkManager, new object[] { newSelectedSDKInfo });
            }
        }
    }
}