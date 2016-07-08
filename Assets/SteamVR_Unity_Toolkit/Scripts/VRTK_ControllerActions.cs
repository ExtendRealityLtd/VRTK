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

        public void SetControllerOpacity(float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            foreach (var renderer in this.gameObject.GetComponentsInChildren<Renderer>())
            {
                if(alpha < 1f)
                {
                    renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    renderer.material.SetInt("_ZWrite", 0);
                    renderer.material.DisableKeyword("_ALPHATEST_ON");
                    renderer.material.DisableKeyword("_ALPHABLEND_ON");
                    renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.material.renderQueue = 3000;
                } else
                {
                    renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    renderer.material.SetInt("_ZWrite", 1);
                    renderer.material.DisableKeyword("_ALPHATEST_ON");
                    renderer.material.DisableKeyword("_ALPHABLEND_ON");
                    renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.material.renderQueue = -1;
                }

                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
            }
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