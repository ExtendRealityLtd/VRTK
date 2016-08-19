using System;
using UnityEngine;

public class BowAnimation : MonoBehaviour, IBowAnimation
{
    public Animation animationTimeline;
    public string animStateName = "BowPullAnimation";

    AnimationState animState;

    void Start() {
        animState = animationTimeline[animStateName];
    }

    public void SetDraw(float frame)
    {
        animState.speed = 0;
        animState.time = frame;
        animationTimeline.Play(animStateName);
    }
}