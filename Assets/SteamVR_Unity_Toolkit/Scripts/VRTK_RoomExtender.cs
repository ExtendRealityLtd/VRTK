using UnityEngine;
using System.Collections;

public class VRTK_RoomExtender : MonoBehaviour
{

    public float movementMultiplicator = 1.0F;
    public float headZoneRadius = 0.25F;
    public Transform debugTransform;

    protected Transform eyeCamera;
    protected Transform cameraRig;
    protected Vector3 headCirclePosition;

    void Start()
    {
        eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
        cameraRig = GameObject.FindObjectOfType<SteamVR_PlayArea>().gameObject.transform;
        headCirclePosition = eyeCamera.localPosition;
        if (debugTransform)
        {
            debugTransform.localScale = new Vector3(headZoneRadius * 2, 0.01F, headZoneRadius * 2);
        }
    }
    void Update()
    {
        moveHeadCircle();
        if (debugTransform)
        {
            debugTransform.localPosition = headCirclePosition;
        }
    }

    void moveHeadCircle()
    {
        var movement = new Vector3(eyeCamera.localPosition.x - headCirclePosition.x, 0, eyeCamera.localPosition.z - headCirclePosition.z);
        if (movement.magnitude > headZoneRadius)
        {
            var deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
            headCirclePosition += deltaMovement;
            cameraRig.localPosition += deltaMovement * movementMultiplicator;
        }
    }
}
