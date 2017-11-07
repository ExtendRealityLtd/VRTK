namespace VRTK
{
    using UnityEngine;

    [System.Serializable]
    public class Limits2D
    {
        public float minimum;
        public float maximum;

        public static Limits2D zero
        {
            get
            {
                return new Limits2D(0f, 0f);
            }
        }

        public Limits2D(float min, float max)
        {
            minimum = min;
            maximum = max;
        }

        public Limits2D(Vector2 limits)
        {
            minimum = limits.x;
            maximum = limits.y;
        }

        public bool WithinLimits(float value)
        {
            return (value >= minimum && value <= maximum);
        }

        public Vector2 AsVector2()
        {
            return new Vector2(minimum, maximum);
        }
    }
}