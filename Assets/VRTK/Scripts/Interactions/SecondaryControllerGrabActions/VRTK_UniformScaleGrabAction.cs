using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

public class VRTK_UniformScaleGrabAction : VRTK_BaseGrabAction
{
	private float initalLength;
	private float initialScaleFactor;

	/// <summary>
	/// The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.
	/// </summary>
	/// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary controller.</param>
	/// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary controller.</param>
	/// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary controller.</param>
	/// <param name="primaryGrabPoint">The point on the object where the primary controller initially grabbed the object.</param>
	/// <param name="secondaryGrabPoint">The point on the object where the secondary controller initially grabbed the object.</param>
	public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
	{
		base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);

		initalLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
		initialScaleFactor = currentGrabbdObject.transform.localScale.x / initalLength;
	}

	/// <summary>
	/// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and performs the scaling action.
	/// </summary>
	public override void ProcessFixedUpdate()
	{
		if (initialised)
		{
			float newLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
			float newScale = initialScaleFactor * newLength;
			grabbedObject.transform.localScale = new Vector3(newScale, newScale, newScale);
		}
	}
}