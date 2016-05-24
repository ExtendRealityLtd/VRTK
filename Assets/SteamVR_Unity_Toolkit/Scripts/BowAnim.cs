using UnityEngine;
using System.Collections;

public class BowAnim : MonoBehaviour {

    public Animation Anim;

	public void SetAnim(float val)
    {
        Anim["Main"].speed = 0;
        Anim["Main"].time = val;
        Anim.Play("Main");
    }
}
