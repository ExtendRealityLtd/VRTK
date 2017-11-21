// Interactable Listener|Interactables|35015
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Provides a base that classes which require to subscribe to the interaction events of an Interactable Object can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides interaction event listener functionality, therefore this script should not be directly used.
    /// </remarks>
    public abstract class VRTK_InteractableListener : MonoBehaviour
    {
        protected Coroutine setupInteractableListenersRoutine;

        protected abstract bool SetupListeners(bool throwError);
        protected abstract void TearDownListeners();

        protected virtual void EnableListeners()
        {
            if (!SetupListeners(false))
            {
                setupInteractableListenersRoutine = StartCoroutine(SetupListenersAtEndOfFrame());
            }
        }

        protected virtual void DisableListeners()
        {
            if (setupInteractableListenersRoutine != null)
            {
                StopCoroutine(setupInteractableListenersRoutine);
                setupInteractableListenersRoutine = null;
            }
            TearDownListeners();
        }

        protected virtual IEnumerator SetupListenersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            SetupListeners(true);
        }
    }
}