using UnityEngine;
using System.Collections;

public class Breakable_Cube : MonoBehaviour {
    float breakForce = 150f;

    void Start()
    {
        this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void OnCollisionEnter(Collision collision)
    {
        if((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce) || collision.collider.name.Contains("Arrow"))
        {
            ExplodeCube();
        }
    }

    void ExplodeCube()
    {
        foreach (Transform face in this.GetComponentsInChildren<Transform>())
        {
            if (face.transform.name == this.transform.name) continue;
            ExplodeFace(face);
        }
        Destroy(this.gameObject, 10f);
    }

    void ExplodeFace(Transform face)
    {
        face.transform.parent = null;
        Rigidbody rb = face.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddExplosionForce(750f, Vector3.zero, 0f);
        Destroy(face.gameObject, 10f);
    }
}
