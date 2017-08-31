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
    public sealed class VRTK_PlayerObject : MonoBehaviour
    {
        /// <summary>
        /// The type of object associated to the player.
        /// </summary>
        public enum ObjectTypes
        {
            /// <summary>
            /// No defined object.
            /// </summary>
            Null,
            /// <summary>
            /// The object that represents the VR camera rig.
            /// </summary>
            CameraRig,
            /// <summary>
            /// The object that represents the VR headset.
            /// </summary>
            Headset,
            /// <summary>
            /// An object that represents a VR controller.
            /// </summary>
            Controller,
            /// <summary>
            /// An object that represents a player generated pointer.
            /// </summary>
            Pointer,
            /// <summary>
            /// An object that represents a player generated highlighter.
            /// </summary>
            Highlighter,
            /// <summary>
            /// An object that represents a player collider.
            /// </summary>
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
            VRTK_PlayerObject currentPlayerObject = obj.GetComponent<VRTK_PlayerObject>();
            if (currentPlayerObject == null)
            {
                currentPlayerObject = obj.AddComponent<VRTK_PlayerObject>();
            }
            currentPlayerObject.objectType = objType;
        }

        /// <summary>
        /// The IsPlayerObject method determines if the given game object is a player object and can also check if it's of a specific type.
        /// </summary>
        /// <param name="obj">The GameObjet to check if it's a player object.</param>
        /// <param name="ofType">An optional ObjectType to check if the given GameObject is of a specific player object.</param>
        /// <returns>Returns true if the object is a player object with the optional given type.</returns>
        public static bool IsPlayerObject(GameObject obj, ObjectTypes ofType = ObjectTypes.Null)
        {
            VRTK_PlayerObject[] playerObjects = obj.GetComponentsInParent<VRTK_PlayerObject>(true);
            for (int i = 0; i < playerObjects.Length; i++)
            {
                if (ofType == ObjectTypes.Null || ofType == playerObjects[i].objectType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}