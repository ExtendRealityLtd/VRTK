// Base Highlighter|Highlighters|0010
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Base Highlighter is an abstract class that all other highlighters inherit and are required to implement the public methods.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseHighlighter : MonoBehaviour
    {
        [Tooltip("Determines if this highlighter is the active highlighter for the object the component is attached to. Only 1 active highlighter can be applied to a game object.")]
        public bool active = true;

        /// <summary>
        /// The Initalise method is used to set up the state of the highlighter.
        /// </summary>
        /// <param name="color">An optional colour may be passed through at point of initialisation in case the highlighter requires it.</param>
        /// <param name="options">An optional dictionary of highlighter specific options that may be differ with highlighter implementations.</param>
        public abstract void Initialise(Color? color = null, Dictionary<string, object> options = null);

        /// <summary>
        /// The Highlight method is used to initiate the highlighting logic to apply to an object.
        /// </summary>
        /// <param name="color">An optional colour to highlight the game object to. The highlight colour may already have been set in the `Initialise` method so may not be required here.</param>
        /// <param name="duration">An optional duration of how long before the highlight has occured. It can be used by highlighters to fade the colour if possible.</param>

        public abstract void Highlight(Color? color = null, float duration = 0f);
        /// <summary>
        /// The Unhighlight method is used to initiate the logic that returns an object back to it's original appearance.
        /// </summary>
        /// <param name="color">An optional colour that could be used during the unhighlight phase. Usually will be left as null.</param>
        /// <param name="duration">An optional duration of how long before the unhighlight has occured.</param>
        public abstract void Unhighlight(Color? color = null, float duration = 0f);

        /// <summary>
        /// The GetOption method is used to return a value from the options array if the given key exists.
        /// </summary>
        /// <typeparam name="T">The system type that is expected to be returned.</typeparam>
        /// <param name="options">The dictionary of options to check in.</param>
        /// <param name="key">The identifier key to look for.</param>
        /// <returns>The value in the options at the given key returned in the provided system type.</returns>
        public virtual T GetOption<T>(Dictionary<string, object> options, string key)
        {
            if (options != null && options.ContainsKey(key) && options[key] != null)
            {
                return (T)options[key];
            }
            return default(T);
        }
    }
}