namespace VRTK.Examples.Archery
{
    using UnityEngine;

    public class BowAnimation : MonoBehaviour
    {
        public Animation animationTimeline;

        public void SetFrame(float frame)
        {
            animationTimeline["BowPullAnimation"].speed = 0;
            animationTimeline["BowPullAnimation"].time = frame;
            animationTimeline.Play("BowPullAnimation");
        }
    }
}