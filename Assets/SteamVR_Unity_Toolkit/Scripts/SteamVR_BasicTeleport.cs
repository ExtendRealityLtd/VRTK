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

public class SteamVR_BasicTeleport : MonoBehaviour {
    public float blinkTransitionSpeed = 0.6f;
    [Range(0f,32f)]
    public float distanceBlinkDelay = 0f;
    public bool headsetPositionCompensation = true;

    protected int listenerInitTries = 5;
    protected Transform eyeCamera;
    protected bool adjustYForTerrain = false;

    private float blinkPause = 0f;
    private float fadeInTime = 0f;
    private float maxBlinkTransitionSpeed = 1.5f;
    private float maxBlinkDistance = 33f;

    protected virtual void Start()
    {
        adjustYForTerrain = false;
        InitPointerListeners();
        eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
    }

    protected virtual void Blink(float transitionSpeed)
    {
        fadeInTime = transitionSpeed;
        SteamVR_Fade.Start(Color.black, 0);
        Invoke("ReleaseBlink", blinkPause);
    }

    protected virtual void DoTeleport(object sender, WorldPointerEventArgs e)
    {
        if (e.target && e.enableTeleport)
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
        if (worldPointers.Length == 0)
        {
            if (listenerInitTries > 0)
            {
                listenerInitTries--;
                Invoke("InitPointerListeners", 0.25f);
            }
            else
            {
                Debug.LogError("A GameObject must exist with a SteamVR_WorldPointer script attached to it");
                return;
            }
        }

        foreach (SteamVR_WorldPointer worldPointer in worldPointers)
        {
            worldPointer.WorldPointerDestinationSet += new WorldPointerEventHandler(DoTeleport);
        }
    }
}
