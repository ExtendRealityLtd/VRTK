// Swap Controller Grab Action|SecondaryControllerGrabActions|60020
namespace VRTK.SecondaryControllerGrabActions
{
    /// <summary>
    /// The Swap Controller Grab Action provides a mechanism to allow grabbed objects to be swapped between controllers.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the ability to swap objects between controllers on grab.
    /// </example>
    public class VRTK_SwapControllerGrabAction : VRTK_BaseGrabAction
    {
        protected virtual void Awake()
        {
            isActionable = false;
            isSwappable = true;
        }
    }
}
