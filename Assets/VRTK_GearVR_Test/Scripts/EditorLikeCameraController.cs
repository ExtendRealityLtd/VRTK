using UnityEngine;

public class EditorLikeCameraController : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Notice: " + GetType().Name + " is active only in Unity Editor.");
    }
#if (UNITY_EDITOR)
    private void Update()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        if (mouseX<0 || mouseY<0 || mouseX>Screen.width || mouseY>Screen.height)
        {
            return;
        }
        float mouseDeltaX = Input.GetAxis("Mouse X");
        float mouseDeltaY = Input.GetAxis("Mouse Y");
        float x = Input.GetAxis("Horizontal") * 0.1f;
        float y = 0.0f;
        float z = Input.GetAxis("Vertical") * 0.1f;

        if (Input.GetMouseButton(1))
        {
            float rx = mouseDeltaY * 2.0f;
            float ry = mouseDeltaX * 2.0f;

            Vector3 rot = transform.eulerAngles;
            rot.x -= rx;
            rot.y += ry;
            transform.eulerAngles = rot;
        }
        if (Input.GetMouseButton(2))
        {
            x -= mouseDeltaX * 0.1f;
            y -= mouseDeltaY * 0.1f;
        }
        z += Input.GetAxis("Mouse ScrollWheel");

        transform.Translate(x, y, z);
    }
#endif
}
