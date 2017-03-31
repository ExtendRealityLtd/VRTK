using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class VRTK_UpdatePrompt : EditorWindow
{
    private const string remoteURL = "https://raw.githubusercontent.com/thestonefox/VRTK/";
    private const string remoteVersionFile = "master/Assets/VRTK/Version.txt";
    private const string remoteChangelogFile = "master/CHANGELOG.md";
    private const string localVersionFile = "/Version.txt";
    private const string pluginURL = "https://www.assetstore.unity3d.com/en/#!/content/64131";
    private const string hidePromptKey = "VRTK.HideVersionPrompt.v{0}";
    private const string lastCheckKey = "VRTK.VersionPromptUpdate";
    private const int checkUpdateHours = 6;

    private static bool versionChecked = false;
    private static WWW versionResource;
    private static WWW changelogResource;
    private static string versionReceived;
    private static string changelogReceived;
    private static string versionLocal;
    private static VRTK_UpdatePrompt promptWindow;

    private Vector2 scrollPosition;
    private bool hideToggle;

    static VRTK_UpdatePrompt()
    {
        EditorApplication.update += Update;
    }

    public void OnGUI()
    {
        EditorGUILayout.HelpBox("A new version of VRTK is available.", MessageType.Warning);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Current version: " + versionLocal);
        GUILayout.Label("New version: " + versionReceived);

        if (changelogReceived != null && changelogReceived.Trim() != "")
        {
            GUILayout.Label("Changelog :");
            EditorGUILayout.HelpBox(changelogReceived.Trim().Replace("\n## ", "\n\n## ").Replace("\n * **", "\n\n * **"), MessageType.None);
        }

        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Get Latest Version"))
        {
            Application.OpenURL(pluginURL);
        }

        EditorGUI.BeginChangeCheck();
        bool hidePromptInFuture = GUILayout.Toggle(hideToggle, "Do not prompt for this version again.");
        if (EditorGUI.EndChangeCheck())
        {
            hideToggle = hidePromptInFuture;
            string key = string.Format(hidePromptKey, versionReceived);
            if (hidePromptInFuture)
            {
                EditorPrefs.SetBool(key, true);
            }
            else
            {
                EditorPrefs.DeleteKey(key);
            }
        }
    }

    private static void Update()
    {
        if (!versionChecked)
        {
            if (EditorPrefs.HasKey(lastCheckKey))
            {
                string lastCheckTicksString = EditorPrefs.GetString(lastCheckKey);
                var lastCheckDateTime = new DateTime(Convert.ToInt64(lastCheckTicksString));

                if (lastCheckDateTime.AddHours(checkUpdateHours) >= DateTime.UtcNow)
                {
                    versionChecked = true;
                    return;
                }
            }

            versionResource = (versionResource ?? new WWW(remoteURL + remoteVersionFile));
            if (!versionResource.isDone)
            {
                return;
            }

            versionReceived = (ValidURL(versionResource) ? versionResource.text : "");
            versionResource = null;
            versionChecked = true;
            EditorPrefs.SetString(lastCheckKey, DateTime.UtcNow.Ticks.ToString());

            if (UpdateRequired())
            {
                changelogResource = new WWW(remoteURL + remoteChangelogFile);
                promptWindow = GetWindow<VRTK_UpdatePrompt>(true);
                promptWindow.minSize = new Vector2(640, 480);
                promptWindow.titleContent = new GUIContent("VRTK Update");
            }
        }

        if (changelogResource != null)
        {
            if (!changelogResource.isDone)
            {
                return;
            }

            changelogReceived = (ValidURL(changelogResource) ? ParseChangelog(changelogResource.text) : "");

            changelogResource = null;

            if (changelogReceived != "")
            {
                promptWindow.Repaint();
            }
        }
        EditorApplication.update -= Update;
    }

    private static bool ValidURL(WWW resource)
    {
        return (string.IsNullOrEmpty(resource.error) && !Regex.IsMatch(resource.text, "404 not found", RegexOptions.IgnoreCase));
    }

    private static bool UpdateRequired()
    {
        string assetGuid = AssetDatabase.FindAssets(typeof(VRTK_UpdatePrompt).FullName).First();
        string path = AssetDatabase.GUIDToAssetPath(assetGuid);
        path = Path.GetDirectoryName(Path.GetDirectoryName(path));

        versionLocal = File.ReadAllText(path + localVersionFile);
        if (string.IsNullOrEmpty(versionLocal) || string.IsNullOrEmpty(versionReceived) || versionReceived == versionLocal || EditorPrefs.HasKey(string.Format(hidePromptKey, versionReceived)))
        {
            return false;
        }

        return (ParseVersion(versionReceived) > ParseVersion(versionLocal));
    }

    private static int ParseVersion(string versionText)
    {
        string[] versionBits = versionText.Trim().Split('.');
        string formattedVersion = "";
        foreach (string versionBit in versionBits)
        {
            formattedVersion += versionBit.PadLeft(3, '0');
        }
        return int.Parse(formattedVersion);
    }

    protected static string ParseChangelog(string fullChangelog)
    {
        string[] changelogBits = fullChangelog.Trim().Split(new string[] { "\n# " }, StringSplitOptions.RemoveEmptyEntries);
        if (changelogBits.Length < 1)
        {
            return "";
        }

        return changelogBits[1];
    }
}
