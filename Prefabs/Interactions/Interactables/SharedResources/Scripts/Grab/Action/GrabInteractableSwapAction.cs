namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using VRTK.Prefabs.Interactions.Interactables.Grab.Provider;

    /// <summary>
    /// Describes the action of swapping a an action from being the secondary action to the primary action.
    /// </summary>
    /// <remarks>
    /// Can only be used in conjunction with <see cref="GrabInteractableStackInteractorProvider"/>.
    /// </remarks>
    public class GrabInteractableSwapAction : GrabInteractableAction
    {
        /// <summary>
        /// Resets the toggle state on the Grab Receiver.
        /// </summary>
        /// <param name="interactor">The Interactor to remove from the toggle state.</param>
        public virtual void ResetToggle(GameObject interactor)
        {
            GrabSetup.GrabReceiver.ToggleList.Remove(interactor);
        }

        /// <summary>
        /// Clears the stack.
        /// </summary>
        public virtual void ClearStack()
        {
            ToStackInteractorProvider(GrabSetup.GrabProvider).EventStack.PopAt(0);
        }

        /// <summary>
        /// Emits the active collision payload.
        /// </summary>
        public virtual void EmitActiveCollisionConsumerPayload()
        {
            GrabSetup.GrabReceiver.OutputActiveCollisionConsumer.EmitPayload();
        }

        /// <summary>
        /// Pushes the given Interactor to the stack.
        /// </summary>
        /// <param name="interactor">The Interactor to push to the stack.</param>
        public virtual void PushToStack(GameObject interactor)
        {
            ToStackInteractorProvider(GrabSetup.GrabProvider).EventStack.Push(interactor);
        }

        /// <summary>
        /// Casts a given <see cref="GrabInteractableInteractorProvider"/> to the required <see cref="GrabInteractableStackInteractorProvider"/> type.
        /// </summary>
        /// <param name="provider">The base provider to cast.</param>
        /// <returns>The casted provider.</returns>
        protected virtual GrabInteractableStackInteractorProvider ToStackInteractorProvider(GrabInteractableInteractorProvider provider)
        {
            return (GrabInteractableStackInteractorProvider)provider;
        }
    }
}