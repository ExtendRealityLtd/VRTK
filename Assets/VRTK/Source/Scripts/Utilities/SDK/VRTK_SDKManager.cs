﻿// SDK Manager|Utilities|90010
namespace VRTK
{
    using UnityEngine;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRSettings = UnityEngine.VR.VRSettings;
    using XRDevice = UnityEngine.VR.VRDevice;
#endif
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditorInternal.VR;
#endif
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The SDK Manager script provides configuration of supported SDKs and manages a list of VRTK_SDKSetups to use.
    /// </summary>
    public sealed class VRTK_SDKManager : MonoBehaviour
    {
        /// <summary>
        /// A helper class that simply holds references to both the SDK_ScriptingDefineSymbolPredicateAttribute and the method info of the method the attribute is defined on.
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
            /// Event Payload. Constructs a new instance with the specified predicate attribute and associated method info.
            /// </summary>
            /// <param name="attribute">The predicate attribute.</param>
            /// <param name="methodInfo">The method info of the method the attribute is defined on.</param>
            public ScriptingDefineSymbolPredicateInfo(SDK_ScriptingDefineSymbolPredicateAttribute attribute, MethodInfo methodInfo)
            {
                this.attribute = attribute;
                this.methodInfo = methodInfo;
            }
        }

        /// <summary>
        /// Event Payload
        /// </summary>
        /// <param name="previousSetup">The previous loaded Setup. `null` if no previous Setup was loaded.</param>
        /// <param name="currentSetup">The current loaded Setup. `null` if no Setup is loaded anymore. See `errorMessage` to check whether this is `null` because of an error.</param>
        /// <param name="errorMessage">Explains why loading a list of Setups wasn't successful if `currentSetup` is `null` and an error occurred. `null` if no error occurred.</param>
        public struct LoadedSetupChangeEventArgs
        {
            public readonly VRTK_SDKSetup previousSetup;
            public readonly VRTK_SDKSetup currentSetup;
            public readonly string errorMessage;

            public LoadedSetupChangeEventArgs(VRTK_SDKSetup previousSetup, VRTK_SDKSetup currentSetup, string errorMessage)
            {
                this.previousSetup = previousSetup;
                this.currentSetup = currentSetup;
                this.errorMessage = errorMessage;
            }
        }

        /// <summary>
        /// Event Payload
        /// </summary>
        /// <param name="sender">this object</param>
        /// <param name="e"><see cref="LoadedSetupChangeEventArgs"/></param>
        public delegate void LoadedSetupChangeEventHandler(VRTK_SDKManager sender, LoadedSetupChangeEventArgs e);

        /// <summary>
        /// All found scripting define symbol predicate attributes with associated method info.
        /// </summary>
        public static ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> AvailableScriptingDefineSymbolPredicateInfos { get; private set; }

