using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
    public float maxArrowLife = 10f;
    [HideInInspector]
    public bool inFlight = false;

    private bool collided = false;
    private Rigidbody rigidBody;
    private GameObject arrowHolder;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public void SetArrowHolder(GameObject holder)
    {
        arrowHolder = holder;
        arrowHolder.SetActive(false);
    }

    public void OnNock()
    {
        collided = false;
        inFlight = false;
    }

    public void Fired()
    {
        DestroyArrow(maxArrowLife);
    }

    public void ResetArrow()
    {
        collided = true;
        inFlight = false;
        RecreateNotch();
        ResetTransform();
    }

    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        SetOrigns();
    }

    private void SetOrigns()
    {
        originalPosition = this.transform.localPosition;
        originalRotation = this.transform.localRotation;
        originalScale = this.transform.localScale;
    }

    private void FixedUpdate()
    {
        if (!collided)
        {
           transform.LookAt(transform.position + rigidBody.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (inFlight)
        {
            ResetArrow();
        }
    }

    private void RecreateNotch()
    {
        //swap the arrow holder to be the parent again
        arrowHolder.transform.parent = null;
        arrowHolder.SetActive(true);

        //make the arrow a child of the holder again
        this.transform.parent = arrowHolder.transform;

        //reset the state of the rigidbodies and colliders
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Collider>().enabled = false;
        arrowHolder.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void ResetTransform()
    {
        arrowHolder.transform.position = this.transform.position;
        arrowHolder.transform.rotation = this.transform.rotation;
        this.transform.localPosition = originalPosition;
        this.transform.localRotation = originalRotation;
        this.transform.localScale = originalScale;
    }

    private void DestroyArrow(float time)
    {
        Destroy(arrowHolder, time);
        Destroy(this.gameObject, time);
    }
}
