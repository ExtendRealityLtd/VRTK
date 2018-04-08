namespace VRTK.Examples.Utilities
{
    using UnityEngine;

    public class VRTKExample_ObjectListToggle : MonoBehaviour
    {
        public GameObject[] objects = new GameObject[0];
        public GameObject[] retoggle = new GameObject[0];
        public VRTK_ControllerEvents controllerEvents;
        public VRTK_ControllerEvents.ButtonAlias toggleButton = VRTK_ControllerEvents.ButtonAlias.ButtonTwoPress;

        protected int currentIndex;

        protected virtual void OnEnable()
        {
            currentIndex = 0;
            if (controllerEvents != null)
            {
                controllerEvents.SubscribeToButtonAliasEvent(toggleButton, false, ButtonPressed);
            }
            ToggleObjects();
        }

        protected virtual void OnDisable()
        {
            if (controllerEvents != null)
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(toggleButton, false, ButtonPressed);
            }
        }

        protected virtual void ButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            currentIndex++;
            if (currentIndex >= objects.Length)
            {
                currentIndex = 0;
            }
            ToggleObjects();
        }

        protected virtual void ToggleObjects()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null && i != currentIndex)
                {
                    objects[i].SetActive(false);
                }
            }

            for (int i = 0; i < retoggle.Length; i++)
            {
                if (retoggle[i] != null && retoggle[i].activeInHierarchy)
                {
                    retoggle[i].SetActive(false);
                }
            }

            Invoke("ToggleOn", 0f);
            Invoke("RetoggleOn", 0f);
        }

        protected virtual void ToggleOn()
        {
            objects[currentIndex].SetActive(true);
        }

        protected virtual void RetoggleOn()
        {
            for (int i = 0; i < retoggle.Length; i++)
            {
                if (retoggle[i] != null && !retoggle[i].activeInHierarchy)
                {
                    retoggle[i].SetActive(true);
                }
            }
        }
    }
}