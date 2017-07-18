namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public class SDK_ControllerSim : MonoBehaviour
    {
        private Vector3 lastPos;
        private Quaternion lastRot;
        private List<Vector3> posList;
        private List<Vector3> rotList;
        private bool selected;
        private float magnitude;
        private Vector3 axis;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }

        public Vector3 GetVelocity()
        {
            Vector3 velocity = Vector3.zero;
            foreach (Vector3 vel in posList)
            {
                velocity += vel;
            }
            velocity /= posList.Count;
            return velocity;
        }

        public Vector3 GetAngularVelocity()
        {
            Vector3 angularVelocity = Vector3.zero;
            foreach (Vector3 vel in rotList)
            {
                angularVelocity += vel;
            }
            angularVelocity /= rotList.Count;
            return angularVelocity;
        }

        private void Awake()
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();
            lastPos = transform.position;
            lastRot = transform.rotation;
        }

        private void Update()
        {
            posList.Add((transform.position - lastPos) / Time.deltaTime);
            if (posList.Count > 4)
            {
                posList.RemoveAt(0);
            }
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse (lastRot);
            deltaRotation.ToAngleAxis(out magnitude, out axis);
            rotList.Add((axis * magnitude));
            if (rotList.Count > 4)
            {
                rotList.RemoveAt(0);
            }
            lastPos = transform.position;
            lastRot = transform.rotation;
        }
    }
}
