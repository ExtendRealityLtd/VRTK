// Height Adjust Teleport|Locomotion|20020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Updates the `x/y/z` position of the SDK Camera Rig with an optional screen fade.
    /// </summary>
    /// <remarks>
    ///   > The Camera Rig can be automatically teleported to the nearest floor `y` position when utilising this teleporter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_HeightAdjustTeleport` script on any active scene GameObject.
    ///
    /// **Script Dependencies:**
    ///  * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/007_CameraRig_HeightAdjustTeleport` has a collection of varying height objects that the user can either walk up and down or use the laser pointer to climb on top of them.
    ///
    /// `VRTK/Examples/010_CameraRig_TerrainTeleporting` shows how the teleportation of a user can also traverse terrain colliders.
    ///
    /// `VRTK/Examples/020_CameraRig_MeshTeleporting` shows how the teleportation of a user can also traverse mesh colliders.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_HeightAdjustTeleport")]
    public class VRTK_HeightAdjustTeleport : VRTK_BasicTeleport
    {
        [Header("Height Adjust Settings")]

        [Tooltip("If this is checked, then the teleported Y position will snap to the nearest available below floor. If it is unchecked, then the teleported Y position will be where ever the destination Y position is.")]
        public bool snapToNearestFloor = true;
        [Tooltip("If this is checked then the teleported Y position will also be offset by the play area parent Transform Y position (if the play area has a parent).")]
        public bool applyPlayareaParentOffset = false;
        [Tooltip("A custom raycaster to use when raycasting to find floors.")]
        public VRTK_CustomRaycast customRaycast;

        protected override void OnEnable()
        {
            base.OnEnable();
            adjustYForTerrain = true;
            AdjustForParentOffset();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override Vector3 GetNewPosition(Vector3 tipPosition, Transform target, bool returnOriginalPosition)
        {
            Vector3 basePosition = base.GetNewPosition(tipPosition, target, returnOriginalPosition);
            if (!returnOriginalPosition)
            {
                basePosition.y = GetTeleportY(target, tipPosition);
            }
            return basePosition;
        }

        protected virtual void AdjustForParentOffset()
        {
            if (snapToNearestFloor && applyPlayareaParentOffset && playArea != null && playArea.parent != null)
            {
                Ray ray = new Ray(playArea.parent.position, -playArea.up);
                RaycastHit rayCollidedWith;
                if (VRTK_CustomRaycast.Raycast(customRaycast, ray, out rayCollidedWith, Physics.IgnoreRaycastLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore))
                {
                    playArea.position = new Vector3(playArea.position.x, playArea.position.y + rayCollidedWith.point.y, playArea.position.z);
                }
            }
        }

        protected virtual float GetParentOffset()
        {
            return (applyPlayareaParentOffset && playArea.parent != null ? playArea.parent.transform.localPosition.y : 0f);
        }

        protected virtual float GetTeleportY(Transform target, Vector3 tipPosition)
        {
            float parentOffset = GetParentOffset();
            if (!snapToNearestFloor || !ValidRigObjects())
            {
                return tipPosition.y + parentOffset;
            }

            float newY = playArea.position.y;
            float heightOffset = 0.1f;
            //Check to see if the tip is on top of an object
            Vector3 rayStartPositionOffset = Vector3.up * heightOffset;
            Ray ray = new Ray(tipPosition + rayStartPositionOffset, -playArea.up);
            RaycastHit rayCollidedWith;
            if (target != null && VRTK_CustomRaycast.Raycast(customRaycast, ray, out rayCollidedWith, Physics.IgnoreRaycastLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore))
            {
                newY = (tipPosition.y - rayCollidedWith.distance) + heightOffset;
            }
            return newY + parentOffset;
        }
    }
}