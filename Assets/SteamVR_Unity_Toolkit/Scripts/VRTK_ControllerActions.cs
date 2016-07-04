namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ControllerActions : MonoBehaviour
    {
        private bool controllerVisible = true;
        private ushort hapticPulseStrength;

        private uint controllerIndex;
        private SteamVR_TrackedObject trackedController;
        private SteamVR_Controller.Device device;
        private ushort maxHapticVibration = 3999;

        public bool IsControllerVisible()
        {
            return controllerVisible;
        }

        public void ToggleControllerModel(bool on, GameObject grabbedChildObject)
        {
            foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = on;
                }
            }

            foreach (SkinnedMeshRenderer renderer in this.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = on;
                }
            }
            controllerVisible = on;
        }

        public void TriggerHapticPulse(ushort strength)
        {
            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            device.TriggerHapticPulse(hapticPulseStrength);
        }

        public void TriggerHapticPulse(ushort strength, float duration, float pulseInterval)
        {
            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            StartCoroutine(Pulse(duration, hapticPulseStrength, pulseInterval));
        }

        private void Awake()
        {
            trackedController = GetComponent<SteamVR_TrackedObject>();
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Update()
        {
            controllerIndex = (uint)trackedController.index;
            device = SteamVR_Controller.Input((int)controllerIndex);
        }

        private IEnumerator Pulse(float duration, int hapticPulseStrength, float pulseInterval)
        {
            if (pulseInterval <= 0)
            {
                yield break;
            }

            while (duration > 0)
            {
                device.TriggerHapticPulse((ushort)hapticPulseStrength);
                yield return new WaitForSeconds(pulseInterval);
                duration -= pulseInterval;
            }
        }
    }
}