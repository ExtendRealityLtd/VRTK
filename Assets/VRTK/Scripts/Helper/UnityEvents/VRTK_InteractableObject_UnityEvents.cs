using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using VRTK;

[RequireComponent (typeof(VRTK_InteractableObject))]
public class VRTK_InteractableObject_UnityEvents : MonoBehaviour 
{

	private VRTK_InteractableObject io;

	[System.Serializable]
	public class UnityObjectEvent: UnityEvent<GameObject> {};
	public UnityObjectEvent OnTouch;
	public UnityObjectEvent OnUntouch; 
	public UnityObjectEvent OnGrab; 
	public UnityObjectEvent OnUngrab; 
	public UnityObjectEvent OnUse; 
	public UnityObjectEvent OnUnuse;  

	private void OnEnable () 
	{
		io.InteractableObjectTouched += Touch;
		io.InteractableObjectUntouched += UnTouch;
		io.InteractableObjectGrabbed += Grab;
		io.InteractableObjectUngrabbed += UnGrab;
		io.InteractableObjectUsed += Use;
		io.InteractableObjectUnused += Unuse;
	}

	private void Touch(object o, InteractableObjectEventArgs e)
	{
		OnTouch.Invoke(e.interactingObject);
	}

	private void UnTouch(object o, InteractableObjectEventArgs e)
	{
		OnUntouch.Invoke(e.interactingObject);
	}

	private void Grab(object o, InteractableObjectEventArgs e)
	{
		OnGrab.Invoke(e.interactingObject);
	}

	private void UnGrab(object o, InteractableObjectEventArgs e)
	{
		OnUngrab.Invoke(e.interactingObject);
	}

	private void Use(object o, InteractableObjectEventArgs e)
	{
		OnUse.Invoke(e.interactingObject);
	}

	private void Unuse(object o, InteractableObjectEventArgs e)
	{
		OnUnuse.Invoke(e.interactingObject);
	}

	private void OnDisable () 
	{
		io.InteractableObjectTouched -= Touch;
		io.InteractableObjectUntouched -= UnTouch;
		io.InteractableObjectGrabbed -= Grab;
		io.InteractableObjectUngrabbed -= UnGrab;
		io.InteractableObjectUsed -= Use;
		io.InteractableObjectUnused -= Unuse;
	}
}
