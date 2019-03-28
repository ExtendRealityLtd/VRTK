namespace VRTK.Prefabs.Helpers.SceneConsole
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the SceneConsole Prefab.
    /// </summary>
    public class SceneConsoleFacade : MonoBehaviour
    {
        #region Console Settings
        /// <summary>
        /// The size of the font the log text is displayed in.
        /// </summary>
        [Serialized]
        [field: Header("Console Settings"), DocumentedByXml]
        public int FontSize { get; set; } = 10;
        /// <summary>
        /// The <see cref="Color"/> of the text for an info log message.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Color InfoMessage { get; set; } = Color.black;
        /// <summary>
        /// The <see cref="Color"/> of the text for an assertion log message.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Color AssertMessage { get; set; } = Color.black;
        /// <summary>
        /// The <see cref="Color"/> of the text for a warning log message.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Color WarningMessage { get; set; } = Color.yellow;
        /// <summary>
        /// The <see cref="Color"/> of the text for an error log message.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Color ErrorMessage { get; set; } = Color.red;
        /// <summary>
        /// The <see cref="Color"/> of the text for an exception log message.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Color ExceptionMessage { get; set; } = Color.red;
        /// <summary>
        /// Determines whether to collapse same messages into one message in the log.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool CollapseLog { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The console scrollable <see cref="ScrollRect"/> area.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public ScrollRect ScrollWindow { get; protected set; }
        /// <summary>
        /// The output content <see cref="RectTransform"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public RectTransform ConsoleRect { get; protected set; }
        /// <summary>
        /// The <see cref="Text "/> element to output the console to.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Text ConsoleOutput { get; protected set; }
        #endregion

        /// <summary>
        /// The character to use for the line ending.
        /// </summary>
        protected const string newline = "\n";
        /// <summary>
        /// The specified line buffer.
        /// </summary>
        protected const int lineBuffer = 50;
        /// <summary>
        /// The current line buffer in use.
        /// </summary>
        protected int currentBuffer;
        /// <summary>
        /// The last log message.
        /// </summary>
        protected string lastMessage;

        /// <summary>
        /// Clears the current log view of all messages
        /// </summary>
        public virtual void ClearLog()
        {
            if (ConsoleOutput != null)
            {
                ConsoleOutput.text = "";
            }
            currentBuffer = 0;
            lastMessage = "";
        }

        protected virtual void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        protected virtual void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            if (ConsoleRect != null)
            {
                ConsoleRect.sizeDelta = Vector2.zero;
            }
        }

        /// <summary>
        /// Gets a rich text styled message for the given <see cref="LogType"/>.
        /// </summary>
        /// <param name="message">The message to style.</param>
        /// <param name="type">The <see cref="LogType"/> of the message.</param>
        /// <returns>A rich text styled message.</returns>
        protected virtual string GetMessage(string message, LogType type)
        {
            Color color;
            switch (type)
            {
                case LogType.Error:
                    color = ErrorMessage;
                    break;
                case LogType.Assert:
                    color = AssertMessage;
                    break;
                case LogType.Warning:
                    color = WarningMessage;
                    break;
                case LogType.Log:
                    color = InfoMessage;
                    break;
                case LogType.Exception:
                    color = ExceptionMessage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            string hexadecimalColorString = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hexadecimalColorString}>{message}</color>{newline}";
        }

        /// <summary>
        /// Digests the current Unity log stream and outputs it to the provided <see cref="ConsoleOutput"/>
        /// </summary>
        /// <param name="message">The message from the log stream.</param>
        /// <param name="stackTrace">The stacktrace data from the log stream</param>
        /// <param name="type">The type of the given log message.</param>
        protected virtual void HandleLog(string message, string stackTrace, LogType type)
        {
            if (ConsoleOutput == null || ScrollWindow == null || ConsoleRect == null)
            {
                return;
            }

            string logOutput = GetMessage(message, type);
            string consoleOutputText = ConsoleOutput.text;

            if (currentBuffer >= lineBuffer)
            {
                const int halfLineBuffer = lineBuffer / 2;
                int lookupIndex = 0;
                int skippedLineCount = 0;

                while (skippedLineCount < halfLineBuffer && lookupIndex < consoleOutputText.Length)
                {
                    int newlineIndex = consoleOutputText.IndexOf(newline, lookupIndex, StringComparison.Ordinal);
                    if (newlineIndex == -1)
                    {
                        break;
                    }

                    lookupIndex = newlineIndex + 1;
                    skippedLineCount++;
                }

                if (lookupIndex < consoleOutputText.Length)
                {
                    consoleOutputText = consoleOutputText.Substring(lookupIndex);
                }

                currentBuffer = halfLineBuffer;
            }

            if (!CollapseLog || lastMessage != logOutput)
            {
                consoleOutputText += logOutput;
                lastMessage = logOutput;
            }

            ConsoleOutput.text = consoleOutputText;
            ConsoleOutput.fontSize = FontSize;
            ConsoleRect.sizeDelta = new Vector2(ConsoleOutput.preferredWidth, ConsoleOutput.preferredHeight);
            ScrollWindow.verticalNormalizedPosition = 0;
            currentBuffer++;
        }
    }
}