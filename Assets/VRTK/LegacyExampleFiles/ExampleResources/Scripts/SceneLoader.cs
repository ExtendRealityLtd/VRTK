namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        public Object sceneConstructor;
        public bool sdkSwitcher = true;
        public GameObject leftScriptAlias;
        public GameObject rightScriptAlias;

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
            leftScriptAlias.SetActive(state);
            rightScriptAlias.SetActive(state);
        }
    }
}