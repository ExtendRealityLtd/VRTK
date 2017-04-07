using UnityEngine;
using System.Collections;

public class SpyCamViewport : MonoBehaviour
{

    public UnityEngine.UI.RawImage viewportWindow;

    public void ActivateViewport(Camera camera)
    {
        this.gameObject.SetActive(true);
        viewportWindow.texture = camera.targetTexture;
    }

    public void DeactivateViewport()
    {
        this.gameObject.SetActive(false);
        viewportWindow.texture = null;
    }

}
