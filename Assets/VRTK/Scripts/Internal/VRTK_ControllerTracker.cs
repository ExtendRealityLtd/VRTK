namespace VRTK
{
    using UnityEngine;

    public class VRTK_ControllerTracker : MonoBehaviour
    {
        private VRTK_TrackedController trackedController;

        private void OnEnable()
        {
            var actualController = VRTK_DeviceFinder.GetActualController(gameObject);
            if (actualController)
            {
                trackedController = actualController.GetComponent<VRTK_TrackedController>();
            }
        }

        private void Update()
        {
            if (trackedController && transform.parent != trackedController.transform)
            {
                transform.SetParent(trackedController.transform);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}