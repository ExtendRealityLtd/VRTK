namespace VRTK.Examples
{
    using UnityEngine;

    public class Menu_Object_Spawner : VRTK_InteractableObject
    {
        public enum PrimitiveTypes
        {
            Cube,
            Sphere
        }

        public PrimitiveTypes shape;
        private Color selectedColor;

        public void SetSelectedColor(Color color)
        {
            selectedColor = color;
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);

            if (shape == PrimitiveTypes.Cube)
            {
                CreateShape(PrimitiveType.Cube, selectedColor);
            }
            else if (shape == PrimitiveTypes.Sphere)
            {
                CreateShape(PrimitiveType.Sphere, selectedColor);
            }
            ResetMenuItems();
        }

        private void CreateShape(PrimitiveType shape, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(shape);
            obj.transform.position = transform.position;
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.GetComponent<MeshRenderer>().material.color = color;
            obj.AddComponent<Rigidbody>();
        }

        private void ResetMenuItems()
        {
            foreach (Menu_Object_Spawner menuObjectSpawner in FindObjectsOfType<Menu_Object_Spawner>())
            {
                menuObjectSpawner.StopUsing();
            }
        }
    }
}