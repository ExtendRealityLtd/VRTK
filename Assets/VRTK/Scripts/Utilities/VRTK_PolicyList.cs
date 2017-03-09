// Policy List|Utilities|90040
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Policy List allows to create a list of either tag names, script names or layer names that can be checked against to see if another operation is permitted.
    /// </summary>
    /// <remarks>
    /// A number of other scripts can use a Policy List to determine if an operation is permitted based on whether a game object has a tag applied, a script component on it or whether it's on a given layer.
    ///
    /// For example, the Teleporter scripts can ignore game object targets as a teleport location if the game object contains a tag that is in the identifiers list and the policy is set to ignore.
    ///
    /// Or the teleporter can only allow teleport to targets that contain a tag that is in the identifiers list and the policy is set to include.
    ///
    /// Add the Policy List script to a game object (preferably the same component utilising the list) and then configure the list accordingly.
    ///
    /// Then in the component that has a Policy List paramter (e.g. BasicTeleporter has `Target List Policy`) simply select the list that has been created and defined.
    /// </remarks>
    public class VRTK_PolicyList : MonoBehaviour
    {
        /// <summary>
        /// The operation to apply on the list of identifiers.
        /// </summary>
        /// <param name="Ignore">Will ignore any game objects that contain either a tag or script component that is included in the identifiers list.</param>
        /// <param name="Include">Will only include game objects that contain either a tag or script component that is included in the identifiers list.</param>
        public enum OperationTypes
        {
            Ignore,
            Include
        }

        /// <summary>
        /// The types of element that can be checked against.
        /// </summary>
        /// <param name="Tag">The tag applied to the game object.</param>
        /// <param name="Script">A script component added to the game object.</param>
        /// <param name="Layer">A layer applied to the game object.</param>
        public enum CheckTypes
        {
            Tag = 1,
            Script = 2,
            Layer = 4
        }

        [Tooltip("The operation to apply on the list of identifiers.")]
        public OperationTypes operation = OperationTypes.Ignore;
        [Tooltip("The element type on the game object to check against.")]
        public CheckTypes checkType = CheckTypes.Tag;
        [Tooltip("A list of identifiers to check for against the given check type (either tag or script).")]
        public List<string> identifiers = new List<string>() { "" };

        /// <summary>
        /// The Find method performs the set operation to determine if the given game object contains one of the identifiers on the set check type.
        /// </summary>
        /// <remarks>
        /// For instance, if the Operation is `Ignore` and the Check Type is `Tag` then the Find method will attempt to see if the given game object has a tag that matches one of the identifiers.
        /// </remarks>
        /// <param name="obj">The game object to check if it has a tag or script that is listed in the identifiers list.</param>
        /// <returns>If the operation is `Ignore` and the game object is matched by an identifier from the list then it returns true. If the operation is `Include` and the game object is not matched by an identifier from the list then it returns true.</returns>
        public virtual bool Find(GameObject obj)
        {
            if (operation == OperationTypes.Ignore)
            {
                return TypeCheck(obj, true);
            }
            else
            {
                return TypeCheck(obj, false);
            }
        }

        /// <summary>
        /// The Check method is used to check if a game object should be ignored based on a given string or policy list.
        /// </summary>
        /// <param name="obj">The game object to check.</param>
        /// <param name="list">The policy list to use for checking.</param>
        /// <returns>Returns true of the given game object matches the policy list or given string logic.</returns>
        public static bool Check(GameObject obj, VRTK_PolicyList list)
        {
            if (list)
            {
                return list.Find(obj);
            }
            return false;
        }

        protected virtual bool ScriptCheck(GameObject obj, bool returnState)
        {
            foreach (var identifier in identifiers)
            {
                if (obj.GetComponent(identifier))
                {
                    return returnState;
                }
            }
            return !returnState;
        }

        protected virtual bool TagCheck(GameObject obj, bool returnState)
        {
            if (returnState)
            {
                return identifiers.Contains(obj.tag);
            }
            else
            {
                return !identifiers.Contains(obj.tag);
            }
        }

        protected virtual bool LayerCheck(GameObject obj, bool returnState)
        {
            if (returnState)
            {
                return identifiers.Contains(LayerMask.LayerToName(obj.layer));
            }
            else
            {
                return !identifiers.Contains(LayerMask.LayerToName(obj.layer));
            }
        }

        protected virtual bool TypeCheck(GameObject obj, bool returnState)
        {
            var selection = 0;

            if (((int)checkType & (int)CheckTypes.Tag) != 0)
            {
                selection += 1;
            }
            if (((int)checkType & (int)CheckTypes.Script) != 0)
            {
                selection += 2;
            }
            if (((int)checkType & (int)CheckTypes.Layer) != 0)
            {
                selection += 4;
            }

            switch (selection)
            {
                case 1:
                    return TagCheck(obj, returnState);
                case 2:
                    return ScriptCheck(obj, returnState);
                case 3:
                    if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    break;
                case 4:
                    return LayerCheck(obj, returnState);
                case 5:
                    if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    break;
                case 6:
                    if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    break;
                case 7:
                    if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
                    {
                        return returnState;
                    }
                    break;
            }

            return !returnState;
        }
    }
}