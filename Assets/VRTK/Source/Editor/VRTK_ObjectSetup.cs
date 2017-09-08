namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using GrabAttachMechanics;
    using SecondaryControllerGrabActions;

    public class VRTK_ObjectSetup : EditorWindow
    {
        private enum PrimaryGrab
        {
            ChildOfController,
            FixedJoint,
            Climbable,
            CustomJoint,
            RotatorTrack,
            SpringJoint,
            TrackObject
        }
        private enum SecondaryGrab
        {
            SwapControllers,
            ControlDirection,
            AxisScale
        }
        private bool useGrab = true;
        private bool holdGrab = false;
        private bool useUse = false;
        private bool useIfGrabbed = false;
        private bool holdUse = false;
        private PrimaryGrab primGrab;
        private SecondaryGrab secGrab;
        private bool disableIdle = true;
        private bool addrb = true;
        private bool addHaptics = true;
        private Color touchColor = Color.clear;

        [MenuItem("Window/VRTK/Setup Interactable Object")]
        private static void Init()
        {
            VRTK_ObjectSetup window = (VRTK_ObjectSetup)EditorWindow.GetWindow(typeof(VRTK_ObjectSetup));

            window.minSize = new Vector2(300f, 370f);
            window.maxSize = new Vector2(400f, 400f);

            window.autoRepaintOnSceneChange = true;
            window.titleContent.text = "Setup Object";
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Touch Options", EditorStyles.boldLabel);
            touchColor = EditorGUILayout.ColorField("Touch Highlight Color", touchColor);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Grab Options", EditorStyles.boldLabel);
            useGrab = EditorGUILayout.Toggle("Is Grabbable", useGrab);
            holdGrab = EditorGUILayout.Toggle("Hold Button To Grab", holdGrab);
            EditorGUILayout.Space();
            primGrab = (PrimaryGrab)EditorGUILayout.EnumPopup("Grab Attach Mechanic", primGrab);
            secGrab = (SecondaryGrab)EditorGUILayout.EnumPopup("Secondary Grab Attach", secGrab);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Use Options", EditorStyles.boldLabel);
            useUse = EditorGUILayout.Toggle("Is Usable", useUse);
            holdUse = EditorGUILayout.Toggle("Hold Button To Use", holdUse);
            useIfGrabbed = EditorGUILayout.Toggle("Use Only If Grabbed", useIfGrabbed);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Misc Options", EditorStyles.boldLabel);
            disableIdle = EditorGUILayout.Toggle("Disable On Idle", disableIdle);
            addrb = EditorGUILayout.Toggle("Add RigidBody", addrb);
            addHaptics = EditorGUILayout.Toggle("Add Haptics", addHaptics);
            EditorGUILayout.Space();

            if (GUILayout.Button("Setup selected object(s)", GUILayout.Height(40)))
            {
                SetupObject();
            }
        }

        private void SetupObject()
        {
            Transform[] transforms = Selection.transforms;
            foreach (Transform currentTransform in transforms)
            {
                VRTK_InteractableObject interactableObject = SetupInteractableObject(currentTransform);
                SetupPrimaryGrab(interactableObject);
                SetupSecondaryGrab(interactableObject);
                SetupRigidbody(interactableObject);
                SetupHaptics(interactableObject);
                SetupHighlighter(interactableObject);
            }
        }

        private VRTK_InteractableObject SetupInteractableObject(Transform givenTransform)
        {
            VRTK_InteractableObject intObj = givenTransform.GetComponent<VRTK_InteractableObject>();
            if (intObj == null)
            {
                intObj = givenTransform.gameObject.AddComponent<VRTK_InteractableObject>();
            }
            intObj.isGrabbable = useGrab;
            intObj.holdButtonToGrab = holdGrab;
            intObj.isUsable = useUse;
            intObj.disableWhenIdle = disableIdle;
            intObj.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            intObj.useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            return intObj;
        }

        private void SetupPrimaryGrab(VRTK_InteractableObject givenObject)
        {
            VRTK_BaseGrabAttach grab = givenObject.GetComponentInChildren<VRTK_BaseGrabAttach>();
            if (grab != null)
            {
                DestroyImmediate(grab);
            }
            switch (primGrab)
            {
                case PrimaryGrab.ChildOfController:
                    grab = givenObject.gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                    break;
                case PrimaryGrab.FixedJoint:
                    grab = givenObject.gameObject.AddComponent<VRTK_FixedJointGrabAttach>();
                    break;
                case PrimaryGrab.Climbable:
                    grab = givenObject.gameObject.AddComponent<VRTK_ClimbableGrabAttach>();
                    break;
                case PrimaryGrab.CustomJoint:
                    grab = givenObject.gameObject.AddComponent<VRTK_CustomJointGrabAttach>();
                    break;
                case PrimaryGrab.RotatorTrack:
                    grab = givenObject.gameObject.AddComponent<VRTK_RotatorTrackGrabAttach>();
                    break;
                case PrimaryGrab.SpringJoint:
                    grab = givenObject.gameObject.AddComponent<VRTK_SpringJointGrabAttach>();
                    break;
                case PrimaryGrab.TrackObject:
                    grab = givenObject.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
                    break;
                default:
                    grab = givenObject.gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                    break;
            }
            givenObject.grabAttachMechanicScript = grab;
        }

        private void SetupSecondaryGrab(VRTK_InteractableObject givenObject)
        {
            VRTK_BaseGrabAction grab = givenObject.GetComponentInChildren<VRTK_BaseGrabAction>();
            if (grab != null)
            {
                DestroyImmediate(grab);
            }
            switch (secGrab)
            {
                case SecondaryGrab.SwapControllers:
                    grab = givenObject.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
                    break;
                case SecondaryGrab.ControlDirection:
                    grab = givenObject.gameObject.AddComponent<VRTK_ControlDirectionGrabAction>();
                    break;
                case SecondaryGrab.AxisScale:
                    grab = givenObject.gameObject.AddComponent<VRTK_AxisScaleGrabAction>();
                    break;
                default:
                    grab = givenObject.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
                    break;
            }
            givenObject.secondaryGrabActionScript = grab;
        }

        private void SetupRigidbody(VRTK_InteractableObject givenObject)
        {
            if (addrb)
            {
                Rigidbody rb = givenObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    givenObject.gameObject.AddComponent<Rigidbody>();
                }
            }
        }

        private void SetupHaptics(VRTK_InteractableObject givenObject)
        {
            if (addHaptics)
            {
                VRTK_InteractHaptics haptics = givenObject.GetComponentInChildren<VRTK_InteractHaptics>();
                if (haptics == null)
                {
                    givenObject.gameObject.AddComponent<VRTK_InteractHaptics>();
                }
            }
        }

        private void SetupHighlighter(VRTK_InteractableObject givenObject)
        {
            if (touchColor != Color.clear)
            {
                VRTK_InteractObjectHighlighter highlighter = givenObject.GetComponentInChildren<VRTK_InteractObjectHighlighter>();
                if (highlighter == null)
                {
                    highlighter = givenObject.gameObject.AddComponent<VRTK_InteractObjectHighlighter>();
                }
                highlighter.touchHighlight = touchColor;
            }
        }
    }
}
