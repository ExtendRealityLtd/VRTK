namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class VRTK_ControllerReference : IEquatable<VRTK_ControllerReference>
    {
        public static Dictionary<uint, VRTK_ControllerReference> controllerReferences = new Dictionary<uint, VRTK_ControllerReference>();

        public static VRTK_ControllerReference GetControllerReference(uint controllerIndex)
        {
            if (controllerIndex < uint.MaxValue)
            {
                VRTK_ControllerReference foundReference = VRTK_SharedMethods.GetDictionaryValue(controllerReferences, controllerIndex);
                if (foundReference != null)
                {
                    return foundReference;
                }
                return new VRTK_ControllerReference(controllerIndex);
            }
            return null;
        }

        public static VRTK_ControllerReference GetControllerReference(GameObject controllerObject)
        {
            //Try and get the index from either the actual or alias
            uint controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(controllerObject);

            //If not found then try and get index from the model object
            if (controllerIndex >= uint.MaxValue)
            {
                controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(GetValidObjectFromHand(VRTK_SDK_Bridge.GetControllerModelHand(controllerObject)));
            }

            VRTK_ControllerReference foundReference = VRTK_SharedMethods.GetDictionaryValue(controllerReferences, controllerIndex);
            if (foundReference != null)
            {
                return foundReference;
            }
            return new VRTK_ControllerReference(controllerIndex);
        }

        public static VRTK_ControllerReference GetControllerReference(SDK_BaseController.ControllerHand controllerHand)
        {
            GameObject scriptAlias = GetValidObjectFromHand(controllerHand);
            uint controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(scriptAlias);
            VRTK_ControllerReference foundReference = VRTK_SharedMethods.GetDictionaryValue(controllerReferences, controllerIndex);
            if (foundReference != null)
            {
                return foundReference;
            }
            return new VRTK_ControllerReference(scriptAlias);
        }

        public static bool IsValid(VRTK_ControllerReference controllerReference)
        {
            return (controllerReference != null ? controllerReference.IsValid() : false);
        }

        public static uint GetRealIndex(VRTK_ControllerReference controllerReference)
        {
            return (IsValid(controllerReference) ? controllerReference.index : uint.MaxValue);
        }

        protected uint storedControllerIndex = uint.MaxValue;

        public VRTK_ControllerReference(uint controllerIndex)
        {
            //Only set up if the given index matches an actual controller
            if (VRTK_SDK_Bridge.GetControllerByIndex(controllerIndex, true) != null)
            {
                storedControllerIndex = controllerIndex;
                AddToCache();
            }
        }

        public VRTK_ControllerReference(GameObject controllerObject) : this(GetControllerHand(controllerObject))
        {
        }

        public VRTK_ControllerReference(SDK_BaseController.ControllerHand controllerHand)
        {
            storedControllerIndex = VRTK_SDK_Bridge.GetControllerIndex(GetValidObjectFromHand(controllerHand));
            AddToCache();
        }

        public uint index
        {
            get
            {
                return storedControllerIndex;
            }
        }

        public GameObject scriptAlias
        {
            get
            {
                return VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, false);
            }
        }

        public GameObject actual
        {
            get
            {
                return VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, true);
            }
        }

        public GameObject model
        {
            get
            {
                return VRTK_SDK_Bridge.GetControllerModel(GetValidObjectFromIndex());
            }
        }

        public SDK_BaseController.ControllerHand hand
        {
            get
            {
                return GetControllerHand(GetValidObjectFromIndex());
            }
        }

        public bool IsValid()
        {
            return (index < uint.MaxValue);
        }

        public override string ToString()
        {
            return base.ToString() + " --> INDEX[" + index + "] - ACTUAL[" + actual + "] - SCRIPT_ALIAS[" + scriptAlias + "] - MODEL[" + model + "] - HAND[" + hand + "]";
        }

        public override int GetHashCode()
        {
            return (int)index;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            VRTK_ControllerReference objAsPart = obj as VRTK_ControllerReference;
            if (objAsPart == null)
            {
                return false;
            }
            else
            {
                return Equals(objAsPart);
            }
        }

        public bool Equals(VRTK_ControllerReference other)
        {
            if (other == null)
            {
                return false;
            }
            return (index == other.index);
        }

        protected virtual GameObject GetValidObjectFromIndex()
        {
            GameObject checkObject = VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, false);
            return (checkObject == null ? VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, true) : checkObject);
        }

        protected virtual void AddToCache()
        {
            if (IsValid())
            {
                VRTK_SharedMethods.AddDictionaryValue(controllerReferences, storedControllerIndex, this, true);
            }
        }

        private static GameObject GetValidObjectFromHand(SDK_BaseController.ControllerHand controllerHand)
        {
            switch (controllerHand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    return (VRTK_SDK_Bridge.GetControllerLeftHand(false) ? VRTK_SDK_Bridge.GetControllerLeftHand(false) : VRTK_SDK_Bridge.GetControllerLeftHand(true));
                case SDK_BaseController.ControllerHand.Right:
                    return (VRTK_SDK_Bridge.GetControllerRightHand(false) ? VRTK_SDK_Bridge.GetControllerRightHand(false) : VRTK_SDK_Bridge.GetControllerRightHand(true));
            }
            return null;
        }

        private static SDK_BaseController.ControllerHand GetControllerHand(GameObject controllerObject)
        {
            if (VRTK_SDK_Bridge.IsControllerLeftHand(controllerObject, false) || VRTK_SDK_Bridge.IsControllerLeftHand(controllerObject, true))
            {
                return SDK_BaseController.ControllerHand.Left;
            }
            else if (VRTK_SDK_Bridge.IsControllerRightHand(controllerObject, false) || VRTK_SDK_Bridge.IsControllerRightHand(controllerObject, true))
            {
                return SDK_BaseController.ControllerHand.Right;
            }
            return VRTK_SDK_Bridge.GetControllerModelHand(controllerObject);
        }
    }
}