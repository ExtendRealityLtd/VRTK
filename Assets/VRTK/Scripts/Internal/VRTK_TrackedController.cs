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

        private Coroutine enableControllerCoroutine = null;
        private GameObject aliasController;

        public virtual void OnControllerEnabled(VRTKTrackedControllerEventArgs e)
        {
            if (index < uint.MaxValue && ControllerEnabled != null)
            {
                ControllerEnabled(this, e);
            }
        }

        public virtual void OnControllerDisabled(VRTKTrackedControllerEventArgs e)
        {
            if (index < uint.MaxValue && ControllerDisabled != null)
            {
                ControllerDisabled(this, e);
            }
        }

        public virtual void OnControllerIndexChanged(VRTKTrackedControllerEventArgs e)
        {
            if (index < uint.MaxValue && ControllerIndexChanged != null)
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

        protected virtual void OnEnable()
        {
            aliasController = VRTK_DeviceFinder.GetScriptAliasController(gameObject);
            if (aliasController == null)
            {
                aliasController = gameObject;
            }

            if (enableControllerCoroutine != null)
            {
                StopCoroutine(enableControllerCoroutine);
            }
            enableControllerCoroutine = StartCoroutine(Enable());
        }

        protected virtual void OnDisable()
        {
            Invoke("Disable", 0f);
        }

        protected virtual void Disable()
        {
            if (enableControllerCoroutine != null)
            {
                StopCoroutine(enableControllerCoroutine);
            }

            OnControllerDisabled(SetEventPayload());
        }

        protected virtual void FixedUpdate()
        {
            VRTK_SDK_Bridge.ControllerProcessFixedUpdate(index);
        }

        protected virtual void Update()
        {
            uint checkIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (index < uint.MaxValue && checkIndex != index)
            {
                uint previousIndex = index;
                index = checkIndex;
                OnControllerIndexChanged(SetEventPayload(previousIndex));
            }

            VRTK_SDK_Bridge.ControllerProcessUpdate(index);

            if (aliasController != null && gameObject.activeInHierarchy && !aliasController.activeSelf)
            {
                aliasController.SetActive(true);
            }
        }

        protected virtual IEnumerator Enable()
        {
            yield return new WaitForEndOfFrame();

            while (!gameObject.activeInHierarchy)
            {
                yield return null;
            }

            index = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            OnControllerEnabled(SetEventPayload());
        }
    }
}