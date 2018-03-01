namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

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

        protected virtual void Awake()
        {
            ToggleScriptAlias(false);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneConstructor.name, LoadSceneMode.Additive);
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
            if (loadedScene.name == sceneConstructor.name)
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