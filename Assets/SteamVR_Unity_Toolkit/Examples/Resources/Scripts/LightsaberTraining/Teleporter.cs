using UnityEngine;
using VRTK;

//=============================================================================
//
// Purpose: Teleport any object colliding with the game object that holds this
//
// This script should be attached to the object that acts as teleport.
//
// A transform must be set to enable the script to teleport objects to a
// destination position (and optionally rotation).
//
//=============================================================================
public class Teleporter : MonoBehaviour
{
    [Tooltip("Transform where objects will be teleported")]
    public Transform destinationTransform;
    [Tooltip("Affect not only position but also rotation")]
    public bool affectRotation = false;

    protected virtual void Teleport(GameObject obj)
    {
        if (destinationTransform!=null && obj.transform.root.gameObject.GetComponent<VRTK_InteractableObject>() != null)
        {
            Rigidbody objRigidBody = obj.GetComponent<Rigidbody>();
            Transform objTransform = obj.transform;
            if (objRigidBody != null)
            {
                objTransform = objRigidBody.transform;
                objRigidBody.velocity = Vector3.zero;
            }
            objTransform.position = destinationTransform.position;
            if (affectRotation) objTransform.rotation = destinationTransform.rotation;
        }
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        Teleport(collider.gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Teleport(collision.gameObject);
    }
}
