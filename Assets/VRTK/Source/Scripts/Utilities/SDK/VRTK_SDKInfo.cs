// SDK Info|Utilities|90040
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Holds all the info necessary to describe an SDK.
    /// </summary>
    [Serializable]
    public sealed class VRTK_SDKInfo : ISerializationCallbackReceiver
    {
        /// <summary>
        /// The type of the SDK.
        /// </summary>
        public Type type { get; private set; }
        /// <summary>
        /// The name of the type of which this SDK info was created from. This is only used if said type wasn't found.
        /// </summary>
        public string originalTypeNameWhenFallbackIsUsed { get; private set; }
        /// <summary>
        /// The description of the SDK.
        /// </summary>
        public SDK_DescriptionAttribute description { get; private set; }

        [SerializeField]
        private string baseTypeName;
        [SerializeField]
        private string fallbackTypeName;
        [SerializeField]
        private string typeName;
        [SerializeField]
        private int descriptionIndex;

        /// <summary>
        /// Creates new SDK infos for a type that is known at compile time.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of SDK_Base.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of `BaseType`.</typeparam>
        /// <typeparam name="ActualType">The SDK type to use. Must be a subclass of `BaseType`.</typeparam>
        /// <returns>Multiple newly created instances.</returns>
        public static VRTK_SDKInfo[] Create<BaseType, FallbackType, ActualType>() where BaseType : SDK_Base where FallbackType : BaseType where ActualType : BaseType
        {
            return Create<BaseType, FallbackType>(typeof(ActualType));
        }

        /// <summary>
        /// Creates new SDK infos for a type.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of SDK_Base.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of `BaseType.</typeparam>
        /// <param name="actualType">The SDK type to use. Must be a subclass of `BaseType.</param>
        /// <returns>Multiple newly created instances.</returns>
        public static VRTK_SDKInfo[] Create<BaseType, FallbackType>(Type actualType) where BaseType : SDK_Base where FallbackType : BaseType
        {
            string actualTypeName = actualType.FullName;

            SDK_DescriptionAttribute[] descriptions = SDK_DescriptionAttribute.GetDescriptions(actualType);
            if (descriptions.Length == 0)
            {
                VRTK_Logger.Fatal(string.Format("'{0}' doesn't specify any SDK descriptions via '{1}'.", actualTypeName, typeof(SDK_DescriptionAttribute).Name));
                return new VRTK_SDKInfo[0];
            }

            HashSet<VRTK_SDKInfo> sdkInfos = new HashSet<VRTK_SDKInfo>();
            foreach (SDK_DescriptionAttribute description in descriptions)
            {
                VRTK_SDKInfo sdkInfo = new VRTK_SDKInfo();
                sdkInfo.SetUp(typeof(BaseType), typeof(FallbackType), actualTypeName, description.index);
                sdkInfos.Add(sdkInfo);
            }

            return sdkInfos.ToArray();
        }

        private VRTK_SDKInfo()
        {
        }

        /// <summary>
        /// Creates a new SDK info by copying an existing one.
        /// </summary>
        /// <param name="infoToCopy">The SDK info to copy.</param>
        public VRTK_SDKInfo(VRTK_SDKInfo infoToCopy)
        {
            SetUp(Type.GetType(infoToCopy.baseTypeName),
                  Type.GetType(infoToCopy.fallbackTypeName),
                  infoToCopy.typeName,
                  infoToCopy.descriptionIndex);
        }

        private void SetUp(Type baseType, Type fallbackType, string actualTypeName, int descriptionIndex)
        {
            if (baseType == null || fallbackType == null)
                return;
            if (!baseType.IsSubclassOf(typeof(SDK_Base)))
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("baseType", baseType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", baseType.Name, typeof(SDK_Base).Name)));
                return;
            }

            if (!fallbackType.IsSubclassOf(baseType))
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("fallbackType", fallbackType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", fallbackType.Name, baseType.Name)));
                return;
            }

            baseTypeName = baseType.FullName;
            fallbackTypeName = fallbackType.FullName;
            typeName = actualTypeName;

            if (string.IsNullOrEmpty(actualTypeName))
            {
                type = fallbackType;
                originalTypeNameWhenFallbackIsUsed = null;
                this.descriptionIndex = -1;
                description = new SDK_DescriptionAttribute(typeof(SDK_FallbackSystem));

                return;
            }

            Type actualType = Type.GetType(actualTypeName);
            if (actualType == null)
            {
                type = fallbackType;
                originalTypeNameWhenFallbackIsUsed = actualTypeName;
                this.descriptionIndex = -1;
                description = new SDK_DescriptionAttribute(typeof(SDK_FallbackSystem));

                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_NOT_FOUND, actualTypeName, fallbackType.Name));

                return;
            }

            if (!actualType.IsSubclassOf(baseType))
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("actualTypeName", actualTypeName, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", actualTypeName, baseType.Name)));
                return;
            }

            SDK_DescriptionAttribute[] descriptions = SDK_DescriptionAttribute.GetDescriptions(actualType);
            if (descriptions.Length <= descriptionIndex)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("descriptionIndex", descriptionIndex, string.Format("'{0}' has no '{1}' at that index.", actualTypeName, typeof(SDK_DescriptionAttribute).Name)));
                return;
            }

            type = actualType;
            originalTypeNameWhenFallbackIsUsed = null;
            this.descriptionIndex = descriptionIndex;
            description = descriptions[descriptionIndex];
        }

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            SetUp(Type.GetType(baseTypeName), Type.GetType(fallbackTypeName), typeName, descriptionIndex);
        }

        #endregion

        #region Equality via type and descriptionIndex

        public override bool Equals(object obj)
        {
            VRTK_SDKInfo other = obj as VRTK_SDKInfo;
            if ((object)other == null)
            {
                return false;
            }

            return this == other;
        }

        public bool Equals(VRTK_SDKInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        public static bool operator ==(VRTK_SDKInfo x, VRTK_SDKInfo y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            return x.type == y.type && x.descriptionIndex == y.descriptionIndex;
        }

        public static bool operator !=(VRTK_SDKInfo x, VRTK_SDKInfo y)
        {
            return !(x == y);
        }

        #endregion
    }
}