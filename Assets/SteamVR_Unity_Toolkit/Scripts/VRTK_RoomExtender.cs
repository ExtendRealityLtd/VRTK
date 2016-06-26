using UnityEngine;
using System.Collections;

public class VRTK_RoomExtender : MonoBehaviour
{
	public enum MovementFunction
	{
		Nonlinear,
		LinearDirect
	}

	public MovementFunction movementFunction = MovementFunction.LinearDirect;
	public bool additionalMovementEnabled = true;
	public bool additionalMovementEnabledOnButtonPress = true;
	[Range(0, 10)]
	public float additionalMovementMultiplier = 1.0f;
	[Range(0, 5)]
	public float headZoneRadius = 0.25f;
	public Transform debugTransform;

	[HideInInspector]
	public Vector3 relativeMovementOfCameraRig = new Vector3();

	protected Transform movementTransform;
	protected Transform cameraRig;
	protected Vector3 headCirclePosition;
	protected Vector3 lastPosition;
	protected Vector3 lastMovement;

	void Start()
	{
		if (movementTransform == null)
		{
			if (VRTK.DeviceFinder.HeadsetTransform() != null)
			{
				movementTransform = VRTK.DeviceFinder.HeadsetTransform();
			}
			else
			{
				Debug.LogWarning("This 'VRTK_RoomExtender' script needs a movementTransform to work.The default 'SteamVR_Camera' or 'SteamVR_GameView' was not found.");
			}
		}
		cameraRig = GameObject.FindObjectOfType<SteamVR_PlayArea>().gameObject.transform;
		//headCirclePosition = new Vector3(movementTransform.localPosition.x, 0, movementTransform.localPosition.z);
		additionalMovementEnabled = !additionalMovementEnabledOnButtonPress;
		if (debugTransform)
		{
			debugTransform.localScale = new Vector3(headZoneRadius * 2, 0.01f, headZoneRadius * 2);
			//debugTransform.localPosition = headCirclePosition;
		}
		moveHeadCircleNonLinearDrift();
		lastPosition = movementTransform.localPosition;
	}

	void Update()
	{
		switch (movementFunction)
		{
			case MovementFunction.Nonlinear:
				moveHeadCircleNonLinearDrift();
				break;
			case MovementFunction.LinearDirect:
				moveHeadCircle();
				break;
			default:
				break;
		}
	}

	private void move(Vector3 movement)
	{
		headCirclePosition += movement;
		if (debugTransform) {
			debugTransform.localPosition = headCirclePosition;
		}
		if (additionalMovementEnabled) {
			cameraRig.localPosition += movement * additionalMovementMultiplier;
			relativeMovementOfCameraRig += movement * additionalMovementMultiplier;
		}
	}

	private void moveHeadCircle()
	{
		//Get the movement of the head relative to the headCircle.
		var circleCenterToHead = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);

		//Get the direction of the head movement.
		updateLastMovement();

		//Checks if the head is outside of the head cirlce and moves the head circle and play area in the movementDirection.
		if (circleCenterToHead.sqrMagnitude > headZoneRadius * headZoneRadius && lastMovement != Vector3.zero)
		{
			//Just move like the headset moved
			move(lastMovement);
		}
	}

	private void moveHeadCircleNonLinearDrift()
	{
		var movement = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
		if (movement.sqrMagnitude > headZoneRadius * headZoneRadius)
		{
			var deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
			move(deltaMovement);
		}
	}

	private void updateLastMovement()
	{
		//Save the last movement
		lastMovement = movementTransform.localPosition - lastPosition;
		lastMovement.y = 0;
		lastPosition = movementTransform.localPosition;
	}
}
