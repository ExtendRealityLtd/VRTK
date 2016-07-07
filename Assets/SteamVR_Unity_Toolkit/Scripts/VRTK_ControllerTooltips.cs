namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ControllerTooltips : MonoBehaviour
    {
        public string triggerText;
        public string gripText;
        public string touchpadText;
        public string appMenuText;

        public Color tipBackgroundColor = Color.black;
        public Color tipTextColor = Color.white;
        public Color tipLineColor = Color.black;

        public Transform trigger;
        public Transform grip;
        public Transform touchpad;
        public Transform appMenu;

        private bool triggerInitialised = false;
        private bool gripInitialised = false;
        private bool touchpadInitialised = false;
        private bool appMenuInitialised = false;


        public void ShowTips(bool state)
        {
            foreach (var tooltip in this.GetComponentsInChildren<VRTK_ObjectTooltip>())
            {
                tooltip.gameObject.SetActive(state);
            }
        }

        private void Start()
        {
            triggerInitialised = false;
            gripInitialised = false;
            touchpadInitialised = false;
            appMenuInitialised = false;
            InitialiseTips();
        }

        private void InitialiseTips()
        {
            foreach (var tooltip in this.GetComponentsInChildren<VRTK_ObjectTooltip>())
            {
                var tipText = "";
                Transform tipTransform = null;

                switch (tooltip.name.Replace("Tooltip", "").ToLower())
                {
                    case "trigger":
                        tipText = triggerText;
                        tipTransform = GetTransform(trigger, "trigger");
                        if (tipTransform != null)
                        {
                            triggerInitialised = true;
                        }
                        break;
                    case "grip":
                        tipText = gripText;
                        tipTransform = GetTransform(grip, "lgrip"); ;
                        if (tipTransform != null)
                        {
                            gripInitialised = true;
                        }
                        break;
                    case "touchpad":
                        tipText = touchpadText;
                        tipTransform = GetTransform(touchpad, "trackpad"); ;
                        if (tipTransform != null)
                        {
                            touchpadInitialised = true;
                        }
                        break;
                    case "appmenu":
                        tipText = appMenuText;
                        tipTransform = GetTransform(appMenu, "button"); ;
                        if (tipTransform != null)
                        {
                            appMenuInitialised = true;
                        }
                        break;
                }

                tooltip.displayText = tipText;
                tooltip.drawLineTo = tipTransform;

                tooltip.containerColor = tipBackgroundColor;
                tooltip.fontColor = tipTextColor;
                tooltip.lineColor = tipLineColor;

                tooltip.Reset();
            }
        }

        private bool TipsInitialised()
        {
            return (triggerInitialised && gripInitialised && touchpadInitialised && appMenuInitialised);
        }

        private Transform GetTransform(Transform setTransform, string findTransform)
        {
            Transform returnTransform = null;
            if (setTransform)
            {
                returnTransform = setTransform;
            }
            else
            {
                returnTransform = this.transform.parent.FindChild("Model/" + findTransform + "/attach");
            }

            return returnTransform;
        }

        private void Update()
        {
            if (!TipsInitialised())
            {
                InitialiseTips();
            }
        }
    }
}