namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    /// <summary>
    /// Updates the position of the Camera Rig by rolling a grabbing the sides of a sphere collider's interior, like a hamster ball.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_HamsterBall` script on a scene GameObject containing a SphereCollider. Assign a CameraRig alias (using VRTK_SDKObjectAlias) at least one controller alias, 
    ///
    /// **Script Dependencies:**
    ///  * An optional Sphere Renderer should by on or on a child of the game object. It's important that there is no collider on the child.
    /// **Questions:**
    ///  *Should the ball reposition to the HMD when we're not rolling?
    ///  *Should there be an option to add a ground collision layer, so can roll, but you don't knock things over when you approach them?
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/045_CameraRig_HamsterBall` uses the `Trigger ON` event on the Controllers to Instantiate a joint. Pull the invisible joint downward to move forward. Try to knock over the objects.  
    /// </example>
    // 

    public class VRTK_HamsterBall : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("CameraRig alias. The parent of which will be moved when rolling the ball.")]
        [SerializeField]
        private Transform cameraRigAlias;
        [Tooltip("Left controller alias. Requires VRTK_ControllerEvents component.")]
        [SerializeField]
        private VRTK_ControllerEvents leftControllerAlias;
        [Tooltip("Left controller alias. Requires VRTK_ControllerEvents component.")]
        [SerializeField]
        private VRTK_ControllerEvents rightControllerAlias;
        [Header("Settings")]
        [Tooltip("The event used to initiate ball push movement.")]
        [SerializeField]
        private VRTK_ControllerEvents.ButtonAlias controllerEvent = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("The strength of force applied to the ball while pushing.")]
        [SerializeField]
        private float strength = 20f;
        [Tooltip("The strength of stopping force when a push is completed. Allows for a gradual, yet controllable stop.")]
        [SerializeField]
        private float brakeStrength = 2f;
        [Header("Optional for fading ball while idle")]
        [Tooltip("The sphere renderer used to fade in/out. Particle shaders recommended")]
        [SerializeField]
        private Renderer sphereRenderer;
        [Tooltip("The amount of time during inactivity to wait before fading the ball.")]
        [SerializeField]
        private float idleTime = 2f;
        [Tooltip("The amount of time it takes to fade the ball.")]
        [SerializeField]
        private float fadeTime = 1f;

        private Rigidbody RigidBody;
        private bool bIsRolling;
        private Rigidbody ConnectedBody;
        private SpringJoint Joint;
        private int nGrabbedCount;
        private float fIdleTimer;
        private bool bIsBallHidden;
        private Transform anchorTransform;
        private Transform bodyAnchorTransform;


        private void Start()
        {
            print(GetComponent<SphereCollider>().radius);
            RigidBody = GetComponent<Rigidbody>();

            // Subscribe to controller events
            if (leftControllerAlias != null)
            {
                leftControllerAlias.SubscribeToButtonAliasEvent(controllerEvent, true, DoPushBall);
                leftControllerAlias.SubscribeToButtonAliasEvent(controllerEvent, false, DoReleaseBall);
            }
            if (rightControllerAlias != null)
            {
                rightControllerAlias.SubscribeToButtonAliasEvent(controllerEvent, true, DoPushBall);
                rightControllerAlias.SubscribeToButtonAliasEvent(controllerEvent, false, DoReleaseBall);
            }
            else if (leftControllerAlias == null && rightControllerAlias == null)
            {
                Debug.LogError("At least one HamsterBall controller alias is required");
            }
            // Create temporary transforms for measurement of joint anchor distance, 
            // which scales spring value, preventing "bounciness."
            anchorTransform = new GameObject("Anchor").transform;
            anchorTransform.parent = transform;
            bodyAnchorTransform = new GameObject("BodyAnchor").transform;
            bodyAnchorTransform.parent = transform;
        }

        // Do what it takes to create the joint and begin rolling...
        private void DoPushBall(object sender, ControllerInteractionEventArgs e)
        {
            // Increment grab count. This will avoid erroneously destroying the joint when the 
            // inactive controller receives an OFF event.
            nGrabbedCount++;
            // Get Controller transform
            Transform controller = e.controllerReference.model.transform;
            // Get the near point on the ball where we're pointing
            Vector3 hitPosition = GetBallPointerPosition(e.controllerReference.model.transform);
            // State that we're rolling (so we don't try to brake)
            bIsRolling = true;
            // Reset the Idle Timer (used to fade the hamster ball renderer)
            ResetTimer();
            // Create the joint
            CreateJoint(e.controllerReference.model.gameObject, hitPosition);
        }

        private void DoReleaseBall(object sender, ControllerInteractionEventArgs e)
        {
            // Decrement the grab count
            nGrabbedCount--;
            // If niether controller has the ball, destroy the joint and state that we're 
            // not rolling (which will cause braking).
            if (nGrabbedCount < 1)
            {
                ReleaseJoint();
                bIsRolling = false;
            }
        }

        private void Update()
        {
            if (!bIsRolling)
            {
                // We're not rolling, so slow down and decerement IdleTimer
                RigidBody.AddForce(-brakeStrength * RigidBody.velocity);
                fIdleTimer = fIdleTimer - Time.deltaTime;
            }

            if (fIdleTimer < 0f && !bIsBallHidden)
            {
                // Idle time is up, hide the ball.
                bIsBallHidden = true;
                StartCoroutine(FadeBall(true));
            }

            if (Joint != null)
            {
                // We have a joint. so..
                // Parent our temp transform to the controller
                bodyAnchorTransform.parent = Joint.connectedBody.transform;
                // Get the position the controller's pointing to
                Vector3 hitPosition = GetBallPointerPosition(Joint.connectedBody.transform);
                // set the Joint's connected anchor to that position (in controller's local space)
                Joint.connectedAnchor = Joint.connectedBody.transform.InverseTransformPoint(hitPosition);
                // Position the temp transform
                bodyAnchorTransform.localPosition = Joint.connectedAnchor;
                // Scale spring strength based on the distance between the temp transforms. 
                // This prevents boingy boingy stuff from happening when the ball reaches it's goal position
                Joint.spring = Mathf.Lerp(strength, 0, Mathf.InverseLerp(5, 0, Vector3.Distance(anchorTransform.position, bodyAnchorTransform.position)));
            }
        }

        private void LateUpdate()
        {
            // Set the camera rig position to the bottom of the ball.
            cameraRigAlias.parent.position = transform.position + Vector3.down * GetComponent<SphereCollider>().radius;
        }

        private Vector3 GetBallPointerPosition(Transform origin)
        {
            // We want to find the point on the outside ball. So we reach out of the ball with our test point.
            float reach = GetComponent<SphereCollider>().radius * 2;
            // Get and return the closest point on the ball.
            Vector3 resultingPoint = GetComponent<SphereCollider>().ClosestPoint(origin.position + origin.forward * reach);
            return resultingPoint;
        }

        private void CreateJoint(GameObject controller, Vector3 anchor)
        {
            // Make sure the controller has a rigidbody, for the purpose of anchoring the joint.
            Rigidbody controllerBody;
            if (!controller.GetComponent<Rigidbody>())
            {
                controller.gameObject.AddComponent<Rigidbody>();
            }
            controllerBody = controller.gameObject.GetComponent<Rigidbody>();
            controllerBody.useGravity = false;
            controllerBody.isKinematic = true;

            // Make a joint if we don't have one already
            if (Joint == null)
                Joint = gameObject.AddComponent<SpringJoint>();
            // Joint set up
            Joint.spring = strength;
            Joint.autoConfigureConnectedAnchor = false;
            // Connect the controller and set the anchor position in controller's local space.
            Joint.connectedBody = controllerBody;
            Joint.connectedAnchor = controllerBody.transform.InverseTransformPoint(anchor);
            // Set the the local anchor in the ball's local space.
            Joint.anchor = RigidBody.transform.InverseTransformPoint(anchor);
            anchorTransform.position = anchor;
        }

        private void ReleaseJoint()
        {
            // Kill the joint. Sadly, there's no way I could find to simply disable the joint.
            // TODO: experiment with setting a super high MinDistance rather than destroying HamsterBall Joint on release.
            if (Joint != null)
            {
                Destroy(Joint);
                Joint = null;
            }
        }

        private void ResetTimer()
        {
            // Reset the idle timer and show the ball if necessary.
            fIdleTimer = idleTime;
            if (bIsBallHidden)
            {
                bIsBallHidden = false;
                StartCoroutine(FadeBall(false));
            }
        }

        // Fade the ball in or out. 
        // TODO: Test if fading the hamster ball works on other shaders.
        private IEnumerator FadeBall(bool fadeOut)
        {
            if (sphereRenderer != null)
            {
                // Get the ball color.
                Color color = sphereRenderer.material.GetColor("_TintColor");
                float fadeValue;
                if (fadeOut)
                {
                    // We're fading out, so loop through until the alpha value is 0.
                    fadeValue = fadeTime;
                    while (fadeValue > 0)
                    {
                        fadeValue -= Time.deltaTime;
                        color.a = Mathf.Clamp01(fadeValue / fadeTime);
                        sphereRenderer.material.SetColor("_TintColor", color);
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    // We're fading out, so loop through until the alpha value is 1.
                    fadeValue = 0;
                    while (fadeValue < 1)
                    {
                        fadeValue += Time.deltaTime;
                        color.a = 0.0f + Mathf.Clamp01(fadeValue / fadeTime);
                        sphereRenderer.material.SetColor("_TintColor", color);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
    }
}
