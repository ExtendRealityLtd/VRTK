using UnityEngine;
using System.Collections;

public class VRTK_RoomExtender : MonoBehaviour
{

    public float movementMultiplicator = 1.0F;
    public float headZoneRadius = 0.25F;
    public Transform debugTransform;
    public Transform movementTransform;

    protected Transform cameraRig;
    protected Vector3 headCirclePosition;

    void Start()
    {
       if(movementTransform == null)
        {
#if UNITY_5_4_OR_NEWER
            if (GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>() == null)
            {
                Debug.LogWarning("This 'VRTK_RoomExtender' script needs a movementTransform to work. The default 'SteamVR_Camera' was not found.");
            }else
            {
                movementTransform = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
            }
#else
            Debug.LogWarning("This 'VRTK_RoomExtender' script needs a movementTransform to work.);
#endif
        }
        cameraRig = GameObject.FindObjectOfType<SteamVR_PlayArea>().gameObject.transform;
        headCirclePosition = new Vector3(movementTransform.localPosition.x, 0, movementTransform.localPosition.z);
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
        var movement = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
        if (movement.magnitude > headZoneRadius)
        {
            var deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
            headCirclePosition += deltaMovement;
            cameraRig.localPosition += deltaMovement * movementMultiplicator;
        }
    }
}
