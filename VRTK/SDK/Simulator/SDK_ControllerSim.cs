namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public class SDK_ControllerSim : MonoBehaviour
    {
        private Vector3 lastPos;
        private Vector3 lastRot;
        private List<Vector3> posList;
        private List<Vector3> rotList;
        private bool selected;

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
            lastRot = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            posList.Add((transform.position - lastPos) / Time.deltaTime);
            if (posList.Count > 10)
            {
                posList.RemoveAt(0);
            }
            rotList.Add((Quaternion.FromToRotation(lastRot, transform.rotation.eulerAngles)).eulerAngles / Time.deltaTime);
            if (rotList.Count > 10)
            {
                rotList.RemoveAt(0);
            }
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
        }
    }
}
