// SDK Setup Switcher|Utilities|90011
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The SDK Setup Switcher adds a GUI overlay to allow switching the loaded VRTK_SDKSetup of the the current VRTK_SDKManager.
    /// </summary>
    /// <remarks>
    /// Use the `SDKSetupSwitcher` prefab to use this.
    /// </remarks>
    public class VRTK_SDKSetupSwitcher : MonoBehaviour
    {
        [Header("Fallback Objects")]

        [SerializeField]
        private Camera fallbackCamera;
        [SerializeField]
        private EventSystem eventSystem;

        [Header("Object References")]

        [SerializeField]
        private Text currentText;
        [SerializeField]
        private RectTransform statusPanel;
        [SerializeField]
        private RectTransform selectionPanel;

        [SerializeField]
        private Button switchButton;
        [SerializeField]
        private Button cancelButton;
        [SerializeField]
        private Button chooseButton;

        protected enum ViewingState
        {
            Status,
            Selection
        }

        private VRTK_SDKManager sdkManager;
        private readonly List<GameObject> chooseButtonGameObjects = new List<GameObject>();

        protected virtual void Awake()
        {
            fallbackCamera.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(false);
            chooseButton.gameObject.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            sdkManager.LoadedSetupChanged += OnLoadedSetupChanged;

            switchButton.onClick.AddListener(OnSwitchButtonClick);
            cancelButton.onClick.AddListener(OnCancelButtonClick);

            Show(ViewingState.Status);
        }

        protected virtual void OnDisable()
        {
            sdkManager.LoadedSetupChanged -= OnLoadedSetupChanged;

            switchButton.onClick.RemoveListener(OnSwitchButtonClick);
            cancelButton.onClick.RemoveListener(OnCancelButtonClick);

            Show(ViewingState.Status);
        }

        protected virtual void OnLoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            Show(ViewingState.Status);
        }

        protected virtual void OnSwitchButtonClick()
        {
            Show(ViewingState.Selection);
        }

        protected virtual void OnCancelButtonClick()
        {
            Show(ViewingState.Status);
        }

        protected virtual void Show(ViewingState viewingState)
        {
            switch (viewingState)
            {
                case ViewingState.Status:
                    RemoveCreatedChooseButtons();
                    UpdateCurrentText();
                    selectionPanel.gameObject.SetActive(false);
                    statusPanel.gameObject.SetActive(true);

                    break;
                case ViewingState.Selection:
                    AddSelectionButtons();
                    selectionPanel.gameObject.SetActive(true);
                    statusPanel.gameObject.SetActive(false);

                    break;
                default:
                    VRTK_Logger.Fatal(new ArgumentOutOfRangeException("viewingState", viewingState, null));
                    return;
            }

            bool isAnyOtherCameraUsed = sdkManager.setups.Any(setup => setup != null && setup.gameObject.activeSelf)
                                        || VRTK_DeviceFinder.HeadsetCamera() != null;
            fallbackCamera.gameObject.SetActive(!isAnyOtherCameraUsed);
            eventSystem.gameObject.SetActive(EventSystem.current == null || EventSystem.current == eventSystem);
        }

        protected virtual void UpdateCurrentText()
        {
            VRTK_SDKSetup loadedSetup = sdkManager.loadedSetup;
            currentText.text = loadedSetup == null ? "None" : loadedSetup.name;
        }

        protected virtual void AddSelectionButtons()
        {
            VRTK_SDKSetup loadedSetup = sdkManager.loadedSetup;
            if (loadedSetup != null)
            {
                GameObject chooseNoneButton = (GameObject)Instantiate(chooseButton.gameObject, chooseButton.transform.parent);
                chooseNoneButton.GetComponentInChildren<Text>().text = "None";
                chooseNoneButton.name = "ChooseNoneButton";
                chooseNoneButton.SetActive(true);

                chooseNoneButton.GetComponent<Button>().onClick.AddListener(
                    () => sdkManager.UnloadSDKSetup(true)
                );

                chooseButtonGameObjects.Add(chooseNoneButton);
            }

            VRTK_SDKSetup[] setups = sdkManager.setups;
            for (int index = 0; index < setups.Length; index++)
            {
                VRTK_SDKSetup setup = setups[index];
                if (setup == null || setup == loadedSetup)
                {
                    continue;
                }

                GameObject chooseButtonCopy = (GameObject)Instantiate(chooseButton.gameObject, chooseButton.transform.parent);
                chooseButtonCopy.GetComponentInChildren<Text>().text = setup.name;
                chooseButtonCopy.name = string.Format("Choose{0}Button", setup.name);
                chooseButtonCopy.SetActive(true);

                int indexCopy = index;
                Button button = chooseButtonCopy.GetComponent<Button>();
                button.onClick.AddListener(
                    () => sdkManager.TryLoadSDKSetup(indexCopy, true, setups)
                );

                ColorBlock buttonColors = button.colors;
                buttonColors.colorMultiplier = setup.isValid ? 1.0f : 0.8f;
                button.colors = buttonColors;

                chooseButtonGameObjects.Add(chooseButtonCopy);
            }
        }

        protected virtual void RemoveCreatedChooseButtons()
        {
            chooseButtonGameObjects.ForEach(Destroy);
            chooseButtonGameObjects.Clear();
        }
    }
}