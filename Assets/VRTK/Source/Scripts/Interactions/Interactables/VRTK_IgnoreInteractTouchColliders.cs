// Ignore Interact Touch Colliders|Interactables|35060
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Ignores the collisions between the given Interact Touch colliders and the colliders on the GameObject this script is attached to.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - Unity Colliders on the current GameObject or child GameObjects to ignore collisions from the given Interact Touch colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_IgnoreInteractTouchColliders` script on the GameObject with colliders to ignore collisions from the given Interact Touch colliders.
    ///  * Increase the size of the `Interact Touch To Ignore` element list.
    ///  * Add the appropriate GameObjects that have the `VRTK_InteractTouch` script attached to use when ignoring collisions with the colliders on GameObject the script is attached to.
    /// </remarks>
    public class VRTK_IgnoreInteractTouchColliders : VRTK_SDKControllerReady
    {
        [Tooltip("The Interact Touch scripts to ignore collisions with.")]
        public List<VRTK_InteractTouch> interactTouchToIgnore = new List<VRTK_InteractTouch>();
        [Tooltip("A collection of GameObjects to not include when ignoring collisions with the provided Interact Touch colliders.")]
        public List<GameObject> skipIgnore = new List<GameObject>();

        protected Collider[] localColliders = new Collider[0];
        protected Coroutine disableAllCollidersRoutine;
        protected Coroutine disableControllerCollidersRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            localColliders = GetComponentsInChildren<Collider>(true);
            disableAllCollidersRoutine = StartCoroutine(DisableAllCollidersAtEndOfFrame());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (disableAllCollidersRoutine != null)
            {
                StopCoroutine(disableAllCollidersRoutine);
            }
            if (disableControllerCollidersRoutine != null)
            {
                StopCoroutine(disableControllerCollidersRoutine);
            }
            ManageAllCollisions(false);
            localColliders = new Collider[0];
        }

        protected virtual IEnumerator DisableAllCollidersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ManageAllCollisions(true);
        }

        protected virtual IEnumerator DisableControllerColliderAtEndOfFrame(VRTK_InteractTouch touchToIgnore)
        {
            yield return new WaitForEndOfFrame();
            ManageTouchCollision(touchToIgnore, true);
        }

        protected override void ControllerReady(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                VRTK_InteractTouch foundTouch = controllerReference.scriptAlias.GetComponentInChildren<VRTK_InteractTouch>();
                if (interactTouchToIgnore.Contains(foundTouch))
                {
                    disableControllerCollidersRoutine = StartCoroutine(DisableControllerColliderAtEndOfFrame(foundTouch));
                }
            }
        }

        protected virtual void ManageAllCollisions(bool ignore)
        {
            for (int touchToIgnoreIndex = 0; touchToIgnoreIndex < interactTouchToIgnore.Count; touchToIgnoreIndex++)
            {
                ManageTouchCollision(interactTouchToIgnore[touchToIgnoreIndex], ignore);
            }
        }

        protected virtual bool ShouldExclude(Transform checkObject)
        {
            if (skipIgnore.Contains(checkObject.gameObject))
            {
                return true;
            }
            if (checkObject.parent != null)
            {
                return ShouldExclude(checkObject.parent);
            }
            return false;
        }

        protected virtual void ManageTouchCollision(VRTK_InteractTouch touchToIgnore, bool ignore)
        {
            Collider[] interactTouchColliders = touchToIgnore.ControllerColliders();

            for (int touchCollidersIndex = 0; touchCollidersIndex < interactTouchColliders.Length; touchCollidersIndex++)
            {
                for (int localCollidersIndex = 0; localCollidersIndex < localColliders.Length; localCollidersIndex++)
                {
                    if (localColliders[localCollidersIndex] != null && interactTouchColliders[touchCollidersIndex] != null && !ShouldExclude(localColliders[localCollidersIndex].transform))
                    {
                        Physics.IgnoreCollision(localColliders[localCollidersIndex], interactTouchColliders[touchCollidersIndex], ignore);
                    }
                }
            }
        }
    }
}