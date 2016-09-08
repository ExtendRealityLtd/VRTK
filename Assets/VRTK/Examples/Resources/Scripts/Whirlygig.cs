namespace VRTK.Examples
{
    using UnityEngine;

    public class Whirlygig : VRTK_InteractableObject
    {
        float spinSpeed = 0f;
        Transform rotator;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            spinSpeed = 360f;
        }

        public override void StopUsing(GameObject usingObject)
        {
            base.StopUsing(usingObject);
            spinSpeed = 0f;
        }

        protected override void Start()
        {
            base.Start();
            rotator = transform.Find("Capsule");
        }

        protected override void Update()
        {
            rotator.transform.Rotate(new Vector3(spinSpeed * Time.deltaTime, 0f, 0f));
        }
    }
}