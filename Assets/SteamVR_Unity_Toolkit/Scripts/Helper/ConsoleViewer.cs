using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class ConsoleViewer : MonoBehaviour
{
    public int fontSize = 14;
    public Color infoMessage = Color.black;
    public Color assertMessage = Color.black;
    public Color warningMessage = Color.yellow;
    public Color errorMessage = Color.red;
    public Color exceptionMessage = Color.red;

    private struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    private Dictionary<LogType, Color> logTypeColors;
    private ScrollRect scrollWindow;
    private RectTransform consoleRect;
    private Text consoleOutput;
    private const string NEWLINE = "\n";
    private int lineBuffer = 50;
    private int currentBuffer;
    private string lastMessage;
    private bool collapseLog = false;

    public void SetCollapse(bool state)
    {
        collapseLog = state;
    }

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