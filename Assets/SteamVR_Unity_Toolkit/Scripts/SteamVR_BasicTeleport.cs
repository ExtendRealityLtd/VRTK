//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the SteamVR_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class SteamVR_BasicTeleport : MonoBehaviour {
    public float blinkTransitionSpeed = 0.6f;
    [Range(0f,32f)]
    public float distanceBlinkDelay = 0f;
    public bool headsetPositionCompensation = true;
    public string ignoreTargetWithTagOrClass;

    protected int listenerInitTries = 0;
    protected Transform eyeCamera;
    protected bool adjustYForTerrain = false;

    private float blinkPause = 0f;
    private float fadeInTime = 0f;
    private float maxBlinkTransitionSpeed = 1.5f;
    private float maxBlinkDistance = 33f;
    private float retryListenersDelay = 0.25f;
    private float retryListenerMultiplier = 1.2f;
    private int listenerInitMax = 5;
    private List<int> connectedControllers;
    private List<int> foundWorldPointers;

    protected virtual void Start()
    {
        listenerInitTries = listenerInitMax;
        connectedControllers = new List<int>();
        foundWorldPointers = new List<int>();
        adjustYForTerrain = false;
        eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
        SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
    }

    protected virtual void Blink(float transitionSpeed)
    {
        fadeInTime = transitionSpeed;
        SteamVR_Fade.Start(Color.black, 0);
        Invoke("ReleaseBlink", blinkPause);
    }

    protected virtual bool ValidLocation(Transform target)
    {
        return (target && target.tag != ignoreTargetWithTagOrClass && target.GetComponent(ignoreTargetWithTagOrClass) == null);
    }

    protected virtual void DoTeleport(object sender, WorldPointerEventArgs e)
    {
        if (ValidLocation(e.target) && e.enableTeleport)
        {
            Vector3 newPosition = GetNewPosition(e.destinationPosition, e.target);
            CalculateBlinkDelay(blinkTransitionSpeed, newPosition);
            Blink(blinkTransitionSpeed);
            SetNewPosition(newPosition, e.target);
        }
    }

    protected virtual void SetNewPosition(Vector3 position, Transform target)
    {
        this.transform.position = CheckTerrainCollision(position, target);
    }

    protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target)
    {
        float newX = (headsetPositionCompensation ? (tipPosition.x - (eyeCamera.position.x - this.transform.position.x)) : tipPosition.x);
        float newY = this.transform.position.y;
        float newZ = (headsetPositionCompensation ? (tipPosition.z - (eyeCamera.position.z - this.transform.position.z)) : tipPosition.z);

        return new Vector3(newX, newY, newZ);
    }

    protected Vector3 CheckTerrainCollision(Vector3 position, Transform target)
    {
        if(adjustYForTerrain && target.GetComponent<Terrain>())
        {
            position.y = Terrain.activeTerrain.SampleHeight(position);
        }
        return position;
    }

    private void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
    {
        blinkPause = 0f;
        if (distanceBlinkDelay > 0f)
        {
            float distance = Vector3.Distance(this.transform.position, newPosition);
            blinkPause = Mathf.Clamp((distance * blinkTransitionSpeed) / (maxBlinkDistance - distanceBlinkDelay), 0, maxBlinkTransitionSpeed);
            blinkPause = (blinkSpeed <= 0.25 ? 0f : blinkPause);
        }
    }

    private void ReleaseBlink()
    {
        SteamVR_Fade.Start(Color.clear, fadeInTime);
        fadeInTime = 0f;
    }

    private void InitPointerListeners()
    {
        SteamVR_WorldPointer[] worldPointers = GameObject.FindObjectsOfType<SteamVR_WorldPointer>();

        // If the WorldPointer Object isn't initialised yet then retry in a quarter of a second
        // Because the Controller is a child of the CameraRig (and the WorldPointer is usually attached
        // to the Controller) then it is likely the WorldPointer object isn't available at start.
        if (worldPointers.Length != connectedControllers.Count)
        {
            if (listenerInitTries > 0)
            {
                listenerInitTries--;
            }
            else
            {
                retryListenersDelay = retryListenersDelay * retryListenerMultiplier;
                listenerInitTries = listenerInitMax;
                Debug.LogWarning("Waiting for controllers to initialise, retrying in " + retryListenersDelay);
            }
            Invoke("InitPointerListeners", retryListenersDelay);
        } else
        {
            foreach (SteamVR_WorldPointer worldPointer in worldPointers)
            {
                int worldPointerControllerIndex = (int)worldPointer.gameObject.GetComponent<SteamVR_ControllerEvents>().GetControllerIndex();
                if (! foundWorldPointers.Contains(worldPointerControllerIndex))
                {
                    worldPointer.WorldPointerDestinationSet += new WorldPointerEventHandler(DoTeleport);
                    worldPointer.SetMissTarget(ignoreTargetWithTagOrClass);
                    foundWorldPointers.Add(worldPointerControllerIndex);
                }
            }
        }
    }

    private void OnDeviceConnected(params object[] args)
    {
        if (IsController((uint)(int)args[0])) {
            if ((bool)args[1])
            {
                connectedControllers.Add((int)args[0]);
            }
            else
            {
                connectedControllers.Remove((int)args[0]);
            }
            Invoke("InitPointerListeners", 0.5f);
        }
    }

    private bool IsController(uint index)
    {
        var system = OpenVR.System;
        return (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller);
    }
}
