namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class VRTK_ControllerActions : MonoBehaviour
    {
        private bool controllerVisible = true;
        private ushort hapticPulseStrength;

        private uint controllerIndex;
        private SteamVR_TrackedObject trackedController;
        private SteamVR_Controller.Device device;
        private ushort maxHapticVibration = 3999;

        private Dictionary<GameObject, Material> storedMaterials;

        public bool IsControllerVisible()
        {
            return controllerVisible;
        }

        public void ToggleControllerModel(bool state, GameObject grabbedChildObject)
        {
            if (!enabled)
            {
                return;
            }

            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = state;
                }
            }

            foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = state;
                }
            }
            controllerVisible = state;
        }

        public void SetControllerOpacity(float alpha)
        {
            if (!enabled)
            {
                return;
            }

            alpha = Mathf.Clamp(alpha, 0f, 1f);
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (alpha < 1f)
                {
                    renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    renderer.material.SetInt("_ZWrite", 0);
                    renderer.material.DisableKeyword("_ALPHATEST_ON");
                    renderer.material.DisableKeyword("_ALPHABLEND_ON");
                    renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.material.renderQueue = 3000;
                }
                else
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

        public void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)
        {
            if (!enabled)
            {
                return;
            }

            var renderer = element.GetComponent<Renderer>();
            if (renderer && renderer.material)
            {
                if (!storedMaterials.ContainsKey(element))
                {
                    storedMaterials.Add(element, new Material(renderer.material));
                }
                renderer.material.SetTexture("_MainTex", new Texture());
                StartCoroutine(CycleColor(renderer.material, new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b), highlight ?? Color.white, fadeDuration));
            }
        }

        public void UnhighlightControllerElement(GameObject element)
        {
            if (!enabled)
            {
                return;
            }

            var renderer = element.GetComponent<Renderer>();
            if (renderer && renderer.material)
            {
                if (storedMaterials.ContainsKey(element))
                {
                    renderer.material = new Material(storedMaterials[element]);
                    storedMaterials.Remove(element);
                }
            }
        }

        public void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)
        {
            if (element)
            {
                if (state)
                {
                    HighlightControllerElement(element.gameObject, highlight ?? Color.white, duration);
                }
                else
                {
                    UnhighlightControllerElement(element.gameObject);
                }
            }
        }

        public void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleHighlightAlias(state, "Model/trigger", highlight, duration);
        }

        public void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleHighlightAlias(state, "Model/lgrip", highlight, duration);
            ToggleHighlightAlias(state, "Model/rgrip", highlight, duration);
        }

        public void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleHighlightAlias(state, "Model/trackpad", highlight, duration);
        }

        public void ToggleHighlightApplicationMenu(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleHighlightAlias(state, "Model/button", highlight, duration);
        }

        public void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleHighlightTrigger(state, highlight, duration);
            ToggleHighlightGrip(state, highlight, duration);
            ToggleHighlightTouchpad(state, highlight, duration);
            ToggleHighlightApplicationMenu(state, highlight, duration);
            ToggleHighlightAlias(state, "Model/sys_button", highlight, duration);
            ToggleHighlightAlias(state, "Model/body", highlight, duration);
        }

        public void TriggerHapticPulse(ushort strength)
        {
            if (!enabled)
            {
                return;
            }

            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            device.TriggerHapticPulse(hapticPulseStrength);
        }

        public void TriggerHapticPulse(ushort strength, float duration, float pulseInterval)
        {
            if (!enabled)
            {
                return;
            }

            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            StartCoroutine(HapticPulse(duration, hapticPulseStrength, pulseInterval));
        }

        private void Awake()
        {
            trackedController = GetComponent<SteamVR_TrackedObject>();
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            storedMaterials = new Dictionary<GameObject, Material>();
        }

        private void Update()
        {
            controllerIndex = (uint)trackedController.index;
            device = SteamVR_Controller.Input((int)controllerIndex);
        }

        private IEnumerator HapticPulse(float duration, int hapticPulseStrength, float pulseInterval)
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

        private IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                material.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
                yield return null;
            }
        }

        private void ToggleHighlightAlias(bool state, string transformPath, Color? highlight, float duration = 0f)
        {
            var element = transform.Find(transformPath);
            if (element)
            {
                ToggleHighlightControllerElement(state, element.gameObject, highlight, duration);
            }
        }
    }
}