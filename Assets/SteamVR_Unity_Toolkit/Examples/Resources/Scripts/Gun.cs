using UnityEngine;
using System.Collections;
using VRTK;

public class Gun : VRTK_InteractableObject
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
        bullet.SetActive(false);
    }

    void FireBullet()
    {
        GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
        bulletClone.SetActive(true);
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        rb.AddForce(-bullet.transform.forward * bulletSpeed);
        Destroy(bulletClone, bulletLife);
    }
}
