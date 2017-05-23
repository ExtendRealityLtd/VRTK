//====================================================================================
//
// Purpose: Provide basic gravity for the player
//
// This script must be attached to the [CameraRig] Prefab
//
// Explanation:
// Script will add a rigidbody and a collider to represent the player.
// Collider is tracked to player position within the room.
// On falling (or jumping) the entire camera rig is moved.
//
// Known limitation:
// If you walk through objects it is possible to glitch the feet collider 
// through the object collider causing the camera rig to fall through.
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_PlayerGravity : MonoBehaviour
{

    private BoxCollider feetCollider;
    private Transform headset;

    public float feetSize = 0.5f;
    public float playerMass = 100f;

    void Start()
    {
        headset = this.transform.FindChild("Camera (head)");
        if (headset == null)
        {
            Debug.LogWarning("Script 'SteamVR_PlayerGravity' failed to find headset.");
        }

        feetCollider = this.gameObject.AddComponent<BoxCollider>();
        feetCollider.size = new Vector3(feetSize, feetSize, feetSize);
        feetCollider.center = new Vector3(0f, feetSize / 2, 0f);

        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        rb.mass = playerMass;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        if (headset != null)
        {
            feetCollider.center = new Vector3(headset.transform.localPosition.x, feetCollider.center.y, headset.transform.localPosition.z);
        }

    }
}
