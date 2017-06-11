// Base Object Control Action|ObjectControlActions|25000
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Object Control Action script is an abstract class that all object control action scripts inherit.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseObjectControlAction : MonoBehaviour
    {
        public enum AxisListeners
        {
            XAxisChanged,
            YAxisChanged
        }

        [Tooltip("The Object Control script to receive axis change events from.")]
        public VRTK_ObjectControl objectControlScript;
        [Tooltip("Determines which Object Control Axis event to listen to.")]
        public AxisListeners listenOnAxisChange;

        protected Collider centerCollider;
        protected Vector3 colliderCenter = Vector3.zero;
        protected float colliderRadius = 0f;
        protected float colliderHeight = 0f;
        protected Transform controlledTransform;
        protected Transform playArea;

        protected abstract void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive);

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            if (objectControlScript)
            {
                switch (listenOnAxisChange)
                {
                    case AxisListeners.XAxisChanged:
                        objectControlScript.XAxisChanged += AxisChanged;
                        break;
                    case AxisListeners.YAxisChanged:
                        objectControlScript.YAxisChanged += AxisChanged;
                        break;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (objectControlScript)
            {
                switch (listenOnAxisChange)
                {
                    case AxisListeners.XAxisChanged:
                        objectControlScript.XAxisChanged -= AxisChanged;
                        break;
                    case AxisListeners.YAxisChanged:
                        objectControlScript.YAxisChanged -= AxisChanged;
                        break;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void AxisChanged(object sender, ObjectControlEventArgs e)
        {
            Process(e.controlledGameObject, e.directionDevice, e.axisDirection, e.axis, e.deadzone, e.currentlyFalling, e.modifierActive);
        }

        protected virtual void RotateAroundPlayer(GameObject controlledGameObject, float angle)
        {
            Vector3 objectCenter = GetObjectCenter(controlledGameObject.transform);
            Vector3 objectPosition = controlledGameObject.transform.TransformPoint(objectCenter);
            controlledGameObject.transform.Rotate(Vector3.up, angle);
            objectPosition -= controlledGameObject.transform.TransformPoint(objectCenter);
            controlledGameObject.transform.position += objectPosition;
        }

        protected virtual void Blink(float blinkSpeed)
        {
            if (blinkSpeed > 0f)
            {
                VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
                ReleaseBlink(blinkSpeed);
            }
        }

        protected virtual void ReleaseBlink(float blinkSpeed)
        {
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, blinkSpeed);
        }

        protected virtual Vector3 GetObjectCenter(Transform checkObject)
        {
            if (centerCollider == null || checkObject != controlledTransform)
            {
                controlledTransform = checkObject;

                if (checkObject == playArea)
                {
                    CapsuleCollider playAreaCollider = playArea.GetComponentInChildren<CapsuleCollider>();
                    centerCollider = playAreaCollider;
                    if (playAreaCollider != null)
                    {
                        colliderRadius = playAreaCollider.radius;
                        colliderHeight = playAreaCollider.height;
                        colliderCenter = playAreaCollider.center;
                    }
                    else
                    {
                        VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PlayArea", "CapsuleCollider", "the same or child"));
                    }
                }
                else
                {
                    centerCollider = checkObject.GetComponentInChildren<Collider>();
                    if (centerCollider == null)
                    {
                        VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "CheckObject", "Collider", "the same or child"));
                    }
                    colliderRadius = 0.1f;
                    colliderHeight = 0.1f;
                }
            }

            return colliderCenter;
        }

        protected virtual int GetAxisDirection(float axis)
        {
            int axisDirection = 0;
            if (axis < 0)
            {
                axisDirection = -1;
            }
            else if (axis > 0)
            {
                axisDirection = 1;
            }

            return axisDirection;
        }

        protected virtual bool CanMove(VRTK_BodyPhysics givenBodyPhysics, Vector3 currentPosition, Vector3 proposedPosition)
        {
            if (givenBodyPhysics == null)
            {
                return true;
            }

            Vector3 proposedDirection = (proposedPosition - currentPosition).normalized;
            float distance = Vector3.Distance(currentPosition, proposedPosition);
            return !givenBodyPhysics.SweepCollision(proposedDirection, distance);
        }
    }
}