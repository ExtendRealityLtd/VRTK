using UnityEngine;
using System.Collections;

public class Breakable_Cube : MonoBehaviour
{
    private float breakForce = 150f;

    private void Start()
    {
        this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionForce = GetCollisionForce(collision);

        if (collisionForce > 0)
        {
            ExplodeCube(collisionForce);
        }
    }

    private float GetCollisionForce(Collision collision)
    {
        if ((collision.collider.name.Contains("Sword") && collision.collider.GetComponent<Sword>().CollisionForce() > breakForce))
        {
            return collision.collider.GetComponent<Sword>().CollisionForce() * 1.2f;
        }

        if (collision.collider.name.Contains("Arrow"))
        {
            return 500f;
        }

        return 0f;
    }

    private void ExplodeCube(float force)
    {
        foreach (Transform face in this.GetComponentsInChildren<Transform>())
        {
            if (face.transform.name == this.transform.name) continue;
            ExplodeFace(face, force);
        }
        Destroy(this.gameObject, 10f);
    }

    private void ExplodeFace(Transform face, float force)
    {
        face.transform.parent = null;
        Rigidbody rb = face.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddExplosionForce(force, Vector3.zero, 0f);
        Destroy(face.gameObject, 10f);
    }
}