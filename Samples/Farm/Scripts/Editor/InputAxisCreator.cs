namespace VRTK.Examples.Editor
{
    using UnityEngine;
    using UnityEditor;

    [InitializeOnLoad]
    public sealed class InputAxisCreator : EditorWindow
    {
        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap;
            public bool invert;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private const string windowTitle = "Manage Input Mappings";
        private const string hidePromptKey = "VRTK.HideInputMappingPrompt";
        private static InputAxisCreator promptWindow;
        private static bool isManualCheck;
        private static Vector2 scrollPosition;

        private static readonly InputAxis rightHorizontal = new InputAxis() { name = "VRTK_Axis4_RightHorizontal", dead = 0.001f, sensitivity = 3f, snap = true, type = AxisType.JoystickAxis, axis = 4, joyNum = 0 };
        private static readonly InputAxis rightVertical = new InputAxis() { name = "VRTK_Axis5_RightVertical", dead = 0.001f, sensitivity = 3f, snap = true, type = AxisType.JoystickAxis, axis = 5, joyNum = 0 };
        private static readonly InputAxis rightTrigger = new InputAxis() { name = "VRTK_Axis10_RightTrigger", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 10, joyNum = 0 };
        private static readonly InputAxis rightGrip = new InputAxis() { name = "VRTK_Axis12_RightGrip", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 12, joyNum = 0 };
        private static readonly InputAxis leftTrigger = new InputAxis() { name = "VRTK_Axis9_LeftTrigger", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 9, joyNum = 0 };
        private static readonly InputAxis leftGrip = new InputAxis() { name = "VRTK_Axis11_LeftGrip", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 11, joyNum = 0 };

        static InputAxisCreator()
        {
            EditorApplication.delayCall += ManageInputMappings;
        }

        public void OnGUI()
        {
            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                bool mappingsExist = AxisDefined(rightHorizontal.name);
                const string mappingsNotFound = "The required Input Mappings have not been found, click the 'Add Input Mappings' button below to automatically created the required Input Mappings.";
                const string mappingsFound = "The required Input Mappings have already been created. If you would like to delete these Input Mappings then manually remove the Input axes from the Unity Input Manager found in the Unity Project Settings.";
                string mappingText = mappingsExist ? mappingsFound : mappingsNotFound;
                MessageType messageType = mappingsExist ? MessageType.Info : MessageType.Warning;

                EditorGUILayout.HelpBox("The VRTK Example Scene requires additional Unity Input Axes to be defined for certain XR controllers.\n\n" + mappingText, messageType);

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    bool hideToggle = EditorPrefs.HasKey(GetProjectKeyName());

                    hideToggle = GUILayout.Toggle(hideToggle, "Do not prompt again.");
                    GUILayout.FlexibleSpace();

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (!mappingsExist)
                        {
                            if (GUILayout.Button("Add Input Mappings"))
                            {
                                AddInputMappings();
                                Close();
                            }
                        }
                    }

                    if (changeCheckScope.changed)
                    {
                        if (hideToggle)
                        {
                            EditorPrefs.SetBool(GetProjectKeyName(), true);
                        }
                        else
                        {
                            EditorPrefs.DeleteKey(GetProjectKeyName());
                        }
                    }
                }
            }
        }

        private static void ManageInputMappings()
        {
            if (isManualCheck)
            {
                ShowWindow();
            }
            else if (EditorPrefs.HasKey(GetProjectKeyName()) || AxisDefined(rightHorizontal.name))
            {
                EditorApplication.delayCall -= ManageInputMappings;
                return;
            }

            EditorApplication.delayCall -= ManageInputMappings;

            ShowWindow();
            isManualCheck = false;
        }

        private static void AddInputMappings()
        {
            AddAxis(rightHorizontal);
            AddAxis(rightVertical);
            AddAxis(rightTrigger);
            AddAxis(rightGrip);
            AddAxis(leftTrigger);
            AddAxis(leftGrip);
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        private static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name))
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }

        private static string GetProjectKeyName()
        {
            return hidePromptKey + AssetDatabase.AssetPathToGUID("Assets/VRTK");
        }

        private static void ShowWindow()
        {
            if (promptWindow != null)
            {
                promptWindow.ShowUtility();
                promptWindow.Repaint();
                return;
            }

            promptWindow = GetWindow<InputAxisCreator>(true);
            promptWindow.titleContent = new GUIContent("[VRTK] - " + windowTitle);
        }

        [MenuItem("Window/VRTK/" + windowTitle)]
        private static void CheckManually()
        {
            isManualCheck = true;
            EditorApplication.delayCall += ManageInputMappings;
        }
    }
}