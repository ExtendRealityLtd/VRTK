//====================================================================================
//
// Purpose: Provide a way of tagging game objects as player specific objects to
// allow other scripts to identify these specific objects without needing to use tags
// or without needing to append the name of the game object. 
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    public class VRTK_PlayerObject : MonoBehaviour
    {
        public enum ObjectTypes
        {
            CameraRig,
            Headset,
            Controller,
            Pointer
        }

        public ObjectTypes objectType;
    }
}