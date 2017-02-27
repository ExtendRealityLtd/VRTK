using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    public float moveYAmount = 20.0f;
    public float moveSpeed = 1.0f;
    public float waitTime = 5.0f;
    public float rotateSpeed = 10.0f;

    private float startY;
    private bool goingUp = true;
    private float stoppedUntilTime;
    private float moveUpAmount;

    protected virtual void Start ()
    {
        startY = transform.position.y;

        moveUpAmount = Mathf.Abs(moveYAmount);

        if(moveYAmount < 0.0f)
        {
            startY -= moveYAmount;
            goingUp = false;
        }

        stoppedUntilTime = Time.time + waitTime;
    }
    
    protected virtual void Update ()
    {
        if( Time.time > stoppedUntilTime)
        {
            if(goingUp)
            {
                if(transform.position.y < startY+moveUpAmount)
                {
                    Vector3 newPosition = transform.position;
                    newPosition.y += Time.deltaTime * moveSpeed;
                    transform.position = newPosition;
                }
                else
                {
                    goingUp = false;
                    stoppedUntilTime = Time.time + waitTime;
                }
            }
            else
            {
                if(transform.position.y > startY)
                {
                    Vector3 newPosition = transform.position;
                    newPosition.y -= Time.deltaTime * moveSpeed;
                    transform.position = newPosition;
                }
                else
                {
                    goingUp = true;
                    stoppedUntilTime = Time.time + waitTime;
                }
            }
        }

        transform.Rotate(new Vector3(0.0f, rotateSpeed * Time.deltaTime, 0.0f));
    }
}
