#if (UNITY_5_4_OR_NEWER)
namespace VRTK
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(VRTK_AdaptiveQuality))]
    public class VRTK_AdaptiveQualityEditor : Editor
    {
        private const string DontDisableHelpBoxText =
            "This script supports command line arguments to configure the adaptive quality scaling."
            + " If this script is disabled it won't respond to the arguments.\n\n"
            + "Leave this script enabled and use the `scaleRenderViewport` property if you want to disable"
            + " the adaptive quality scaling for now, but want to leave it possible for your built"
            + " binary to respond to the arguments.";
        private const string MaximumRenderScaleTooBigHelpBoxText =
            "The maximum render scale is too big. It's constrained by the maximum render target dimension below.";
        private const string ScaleRenderTargetResolutionCostlyHelpBoxText =
            "Changing the render target resolution is very costly and should be reduced to a minimum, therefore"
            + " this setting is turned off by default. Make sure you understand the performance implication this"
            + " setting has when leaving it enabled.";
        private const string NoRenderScaleLevelsYetHelpBoxText =
            "Overriding render viewport scale levels is disabled because there are no render viewport scale levels calculated yet."
            + " They will be calculated at runtime.";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VRTK_AdaptiveQuality adaptiveQuality = (VRTK_AdaptiveQuality)target;

            EditorGUILayout.HelpBox(DontDisableHelpBoxText, adaptiveQuality.enabled ? MessageType.Warning : MessageType.Error);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawDebugVisualization"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowKeyboardShortcuts"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowCommandLineArguments"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("msaaLevel"));

            EditorGUILayout.Space();
            serializedObject.FindProperty("scaleRenderViewport").boolValue =
                EditorGUILayout.BeginToggleGroup(VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("scaleRenderViewport"),
                                                 adaptiveQuality.scaleRenderViewport);
            {
                Limits2D renderScaleLimits = adaptiveQuality.renderScaleLimits;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("renderScaleLimits"));

                if (renderScaleLimits.maximum > adaptiveQuality.BiggestAllowedMaximumRenderScale())
                {
                    EditorGUILayout.HelpBox(MaximumRenderScaleTooBigHelpBoxText, MessageType.Error);
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumRenderTargetDimension"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("renderScaleFillRateStepSizeInPercent"));

                serializedObject.FindProperty("scaleRenderTargetResolution").boolValue =
                    EditorGUILayout.Toggle(VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("scaleRenderTargetResolution"),
                                           adaptiveQuality.scaleRenderTargetResolution);
                if (adaptiveQuality.scaleRenderTargetResolution)
                {
                    EditorGUILayout.HelpBox(ScaleRenderTargetResolutionCostlyHelpBoxText, MessageType.Warning);
                }

                int maxRenderScaleLevel = Mathf.Max(adaptiveQuality.renderScales.Count - 1, 0);
                bool disabled = maxRenderScaleLevel == 0 || !Application.isPlaying;

                EditorGUI.BeginDisabledGroup(disabled);
                {
                    VRTK_EditorUtilities.AddHeader<VRTK_AdaptiveQuality>("overrideRenderViewportScale");

                    if (disabled)
                    {
                        EditorGUI.EndDisabledGroup();
                        {
                            EditorGUILayout.HelpBox(NoRenderScaleLevelsYetHelpBoxText, MessageType.Info);
                        }
                        EditorGUI.BeginDisabledGroup(true);
                    }

                    adaptiveQuality.overrideRenderViewportScale = EditorGUILayout.Toggle(
                        VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("overrideRenderViewportScale"),
                        adaptiveQuality.overrideRenderViewportScale);

                    EditorGUI.BeginDisabledGroup(!adaptiveQuality.overrideRenderViewportScale);
                    {
                        adaptiveQuality.overrideRenderViewportScaleLevel =
                            EditorGUILayout.IntSlider(
                                VRTK_EditorUtilities.BuildGUIContent<VRTK_AdaptiveQuality>("overrideRenderViewportScaleLevel"),
                                adaptiveQuality.overrideRenderViewportScaleLevel,
                                0,
                                maxRenderScaleLevel);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndToggleGroup();

            if (Application.isPlaying)
            {
                string summary = adaptiveQuality.ToString();
                summary = summary.Substring(summary.IndexOf("\n", StringComparison.Ordinal) + 1);

                VRTK_EditorUtilities.AddHeader("Current State");
                EditorGUILayout.HelpBox(summary, MessageType.None);

                if (GUILayout.RepeatButton("Refresh"))
                {
                    Repaint();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif