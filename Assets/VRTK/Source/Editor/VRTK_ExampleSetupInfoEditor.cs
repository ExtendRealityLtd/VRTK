namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [InitializeOnLoad]
    public sealed class VRTK_ExampleSetupInfoEditor : EditorWindow
    {
        private static VRTK_ExampleSetupInfoEditor promptWindow;
        private static Vector2 scrollPosition;
        private const string hideInfoBoxKey = "VRTK.ExampleSetupInfo";
        private const string toggleText = "Do not show this message again.";
        private const string buttonText = "Close";

        static VRTK_ExampleSetupInfoEditor()
        {
            EditorSceneManager.sceneOpened += SceneOpened;
        }

        [MenuItem("Window/VRTK/Example Setup Information")]
        public static void ShowWindow()
        {
            if (promptWindow != null)
            {
                promptWindow.ShowUtility();
                promptWindow.Repaint();
                return;
            }

            promptWindow = GetWindow<VRTK_ExampleSetupInfoEditor>(true);
            promptWindow.titleContent = new GUIContent("VRTK Example Setup Information");
            promptWindow.minSize = new Vector2(500, 150);
        }

        private static void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            AttemptShowWindow();
        }

        private static void AttemptShowWindow()
        {
            if (!EditorPrefs.HasKey(hideInfoBoxKey))
            {
                ShowWindow();
            }
        }

        private void OnGUI()
        {
            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;
                using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
                {
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 14,
                        wordWrap = true,
                        margin = new RectOffset(10, 10, 5, 0)
                    };
                    EditorGUILayout.LabelField(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_NOT_FOUND), labelStyle, GUILayout.ExpandHeight(true));
                }
            }

            GUILayout.Space(10);

            using (EditorGUI.ChangeCheckScope changeShowMessage = new EditorGUI.ChangeCheckScope())
            {
                bool hideToggle = EditorPrefs.HasKey(hideInfoBoxKey);

                hideToggle = GUILayout.Toggle(hideToggle, toggleText);

                if (changeShowMessage.changed)
                {
                    if (hideToggle)
                    {
                        EditorPrefs.SetBool(hideInfoBoxKey, true);
                    }
                    else
                    {
                        EditorPrefs.DeleteKey(hideInfoBoxKey);
                    }
                }
            }

            if (GUILayout.Button(buttonText))
            {
                Close();
            }
        }
    }
}