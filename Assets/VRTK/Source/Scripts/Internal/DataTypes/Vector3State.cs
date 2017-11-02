[System.Serializable]
public class Vector3State
{
    public bool xState;
    public bool yState;
    public bool zState;

    public Vector3State(bool x, bool y, bool z)
    {
        xState = x;
        yState = y;
        zState = z;
    }
}