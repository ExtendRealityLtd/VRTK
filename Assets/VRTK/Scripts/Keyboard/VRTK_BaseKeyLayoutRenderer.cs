// Base Key Layout Renderer|Keyboard|81020
namespace VRTK
{
    using UnityEngine;
    using RKeyLayout = VRTK_RenderableKeyLayout;

    /// <summary>
    /// This abstract class is the base class for key layout renderers used to render functional keyboard to a GameObject
    /// </summary>
    /// <remarks>
    /// A key layout calculator component is required on the same gameobject as the key layout renderer.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseKeyLayoutRenderer : MonoBehaviour
    {
        protected int currentKeyset = 0;

        protected virtual void Start()
        {
            if (GetComponent<VRTK_BaseKeyLayoutCalculator>() == null)
            {
                Debug.LogError(GetType().Name + " requires a Key Layout Calculator on the same object.");
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
                Debug.LogWarning(calculator.GetType().Name + " did not return a renderable key layout");
                return null;
            }

            return layout;
        }

        /// <summary>
        /// The SetupKeyboardUI method should reset the canvas's children and create
        /// the game objects that make up a rendered keyboard.
        /// </summary>
        public abstract void SetupKeyboardUI();
    }
}
