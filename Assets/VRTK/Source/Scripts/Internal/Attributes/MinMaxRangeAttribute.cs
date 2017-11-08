namespace VRTK
{
    using UnityEngine;

    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float max;
        public readonly float min;

        public MinMaxRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}