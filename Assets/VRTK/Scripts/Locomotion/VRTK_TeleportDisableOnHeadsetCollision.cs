// Teleport Disable On Headset Collision|Locomotion|20040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The purpose of the Teleport Disable On Headset Collision script is to detect when the headset is colliding with a valid object and prevent teleportation from working. This is to ensure that if a user is clipping their head into a wall then they cannot teleport to an area beyond the wall.
    /// </summary>
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

            if (headsetCollision)
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
            if (headsetCollision)
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