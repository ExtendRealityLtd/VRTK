// Base Key Layout Renderer|Keyboard|81020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using IKey = VRTK_Keyboard.IKey;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using RKeyLayout = VRTK_RenderableKeyLayout;

    /// <summary>
    /// This abstract class is the base class for key layout renderers used to render functional keyboard to a GameObject
    /// </summary>
    /// <remarks>
    /// A key layout calculator or source component may be required on the same gameobject as the key layout renderer.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    [RequireComponent(typeof(VRTK_Keyboard))]
    public abstract class VRTK_BaseKeyLayoutRenderer : MonoBehaviour
    {
        public const string RUNTIME_OBJECT_CONTAINER_NAME = "__RuntimeObjects__";
        protected VRTK_Keyboard keyboard;
        protected int currentKeyset = 0;
        protected int keysetCount = 0;
        protected Dictionary<GameObject, int> keysetObjects;
        protected bool isEnterEnabled = true;

        protected virtual void OnEnable()
        {
            keyboard = GetComponent<VRTK_Keyboard>();

            SetupKeyboardUI();
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Update()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                SetupKeyboardUI();
            }
        }

        /// <summary>
        /// Apply hide flags to a game object in the editor so it is not saved
        /// </summary>
        /// <param name="obj">The runtime game object</param>
        protected void ProcessRuntimeObject(GameObject obj, bool hide = false)
        {
            if (Application.isEditor)
            {
                obj.hideFlags = HideFlags.DontSave;// | HideFlags.NotEditable;
                if (hide)
                {
                    obj.hideFlags |= HideFlags.HideInHierarchy;
                }
            }
        }

        /// <summary>
        /// Destroy an object which may be created in the editor using hide flag
        /// </summary>
        /// <param name="obj">The runtime game object</param>
        protected void DestroyRuntimeObject(GameObject obj)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        /// <summary>
        /// Return a container to use when inserting runtime objects into a GameObject
        /// </summary>
        /// <remarks>
        /// The runtime object container will use the same transform type as its parent.
        /// 
        /// The runtime object container will implicitly be passed to `ProcessRuntimeObject`.
        /// </remarks>
        /// <param name="parent">The parent gameobject to return a runtime container for</param>
        /// <returns>The runtime object container</returns>
        protected GameObject GetRuntimeObjectContainer(GameObject parent, bool empty = false, bool create = true)
        {
            GameObject root = null;

            // Check for a previously created container
            Transform preexistingRootTransform = parent.transform.Find(RUNTIME_OBJECT_CONTAINER_NAME);
            if (preexistingRootTransform != null)
            {
                root = preexistingRootTransform.gameObject;

                // Purge the container's children if requested
                if (empty)
                {
                    for (int i = root.transform.childCount - 1; i >= 0; i--)
                    {
                        DestroyRuntimeObject(root.transform.GetChild(i).gameObject);
                    }
                }
            }

            // Create a new container if it does not exist
            if (root == null & create)
            {
                if (parent.transform.GetType() == typeof(Transform))
                {
                    root = new GameObject(RUNTIME_OBJECT_CONTAINER_NAME);
                }
                else
                {
                    root = new GameObject(RUNTIME_OBJECT_CONTAINER_NAME, parent.transform.GetType());
                }

                root.transform.SetParent(parent.transform, false);

                // Make sure RectTransforms fill their parent
                RectTransform rootRectTransform = root.transform as RectTransform;
                if (rootRectTransform != null)
                {
                    rootRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rootRectTransform.anchorMin = new Vector2(0, 0);
                    rootRectTransform.anchorMax = new Vector2(1, 1);
                    rootRectTransform.offsetMin = new Vector2(0, 0);
                    rootRectTransform.offsetMax = new Vector2(0, 0);
                }

                // Process the container as a runtime object itself so it does not persist
                ProcessRuntimeObject(root);
            }

            return root;
        }

        /// <summary>
        /// Get a KeyboardLayout from the KeyLayoutSource attached to this keyboard renderer
        /// </summary>
        /// <returns>A KeyboardLayout to render</returns>
        public KeyboardLayout GetKeyLayout()
        {
            VRTK_BaseKeyLayoutSource source = GetComponent<VRTK_BaseKeyLayoutSource>();
            if (source == null)
            {
                return null;
            }

            KeyboardLayout layout = source.GetKeyLayout();

            if (layout == null)
            {
                Debug.LogWarning(source.GetType().Name + " in " + name + " did not return a renderable key layout");
                return null;
            }

            keysetCount = layout.keysets.Length;

            return layout;
        }

        /// <summary>
        /// Get a RenderableKeyLayout from the KeyLayoutCalculator attached to this keyboard renderer
        /// </summary>
        /// <returns>A RenderableKeyLayout to render</returns>
        public RKeyLayout CalculateRenderableKeyLayout(Vector2[] containerSizes)
        {
            VRTK_BaseKeyLayoutCalculator calculator = GetComponent<VRTK_BaseKeyLayoutCalculator>();
            if (calculator == null)
            {
                Debug.LogError(GetType().Name + " in " + name + " requires a Key Layout Calculator on the same object.");
                return null;
            }

            RKeyLayout layout = calculator.CalculateKeyLayout(containerSizes);
            if (layout == null)
            {
                Debug.LogWarning(calculator.GetType().Name + " in " + name + " did not return a renderable key layout");
                return null;
            }

            keysetCount = layout.keysets.Length;

            return layout;
        }

        /// <summary>
        /// Return a UnityAction that can be used as a listener to handle keypresses for a renderable key
        /// </summary>
        /// <param name="key">The RenderableKeyLayout.Key to handle a keypress for</param>
        /// <returns>A UnityAction to use in listeners</returns>
        protected UnityAction GetKeypressHandler(IKey key)
        {
            return new UnityAction(() =>
            {
                keyboard.HandleKeypress(key);
            });
        }

        /// <summary>
        /// The SetupKeyboardUI method should reset the canvas's children and create
        /// the game objects that make up a rendered keyboard.
        /// </summary>
        public abstract void SetupKeyboardUI();

        /// <summary>
        /// Changes the currently active keyset
        /// </summary>
        /// <param name="keyset">The index of the keyset to switch to</param>
        public void SetKeyset(int keyset)
        {
            if (keysetCount == 0)
            {
                Debug.LogError("SetKeyset called on " + name + " before keysets have been rendered");
                return;
            }

            if (keyset >= keysetCount)
            {
                Debug.LogWarning(name + " does not have a keyset with the index " + keyset);
                return;
            }

            currentKeyset = keyset;
            UpdateActiveKeyset();
        }

        /// <summary>
        /// Update enabled/active on keyset objects to change which one is dispalyed
        /// </summary>
        protected virtual void UpdateActiveKeyset()
        {
            foreach (KeyValuePair<GameObject, int> keysetObject in keysetObjects)
            {
                keysetObject.Key.SetActive(keysetObject.Value == currentKeyset);
            }
        }

        /// <summary>
        /// Sets whether enter keys should be enabled or disabled/replaced with a done button.
        /// </summary>
        /// <param name="enterEnabled">Is the input field multiline?</param>
        public void SetEnterEnabled(bool enterEnabled)
        {
            isEnterEnabled = enterEnabled;
        }
    }
}
