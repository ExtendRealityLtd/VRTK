namespace VRTK
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class VRTK_Logger : MonoBehaviour
    {
        public enum LogLevels
        {
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
            None
        }

        public enum CommonMessageKeys
        {
            NOT_DEFINED,
            REQUIRED_COMPONENT_MISSING_FROM_SCENE,
            REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT,
            REQUIRED_COMPONENT_MISSING_FROM_PARAMETER,
            REQUIRED_COMPONENT_MISSING_NOT_INJECTED,
            COULD_NOT_FIND_OBJECT_FOR_ACTION,
            SDK_OBJECT_NOT_FOUND,
            SDK_NOT_FOUND,
            SDK_MANAGER_ERRORS,
            SCRIPTING_DEFINE_SYMBOLS_ADDED,
            SCRIPTING_DEFINE_SYMBOLS_REMOVED
        }

        public static VRTK_Logger instance = null;

        public static Dictionary<CommonMessageKeys, string> commonMessages = new Dictionary<CommonMessageKeys, string>()
        {
            { CommonMessageKeys.NOT_DEFINED, "`{0}` not defined{1}." },
            { CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "`{0}` requires the `{1}` component to be available in the scene{2}." },
            { CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "`{0}` requires the `{1}` component to be attached to {2} GameObject{3}." },
            { CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_PARAMETER, "`{0}` requires a `{1}` component to be specified as the `{2}` parameter{3}." },
            { CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "`{0}` requires the `{1}` component. Either the `{2}` parameter is not set or no `{1}` component is attached to {3} GameObject{4}." },
            { CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION, "The `{0}` could not automatically find {1} to {2}." },
            { CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "No {0} could be found. Have you selected a valid {1} in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a {1} from the dropdown." },
            { CommonMessageKeys.SDK_NOT_FOUND, "The SDK '{0}' doesn't exist anymore. The fallback SDK '{1}' will be used instead." },
            { CommonMessageKeys.SDK_MANAGER_ERRORS, "The current SDK Manager setup is causing the following errors:\n\n{0}" },
            { CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_ADDED, "Scripting Define Symbols added to [Project Settings->Player] for {0}: {1}" },
            { CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_REMOVED, "Scripting Define Symbols removed from [Project Settings->Player] for {0}: {1}" }
        };

        public static Dictionary<CommonMessageKeys, int> commonMessageParts = new Dictionary<CommonMessageKeys, int>();

        public LogLevels minLevel = LogLevels.Trace;
        public bool throwExceptions = true;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void CreateIfNotExists()
        {
            if (instance == null)
            {
                GameObject loggerObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "Logger"))
                {
                    hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy
                };
                instance = loggerObject.AddComponent<VRTK_Logger>();
            }

            if (commonMessageParts.Count != commonMessages.Count)
            {
                commonMessageParts.Clear();
                foreach (KeyValuePair<CommonMessageKeys, string> commonMessage in commonMessages)
                {
                    int bitCount = Regex.Matches(commonMessage.Value, @"(?<!\{)\{([0-9]+).*?\}(?!})")
                                        .Cast<Match>().DefaultIfEmpty()
                                        .Max(m => m == null ? -1 : int.Parse(m.Groups[1].Value)) + 1;
                    commonMessageParts.Add(commonMessage.Key, bitCount);
                }
            }
        }

        public static string GetCommonMessage(CommonMessageKeys messageKey, params object[] parameters)
        {
            CreateIfNotExists();

            string returnMessage = "";
            string outputMessage = VRTK_SharedMethods.GetDictionaryValue(commonMessages, messageKey);
            if (outputMessage != null)
            {
                int outputMessageParts = VRTK_SharedMethods.GetDictionaryValue(commonMessageParts, messageKey);
                if (parameters.Length != outputMessageParts)
                {
                    Array.Resize(ref parameters, outputMessageParts);
                }
                returnMessage = string.Format(outputMessage, parameters);
            }
            return returnMessage;
        }

        public static void Trace(string message)
        {
            Log(LogLevels.Trace, message);
        }

        public static void Debug(string message)
        {
            Log(LogLevels.Debug, message);
        }

        public static void Info(string message)
        {
            Log(LogLevels.Info, message);
        }

        public static void Warn(string message)
        {
            Log(LogLevels.Warn, message);
        }

        public static void Error(string message)
        {
            Log(LogLevels.Error, message);
        }

        public static void Fatal(string message)
        {
            Log(LogLevels.Fatal, message);
        }

        public static void Fatal(Exception exception)
        {
            Log(LogLevels.Fatal, exception.Message);
        }

        public static void Log(LogLevels level, string message)
        {
#if VRTK_NO_LOGGING
            return;
#endif
            CreateIfNotExists();

            if (instance.minLevel > level)
            {
                return;
            }

            switch (level)
            {
                case LogLevels.Trace:
                case LogLevels.Debug:
                case LogLevels.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case LogLevels.Warn:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogLevels.Error:
                case LogLevels.Fatal:
                    if (instance.throwExceptions)
                    {
                        throw new Exception(message);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(message);
                    }
                    break;
            }
        }

        protected virtual void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}