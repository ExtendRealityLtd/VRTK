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
        [Tooltip("An optional skybox texture to use for the tunnel effect.")]
        public Texture effectSkybox;
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
        protected Camera headsetCamera;
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
        protected int shaderPropertySkyboxTexture;
        protected Color originalColor;
        protected float originalAngularVelocity;
        protected float originalFeatherSize;
        protected Texture originalSkyboxTexture;
        protected float maximumEffectCoverage = 1.15f;
        protected bool createEffectSkybox = false;

        protected virtual void Awake()
        {
            matCameraEffect = Resources.Load<Material>("TunnelOverlay");
            shaderPropertyColor = Shader.PropertyToID("_Color");
            shaderPropertyAV = Shader.PropertyToID("_AngularVelocity");
            shaderPropertyFeather = Shader.PropertyToID("_FeatherSize");
            shaderPropertySkyboxTexture = Shader.PropertyToID("_SecondarySkyBox");
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            headset = VRTK_DeviceFinder.HeadsetCamera();
            if (headset != null)
            {
                headsetCamera = headset.GetComponent<Camera>();
                cameraEffect = headset.GetComponent<VRTK_TunnelEffect>();
            }
            playarea = VRTK_DeviceFinder.PlayAreaTransform();
            originalAngularVelocity = matCameraEffect.GetFloat(shaderPropertyAV);
            originalFeatherSize = matCameraEffect.GetFloat(shaderPropertyFeather);
            originalColor = matCameraEffect.GetColor(shaderPropertyColor);
            CheckSkyboxTexture();
            if (effectSkybox != null)
            {
                originalSkyboxTexture = matCameraEffect.GetTexture(shaderPropertySkyboxTexture);
                matCameraEffect.SetTexture("_SecondarySkyBox", effectSkybox);
            }

            if (cameraEffect == null && headset != null)
            {
                cameraEffect = headset.gameObject.AddComponent<VRTK_TunnelEffect>();
                cameraEffect.SetMaterial(matCameraEffect);
            }
        }

        protected virtual void OnDisable()
        {
            headset = null;
            headsetCamera = null;
            playarea = null;

            if (cameraEffect != null)
            {
                matCameraEffect.SetTexture("_SecondarySkyBox", originalSkyboxTexture);
                originalSkyboxTexture = null;

                SetShaderFeather(originalColor, originalAngularVelocity, originalFeatherSize);
                matCameraEffect.SetColor(shaderPropertyColor, originalColor);
                Destroy(cameraEffect);
            }

            if (createEffectSkybox)
            {
                effectSkybox = null;
                createEffectSkybox = false;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
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

            if (effectSkybox != null)
            {
                matCameraEffect.SetMatrixArray("_EyeToWorld", new Matrix4x4[2]
                {
                headsetCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse,
                headsetCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse
                });

                Matrix4x4[] eyeProjection = new Matrix4x4[2];
                eyeProjection[0] = headsetCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                eyeProjection[1] = headsetCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                eyeProjection[0] = GL.GetGPUProjectionMatrix(eyeProjection[0], true).inverse;
                eyeProjection[1] = GL.GetGPUProjectionMatrix(eyeProjection[1], true).inverse;
                eyeProjection[0][1, 1] *= -1f;
                eyeProjection[1][1, 1] *= -1f;

                matCameraEffect.SetMatrixArray("_EyeProjection", eyeProjection);
            }
        }

        protected virtual void SetShaderFeather(Color givenTunnelColor, float givenAngularVelocity, float givenFeatherSize)
        {
            matCameraEffect.SetColor(shaderPropertyColor, givenTunnelColor);
            matCameraEffect.SetFloat(shaderPropertyAV, givenAngularVelocity);
            matCameraEffect.SetFloat(shaderPropertyFeather, givenFeatherSize);
        }

        protected virtual void CheckSkyboxTexture()
        {
            if (effectSkybox == null)
            {
                Cubemap tempTexture = new Cubemap(1, TextureFormat.ARGB32, false);
                tempTexture.SetPixel(CubemapFace.NegativeX, 0, 0, Color.white);
                tempTexture.SetPixel(CubemapFace.NegativeY, 0, 0, Color.white);
                tempTexture.SetPixel(CubemapFace.NegativeZ, 0, 0, Color.white);
                tempTexture.SetPixel(CubemapFace.PositiveX, 0, 0, Color.white);
                tempTexture.SetPixel(CubemapFace.PositiveY, 0, 0, Color.white);
                tempTexture.SetPixel(CubemapFace.PositiveZ, 0, 0, Color.white);
                effectSkybox = tempTexture;
                createEffectSkybox = true;
            }
            else if (effectColor.r < 0.15f && effectColor.g < 0.15 && effectColor.b < 0.15)
            {
                VRTK_Logger.Warn("`VRTK_TunnelOverlay` has an `Effect Skybox` texture but the `Effect Color` is too dark which will tint the texture so it is not visible.");
            }
        }
    }
}