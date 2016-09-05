namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
    {
        private VRTK_BasicTeleport basicTeleport;
        private VRTK_HeadsetCollision headset;

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

            headset = VRTK_ObjectCache.registeredHeadsetCollider;
            if (headset)
            {
                headset.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headset.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
            }
        }

        private void OnDisable()
        {
            if (basicTeleport == null)
            {
                return;
            }

            if (headset)
            {
                headset.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(DisableTeleport);
                headset.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(EnableTeleport);
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