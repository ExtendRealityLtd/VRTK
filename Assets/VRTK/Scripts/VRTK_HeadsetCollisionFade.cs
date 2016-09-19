// Headset Collision Fade|Scripts|0110
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The purpose of the Headset Collision Fade is to detect when the user's VR headset collides with another game object and fades the screen to a solid colour. This is to deal with a user putting their head into a game object and seeing the inside of the object clipping, which is an undesired effect. The reasoning behind this is if the user puts their head where it shouldn't be, then fading to a colour (e.g. black) will make the user realise they've done something wrong and they'll probably naturally step backwards.
    /// </summary>
    /// <remarks>
    /// The Headset Collision Fade uses a composition of the Headset Collision and Headset Fade scripts to derive the desired behaviour.
    ///
    ///   > **Unity Version Information**
    ///   > * If using `Unity 5.3` or older then the Headset Collision Fade script is attached to the `Camera(head)` object within the `[CameraRig]` prefab.
    ///   > * If using `Unity 5.4` or newer then the Headset Collision Fade script is attached to the `Camera(eye)` object within the `[CameraRig]->Camera(head)` prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    public class VRTK_HeadsetCollisionFade : MonoBehaviour
    {
        [Tooltip("The fade blink speed on collision.")]
        public float blinkTransitionSpeed = 0.1f;
        [Tooltip("The colour to fade the headset to on collision.")]
        public Color fadeColor = Color.black;
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and will prevent the object from fading the headset view on collision.")]
        public string ignoreTargetWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether any objects will be acted upon by the Headset Collision Fade. If a list is provided then the 'Ignore Target With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        private VRTK_HeadsetCollision headsetCollision;
        private VRTK_HeadsetFade headsetFade;

        private void OnEnable()
        {
            headsetCollision = gameObject.AddComponent<VRTK_HeadsetCollision>();
            headsetCollision.ignoreTargetWithTagOrClass = ignoreTargetWithTagOrClass;
            headsetCollision.targetTagOrScriptListPolicy = targetTagOrScriptListPolicy;

            headsetFade = gameObject.AddComponent<VRTK_HeadsetFade>();

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

            Destroy(headsetCollision);
            Destroy(headsetFade);
        }
    }
}