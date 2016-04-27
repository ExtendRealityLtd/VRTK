using UnityEngine;
using System.Collections;

public class Gun : SteamVR_InteractableObject
{
    GameObject bullet;
    float bulletSpeed = 1000f;
    float bulletLife = 5f;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        FireBullet();
    }

    protected override void Start()
    {
        base.Start();
        bullet = this.transform.Find("Bullet").gameObject;
    }

    void FireBullet()
    {
        GameObject bulletClone = Instantiate(bullet, bullet.transform.position, Quaternion.identity) as GameObject;
        bulletClone.SetActive(true);
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        rb.AddRelativeForce(-transform.forward * bulletSpeed);
        Destroy(bulletClone, bulletLife);
    }
}
