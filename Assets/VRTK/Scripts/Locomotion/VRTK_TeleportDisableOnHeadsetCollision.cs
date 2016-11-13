// Teleport Disable On Headset Collision|Scripts|0120
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The purpose of the Teleport Disable On Headset Collision script is to detect when the headset is colliding with a valid object and prevent teleportation from working. This is to ensure that if a user is clipping their head into a wall then they cannot teleport to an area beyond the wall.
    /// </summary>
    public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
    {
        private VRTK_BasicTeleport basicTeleport;
        private VRTK_HeadsetCollision headsetCollision;

        private void OnEnable()
        {
            basicTeleport = GetComponent<VRTK_BasicTeleport>();
            StartCoroutine(EnableAtEndOfFrame());
        }

        private IEnumerator EnableAtEndOfFrame()
        {
            if (basicTeleport == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headsetCollision = VRTK_ObjectCache.registeredHeadsetCollider;
            if (headsetCollision)
            {
                headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        private void OnDisable()
        {
            if (basicTeleport == null)
            {
                return;
            }

            if (headsetCollision)
            {
                headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(DisableTeleport);
                headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        private void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(false);
        }

        private void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(true);
        }
    }
}