namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public struct GazeEventArgs
    {
        public GameObject gazedObject;
    }

    public delegate void GazeEventHandler(object sender, GazeEventArgs e);

    public class VRTK_GazeDetection : MonoBehaviour
    {
        public List<GameObject> detectableObjects;
        public float boundsIncreasePercentage;
        protected Transform eyeCamera;
        protected Dictionary<GameObject, HashSet<Renderer>> gazedObjects;

        public event GazeEventHandler GazeStarted;
        public event GazeEventHandler GazeStopped;

        public virtual void OnGazeStart(GazeEventArgs e)
        {
            if (GazeStarted != null)
                GazeStarted(this, e);
        }

        public virtual void OnGazeStopped(GazeEventArgs e)
        {
            if (GazeStopped != null)
                GazeStopped(this, e);
        }

        protected GazeEventArgs SetGazeEventArgs(GameObject gazedObject)
        {
            GazeEventArgs e;
            e.gazedObject = gazedObject;

            return e;
        }

        // Use this for initialization
        void Start()
        {
            Utilities.SetPlayerObject(this.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            eyeCamera = DeviceFinder.HeadsetCamera();
            gazedObjects = new Dictionary<GameObject, HashSet<Renderer>>();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(eyeCamera.position, eyeCamera.forward);

            foreach (GameObject detectable in detectableObjects)
            {
                Renderer[] renderers = detectable.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    Bounds bounds = renderer.bounds;
                    Vector3 boundSizeIncrease = bounds.size * boundsIncreasePercentage;
                    bounds.Expand(boundSizeIncrease);

                    if (bounds.IntersectRay(ray))
                    {
                        if (!gazedObjects.ContainsKey(detectable) || gazedObjects[detectable].Count <= 0)
                        {
                            Debug.Log("GAZE START");
                            gazedObjects[detectable] = new HashSet<Renderer>();
                            gazedObjects[detectable].Add(renderer);
                            OnGazeStart(SetGazeEventArgs(detectable));
                        }
                        else
                        {
                            gazedObjects[detectable].Add(renderer);
                        }
                    }
                    else
                    {
                        if (gazedObjects.ContainsKey(detectable))
                        {
                            var originalCount = gazedObjects[detectable].Count;
                            gazedObjects[detectable].Remove(renderer);

                            if (gazedObjects[detectable].Count == 0 && originalCount > 0)
                            {
                                Debug.Log("GAZE STOP");
                                OnGazeStopped(SetGazeEventArgs(detectable));
                            }
                        }
                    }
                }
            }
        }
    }
}
