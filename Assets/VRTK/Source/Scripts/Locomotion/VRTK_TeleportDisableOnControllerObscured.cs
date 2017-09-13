// Teleport Disable On Controller Obscured|Locomotion|20050
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Prevents teleportation when the controllers are obscured from line of sight of the HMD.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_BasicTeleport` - A Teleport script to utilise for teleporting the play area.
    ///  * `VRTK_HeadsetControllerAware` - A Headset Controller Aware script to determine when the HMD has line of sight to the controllers.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_TeleportDisableOnControllerObscured` script on the same GameObject as the active Teleport script.
    /// </remarks>
    [RequireComponent(typeof(VRTK_HeadsetControllerAware))]
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TeleportDisableOnControllerObscured")]
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

            if (headset != null)
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
            if (headset != null)
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