namespace VRTK.Examples
{
    using UnityEngine;
    using Controllables;
    using Controllables.PhysicsBased;
    using Controllables.ArtificialBased;

    public class PusherStickyToggle : MonoBehaviour
    {
        public VRTK_BaseControllable buttonOne;
        public VRTK_BaseControllable buttonTwo;

        public Color onColor = Color.green;
        public Color offColor = Color.red;

        protected bool buttonOnePressed = false;
        protected bool buttonTwoPressed = false;

        protected virtual void OnEnable()
        {
            SetStayPressed(buttonOne, true);
            SetStayPressed(buttonTwo, true);

            buttonOne.MaxLimitReached += ButtonOne_MaxLimitReached;
            buttonTwo.MaxLimitReached += ButtonTwo_MaxLimitReached;
            buttonOne.MaxLimitExited += ButtonOne_MaxLimitExited;
            buttonTwo.MaxLimitExited += ButtonTwo_MaxLimitExited;
        }

        protected virtual void OnDisable()
        {
            buttonOne.MaxLimitReached -= ButtonOne_MaxLimitReached;
            buttonTwo.MaxLimitReached -= ButtonTwo_MaxLimitReached;
            buttonOne.MaxLimitExited -= ButtonOne_MaxLimitExited;
            buttonTwo.MaxLimitExited -= ButtonTwo_MaxLimitExited;
        }

        protected virtual void ButtonOne_MaxLimitReached(object sender, Controllables.ControllableEventArgs e)
        {
            if (buttonTwoPressed)
            {
                SetStayPressed(buttonTwo, false);
            }
            buttonOnePressed = true;
            SetPositionTarget(buttonOne, 0f);
            ChangeColor(buttonOne.gameObject, onColor);
        }

        protected virtual void ButtonTwo_MaxLimitReached(object sender, Controllables.ControllableEventArgs e)
        {
            if (buttonOnePressed)
            {
                SetStayPressed(buttonOne, false);
            }
            buttonTwoPressed = true;
            SetPositionTarget(buttonTwo, 0f);
            ChangeColor(buttonTwo.gameObject, onColor);
        }

        protected virtual void ButtonOne_MaxLimitExited(object sender, Controllables.ControllableEventArgs e)
        {
            SetStayPressed(buttonOne, true);
            buttonOnePressed = false;
            ChangeColor(buttonOne.gameObject, offColor);
        }

        protected virtual void ButtonTwo_MaxLimitExited(object sender, Controllables.ControllableEventArgs e)
        {
            SetStayPressed(buttonTwo, true);
            buttonTwoPressed = false;
            ChangeColor(buttonTwo.gameObject, offColor);
        }

        protected virtual void ChangeColor(GameObject obj, Color col)
        {
            obj.GetComponent<Renderer>().material.color = col;
        }


        protected virtual void SetStayPressed(VRTK_BaseControllable obj, bool state)
        {
            if (obj.GetType() == typeof(VRTK_PhysicsPusher))
            {
                VRTK_PhysicsPusher physicsObj = obj as VRTK_PhysicsPusher;
                physicsObj.stayPressed = state;
            }
            else if (obj.GetType() == typeof(VRTK_ArtificialPusher))
            {
                VRTK_ArtificialPusher artificialObj = obj as VRTK_ArtificialPusher;
                artificialObj.SetStayPressed(state);
            }
        }

        protected virtual void SetPositionTarget(VRTK_BaseControllable obj, float newTarget)
        {
            if (obj.GetType() == typeof(VRTK_PhysicsPusher))
            {
                VRTK_PhysicsPusher physicsObj = obj as VRTK_PhysicsPusher;
                physicsObj.positionTarget = newTarget;
            }
            else if (obj.GetType() == typeof(VRTK_ArtificialPusher))
            {
                VRTK_ArtificialPusher artificialObj = obj as VRTK_ArtificialPusher;
                artificialObj.SetPositionTarget(newTarget);
            }
        }
    }
}