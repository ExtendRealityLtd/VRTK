namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public struct ControllerGlanceEventArgs
    {
        public float distance;
        public GameObject controller;
    }

    public delegate void ControllerGlanceEventHandler(object sender, ControllerGlanceEventArgs e);

    public class VRTK_HeadsetControllerGlance : MonoBehaviour
    {
        public float colliderRadius = 0.12f;
        public event ControllerGlanceEventHandler ControllerGlanceEnter;
        public event ControllerGlanceEventHandler ControllerGlanceExit;

        private GameObject controllerHit = null;

        public virtual void OnControllerGlanceEnter(ControllerGlanceEventArgs e)
        {
            if (ControllerGlanceEnter != null)
                ControllerGlanceEnter(this, e);
        }

        public virtual void OnControllerGlanceExit(ControllerGlanceEventArgs e)
        {
            if (ControllerGlanceExit != null)
                ControllerGlanceExit(this, e);
        }

        private void Start()
        {
            var controllerManager = DeviceFinder.ControllerManager();
            CreateControllerTracker(controllerManager.left);
            CreateControllerTracker(controllerManager.right);
        }

        private void Update()
        {
            var ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hit;
            var hasHit = Physics.Raycast(ray, out hit, 10f);
            if (!hasHit && controllerHit != null)
            {
                OnControllerGlanceExit(SetControllerGlanceEvent(0f, controllerHit));
                controllerHit = null;
            }

            if (hasHit && hit.collider.GetComponent<VRTK_PlayerObject>() && hit.collider.GetComponent<VRTK_PlayerObject>().objectType == VRTK_PlayerObject.ObjectTypes.ControllerTracker)
            {
                if(controllerHit != hit.collider.transform.parent.gameObject)
                {
                    if (controllerHit != null)
                    {
                        OnControllerGlanceExit(SetControllerGlanceEvent(0f, controllerHit));
                        controllerHit = null;
                    }

                    controllerHit = hit.collider.transform.parent.gameObject;
                    OnControllerGlanceEnter(SetControllerGlanceEvent(hit.distance, controllerHit));
                }
            }
        }

        private ControllerGlanceEventArgs SetControllerGlanceEvent(float distance, GameObject controller)
        {
            ControllerGlanceEventArgs e;
            e.distance = distance;
            e.controller = controller;
            return e;
        }

        private void CreateControllerTracker(GameObject controller)
        {
            var tracker = new GameObject("HeadsetRay_Tracker");
            tracker.transform.parent = controller.transform;
            tracker.transform.localPosition = new Vector3(0f, 0f, 0f);
            var collider = tracker.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;
            collider.center = new Vector3(0f, 0f, -0.04f);
            collider.isTrigger = true;
            tracker.AddComponent<VRTK_PlayerObject>().objectType = VRTK_PlayerObject.ObjectTypes.ControllerTracker;
        }
    }
}