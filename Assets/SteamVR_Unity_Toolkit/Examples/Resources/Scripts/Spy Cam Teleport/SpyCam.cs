using UnityEngine;
using System.Collections;
using VRTK;

public class SpyCam : VRTK_DestinationMarker
{

    public Vector3 hoverDistance = new Vector3(0f, 0.5f, 0f);
    public float repositionTime = 1f;

    public System.Action<Camera> SpyCamActivated;
    public System.Action SpyCamDeactivated;

    private Rigidbody myRigidbody;
    private Camera viewportCamera;
    private DestinationMarkerEventArgs savedDestMarkerEventArgs;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            myRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        viewportCamera = GetComponentInChildren<Camera>();
    }

    void OnDestroy()
    {
        if (SpyCamDeactivated != null)
        {
            SpyCamDeactivated.Invoke();
        }
    }

    public void InitSpyCam(DestinationMarkerEventArgs e)
    {
        savedDestMarkerEventArgs = e;
        if (viewportCamera == null)
        {
            viewportCamera = GetComponentInChildren<Camera>();
        }
        Reposition();
    }

    public void UpdateCameraRotation(Quaternion newRotation)
    {
        this.transform.rotation = newRotation;
    }

    public void Teleport()
    {
        savedDestMarkerEventArgs.enableTeleport = true;
        OnDestinationMarkerSet(savedDestMarkerEventArgs);
    }

    private void Reposition()
    {
        StartCoroutine(HoverUp());
    }

    private IEnumerator HoverUp()
    {
        Vector3 newPos = this.transform.position + hoverDistance;
        float t = 0f;
        while (t < repositionTime)
        {
            if (t > (repositionTime - 0.05f))
            {
                this.transform.rotation = Quaternion.identity;
                this.transform.position = newPos;
                break;
            }
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.identity, (t / repositionTime));
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, (t / repositionTime));
            t += Time.deltaTime;
            yield return null;
        }

        GameObject.FindObjectOfType<VRTK_BasicTeleport>().InitDestinationSetListener(this.gameObject, true);

        if (SpyCamActivated != null)
        {
            SpyCamActivated.Invoke(viewportCamera);
        }
    }

}
