using UnityEngine;
using System.Collections;

public class VRTK_RoomExtender : MonoBehaviour
{
	public float additionalMovementMultiplier = 1.0f;
	public float headZoneRadius = 0.25f;
	public Transform debugTransform;
	protected Transform movementTransform;
	protected Transform cameraRig;
	protected Vector3 headCirclePosition;

	void Start ()
	{
		if (movementTransform == null) {
			if (VRTK.DeviceFinder.HeadsetTransform () != null) {
				movementTransform = VRTK.DeviceFinder.HeadsetTransform ();
			} else {
				Debug.LogWarning ("This 'VRTK_RoomExtender' script needs a movementTransform to work.The default 'SteamVR_Camera' or 'SteamVR_GameView' was not found.");
			}
		}
		cameraRig = GameObject.FindObjectOfType<SteamVR_PlayArea> ().gameObject.transform;
		headCirclePosition = new Vector3 (movementTransform.localPosition.x, 0, movementTransform.localPosition.z);
		if (debugTransform) {
			debugTransform.localScale = new Vector3 (headZoneRadius * 2, 0.01f, headZoneRadius * 2);
		}
	}

	void Update ()
	{
		moveHeadCircle ();
		if (debugTransform) {
			debugTransform.localPosition = headCirclePosition;
		}
	}

	void moveHeadCircle ()
	{
		var movement = new Vector3 (movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
		if (movement.magnitude > headZoneRadius) {
			var deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
			headCirclePosition += deltaMovement;
			cameraRig.localPosition += deltaMovement * additionalMovementMultiplier;
		}
	}
}
