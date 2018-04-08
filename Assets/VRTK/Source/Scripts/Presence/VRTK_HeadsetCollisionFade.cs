// Headset Collision Fade|Presence|70030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Initiates a fade of the headset view when a headset collision event is detected.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the HMD has collided with valid geometry.
    ///  * `VRTK_HeadsetFade` - A Headset Fade script to alter the visible colour on the HMD view.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_HeadsetCollisionFade` script on any active scene GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetCollisionFade")]
    public class VRTK_HeadsetCollisionFade : MonoBehaviour
    {
        [Header("Collision Fade Settings")]

        [Tooltip("The amount of time to wait until a fade occurs.")]
        public float timeTillFade = 0f;
        [Tooltip("The fade blink speed on collision.")]
        public float blinkTransitionSpeed = 0.1f;
        [Tooltip("The colour to fade the headset to on collision.")]
        public Color fadeColor = Color.black;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision Fade.")]
        public VRTK_PolicyList targetListPolicy;

        [Header("Custom Settings")]

        [Tooltip("The VRTK Headset Collision script to use when determining headset collisions. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetCollision headsetCollision;
        [Tooltip("The VRTK Headset Fade script to use when fading the headset. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetFade headsetFade;

        protected virtual void OnEnable()
        {
            headsetFade = (headsetFade != null ? headsetFade : FindObjectOfType<VRTK_HeadsetFade>());
            headsetCollision = (headsetCollision != null ? headsetCollision : FindObjectOfType<VRTK_HeadsetCollision>());

            if (headsetFade == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_HeadsetCollisionFade", "VRTK_HeadsetFade", "the same or child"));
                return;
            }

            if (headsetCollision == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_HeadsetCollisionFade", "VRTK_HeadsetCollision", "the same or child"));
                return;
            }

            headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollisionDetect);
            headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
        }

        protected virtual void OnDisable()
        {
            if (headsetCollision != null)
            {
                headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(OnHeadsetCollisionDetect);
                headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
            }
        }

        protected virtual void OnHeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
        {
            if (ValidTarget(e.collider))
            {
                Invoke("StartFade", timeTillFade);
            }
        }

        protected virtual void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            if (ValidTarget(e.collider))
            {
                CancelInvoke("StartFade");
                headsetFade.Unfade(blinkTransitionSpeed);
            }
        }

        protected virtual void StartFade()
        {
            headsetFade.Fade(fadeColor, blinkTransitionSpeed);
        }

        protected virtual bool ValidTarget(Collider target)
        {
            return (target != null && !(VRTK_PolicyList.Check(target.gameObject, targetListPolicy)));
        }
    }
}