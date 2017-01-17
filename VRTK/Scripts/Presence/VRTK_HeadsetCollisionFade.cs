// Headset Collision Fade|Presence|70030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The purpose of the Headset Collision Fade is to detect when the user's VR headset collides with another game object and fades the screen to a solid colour.
    /// </summary>
    /// <remarks>
    /// This is to deal with a user putting their head into a game object and seeing the inside of the object clipping, which is an undesired effect. The reasoning behind this is if the user puts their head where it shouldn't be, then fading to a colour (e.g. black) will make the user realise they've done something wrong and they'll probably naturally step backwards.
    ///
    /// The Headset Collision Fade uses a composition of the Headset Collision and Headset Fade scripts to derive the desired behaviour.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    [RequireComponent(typeof(VRTK_HeadsetCollision)), RequireComponent(typeof(VRTK_HeadsetFade))]
    public class VRTK_HeadsetCollisionFade : MonoBehaviour
    {
        [Tooltip("The fade blink speed on collision.")]
        public float blinkTransitionSpeed = 0.1f;
        [Tooltip("The colour to fade the headset to on collision.")]
        public Color fadeColor = Color.black;

        private VRTK_HeadsetCollision headsetCollision;
        private VRTK_HeadsetFade headsetFade;

        private void OnEnable()
        {
            headsetFade = GetComponent<VRTK_HeadsetFade>();
            headsetCollision = GetComponent<VRTK_HeadsetCollision>();

            headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollisionDetect);
            headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
        }

        private void OnHeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
        {
            headsetFade.Fade(fadeColor, blinkTransitionSpeed);
        }

        private void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            headsetFade.Unfade(blinkTransitionSpeed);
        }

        private void OnDisable()
        {
            headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(OnHeadsetCollisionDetect);
            headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
        }
    }
}