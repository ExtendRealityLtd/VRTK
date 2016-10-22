namespace VRTK.Examples
{
    using UnityEngine;

    public class ScaleInteractableObject : VRTK_InteractableObject
    {
        private const float MinScale = 0.01f;

        private float initialDistance = 0f;
        private float initialScale = 1f;

        private bool scaleInProgress = false;
        private bool kinematicState = false;

        private GameObject controllerUsing = null;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            controllerUsing = usingObject;

            initialDistance = VRTK.Utilities.GetDistanceBetweenControllers(); 
            initialScale = transform.localScale.x;
            scaleInProgress = true;

            // remember kinematic state so we can reset it when we're done
            var rBody = GetComponent<Rigidbody>();
            kinematicState = rBody.isKinematic;
            rBody.isKinematic = true;
        }

        public override void StopUsing(GameObject previousUsingObject)
        {
            base.StopUsing(previousUsingObject);
            controllerUsing = null;
            scaleInProgress = false;
            var rBody = GetComponent<Rigidbody>();
            rBody.isKinematic = kinematicState;
        }

        protected override void Update()
        {
            if (!scaleInProgress)
            {
                return;
            }

            float currentDistance = VRTK.Utilities.GetDistanceBetweenControllers();
            float newScale = (initialScale / initialDistance) * currentDistance;
            transform.localScale = new Vector3(newScale, newScale, newScale);

            // make sure object's scale doesn't go negative, colliders don't like that
            if (transform.localScale.x < MinScale)
            {
                transform.localScale = new Vector3(MinScale, MinScale, MinScale);
            }

            // force continued touching of the object, makes scaling easier as object gets tiny or huge 
            if (controllerUsing != null)
            {
                controllerUsing.GetComponent<VRTK_InteractTouch>().ForceTouch(gameObject);
            }
            else
            {
                Debug.Log("controllerUsing == null");
            }
        }

    }

}
