// SDK Scripting Define Symbol Predicate|SDK_Base|003
namespace VRTK
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using System;

    /// <summary>
    /// Specifies a method to be used as a predicate to allow <see cref="VRTK_SDKManager"/> to automatically add and remove scripting define symbols. Only allowed on <see langword="static"/> methods that take no arguments and return <see cref="bool"/>.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class SDK_ScriptingDefineSymbolPredicateAttribute : Attribute, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The prefix of scripting define symbols that must be used to be able to automatically remove the symbols.
        /// </summary>
        public const string RemovableSymbolPrefix = "VRTK_DEFINE_";

        /// <summary>
        /// The scripting define symbol to conditionally add or remove.
        /// </summary>
        public string symbol;

#if UNITY_EDITOR
        /// <summary>
        /// The build target group to use when conditionally adding or removing <see cref="symbol"/>.
        /// </summary>
        [NonSerialized]
        public BuildTargetGroup buildTargetGroup;
#endif
        [SerializeField]
        private string buildTargetGroupName;

        private SDK_ScriptingDefineSymbolPredicateAttribute()
        {
        }

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

            SetBuildTarget(buildTargetGroupName);
        }

        /// <summary>
        /// Creates a new attribute by copying an existing one.
        /// </summary>
        /// <param name="attributeToCopy">The attribute to copy.</param>
        public SDK_ScriptingDefineSymbolPredicateAttribute(SDK_ScriptingDefineSymbolPredicateAttribute attributeToCopy)
        {
            symbol = attributeToCopy.symbol;
            SetBuildTarget(attributeToCopy.buildTargetGroupName);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            SetBuildTarget(buildTargetGroupName);
        }

        private void SetBuildTarget(string groupName)
        {
            buildTargetGroupName = groupName;

#if UNITY_EDITOR
            Type buildTargetGroupType = typeof(BuildTargetGroup);
            try
            {
                buildTargetGroup = (BuildTargetGroup)Enum.Parse(buildTargetGroupType, groupName);
            }
            catch (Exception exception)
            {
                throw new ArgumentOutOfRangeException(string.Format("'{0}' isn't a valid constant name of '{1}'.", groupName, buildTargetGroupType.Name), exception);
            }

            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                throw new ArgumentOutOfRangeException("groupName", groupName, string.Format("'{0}' isn't allowed.", groupName));
            }
#endif
        }
    }
}