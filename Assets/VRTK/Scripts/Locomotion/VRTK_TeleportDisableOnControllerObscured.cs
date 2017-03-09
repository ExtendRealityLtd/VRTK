// Teleport Disable On Controller Obscured|Locomotion|20050
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The purpose of the Teleport Disable On Controller Obscured script is to detect when the headset does not have a line of sight to the controllers and prevent teleportation from working. This is to ensure that if a user is clipping their controllers through a wall then they cannot teleport to an area beyond the wall.
    /// </summary>
    [RequireComponent(typeof(VRTK_HeadsetControllerAware))]
    public class VRTK_TeleportDisableOnControllerObscured : MonoBehaviour
    {
        protected VRTK_BasicTeleport basicTeleport;
        protected VRTK_HeadsetControllerAware headset;

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

            if (headset)
            {
                headset.ControllerObscured -= new HeadsetControllerAwareEventHandler(DisableTeleport);
                headset.ControllerUnobscured -= new HeadsetControllerAwareEventHandler(EnableTeleport);
            }
        }

        protected virtual IEnumerator EnableAtEndOfFrame()
        {
            if (basicTeleport == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headset = VRTK_ObjectCache.registeredHeadsetControllerAwareness;
            if (headset)
            {
                headset.ControllerObscured += new HeadsetControllerAwareEventHandler(DisableTeleport);
                headset.ControllerUnobscured += new HeadsetControllerAwareEventHandler(EnableTeleport);
            }
        }

        protected virtual void DisableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(false);
        }

        protected virtual void EnableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            basicTeleport.ToggleTeleportEnabled(true);
        }
    }
}