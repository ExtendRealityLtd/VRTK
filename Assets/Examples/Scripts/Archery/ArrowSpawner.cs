using UnityEngine;
using System.Collections;

public class ArrowSpawner : MonoBehaviour {
    public GameObject arrowPrefab;
    public float spawnDelay = 1f;

    private float spawnDelayTimer = 0f;
    private SteamVR_ControllerManager controllers;
    private BowAim bow;

    private void Start()
    {
        controllers = FindObjectOfType<SteamVR_ControllerManager>();
        spawnDelayTimer = 0f;
    }

    private void OnTriggerStay(Collider collider)
    {
        VRTK_InteractGrab grabbingController = collider.gameObject.GetComponent<VRTK_InteractGrab>();
        if (CanGrab(grabbingController) && NoArrowNotched(grabbingController.gameObject) && spawnDelayTimer <= 0f)
        {
            GameObject newArrow = Instantiate(arrowPrefab);
            newArrow.name = "ArrowClone";
            grabbingController.gameObject.GetComponent<VRTK_InteractTouch>().ForceTouch(newArrow);
            grabbingController.AttemptGrab();
            spawnDelayTimer = spawnDelay;
        }

        if (spawnDelayTimer > 0)
        {
            spawnDelayTimer -= Time.deltaTime;
        }
    }

    private bool CanGrab(VRTK_InteractGrab grabbingController)
    {
        return (grabbingController && grabbingController.GetGrabbedObject() == null && grabbingController.gameObject.GetComponent<VRTK_ControllerEvents>().grabPressed);
    }

    private bool NoArrowNotched(GameObject controller)
    {
        if (controller == controllers.left)
        {
            bow = controllers.right.GetComponentInChildren<BowAim>();
        }
        else if(controller == controllers.right)
        {
            bow = controllers.left.GetComponentInChildren<BowAim>();
        }

        return (bow == null || !bow.HasArrow());
    }
}
