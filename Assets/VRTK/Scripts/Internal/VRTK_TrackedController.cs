namespace VRTK
{
    using UnityEngine;

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

        protected GameObject aliasController;

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
        }

        protected virtual void OnDisable()
        {
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
    }
}