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
            Pointer,
            Highlighter,
            Collider
        }

        public ObjectTypes objectType;

        /// <summary>
        /// The SetPlayerObject method tags the given game object with a special player object class for easier identification.
        /// </summary>
        /// <param name="obj">The game object to add the player object class to.</param>
        /// <param name="objType">The type of player object that is to be assigned.</param>
        public static void SetPlayerObject(GameObject obj, ObjectTypes objType)
        {
            if (!obj.GetComponent<VRTK_PlayerObject>())
            {
                var playerObject = obj.AddComponent<VRTK_PlayerObject>();
                playerObject.objectType = objType;
            }
        }
    }
}