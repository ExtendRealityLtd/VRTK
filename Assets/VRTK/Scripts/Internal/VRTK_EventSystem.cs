namespace VRTK
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine.EventSystems;

    public class VRTK_EventSystem : EventSystem
    {
        private static readonly FieldInfo[] EVENT_SYSTEM_FIELD_INFOS = typeof(EventSystem).GetFields(BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo[] EVENT_SYSTEM_PROPERTY_INFOS = typeof(EventSystem).GetProperties(BindingFlags.Public | BindingFlags.Instance).Except(new[] { typeof(EventSystem).GetProperty("enabled") }).ToArray();
        private static readonly FieldInfo BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD_INFO = typeof(BaseInputModule).GetField("m_EventSystem", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<VRTK_VRInputModule> vrInputModules = new List<VRTK_VRInputModule>();

        public void RegisterVRInputModule(VRTK_VRInputModule vrInputModule)
        {
            vrInputModules.Add(vrInputModule);
        }

        public void UnregisterVRInputModule(VRTK_VRInputModule vrInputModule)
        {
            vrInputModules.Remove(vrInputModule);
        }

        protected override void OnEnable()
        {
            EventSystem currentEventSystem = current;
            if (currentEventSystem == null)
            {
                return;
            }

            currentEventSystem.enabled = false;

            CopyValuesFrom(currentEventSystem, this);
            Destroy(currentEventSystem);

            var vrInputModule = gameObject.AddComponent<VRTK_VRInputModule>();
            RegisterVRInputModule(vrInputModule);
            vrInputModule.Initialise();

            SetEventSystemOfBaseInputModulesTo(this);
            UpdateModules();

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var vrInputModule = GetComponent<VRTK_VRInputModule>();
            if (vrInputModule)
            {
                UnregisterVRInputModule(vrInputModule);
                Destroy(vrInputModule);
            }

            enabled = false;

            var eventSystem = gameObject.AddComponent<EventSystem>();
            CopyValuesFrom(this, eventSystem);
            Destroy(this);

            SetEventSystemOfBaseInputModulesTo(eventSystem);
            eventSystem.UpdateModules();
        }

        protected override void Update()
        {
            base.Update();

            if (current != this)
            {
                return;
            }

            foreach (VRTK_VRInputModule vrInputModule in vrInputModules)
            {
                vrInputModule.Process();
            }
        }

#if UNITY_5_5_OR_NEWER
        protected override void OnApplicationFocus(bool hasFocus)
        {
            //Don't call the base implementation because it will set a pause flag for this EventSystem
        }
#endif

        private static void CopyValuesFrom(EventSystem fromEventSystem, EventSystem toEventSystem)
        {
            foreach (FieldInfo fieldInfo in EVENT_SYSTEM_FIELD_INFOS)
            {
                fieldInfo.SetValue(toEventSystem, fieldInfo.GetValue(fromEventSystem));
            }

            foreach (PropertyInfo propertyInfo in EVENT_SYSTEM_PROPERTY_INFOS)
            {
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(toEventSystem, propertyInfo.GetValue(fromEventSystem, null), null);
                }
            }
        }

        private static void SetEventSystemOfBaseInputModulesTo(EventSystem eventSystem)
        {
            //BaseInputModule has a private field referencing the current EventSystem
            //this field is set in BaseInputModule.OnEnable only
            //it's used in BaseInputModule.OnEnable and BaseInputModule.OnDisable to call EventSystem.UpdateModules
            //this means we could just disable and enable every enabled BaseInputModule to fix that reference
            //
            //but the StandaloneInputModule (which is added by default when adding an EventSystem in the Editor) requires EventSystem
            //which means we can't correctly destroy the old EventSystem first and then add our own one
            //we also want to leave the existing EventSystem as is, so it can be used again whenever VRTK_UIPointer.RemoveEventSystem is called
            foreach (BaseInputModule module in FindObjectsOfType<BaseInputModule>())
            {
                BASE_INPUT_MODULE_EVENT_SYSTEM_FIELD_INFO.SetValue(module, eventSystem);
            }
        }
    }
}