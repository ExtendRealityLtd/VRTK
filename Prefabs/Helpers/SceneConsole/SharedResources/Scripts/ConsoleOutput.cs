namespace VRTK.Prefabs.Helpers.SceneConsole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Relays console information to a decorated world space UI canvas.
    /// </summary>
    public class ConsoleOutput : MonoBehaviour
    {
        /// <summary>
        /// The size of the font the log text is displayed in.
        /// </summary>
        [Tooltip("The size of the font the log text is displayed in.")]
        public int fontSize = 10;
        /// <summary>
        /// The <see cref="Color"/> of the text for an info log message.
        /// </summary>
        [Tooltip("The color of the text for an info log message.")]
        public Color infoMessage = Color.black;
        /// <summary>
        /// The <see cref="Color"/> of the text for an assertion log message.
        /// </summary>
        [Tooltip("The color of the text for an assertion log message.")]
        public Color assertMessage = Color.black;
        /// <summary>
        /// The <see cref="Color"/> of the text for a warning log message.
        /// </summary>
        [Tooltip("The color of the text for a warning log message.")]
        public Color warningMessage = Color.yellow;
        /// <summary>
        /// The <see cref="Color"/> of the text for an error log message.
        /// </summary>
        [Tooltip("The color of the text for an error log message.")]
        public Color errorMessage = Color.red;
        /// <summary>
        /// The <see cref="Color"/> of the text for an exception log message.
        /// </summary>
        [Tooltip("The color of the text for an exception log message.")]
        public Color exceptionMessage = Color.red;

        /// <summary>
        /// The console scrollable <see cref="ScrollRect"/> area.
        /// </summary>
        [Tooltip("The console scrollable ScrollRect area."), SerializeField]
        protected ScrollRect scrollWindow;
        /// <summary>
        /// The output content <see cref="RectTransform"/>.
        /// </summary>
        [Tooltip("The output content RectTransform."), SerializeField]
        protected RectTransform consoleRect;
        /// <summary>
        /// The <see cref="Text "/> element to output the console to.
        /// </summary>
        [Tooltip("The Text element to output the console to."), SerializeField]
        protected Text consoleOutput;

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
        /// Determines whether to collapse same messages into one message in the log.
        /// </summary>
        protected bool collapseLog;

        /// <summary>
        /// Determines whether the console will collapse same message output into the same line. A state of <see langword="true"/> will collapse messages and <see langword="false"/> will print the same message for each line.
        /// </summary>
        /// <param name="state">The state of whether to collapse the output messages, <see langword="true"/> will collapse and <see langword="false"/> will not collapse.</param>
        public virtual void SetCollapse(bool state)
        {
            collapseLog = state;
        }

        /// <summary>
        /// Clears the current log view of all messages
        /// </summary>
        public virtual void ClearLog()
        {
            if (consoleOutput != null)
            {
                consoleOutput.text = "";
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
            if (consoleRect != null)
            {
                consoleRect.sizeDelta = Vector2.zero;
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
                    color = errorMessage;
                    break;
                case LogType.Assert:
                    color = assertMessage;
                    break;
                case LogType.Warning:
                    color = warningMessage;
                    break;
                case LogType.Log:
                    color = infoMessage;
                    break;
                case LogType.Exception:
                    color = exceptionMessage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            string hexadecimalColorString = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hexadecimalColorString}>{message}</color>{newline}";
        }

        /// <summary>
        /// Digests the current Unity log stream and outputs it to the provided <see cref="consoleOutput"/>
        /// </summary>
        /// <param name="message">The message from the log stream.</param>
        /// <param name="stackTrace">The stacktrace data from the log stream</param>
        /// <param name="type">The type of the given log message.</param>
        protected virtual void HandleLog(string message, string stackTrace, LogType type)
        {
            if (consoleOutput == null || scrollWindow == null || consoleRect == null)
            {
                return;
            }

            string logOutput = GetMessage(message, type);
            string consoleOutputText = consoleOutput.text;

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

            if (!collapseLog || lastMessage != logOutput)
            {
                consoleOutputText += logOutput;
                lastMessage = logOutput;
            }

            consoleOutput.text = consoleOutputText;
            consoleOutput.fontSize = fontSize;
            consoleRect.sizeDelta = new Vector2(consoleOutput.preferredWidth, consoleOutput.preferredHeight);
            scrollWindow.verticalNormalizedPosition = 0;
            currentBuffer++;
        }
    }
}