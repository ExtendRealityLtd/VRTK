// Policy List|Utilities|90071
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Policy List String Identifier is the script to specify a string on to be checked against in VRTK_PolicyList to see if another operation is permitted.
    /// </summary>
    /// <remarks>
    /// A number of other scripts can use a Policy List to determine if an operation is permitted based on whether a game object has a tag applied, a script component on it, whether it's on a given layer or whether a game object has the VRTK_PolicyListStringIdentifier component on which a given string is specified on it.    
    /// If a game object has this component, Policy Lists can check, wheter a string is specified on this game object. 
    /// Using this script is similar to use identification via game object's tag but has the advantage to keep the tag list clean.
    /// 
    /// For example, if you use 100 SnapDropZones (e.g. Keyholes on 100 different locked doors) you would need 100 scripts or 100 different tags and thereby mess up your tag structure or script structure. Using this script you can attach this script to the game object which should be checked and identify a string (e.g. "Key75").
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_PolicyListStringIdentifier")]
    public class VRTK_PolicyListStringIdentifier : MonoBehaviour
    {
        [Tooltip("This string will be checked against VRTK_PolicyList's identifiers, if VRTK_PolicyList's checkType is set to CheckTypes.StringIdentifier, among others.")]
        public string stringIdentifier;

        /// <summary>
        /// This method is called by VRTK_PolicyList's StringIdentifierCheck method to check whether one of it's indentifiers equals the string specified in this' stringIdentifier.
        /// <param name="obj">This method is called by VRTK_PolicyList's StringIdentifierCheck method and passes itself as parameter.</param>
        /// <returns>If the string specified in this' stringIdentifier matches the VRTK_PolicyList's identifier list then it returns true.</returns>
        public virtual bool CheckString(VRTK_PolicyList list)
        {
            if (list != null)
            {
                return list.identifiers.Contains(stringIdentifier);
            }
            return false;
        }
    }
}