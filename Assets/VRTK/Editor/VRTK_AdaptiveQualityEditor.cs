#if (UNITY_5_4_OR_NEWER)
namespace VRTK
{
    using System;
    using System.Globalization;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(VRTK_AdaptiveQuality))]
    public class VRTK_AdaptiveQualityEditor : Editor
    {
        private const string NoSteamVR_CameraFoundHelpBoxText =
          "This script needs to be added to a `GameObject` that has a `SteamVR_Camera` attached to it.";
        private const string DontDisableHelpBoxText =
          "This script supports command line arguments to configure the adaptive quality scaling."
          + " If this script is disabled it won't respond to the arguments.\n\n"
          + "Leave this script enabled and use the `active` property if you want to disable"
          + " the adaptive quality scaling for now, but want to leave it possible for your built"
          + " binary to respond to the arguments.";
        private const string MaximumRenderScaleTooBigHelpBoxText =
          "The maximum render scale is too big. It's constrained by the maximum render target dimension below.";
        private const string NoRenderScaleLevelsYetHelpBoxText =
          "This is disabled because there are no render scale levels calculated yet."
          + " They will be calculated at runtime.";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var adaptiveQuality = (VRTK_AdaptiveQuality)target;
            if (adaptiveQuality.GetComponent<SteamVR_Camera>() == null)
            {
                EditorGUILayout.HelpBox(NoSteamVR_CameraFoundHelpBoxText, MessageType.Error);
                return;
            }

            EditorGUILayout.HelpBox(DontDisableHelpBoxText, adaptiveQuality.enabled ? MessageType.Warning : MessageType.Error);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("active"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawDebugVisualization"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("respondsToKeyboardShortcuts"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("respondsToCommandLineArguments"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("msaaLevel"));

            float minimumRenderScale = adaptiveQuality.minimumRenderScale;
            float maximumRenderScale = adaptiveQuality.maximumRenderScale;

            AddHeader(adaptiveQuality, "minimumRenderScale");
            EditorGUILayout.BeginHorizontal();
            {
                var fieldInfo = adaptiveQuality.GetType().GetField("minimumRenderScale");
                var rangeAttribute = (RangeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(RangeAttribute));

                EditorGUI.BeginChangeCheck();
                {
                    float maxFloatWidth = GUI.skin.textField.CalcSize(new GUIContent(1.23f.ToString(CultureInfo.InvariantCulture))).x;
                    minimumRenderScale = EditorGUILayout.FloatField(minimumRenderScale, GUILayout.MaxWidth(maxFloatWidth));

                    EditorGUILayout.MinMaxSlider(
                      ref minimumRenderScale,
                      ref maximumRenderScale,
                      rangeAttribute.min,
                      rangeAttribute.max);

                    maximumRenderScale = EditorGUILayout.FloatField(maximumRenderScale, GUILayout.MaxWidth(maxFloatWidth));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.FindProperty("minimumRenderScale").floatValue =
                      Mathf.Clamp((float)Math.Round(minimumRenderScale, 2), rangeAttribute.min, rangeAttribute.max);
                    serializedObject.FindProperty("maximumRenderScale").floatValue =
                      Mathf.Clamp((float)Math.Round(maximumRenderScale, 2), rangeAttribute.min, rangeAttribute.max);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (maximumRenderScale > adaptiveQuality.BiggestAllowedMaximumRenderScale())
            {
                EditorGUILayout.HelpBox(MaximumRenderScaleTooBigHelpBoxText, MessageType.Error);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumRenderTargetDimension"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("renderScaleFillRateStepSizeInPercent"));

            int maxRenderScaleLevel = Mathf.Max(adaptiveQuality.renderScales.Count - 1, 0);
            bool disabled = maxRenderScaleLevel == 0 || !Application.isPlaying;

            EditorGUI.BeginDisabledGroup(disabled);
            {
                AddHeader(adaptiveQuality, "overrideRenderScale");

                if (disabled)
                {
                    EditorGUI.EndDisabledGroup();
                    {
                        EditorGUILayout.HelpBox(NoRenderScaleLevelsYetHelpBoxText, MessageType.Info);
                    }
                    EditorGUI.BeginDisabledGroup(true);
                }

                adaptiveQuality.overrideRenderScale = EditorGUILayout.Toggle(
                  VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("overrideRenderScale"),
                  adaptiveQuality.overrideRenderScale);

                EditorGUI.BeginDisabledGroup(!adaptiveQuality.overrideRenderScale);
                {
                    adaptiveQuality.overrideRenderScaleLevel =
                      EditorGUILayout.IntSlider(
                        VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("overrideRenderScaleLevel"),
                        adaptiveQuality.overrideRenderScaleLevel,
                        0,
                        maxRenderScaleLevel);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private static void AddHeader(VRTK_AdaptiveQuality adaptiveQuality, string fieldName)
        {
            var fieldInfo = adaptiveQuality.GetType().GetField(fieldName);
            var headerAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(HeaderAttribute));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(headerAttribute.header, EditorStyles.boldLabel);
        }
    }
}
#endif