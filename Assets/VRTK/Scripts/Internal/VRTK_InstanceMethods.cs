namespace VRTK
{
    using UnityEngine;

    public class VRTK_InstanceMethods : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance to set up the InstanceMethods classes.
        /// </summary>
        public static VRTK_InstanceMethods instance = null;

        public VRTK_Haptics haptics;
        public VRTK_ObjectAppearance objectAppearance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.persistOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            haptics = gameObject.AddComponent<VRTK_Haptics>();
            objectAppearance = gameObject.AddComponent<VRTK_ObjectAppearance>();
        }
    }
}