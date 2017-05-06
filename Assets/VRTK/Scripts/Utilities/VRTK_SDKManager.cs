// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;
#if UNITY_EDITOR
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
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_SDKManager")]
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
        public static VRTK_SDKManager instance
        {
            get
            {
                if (_instance == null)
                {
                    var sdkManager = FindObjectOfType<VRTK_SDKManager>();
                    if (sdkManager)
                    {
                        sdkManager.CreateInstance();
                    }
                }

                return _instance;
            }
        }
        private static VRTK_SDKManager _instance;

        [Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
        public bool persistOnLoad;

        [Tooltip("This determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.")]
        public bool autoPopulateObjectReferences = true;

        [Tooltip("This determines whether the scripting define symbols required by the selected SDKs are automatically added to and removed from the player settings. If this is true managing is done whenever the selected SDKs or the current active SDK Manager change in the Editor.")]
        public bool autoManageScriptDefines = true;

        /// <summary>
        /// The info of the SDK to use to deal with all system actions. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo systemSDKInfo
        {
            get
            {
                return cachedSystemSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>();
                if (cachedSystemSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedSystemSDK);
#else
                Destroy(cachedSystemSDK);
#endif
                cachedSystemSDK = null;

                cachedSystemSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize room scale boundaries. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo boundariesSDKInfo
        {
            get
            {
                return cachedBoundariesSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>();
                if (cachedBoundariesSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedBoundariesSDK);
#else
                Destroy(cachedBoundariesSDK);
#endif
                cachedBoundariesSDK = null;

                cachedBoundariesSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize the VR headset. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo headsetSDKInfo
        {
            get
            {
                return cachedHeadsetSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>();
                if (cachedHeadsetSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedHeadsetSDK);
#else
                Destroy(cachedHeadsetSDK);
#endif
                cachedHeadsetSDK = null;

                cachedHeadsetSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }
        /// <summary>
        /// The info of the SDK to use to utilize the input devices. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo controllerSDKInfo
        {
            get
            {
                return cachedControllerSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>();
                if (cachedControllerSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedControllerSDK);
#else
                Destroy(cachedControllerSDK);
#endif
                cachedControllerSDK = null;

                cachedControllerSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The active (i.e. to be added to the <see cref="PlayerSettings"/>) scripting define symbol predicate attributes that have no associated SDK classes.
        /// </summary>
        public List<SDK_ScriptingDefineSymbolPredicateAttribute> activeScriptingDefineSymbolsWithoutSDKClasses;

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
        [Tooltip("A reference to the GameObject that models for the Right Hand Controller.")]
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

        [SerializeField]
        private VRTK_SDKInfo cachedSystemSDKInfo = VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>();
        [SerializeField]
        private VRTK_SDKInfo cachedBoundariesSDKInfo = VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>();
        [SerializeField]
        private VRTK_SDKInfo cachedHeadsetSDKInfo = VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>();
        [SerializeField]
        private VRTK_SDKInfo cachedControllerSDKInfo = VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>();

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

            actualBoundaries = null;
            actualHeadset = null;
            actualLeftController = null;
            actualRightController = null;
            modelAliasLeftController = null;
            modelAliasRightController = null;

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

#if UNITY_EDITOR
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

            Func<VRTK_SDKInfo, string> symbolSelector = info => info.description.symbol;
            var sdkSymbols = new HashSet<string>(
                AvailableSystemSDKInfos.Select(symbolSelector)
                                       .Concat(AvailableBoundariesSDKInfos.Select(symbolSelector))
                                       .Concat(AvailableHeadsetSDKInfos.Select(symbolSelector))
                                       .Concat(AvailableControllerSDKInfos.Select(symbolSelector))
            );
            var activeSymbols = new HashSet<string>(activeScriptingDefineSymbolsWithoutSDKClasses.Select(attribute => attribute.symbol));

            //get scripting define symbols and check whether the predicates allow us to add the symbols
            foreach (ScriptingDefineSymbolPredicateInfo predicateInfo in AvailableScriptingDefineSymbolPredicateInfos)
            {
                string symbol = predicateInfo.attribute.symbol;
                if (!sdkSymbols.Contains(symbol) && !activeSymbols.Contains(symbol))
                {
                    continue;
                }

                MethodInfo methodInfo = predicateInfo.methodInfo;
                if (!(bool)methodInfo.Invoke(null, null))
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
                    VRTK_Logger.Info("Scripting Define Symbols removed from [Project Settings->Player]: " + string.Join(", ", removedSymbols));
                }

                string[] addedSymbols = newSymbols.Except(currentSymbols).ToArray();
                if (addedSymbols.Length > 0)
                {
                    VRTK_Logger.Info("Scripting Define Symbols added To [Project Settings->Player]: " + string.Join(", ", addedSymbols));
                }

                if (!changedSymbols)
                {
                    changedSymbols = removedSymbols.Length > 0 || addedSymbols.Length > 0;
                }
            }

            return changedSymbols;
        }
#endif

        public string[] GetSimplifiedSDKErrorDescriptions()
        {
            var sdkErrorDescriptions = new List<string>();

            var installedSDKInfosList = new[] { InstalledSystemSDKInfos, InstalledBoundariesSDKInfos, InstalledHeadsetSDKInfos, InstalledControllerSDKInfos };
            var currentSDKInfos = new[] { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo };

            for (var index = 0; index < installedSDKInfosList.Length; index++)
            {
                ReadOnlyCollection<VRTK_SDKInfo> installedSDKInfos = installedSDKInfosList[index];
                VRTK_SDKInfo currentSDKInfo = currentSDKInfos[index];

                Type baseType = currentSDKInfo.type.BaseType;
                if (baseType == null)
                {
                    continue;
                }

                string baseName = baseType.Name.Remove(0, typeof(SDK_Base).Name.Length);

                if (!installedSDKInfos.Contains(currentSDKInfo))
                {
                    sdkErrorDescriptions.Add(string.Format("The vendor SDK '{0}' is not installed.", currentSDKInfo.description.prettyName));
                }
                else if (currentSDKInfo.type == typeof(SDK_FallbackSystem))
                {
                    if (currentSDKInfo.originalTypeNameWhenFallbackIsUsed != null)
                    {
                        sdkErrorDescriptions.Add(string.Format("The SDK '{0}' doesn't exist anymore. The {1} fallback SDK will be used instead.", currentSDKInfo.originalTypeNameWhenFallbackIsUsed, baseName));
                    }
                    else
                    {
                        sdkErrorDescriptions.Add("A fallback SDK is used. Make sure to set a real SDK.");
                    }
                }
            }

            return sdkErrorDescriptions.Distinct().ToArray();
        }

        static VRTK_SDKManager()
        {
            PopulateAvailableScriptingDefineSymbolPredicateInfos();
            PopulateAvailableAndInstalledSDKInfos();

#if UNITY_EDITOR
            //call AutoManageScriptingDefineSymbolsAndPopulateObjectReferences when the currently active scene changes
            EditorApplication.hierarchyWindowChanged += AutoManageScriptingDefineSymbolsAndPopulateObjectReferences;
#endif
        }

        protected virtual void Awake()
        {
            CreateInstance();
            SetupHeadset();
            SetupControllers();
            GetBoundariesSDK().InitBoundaries();
            gameObject.AddComponent<VRTK_InstanceMethods>();
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

            PopulateAvailableAndInstalledSDKInfos<SDK_BaseSystem, SDK_FallbackSystem>(availableSystemSDKInfos, installedSystemSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseBoundaries, SDK_FallbackBoundaries>(availableBoundariesSDKInfos, installedBoundariesSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseHeadset, SDK_FallbackHeadset>(availableHeadsetSDKInfos, installedHeadsetSDKInfos, symbolsOfInstalledSDKs);
            PopulateAvailableAndInstalledSDKInfos<SDK_BaseController, SDK_FallbackController>(availableControllerSDKInfos, installedControllerSDKInfos, symbolsOfInstalledSDKs);

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
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of <typeparamref name="BaseType"/>.</typeparam>
        /// <param name="availableSDKInfos">The list of available SDK infos to populate.</param>
        /// <param name="installedSDKInfos">The list of installed SDK infos to populate.</param>
        /// <param name="symbolsOfInstalledSDKs">The list of symbols of all the installed SDKs.</param>
        private static void PopulateAvailableAndInstalledSDKInfos<BaseType, FallbackType>(List<VRTK_SDKInfo> availableSDKInfos, List<VRTK_SDKInfo> installedSDKInfos, ICollection<string> symbolsOfInstalledSDKs) where BaseType : SDK_Base where FallbackType : BaseType
        {
            Type baseType = typeof(BaseType);
            Type fallbackType = SDKFallbackTypesByBaseType[baseType];

            availableSDKInfos.Add(VRTK_SDKInfo.Create<BaseType, FallbackType, FallbackType>());
            availableSDKInfos.AddRange(baseType.Assembly.GetExportedTypes()
                .Where(type => type.IsSubclassOf(baseType) && type != fallbackType && !type.IsAbstract)
                .Select<Type, VRTK_SDKInfo>(VRTK_SDKInfo.Create<BaseType, FallbackType>));
            availableSDKInfos.Sort((x, y) => x.description.prettyName == "Fallback"
                ? -1 //the fallback SDK should always be the first SDK in the list
                : string.Compare(x.description.prettyName, y.description.prettyName, StringComparison.Ordinal));

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

            if (instance != null && !instance.ManageScriptingDefineSymbols(false, false))
            {
                instance.PopulateObjectReferences(false);
            }
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
                    VRTK_Logger.Info("Legacy (i.e. used by previous VRTK versions only) Scripting Define Symbols removed from [Project Settings->Player]: " + string.Join(", ", removedSymbols));
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
            if (actualLeftController && !actualLeftController.GetComponent<VRTK_TrackedController>())
            {
                actualLeftController.AddComponent<VRTK_TrackedController>();
            }

            if (actualRightController && !actualRightController.GetComponent<VRTK_TrackedController>())
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
            if (_instance == null)
            {
                _instance = this;
                VRTK_SDK_Bridge.InvalidateCaches();

                string sdkErrorDescriptions = string.Join("\n- ", GetSimplifiedSDKErrorDescriptions());
                if (!string.IsNullOrEmpty(sdkErrorDescriptions))
                {
                    sdkErrorDescriptions = "- " + sdkErrorDescriptions;
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_MANAGER_ERRORS, new string[] { sdkErrorDescriptions }));
                }

                if (persistOnLoad && !VRTK_SharedMethods.IsEditTime())
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Handles the various SDK getters by logging potential errors.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to handle the getter for. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="prettyName">The pretty name of the base SDK to use when logging errors.</param>
        /// <param name="info">The SDK info of which the SDK getter was called.</param>
        /// <param name="installedInfos">The installed SDK infos of which the SDK getter was called.</param>
        private static void HandleSDKGetter<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                return;
            }

            string sdkErrorDescription = GetSDKErrorDescription<BaseType>(prettyName, info, installedInfos);
            if (!string.IsNullOrEmpty(sdkErrorDescription))
            {
                VRTK_Logger.Error(sdkErrorDescription);
            }
        }

        /// <summary>
        /// Returns an error description in case any of these are true for the current SDK info:
        /// <list type="bullet">
        /// <item> <description>Its type doesn't exist anymore.</description> </item>
        /// <item> <description>It's a fallback SDK.</description> </item>
        /// <item> <description>It doesn't have its scripting define symbols added.</description> </item>
        /// <item> <description>It's missing its vendor SDK.</description> </item>
        /// </list>
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to return the error description for. Must be a subclass of <see cref="SDK_Base"/>.</typeparam>
        /// <param name="prettyName">The pretty name of the base SDK to use when returning error descriptions.</param>
        /// <param name="info">The SDK info of which to return the error description for.</param>
        /// <param name="installedInfos">The installed SDK infos.</param>
        /// <returns>An error description if there is one, else <see langword="null"/>.</returns>
        private static string GetSDKErrorDescription<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
        {
            Type selectedType = info.type;
            Type baseType = typeof(BaseType);
            Type fallbackType = SDKFallbackTypesByBaseType[baseType];

            if (selectedType == fallbackType)
            {
                return string.Format("The fallback {0} SDK is being used because there is no other {0} SDK set in the SDK Manager.", prettyName);
            }

            if (!baseType.IsAssignableFrom(selectedType) || fallbackType.IsAssignableFrom(selectedType))
            {
                string description = string.Format("The fallback {0} SDK is being used despite being set to '{1}'.", prettyName, selectedType.Name);

                if (installedInfos.Select(installedInfo => installedInfo.type).Contains(selectedType))
                {
                    return description + " Its needed scripting define symbols are not added. You can click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and choose to automatically let the manager handle the scripting define symbols.";
                }

                return description + " The needed vendor SDK isn't installed.";
            }

            return null;
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