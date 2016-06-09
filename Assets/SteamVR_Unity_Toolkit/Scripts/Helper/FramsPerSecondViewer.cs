//====================================================================================
//
// Purpose: Provide GUI view of the current frames per second in the game
//
// This script must be attached to a Text element within a Canvas that has the
// Render Mode set to "Screen Space - Camera" and the Render Camera set to the
// [CameraRig]-> Camera (head) -> Camera (eye) object with a plane distance of 0.1.
//
// This script is pretty much a copy and paste from the script at:
// http://talesfromtherift.com/vr-fps-counter/
//
// So all credit to Peter Koch for his work. Twitter: @peterept
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FramsPerSecondViewer : MonoBehaviour
    {
        public bool displayFPS = true;
        public int targetFPS = 90;
        public int fontSize = 32;
        public Vector3 position = Vector3.zero;
        public Color goodColor = Color.green;
        public Color warnColor = Color.yellow;
        public Color badColor = Color.red;

        private const float updateInterval = 0.5f;
        private int framesCount;
        private float framesTime;
        private Text text;

        void Start()
        {
            text = this.GetComponent<Text>();
            text.fontSize = fontSize;
            text.transform.localPosition = position;
        }

        void Update()
        {
            framesCount++;
            framesTime += Time.unscaledDeltaTime;

            if (framesTime > updateInterval)
            {
                if (text != null)
                {
                    if (displayFPS)
                    {
                        float fps = framesCount / framesTime;
                        text.text = System.String.Format("{0:F2} FPS", fps);
                        text.color = (fps > (targetFPS - 5) ? goodColor :
                                     (fps > (targetFPS - 30) ? warnColor :
                                      badColor));
                    }
                    else
                    {
                        text.text = "";
                    }
                }
                framesCount = 0;
                framesTime = 0;
            }
        }
    }
}