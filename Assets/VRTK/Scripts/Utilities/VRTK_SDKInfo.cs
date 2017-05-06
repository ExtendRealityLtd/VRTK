// SDK Info|Utilities|90011
namespace VRTK
{
    using UnityEngine;
    using System;
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

        /// <summary>
        /// Creates a new SDK info for a type that is known at compile time.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <typeparam name="ActualType">The SDK type to use. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <returns>A newly created instance.</returns>
        public static VRTK_SDKInfo Create<BaseType, FallbackType, ActualType>() where BaseType : SDK_Base where FallbackType : BaseType where ActualType : BaseType
        {
            var sdkInfo = new VRTK_SDKInfo();
            sdkInfo.SetUp(typeof(BaseType), typeof(FallbackType), typeof(ActualType).FullName);

            return sdkInfo;
        }

        /// <summary>
        /// Creates a new SDK info for a type.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <param name="actualType">The SDK type to use. Must be a subclass of <typeparamref name="BaseType"/>.</param>
        /// <returns>A newly created instance.</returns>
        public static VRTK_SDKInfo Create<BaseType, FallbackType>(Type actualType) where BaseType : SDK_Base where FallbackType : BaseType
        {
            var sdkInfo = new VRTK_SDKInfo();
            sdkInfo.SetUp(typeof(BaseType), typeof(FallbackType), actualType.FullName);

            return sdkInfo;
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
            SetUp(Type.GetType(infoToCopy.baseTypeName), Type.GetType(infoToCopy.fallbackTypeName), infoToCopy.typeName);
        }

        private void SetUp(Type baseType, Type fallbackType, string actualTypeName)
        {
            if (!baseType.IsSubclassOf(typeof(SDK_Base)))
            {
                throw new ArgumentOutOfRangeException("baseType", baseType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", baseType.Name, typeof(SDK_Base).Name));
            }

            if (!fallbackType.IsSubclassOf(baseType))
            {
                throw new ArgumentOutOfRangeException("fallbackType", fallbackType, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", fallbackType.Name, baseType.Name));
            }

            baseTypeName = baseType.FullName;
            fallbackTypeName = fallbackType.FullName;
            typeName = actualTypeName;

            if (string.IsNullOrEmpty(actualTypeName))
            {
                type = fallbackType;
                originalTypeNameWhenFallbackIsUsed = null;
                description = SDK_DescriptionAttribute.Fallback;

                return;
            }

            Type actualType = Type.GetType(actualTypeName);
            if (actualType == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_NOT_FOUND, new string[] { actualTypeName, fallbackType.Name }));

                type = fallbackType;
                originalTypeNameWhenFallbackIsUsed = actualTypeName;
                description = SDK_DescriptionAttribute.Fallback;

                return;
            }

            if (!actualType.IsSubclassOf(baseType))
            {
                throw new ArgumentOutOfRangeException("actualTypeName", actualTypeName, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", actualTypeName, baseType.Name));
            }

            string fallbackNamespace = typeof(SDK_FallbackSystem).Namespace;
            string fallbackNamePrefix = typeof(SDK_FallbackSystem).Name.Replace("System", "");
            if (actualType.Namespace == fallbackNamespace && actualType.Name.StartsWith(fallbackNamePrefix, StringComparison.Ordinal))
            {
                type = actualType;
                originalTypeNameWhenFallbackIsUsed = null;
                description = SDK_DescriptionAttribute.Fallback;

                return;
            }

            var actualDescription = (SDK_DescriptionAttribute)actualType.GetCustomAttributes(typeof(SDK_DescriptionAttribute), false).FirstOrDefault();
            if (actualDescription == null)
            {
                throw new ArgumentException(string.Format("'{0}' doesn't specify an SDK description via '{1}'.", actualTypeName, typeof(SDK_DescriptionAttribute).Name), "actualTypeName");
            }

            type = actualType;
            originalTypeNameWhenFallbackIsUsed = null;
            description = actualDescription;
        }

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            SetUp(Type.GetType(baseTypeName), Type.GetType(fallbackTypeName), typeName);
        }

        #endregion

        #region Equality via type

        public override bool Equals(object obj)
        {
            var other = obj as VRTK_SDKInfo;
            if ((object)other == null)
            {
                return false;
            }

            return type == other.type;
        }

        public bool Equals(VRTK_SDKInfo other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return type == other.type;
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

            return x.type == y.type;
        }

        public static bool operator !=(VRTK_SDKInfo x, VRTK_SDKInfo y)
        {
            return !(x == y);
        }

        #endregion
    }
}