        /// <summary>
        /// Specifies the fallback SDK types for every base SDK type.
        /// </summary>
        public static readonly Dictionary<Type, Type> SDKFallbackTypesByBaseType = new Dictionary<Type, Type>
        {
            { typeof(SDK_BaseSystem), typeof(SDK_FallbackSystem) },
            { typeof(SDK_BaseBoundaries), typeof(SDK_FallbackBoundaries) },
            { typeof(SDK_BaseHeadset), typeof(SDK_FallbackHeadset) },
            { typeof(SDK_BaseController), typeof(SDK_FallbackController) }
        };

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
        /// All installed system SDK infos. This is a subset of `AvailableSystemSDKInfos`.
        /// It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists that
        /// uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledSystemSDKInfos { get; private set; }
        /// <summary>
        /// All installed boundaries SDK infos. This is a subset of `AvailableBoundariesSDKInfos`.
        /// It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists
        /// that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledBoundariesSDKInfos { get; private set; }
        /// <summary>
        /// All installed headset SDK infos. This is a subset of `AvailableHeadsetSDKInfos`.
        /// It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists
        /// that uses the same symbol and whose associated method returns true.
        /// </summary>
        public static ReadOnlyCollection<VRTK_SDKInfo> InstalledHeadsetSDKInfos { get; private set; }
        /// <summary>
        /// All installed controller SDK infos. This is a subset of `AvailableControllerSDKInfos`.
        /// It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists
        /// that uses the same symbol and whose associated method returns true.
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
                    VRTK_SDKManager sdkManager = VRTK_SharedMethods.FindEvenInactiveComponent<VRTK_SDKManager>(true);
                    if (sdkManager != null)
                    {
                        sdkManager.CreateInstance();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// A collection of behaviours to toggle on loaded setup change.
        /// </summary>
        public static HashSet<Behaviour> delayedToggleBehaviours = new HashSet<Behaviour>();

        /// <summary>
        /// The ValidInstance method returns whether the SDK Manager isntance is valid (i.e. it's not null).
        /// </summary>
        /// <returns>Returns `true` if the SDK Manager instance is valid or returns `false` if it is null.</returns>
        public static bool ValidInstance()
        {
            return (instance != null);
        }

        /// <summary>
        /// The AttemptAddBehaviourToToggleOnLoadedSetupChange method will attempt to add the given behaviour to the loaded setup change toggle if the SDK Manager instance exists. If it doesn't exist then it adds it to the `delayedToggleBehaviours` HashSet to be manually added later with the `ProcessDelayedToggleBehaviours` method.
        /// </summary>
        /// <param name="givenBehaviour">The behaviour to add.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool AttemptAddBehaviourToToggleOnLoadedSetupChange(Behaviour givenBehaviour)
        {
            if (ValidInstance())
            {
                instance.AddBehaviourToToggleOnLoadedSetupChange(givenBehaviour);
                return true;
            }
            delayedToggleBehaviours.Add(givenBehaviour);
            return false;
        }

        /// <summary>
        /// The AttemptRemoveBehaviourToToggleOnLoadedSetupChange method will attempt to remove the given behaviour from the loaded setup change toggle if the SDK Manager instance exists.
        /// </summary>
        /// <param name="givenBehaviour">The behaviour to remove.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool AttemptRemoveBehaviourToToggleOnLoadedSetupChange(Behaviour givenBehaviour)
        {
            if (ValidInstance())
            {
                instance.RemoveBehaviourToToggleOnLoadedSetupChange(givenBehaviour);
                delayedToggleBehaviours.Remove(givenBehaviour);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The ProcessDelayedToggleBehaviours method will attempt to addd the behaviours in the `delayedToggleBehaviours` HashSet to the loaded setup change toggle.
        /// </summary>
        public static void ProcessDelayedToggleBehaviours()
        {
            if (ValidInstance())
            {
                foreach (Behaviour currentBehaviour in new HashSet<Behaviour>(delayedToggleBehaviours))
                {
                    instance.AddBehaviourToToggleOnLoadedSetupChange(currentBehaviour);
                }
                delayedToggleBehaviours.Clear();
            }
        }

        /// <summary>
        /// The SubscribeLoadedSetupChanged method attempts to register the given callback with the `LoadedSetupChanged` event.
        /// </summary>
        /// <param name="callback">The callback to register.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool SubscribeLoadedSetupChanged(LoadedSetupChangeEventHandler callback)
        {
            if (ValidInstance())
            {
                instance.LoadedSetupChanged += callback;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The UnsubscribeLoadedSetupChanged method attempts to unregister the given callback from the `LoadedSetupChanged` event. 
        /// </summary>
        /// <param name="callback">The callback to unregister.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool UnsubscribeLoadedSetupChanged(LoadedSetupChangeEventHandler callback)
        {
            if (ValidInstance())
            {
                instance.LoadedSetupChanged -= callback;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The GetLoadedSDKSetup method returns the current loaded SDK Setup for the SDK Manager instance.
        /// </summary>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static VRTK_SDKSetup GetLoadedSDKSetup()
        {
            if (ValidInstance())
            {
                return instance.loadedSetup;
            }
            return null;
        }

        /// <summary>
        /// The GetAllSDKSetups method returns all valid SDK Setups attached to the SDK Manager instance.
        /// </summary>
        /// <returns>An SDKSetup array of all valid SDK Setups for the current SDK Manager instance. If no SDK Manager instance is found then an empty array is returned.</returns>
        public static VRTK_SDKSetup[] GetAllSDKSetups()
        {
            if (ValidInstance())
            {
                return instance.setups;
            }
            return new VRTK_SDKSetup[0];
        }

        /// <summary>
        /// The AttemptTryLoadSDKSetup method attempts to load a valid VRTK_SDKSetup from a list if the SDK Manager instance is valid.
        /// </summary>
        /// <param name="startIndex">The index of the VRTK_SDKSetup to start the loading with.</param>
        /// <param name="tryToReinitialize">Whether or not to retry initializing and using the currently set but unusable VR Device.</param>
        /// <param name="sdkSetups">The list to try to load a VRTK_SDKSetup from.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool AttemptTryLoadSDKSetup(int startIndex, bool tryToReinitialize, params VRTK_SDKSetup[] sdkSetups)
        {
            if (ValidInstance())
            {
                instance.TryLoadSDKSetup(startIndex, tryToReinitialize, sdkSetups);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The AttemptUnloadSDKSetup method tries to load a valid VRTK_SDKSetup from setups if the SDK Manager instance is valid.
        /// </summary>
        /// <param name="tryUseLastLoadedSetup">Attempt to use the last loaded setup if it's available.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool AttemptTryLoadSDKSetupFromList(bool tryUseLastLoadedSetup = true)
        {
            if (ValidInstance())
            {
                instance.TryLoadSDKSetupFromList(tryUseLastLoadedSetup);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The AttemptUnloadSDKSetup method attempts to unload the currently loaded VRTK_SDKSetup, if there is one and if the SDK Manager instance is valid.
        /// </summary>
        /// <param name="disableVR">Whether to disable VR altogether after unloading the SDK Setup.</param>
        /// <returns>Returns `true` if the SDK Manager instance was valid.</returns>
        public static bool AttemptUnloadSDKSetup(bool disableVR = false)
        {
            if (ValidInstance())
            {
                instance.UnloadSDKSetup(disableVR);
                return true;
            }
            return false;
        }

        private static VRTK_SDKManager _instance;

        [Tooltip("Determines whether the scripting define symbols required by the installed SDKs are automatically added to and removed from the player settings.")]
        public bool autoManageScriptDefines = true;

        /// <summary>
        /// The active (i.e. to be added to the PlayerSettings) scripting define symbol predicate attributes that have no associated SDK classes.
        /// </summary>
        public List<SDK_ScriptingDefineSymbolPredicateAttribute> activeScriptingDefineSymbolsWithoutSDKClasses = new List<SDK_ScriptingDefineSymbolPredicateAttribute>();

        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
        public GameObject scriptAliasLeftController;
        [Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
        public GameObject scriptAliasRightController;

        [Tooltip("Determines whether the VR settings of the Player Settings are automatically adjusted to allow for all the used SDKs in the SDK Setups list below.")]
        public bool autoManageVRSettings = true;
        [Tooltip("Determines whether the SDK Setups list below is used whenever the SDK Manager is enabled. The first loadable Setup is then loaded.")]
        public bool autoLoadSetup = true;
        [Tooltip("The list of SDK Setups to choose from.")]
        public VRTK_SDKSetup[] setups = new VRTK_SDKSetup[0];
#if UNITY_EDITOR
        [Tooltip("The list of Build Target Groups to exclude.")]
        public BuildTargetGroup[] excludeTargetGroups = new BuildTargetGroup[] {
#if UNITY_2017_1_OR_NEWER
            BuildTargetGroup.Switch,
            BuildTargetGroup.Facebook
#endif
        };
#endif

        [Header("Obsolete Settings")]

        [Obsolete("`VRTK_SDKManager.persistOnLoad` has been deprecated and will be removed in a future version of VRTK. See https://github.com/thestonefox/VRTK/issues/1316 for details.")]
        [ObsoleteInspector]
        public bool persistOnLoad;

        /// <summary>
        /// The loaded SDK Setup. `null` if no setup is currently loaded.
        /// </summary>
        public VRTK_SDKSetup loadedSetup
        {
            get
            {
                if (_loadedSetup == null && setups.Length == 1 && setups[0].isValid && setups[0].isActiveAndEnabled)
                {
                    _loadedSetup = setups[0];
                }

                return _loadedSetup;
            }
            private set { _loadedSetup = value; }
        }

        private VRTK_SDKSetup _loadedSetup;
        private static HashSet<VRTK_SDKInfo> _previouslyUsedSetupInfos = new HashSet<VRTK_SDKInfo>();

        /// <summary>
        /// All behaviours that need toggling whenever `loadedSetup` changes.
        /// </summary>
        public ReadOnlyCollection<Behaviour> behavioursToToggleOnLoadedSetupChange { get; private set; }
        private List<Behaviour> _behavioursToToggleOnLoadedSetupChange = new List<Behaviour>();
        private Dictionary<Behaviour, bool> _behavioursInitialState = new Dictionary<Behaviour, bool>();
        private Coroutine checkLeftControllerReadyRoutine = null;
        private Coroutine checkRightControllerReadyRoutine = null;
        private float checkControllerReadyDelay = 1f;
        private int checkControllerValidTimer = 50;
#if UNITY_EDITOR
        private BuildTargetGroup[] targetGroupsToExclude;
#endif
        /// <summary>
        /// The event invoked whenever the loaded SDK Setup changes.
        /// </summary>
        public event LoadedSetupChangeEventHandler LoadedSetupChanged;

#if UNITY_EDITOR
        /// <summary>
        /// The ManageScriptingDefineSymbols method manages (i.e. adds and removes) the scripting define symbols of the PlayerSettings for the currently set SDK infos.
        /// This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used
        /// in a type that is also compiled for a standalone build.
        /// </summary>
        /// <param name="ignoreAutoManageScriptDefines">Whether to ignore `autoManageScriptDefines` while deciding to manage.</param>
        /// <param name="ignoreIsActiveAndEnabled">Whether to ignore `Behaviour.isActiveAndEnabled` while deciding to manage.</param>
        /// <returns>Whether the PlayerSettings' scripting define symbols were changed.</returns>
        public bool ManageScriptingDefineSymbols(bool ignoreAutoManageScriptDefines, bool ignoreIsActiveAndEnabled)
        {
            if (!((ignoreAutoManageScriptDefines || autoManageScriptDefines) && (ignoreIsActiveAndEnabled || isActiveAndEnabled)))
            {
                return false;
            }

            //get valid BuildTargetGroups
            BuildTargetGroup[] targetGroups = VRTK_SharedMethods.GetValidBuildTargetGroups();
            Dictionary<BuildTargetGroup, HashSet<string>> newSymbolsByTargetGroup = new Dictionary<BuildTargetGroup, HashSet<string>>(targetGroups.Length);

            //get current non-removable scripting define symbols
            foreach (BuildTargetGroup targetGroup in targetGroups)
            {
                IEnumerable<string> nonSDKSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                    .Split(';')
                    .Where(symbol => !symbol.StartsWith(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix, StringComparison.Ordinal));
                newSymbolsByTargetGroup[targetGroup] = new HashSet<string>(nonSDKSymbols);
            }

            VRTK_SDKInfo[] availableSDKInfos = AvailableSystemSDKInfos
                .Concat(AvailableBoundariesSDKInfos)
                .Concat(AvailableHeadsetSDKInfos)
                .Concat(AvailableControllerSDKInfos)
                .ToArray();

            HashSet<SDK_DescriptionAttribute> descriptions = new HashSet<SDK_DescriptionAttribute>(
                availableSDKInfos.Select(info => info.description)
                                 .Where(description => !description.describesFallbackSDK)
            );
            HashSet<string> activeSymbols = new HashSet<string>(activeScriptingDefineSymbolsWithoutSDKClasses.Select(attribute => attribute.symbol));

            //get scripting define symbols and check whether the predicates allow us to add the symbols
            foreach (ScriptingDefineSymbolPredicateInfo predicateInfo in AvailableScriptingDefineSymbolPredicateInfos)
            {
                SDK_ScriptingDefineSymbolPredicateAttribute predicateAttribute = predicateInfo.attribute;
                string symbol = predicateAttribute.symbol;
                if (!activeSymbols.Contains(symbol)
                    && !descriptions.Any(description => description.symbol == symbol
                                                        && description.buildTargetGroup == predicateAttribute.buildTargetGroup))
                {
                    continue;
                }

                MethodInfo methodInfo = predicateInfo.methodInfo;
                if (!(bool)methodInfo.Invoke(null, null))
                {
                    continue;
                }

                //add symbols from all predicate attributes on the method since multiple ones are allowed
                SDK_ScriptingDefineSymbolPredicateAttribute[] allAttributes = (SDK_ScriptingDefineSymbolPredicateAttribute[])methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), false);
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

            bool changedSymbols = false;

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
                    VRTK_Logger.Info(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_REMOVED, targetGroup, string.Join(", ", removedSymbols)));
                }

                string[] addedSymbols = newSymbols.Except(currentSymbols).ToArray();
                if (addedSymbols.Length > 0)
                {
                    VRTK_Logger.Info(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_ADDED, targetGroup, string.Join(", ", addedSymbols)));
                }

                if (!changedSymbols)
                {
                    changedSymbols = removedSymbols.Length > 0 || addedSymbols.Length > 0;
                }
            }

            return changedSymbols;
        }

        /// <summary>
        /// The ManageVRSettings method manages (i.e. adds and removes) the VR SDKs of the PlayerSettings for the currently set SDK infos.
        /// This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used
        /// in a type that is also compiled for a standalone build.
        /// </summary>
        /// <param name="force">Whether to ignore `autoManageVRSettings` while deciding to manage.</param>
        public void ManageVRSettings(bool force)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !(force || autoManageVRSettings))
            {
                return;
            }

            Dictionary<BuildTargetGroup, string[]> deviceNamesByTargetGroup = setups
                .Where(setup => setup != null && setup.isValid)
                .SelectMany(setup => new[]
                {
                    setup.systemSDKInfo, setup.boundariesSDKInfo, setup.headsetSDKInfo, setup.controllerSDKInfo
                })
                .GroupBy(info => info.description.buildTargetGroup)
                .ToDictionary(grouping => grouping.Key,
                              grouping => grouping.Select(info => info.description.vrDeviceName)
                                                  .Distinct()
                                                  .ToArray());

            foreach (BuildTargetGroup targetGroup in VRTK_SharedMethods.GetValidBuildTargetGroups())
            {
                if (targetGroupsToExclude.Contains(targetGroup))
                {
                    continue;
                }
                string[] deviceNames;
                deviceNamesByTargetGroup.TryGetValue(targetGroup, out deviceNames);

                int setupCount = deviceNames == null ? 0 : deviceNames.Length;
                bool vrEnabled = deviceNames != null && deviceNames.Length > 0;

                if (deviceNames != null)
                {
                    deviceNames = deviceNames.Except(new[] { "None" }).ToArray();
                }

#if UNITY_5_5_OR_NEWER
                VREditor.SetVREnabledOnTargetGroup(targetGroup, vrEnabled);
#else
                VREditor.SetVREnabled(targetGroup, vrEnabled);
#endif

                string[] devices;
                if (vrEnabled)
                {
                    devices = setupCount > 1
                                  ? new[] { "None" }.Concat(deviceNames).ToArray()
                                  : deviceNames;
                }
                else
                {
                    devices = new string[0];
                }

#if UNITY_5_5_OR_NEWER
                VREditor.SetVREnabledDevicesOnTargetGroup(
#else
                VREditor.SetVREnabledDevices(
#endif
                    targetGroup,
                    devices
                );
            }
        }
#endif

        /// <summary>
        /// The AddBehaviourToToggleOnLoadedSetupChange method adds a behaviour to the list of behaviours to toggle when `loadedSetup` changes.
        /// </summary>
        /// <param name="behaviour">The behaviour to add.</param>
        public void AddBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)
        {
            if (!_behavioursToToggleOnLoadedSetupChange.Contains(behaviour))
            {
                _behavioursToToggleOnLoadedSetupChange.Add(behaviour);
                _behavioursInitialState.Add(behaviour, behaviour.enabled);
            }

            if (loadedSetup == null && behaviour.enabled)
            {
                behaviour.enabled = false;
            }
        }

        /// <summary>
        /// The RemoveBehaviourToToggleOnLoadedSetupChange method removes a behaviour of the list of behaviours to toggle when `loadedSetup` changes.
        /// </summary>
        /// <param name="behaviour">The behaviour to remove.</param>
        public void RemoveBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)
        {
            _behavioursToToggleOnLoadedSetupChange.Remove(behaviour);
        }

        /// <summary>
        /// The TryLoadSDKSetupFromList method tries to load a valid VRTK_SDKSetup from setups.
        /// </summary>
        /// <param name="tryUseLastLoadedSetup">Attempt to use the last loaded setup if it's available.</param>
        public void TryLoadSDKSetupFromList(bool tryUseLastLoadedSetup = true)
        {
            int index = 0;

            if (tryUseLastLoadedSetup && _previouslyUsedSetupInfos.Count > 0)
            {
                index = Array.FindIndex(
                    setups,
                    setup => _previouslyUsedSetupInfos.SetEquals(
                        new[]
                        {
                            setup.systemSDKInfo,
                            setup.boundariesSDKInfo,
                            setup.headsetSDKInfo,
                            setup.controllerSDKInfo
                        })
                );
            }
            else if (XRSettings.enabled)
            {
                // Use the SDK Setup for the current VR Device if it's working already
                // (may be due to command line argument '-vrmode')
                index = Array.FindIndex(
                    setups,
                    setup => setup.usedVRDeviceNames.Contains(XRSettings.loadedDeviceName)
                );
            }
            else
            {
                // If '-vrmode none' was used try to load the respective SDK Setup
                string[] commandLineArgs = Environment.GetCommandLineArgs();
                int commandLineArgIndex = Array.IndexOf(commandLineArgs, "-vrmode", 1);
                if (XRSettings.loadedDeviceName == "None"
                    || (commandLineArgIndex != -1
                        && commandLineArgIndex + 1 < commandLineArgs.Length
                        && commandLineArgs[commandLineArgIndex + 1].ToLowerInvariant() == "none"))
                {
                    index = Array.FindIndex(
                        setups,
                        setup => setup.usedVRDeviceNames.All(vrDeviceName => vrDeviceName == "None")
                    );
                }
            }

            index = index == -1 ? 0 : index;
            TryLoadSDKSetup(index, false, setups.ToArray());
        }

        /// <summary>
        /// The TryLoadSDKSetup method tries to load a valid VRTK_SDKSetup from a list.
        /// </summary>
        /// <remarks>
        /// The first loadable VRTK_SDKSetup in the list will be loaded. Will fall back to disable VR if none of the provided Setups is useable.
        /// </remarks>
        /// <param name="startIndex">The index of the VRTK_SDKSetup to start the loading with.</param>
        /// <param name="tryToReinitialize">Whether or not to retry initializing and using the currently set but unusable VR Device.</param>
        /// <param name="sdkSetups">The list to try to load a VRTK_SDKSetup from.</param>
        public void TryLoadSDKSetup(int startIndex, bool tryToReinitialize, params VRTK_SDKSetup[] sdkSetups)
        {
            if (sdkSetups.Length == 0)
            {
                return;
            }

            if (startIndex < 0 || startIndex >= sdkSetups.Length)
            {
                VRTK_Logger.Fatal(new ArgumentOutOfRangeException("startIndex"));
                return;
            }

            sdkSetups = sdkSetups.ToList()
                                 .GetRange(startIndex, sdkSetups.Length - startIndex)
                                 .ToArray();

            foreach (VRTK_SDKSetup invalidSetup in sdkSetups.Where(setup => !setup.isValid))
            {
                string setupErrorDescriptions = string.Join("\n- ", invalidSetup.GetSimplifiedErrorDescriptions());
                if (!string.IsNullOrEmpty(setupErrorDescriptions))
                {
                    setupErrorDescriptions = "- " + setupErrorDescriptions;
                    VRTK_Logger.Warn(string.Format("Ignoring SDK Setup '{0}' because there are some errors with it:\n{1}", invalidSetup.name, setupErrorDescriptions));
                }
            }

            sdkSetups = sdkSetups.Where(setup => setup.isValid).ToArray();

            VRTK_SDKSetup previousLoadedSetup = loadedSetup;
            ToggleBehaviours(false);
            loadedSetup = null;
            if (previousLoadedSetup != null)
            {
                previousLoadedSetup.OnUnloaded(this);
            }

            string loadedDeviceName = string.IsNullOrEmpty(XRSettings.loadedDeviceName) ? "None" : XRSettings.loadedDeviceName;
            bool isDeviceAlreadyLoaded = sdkSetups[0].usedVRDeviceNames.Contains(loadedDeviceName);
            if (!isDeviceAlreadyLoaded)
            {
                if (!tryToReinitialize && !XRSettings.enabled && loadedDeviceName != "None")
                {
                    sdkSetups = sdkSetups.Where(setup => !setup.usedVRDeviceNames.Contains(loadedDeviceName))
                                         .ToArray();
                }

                VRTK_SDKSetup[] missingVRDeviceSetups = sdkSetups
                    .Where(setup => setup.usedVRDeviceNames.Except(XRSettings.supportedDevices.Concat(new[] { "None" })).Any())
                    .ToArray();
                foreach (VRTK_SDKSetup missingVRDeviceSetup in missingVRDeviceSetups)
                {
                    string missingVRDevicesText = string.Join(
                        ", ",
                        missingVRDeviceSetup.usedVRDeviceNames
                                            .Except(XRSettings.supportedDevices)
                                            .ToArray()
                    );
                    VRTK_Logger.Warn(string.Format("Ignoring SDK Setup '{0}' because the following VR device names are missing from the PlayerSettings:\n{1}",
                                                   missingVRDeviceSetup.name,
                                                   missingVRDevicesText));
                }

                sdkSetups = sdkSetups.Except(missingVRDeviceSetups).ToArray();
                string[] vrDeviceNames = sdkSetups
                    .SelectMany(setup => setup.usedVRDeviceNames)
                    .Distinct()
                    .Concat(new[] { "None" }) // Add "None" to the end to fall back to
                    .ToArray();
                XRSettings.LoadDeviceByName(vrDeviceNames);
            }

            StartCoroutine(FinishSDKSetupLoading(sdkSetups, previousLoadedSetup));
        }

#if UNITY_EDITOR
        /// <summary>
        /// The SetLoadedSDKSetupToPopulateObjectReferences method sets a given VRTK_SDKSetup as the loaded SDK Setup to be able to use it when populating object references in the SDK Setup.
        /// </summary>
        /// <remarks>
        /// This method should only be called when not playing as it's only for populating the object references.
        /// This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used
        /// in a type that is also compiled for a standalone build.
        /// </remarks>
        /// <param name="setup">The SDK Setup to set as the loaded SDK.</param>
        public void SetLoadedSDKSetupToPopulateObjectReferences(VRTK_SDKSetup setup)
        {
            if (EditorApplication.isPlaying)
            {
                VRTK_Logger.Fatal("The method SetLoadedSDKSetupToPopulateObjectReferences should not be used when the application is playing.");
                return;
            }

            loadedSetup = setup;
        }
#endif

        /// <summary>
        /// The UnloadSDKSetup method unloads the currently loaded VRTK_SDKSetup, if there is one.
        /// </summary>
        /// <param name="disableVR">Whether to disable VR altogether after unloading the SDK Setup.</param>
        public void UnloadSDKSetup(bool disableVR = false)
        {
            if (loadedSetup != null)
            {
                ToggleBehaviours(false);
            }

            VRTK_SDKSetup previousLoadedSetup = loadedSetup;
            loadedSetup = null;

            if (previousLoadedSetup != null)
            {
                previousLoadedSetup.OnUnloaded(this);
            }

            if (disableVR)
            {
                XRSettings.LoadDeviceByName("None");
                XRSettings.enabled = false;
            }

            if (previousLoadedSetup != null)
            {
                OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, null));
            }

            _previouslyUsedSetupInfos.Clear();
            if (previousLoadedSetup != null)
            {
                _previouslyUsedSetupInfos.UnionWith(
                    new[]
                    {
                        previousLoadedSetup.systemSDKInfo,
                        previousLoadedSetup.boundariesSDKInfo,
                        previousLoadedSetup.headsetSDKInfo,
                        previousLoadedSetup.controllerSDKInfo
                    }
                );
            }
        }

        static VRTK_SDKManager()
        {
            PopulateAvailableScriptingDefineSymbolPredicateInfos();
            PopulateAvailableAndInstalledSDKInfos();

#if UNITY_EDITOR
            //call AutoManageScriptingDefineSymbolsAndManageVRSettings when the currently active scene changes
            EditorApplication.hierarchyWindowChanged += AutoManageScriptingDefineSymbolsAndManageVRSettings;
#endif
        }

        private void OnEnable()
        {
            behavioursToToggleOnLoadedSetupChange = _behavioursToToggleOnLoadedSetupChange.AsReadOnly();

            CreateInstance();

            if (loadedSetup == null && autoLoadSetup)
            {
                TryLoadSDKSetupFromList();
            }
        }

        private void OnDisable()
        {
            if (checkLeftControllerReadyRoutine != null)
            {
                StopCoroutine(checkLeftControllerReadyRoutine);
            }

            if (checkRightControllerReadyRoutine != null)
            {
                StopCoroutine(checkRightControllerReadyRoutine);
            }

#pragma warning disable 618
            if (_instance == this && !persistOnLoad)
#pragma warning restore 618
            {
                UnloadSDKSetup();
            }
        }

        private void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = this;
                VRTK_SDK_Bridge.InvalidateCaches();

#pragma warning disable 618
                if (persistOnLoad && Application.isPlaying)
#pragma warning restore 618
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnLoadedSetupChanged(LoadedSetupChangeEventArgs e)
        {
            LoadedSetupChangeEventHandler handler = LoadedSetupChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private IEnumerator FinishSDKSetupLoading(VRTK_SDKSetup[] sdkSetups, VRTK_SDKSetup previousLoadedSetup)
        {
            yield return null;

            string loadedDeviceName = string.IsNullOrEmpty(XRSettings.loadedDeviceName) ? "None" : XRSettings.loadedDeviceName;
            loadedSetup = sdkSetups.FirstOrDefault(setup => setup.usedVRDeviceNames.Contains(loadedDeviceName));

            if (loadedSetup == null)
            {
                // The loaded VR Device doesn't match any SDK Setup
                UnloadSDKSetup();

                const string errorMessage = "No SDK Setup from the provided list could be loaded.";
                OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, errorMessage));
                VRTK_Logger.Error(errorMessage);

                yield break;
            }

            if (loadedSetup.usedVRDeviceNames.Except(new[] { "None" }).Any())
            {
                // The loaded VR Device is actually a VR Device
                XRSettings.enabled = true;

                if (!XRDevice.isPresent)
                {
                    // Despite being loaded, the loaded VR Device isn't working correctly
                    int nextSetupIndex = Array.IndexOf(sdkSetups, loadedSetup) + 1;
                    string errorMessage = "An SDK Setup from the provided list could be loaded, but the device is not in working order.";

                    ToggleBehaviours(false);
                    loadedSetup = null;

                    if (nextSetupIndex < sdkSetups.Length && sdkSetups.Length - nextSetupIndex > 0)
                    {
                        // Let's try loading the remaining SDK Setups
                        errorMessage += " Now retrying with the remaining SDK Setups from the provided list...";
                        VRTK_Logger.Warn(errorMessage);
                        OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, errorMessage));

                        TryLoadSDKSetup(nextSetupIndex, false, sdkSetups);
                        yield break;
                    }

                    // There are no other SDK Setups
                    UnloadSDKSetup();

                    errorMessage += " There are no other Setups in the provided list to try.";
                    OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, errorMessage));
                    VRTK_Logger.Error(errorMessage);

