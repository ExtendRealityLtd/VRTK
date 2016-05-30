﻿//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the VRTK_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class VRTK_BasicTeleport : MonoBehaviour {
    public float blinkTransitionSpeed = 0.6f;
    [Range(0f,32f)]
    public float distanceBlinkDelay = 0f;
    public bool headsetPositionCompensation = true;
    public string ignoreTargetWithTagOrClass;

    protected Transform eyeCamera;
    protected bool adjustYForTerrain = false;

    private float blinkPause = 0f;
    private float fadeInTime = 0f;
    private float maxBlinkTransitionSpeed = 1.5f;
    private float maxBlinkDistance = 33f;

    protected virtual void Start()
    {
        this.name = "PlayerObject_" + this.name;
        adjustYForTerrain = false;
        eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();

        var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
        InitControllerListeners(controllerManager.left);
        InitControllerListeners(controllerManager.right);
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

    private void InitControllerListeners(GameObject controller)
    {
        if (controller)
        {
            var worldPointer = controller.GetComponent<VRTK_WorldPointer>();
            if (worldPointer)
            {
                worldPointer.WorldPointerDestinationSet += new WorldPointerEventHandler(DoTeleport);
                worldPointer.SetMissTarget(ignoreTargetWithTagOrClass);
            }
        }
    }
}
