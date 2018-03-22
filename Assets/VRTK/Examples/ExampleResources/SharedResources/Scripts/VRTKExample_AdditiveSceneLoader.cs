namespace VRTK.Examples.Utilities
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [ExecuteInEditMode]
    public class VRTKExample_AdditiveSceneLoader : MonoBehaviour
    {
        [Tooltip("The constructor scene containing the VRTK SDK Manager setup to load into the scene.")]
        public Object sceneConstructor;
        [Tooltip("The GameObject to inject into the VRTK SDK Manager as the Left Controller Script Alias.")]
        public GameObject leftScriptAlias;
        [Tooltip("The GameObject to inject into the VRTK SDK Manager as the Right Controller Script Alias.")]
        public GameObject rightScriptAlias;
        [Tooltip("A Transform to set the location and orentiation of SDK Manager to when it is spawned into the scene.")]
        public Transform spawnPoint;
        [Tooltip("If this is checked then the SDK Switcher will be displayed.")]
        public bool sdkSwitcher = true;

        protected VRTK_SDKSetupSwitcher setupSwitcher;
        protected int constructorSceneIndex;
        protected string constructorPath = "Assets/VRTK/Examples/VRTK_SDKManager_Constructor.unity";

        protected virtual void Awake()
        {
            if (!Application.isPlaying && Application.isEditor)
            {
                ManageBuildSettings();
            }
            else
            {
                constructorSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
                ToggleScriptAlias(false);
                SceneManager.sceneLoaded += OnSceneLoaded;
                if (sceneConstructor != null)
                {
                    SceneManager.LoadScene(sceneConstructor.name, LoadSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadScene(constructorSceneIndex, LoadSceneMode.Additive);
                }
            }
        }

        protected virtual void ManageBuildSettings()
        {
#if UNITY_EDITOR
            EditorBuildSettingsScene[] currentBuildScenes = EditorBuildSettings.scenes;
            if (currentBuildScenes.Length == 0 || currentBuildScenes[currentBuildScenes.Length - 1].path != constructorPath)
            {
                EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[currentBuildScenes.Length + 1];
                System.Array.Copy(currentBuildScenes, newBuildScenes, currentBuildScenes.Length);
                EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(constructorPath, true);
                newBuildScenes[newBuildScenes.Length - 1] = sceneToAdd;
                EditorBuildSettings.scenes = newBuildScenes;
            }
#endif
        }

        protected virtual void LateUpdate()
        {
            if (setupSwitcher != null)
            {
                setupSwitcher.gameObject.SetActive(sdkSwitcher);
            }
        }

        protected virtual void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
        {
            if (IsConstructorScene(loadedScene))
            {
                VRTK_SDKManager sdkManager = FindObjectOfType<VRTK_SDKManager>();
                sdkManager.gameObject.SetActive(false);
                if (spawnPoint != null)
                {
                    sdkManager.transform.position = spawnPoint.transform.position;
                    sdkManager.transform.rotation = spawnPoint.transform.rotation;
                    sdkManager.transform.localScale = spawnPoint.transform.localScale;
                }
                sdkManager.scriptAliasLeftController = leftScriptAlias;
                sdkManager.scriptAliasRightController = rightScriptAlias;
                sdkManager.gameObject.SetActive(true);
                ToggleScriptAlias(true);
                VRTK_SDKManager.ProcessDelayedToggleBehaviours();
                setupSwitcher = sdkManager.GetComponentInChildren<VRTK_SDKSetupSwitcher>();
            }
        }

        protected virtual bool IsConstructorScene(Scene checkScene)
        {
            return ((sceneConstructor != null && checkScene.name == sceneConstructor.name) || (sceneConstructor == null && checkScene.buildIndex == constructorSceneIndex));
        }

        protected virtual void ToggleScriptAlias(bool state)
        {
            if (leftScriptAlias != null)
            {
                leftScriptAlias.SetActive(state);
            }

            if (rightScriptAlias != null)
            {
                rightScriptAlias.SetActive(state);
            }
        }
    }
}