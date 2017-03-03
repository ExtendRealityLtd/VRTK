// SDK Scripting Define Symbol Predicate|SDK_Base|003
namespace VRTK
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;

    /// <summary>
    /// Specifies a method to be used as a predicate to allow <see cref="VRTK_SDKManager"/> to automatically add and remove scripting define symbols. Only allowed on <see langword="static"/> methods that take no arguments and return <see cref="bool"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class SDK_ScriptingDefineSymbolPredicateAttribute : Attribute
    {
        /// <summary>
        /// The prefix of scripting define symbols that must be used to be able to automatically remove the symbols.
        /// </summary>
        public const string RemovableSymbolPrefix = "VRTK_DEFINE_";

        /// <summary>
        /// The scripting define symbol to conditionally add or remove.
        /// </summary>
        public readonly string symbol;
#if UNITY_EDITOR
        /// <summary>
        /// The build target group to use when conditionally adding or removing <see cref="symbol"/>.
        /// </summary>
        public readonly BuildTargetGroup buildTargetGroup;
#endif

        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        /// <param name="symbol">The scripting define symbol to conditionally add or remove. Needs to start with <see cref="RemovableSymbolPrefix"/> to be able to automatically remove the symbol. <see langword="null"/> and <see cref="string.Empty"/> aren't allowed.</param>
        /// <param name="buildTargetGroupName">The name of a constant of <see cref="BuildTargetGroup"/>. <see cref="BuildTargetGroup.Unknown"/>, <see langword="null"/> and <see cref="string.Empty"/> aren't allowed.</param>
        public SDK_ScriptingDefineSymbolPredicateAttribute(string symbol, string buildTargetGroupName)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            if (symbol == string.Empty)
            {
                throw new ArgumentOutOfRangeException("symbol", symbol, "An empty string isn't allowed.");
            }

            this.symbol = symbol;

            if (buildTargetGroupName == null)
            {
                throw new ArgumentNullException("buildTargetGroupName");
            }
            if (buildTargetGroupName == string.Empty)
            {
                throw new ArgumentOutOfRangeException("buildTargetGroupName", buildTargetGroupName, "An empty string isn't allowed.");
            }

#if UNITY_EDITOR
            Type buildTargetGroupType = typeof(BuildTargetGroup);
            try
            {
                buildTargetGroup = (BuildTargetGroup)Enum.Parse(buildTargetGroupType, buildTargetGroupName);
            }
            catch (Exception exception)
            {
                throw new ArgumentOutOfRangeException(string.Format("'{0}' isn't a valid constant name of '{1}'.", buildTargetGroupName, buildTargetGroupType.Name), exception);
            }

            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                throw new ArgumentOutOfRangeException("buildTargetGroupName", buildTargetGroupName, string.Format("The buildTargetGroupName '{0}' isn't allowed.", buildTargetGroupName));
            }
#endif
        }
    }
}