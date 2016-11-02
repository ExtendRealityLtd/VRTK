namespace VRTK
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class VRTK_RoomExtender_PlayAreaGizmo : MonoBehaviour
    {
        public Color color = Color.red;
        public float wireframeHeight = 2.0f;
        public bool drawWireframeWhenSelectedOnly = false;

        private Transform playArea;
        private VRTK_RoomExtender vrtk_RoomExtender;

        private void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            vrtk_RoomExtender = FindObjectOfType<VRTK_RoomExtender>();
            if (playArea == null || vrtk_RoomExtender == null)
            {
                Debug.LogWarning("Could not find PlayArea or 'VRTK_RoomExtender'. Please check if they are attached to the CameraRig");
                return;
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawWireframeWhenSelectedOnly)
            {
                DrawWireframe();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (drawWireframeWhenSelectedOnly)
            {
                DrawWireframe();
            }
        }

        private void DrawWireframe()
        {
            var vertices = VRTK_SDK_Bridge.GetPlayAreaVertices(playArea.gameObject);

            if (vertices == null || vertices.Length == 0)
            {
                return;
            }

            var btmRight = 4;
            var btmLeft = 5;
            var topLeft = 6;
            var topRight = 7;

            var btmRightVertex = vertices[btmRight] * vrtk_RoomExtender.additionalMovementMultiplier;
            var btmLeftVertex = vertices[btmLeft] * vrtk_RoomExtender.additionalMovementMultiplier;
            var topLeftVertex = vertices[topLeft] * vrtk_RoomExtender.additionalMovementMultiplier;
            var topRightVertex = vertices[topRight] * vrtk_RoomExtender.additionalMovementMultiplier;

            var btmOffset = new Vector3(0f, vrtk_RoomExtender.transform.localPosition.y, 0f);
            var topOffset = btmOffset + transform.TransformVector(Vector3.up * wireframeHeight);
            Gizmos.color = color;
            //bottom rectangle
            Gizmos.DrawLine(btmRightVertex + btmOffset, btmLeftVertex + btmOffset);
            Gizmos.DrawLine(topLeftVertex + btmOffset, topRightVertex + btmOffset);
            Gizmos.DrawLine(btmRightVertex + btmOffset, topRightVertex + btmOffset);
            Gizmos.DrawLine(btmLeftVertex + btmOffset, topLeftVertex + btmOffset);

            //top rectangle
            Gizmos.DrawLine(btmRightVertex + topOffset, btmLeftVertex + topOffset);
            Gizmos.DrawLine(topLeftVertex + topOffset, topRightVertex + topOffset);
            Gizmos.DrawLine(btmRightVertex + topOffset, topRightVertex + topOffset);
            Gizmos.DrawLine(btmLeftVertex + topOffset, topLeftVertex + topOffset);

            //sides
            Gizmos.DrawLine(btmRightVertex + btmOffset, btmRightVertex + topOffset);
            Gizmos.DrawLine(btmLeftVertex + btmOffset, btmLeftVertex + topOffset);
            Gizmos.DrawLine(topRightVertex + btmOffset, topRightVertex + topOffset);
            Gizmos.DrawLine(topLeftVertex + btmOffset, topLeftVertex + topOffset);
        }
    }
}