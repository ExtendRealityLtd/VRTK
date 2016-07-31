namespace VRTK
{
    using UnityEngine;
    using System.Collections;









        {
            PlayerPhysicsEventArgs e;
            e.fallDistance = fallDistance;
            return e;
        }




        {
            rb.mass = 80;
            rb.freezeRotation = true;

            bc = this.gameObject.GetComponent<BoxCollider>();

            bc.center = new Vector3(0f, 0.25f, 0f);
            bc.size = new Vector3(0.5f, 0.5f, 0.5f);

            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }





        {
        }

    }
}
