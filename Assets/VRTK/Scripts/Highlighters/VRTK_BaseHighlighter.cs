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
    }
}