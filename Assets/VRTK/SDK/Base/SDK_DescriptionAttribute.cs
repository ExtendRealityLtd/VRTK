// SDK Description|SDK_Base|002
namespace VRTK
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;
    using System.Linq;

    /// <summary>
    /// Describes a class that represents an SDK. Only allowed on classes that inherit from <see cref="SDK_Base"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SDK_DescriptionAttribute : Attribute
    {
        /// <summary>
        /// The pretty name of the SDK. Uniquely identifies the SDK.
        /// </summary>
        public readonly string prettyName;
        /// <summary>
        /// The scripting define symbol needed for the SDK. Needs to be the same as <see cref="SDK_ScriptingDefineSymbolPredicateAttribute.symbol"/> to add and remove the scripting define symbol automatically using <see cref="VRTK_SDKManager"/>.
        /// </summary>
        public readonly string symbol;
        /// <summary>
        /// The name of the VR Device to load.
        /// </summary>
        public readonly string vrDeviceName;
        /// <summary>
        /// The index of this attribute, in case there are multiple on the same target.
        /// </summary>
        public readonly int index;

#if UNITY_EDITOR
        /// <summary>
        /// The build target group this SDK is for.
        /// </summary>
        public BuildTargetGroup buildTargetGroup;
#endif

        /// <summary>
        /// Whether this description describes a fallback SDK.
        /// </summary>
        public bool describesFallbackSDK
        {
            get
            {
                return prettyName == "Fallback";
            }
        }

        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        /// <param name="prettyName">The pretty name of the SDK. Uniquely identifies the SDK. <see langword="null"/> and <see cref="string.Empty"/> aren't allowed.</param>
        /// <param name="symbol">The scripting define symbol needed for the SDK. Needs to be the same as <see cref="SDK_ScriptingDefineSymbolPredicateAttribute.symbol"/> to add and remove the scripting define symbol automatically using <see cref="VRTK_SDKManager"/>. <see langword="null"/> and <see cref="string.Empty"/> are allowed.</param>
        /// <param name="vrDeviceName">The name of the VR Device to load. Set to <see langword="null"/> or <see cref="string.Empty"/> if no VR Device is needed.</param>
        /// <param name="buildTargetGroupName">The name of a constant of <see cref="BuildTargetGroup"/>. "<see cref="BuildTargetGroup.Unknown"/>", <see langword="null"/> and <see cref="string.Empty"/> are not allowed.</param>
        /// <param name="index">The index of this attribute, in case there are multiple on the same target.</param>
        public SDK_DescriptionAttribute(string prettyName, string symbol, string vrDeviceName, string buildTargetGroupName, int index = 0)
        {
            if (prettyName == null)
            {
                VRTK_Logger.Fatal(new ArgumentNullException("prettyName"));
                return;
            }
            if (prettyName == string.Empty)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("prettyName", prettyName, "An empty string isn't allowed."));
                return;
            }

            this.prettyName = prettyName;
            this.symbol = symbol;
            this.vrDeviceName = string.IsNullOrEmpty(vrDeviceName) ? "None" : vrDeviceName;
            this.index = index;

            if (string.IsNullOrEmpty(buildTargetGroupName))
            {
                buildTargetGroupName = "Unknown";
            }

#if UNITY_EDITOR
            Type buildTargetGroupType = typeof(BuildTargetGroup);
            try
            {
                buildTargetGroup = (BuildTargetGroup)Enum.Parse(buildTargetGroupType, buildTargetGroupName);
            }
            catch (Exception exception)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException(string.Format("'{0}' isn't a valid constant name of '{1}'.", buildTargetGroupName, buildTargetGroupType.Name), exception));
                return;
            }

            if (buildTargetGroup == BuildTargetGroup.Unknown && !describesFallbackSDK)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("buildTargetGroupName", buildTargetGroupName, string.Format("'{0}' isn't allowed.", buildTargetGroupName)));
                return;
            }
#endif
        }

        /// <summary>
        /// Creates a new attribute by copying from another attribute on a given type.
        /// </summary>
        /// <param name="typeToCopyExistingDescriptionFrom">The type to copy the existing <see cref="SDK_DescriptionAttribute"/> from. <see langword="null"/> is not allowed.</param>
        /// <param name="index">The index of the description to copy from the the existing <see cref="SDK_DescriptionAttribute"/>.</param>
        public SDK_DescriptionAttribute(Type typeToCopyExistingDescriptionFrom, int index = 0)
        {
            if (typeToCopyExistingDescriptionFrom == null)
            {
                VRTK_Logger.Fatal(new ArgumentNullException("typeToCopyExistingDescriptionFrom"));
                return;
            }

            Type descriptionType = typeof(SDK_DescriptionAttribute);
            SDK_DescriptionAttribute[] descriptions = GetDescriptions(typeToCopyExistingDescriptionFrom);
            if (descriptions.Length == 0)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("typeToCopyExistingDescriptionFrom", typeToCopyExistingDescriptionFrom, string.Format("'{0}' doesn't specify any SDK descriptions via '{1}' to copy.", typeToCopyExistingDescriptionFrom.Name, descriptionType.Name)));
                return;
            }

            if (descriptions.Length <= index)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("index", index, string.Format("'{0}' has no '{1}' at that index.", typeToCopyExistingDescriptionFrom.Name, descriptionType.Name)));
                return;
            }

            SDK_DescriptionAttribute description = descriptions[index];
            prettyName = description.prettyName;
            symbol = description.symbol;
            vrDeviceName = description.vrDeviceName;
            this.index = index;
#if UNITY_EDITOR
            buildTargetGroup = description.buildTargetGroup;
#endif
        }

        public static SDK_DescriptionAttribute[] GetDescriptions(Type type)
        {
            return type.GetCustomAttributes(typeof(SDK_DescriptionAttribute), false)
                       .Cast<SDK_DescriptionAttribute>()
                       .OrderBy(attribute => attribute.index)
                       .ToArray();
        }
    }
}