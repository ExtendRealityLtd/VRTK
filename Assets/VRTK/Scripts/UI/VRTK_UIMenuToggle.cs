// Menu Toggle|Utilities|90040
namespace VRTK
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using VRTK;

	[AddComponentMenu("VRTK/Scripts/UI/VRTK_UIMenuToggle")]
	public class VRTK_UIMenuToggle : MonoBehaviour
	{
		[Header("Controller Settings")]
		[Tooltip("The VRTK_pointer on the controller.")]
		public VRTK_Pointer pointer;
		[Tooltip("The renderer for menu interaction.")]
		public VRTK_StraightPointerRenderer straightPointerRenderer;
		[Tooltip("The renderer for teleportation interaction.")]
		public VRTK_BezierPointerRenderer bezierPointerRenderer;

		[Tooltip("ControllerEvents for activating menu.")]
		public VRTK_ControllerEvents controllerEvents;

		[Header("Menu Settings")]
		[Tooltip("Game object that holds the menu.")]
		public GameObject menu;
		[Tooltip("whether the menu is positioned at the controller each time it is enabled")]
		public bool putMenuAtController = true;
		[Tooltip("relative position for menu from controller when activated.")]
		public Vector3 offsetFromController = new Vector3 (0.0f, 0.1f, 0.3f);
		[Tooltip("how much X axis rotation to apply to the menu object when activated.")]
		public float tiltBackAngle = 45.0f;

		void Reset()
		{
			if (straightPointerRenderer == null)
				straightPointerRenderer = GetComponent<VRTK_StraightPointerRenderer> ();
			if (bezierPointerRenderer == null)
				bezierPointerRenderer = GetComponent<VRTK_BezierPointerRenderer> ();
			if (controllerEvents == null)
				controllerEvents = GetComponent<VRTK_ControllerEvents> ();
			if (pointer == null)
				pointer = GetComponent<VRTK_Pointer> ();

		}

		void Start()
		{
				useBezierTeleportPointer ();
		}

		void OnEnable()
		{
			controllerEvents.ButtonTwoReleased += ControllerEvents_ButtonTwoReleased;
		}

		void OnDisable()
		{
			controllerEvents.ButtonTwoReleased -= ControllerEvents_ButtonTwoReleased;
		}

		/// <summary>
		/// toggle the pointer state so it can be adjusted
		/// </summary>
		/// <param name="onOff">If set to <c>true</c> the pointer & renderer is enabled else they are disabled.</param>
		void setPointerState(bool onOff)
		{
			pointer.enabled = onOff;
			pointer.pointerRenderer.enabled = onOff;
		}

		/// <summary>
		/// Positions the menu near the controller.
		/// </summary>
		void positionMenu()
		{
			menu.transform.position = transform.TransformPoint(offsetFromController);
			menu.transform.rotation = transform.rotation ;
			menu.transform.Rotate (new Vector3 (tiltBackAngle, 0.0f, 0.0f));
		}

		/// <summary>
		/// Turn on the menu & its pointer, disable teleport pointer
		/// </summary>
		public void useStraightPointerWithMenu()
		{
			setPointerState (false);

			if (putMenuAtController) positionMenu ();

			pointer.enableTeleport = false;
			pointer.pointerRenderer = straightPointerRenderer;
			pointer.activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
			pointer.holdButtonToActivate = false;
			pointer.activateOnEnable = true;

			menu.SetActive (true);

			setPointerState (true); // re-enable, re-initialize pointer & renderer
		}

		/// <summary>
		/// Turn off the menu and its pointer renderer, enable the teleport pointer
		/// </summary>
		public void useBezierTeleportPointer()
		{
			menu.SetActive (false);

			setPointerState (false); 

			pointer.enableTeleport = true;
			pointer.pointerRenderer = bezierPointerRenderer;
			pointer.activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
			pointer.holdButtonToActivate = true;
			pointer.activateOnEnable = false;

			setPointerState (true); // re-enable, re-initialize pointer & renderer
		}


		/// <summary>
		/// called when the menu activation button is released
		/// </summary>
		/// <param name="sender">this object</param>
		/// <param name="e"><see cref="UIPointerEventArgs"/></param>
		private void ControllerEvents_ButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
		{
			bool menuWillActivate = !menu.activeSelf;
		
			if (menuWillActivate)
				useStraightPointerWithMenu ();
			else
				useBezierTeleportPointer ();
		}
	}
}