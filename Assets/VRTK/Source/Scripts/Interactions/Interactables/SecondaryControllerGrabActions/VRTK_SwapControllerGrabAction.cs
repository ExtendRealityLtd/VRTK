// Swap Controller Grab Action|SecondaryControllerGrabActions|60020
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// Swaps the grabbed Interactable Object to the new grabbing object.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_SwapControllerGrabAction` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the ability to swap objects between controllers on grab.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Secondary Controller Grab Actions/VRTK_SwapControllerGrabAction")]
    public class VRTK_SwapControllerGrabAction : VRTK_BaseGrabAction
    {
        protected virtual void Awake()
        {
            isActionable = false;
            isSwappable = true;
        }
    }
}
