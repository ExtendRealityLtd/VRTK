// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEngine.SceneManagement;
    using UnityEditor;
    using UnityEditor.Callbacks;
#endif
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The SDK Manager script provides configuration of supported SDKs.
    /// </summary>
    public class VRTK_SDKManager : MonoBehaviour
    {
        /// <summary>
        /// All found scripting define symbol predicate attributes with associated method info.
        /// </summary>
        public static ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> AvailableScriptingDefineSymbolPredicateInfos { get; private set; }

        /// <summary>
        /// All available system SDK infos.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> AvailableSystemSDKInfos { get; private set; }
        /// <summary>
        /// All available boundaries SDK infos.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> AvailableBoundariesSDKInfos { get; private set; }
        /// <summary>
        /// All available headset SDK infos.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> AvailableHeadsetSDKInfos { get; private set; }
        /// <summary>
        /// All available controller SDK infos.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> AvailableControllerSDKInfos { get; private set; }

        /// <summary>
        /// All installed system SDK infos. This is a subset of <see cref="AvailableSystemSDKInfos"/>. It contains only those available SDK infos for which an <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/> exists that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledSystemSDKInfos { get; private set; }
        /// <summary>
        /// All installed boundaries SDK infos. This is a subset of <see cref="AvailableBoundariesSDKInfos"/>. It contains only those available SDK infos for which an <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/> exists that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledBoundariesSDKInfos { get; private set; }
        /// <summary>
        /// All installed headset SDK infos. This is a subset of <see cref="AvailableHeadsetSDKInfos"/>. It contains only those available SDK infos for which an <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/> exists that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledHeadsetSDKInfos { get; private set; }
        /// <summary>
        /// All installed controller SDK infos. This is a subset of <see cref="AvailableControllerSDKInfos"/>. It contains only those available SDK infos for which an <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/> exists that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledControllerSDKInfos { get; private set; }

        /// <summary>
        /// The singleton instance to access the SDK Manager variables from.
        /// </summary>
        public static VRTK_SDKManager instance;

        [Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
        public bool persistOnLoad;

        [Tooltip("This determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.")]
        public bool autoPopulateObjectReferences;

        [Tooltip("This determines whether the scripting define symbols required by the selected SDKs are automatically added to and removed from the player settings. If this is true managing is done whenever the selected SDKs or the current active SDK manager change in the Editor.")]
        public bool autoManageScriptDefines = true;

        /// <summary>
        /// The info of the SDK to use to deal with all system actions. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo systemSDKInfo
        {
            get
            {
                if (cachedSystemSDKInfo == null || cachedSystemSDKInfo.type.FullName != systemSDKTypeName)
                {
#if UNITY_EDITOR
                    DestroyImmediate(cachedSystemSDK);
#else
                    Destroy(cachedSystemSDK);
#endif
                    cachedSystemSDK = null;
                    cachedSystemSDKInfo = VRTK_SDKInfo.FromTypeName<SDK_BaseSystem, SDK_FallbackSystem>(systemSDKTypeName);
                }

                return cachedSystemSDKInfo;
            }
            set
            {
                string newSystemSDKTypeName = value == null ? null : value.type.FullName;
                if (systemSDKTypeName == newSystemSDKTypeName)
                {
                    return;
                }

                systemSDKTypeName = newSystemSDKTypeName;
                HandleSDKInfoSetter<SDK_BaseSystem>(value);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize room scale boundaries. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo boundariesSDKInfo
        {
            get
            {
                if (cachedBoundariesSDKInfo == null || cachedBoundariesSDKInfo.type.FullName != boundariesSDKTypeName)
                {
#if UNITY_EDITOR
                    DestroyImmediate(cachedBoundariesSDK);
#else
                    Destroy(cachedBoundariesSDK);
#endif
                    cachedBoundariesSDK = null;
                    cachedBoundariesSDKInfo = VRTK_SDKInfo.FromTypeName<SDK_BaseBoundaries, SDK_FallbackBoundaries>(boundariesSDKTypeName);
                }

                return cachedBoundariesSDKInfo;
            }
            set
            {
                string newBoundariesSDKTypeName = value == null ? null : value.type.FullName;
                if (boundariesSDKTypeName == newBoundariesSDKTypeName)
                {
                    return;
                }

                boundariesSDKTypeName = newBoundariesSDKTypeName;
                HandleSDKInfoSetter<SDK_BaseBoundaries>(value);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize the VR headset. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo headsetSDKInfo
        {
            get
            {
                if (cachedHeadsetSDKInfo == null || cachedHeadsetSDKInfo.type.FullName != headsetSDKTypeName)
                {
#if UNITY_EDITOR
                    DestroyImmediate(cachedHeadsetSDK);
#else
                    Destroy(cachedHeadsetSDK);
#endif
                    cachedHeadsetSDK = null;
                    cachedHeadsetSDKInfo = VRTK_SDKInfo.FromTypeName<SDK_BaseHeadset, SDK_FallbackHeadset>(headsetSDKTypeName);
                }

                return cachedHeadsetSDKInfo;
            }
            set
            {
                string newHeadsetSDKTypeName = value == null ? null : value.type.FullName;
                if (headsetSDKTypeName == newHeadsetSDKTypeName)
                {
                    return;
                }

                headsetSDKTypeName = newHeadsetSDKTypeName;
                HandleSDKInfoSetter<SDK_BaseHeadset>(value);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize the input devices. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo controllerSDKInfo
        {
            get
            {
                if (cachedControllerSDKInfo == null || cachedControllerSDKInfo.type.FullName != controllerSDKTypeName)
                {
#if UNITY_EDITOR
                    DestroyImmediate(cachedControllerSDK);
#else
                    Destroy(cachedControllerSDK);
#endif
                    cachedControllerSDK = null;
                    cachedControllerSDKInfo = VRTK_SDKInfo.FromTypeName<SDK_BaseController, SDK_FallbackController>(controllerSDKTypeName);
                }

                return cachedControllerSDKInfo;
            }
            set
            {
                string newControllerSDKTypeName = value == null ? null : value.type.FullName;
                if (controllerSDKTypeName == newControllerSDKTypeName)
                {
                    return;
                }

                controllerSDKTypeName = newControllerSDKTypeName;
                HandleSDKInfoSetter<SDK_BaseController>(value);
            }
        }

        [Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
        public GameObject actualBoundaries;
        [Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
        public GameObject actualHeadset;
        [Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
        public GameObject actualLeftController;
        [Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
        public GameObject actualRightController;

        [Header("Controller Aliases")]

        [Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
        public GameObject modelAliasLeftController;
        [Tooltip("A reference to the GameObject that models for the Right Hand Controller")]
        public GameObject modelAliasRightController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
        public GameObject scriptAliasLeftController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
        public GameObject scriptAliasRightController;

        /// <summary>
        /// Specifies the fallback SDK types for every base SDK type.
        /// </summary>
        private static readonly Dictionary<Type, Type> SDKFallbackTypesByBaseType = new Dictionary<Type, Type>
        {
            { typeof(SDK_BaseSystem), typeof(SDK_FallbackSystem) },
            { typeof(SDK_BaseBoundaries), typeof(SDK_FallbackBoundaries) },
            { typeof(SDK_BaseHeadset), typeof(SDK_FallbackHeadset) },
            { typeof(SDK_BaseController), typeof(SDK_FallbackController) }
        };

#if UNITY_EDITOR
        /// <summary>
        /// The previously active scene's path. Used to call <see cref="AutoManageScriptingDefineSymbolsAndPopulateObjectReferences"/> when the currently active scene changes. See the static constructor for the usage.
        /// </summary>
        private static string PreviousActiveScenePath;
#endif

        //the following fields in combination with the SDK info setters above are needed because Unity doesn't serialize custom generic classes
        //these should only be allowed to be changed in the above SDK info setters so their respective cached SDK will be invalidated correctly
        [SerializeField]
        private string systemSDKTypeName;
        [SerializeField]
        private string boundariesSDKTypeName;
        [SerializeField]
        private string headsetSDKTypeName;
        [SerializeField]
        private string controllerSDKTypeName;

        private VRTK_SDKInfo cachedSystemSDKInfo;
        private VRTK_SDKInfo cachedBoundariesSDKInfo;
        private VRTK_SDKInfo cachedHeadsetSDKInfo;
        private VRTK_SDKInfo cachedControllerSDKInfo;

        private SDK_BaseSystem cachedSystemSDK;
        private SDK_BaseBoundaries cachedBoundariesSDK;
        private SDK_BaseHeadset cachedHeadsetSDK;
        private SDK_BaseController cachedControllerSDK;

        /// <summary>
        /// The GetSystemSDK method returns the selected system SDK.
        /// </summary>
        /// <returns>The currently selected system SDK.</returns>
        public SDK_BaseSystem GetSystemSDK()
        {
            if (cachedSystemSDK == null)
            {
                HandleSDKGetter<SDK_BaseSystem>("System", systemSDKInfo, InstalledSystemSDKInfos);
                cachedSystemSDK = (SDK_BaseSystem)ScriptableObject.CreateInstance(systemSDKInfo.type);
            }

            return cachedSystemSDK;
        }

        /// <summary>
        /// The GetBoundariesSDK method returns the selected boundaries SDK.
        /// </summary>
        /// <returns>The currently selected boundaries SDK.</returns>
        public SDK_BaseBoundaries GetBoundariesSDK()
        {
            if (cachedBoundariesSDK == null)
            {
                HandleSDKGetter<SDK_BaseBoundaries>("Boundaries", boundariesSDKInfo, InstalledBoundariesSDKInfos);
                cachedBoundariesSDK = (SDK_BaseBoundaries)ScriptableObject.CreateInstance(boundariesSDKInfo.type);
            }

            return cachedBoundariesSDK;
        }

        /// <summary>
        /// The GetHeadsetSDK method returns the selected headset SDK.
        /// </summary>
        /// <returns>The currently selected headset SDK.</returns>
        public SDK_BaseHeadset GetHeadsetSDK()
        {
            if (cachedHeadsetSDK == null)
            {
                HandleSDKGetter<SDK_BaseHeadset>("Headset", headsetSDKInfo, InstalledHeadsetSDKInfos);
                cachedHeadsetSDK = (SDK_BaseHeadset)ScriptableObject.CreateInstance(headsetSDKInfo.type);
            }

            return cachedHeadsetSDK;
        }

        /// <summary>
        /// The GetControllerSDK method returns the selected controller SDK.
        /// </summary>
        /// <returns>The currently selected controller SDK.</returns>
        public SDK_BaseController GetControllerSDK()
        {
            if (cachedControllerSDK == null)
            {
                HandleSDKGetter<SDK_BaseController>("Controller", controllerSDKInfo, InstalledControllerSDKInfos);
                cachedControllerSDK = (SDK_BaseController)ScriptableObject.CreateInstance(controllerSDKInfo.type);
            }

            return cachedControllerSDK;
        }

        /// <summary>
        /// Populates the object references by using the currently set SDKs.
        /// </summary>
        /// <param name="force">Whether to ignore <see cref="autoPopulateObjectReferences"/> while deciding to populate.</param>
        public void PopulateObjectReferences(bool force)
        {
            if (!(force || autoPopulateObjectReferences))
            {
                return;
            }

            //log potential errors
            GetSystemSDK();
            SDK_BaseBoundaries boundariesSDK = GetBoundariesSDK();
            SDK_BaseHeadset headsetSDK = GetHeadsetSDK();
            SDK_BaseController controllerSDK = GetControllerSDK();

            Transform playAreaTransform = boundariesSDK.GetPlayArea();
            Transform headsetTransform = headsetSDK.GetHeadset();

            actualBoundaries = playAreaTransform == null ? null : playAreaTransform.gameObject;
            actualHeadset = headsetTransform == null ? null : headsetTransform.gameObject;
            actualLeftController = controllerSDK.GetControllerLeftHand(true);
            actualRightController = controllerSDK.GetControllerRightHand(true);
            modelAliasLeftController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Left);
            modelAliasRightController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Right);
        }

        /// <summary>
        /// Manages (i.e. adds and removes) the scripting define symbols of the <see cref="PlayerSettings"/> for the currently set SDK infos. This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used in a type that is also compiled for a standalone build.
        /// </summary>
        /// <param name="ignoreAutoManageScriptDefines">Whether to ignore <see cref="autoManageScriptDefines"/> while deciding to manage.</param>
        /// <param name="ignoreIsActiveAndEnabled">Whether to ignore <see cref="Behaviour.isActiveAndEnabled"/> while deciding to manage.</param>
        /// <returns>Whether the <see cref="PlayerSettings"/>' scripting define symbols were changed.</returns>
        public bool ManageScriptingDefineSymbols(bool ignoreAutoManageScriptDefines, bool ignoreIsActiveAndEnabled)
        {
            if (!((ignoreAutoManageScriptDefines || autoManageScriptDefines) && (ignoreIsActiveAndEnabled || isActiveAndEnabled)))
            {
                return false;
            }

            //get valid BuildTargetGroups
            BuildTargetGroup[] targetGroups = Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>().Where(group =>
            {
                if (group == BuildTargetGroup.Unknown)
                {
                    return false;
                }

                string targetGroupName = Enum.GetName(typeof(BuildTargetGroup), group);
                FieldInfo targetGroupFieldInfo = typeof(BuildTargetGroup).GetField(targetGroupName, BindingFlags.Public | BindingFlags.Static);

                return targetGroupFieldInfo != null && targetGroupFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0;
            }).ToArray();
            var newSymbolsByTargetGroup = new Dictionary<BuildTargetGroup, HashSet<string>>(targetGroups.Length);

            //get current non-removable scripting define symbols
            foreach (BuildTargetGroup targetGroup in targetGroups)
            {
                IEnumerable<string> nonSDKSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                    .Split(';')
                    .Where(symbol => !symbol.StartsWith(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix, StringComparison.Ordinal));
                newSymbolsByTargetGroup[targetGroup] = new HashSet<string>(nonSDKSymbols);
            }

            //get scripting define symbols for active SDKs and check whether the predicates allow us to add the symbols
            var activeSymbols = new[] { systemSDKInfo.description.symbol, boundariesSDKInfo.description.symbol, headsetSDKInfo.description.symbol, controllerSDKInfo.description.symbol };
            foreach (string activeSymbol in activeSymbols)
            {
                foreach (ScriptingDefineSymbolPredicateInfo predicateInfo in AvailableScriptingDefineSymbolPredicateInfos)
                {
                    MethodInfo methodInfo = predicateInfo.methodInfo;
                    if (predicateInfo.attribute.symbol != activeSymbol || !(bool)methodInfo.Invoke(null, null))
                    {
                        continue;
                    }

                    //add symbols from all predicate attributes on the method since multiple ones are allowed
                    var allAttributes = (SDK_ScriptingDefineSymbolPredicateAttribute[])methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), false);
                    foreach (SDK_ScriptingDefineSymbolPredicateAttribute attribute in allAttributes)
                    {
                        BuildTargetGroup buildTargetGroup = attribute.buildTargetGroup;
                        HashSet<string> newSymbols;
                        if (!newSymbolsByTargetGroup.TryGetValue(buildTargetGroup, out newSymbols))
                        {
                            newSymbols = new HashSet<string>();
                            newSymbolsByTargetGroup[buildTargetGroup] = newSymbols;
                        }

                        newSymbols.Add(attribute.symbol);
                    }
                }
            }

            var changedSymbols = false;

            //apply new set of scripting define symbols
            foreach (KeyValuePair<BuildTargetGroup, HashSet<string>> keyValuePair in newSymbolsByTargetGroup)
            {
                BuildTargetGroup targetGroup = keyValuePair.Key;
                string[] currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                    .Split(';')
                    .Distinct()
                    .OrderBy(symbol => symbol, StringComparer.Ordinal)
                    .ToArray();
                string[] newSymbols = keyValuePair.Value.OrderBy(symbol => symbol, StringComparer.Ordinal).ToArray();

                if (currentSymbols.SequenceEqual(newSymbols))
                {
                    continue;
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", newSymbols));

                string[] removedSymbols = currentSymbols.Except(newSymbols).ToArray();
                if (removedSymbols.Length > 0)
                {
                    Debug.Log("Scripting Define Symbols removed from [Project Settings->Player]: " + string.Join(", ", removedSymbols));
                }

                string[] addedSymbols = newSymbols.Except(currentSymbols).ToArray();
                if (addedSymbols.Length > 0)
                {
                    Debug.Log("Scripting Define Symbols added To [Project Settings->Player]: " + string.Join(", ", addedSymbols));
                }

                if (!changedSymbols)
                {
                    changedSymbols = removedSymbols.Length > 0 || addedSymbols.Length > 0;
                }
            }

            return changedSymbols;
        }

        static VRTK_SDKManager()
        {
            PopulateAvailableScriptingDefineSymbolPredicateInfos();
            PopulateAvailableAndInstalledSDKInfos();

#if UNITY_EDITOR
            //call AutoManageScriptingDefineSymbolsAndPopulateObjectReferences when the currently active scene changes
            EditorApplication.hierarchyWindowChanged += () =>
            {
                string currentActiveScenePath = SceneManager.GetActiveScene().path;
                if (currentActiveScenePath != PreviousActiveScenePath)
                {
                    PreviousActiveScenePath = currentActiveScenePath;
                    AutoManageScriptingDefineSymbolsAndPopulateObjectReferences();
                }
            };
#endif
        }

        protected virtual void Awake()
        {
            //log potential errors
            GetSystemSDK();
            GetBoundariesSDK();
            GetHeadsetSDK();
            GetControllerSDK();

            CreateInstance();
            SetupHeadset();
            SetupControllers();
            GetBoundariesSDK().InitBoundaries();
        }

        /// <summary>
        /// Populates <see cref="AvailableScriptingDefineSymbolPredicateInfos"/> with all the available <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/>s and associated method infos.
        /// </summary>
        private static void PopulateAvailableScriptingDefineSymbolPredicateInfos()
        {
            var predicateInfos = new List<ScriptingDefineSymbolPredicateInfo>();

            foreach (Type type in typeof(VRTK_SDKManager).Assembly.GetTypes())
            {
                for (var index = 0; index < type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Length; index++)
                {
                    MethodInfo methodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)[index];
                    var predicateAttributes = (SDK_ScriptingDefineSymbolPredicateAttribute[])methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), false);
                    if (predicateAttributes.Length == 0)
                    {
                        continue;
                    }

                    if (methodInfo.ReturnType != typeof(bool) || methodInfo.GetParameters().Length != 0)
                    {
                        throw new InvalidOperationException(string.Format("The method '{0}' on '{1}' has '{2}' specified but its signature is wrong. The method must take no arguments and return bool.", methodInfo.Name, type, typeof(SDK_ScriptingDefineSymbolPredicateAttribute)));
                    }

                    predicateInfos.AddRange(predicateAttributes.Select(predicateAttribute => new ScriptingDefineSymbolPredicateInfo(predicateAttribute, methodInfo)));
                }
            }

            predicateInfos.Sort((x, y) => string.Compare(x.attribute.symbol, y.attribute.symbol, StringComparison.Ordinal));

            AvailableScriptingDefineSymbolPredicateInfos = predicateInfos.AsReadOnly();
        }

        /// <summary>
        /// Populates the various lists of available and installed SDK infos.
        /// </summary>
        private static void PopulateAvailableAndInstalledSDKInfos()
        {
            List<string> symbolsOfInstalledSDKs = AvailableScriptingDefineSymbolPredicateInfos
                .Where(predicateInfo => (bool)predicateInfo.methodInfo.Invoke(null, null))
                .Select(predicateInfo => predicateInfo.attribute.symbol)
                .ToList();

            var availableSystemSDKInfos = new List<VRTK_SDKInfo>();
            var availableBoundariesSDKInfos = new List<VRTK_SDKInfo>();
            var availableHeadsetSDKInfos = new List<VRTK_SDKInfo>();
            var availableControllerSDKInfos = new List<VRTK_SDKInfo>();

            var installedSystemSDKInfos = new List<VRTK_SDKInfo>();
            var installedBoundariesSDKInfos = new List<VRTK_SDKInfo>();
            var installedHeadsetSDKInfos = new List<VRTK_SDKInfo>();
            var installedControllerSDKInfos = new List<VRTK_SDKInfo>();

            PopulateAvailableAndInstalledSDKInfos<SDK_BaseSystem>(availableSystemSDKInfos, installedSystemSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseBoundaries>(availableBoundariesSDKInfos, installedBoundariesSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseHeadset>(availableHeadsetSDKInfos, installedHeadsetSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseController>(availableControllerSDKInfos, installedControllerSDKInfos, symbolsOfInstalledSDKs);

            AvailableSystemSDKInfos = availableSystemSDKInfos.AsReadOnly();
            AvailableBoundariesSDKInfos = availableBoundariesSDKInfos.AsReadOnly();
            AvailableHeadsetSDKInfos = availableHeadsetSDKInfos.AsReadOnly();
            AvailableControllerSDKInfos = availableControllerSDKInfos.AsReadOnly();

            InstalledSystemSDKInfos = installedSystemSDKInfos.AsReadOnly();
            InstalledBoundariesSDKInfos = installedBoundariesSDKInfos.AsReadOnly();
            InstalledHeadsetSDKInfos = installedHeadsetSDKInfos.AsReadOnly();
            InstalledControllerSDKInfos = installedControllerSDKInfos.AsReadOnly();
        }

        /// <summary>
        /// Populates the lists of available and installed SDK infos for a specific SDK base type.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to populate the lists for. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="availableSDKInfos">The list of available SDK infos to populate.</param>
        /// <param name="installedSDKInfos">The list of installed SDK infos to populate.</param>
        /// <param name="symbolsOfInstalledSDKs">The list of symbols of all the installed SDKs.</param>
        private static void PopulateAvailableAndInstalledSDKInfos<BaseType>(List<VRTK_SDKInfo> availableSDKInfos, List<VRTK_SDKInfo> installedSDKInfos, ICollection<string> symbolsOfInstalledSDKs) where BaseType : SDK_Base
        {
            Type baseType = typeof(BaseType);
            Type fallbackType = SDKFallbackTypesByBaseType[baseType];

            availableSDKInfos.Add(VRTK_SDKInfo.FromType<BaseType>(fallbackType));
            availableSDKInfos.AddRange(baseType.Assembly.GetExportedTypes()
                .Where(type => type.IsSubclassOf(baseType) && type != fallbackType && !type.IsAbstract)
                .Select<Type, VRTK_SDKInfo>(VRTK_SDKInfo.FromType<BaseType>));
            availableSDKInfos.Sort((x, y) => string.Compare(x.description.prettyName, y.description.prettyName, StringComparison.Ordinal));

            installedSDKInfos.AddRange(availableSDKInfos.Where(info =>
            {
                string symbol = info.description.symbol;
                return string.IsNullOrEmpty(symbol) || symbolsOfInstalledSDKs.Contains(symbol);
            }));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Calls <see cref="ManageScriptingDefineSymbols"/> and <see cref="PopulateObjectReferences"/> (both without forcing) at the appropriate times when in the editor.
        /// </summary>
        [DidReloadScripts]
        private static void AutoManageScriptingDefineSymbolsAndPopulateObjectReferences()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            RemoveLegacyScriptingDefineSymbols();

            var sdkManager = FindObjectOfType<VRTK_SDKManager>();
            if (sdkManager != null && !sdkManager.ManageScriptingDefineSymbols(false, false))
            {
                sdkManager.PopulateObjectReferences(false);
            }

            PreviousActiveScenePath = SceneManager.GetActiveScene().path;
        }

        /// <summary>
        /// Removes scripting define symbols used by previous VRTK versions.
        /// </summary>
        private static void RemoveLegacyScriptingDefineSymbols()
        {
            string[] currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone)
                .Split(';')
                .Distinct()
                .OrderBy(symbol => symbol, StringComparer.Ordinal)
                .ToArray();
            string[] newSymbols = currentSymbols.Where(symbol => !symbol.StartsWith("VRTK_SDK_", StringComparison.Ordinal)).ToArray();

            if (!currentSymbols.SequenceEqual(newSymbols))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", newSymbols));

                string[] removedSymbols = currentSymbols.Except(newSymbols).ToArray();
                if (removedSymbols.Length > 0)
                {
                    Debug.Log("Legacy (i.e. used by previous VRTK versions only) Scripting Define Symbols removed from [Project Settings->Player]: " + string.Join(", ", removedSymbols));
                }
            }
        }
#endif

        private void SetupHeadset()
        {
            if (!actualHeadset.GetComponent<VRTK_TrackedHeadset>())
            {
                actualHeadset.AddComponent<VRTK_TrackedHeadset>();
            }
        }

        private void SetupControllers()
        {
            if (!actualLeftController.GetComponent<VRTK_TrackedController>())
            {
                actualLeftController.AddComponent<VRTK_TrackedController>();
            }

            if (!actualRightController.GetComponent<VRTK_TrackedController>())
            {
                actualRightController.AddComponent<VRTK_TrackedController>();
            }

            if (scriptAliasLeftController && !scriptAliasLeftController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasLeftController.AddComponent<VRTK_ControllerTracker>();
            }

            if (scriptAliasRightController && !scriptAliasRightController.GetComponent<VRTK_ControllerTracker>())
            {
                scriptAliasRightController.AddComponent<VRTK_ControllerTracker>();
            }
        }

        private void CreateInstance()
        {
            if (instance == null)
            {
                instance = this;

                if (persistOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Handles the various SDK getters by logging different errors in case any of these are true for the current SDK info:
        /// <list type="bullet">
        /// <item> <description>Its type doesn't exist anymore.</description> </item>
        /// <item> <description>It's a fallback SDK.</description> </item>
        /// <item> <description>It doesn't have its scripting define symbols added.</description> </item>
        /// <item> <description>It's missing its vendor SDK.</description> </item>
        /// </list>
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to handle the getter for. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="prettyName">The pretty name of the base SDK to use when logging errors.</param>
        /// <param name="info">The SDK info of which the SDK getter was called.</param>
        /// <param name="installedInfos">The installed SDK infos of which the SDK getter was called.</param>
        private void HandleSDKGetter<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
        {
            Type selectedType = info.type;
            Type baseType = typeof(BaseType);
            Type fallbackType = SDKFallbackTypesByBaseType[baseType];

            if (selectedType == fallbackType)
            {
                Debug.LogError(string.Format("The fallback {0} SDK is being used because there is no other {0} SDK set in the SDK manager.", prettyName));
            }
            else if (!baseType.IsAssignableFrom(selectedType) || fallbackType.IsAssignableFrom(selectedType))
            {
                string description = string.Format("The fallback {0} SDK is being used despite being set to '{1}'.", prettyName, selectedType.Name);

                if (installedInfos.Select(installedInfo => installedInfo.type).Contains(selectedType))
                {
                    Debug.LogError(description + " Its needed scripting define symbols are not added. You can click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and choose to automatically let the manager handle the scripting define symbols.");
                }
                else
                {
                    Debug.LogError(description + " The needed vendor SDK isn't installed.");
                }
            }
        }

        /// <summary>
        /// Handles the various SDK info setters by making sure to automatically manage the scripting define symbols or populate the object references if the SDK manager is set to do so.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to handle the setter for. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="info">The SDK info of which the SDK setter was called.</param>
        private void HandleSDKInfoSetter<BaseType>(VRTK_SDKInfo info) where BaseType : SDK_Base
        {
            if (info != null && !info.type.IsSubclassOf(typeof(BaseType)))
            {
                throw new ArgumentOutOfRangeException("info", info, string.Format("'{0}' is not a subclass of the SDK base type '{1}'.", info.type.Name, typeof(BaseType).Name));
            }

#if UNITY_EDITOR
            if (!ManageScriptingDefineSymbols(false, false))
            {
                PopulateObjectReferences(false);
            }
#else
            PopulateObjectReferences(false);
#endif
        }

        /// <summary>
        /// A helper class that simply holds references to both the <see cref="SDK_ScriptingDefineSymbolPredicateAttribute"/> and the method info of the method the attribute is defined on.
        /// </summary>
        public sealed class ScriptingDefineSymbolPredicateInfo
        {
            /// <summary>
            /// The predicate attribute.
            /// </summary>
            public readonly SDK_ScriptingDefineSymbolPredicateAttribute attribute;
            /// <summary>
            /// The method info of the method the attribute is defined on.
            /// </summary>
            public readonly MethodInfo methodInfo;

            /// <summary>
            /// Constructs a new instance with the specified predicate attribute and associated method info.
            /// </summary>
            /// <param name="attribute">The predicate attribute.</param>
            /// <param name="methodInfo">The method info of the method the attribute is defined on.</param>
            public ScriptingDefineSymbolPredicateInfo(SDK_ScriptingDefineSymbolPredicateAttribute attribute, MethodInfo methodInfo)
            {
                this.attribute = attribute;
                this.methodInfo = methodInfo;
            }
        }
    }
}