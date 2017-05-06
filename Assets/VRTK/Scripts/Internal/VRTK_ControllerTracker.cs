namespace VRTK
{
    using UnityEngine;

    public class VRTK_ControllerTracker : MonoBehaviour
    {
        protected VRTK_TrackedController trackedController;

        protected virtual void OnEnable()
        {
            GameObject actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            trackedController = (actualController != null ? actualController.GetComponent<VRTK_TrackedController>() : GetComponent<VRTK_TrackedController>());
            Update();
        }

        protected virtual void Update()
        {
            if (trackedController && transform.parent != trackedController.transform)
            {
                var transformLocalScale = transform.localScale;
                transform.SetParent(trackedController.transform);
                transform.localPosition = Vector3.zero;
                transform.localScale = transformLocalScale;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}