// Tunnel Overlay|Locomotion|20140
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Applys a tunnel overlay effect to the active VR camera when the play area is moving or rotating to reduce potential nausea caused by simulation sickness.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_TunnelOverlay` script on any active scene GameObject.
    ///
    ///   > This implementation is based on a project made by SixWays at https://github.com/SixWays/UnityVrTunnelling
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_TunnelOverlay")]
    public class VRTK_TunnelOverlay : MonoBehaviour
    {
        [Header("Movement Settings")]

        [Tooltip("Minimum rotation speed for the effect to activate (degrees per second).")]
        public float minimumRotation = 0f;
        [Tooltip("Maximum rotation speed for the effect have its max settings applied (degrees per second).")]
        public float maximumRotation = 45f;
        [Tooltip("Minimum movement speed for the effect to activate.")]
        public float minimumSpeed = 0f;
        [Tooltip("Maximum movement speed where the effect will have its max settings applied.")]
        public float maximumSpeed = 1f;

        [Header("Effect Settings")]

        [Tooltip("The color to use for the tunnel effect.")]
        public Color effectColor = Color.black;
        [Tooltip("The initial amount of screen coverage the tunnel to consume without any movement.")]
        [Range(0f, 1f)]
        public float initialEffectSize = 0f;
        [Tooltip("Screen coverage at the maximum tracked values.")]
        [Range(0f, 1f)]
        public float maximumEffectSize = 0.65f;
        [Tooltip("Feather effect size around the cut-off as fraction of screen.")]
        [Range(0f, 0.5f)]
        public float featherSize = 0.1f;
        [Tooltip("Smooth out radius over time.")]
        public float smoothingTime = 0.15f;

        protected Transform headset;
        protected Transform playarea;
        protected VRTK_TunnelEffect cameraEffect;
        protected float angularVelocity;
        protected float angularVelocitySlew;
        protected Vector3 lastForward;
        protected Vector3 lastPosition;
        protected Material matCameraEffect;
        protected int shaderPropertyColor;
        protected int shaderPropertyAV;
        protected int shaderPropertyFeather;
        protected Color originalColor;
        protected float originalAngularVelocity;
        protected float originalFeatherSize;
        protected float maximumEffectCoverage = 1.15f;

        protected virtual void Awake()
        {
            matCameraEffect = Resources.Load<Material>("TunnelOverlay");
            shaderPropertyColor = Shader.PropertyToID("_Color");
            shaderPropertyAV = Shader.PropertyToID("_AngularVelocity");
            shaderPropertyFeather = Shader.PropertyToID("_FeatherSize");
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            headset = VRTK_DeviceFinder.HeadsetCamera();
            playarea = VRTK_DeviceFinder.PlayAreaTransform();
            cameraEffect = headset.GetComponent<VRTK_TunnelEffect>();
            originalAngularVelocity = matCameraEffect.GetFloat(shaderPropertyAV);
            originalFeatherSize = matCameraEffect.GetFloat(shaderPropertyFeather);
            originalColor = matCameraEffect.GetColor(shaderPropertyColor);

            if (cameraEffect == null)
            {
                cameraEffect = headset.gameObject.AddComponent<VRTK_TunnelEffect>();
                cameraEffect.SetMaterial(matCameraEffect);
            }
        }

        protected virtual void OnDisable()
        {
            headset = null;
            playarea = null;

            if (cameraEffect != null)
            {
                SetShaderFeather(originalColor, originalAngularVelocity, originalFeatherSize);
                matCameraEffect.SetColor(shaderPropertyColor, originalColor);
                Destroy(cameraEffect);
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void FixedUpdate()
        {
            Vector3 fwd = playarea.forward;
            Vector3 pos = playarea.position;

            float newAngularVelocity = Vector3.Angle(lastForward, fwd) / Time.fixedDeltaTime;
            newAngularVelocity = (newAngularVelocity - minimumRotation) / (maximumRotation - minimumRotation);

            if (maximumSpeed > 0)
            {
                float speed = (pos - lastPosition).magnitude / Time.fixedDeltaTime;
                speed = (speed - minimumSpeed) / (maximumSpeed - minimumSpeed);

                if (speed > newAngularVelocity)
                {
                    newAngularVelocity = speed;
                }
            }

            float actualInitialSize = (initialEffectSize * maximumEffectCoverage);
            float actualMaxSize = (maximumEffectSize * maximumEffectCoverage) - actualInitialSize;

            newAngularVelocity = Mathf.Clamp01(newAngularVelocity) * actualMaxSize;
            angularVelocity = Mathf.SmoothDamp(angularVelocity, newAngularVelocity, ref angularVelocitySlew, smoothingTime);

            SetShaderFeather(effectColor, angularVelocity + actualInitialSize, featherSize);

            lastForward = fwd;
            lastPosition = pos;
        }

        protected virtual void SetShaderFeather(Color givenTunnelColor, float givenAngularVelocity, float givenFeatherSize)
        {
            matCameraEffect.SetColor(shaderPropertyColor, givenTunnelColor);
            matCameraEffect.SetFloat(shaderPropertyAV, givenAngularVelocity);
            matCameraEffect.SetFloat(shaderPropertyFeather, givenFeatherSize);
        }
    }
}