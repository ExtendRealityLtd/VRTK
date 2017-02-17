// Base Key Layout Renderer|Keyboard|81020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;
    using IKey = VRTK_Keyboard.IKey;
    using RKeyLayout = VRTK_RenderableKeyLayout;

    /// <summary>
    /// This abstract class is the base class for key layout renderers used to render functional keyboard to a GameObject
    /// </summary>
    /// <remarks>
    /// A key layout calculator component is required on the same gameobject as the key layout renderer.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    [RequireComponent(typeof(VRTK_Keyboard))]
    public abstract class VRTK_BaseKeyLayoutRenderer : MonoBehaviour
    {
        protected VRTK_Keyboard keyboard;
        protected int currentKeyset = 0;
        protected int keysetCount = 0;
        protected bool isEnterEnabled = true;

        protected virtual void Start()
        {
            keyboard = GetComponent<VRTK_Keyboard>();

            if (GetComponent<VRTK_BaseKeyLayoutCalculator>() == null)
            {
                Debug.LogError(GetType().Name + " in " + name + " requires a Key Layout Calculator on the same object.");
            }

            SetupKeyboardUI();
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
        protected void ProcessRuntimeObject(GameObject obj)
        {
            if (Application.isEditor)
            {
                obj.hideFlags = HideFlags.DontSave;// | HideFlags.NotEditable;
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
        /// Get a RenderableKeyLayout from the KeyLayoutCalculator attached to this keyboard renderer
        /// </summary>
        /// <returns>A RenderableKeyLayout to render</returns>
        public RKeyLayout CalculateRenderableKeyLayout(Vector2 containerSize)
        {
            VRTK_BaseKeyLayoutCalculator calculator = GetComponent<VRTK_BaseKeyLayoutCalculator>();
            if (calculator == null)
            {
                return null;
            }

            RKeyLayout layout = calculator.CalculateKeyLayout(containerSize);
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
