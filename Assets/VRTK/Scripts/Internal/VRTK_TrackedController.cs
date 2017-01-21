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
        public uint index;

        public event VRTKTrackedControllerEventHandler ControllerEnabled;
        public event VRTKTrackedControllerEventHandler ControllerDisabled;
        public event VRTKTrackedControllerEventHandler ControllerIndexChanged;

        private uint currentIndex = uint.MaxValue;
        private Coroutine enableControllerCoroutine = null;
        private GameObject aliasController;

        public virtual void OnControllerEnabled(VRTKTrackedControllerEventArgs e)
        {
            if (currentIndex < uint.MaxValue && ControllerEnabled != null)
            {
                ControllerEnabled(this, e);
            }
        }

        public virtual void OnControllerDisabled(VRTKTrackedControllerEventArgs e)
        {
            if (currentIndex < uint.MaxValue && ControllerDisabled != null)
            {
                ControllerDisabled(this, e);
            }
        }

        public virtual void OnControllerIndexChanged(VRTKTrackedControllerEventArgs e)
        {
            if (currentIndex < uint.MaxValue && ControllerIndexChanged != null)
            {
                ControllerIndexChanged(this, e);
            }
        }

        private VRTKTrackedControllerEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
        {
            VRTKTrackedControllerEventArgs e;
            e.currentIndex = currentIndex;
            e.previousIndex = previousIndex;
            return e;
        }

        protected virtual void OnEnable()
        {
            aliasController = VRTK_DeviceFinder.GetScriptAliasController(gameObject);

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

        protected virtual void Update()
        {
            var checkIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (currentIndex < uint.MaxValue && checkIndex != currentIndex)
            {
                var previousIndex = currentIndex;
                currentIndex = checkIndex;
                OnControllerIndexChanged(SetEventPayload(previousIndex));
            }

            VRTK_SDK_Bridge.ControllerProcessUpdate(currentIndex);

            if (aliasController && gameObject.activeInHierarchy && !aliasController.activeSelf)
            {
                aliasController.SetActive(true);
            }
        }

        private IEnumerator Enable()
        {
            yield return new WaitForEndOfFrame();

            while (!gameObject.activeInHierarchy)
            {
                yield return null;
            }

            currentIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            OnControllerEnabled(SetEventPayload());
        }
    }
}