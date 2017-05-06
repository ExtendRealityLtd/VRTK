namespace VRTK.Examples.Archery
{
    using UnityEngine;

    public class BowHandle : MonoBehaviour
    {
        public Transform arrowNockingPoint;
        public BowAim aim;
        [HideInInspector]
        public Transform nockSide;
    }
}