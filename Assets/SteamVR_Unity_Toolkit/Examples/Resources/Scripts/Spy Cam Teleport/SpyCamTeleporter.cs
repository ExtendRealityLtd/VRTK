using UnityEngine;
using System.Collections;
using VRTK;

public class SpyCamTeleporter : MonoBehaviour
{

    public GameObject spyCamPrefab;
    public SpyCamViewport spyCamViewport;
    public Vector3 cameraOffset;

    private VRTK_DestinationMarker destinationMarker;
    private VRTK_ControllerEvents controllerEvents;
    private SpyCam currentSpawnedSpyCam = null;

    void Awake()
    {
        destinationMarker = GetComponent<VRTK_DestinationMarker>();
        controllerEvents = GetComponent<VRTK_ControllerEvents>();

        if (destinationMarker == null)
        {
            Debug.LogError("VRTK_SpyCamTeleporter requires a VRTK_DestinationMarker be attached to the same GameObject.");
            return;
        }

        if (spyCamPrefab == null)
        {
            Debug.LogError("VRTK_SpyCamTeleporter requires the spyCamPrefab to be assigned.");
            return;
        }

        if (spyCamViewport == null)
        {
            Debug.LogError("VRTK_SpyCamTeleporter requires the spyCamViewport to be assigned.");
            return;
        }

        // Ensure our viewport is disabled
        spyCamViewport.DeactivateViewport();
    }

    void OnEnable()
    {
        destinationMarker.DestinationMarkerEnter += OnDestinationMarkerEnter;
        destinationMarker.DestinationMarkerSet += OnDestinationMarkerSet;
        controllerEvents.AliasPointerSet += OnAliasPointerSet;
    }

    void OnDisable()
    {
        destinationMarker.DestinationMarkerEnter -= OnDestinationMarkerEnter;
        destinationMarker.DestinationMarkerSet -= OnDestinationMarkerSet;
        controllerEvents.AliasPointerSet -= OnAliasPointerSet;
    }

    void Update()
    {
        if (currentSpawnedSpyCam != null)
        {
            // We need to update our SpyCam's camera based on our controller rotation
            Quaternion newRotation = this.transform.rotation * Quaternion.Euler(cameraOffset);
            currentSpawnedSpyCam.UpdateCameraRotation(newRotation);
        }
    }

    private void OnDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
    {
        // Check to see if we already have a spy cam spawned
        // If we do, destroy it now before we spawn the next
        if (currentSpawnedSpyCam != null)
        {
            Destroy(currentSpawnedSpyCam.gameObject);
            currentSpawnedSpyCam = null;
        }

        // Hide our viewport
        spyCamViewport.DeactivateViewport();
    }

    private void OnDestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
    {
        // Spawn our spy cam
        currentSpawnedSpyCam = (Instantiate(spyCamPrefab, e.destinationPosition, Quaternion.identity) as GameObject).GetComponent<SpyCam>();

        // Setup the event listeners on the SpyCam
        currentSpawnedSpyCam.SpyCamActivated += OnSpyCamActivated;

        // Initialise the SpyCam for use
        currentSpawnedSpyCam.InitSpyCam(e);

        // Disable our pointer. This allows us to teleport to the desired location without using the pointer again.
        destinationMarker.enabled = false;
    }

    private void OnAliasPointerSet(object sender, ControllerInteractionEventArgs e)
    {
        // Check to see if we have actually spawned a SpyCam
        if (currentSpawnedSpyCam == null)
        {
            return;
        }

        // Teleport!
        currentSpawnedSpyCam.Teleport();

        // Enable the pointer for use again.
        destinationMarker.enabled = true;

        // Hide the viewport
        spyCamViewport.DeactivateViewport();

        Destroy(currentSpawnedSpyCam.gameObject);
        currentSpawnedSpyCam = null;
    }

    private void OnSpyCamActivated(Camera camera)
    {
        // Activate our spyCamViewport
        spyCamViewport.gameObject.SetActive(true);

        // Assign the targetTexture from the SpyCam Camera to our viewportWindow
        // This lets us view what the SpyCam is seeing!
        spyCamViewport.ActivateViewport(camera);
    }

}