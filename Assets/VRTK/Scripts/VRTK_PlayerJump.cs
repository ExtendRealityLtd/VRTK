// Player Jump|Scripts|0240
namespace VRTK
{
    using UnityEngine;
    using System.IO;
    using System;

    /// <summary>
    /// The player jump provides a jump behavior by tracking HMD world position over time, calculating velocities of a crouch and a spring motion, and adding velocity to the attached rigidbody on playerpresence.  It should be attached to the `[CameraRig]` object.
    /// </summary>
    /// <remarks>
    /// This relies heavily on the status of the playerPresence, and will probably have to be updated as that changes.  In particular, playerPresence sometimes deactivates physics, which can leave this in a stuck state
    /// the checkForJumpEnd method sends a startPhysicsFall with a velocity of 0 to prevent this issue ( cf. HeightAdjustTeleport )
    /// 
    /// Jump scaling for non-fixed jump factors is calculated in CalcAdjustedFinalJumpVelocity
    /// </remarks>
    /// <example>
    /// VRTK/Examples/039_CameraRig_PlayerJump
    /// </example>
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
        public float upThreshold = 1.0f;

        [Tooltip("Time in seconds to reset")]
        public float timeout = 1.0f;

        [Tooltip("Fixed vs dynamic jump power - uncheck to scale based on player jump intensity")]
        public bool jumpFixed = false;

        [Tooltip("Amount to magnify the player jump intensity when jump scaling not fixed - modifies Up Factor and Forward Factor")]
        public float jumpScaling = 3.0f;

        #endregion

        #region ### Private Properties ###

        // deviations less than this will not be counted against baseline establishment - magic number alert!
        private float baselineUpdateSensitivity = 0.5f;

        // this is the baseline height recorded from the HMD over time
        private float baselineHeight = 0;

        // this is a time since starting crouch.  Zero if not in jump state
        private float crouchStartTime = 0;

        // player jumping state as understood by this script
        private bool jumping = false;

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

        private Transform HMDTransform;
        private VRTK_PlayerPresence playerPresence;

        #endregion

        #region ### private logging vars ###

        // set to writable directory eg. C:/Users/Public/Documents/Unity Projects/MyProject/Assets
        private string loggingTarget = "";
        private StreamWriter fileWriter;
        private bool logging = false;
        private bool debugging = true;

        #endregion

        private void Awake()
        {
            // Required Component: VRTK_PlayerPresence
            playerPresence = GetComponent<VRTK_PlayerPresence>();
            if (!playerPresence)
            {
                playerPresence = gameObject.AddComponent<VRTK_PlayerPresence>();
            }
            playerPresence.SetFallingPhysicsOnlyParams(true);
            playerPresence.StartPhysicsFall(Vector3.zero);

            HMDTransform = VRTK_DeviceFinder.HeadsetTransform();

            historicVelocity = new float[historyLen];
        }

        private void Start()
        {
            baselineHeight = HMDTransform.position.y;
            // TODO - may want a cooldown timer to prevent spurious jumps when headset is first put on
            crouchLowTime = -1;

            if (logging)
            {
                InitLogging();
            }
        }

        private void Update()
        {
            CheckPosture();
        }

