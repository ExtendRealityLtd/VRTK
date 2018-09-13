namespace VRTK.Examples.Editor
{
    using UnityEditor;

    [InitializeOnLoad]
    public class InputAxisCreator
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

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        static InputAxisCreator()
        {
            AddAxis(new InputAxis() { name = "VRTK_Axis4_RightHorizontal", dead = 0.001f, sensitivity = 3f, snap = true, type = AxisType.JoystickAxis, axis = 4, joyNum = 0 });
            AddAxis(new InputAxis() { name = "VRTK_Axis5_RightVertical", dead = 0.001f, sensitivity = 3f, snap = true, type = AxisType.JoystickAxis, axis = 5, joyNum = 0 });
            AddAxis(new InputAxis() { name = "VRTK_Axis9_LeftTrigger", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 9, joyNum = 0 });
            AddAxis(new InputAxis() { name = "VRTK_Axis10_RightTrigger", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 10, joyNum = 0 });
            AddAxis(new InputAxis() { name = "VRTK_Axis11_LeftGrip", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 11, joyNum = 0 });
            AddAxis(new InputAxis() { name = "VRTK_Axis12_RightGrip", dead = 0.001f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 12, joyNum = 0 });
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
            if (AxisDefined(axis.name)) return;

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
    }
}