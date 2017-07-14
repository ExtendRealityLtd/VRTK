using System;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif

namespace VRTK
{
    public static class VRTK_Defines
    {
        /// <summary>
        /// The current version of VRTK.
        /// </summary>
        public static readonly Version CurrentVersion = new Version(3, 2, 1);

        /// <summary>
        /// The previously known versions of VRTK.
        /// </summary>
        public static readonly Version[] PreviousVersions =
        {
            new Version(3, 1, 0),
            new Version(3, 2, 0)
        };

        /// <summary>
        /// The scripting define symbol that is used for the current version of VRTK.
        /// </summary>
        public static string CurrentExactVersionScriptingDefineSymbol { get; private set; }

        public const string VersionScriptingDefineSymbolPrefix = "VRTK_VERSION_";
        public const string VersionScriptingDefineSymbolSuffix = "_OR_NEWER";

        static VRTK_Defines()
        {
            CurrentExactVersionScriptingDefineSymbol = ExactVersionSymbol(CurrentVersion);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EnsureVersionSymbolIsSet()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            IEnumerable<string> atLeastVersionSymbols = new[] { CurrentVersion }
                .Concat(PreviousVersions)
                .Select(AtLeastVersionSymbol);
            string[] versionSymbols = new[] { CurrentExactVersionScriptingDefineSymbol }
                .Concat(atLeastVersionSymbols)
                .ToArray();

            foreach (BuildTargetGroup targetGroup in VRTK_SharedMethods.GetValidBuildTargetGroups())
            {
                string[] currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                                                        .Split(';')
                                                        .Distinct()
                                                        .OrderBy(symbol => symbol, StringComparer.Ordinal)
                                                        .ToArray();
                string[] newSymbols = currentSymbols.Where(symbol => !symbol.StartsWith(VersionScriptingDefineSymbolPrefix, StringComparison.Ordinal))
                                                    .Concat(versionSymbols)
                                                    .ToArray();

                if (!currentSymbols.SequenceEqual(newSymbols))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", newSymbols));
                }
            }
        }
#endif

        private static string ExactVersionSymbol(Version version)
        {
            return string.Format("{0}{1}", VersionScriptingDefineSymbolPrefix, version.ToString().Replace(".", "_"));
        }

        private static string AtLeastVersionSymbol(Version version)
        {
            return string.Format("{0}{1}", ExactVersionSymbol(version), VersionScriptingDefineSymbolSuffix);
        }
    }
}
