namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_Keyboard))]
    public class VRTK_KeyboardEditor : Editor
    {
        SerializedProperty fixedInputField;

        protected Dictionary<VRTK_KeyLayoutRendererAttribute, Type> keyLayoutRenderers;
        protected Dictionary<VRTK_KeyLayoutCalculatorAttribute, Type> keyLayoutCalculators;

        private void OnEnable()
        {
            fixedInputField = serializedObject.FindProperty("fixedInputField");

            keyLayoutRenderers = GetAttributesFromVRTKClasses<VRTK_KeyLayoutRendererAttribute>();
            keyLayoutCalculators = GetAttributesFromVRTKClasses<VRTK_KeyLayoutCalculatorAttribute>();
        }

        /// <summary>
        /// Finds all usage of a VRTK class attribute
        /// </summary>
        /// <typeparam name="A">The VRTK class attribute</typeparam>
        /// <returns>A list of instances of this attribute class</returns>
        private Dictionary<A, Type> GetAttributesFromVRTKClasses<A>() where A : Attribute
        {
            Dictionary<A, Type> attributes = new Dictionary<A, Type>();

            string definedIn = typeof(A).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache)
                {
                    continue;
                }

                if (assembly.GetName().Name != definedIn)
                {
                    bool isReferenced = false;
                    foreach (AssemblyName refAssembly in assembly.GetReferencedAssemblies())
                    {
                        if (refAssembly.Name == definedIn)
                        {
                            isReferenced = true;
                            break;
                        }
                    }

                    if (!isReferenced)
                    {
                        continue;
                    }
                }

                foreach (Type type in assembly.GetTypes())
                {
                    Attribute attribute = null;
                    foreach (Attribute a in type.GetCustomAttributes(typeof(A), true))
                    {
                        attribute = a;
                        break;
                    }
                    if (attribute != null)
                    {
                        attributes.Add((A)attribute, type);
                    }
                }
            }

            return attributes;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(fixedInputField);

            /// TODO: Give VRTK_Keyboard an editor
            ///       - Next to fixedInputField add some text ("All keyboard input passed to InputField", "Keyboard available for use by keyboard actors")
            ///       - Add a selector interface to select a key renderer and key layout calculator
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

            if (keyboard.GetComponent<VRTK_BaseKeyLayoutCalculator>() == null)
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

            if (keyboard.GetComponent<VRTK_BaseKeyLayoutRenderer>() == null)
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
        }
    }
}
