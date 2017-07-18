namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using GrabAttachMechanics;

    public class VRTK_HandleHelperEditor : EditorWindow
    {
        private static VRTK_HandleHelperEditor window;

        private VRTK_InteractableObject currentIO;
        private static VRTK_BaseGrabAttach[] grabAttaches;
        private static bool[] toggleButtons;

        private SDK_BaseController.ControllerType controllerType = SDK_BaseController.ControllerType.SteamVR_ViveWand;

        [MenuItem("Window/VRTK/Handle Helper")]
        private static void Init()
        {
            window = (VRTK_HandleHelperEditor)GetWindow(typeof(VRTK_HandleHelperEditor));

            window.minSize = new Vector2(300f, 100f);
            
            window.autoRepaintOnSceneChange = true;
            window.titleContent.text = "Handle Helper";
            window.Show();
        }

        private void OnGUI()
        {
            controllerType = (SDK_BaseController.ControllerType)EditorGUILayout.EnumPopup("Controller Type: ", controllerType);

            if (grabAttaches.Length > 0)
            {
                for (int i = 0; i < grabAttaches.Length; i++)
                    toggleButtons[i] = GUILayout.Toggle(toggleButtons[i], ObjectNames.GetInspectorTitle(grabAttaches[i]));
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Update All Handles", GUILayout.Height(40)))
            {
                VRTK_BaseGrabAttach[] grabAttaches = FindObjectsOfType<VRTK_BaseGrabAttach>();

                foreach (VRTK_BaseGrabAttach grabAttach in grabAttaches)
                {
                    if (!grabAttach.IsTracked())
                    {
                        if (grabAttach.rightSnapHandle != null)
                            grabAttach.rightSnapHandle.localRotation = Quaternion.Inverse(grabAttach.rightSnapHandle.localRotation);
                        if (grabAttach.leftSnapHandle != null)
                            grabAttach.leftSnapHandle.localRotation = Quaternion.Inverse(grabAttach.leftSnapHandle.localRotation);
                    }
                }
            }
        }

        private void OnSelectionChange()
        {
            VRTK_InteractableObject soonestIO = Selection.activeTransform.GetComponent<VRTK_InteractableObject>();
            if(soonestIO == null) soonestIO = Selection.activeTransform.GetComponentInParent<VRTK_InteractableObject>();
            if (currentIO == null || currentIO != soonestIO)
            {
                currentIO = soonestIO;

                grabAttaches = currentIO.GetComponents<VRTK_BaseGrabAttach>();
                toggleButtons = new bool[grabAttaches.Length];
            }
        }

        private void OnEnable()
        {
            grabAttaches = new VRTK_BaseGrabAttach[0];
            toggleButtons = new bool[0];
            
            window = this;
        }

        private void OnDisable()
        {
            window = null;
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void RenderControllerGizmos(Transform objectTransform, GizmoType gizmoType)
        {
            if (window == null)
                return;

            for(int i = 0; i < grabAttaches.Length; i++)
            {
                if (toggleButtons[i])
                {
                    if (grabAttaches[i].rightSnapHandle != null)
                        window.DrawController(grabAttaches[i].rightSnapHandle, true);
                    if (grabAttaches[i].leftSnapHandle != null)
                        window.DrawController(grabAttaches[i].leftSnapHandle, false);
                }
            }
        }

        private void DrawController(Transform handle, bool rightHand)
        {
            switch(controllerType)
            {
                case (SDK_BaseController.ControllerType.SteamVR_ViveWand):
                    DrawViveControllerRough(handle);
                    break;
                case (SDK_BaseController.ControllerType.SteamVR_ValveKnuckles):
                    break;
                case (SDK_BaseController.ControllerType.SteamVR_OculusTouch):
                    break;
                case (SDK_BaseController.ControllerType.Oculus_OculusTouch):
                    break;
            }
        }

        private void DrawViveControllerRough(Transform transform)
        {
            Matrix4x4 mat = transform.localToWorldMatrix;
            Gizmos.matrix = mat;

            //Body
            Gizmos.DrawCube(new Vector3(0, 0, -0.1f), new Vector3(0.04f, 0.02f, 0.16f));

            //Trigger
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(0, -0.015f, -0.06f), new Vector3(0.02f, 0.04f, 0.01f));

            //Touchpad
            Gizmos.color = Color.blue;
            mat.SetTRS(transform.position, transform.rotation * Quaternion.Euler(0, 45, 0), transform.localScale);
            Gizmos.matrix = mat;
            Gizmos.DrawCube(new Vector3(0.04f, 0.01f, -0.04f), new Vector3(0.04f, 0.01f, 0.04f));

            //Tracking Circle
            Gizmos.color = Color.white;
            mat.SetTRS(transform.position, transform.rotation * Quaternion.Euler(60, 0, 0), transform.localScale);
            Gizmos.matrix = mat;
            Gizmos.DrawCube(new Vector3(0, -0.01f, 0.03f), new Vector3(0.075f, 0.02f, 0.075f));

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}