namespace VRTK.Examples
{
    using UnityEngine;

    public class Menu_Color_Changer : VRTK_InteractableObject
    {
        public Color newMenuColor = Color.black;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            transform.parent.gameObject.GetComponent<Menu_Container_Object_Colors>().SetSelectedColor(newMenuColor);
            ResetMenuItems();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            gameObject.GetComponent<MeshRenderer>().material.color = newMenuColor;
        }

        private void ResetMenuItems()
        {
            foreach (Menu_Color_Changer menuColorChanger in FindObjectsOfType<Menu_Color_Changer>())
            {
                menuColorChanger.StopUsing(null);
            }
        }
    }
}