// Player Jump|Scripts|0215
namespace VRTK
{
    using UnityEngine;
    using System.IO;
    using System;


    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="jumpVelocity">The velocity added when a player jumps</param>
    public struct PlayerJumpEventArgs
    {
        public Vector3 jumpVelocity;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayerJumpEventArgs"/></param>
    public delegate void PlayerJumpEventHandler(object sender, PlayerJumpEventArgs e);

    /// <summary>
    /// The player jump provides a jump behavior by tracking HMD world position over time, calculating velocities of a crouch and a spring motion, and adding velocity to the attached rigidbody on playerpresence.  It should be attached to the `[CameraRig]` object.
    /// </summary>
    /// <remarks>
    /// Jump scaling for non-fixed jump factors is calculated in CalcAdjustedFinalJumpVelocity
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/041_CameraRig_PlayerJump` provides a camerarig with the jump script set to low power settings.
    /// </example>

    [RequireComponent(typeof(VRTK_PlayerPresence))]
    public class VRTK_PlayerJump : MonoBehaviour
    {
        #region ### Public Properties ###

        [Tooltip("Jump velocity in the up direction is magnified by this")]
        public float upFactor = 5.0f;

        [Tooltip("Jump velocity in the facing direction is magnified by this")]
        public float forwardFactor = 3.0f;

        [Tooltip("Offset to detect a crouch before jump")]
        public float downThreshold = -0.8f;

        [Tooltip("Offset to detect a spring into jump")]
        public float upThreshold = 1.2f;

        [Tooltip("Time in seconds to reset")]
        public float timeout = 0.5f;

        // TODO - the scaled jump is in progress
        [Tooltip("Fixed vs dynamic jump power - uncheck to scale based on player jump intensity")]
        private bool jumpFixed = true;

        [Tooltip("Amount to magnify the player jump intensity when jump scaling not fixed - modifies Up Factor and Forward Factor")]
        private float jumpScaling = 3.0f;

        #endregion

        #region ### Events ###

        /// <summary>
        /// Emitted when a jump has started.
        /// </summary>
        public event PlayerJumpEventHandler PlayerJumpStarted;
        /// <summary>
        /// Emitted when a jump has ended.
        /// </summary>
        public event PlayerJumpEventHandler PlayerJumpEnded;

        #endregion

        #region ### Private Properties ###

        // deviations less than this will not be counted against baseline establishment - magic number alert!
        private float baselineUpdateSensitivity = 0.5f;

        // this is the baseline height recorded from the HMD over time
        private float baselineHeight = 0;

        // this is a time since starting crouch.  Zero if not in jump state
        private float crouchStartTime = 0;

        // player jumping state as understood by this script
        private bool isJumping = false;

        // previous height of HMD;
        private float lastHeight;

        // lowest point in crouch
        private float crouchLow;

        // time of crouchLow
        private float crouchLowTime;

        // velocity history
        private float[] historicVelocity;

        // number of history points to compare
        private int historyLen = 5;

        // sets crouchlow above a likely distance for player to travel in a frame
        private const int RESETCROUCHLOW = 100000;

        // represents the state informed by the climbing Component
        private bool isClimbing = false;

        // represents the state informed by the player presence Component
        //private bool isFalling = false;


        // external components
        private Transform HMDTransform;
        private VRTK_PlayerPresence playerPresence;

        #endregion

        private void Awake()
        {
            playerPresence = GetComponent<VRTK_PlayerPresence>();
            playerPresence.SetFallingPhysicsOnlyParams(true);
            playerPresence.StartPhysicsFall(Vector3.zero);

            HMDTransform = VRTK_DeviceFinder.HeadsetTransform();

            historicVelocity = new float[historyLen];
        }

        protected virtual void Start()
        {
            VRTK_HeightAdjustTeleport hat = gameObject.GetComponent<VRTK_HeightAdjustTeleport>();
            if ( hat )
            {
                hat.useGravity = true;
            }

            baselineHeight = HMDTransform.localPosition.y;
            // TODO - may want a cooldown timer to prevent spurious jumps when headset is first put on
            crouchLowTime = -1;
        }

        protected virtual void Update()
        {
            if (!isClimbing)
            {
                CheckPosture();
            }
        }

        protected void CheckPosture()
        {
            float jumpVelocity = 0;

            var dv = UpdateHeightHistoryAndReturnVelocity(HMDTransform.localPosition.y);

            if (isJumping)
            {
                CheckForJumpEnd(dv);
            }
            else
            {
                UpdateBaseline(dv);
                CheckForCrouchStart(dv);

                if (CrouchWasDetected())
                {
                    UpdateCrouchLowpoint();

                    if (CrouchTimerNotExpired())
                    {
                        if (VelocityChangeAboveThreshold(dv) && HMDHeightExceedsBaseline() && IsCloseToGround())
                        {
                            isJumping = true;

                            jumpVelocity = jumpFixed ? 1 : CalcAdjustedFinalJumpVelocity();

                            DoJump(jumpVelocity);
                        }
                    }
                    else
                    {
                        ResetJumpVars();
                    }

                }
                else
                {
                    isJumping = false;
                }
            }
        }

        protected void ResetJumpVars()
        {
            crouchStartTime = 0;
            crouchLowTime = Time.time;
            crouchLow = baselineHeight + RESETCROUCHLOW;
            isJumping = false;
        }



        protected void OnEnable()
        {
            InitClimbEvents(true);
        }

        protected void OnDisable()
        {
            InitClimbEvents(false);
        }

        private void OnPlayerJumpStarted(PlayerJumpEventArgs e)
        {
            if (PlayerJumpStarted != null)
            {
                PlayerJumpStarted(this, e);
            }
        }

        private void OnPlayerJumpEnded(PlayerJumpEventArgs e)
        {
            if (PlayerJumpEnded != null)
            {
                PlayerJumpEnded(this, e);
            }
        }

        private PlayerJumpEventArgs SetPlayerJumpEvent(Vector3 velocity)
        {
            PlayerJumpEventArgs e;
            e.jumpVelocity = velocity;
            return e;
        }

        private bool HMDHeightExceedsBaseline()
        {
            return HMDTransform.localPosition.y > baselineHeight;
        }

        private bool VelocityChangeAboveThreshold(float dv)
        {
            return dv > upThreshold;
        }

        private bool CrouchTimerNotExpired()
        {
            return Time.time - crouchStartTime < timeout;
        }

        private bool CrouchWasDetected()
        {
            return crouchStartTime > 0;
        }

        private float CalcAdjustedFinalJumpVelocity()
        {
            if (crouchLowTime == -1)
            {
                crouchLowTime = (crouchStartTime / 2);
            }

            float lowpointElapsed = Time.time - crouchLowTime;
            float velocity = lowpointElapsed <= 0 ? 0 : (baselineHeight - crouchLow) / lowpointElapsed;
            float scaled = velocity * jumpScaling;

            return scaled;
        }

        private void UpdateCrouchLowpoint()
        {
            if (HMDTransform.localPosition.y < crouchLow)
            {
                crouchLow = HMDTransform.localPosition.y;
                crouchLowTime = Time.time;
            }
        }

        private void CheckForCrouchStart(float dv)
        {
            if (dv < downThreshold)
            {
                crouchStartTime = Time.time;
            }
        }

        private void CheckForJumpEnd(float dv)
        {
            if (IsCloseToGround())
            {
                isJumping = false;
                crouchStartTime = 0;
                OnPlayerJumpEnded(SetPlayerJumpEvent(Vector3.zero));

                // Trick to reset playerPresence so player can jump again
                playerPresence.StartPhysicsFall(Vector3.zero);
            }
        }

        private bool IsCloseToGround()
        {
            Ray ray = new Ray(HMDTransform.position, -transform.up);
            RaycastHit rayCollidedWith;
            bool rayHit = Physics.Raycast(ray, out rayCollidedWith);

            if (rayHit && rayCollidedWith.point.y >= (transform.position.y))
            {
                return true;
            }
            return false;
        }

        private void UpdateBaseline(float dv)
        {
            if (Mathf.Abs(dv) < baselineUpdateSensitivity)
            {
                baselineHeight = HMDTransform.localPosition.y;
                playerPresence.StartPhysicsFall(Vector3.zero);
            }
        }

        private float UpdateHeightHistoryAndReturnVelocity(float headHeight)
        {
            // TODO - calculation of difference will be off around the y origin
            var deltaHeight = lastHeight - headHeight;
            lastHeight = headHeight;

            // update the history and get a sum
            float heightSum = 0;
            for (int i = 0; i < historyLen - 1; i++)
            {
                heightSum += historicVelocity[i];
                historicVelocity[i] = historicVelocity[i + 1];
            }

            heightSum += historicVelocity[historyLen - 1];
            historicVelocity[historyLen - 1] = deltaHeight;

            var scalingValue = -100;
            return (heightSum / historyLen) * scalingValue;
        }

        private void DoJump(float jumpVelocity)
        {
            Vector3 forward = GetPlayerForward() * jumpVelocity;

            Vector3 calcVelocity;
            calcVelocity.x = forwardFactor * forward.x;
            calcVelocity.y = upFactor * forward.y;
            calcVelocity.z = forwardFactor * forward.z;

            // offset .001 for discount in playerPresence
            playerPresence.StartPhysicsFall(new Vector3(0, 0.001f, 0));

            OnPlayerJumpStarted(SetPlayerJumpEvent(calcVelocity));

            playerPresence.GetComponent<Rigidbody>().velocity = calcVelocity;
        }

        private Vector3 GetPlayerForward()
        {
            Vector3 gaze = HMDTransform.forward;
            return new Vector3(gaze.x, 1, gaze.z);
        }


        private void InitClimbEvents(bool state)
        {
            // Listen for climb events
            var climbComponent = GetComponent<VRTK_PlayerClimb>();
            if (climbComponent)
            {
                if (state)
                {
                    climbComponent.PlayerClimbStarted += new PlayerClimbEventHandler(OnClimbStarted);
                    climbComponent.PlayerClimbEnded += new PlayerClimbEventHandler(OnClimbEnded);
                }
                else
                {
                    climbComponent.PlayerClimbStarted -= new PlayerClimbEventHandler(OnClimbStarted);
                    climbComponent.PlayerClimbEnded -= new PlayerClimbEventHandler(OnClimbEnded);
                }
            }
        }

        private void OnClimbStarted(object sender, PlayerClimbEventArgs e)
        {
            isClimbing = true;
        }

        private void OnClimbEnded(object sender, PlayerClimbEventArgs e)
        {
            isClimbing = false;
        }
    }
}
