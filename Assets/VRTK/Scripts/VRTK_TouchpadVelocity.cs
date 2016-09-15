namespace VRTK
{
	using UnityEngine;
	using System.Collections;

	[RequireComponent(typeof(VRTK_PlayerPresence_NoGrav))]
	public class VRTK_TouchpadVelocity : MonoBehaviour
	{

		[SerializeField]
		private bool leftController = true;
		public bool LeftController
		{
			get { return leftController; }
			set
			{
				leftController = value;
				SetControllerListeners(controllerManager.left);
			}
		}

		[SerializeField]
		private bool rightController = true;
		public bool RightController
		{
			get { return rightController; }
			set
			{
				rightController = value;
				SetControllerListeners(controllerManager.right);
			}
		}

		public float boostRegen = 33f;
		public float boostPower = 2.3f;
		public float boostUse = 33f;
		public float maxBoost = 99f; 
		public float regenTime = .5f;

		private SteamVR_ControllerManager controllerManager;
		private Vector2 touchAxis;
		private Vector2 triggerAxis;
		private bool controllerCheck;
		private float currentBoost = 99f;
		private float timeSinceUnderMax = 0;

		private bool leftSubscribed;
		private bool rightSubscribed;

		private ControllerInteractionEventHandler touchpadAxisChanged;
		private ControllerInteractionEventHandler touchpadUntouched;
		private ControllerInteractionEventHandler triggerPressed;
		private ControllerInteractionEventHandler rightTriggerPressed;
		private ControllerInteractionEventHandler leftTriggerPressed;
		private ControllerInteractionEventHandler triggerReleased;

		private VRTK_PlayerPresence_NoGrav playerPresence;

		private void Awake()
		{
			if (this.GetComponent<VRTK_PlayerPresence_NoGrav>())
			{
				playerPresence = this.GetComponent<VRTK_PlayerPresence_NoGrav>();
			}
			else
			{
				Debug.LogError("The VRTK_TouchpadWalking script requires the VRTK_PlayerPresence script to be attached to the [CameraRig]");
			}

			touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
			touchpadUntouched = new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
			triggerPressed = new ControllerInteractionEventHandler (DoTriggerPressed);
			controllerManager = this.GetComponent<SteamVR_ControllerManager>();
		}

		private void Start()
		{
			Utilities.SetPlayerObject(this.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
			var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
			SetControllerListeners(controllerManager.left);
			SetControllerListeners(controllerManager.right);
		}

		private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			if (currentBoost >= boostUse) {
				touchAxis = e.touchpadAxis;
				currentBoost -= boostUse;
				CalculateCardinalBoost (ref boostPower, touchAxis);
			}
		}

		private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			touchAxis = Vector2.zero;
		}
			

		private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
		{
			Vector3 headDirection = playerPresence.GetHeadset ().rotation.eulerAngles;
			int leftController = SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost);
			int rightController = SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Rightmost);

			triggerAxis = e.triggerPressedVal;

			if (currentBoost >= boostUse) {
				currentBoost -= boostUse;

				if (e.controllerIndex == rightController) {
					if (headDirection.y > 315 && headDirection.y < 360) {
						playerPresence.rb.velocity = new Vector3 (0, 0, triggerAxis.x) * boostPower;
					}else if (headDirection.y >= 0 && headDirection.y < 45) {
						playerPresence.rb.velocity = new Vector3 (0, 0, triggerAxis.x) * boostPower;
					} else if (headDirection.y > 45 && headDirection.y < 135) {
						playerPresence.rb.velocity = new Vector3 (triggerAxis.x, 0, 0) * boostPower;
					} else if (headDirection.y > 135 && headDirection.y < 225) {
						playerPresence.rb.velocity = new Vector3 (0, 0, -triggerAxis.x) * boostPower;
					} else if (headDirection.y > 225 && headDirection.y < 315) {
						playerPresence.rb.velocity = new Vector3 (-triggerAxis.x, 0, 0) * boostPower;
					} else {
					}

				} else if (e.controllerIndex == leftController) {
					if (headDirection.y > 315 && headDirection.y < 360) {
						playerPresence.rb.velocity = new Vector3 (0, 0, -triggerAxis.x) * boostPower;
					}else if (headDirection.y >= 0 && headDirection.y < 45) {
						playerPresence.rb.velocity = new Vector3 (0, 0, -triggerAxis.x) * boostPower;
					} else if (headDirection.y > 45 && headDirection.y < 135) {
						playerPresence.rb.velocity = new Vector3 (-triggerAxis.x, 0, 0) * boostPower;
					} else if (headDirection.y > 135 && headDirection.y < 225) {
						playerPresence.rb.velocity = new Vector3 (0, 0, triggerAxis.x) * boostPower;
					} else if (headDirection.y > 225 && headDirection.y < 315) {
						playerPresence.rb.velocity = new Vector3 (triggerAxis.x, 0, 0) * boostPower;
					} else {
					}
				}
			}
		}
			
		private void CalculateCardinalBoost(ref float boostPower, Vector2 inputValue)
		{
			float x = inputValue.x;
			float y = inputValue.y;
			Vector3 headDirection = playerPresence.GetHeadset ().rotation.eulerAngles;
				if (x != 0f && y != 0f) {
					if (headDirection.y > 315 || headDirection.y < 45) {
						playerPresence.rb.velocity = new Vector3 (x, y, 0) * boostPower;
					} else if (headDirection.y > 45 && headDirection.y < 135) {
						playerPresence.rb.velocity = new Vector3 (0, y, -x) * boostPower;
					} else if (headDirection.y > 135 && headDirection.y < 225) {
						playerPresence.rb.velocity = new Vector3 (-x, y, 0) * boostPower;
					} else if (headDirection.y > 225 && headDirection.y < 315) {
						playerPresence.rb.velocity = new Vector3 (0, y, x) * boostPower;
					} else {
					}
				}

		}

		private void BoostRegeneration(){
			currentBoost += boostRegen;
			timeSinceUnderMax = 0;
		}

		private void FixedUpdate(){
			if (currentBoost < maxBoost) {
				timeSinceUnderMax += Time.deltaTime;
				if (timeSinceUnderMax >= regenTime) {
					BoostRegeneration();
				}
			}
		}

		private void SetControllerListeners(GameObject controller)
		{
			if (controller && controller == controllerManager.left)
			{
				ToggleControllerListeners(controller, leftController, ref leftSubscribed);
			}
			else if (controller && controller == controllerManager.right)
			{
				ToggleControllerListeners(controller, rightController, ref rightSubscribed);
			}
		}

		private void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
		{
			var controllerEvent = controller.GetComponent<VRTK_ControllerEvents>();
			if (controllerEvent && toggle && !subscribed)
			{
				controllerEvent.TouchpadPressed += touchpadAxisChanged;
				controllerEvent.TouchpadReleased += touchpadUntouched;
				controllerEvent.TriggerPressed += triggerPressed;
				subscribed = true;
			}
			else if (controllerEvent && !toggle && subscribed)
			{
				controllerEvent.TouchpadAxisChanged -= touchpadAxisChanged;
				controllerEvent.TouchpadTouchEnd -= touchpadUntouched;
				controllerEvent.TriggerPressed -= triggerPressed;
				touchAxis = Vector2.zero;
				subscribed = false;
			}
		}
	}
}
