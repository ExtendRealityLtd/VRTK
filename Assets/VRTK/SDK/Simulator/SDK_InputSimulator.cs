﻿namespace VRTK
{
	using UnityEngine;

	/// <summary>
    /// Use the mouse and keyboard to move around both player and hands and interacting with objects
	/// without the need of a hmd or VR controls.
    /// </summary>
    /// <remarks>
	///
    /// </remarks>
    /// <example>
    /// 
    /// </example>
	public class SDK_InputSimulator : MonoBehaviour
	{
		#region Public fields

		[Tooltip("Hide hands when disabling them.")]
		public bool hideHandsAtSwitch = false;
		[Tooltip("Reset hand position and rotation when enabling them.")]
		public bool resetHandsAtSwitch = true;
		[Header("Adjustments")]
		[Tooltip("Adjust hand movement speed.")]
		public float handMoveMultiplier = 0.002f;
		[Tooltip("Adjust hand rotation speed.")]
		public float handRotationMultiplier = 0.5f;
		[Tooltip("Adjust player movement speed.")]
		public float playerMoveMultiplier = 5;
		[Tooltip("Adjust player rotation speed.")]
		public float playerRotationMultiplier = 0.5f;
		[Header("Key bindings")]
		[Tooltip("Key used to switch between left and righ hand.")]
		public KeyCode changeHands = KeyCode.Tab;
		[Tooltip("Key used to switch hands On/Off.")]
		public KeyCode handsOnOff = KeyCode.LeftAlt;
		[Tooltip("Key used to switch between positional and rotational movement.")]
		public KeyCode rotationPosition = KeyCode.LeftShift;
		[Tooltip("Key used to switch between X/Y and X/Z axis.")]
		public KeyCode changeAxis = KeyCode.LeftControl;

		#endregion
		#region Private fields

		private bool isHand = false;
		private Transform rightHand;
		private Transform leftHand;
		private Transform currentHand;
		private Vector3 oldPos;
		private Transform myCamera;
		private SDK_ControllerSim rightController;
		private SDK_ControllerSim leftController;

		#endregion

		private void Awake ()
		{
			rightHand = transform.FindChild("RightHand");
			rightHand.gameObject.SetActive(false);
			leftHand = transform.FindChild("LeftHand");
			leftHand.gameObject.SetActive(false);
			currentHand = rightHand;
			oldPos = Input.mousePosition;
			myCamera = transform.FindChild("Camera");
			leftHand.FindChild("Hand").GetComponent<Renderer>().material.color = Color.red;
			rightHand.FindChild("Hand").GetComponent<Renderer>().material.color = Color.green;
			rightController = rightHand.GetComponent<SDK_ControllerSim>();
            leftController = leftHand.GetComponent<SDK_ControllerSim>();
			rightController.Selected = true;
			leftController.Selected = false;
		}
		
		private void Update ()
		{
			if(Input.GetKeyDown(handsOnOff))
			{
				if(isHand)
					SetMove();
				else
					SetHand();
			}

			if(Input.GetKeyDown(changeHands))
			{
				if(currentHand.name == "LeftHand")
				{
					currentHand = rightHand;
					rightController.Selected = true;
					leftController.Selected = false;
				}
				else
				{
					currentHand = leftHand;
					rightController.Selected = false;
					leftController.Selected = true;
				}
			}

			if(isHand)
				UpdateHands();
			else
				UpdateRotation();

			UpdatePosition();
		}

		private void UpdateHands()
		{
			Vector3 mouseDiff = Input.mousePosition - oldPos;

			if(Input.GetKey(rotationPosition)) //Rotation
			{
				if(Input.GetKey(changeAxis))
				{
					Vector3 rot = Vector3.zero;
					rot.x += (mouseDiff * handRotationMultiplier).y;
					rot.y += (mouseDiff * handRotationMultiplier).x;
					currentHand.transform.FindChild("Hand").Rotate(rot * Time.deltaTime);
				}
				else
				{
					Vector3 rot = Vector3.zero;
					rot.z += (mouseDiff * handRotationMultiplier).x;
					rot.x += (mouseDiff * handRotationMultiplier).y;
					currentHand.transform.FindChild("Hand").Rotate(rot * Time.deltaTime);
				}
			}
			else //Position
			{
				if(Input.GetKey(changeAxis))
				{
					Vector3 pos = Vector3.zero;
					pos += mouseDiff * handMoveMultiplier;
					currentHand.transform.Translate(pos * Time.deltaTime);
				}
				else
				{
					Vector3 pos = Vector3.zero;
					pos.x += (mouseDiff * handMoveMultiplier).x;
					pos.z += (mouseDiff * handMoveMultiplier).y;
					currentHand.transform.Translate(pos * Time.deltaTime);
				}
			}
			oldPos = Input.mousePosition;
		}

		private void UpdateRotation()
		{
			Vector3 mouseDiff = Input.mousePosition - oldPos;

			Vector3 rot = transform.rotation.eulerAngles;
			rot.y += (mouseDiff * playerRotationMultiplier).x;
			transform.localRotation = Quaternion.Euler(rot);

			rot = myCamera.localRotation.eulerAngles;

			if(rot.x > 180)
				rot.x -= 360;
			
			if(rot.x < 80 && rot.x > -80)
			{
				rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
				rot.x = Mathf.Clamp (rot.x, -79, 79);
				myCamera.localRotation = Quaternion.Euler(rot);
			}

			oldPos = Input.mousePosition;
		}

		private void UpdatePosition()
		{
			if(Input.GetKey(KeyCode.W))
			{
				transform.Translate(Vector3.forward * Time.deltaTime * playerMoveMultiplier);
			}
			else if(Input.GetKey(KeyCode.S))
			{
				transform.Translate(Vector3.back * Time.deltaTime * playerMoveMultiplier);
			}
			if(Input.GetKey(KeyCode.A))
			{
				transform.Translate(Vector3.left * Time.deltaTime * playerMoveMultiplier);
			}
			else if(Input.GetKey(KeyCode.D))
			{
				transform.Translate(Vector3.right * Time.deltaTime * playerMoveMultiplier);
			}
		}

		private void SetHand()
		{
			Cursor.visible = false;
			isHand = true;
			rightHand.gameObject.SetActive(true);
			leftHand.gameObject.SetActive(true);
			oldPos = Input.mousePosition;
			if(resetHandsAtSwitch)
			{
				rightHand.transform.localPosition = new Vector3(0.2f, 1.2f, 0.5f);
				rightHand.transform.localRotation = Quaternion.identity;
				leftHand.transform.localPosition = new Vector3(-0.2f, 1.2f, 0.5f);
				leftHand.transform.localRotation = Quaternion.identity;
			}
		}

		private void SetMove()
		{
			Cursor.visible = true;
			isHand = false;
			if(hideHandsAtSwitch)
			{
				rightHand.gameObject.SetActive(false);
				leftHand.gameObject.SetActive(false);
			}
		}
	}
}
