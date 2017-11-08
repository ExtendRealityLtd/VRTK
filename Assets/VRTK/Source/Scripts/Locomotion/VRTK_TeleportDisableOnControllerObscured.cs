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
    ///  * Place the `VRTK_TeleportDisableOnControllerObscured` script on any active scene GameObject.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TeleportDisableOnControllerObscured")]
    public class VRTK_TeleportDisableOnControllerObscured : MonoBehaviour
    {
        [Header("Custom Settings")]

        [Tooltip("The Teleporter script to deal play area teleporting. If the script is being applied onto an object that already has a VRTK_BasicTeleport component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        public VRTK_BasicTeleport teleporter;
        [Tooltip("The VRTK Headset Controller Aware script to use when dealing with the headset to controller awareness. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetControllerAware headsetControllerAware;

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

            if (headsetControllerAware != null)
            {
                headsetControllerAware.ControllerObscured -= new HeadsetControllerAwareEventHandler(DisableTeleport);
                headsetControllerAware.ControllerUnobscured -= new HeadsetControllerAwareEventHandler(EnableTeleport);
            }
        }

        protected virtual IEnumerator EnableAtEndOfFrame()
        {
            if (teleporter == null)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();

            headsetControllerAware = (headsetControllerAware != null ? headsetControllerAware : FindObjectOfType<VRTK_HeadsetControllerAware>());
            if (headsetControllerAware == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_TeleportDisableOnControllerObscured", "VRTK_HeadsetControllerAware"));
            }
            else
            {
                headsetControllerAware.ControllerObscured += new HeadsetControllerAwareEventHandler(DisableTeleport);
                headsetControllerAware.ControllerUnobscured += new HeadsetControllerAwareEventHandler(EnableTeleport);
            }
        }

        protected virtual void DisableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(false);
        }

        protected virtual void EnableTeleport(object sender, HeadsetControllerAwareEventArgs e)
        {
            teleporter.ToggleTeleportEnabled(true);
        }
    }
}