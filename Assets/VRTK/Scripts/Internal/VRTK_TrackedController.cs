namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public struct VRTKTrackedControllerEventArgs
    {
        public uint currentIndex;
        public uint previousIndex;
    }

    public delegate void VRTKTrackedControllerEventHandler(object sender, VRTKTrackedControllerEventArgs e);

    public class VRTK_TrackedController : MonoBehaviour
    {
        public uint index = uint.MaxValue;

        public event VRTKTrackedControllerEventHandler ControllerEnabled;
        public event VRTKTrackedControllerEventHandler ControllerDisabled;
        public event VRTKTrackedControllerEventHandler ControllerIndexChanged;
        public event VRTKTrackedControllerEventHandler ControllerTypeAvailable;

        protected GameObject aliasController;
        protected Coroutine attemptFindControllerTypeRoutine;
        protected Coroutine attemptEmitFoundControllerTypeRoutine;
        protected int findControllerTypeAttempts = 30;
        protected float findControllerTypeAttemptsDelay = 0.1f;
        protected SDK_BaseController.ControllerType controllerType = SDK_BaseController.ControllerType.Undefined;
        protected float controllerModelInitTime = 0.5f;

        public virtual void OnControllerEnabled(VRTKTrackedControllerEventArgs e)
        {
            if (ControllerEnabled != null)
            {
                ControllerEnabled(this, e);
            }
        }

        public virtual void OnControllerDisabled(VRTKTrackedControllerEventArgs e)
        {
            if (ControllerDisabled != null)
            {
                ControllerDisabled(this, e);
            }
        }

        public virtual void OnControllerIndexChanged(VRTKTrackedControllerEventArgs e)
        {
            if (ControllerIndexChanged != null)
            {
                ControllerIndexChanged(this, e);
            }
        }

        public virtual void OnControllerTypeAvailable(VRTKTrackedControllerEventArgs e)
        {
            if (ControllerTypeAvailable != null)
            {
                ControllerTypeAvailable(this, e);
            }
        }

        public virtual SDK_BaseController.ControllerType GetControllerType()
        {
            return controllerType;
        }

        protected virtual VRTKTrackedControllerEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
        {
            VRTKTrackedControllerEventArgs e;
            e.currentIndex = index;
            e.previousIndex = previousIndex;
            return e;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            aliasController = VRTK_DeviceFinder.GetScriptAliasController(gameObject);
            if (aliasController == null)
            {
                aliasController = gameObject;
            }

            index = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            OnControllerEnabled(SetEventPayload());
            attemptFindControllerTypeRoutine = StartCoroutine(AttemptFindControllerType(findControllerTypeAttempts, findControllerTypeAttemptsDelay));
        }

        protected virtual void OnDisable()
        {
            if (attemptFindControllerTypeRoutine != null)
            {
                StopCoroutine(attemptFindControllerTypeRoutine);
            }

            if (attemptEmitFoundControllerTypeRoutine != null)
            {
                StopCoroutine(attemptEmitFoundControllerTypeRoutine);
            }

            OnControllerDisabled(SetEventPayload());
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void FixedUpdate()
        {
            VRTK_SDK_Bridge.ControllerProcessFixedUpdate(VRTK_ControllerReference.GetControllerReference(index));
        }

        protected virtual void Update()
        {
            uint checkIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (checkIndex != index)
            {
                uint previousIndex = index;
                index = checkIndex;
                OnControllerIndexChanged(SetEventPayload(previousIndex));
            }

            VRTK_SDK_Bridge.ControllerProcessUpdate(VRTK_ControllerReference.GetControllerReference(index));

            if (aliasController != null && gameObject.activeInHierarchy && !aliasController.activeSelf)
            {
                aliasController.SetActive(true);
            }
        }

        protected virtual IEnumerator AttemptFindControllerType(int attempts, float delay)
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(delay);
            controllerType = VRTK_DeviceFinder.GetCurrentControllerType(VRTK_ControllerReference.GetControllerReference(index));
            while (controllerType == SDK_BaseController.ControllerType.Undefined && attempts > 0)
            {
                controllerType = VRTK_DeviceFinder.GetCurrentControllerType(VRTK_ControllerReference.GetControllerReference(index));
                attempts--;
                yield return delayInstruction;
            }

            if (controllerType != SDK_BaseController.ControllerType.Undefined)
            {
                attemptEmitFoundControllerTypeRoutine = StartCoroutine(EmitControllerTypeAvailableAfterPause());
            }
        }

        protected virtual IEnumerator EmitControllerTypeAvailableAfterPause()
        {
            //pause for a short time to allow models to prep
            yield return new WaitForSeconds(controllerModelInitTime);
            OnControllerTypeAvailable(SetEventPayload());
        }
    }
}