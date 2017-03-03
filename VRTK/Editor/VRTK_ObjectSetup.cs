namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using VRTK.GrabAttachMechanics;
    using VRTK.SecondaryControllerGrabActions;

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
            
            window.minSize = new Vector2( 300f, 300f );
            window.maxSize = new Vector2( 400f, 300f );
            
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

            if(GUILayout.Button("Setup selected object", GUILayout.Height(40)))
            {
                SetupObject();
            }
        }

        private void SetupObject()
        {
            GameObject go = Selection.activeGameObject;
            if(go)
            {
                VRTK_InteractableObject intObj = go.GetComponent<VRTK_InteractableObject>();
                if(intObj == null)
                {
                    intObj = go.AddComponent<VRTK_InteractableObject>();
                }
                intObj.touchHighlightColor = touchColor;
                intObj.isGrabbable = useGrab;
                intObj.holdButtonToGrab = holdGrab;
                intObj.isUsable = useUse;
                intObj.disableWhenIdle = disableIdle;
                intObj.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                intObj.useOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                VRTK_BaseGrabAttach grab = go.GetComponent<VRTK_BaseGrabAttach>();
                if(grab != null)
                {
                    DestroyImmediate(grab);
                }
                switch(primGrab)
                {
                    case PrimaryGrab.ChildOfController:
                        grab = go.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                        break;
                    case PrimaryGrab.FixedJoint:
                        grab = go.AddComponent<VRTK_FixedJointGrabAttach>();
                        break;
                    case PrimaryGrab.Climbable:
                        grab = go.AddComponent<VRTK_ClimbableGrabAttach>();
                        break;
                    case PrimaryGrab.CustomJoint:
                        grab = go.AddComponent<VRTK_CustomJointGrabAttach>();
                        break;
                    case PrimaryGrab.RotatorTrack:
                        grab = go.AddComponent<VRTK_RotatorTrackGrabAttach>();
                        break;
                    case PrimaryGrab.SpringJoint:
                        grab = go.AddComponent<VRTK_SpringJointGrabAttach>();
                        break;
                    case PrimaryGrab.TrackObject:
                        grab = go.AddComponent<VRTK_TrackObjectGrabAttach>();
                        break;
                    default:
                        grab = go.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                        break;
                }
                intObj.grabAttachMechanicScript = grab;
                VRTK_BaseGrabAction grab2 = go.GetComponent<VRTK_BaseGrabAction>();
                if(grab2 != null)
                {
                    DestroyImmediate(grab2);
                }
                switch(secGrab)
                {
                    case SecondaryGrab.SwapControllers:
                        grab2 = go.AddComponent<VRTK_SwapControllerGrabAction>();
                        break;
                    case SecondaryGrab.ControlDirection:
                        grab2 = go.AddComponent<VRTK_ControlDirectionGrabAction>();
                        break;
                    case SecondaryGrab.AxisScale:
                        grab2 = go.AddComponent<VRTK_AxisScaleGrabAction>();
                        break;
                    default:
                        grab2 = go.AddComponent<VRTK_SwapControllerGrabAction>();
                        break;
                }
                intObj.secondaryGrabActionScript = grab2;
                if(addrb)
                {
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if(rb == null)
                    {
                        go.AddComponent<Rigidbody>();
                    }
                }
                if(addHaptics)
                {
                    VRTK_InteractHaptics haptics = go.GetComponent<VRTK_InteractHaptics>();
                    if(haptics == null)
                    {
                        go.AddComponent<VRTK_InteractHaptics>();
                    }
                }
            }
        }
    }
}
