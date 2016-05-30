using UnityEngine;
using System.Collections;

public class VRTK_TouchpadWalking : MonoBehaviour {
    public float maxWalkSpeed = 3f;
    public float deceleration = 0.1f;

    private Vector2 touchAxis;
    private float movementSpeed = 0f;
    private float strafeSpeed = 0f;

    private VRTK_PlayerPresence playerPresence;

    private void Awake()
    {
        if (this.GetComponent<VRTK_PlayerPresence>())
        {
            playerPresence = this.GetComponent<VRTK_PlayerPresence>();
        }
        else
        {
            Debug.LogError("The VRTK_TouchpadWalking script requires the VRTK_PlayerPresence script to be attached to the [CameraRig]");
        }
    }

    private void Start () {
        this.name = "PlayerObject_" + this.name;

        var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
        InitControllerListeners(controllerManager.left);
        InitControllerListeners(controllerManager.right);
    }

    private void DoTouchpadAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        touchAxis = e.touchpadAxis;
    }

    private void DoTouchpadUntouched(object sender, ControllerClickedEventArgs e)
    {
        touchAxis = Vector2.zero;
    }

    private void CalculateSpeed(ref float speed, float inputValue)
    {
        if (inputValue != 0f)
        {
            speed = (maxWalkSpeed * inputValue);
        }
        else
        {
            Decelerate(ref speed);
        }
    }

    private void Decelerate(ref float speed)
    {
        if (speed > 0)
        {
            speed -= Mathf.Lerp(deceleration, maxWalkSpeed, 0f);
        }
        else if (speed < 0)
        {
            speed += Mathf.Lerp(deceleration, -maxWalkSpeed, 0f);
        }
        else
        {
            speed = 0;
        }

        float deadzone = 0.1f;
        if (speed < deadzone && speed > -deadzone)
        {
            speed = 0;
        }
    }

    private void Move()
    {
        var movement = playerPresence.GetHeadset().forward * movementSpeed * Time.deltaTime;
        var strafe = playerPresence.GetHeadset().right * strafeSpeed * Time.deltaTime;
        float fixY = this.transform.position.y;
        this.transform.position += (movement + strafe);
        this.transform.position = new Vector3(this.transform.position.x, fixY, this.transform.position.z);
    }

    private void FixedUpdate()
    {
        CalculateSpeed(ref movementSpeed, touchAxis.y);
        CalculateSpeed(ref strafeSpeed, touchAxis.x);
        Move();
    }

    private void InitControllerListeners(GameObject controller)
    {
        if (controller)
        {
            var controllerEvent = controller.GetComponent<VRTK_ControllerEvents>();
            if (controllerEvent)
            {
                controllerEvent.TouchpadAxisChanged += new ControllerClickedEventHandler(DoTouchpadAxisChanged);
                controllerEvent.TouchpadUntouched += new ControllerClickedEventHandler(DoTouchpadUntouched);
            }
        }
    }
}