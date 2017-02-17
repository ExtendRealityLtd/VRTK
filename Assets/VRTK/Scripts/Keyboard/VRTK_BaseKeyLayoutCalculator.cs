// Base Key Layout Calculator|Keyboard|81030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This abstract class is the base class for any class used to generate a `VRTK_RenderableKeyLayout`
    /// </summary>
    /// <remarks>
    /// Most implementations will extend `VRTK_BaseKeyboardLayoutCalculator` and make calculations for a `VRTK_KeyboardLayout`.
    /// 
    /// However custom implementations can extend this directly to render a custom key layout not based off of a keyboard.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseKeyLayoutCalculator : MonoBehaviour
    {
        /// <summary>
        /// Generate a renderable key layout to be rendered into a container (canvas, mesh, ...)
        /// </summary>
        /// <param name="containerSize">The dimensions of the container to render into</param>
        /// <returns>The generated renderable key layout</returns>
        public abstract VRTK_RenderableKeyLayout CalculateKeyLayout(Vector2 containerSize);
    }
}
