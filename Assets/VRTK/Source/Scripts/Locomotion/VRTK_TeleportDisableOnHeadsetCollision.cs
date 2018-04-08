// Teleport Disable On Headset Collision|Locomotion|20040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Prevents teleportation when the HMD is colliding with valid geometry.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_BasicTeleport` - A Teleport script to utilise for teleporting the play area.
    ///  * `VRTK_HeadsetCollision` - A Headset Collision script for detecting when the headset has collided with valid geometry.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_TeleportDisableOnHeadsetCollision` script on any active scene GameObject.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TeleportDisableOnHeadsetCollision")]
    public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
    {
        [Header("Custom Settings")]

        [Tooltip("The Teleporter script to deal play area teleporting. If the script is being applied onto an object that already has a VRTK_BasicTeleport component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        public VRTK_BasicTeleport teleporter;

        [Tooltip("The VRTK Headset Collision script to use when determining headset collisions. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetCollision headsetCollision;

        protected Coroutine enableAtEndOfFrameRoutine;

        protected virtual void OnEnable()
        {
            teleporter = (teleporter != null ? teleporter : FindObjectOfType<VRTK_BasicTeleport>());
            enableAtEndOfFrameRoutine = StartCoroutine(EnableAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            if (enableAtEndOfFrameRoutine != null)
            {
                StopCoroutine(enableAtEndOfFrameRoutine);
            }

            if (teleporter == null)
            {
                return;
            }

            if (headsetCollision != null)
            {
                headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        protected virtual IEnumerator EnableAtEndOfFrame()
        {
            if (teleporter == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headsetCollision = (headsetCollision != null ? headsetCollision : FindObjectOfType<VRTK_HeadsetCollision>());
            if (headsetCollision == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_TeleportDisableOnHeadsetCollision", "VRTK_HeadsetCollision"));
            }
            else
            {
                headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        protected virtual void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(false);
        }

        protected virtual void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(true);
        }
    }
}