using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Mechanim based bow animator that utilizes parameter to animate the bow.  Allows use of blend trees etc.
/// </summary>
public class BowAnimationMecanim : MonoBehaviour, IBowAnimation {
    [SerializeField]
    Animator anim;
    [SerializeField]
    [Tooltip("The name of the parameter your animator is using to represent bow draw.")]
    string paramName = "Draw";

    int drawParamHash;

    void Start() {
        // If anim not set, find an animator on our gameobject or its children
        if (!anim) {
            anim = GetComponentInChildren<Animator>();
        }
        // Get hash of paramName to improve performance
        drawParamHash = Animator.StringToHash(paramName);
    }

    /// <summary>
    /// Set draw parameter in mechanim state machine.
    /// </summary>
    /// <param name="draw">Draw parameter to be passed to mechanim</param>
    public void SetDraw(float draw) {
        anim.SetFloat(drawParamHash, draw);
    }
}
