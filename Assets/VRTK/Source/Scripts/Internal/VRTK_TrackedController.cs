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
        public event VRTKTrackedControllerEventHandler ControllerModelAvailable;

        protected GameObject aliasController;
        protected SDK_BaseController.ControllerType controllerType = SDK_BaseController.ControllerType.Undefined;
        protected bool controllerModelWaitSubscribed = false;
        protected Coroutine emitControllerEnabledRoutine = null;
        protected Coroutine emitControllerModelAvailableRoutine = null;

        protected VRTK_ControllerReference controllerReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference(index);
            }
        }

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

        public virtual void OnControllerModelAvailable(VRTKTrackedControllerEventArgs e)
        {
            if (ControllerModelAvailable != null)
            {
                ControllerModelAvailable(this, e);
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
            SetControllerType();
            StartEmitControllerEnabledAtEndOfFrame();
            ManageControllerModelListeners(true);
        }

        protected virtual void OnDisable()
        {
            CancelCoroutines();
            index = uint.MaxValue;
            ManageControllerModelListeners(false);
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
                //If the controller model listeners have already been registered, then make sure to unsub and sub when index changes.
                if (controllerModelWaitSubscribed)
                {
                    ManageControllerModelListeners(false);
                    ManageControllerModelListeners(true);
                }
                OnControllerIndexChanged(SetEventPayload(previousIndex));
                SetControllerType();
            }

            VRTK_SDK_Bridge.ControllerProcessUpdate(VRTK_ControllerReference.GetControllerReference(index));

            if (aliasController != null && gameObject.activeInHierarchy && !aliasController.activeSelf)
            {
                aliasController.SetActive(true);
            }
        }

        protected virtual void ManageLeftControllerListener(bool register, VRTKSDKBaseControllerEventHandler callbackMethod)
        {
            if (register)
            {
                VRTK_SDK_Bridge.GetControllerSDK().LeftControllerModelReady += callbackMethod;
            }
            else
            {
                VRTK_SDK_Bridge.GetControllerSDK().LeftControllerModelReady -= callbackMethod;
            }
        }

        protected virtual void ManageRightControllerListener(bool register, VRTKSDKBaseControllerEventHandler callbackMethod)
        {
            if (register)
            {
                VRTK_SDK_Bridge.GetControllerSDK().RightControllerModelReady += callbackMethod;
            }
            else
            {
                VRTK_SDK_Bridge.GetControllerSDK().RightControllerModelReady -= callbackMethod;
            }
        }

        protected virtual void RegisterHandControllerListener(bool register, SDK_BaseController.ControllerHand givenHand)
        {
            switch (givenHand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    ManageLeftControllerListener(register, ControllerModelReady);
                    break;
                case SDK_BaseController.ControllerHand.Right:
                    ManageRightControllerListener(register, ControllerModelReady);
                    break;
            }
            controllerModelWaitSubscribed = register;
        }

        protected virtual void ManageControllerModelListener(bool register, SDK_BaseController.ControllerHand givenHand)
        {
            if (VRTK_SDK_Bridge.WaitForControllerModel(givenHand))
            {
                RegisterHandControllerListener(register, givenHand);
            }
            else
            {
                if (register)
                {
                    StartEmitControllerModelReadyAtEndOfFrame();
                }
                else if (controllerModelWaitSubscribed)
                {
                    RegisterHandControllerListener(register, givenHand);
                }
            }
        }

        protected virtual void ManageControllerModelListeners(bool register)
        {
            ManageControllerModelListener(register, VRTK_DeviceFinder.GetControllerHand(gameObject));
        }

        protected virtual void SetControllerType()
        {
            controllerType = (controllerReference != null ? VRTK_DeviceFinder.GetCurrentControllerType(controllerReference) : SDK_BaseController.ControllerType.Undefined);
        }

        protected virtual void StartEmitControllerEnabledAtEndOfFrame()
        {
            if (gameObject.activeInHierarchy)
            {
                emitControllerEnabledRoutine = StartCoroutine(EmitControllerEnabledAtEndOfFrame());
            }
        }

        protected virtual IEnumerator EmitControllerEnabledAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            OnControllerEnabled(SetEventPayload());
        }

        protected virtual void ControllerModelReady(object sender, VRTKSDKBaseControllerEventArgs e)
        {
            SetControllerType();
            if (e.controllerReference == null || controllerReference == e.controllerReference)
            {
                StartEmitControllerModelReadyAtEndOfFrame();
            }
        }

        protected virtual void StartEmitControllerModelReadyAtEndOfFrame()
        {
            if (gameObject != null && gameObject.activeInHierarchy)
            {
                emitControllerModelAvailableRoutine = StartCoroutine(EmitControllerModelReadyAtEndOfFrame());
            }
        }

        protected virtual IEnumerator EmitControllerModelReadyAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            OnControllerModelAvailable(SetEventPayload());
        }

        protected virtual void CancelCoroutines()
        {
            if (emitControllerModelAvailableRoutine != null)
            {
                StopCoroutine(emitControllerModelAvailableRoutine);
            }

            if (emitControllerEnabledRoutine != null)
            {
                StopCoroutine(emitControllerEnabledRoutine);
            }
        }
    }
}