                    yield break;
                }
            }

            // A VR Device was correctly loaded, is working and matches an SDK Setup
            loadedSetup.OnLoaded(this);
            ToggleBehaviours(true);
            CheckControllersReady();
            OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, loadedSetup, null));
        }

        private void CheckControllersReady()
        {
            if (checkLeftControllerReadyRoutine != null)
            {
                StopCoroutine(checkLeftControllerReadyRoutine);
            }
            checkLeftControllerReadyRoutine = StartCoroutine(CheckLeftControllerReady());

            if (checkRightControllerReadyRoutine != null)
            {
                StopCoroutine(checkRightControllerReadyRoutine);
            }
            checkRightControllerReadyRoutine = StartCoroutine(CheckRightControllerReady());
        }

        private IEnumerator CheckLeftControllerReady()
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(checkControllerReadyDelay);
            int maxCheckTime = checkControllerValidTimer;
            while (true)
            {
                if (loadedSetup != null && loadedSetup.actualLeftController != null && loadedSetup.actualLeftController.activeInHierarchy && (loadedSetup.controllerSDK.GetCurrentControllerType() != SDK_BaseController.ControllerType.Undefined || maxCheckTime < 0))
                {
                    break;
                }
                maxCheckTime--;
                yield return delayInstruction;
            }
            loadedSetup.controllerSDK.OnControllerReady(SDK_BaseController.ControllerHand.Left);
        }

        private IEnumerator CheckRightControllerReady()
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(checkControllerReadyDelay);
            int maxCheckTime = checkControllerValidTimer;
            while (true)
            {
                if (loadedSetup != null && loadedSetup.actualRightController != null && loadedSetup.actualRightController.activeInHierarchy && (loadedSetup.controllerSDK.GetCurrentControllerType() != SDK_BaseController.ControllerType.Undefined || maxCheckTime < 0))
                {
                    break;
                }
                maxCheckTime--;
                yield return delayInstruction;
            }
            loadedSetup.controllerSDK.OnControllerReady(SDK_BaseController.ControllerHand.Right);
        }

        private void ToggleBehaviours(bool state)
        {
            List<Behaviour> listCopy = _behavioursToToggleOnLoadedSetupChange.ToList();
            if (!state)
            {
                listCopy.Reverse();
            }

            for (int index = 0; index < listCopy.Count; index++)
            {
                Behaviour behaviour = listCopy[index];
                if (behaviour == null)
                {
                    VRTK_Logger.Error(string.Format("A behaviour to toggle has been destroyed. Have you forgot the corresponding call `VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this)` in the `OnDestroy` method of `{0}`?", behaviour.GetType()));
                    _behavioursToToggleOnLoadedSetupChange.RemoveAt(state ? index : _behavioursToToggleOnLoadedSetupChange.Count - 1 - index);

                    continue;
                }

                behaviour.enabled = (state && _behavioursInitialState.ContainsKey(behaviour) ? _behavioursInitialState[behaviour] : state);
            }
        }

        /// <summary>
        /// Populates `AvailableScriptingDefineSymbolPredicateInfos` with all the available SDK_ScriptingDefineSymbolPredicateAttributes and associated method infos.
        /// </summary>
        private static void PopulateAvailableScriptingDefineSymbolPredicateInfos()
        {
            List<ScriptingDefineSymbolPredicateInfo> predicateInfos = new List<ScriptingDefineSymbolPredicateInfo>();

            foreach (Type type in typeof(VRTK_SDKManager).Assembly.GetTypes())
            {
                for (int index = 0; index < type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Length; index++)
                {
                    MethodInfo methodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)[index];
                    SDK_ScriptingDefineSymbolPredicateAttribute[] predicateAttributes = (SDK_ScriptingDefineSymbolPredicateAttribute[])methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), false);
                    if (predicateAttributes.Length == 0)
                    {
                        continue;
                    }

                    if (methodInfo.ReturnType != typeof(bool) || methodInfo.GetParameters().Length != 0)
                    {
                        VRTK_Logger.Fatal(new InvalidOperationException(string.Format("The method '{0}' on '{1}' has '{2}' specified but its signature is wrong. The method must take no arguments and return bool.",
                                                                                      methodInfo.Name,
                                                                                      type,
                                                                                      typeof(SDK_ScriptingDefineSymbolPredicateAttribute))));
                        return;
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

            List<VRTK_SDKInfo> availableSystemSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> availableBoundariesSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> availableHeadsetSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> availableControllerSDKInfos = new List<VRTK_SDKInfo>();

            List<VRTK_SDKInfo> installedSystemSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> installedBoundariesSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> installedHeadsetSDKInfos = new List<VRTK_SDKInfo>();
            List<VRTK_SDKInfo> installedControllerSDKInfos = new List<VRTK_SDKInfo>();

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
        /// <typeparam name="BaseType">The SDK base type of which to populate the lists for. Must be a subclass of `SDK_Base`.</typeparam>
        /// <typeparam name="FallbackType">The SDK type to fall back on if problems occur. Must be a subclass of `BaseType`.</typeparam>
        /// <param name="availableSDKInfos">The list of available SDK infos to populate.</param>
        /// <param name="installedSDKInfos">The list of installed SDK infos to populate.</param>
        /// <param name="symbolsOfInstalledSDKs">The list of symbols of all the installed SDKs.</param>
        private static void PopulateAvailableAndInstalledSDKInfos<BaseType, FallbackType>(List<VRTK_SDKInfo> availableSDKInfos,
                                                                                          List<VRTK_SDKInfo> installedSDKInfos,
                                                                                          ICollection<string> symbolsOfInstalledSDKs)
            where BaseType : SDK_Base where FallbackType : BaseType
        {
            Type baseType = typeof(BaseType);
            Type fallbackType = SDKFallbackTypesByBaseType[baseType];

            availableSDKInfos.AddRange(VRTK_SDKInfo.Create<BaseType, FallbackType, FallbackType>());
            availableSDKInfos.AddRange(baseType.Assembly.GetExportedTypes()
                                               .Where(type => type.IsSubclassOf(baseType) && type != fallbackType && !type.IsAbstract)
                                               .SelectMany<Type, VRTK_SDKInfo>(VRTK_SDKInfo.Create<BaseType, FallbackType>));
            availableSDKInfos.Sort((x, y) => x.description.describesFallbackSDK
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
        /// Calls `ManageScriptingDefineSymbols` and `ManageVRSettings` (both without forcing) at the appropriate times when in the editor.
        /// </summary>
        [DidReloadScripts(1)]
        private static void AutoManageScriptingDefineSymbolsAndManageVRSettings()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            RemoveLegacyScriptingDefineSymbols();

            if (instance != null && !instance.ManageScriptingDefineSymbols(false, false))
            {
                instance.targetGroupsToExclude = instance.excludeTargetGroups;
                instance.ManageVRSettings(false);
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

            if (currentSymbols.SequenceEqual(newSymbols))
            {
                return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", newSymbols));

            string[] removedSymbols = currentSymbols.Except(newSymbols).ToArray();
            if (removedSymbols.Length > 0)
            {
                VRTK_Logger.Info(string.Format("Legacy (i.e. used by previous VRTK versions only) Scripting Define Symbols removed from [Project Settings->Player] for {0}: {1}",
                                               BuildTargetGroup.Standalone,
                                               string.Join(", ", removedSymbols)));
            }
        }
#endif
    }
}