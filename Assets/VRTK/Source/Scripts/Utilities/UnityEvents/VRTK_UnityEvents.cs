namespace VRTK.UnityEventHelper
{
    using UnityEngine;

    public abstract class VRTK_UnityEvents<T> : MonoBehaviour where T : Component
    {
        private T component;

        protected abstract void AddListeners(T component);
        protected abstract void RemoveListeners(T component);

        protected virtual void OnEnable()
        {
            component = GetComponent<T>();

            if (component != null)
            {
                AddListeners(component);
            }
            else
            {
                string eventsScriptName = GetType().Name;
                string scriptName = component.GetType().Name;
                VRTK_Logger.Error(string.Format("The {0} script requires to be attached to a GameObject that contains a {1} script.", eventsScriptName, scriptName));
            }
        }

        protected virtual void OnDisable()
        {
            if (component != null)
            {
                RemoveListeners(component);
            }
        }
    }
}