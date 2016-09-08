using UnityEngine;
using VRTK;

public class Gun : VRTK_InteractableObject
{
    private GameObject bullet;
    private float bulletSpeed = 1000f;
    private float bulletLife = 5f;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        FireBullet();
    }

    protected override void Start()
    {
        base.Start();
		Transform bulletTransform = transform.Find ("Bullet");
		if (!bulletTransform) {
			Debug.LogError ("VRTK Gun.cs: Unable to find a GameObject named 'Bullet'", gameObject);
			return;
		}
        bullet = transform.Find("Bullet").gameObject;
        bullet.SetActive(false);
    }

    private void FireBullet()
    {
		if (bullet) {
	        GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
	        bulletClone.SetActive(true);
	        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
	        rb.AddForce(-bullet.transform.forward * bulletSpeed);
	        Destroy(bulletClone, bulletLife);
		} else {
			Debug.LogError ("VRTK Gun.cs: Unable to fire bullet since bullet gameobject is not assigned", gameObject);
		}
    }
}