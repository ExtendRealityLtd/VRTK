// Console Viewer Canvas|Prefabs|0060
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;

    /// <summary>
    /// This canvas adds the unity console log to a world game object. To use the prefab, it simply needs to be placed into the scene and it will be visible in world space. It's also possible to child it to other objects such as the controller so it can track where the user is.
    /// </summary>
    /// <remarks>
    /// It's also recommended to use the Simple Pointer and UI Pointer on a controller to interact with the Console Viewer Canvas as it has a scrollable text area, a button to clear the log and a checkbox to toggle whether the log messages are collapsed.
    /// </remarks>
    public class VRTK_ConsoleViewer : MonoBehaviour
    {
        [Tooltip("The size of the font the log text is displayed in.")]
        public int fontSize = 14;
        [Tooltip("The colour of the text for an info log message.")]
        public Color infoMessage = Color.black;
        [Tooltip("The colour of the text for an assertion log message.")]
        public Color assertMessage = Color.black;
        [Tooltip("The colour of the text for a warning log message.")]
        public Color warningMessage = Color.yellow;
        [Tooltip("The colour of the text for an error log message.")]
        public Color errorMessage = Color.red;
        [Tooltip("The colour of the text for an exception log message.")]
        public Color exceptionMessage = Color.red;

        private Dictionary<LogType, Color> logTypeColors;
        private ScrollRect scrollWindow;
        private RectTransform consoleRect;
        private Text consoleOutput;
        private const string NEWLINE = "\n";
        private int lineBuffer = 50;
        private int currentBuffer;
        private string lastMessage;
        private bool collapseLog = false;

        /// <summary>
        /// The SetCollapse method determines whether the console will collapse same message output into the same line. A state of `true` will collapse messages and `false` will print the same message for each line.
        /// </summary>
        /// <param name="state">The state of whether to collapse the output messages, true will collapse and false will not collapse.</param>
        public void SetCollapse(bool state)
        {
            collapseLog = state;
        }

        /// <summary>
        /// The ClearLog method clears the current log view of all messages
        /// </summary>
        public void ClearLog()
        {
            consoleOutput.text = "";
            currentBuffer = 0;
            lastMessage = "";
        }

        private void Awake()
        {
            logTypeColors = new Dictionary<LogType, Color>()
        {
            { LogType.Assert, assertMessage },
            { LogType.Error, errorMessage },
            { LogType.Exception, exceptionMessage },
            { LogType.Log, infoMessage },
            { LogType.Warning, warningMessage }
        };
            scrollWindow = transform.FindChild("Panel/Scroll View").GetComponent<ScrollRect>();
            consoleRect = transform.FindChild("Panel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            consoleOutput = transform.FindChild("Panel/Scroll View/Viewport/Content/ConsoleOutput").GetComponent<Text>();

            consoleOutput.fontSize = fontSize;
            ClearLog();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            consoleRect.sizeDelta = Vector2.zero;
        }

        private string GetMessage(string message, LogType type)
        {
            var color = ColorUtility.ToHtmlStringRGBA(logTypeColors[type]);
            return "<color=#" + color + ">" + message + "</color>" + NEWLINE;
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            var logOutput = GetMessage(message, type);

            if (!collapseLog || lastMessage != logOutput)
            {
                consoleOutput.text += logOutput;
                lastMessage = logOutput;
            }

            consoleRect.sizeDelta = new Vector2(consoleOutput.preferredWidth, consoleOutput.preferredHeight);
            scrollWindow.verticalNormalizedPosition = 0;
            currentBuffer++;
            if (currentBuffer >= lineBuffer)
            {
                var lines = Regex.Split(consoleOutput.text, NEWLINE).Skip(lineBuffer / 2);
                consoleOutput.text = string.Join(NEWLINE, lines.ToArray());
                currentBuffer = lineBuffer / 2;
            }
        }
    }
}