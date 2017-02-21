// SDK Description|SDK_Base|002
namespace VRTK
{
    using System;
    using System.Linq;

    /// <summary>
    /// Describes a class that represents an SDK. Only allowed on classes that inherit from <see cref="SDK_Base"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SDK_DescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description of a fallback SDK.
        /// </summary>
        public static readonly SDK_DescriptionAttribute Fallback = new SDK_DescriptionAttribute("Fallback", null);

        /// <summary>
        /// The pretty name of the SDK. Uniquely identifies the SDK.
        /// </summary>
        public readonly string prettyName;
        /// <summary>
        /// The scripting define symbol needed for the SDK. Needs to be the same as <see cref="SDK_ScriptingDefineSymbolPredicateAttribute.symbol"/> to add and remove the scripting define symbol automatically using <see cref="VRTK_SDKManager"/>.
        /// </summary>
        public readonly string symbol;

        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        /// <param name="prettyName">The pretty name of the SDK. Uniquely identifies the SDK. <see langword="null"/> and <see cref="string.Empty"/> aren't allowed.</param>
        /// <param name="symbol">The scripting define symbol needed for the SDK. Needs to be the same as <see cref="SDK_ScriptingDefineSymbolPredicateAttribute.symbol"/> to add and remove the scripting define symbol automatically using <see cref="VRTK_SDKManager"/>. <see langword="null"/> and <see cref="string.Empty"/> are allowed.</param>
        public SDK_DescriptionAttribute(string prettyName, string symbol)
        {
            if (prettyName == null)
            {
                throw new ArgumentNullException("prettyName");
            }
            if (prettyName == string.Empty)
            {
                throw new ArgumentOutOfRangeException("prettyName", prettyName, "An empty string isn't allowed.");
            }

            this.prettyName = prettyName;
            this.symbol = symbol;
        }

        /// <summary>
        /// Creates a new attribute by copying from another attribute on a given type.
        /// </summary>
        /// <param name="typeToCopyExistingDescriptionFrom">The type to copy the existing <see cref="SDK_DescriptionAttribute"/> from. <see langword="null"/> is not allowed.</param>
        public SDK_DescriptionAttribute(Type typeToCopyExistingDescriptionFrom)
        {
            if (typeToCopyExistingDescriptionFrom == null)
            {
                throw new ArgumentNullException("typeToCopyExistingDescriptionFrom");
            }

            Type descriptionType = typeof(SDK_DescriptionAttribute);
            var description = (SDK_DescriptionAttribute)typeToCopyExistingDescriptionFrom.GetCustomAttributes(descriptionType, false).FirstOrDefault();
            if (description == null)
            {
                throw new ArgumentOutOfRangeException("typeToCopyExistingDescriptionFrom", typeToCopyExistingDescriptionFrom, string.Format("'{0}' doesn't specify an SDK description via '{1}' to copy.", typeToCopyExistingDescriptionFrom.Name, descriptionType.Name));
            }

            prettyName = description.prettyName;
            symbol = description.symbol;
        }
    }
}