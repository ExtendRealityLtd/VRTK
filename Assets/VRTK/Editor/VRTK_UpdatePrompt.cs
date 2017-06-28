using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRTK;

[InitializeOnLoad]
public sealed class VRTK_UpdatePrompt : EditorWindow
{
    [Serializable]
    private sealed class LatestRelease
    {
        [NonSerialized]
        public Version version;
        [NonSerialized]
        public DateTime publishedDateTime;

#pragma warning disable 649
        public string html_url;
        public string tag_name;
        public string name;
        public string published_at;
        public string zipball_url;
        public string body;
#pragma warning restore 649

        public static LatestRelease CreateFromJSON(string json)
        {
            LatestRelease latestRelease = JsonUtility.FromJson<LatestRelease>(json);
            latestRelease.version = new Version(latestRelease.tag_name);
            latestRelease.publishedDateTime = DateTime.Parse(latestRelease.published_at);

            latestRelease.body = latestRelease.body.Trim();
            latestRelease.body = Regex.Replace(latestRelease.body, @"(.*\*{2}.*\*{2}\n)", "\n$1");

            return latestRelease;
        }
    }

    private const string remoteURL = "https://api.github.com/repos/thestonefox/vrtk/releases/latest";
    private const string assetStoreURL = "/content/64131";
    private const string hidePromptKeyFormat = "VRTK.HideVersionPrompt.v{0}";
    private const string lastCheckKey = "VRTK.VersionPromptUpdate";
    private const int checkUpdateHours = 6;

    private static bool isManualCheck;
    private static bool versionChecked;
    private static WWW versionResource;
    private static LatestRelease latestRelease;
    private static VRTK_UpdatePrompt promptWindow;

    private Vector2 scrollPosition;
    private Vector2 changelogScrollPosition;
    private bool isChangelogFoldOut = true;

    static VRTK_UpdatePrompt()
    {
        EditorApplication.update += CheckForUpdate;
    }

    public void OnGUI()
    {
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollViewScope.scrollPosition;

            if (versionResource != null && !versionResource.isDone)
            {
                EditorGUILayout.HelpBox("Checking for updates...", MessageType.Info);
                return;
            }

            if (latestRelease == null)
            {
                EditorGUILayout.HelpBox("There was a problem checking for updates.", MessageType.Error);
                DrawCheckAgainButton();

                return;
            }

            string newVersionName = latestRelease.name == string.Format("Version {0}", latestRelease.version)
                                        ? string.Empty
                                        : string.Format(" \"{0}\"", latestRelease.name);
            bool isUpToDate = VRTK_Defines.CurrentVersion >= latestRelease.version;

            EditorGUILayout.HelpBox(
                string.Format(
                    "{0}.\n\nInstalled Version: {1}\nAvailable version: {2}{3} (published on {4})",
                    isUpToDate ? "Already up to date" : "A new version of VRTK is available",
                    VRTK_Defines.CurrentVersion,
                    latestRelease.version,
                    newVersionName,
                    latestRelease.publishedDateTime.ToLocalTime()),
                isUpToDate ? MessageType.Info : MessageType.Warning);

            DrawCheckAgainButton();

            isChangelogFoldOut = EditorGUILayout.Foldout(isChangelogFoldOut, "Changelog", true);
            if (isChangelogFoldOut)
            {
                using (GUILayout.ScrollViewScope changelogScrollViewScope = new GUILayout.ScrollViewScope(changelogScrollPosition))
                {
                    changelogScrollPosition = changelogScrollViewScope.scrollPosition;

                    if (GUILayout.Button("View on GitHub"))
                    {
                        Application.OpenURL(latestRelease.html_url);
                    }

                    EditorGUILayout.HelpBox(latestRelease.body, MessageType.None);
                }
            }

            if (isUpToDate)
            {
                return;
            }

            GUILayout.FlexibleSpace();

            VRTK_EditorUtilities.AddHeader("Get Latest Version");
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("From Asset Store"))
                {
                    AssetStore.Open(assetStoreURL);
                    Close();
                }
                if (GUILayout.Button("From GitHub"))
                {
                    Application.OpenURL(latestRelease.zipball_url);
                }
            }

            using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                string key = string.Format(hidePromptKeyFormat, latestRelease.version);
                bool hideToggle = EditorPrefs.HasKey(key);

                hideToggle = GUILayout.Toggle(hideToggle, "Do not prompt for this version again.");

                if (changeCheckScope.changed)
                {
                    if (hideToggle)
                    {
                        EditorPrefs.SetBool(key, true);
                    }
                    else
                    {
                        EditorPrefs.DeleteKey(key);
                    }
                }
            }
        }
    }

    private static void DrawCheckAgainButton()
    {
        if (GUILayout.Button("Check again"))
        {
            CheckManually();
        }
    }

    private static void CheckForUpdate()
    {
        if (isManualCheck)
        {
            ShowWindow();
        }
        else
        {
            if (versionChecked)
            {
                EditorApplication.update -= CheckForUpdate;
                return;
            }

            if (EditorPrefs.HasKey(lastCheckKey))
            {
                string lastCheckTicksString = EditorPrefs.GetString(lastCheckKey);
                DateTime lastCheckDateTime = new DateTime(Convert.ToInt64(lastCheckTicksString));

                if (lastCheckDateTime.AddHours(checkUpdateHours) >= DateTime.UtcNow)
                {
                    versionChecked = true;
                    return;
                }
            }
        }

        versionResource = versionResource == null ? new WWW(remoteURL) : versionResource;
        if (!versionResource.isDone)
        {
            return;
        }

        EditorApplication.update -= CheckForUpdate;

        if (string.IsNullOrEmpty(versionResource.error))
        {
            latestRelease = LatestRelease.CreateFromJSON(versionResource.text);
        }

        versionResource.Dispose();
        versionResource = null;
        versionChecked = true;
        EditorPrefs.SetString(lastCheckKey, DateTime.UtcNow.Ticks.ToString());

        // Clean up the existing hidePromptKeys (except the one for the current version)
        new[] { VRTK_Defines.CurrentVersion }
            .Concat(VRTK_Defines.PreviousVersions)
            .Where(version => latestRelease == null || version != latestRelease.version)
            .Select(version => string.Format(hidePromptKeyFormat, version))
            .Where(EditorPrefs.HasKey)
            .ToList()
            .ForEach(EditorPrefs.DeleteKey);

        if (!isManualCheck
            && latestRelease != null
            && (VRTK_Defines.CurrentVersion >= latestRelease.version
                || EditorPrefs.HasKey(string.Format(hidePromptKeyFormat, latestRelease.version))))
        {
            return;
        }

        ShowWindow();
        isManualCheck = false;
    }

    private static void ShowWindow()
    {
        if (promptWindow != null)
        {
            promptWindow.ShowUtility();
            promptWindow.Repaint();
            return;
        }

        promptWindow = GetWindow<VRTK_UpdatePrompt>(true);
        promptWindow.titleContent = new GUIContent("VRTK Update");
    }

    [MenuItem("Window/VRTK/Check for Updates")]
    private static void CheckManually()
    {
        isManualCheck = true;

        if (versionResource == null)
        {
            EditorApplication.update += CheckForUpdate;
        }
    }
}
