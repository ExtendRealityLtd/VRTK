// SDK Info|Utilities|90011
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Linq;

    /// <summary>
    /// Holds all the info necessary to describe an SDK.
    /// </summary>
    public sealed class VRTK_SDKInfo
    {
        /// <summary>
        /// The type of the SDK.
        /// </summary>
        public readonly Type type;
        /// <summary>
        /// The name of the type of which this SDK info was created from. This is only used if said type wasn't found.
        /// </summary>
        public readonly string originalTypeNameWhenFallbackIsUsed;
        /// <summary>
        /// The description of the SDK.
        /// </summary>
        public readonly SDK_DescriptionAttribute description;

        private VRTK_SDKInfo(Type type, string originalTypeNameWhenFallbackIsUsed, SDK_DescriptionAttribute description)
        {
            this.type = type;
            this.originalTypeNameWhenFallbackIsUsed = originalTypeNameWhenFallbackIsUsed;
            this.description = description;
        }

        /// <summary>
        /// Creates a new SDK info for a type that is known at compile time.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <typeparam name="ActualType">The SDK type to use. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <returns>A newly created instance.</returns>
        public static VRTK_SDKInfo FromType<BaseType, ActualType>() where BaseType : SDK_Base where ActualType : BaseType
        {
            return FromType<BaseType>(typeof(ActualType));
        }

        /// <summary>
        /// Creates a new SDK info for a type.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="type">The SDK type to use. Must be a subclass of <typeparamref name="BaseType"/>.</param>
        /// <returns>A newly created instance.</returns>
        public static VRTK_SDKInfo FromType<BaseType>(Type type) where BaseType : SDK_Base
        {
            if (!type.IsSubclassOf(typeof(BaseType)))
            {
                throw new ArgumentOutOfRangeException("type", type, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", type.Name, typeof(BaseType).Name));
            }

            string fallbackNamespace = typeof(SDK_FallbackSystem).Namespace;
            string fallbackNamePrefix = typeof(SDK_FallbackSystem).Name.Replace("System", "");
            if (type.Namespace == fallbackNamespace && type.Name.StartsWith(fallbackNamePrefix, StringComparison.Ordinal))
            {
                return new VRTK_SDKInfo(type, null, SDK_DescriptionAttribute.Fallback);
            }

            var description = (SDK_DescriptionAttribute)type.GetCustomAttributes(typeof(SDK_DescriptionAttribute), false).FirstOrDefault();
            if (description == null)
            {
                throw new ArgumentException(string.Format("'{0}' doesn't specify an SDK description via '{1}'.", type.Name, typeof(SDK_DescriptionAttribute).Name), "type");
            }

            return new VRTK_SDKInfo(type, null, description);
        }

        /// <summary>
        /// Creates a new SDK info for a type name. Returns a new SDK info instance for a fallback SDK if problems occur when using the type name.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fallback on if problems occur. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <param name="typeName">The name of the SDK type to use. The type must be a subclass of <typeparamref name="BaseType"/>.</param>
        /// <returns>A newly created instance.</returns>
        public static VRTK_SDKInfo FromTypeName<BaseType, FallbackType>(string typeName) where BaseType : SDK_Base where FallbackType : BaseType
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return FromType<BaseType, FallbackType>();
            }

            Type type = Type.GetType(typeName);
            if (type == null)
            {
                string baseName = typeof(FallbackType).Name.Remove(0, typeof(SDK_FallbackSystem).Name.Length - "System".Length);
                Debug.LogError(string.Format("The SDK '{0}' doesn't exist anymore. The {1} fallback SDK will be used instead.", typeName, baseName));
                return new VRTK_SDKInfo(typeof(FallbackType), typeName, SDK_DescriptionAttribute.Fallback);
            }

            return FromType<BaseType>(type);
        }

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