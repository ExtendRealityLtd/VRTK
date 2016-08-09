using UnityEngine;
using System.Collections;

//=============================================================================
//
// Purpose: simple behaviour of a training remote (cf. Starwars Jedi training)
//
// This script must be attached to an object representing the training remote.
//
// A transform must be set to enable the training remote aiming at a target
// (e.g. user head).
//
// The training remote continues shhoting at its target at a given rate.
// When it has been hit with another object it "dies", disabiling its
// kinematics and enabling gravity (it falls down and the "resurrects" after a
// time period). 
//=============================================================================
public class TrainingRemote : MonoBehaviour
{
    [Tooltip("Target to shot")]
    public Transform targetTrainee;
    [Tooltip("Laser shot object (prefab)")]
    public GameObject laserShotGameObject;
    [Tooltip("Time interval between two shots (sec)")]
    public float shotInterval = 3f;
    [Tooltip("Speeed of the laser shot (m/s)")]
    public float laserShotSpeed = 3f;
    [Tooltip("Maximum move speeed along each axis (m/s)")]
    public Vector3 maxMoveSpeed = Vector3.one;
    [Tooltip("Preferred distance from the target (m)")]
    public float preferredDistance = 3f;
    [Tooltip("Maximum vertical distance from the target (m)")]
    public float maxVerticalDistance = 1f;
    [Tooltip("Time interval before training remote resurrection (sec)")]
    public float deathDuration = 5f;

    private GameObject laserShot;
    private AudioSource laserShotSoundSource;
    private Rigidbody laserShotRb;
    private float lastShotTime = 0f;
    private float deadTimer = 0f;

    private void Start()
    {
        laserShot = Instantiate(laserShotGameObject);
        laserShotRb = laserShot.GetComponent<Rigidbody>();
        laserShotSoundSource = laserShot.GetComponent<AudioSource>();
        if (laserShotSoundSource)
        {
            laserShotSoundSource.loop = false;
        }
        laserShot.SetActive(false);
        Physics.IgnoreCollision(this.GetComponent<Collider>(), laserShot.GetComponent<Collider>(), true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 0.01f)
        {
            Die();
        }
    }

    private void Die()
    {
        deadTimer = deathDuration;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void Update()
    {
        if (deadTimer > 0f)
        {
            deadTimer -= Time.deltaTime;
            if (deadTimer > 0f) return;
            // resurrect (re-enable "anti-gravity engine")
            GetComponent<Rigidbody>().useGravity = false;
        }
        transform.LookAt(targetTrainee);
        if (Time.time - lastShotTime > shotInterval)
        {
            if (laserShotSoundSource != null && laserShotSoundSource.isActiveAndEnabled)
            {
                laserShotSoundSource.Play();
            }
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            lastShotTime = Time.time;
            laserShot.SetActive(true);
            laserShotRb.transform.position = transform.position;
            laserShotRb.transform.LookAt(targetTrainee.position);
            Vector3 offset = targetTrainee.position - transform.position;
            Vector3 dir = offset.normalized;
            laserShotRb.velocity = dir * laserShotSpeed;
            Vector3 currentVelocity = dir * (offset.magnitude - preferredDistance);
            currentVelocity.x += Random.Range(-maxMoveSpeed.x, maxMoveSpeed.x);
            currentVelocity.y += Random.Range(-maxMoveSpeed.y, maxMoveSpeed.y);
            currentVelocity.z += Random.Range(-maxMoveSpeed.z, maxMoveSpeed.z);
            if (Mathf.Abs(offset.y) > maxVerticalDistance)
            {
                currentVelocity.y += offset.y;
            }
            if (shotInterval > 0) currentVelocity /= shotInterval;
            GetComponent<Rigidbody>().velocity = currentVelocity;
        }
    }
}
