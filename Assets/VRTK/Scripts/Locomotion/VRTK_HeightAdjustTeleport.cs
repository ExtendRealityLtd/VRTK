// Height Adjust Teleport|Locomotion|20020
namespace VRTK
{
    using UnityEngine;
    using System;

    /// <summary>
    /// The height adjust teleporter extends the basic teleporter and allows for the y position of the user's position to be altered based on whether the teleport location is on top of another object.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/007_CameraRig_HeightAdjustTeleport` has a collection of varying height objects that the user can either walk up and down or use the laser pointer to climb on top of them.
    ///
    /// `VRTK/Examples/010_CameraRig_TerrainTeleporting` shows how the teleportation of a user can also traverse terrain colliders.
    ///
    /// `VRTK/Examples/020_CameraRig_MeshTeleporting` shows how the teleportation of a user can also traverse mesh colliders.
    /// </example>
    public class VRTK_HeightAdjustTeleport : VRTK_BasicTeleport
    {
        [Header("Height Adjust Settings")]

        [Tooltip("A custom raycaster to use when raycasting to find floors.")]
        public VRTK_CustomRaycast customRaycast;
        [Tooltip("**OBSOLETE [Use customRaycast]** The layers to ignore when raycasting to find floors.")]
        [Obsolete("`VRTK_HeightAdjustTeleport.layersToIgnore` is no longer used in the `VRTK_HeightAdjustTeleport` class, use the `customRaycast` parameter instead. This parameter will be removed in a future version of VRTK.")]
        public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

        protected override void OnEnable()
        {
            base.OnEnable();
            adjustYForTerrain = true;
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

        protected virtual float GetTeleportY(Transform target, Vector3 tipPosition)
        {
            float newY = playArea.position.y;
            float heightOffset = 0.1f;
            //Check to see if the tip is on top of an object
            Vector3 rayStartPositionOffset = Vector3.up * heightOffset;
            Ray ray = new Ray(tipPosition + rayStartPositionOffset, -playArea.up);
            RaycastHit rayCollidedWith;
            if (target && VRTK_CustomRaycast.Raycast(customRaycast, ray, out rayCollidedWith, layersToIgnore, Mathf.Infinity))
            {
                newY = (tipPosition.y - rayCollidedWith.distance) + heightOffset;
            }
            return newY;
        }
    }
}