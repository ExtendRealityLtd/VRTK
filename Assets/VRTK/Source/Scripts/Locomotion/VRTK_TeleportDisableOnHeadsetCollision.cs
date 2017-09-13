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
    ///  * Place the `VRTK_TeleportDisableOnHeadsetCollision` script on the same GameObject as the active Teleport script.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TeleportDisableOnHeadsetCollision")]
    public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
    {
        protected VRTK_BasicTeleport basicTeleport;
        protected VRTK_HeadsetCollision headsetCollision;

        protected virtual void OnEnable()
        {
            basicTeleport = GetComponent<VRTK_BasicTeleport>();
            StartCoroutine(EnableAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            if (basicTeleport == null)
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
            if (basicTeleport == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headsetCollision = VRTK_ObjectCache.registeredHeadsetCollider;
            if (headsetCollision != null)
            {
                headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        protected virtual void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(false);
        }

        protected virtual void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(true);
        }
    }
}