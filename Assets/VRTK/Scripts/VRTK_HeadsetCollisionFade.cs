namespace VRTK
{
    using UnityEngine;

    public class VRTK_HeadsetCollisionFade : MonoBehaviour
    {
        public float blinkTransitionSpeed = 0.1f;
        public Color fadeColor = Color.black;
        public string ignoreTargetWithTagOrClass;

        private VRTK_HeadsetCollision headsetCollision;
        private VRTK_HeadsetFade headsetFade;

        private void OnEnable()
        {
            headsetCollision = gameObject.AddComponent<VRTK_HeadsetCollision>();
            headsetCollision.ignoreTargetWithTagOrClass = ignoreTargetWithTagOrClass;

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