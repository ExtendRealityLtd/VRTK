using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    public class SmoothedVector3
    {
        private int maxSampleCount;

        private List<Vector3> samples = new List<Vector3>();

        public SmoothedVector3(int maxSampleCount)
        {
            this.maxSampleCount = maxSampleCount;
        }

        public Vector3 Position
        {
            get
            {
                return GetMean(samples);
            }
        }

        public void AddSample(Vector3 s)
        {
            if (samples.Count == maxSampleCount)
            {
                // remove oldest sample
                samples.RemoveAt(0);
            }
            samples.Add(s);

        }

        public Vector3 GetMean(List<Vector3> values)
        {
            if (values.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 mean = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                mean += values[i];
            }
            mean /= values.Count;
            return mean;
        }
    }
}
