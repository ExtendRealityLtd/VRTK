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

        private EventSystem previousEventSystem;
        private VRTK_VRInputModule vrInputModule;

        protected override void OnEnable()
        {
            previousEventSystem = current;
            if (previousEventSystem != null)
            {
                previousEventSystem.enabled = false;
                CopyValuesFrom(previousEventSystem, this);
            }

            vrInputModule = gameObject.AddComponent<VRTK_VRInputModule>();
            base.OnEnable();
            SetEventSystemOfBaseInputModulesTo(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Destroy(vrInputModule);

            if (previousEventSystem != null)
            {
                previousEventSystem.enabled = true;
                CopyValuesFrom(this, previousEventSystem);
                SetEventSystemOfBaseInputModulesTo(previousEventSystem);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (current == this)
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

            eventSystem.UpdateModules();
        }
    }
}