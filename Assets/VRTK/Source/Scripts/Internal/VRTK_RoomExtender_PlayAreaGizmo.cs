namespace VRTK
{
    using UnityEngine;

    [ExecuteInEditMode]
    [System.Obsolete("`VRTK_RoomExtender_PlayAreaGizmo` will be removed in a future version of VRTK.")]
    public class VRTK_RoomExtender_PlayAreaGizmo : MonoBehaviour
    {
        public Color color = Color.red;
        public float wireframeHeight = 2.0f;
        public bool drawWireframeWhenSelectedOnly = false;

        protected Transform playArea;
        protected VRTK_RoomExtender roomExtender;

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            roomExtender = FindObjectOfType<VRTK_RoomExtender>();
            if (playArea == null || roomExtender == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_RoomExtender_PlayAreaGizmo", "PlayArea or VRTK_RoomExtender", "an active"));
                return;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnDrawGizmos()
        {
            if (!drawWireframeWhenSelectedOnly)
            {
                DrawWireframe();
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (drawWireframeWhenSelectedOnly)
            {
                DrawWireframe();
            }
        }

        protected virtual void DrawWireframe()
        {
            if (playArea == null || roomExtender == null)
            {
                return;
            }

            Vector3[] vertices = VRTK_SDK_Bridge.GetPlayAreaVertices();
            if (vertices == null || vertices.Length == 0)
            {
                return;
            }

            int btmRight = 4;
            int btmLeft = 5;
            int topLeft = 6;
            int topRight = 7;

            Vector3 btmRightVertex = vertices[btmRight] * roomExtender.additionalMovementMultiplier;
            Vector3 btmLeftVertex = vertices[btmLeft] * roomExtender.additionalMovementMultiplier;
            Vector3 topLeftVertex = vertices[topLeft] * roomExtender.additionalMovementMultiplier;
            Vector3 topRightVertex = vertices[topRight] * roomExtender.additionalMovementMultiplier;

            Vector3 btmOffset = new Vector3(0f, roomExtender.transform.localPosition.y, 0f);
            Vector3 topOffset = btmOffset + playArea.TransformVector(Vector3.up * wireframeHeight);
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