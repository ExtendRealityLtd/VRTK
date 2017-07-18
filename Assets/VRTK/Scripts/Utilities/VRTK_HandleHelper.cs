// Handle Helper|Utilities|90075
namespace VRTK
{
    using UnityEngine;

    public class VRTK_HandleHelper : MonoBehaviour
    {

        public SDK_BaseController.ControllerType Controller = SDK_BaseController.ControllerType.SteamVR_ViveWand;
        public SDK_BaseController.ControllerHand Hand = SDK_BaseController.ControllerHand.Left;

        private void OnDrawGizmos()
        {
            switch (Controller)
            {
                case SDK_BaseController.ControllerType.SteamVR_ViveWand:
                    DrawViveControllerRough();
                    break;
                case SDK_BaseController.ControllerType.SteamVR_ValveKnuckles:
                    break;
                case SDK_BaseController.ControllerType.SteamVR_OculusTouch:
                    break;
                case SDK_BaseController.ControllerType.Oculus_OculusTouch:
                    break;
            }
        }

        private void DrawViveControllerRough()
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