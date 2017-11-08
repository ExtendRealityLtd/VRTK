namespace VRTK
{
    [System.Serializable]
    public class Vector3State
    {
        public bool xState;
        public bool yState;
        public bool zState;

        public static Vector3State False
        {
            get
            {
                return new Vector3State(false, false, false);
            }
        }

        public static Vector3State True
        {
            get
            {
                return new Vector3State(true, true, true);
            }
        }

        public Vector3State(bool x, bool y, bool z)
        {
            xState = x;
            yState = y;
            zState = z;
        }
    }
}