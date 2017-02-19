namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using AttributeUtils = VRTK_AttributeUtilities;

    [CustomEditor(typeof(VRTK_Keyboard))]
    public class VRTK_KeyboardEditor : Editor
    {
        SerializedProperty fixedInputField;

        protected Dictionary<VRTK_KeyLayoutRendererAttribute, Type> keyLayoutRenderers;
        protected Dictionary<VRTK_KeyLayoutCalculatorAttribute, Type> keyLayoutCalculators;
        protected Dictionary<VRTK_KeyLayoutSourceAttribute, Type> keyLayoutSources;
        protected Dictionary<Type, Type> keyLayoutSourceSelectors;
        protected Dictionary<Type, VRTK_BaseKeyboardLayoutSourceSelectorEditor> customSourceSelectorInstances;

        private void OnEnable()
        {
            fixedInputField = serializedObject.FindProperty("fixedInputField");

            keyLayoutRenderers = AttributeUtils.GetAttributeUsage<VRTK_KeyLayoutRendererAttribute>();
            keyLayoutCalculators = AttributeUtils.GetAttributeUsage<VRTK_KeyLayoutCalculatorAttribute>();
            keyLayoutSources = AttributeUtils.GetAttributeUsage<VRTK_KeyLayoutSourceAttribute>();
            customSourceSelectorInstances = new Dictionary<Type, VRTK_BaseKeyboardLayoutSourceSelectorEditor>();

            keyLayoutSourceSelectors = new Dictionary<Type, Type>();
            Dictionary<VRTK_CustomLayoutSourceSelectorAttribute, Type> customLayoutSourceSelectors = AttributeUtils.GetAttributeUsage<VRTK_CustomLayoutSourceSelectorAttribute>();
            foreach (KeyValuePair<VRTK_CustomLayoutSourceSelectorAttribute, Type> customLayoutSourceSelector in customLayoutSourceSelectors)
            {
                keyLayoutSourceSelectors.Add(customLayoutSourceSelector.Key.sourceType, customLayoutSourceSelector.Value);
            }
        }

        /// <summary>
        /// Return the keyboard layout source selector instance for a source type, create it if it does not exist
        /// </summary>
        /// <param name="sourceType">The VRTK_BaseKeyLayoutSource subtype</param>
        /// <returns>The keyboard layout source selector instance or null</returns>
        public VRTK_BaseKeyboardLayoutSourceSelectorEditor GetCustomSelectorInstance(Type sourceType)
        {
            if (!customSourceSelectorInstances.ContainsKey(sourceType))
            {
                Type selectorType;
                if (keyLayoutSourceSelectors.TryGetValue(sourceType, out selectorType))
                {
                    customSourceSelectorInstances.Add(sourceType, Activator.CreateInstance(selectorType) as VRTK_BaseKeyboardLayoutSourceSelectorEditor);
                }
            }

            VRTK_BaseKeyboardLayoutSourceSelectorEditor selectorInstance = null;
            customSourceSelectorInstances.TryGetValue(sourceType, out selectorInstance);
            return selectorInstance;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(fixedInputField);

            if (fixedInputField.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Keyboard available for use by keyboard actors", MessageType.None);
            }
            else
            {
                EditorGUILayout.HelpBox("All keyboard input passed to InputField", MessageType.None);
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            VRTK_Keyboard keyboard = (VRTK_Keyboard)target;
            VRTK_BaseKeyLayoutRenderer targetRenderer = keyboard.GetComponent<VRTK_BaseKeyLayoutRenderer>();
            VRTK_BaseKeyLayoutCalculator targetCalculator = keyboard.GetComponent<VRTK_BaseKeyLayoutCalculator>();
            VRTK_BaseKeyLayoutSource targetSource = keyboard.GetComponent<VRTK_BaseKeyLayoutSource>();
            bool calculatorNeeded = false;
            bool sourceNeeded = false;

            if (targetRenderer != null)
            {
                VRTK_KeyLayoutRendererAttribute rendererAttribute = AttributeUtils.GetAttribute<VRTK_KeyLayoutRendererAttribute>(targetRenderer.GetType());
                if (rendererAttribute != null)
                {
                    calculatorNeeded = rendererAttribute.requireCalculator;
                    sourceNeeded = rendererAttribute.requireSource;
                }
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label("Please select a renderer for the keyboard keys");
                foreach (KeyValuePair<VRTK_KeyLayoutRendererAttribute, Type> renderer in keyLayoutRenderers)
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label(renderer.Key.name, EditorStyles.boldLabel);
                    if (renderer.Key.help != null)
                    {
                        GUILayout.Label(renderer.Key.help);
                    }
                    if (GUILayout.Button("Create Component"))
                    {
                        GameObject gameObject = ((VRTK_Keyboard)target).gameObject;
                        gameObject.AddComponent(renderer.Value);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            if (calculatorNeeded)
            {
                if (targetCalculator != null)
                {
                    VRTK_KeyLayoutCalculatorAttribute calculatorAttribute = AttributeUtils.GetAttribute<VRTK_KeyLayoutCalculatorAttribute>(targetCalculator.GetType());
                    if (calculatorAttribute != null)
                    {
                        sourceNeeded = calculatorAttribute.requireSource;
                    }
                }
                else
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label("Please select a keyboard layout style");
                    foreach (KeyValuePair<VRTK_KeyLayoutCalculatorAttribute, Type> calculator in keyLayoutCalculators)
                    {
                        EditorGUILayout.BeginVertical("box");
                        GUILayout.Label(calculator.Key.name, EditorStyles.boldLabel);
                        if (calculator.Key.help != null)
                        {
                            GUILayout.Label(calculator.Key.help);
                        }
                        if (calculator.Key.helpList != null)
                        {
                            foreach (string helpLine in calculator.Key.helpList)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("-");
                                GUILayout.Label(helpLine);
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        if (GUILayout.Button("Create Component"))
                        {
                            GameObject gameObject = ((VRTK_Keyboard)target).gameObject;
                            gameObject.AddComponent(calculator.Value);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            if (sourceNeeded && targetSource == null)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label("Please select a keyboard layout source");
                foreach (KeyValuePair<VRTK_KeyLayoutSourceAttribute, Type> source in keyLayoutSources)
                {
                    VRTK_BaseKeyboardLayoutSourceSelectorEditor customSelector = GetCustomSelectorInstance(source.Value);

                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label(source.Key.name, EditorStyles.boldLabel);
                    if (source.Key.help != null)
                    {
                        GUILayout.Label(source.Key.help);
                    }
                    if (customSelector != null)
                    {
                        customSelector.KeyboardEditorGUI();
                    }
                    if (GUILayout.Button("Create Component"))
                    {
                        GameObject gameObject = ((VRTK_Keyboard)target).gameObject;
                        Component component = gameObject.AddComponent(source.Value);
                        if (customSelector != null)
                        {
                            customSelector.SetupComponent(component);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