        private void CheckPosture()
        {
            float jumpVelocity = 0;

            var dv = UpdateHeightHistoryAndReturnVelocity(HMDTransform.position.y);

            if (jumping)
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
                        if (VelocityChangeAboveThreshold(dv))
                        {
                            // if head position exceeds the baseline
                            if (HMDHeightExceedsBaseline())
                            {
                                jumping = true;

                                jumpVelocity = jumpFixed ? 1 : CalcAdjustedFinalJumpVelocity();

                                DoJump( jumpVelocity );
                            }
                        }
                    }
                    else
                    {
                        crouchStartTime = 0;
                        crouchLowTime = Time.time;
                        crouchLow = baselineHeight + 1000000; // TODO probably there is a better way
                        jumping = false;
                    }
                }
                else
                {
                    jumping = false;
                }
            }
            if (debugging)
            {
                //DebugPosture( dv, jumpVelocity );
            }
        }

        private bool HMDHeightExceedsBaseline()
        {
            return HMDTransform.position.y > baselineHeight;
        }

        private bool VelocityChangeAboveThreshold( float dv )
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
            if ( crouchLowTime == -1)
            {
                crouchLowTime = (crouchStartTime / 2);
            }

            float lowpointElapsed = Time.time - crouchLowTime;

            // it may be that the time component should be deemphasized to exaggerated the height change vs the time delay
            float velocity = lowpointElapsed <= 0 ? 0 : (baselineHeight - crouchLow) / lowpointElapsed;

            // This calc attempts a nicer scaling per player effort.
            // could possibly be interpolated between the up and down threshold values or something to make it have some sense
            float scaled = velocity * jumpScaling;

            if (debugging)
            {
                //DebugHelper("lpe: " + lowpointElapsed + " DeltaP: " + (baselineHeight - crouchLow) + " crouchLow: " + crouchLow + " calculated: " + velocity + " scaled: " + scaled);
                //DebugHelper("lpe: " + lowpointElapsed.ToString("0.000") + " DeltaP: " + (baselineHeight - crouchLow).ToString("0.000") + " calculated: " + velocity.ToString("0.000") + " scaled: " + scaled.ToString("0.000"));
            }

            return scaled;
        }

        private void UpdateCrouchLowpoint()
        {
            if (HMDTransform.position.y < crouchLow)
            {
                crouchLow = HMDTransform.position.y;
                crouchLowTime = Time.time;

                //DebugHelper(" Update CrouchLow: " + crouchLow + " CLT: " + crouchLowTime);
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
            if (Mathf.Abs(dv) < baselineUpdateSensitivity)
            {
                jumping = false;
                crouchStartTime = 0;
                //DebugHelper("Ending jump - isFalling = false");
                // Trick to reset playerPresence so player can jump again
                playerPresence.StartPhysicsFall(Vector3.zero);
            }
        }

        private void UpdateBaseline(float dv)
        {
            if (Mathf.Abs(dv) < baselineUpdateSensitivity)
            {
                baselineHeight = HMDTransform.position.y;
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

            // Velocity, scaled for convenience, signreversed for sanity or lazyness or something
            return (heightSum / historyLen) * 100 * -1;
        }

        private void DoJump( float jumpVelocity )
        {
            Vector3 forward = GetPlayerForward() * jumpVelocity;

            Vector3 calcVelocity;
            calcVelocity.x = forwardFactor * forward.x;
            calcVelocity.y = upFactor * forward.y;
            calcVelocity.z = forwardFactor * forward.z;

            //DebugHelper("JUMP EVENT: " + calcVelocity.ToString());

            playerPresence.StartPhysicsFall(Vector3.zero);

            playerPresence.GetComponent<Rigidbody>().velocity = calcVelocity;
        }

        private Vector3 GetPlayerForward()
        {
            // necessaries for triangulation

            //var headPos = VRTK_DeviceFinder.HeadsetTransform().position;
            //var rhand = VRTK_DeviceFinder.GetControllerRightHand().transform.position;
            //var lhand = VRTK_DeviceFinder.GetControllerLeftHand().transform.position;

            // these two numbers shall always add to 1.
            // float xAmount = 0.5f;
            // float zAmount = 0.5f;

            // OR 

            // a simpler method :) - assumes an X/Z plane
            // guess I should normalize this.?.
            Vector3 gaze = HMDTransform.forward;
            return new Vector3(gaze.x, 1, gaze.z);
        }

        private void OnApplicationQuit()
        {
            QuitLogging();
        }

        #region ### Private Logging and Debugging Methods ###

        private void InitLogging()
        {

            string fileName = loggingTarget + "/" + "CurrentLog_" + System.DateTime.Now.Millisecond + ".txt";
            fileWriter = File.CreateText(fileName);
        }

        private void QuitLogging()
        {
            if (logging && fileWriter != null)
            {
                fileWriter.Close();
            }
        }

        private void DebugPosture( float dv, float jumpVelocity )
        {
            string pw = (crouchStartTime > 0 && Time.time - crouchStartTime < timeout) ? "P" : " ";
            string pj = crouchStartTime > 0 ? "S" : " ";
            string wt = (dv > upThreshold) ? "W" : " ";
            string ju = jumping ? "J" : " ";

            string flags = " " + pw + " " + pj + " " + wt + " " + ju + " ";

            string dataOut = "Time: " + Time.time.ToString("0.000") + "  :: Head: " + HMDTransform.position.ToString() + "  :: dVelocity " + dv.ToString("0.0000") + "  :: jVelocity " + jumpVelocity.ToString("0.0000") + "  :: baseline: " + baselineHeight.ToString("0.0000") + " " + flags + " <::";

            DebugHelper(dataOut);
        }

        private void DebugHelper(string data)
        {
            Debug.Log(data);

            if (logging && fileWriter != null)
            {
                fileWriter.WriteLine(data);
            }
        }

        #endregion
    }
}