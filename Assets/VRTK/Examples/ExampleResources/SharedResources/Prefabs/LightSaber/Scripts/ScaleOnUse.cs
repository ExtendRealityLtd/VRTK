namespace VRTK.Examples
{
    using UnityEngine;
    using System.Collections;

    public class ScaleOnUse : MonoBehaviour
    {
        public VRTK_InteractableObject linkedObject;
        public GameObject objectToScale;
        public Vector3 unuseScale = Vector3.zero;
        public Vector3 useScale = Vector3.one;
        public float scaleSpeed = 1f;

        protected Vector3 scaleTarget;
        protected Coroutine scaleRoutine;

        protected virtual void OnEnable()
        {
            linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

            if (linkedObject != null)
            {
                linkedObject.InteractableObjectUsed += InteractableObjectUsed;
                linkedObject.InteractableObjectUnused += InteractableObjectUnused;
            }
            ForceScale(unuseScale);
        }

        protected virtual void OnDisable()
        {
            CancelScale();

            if (linkedObject != null)
            {
                linkedObject.InteractableObjectUsed -= InteractableObjectUsed;
                linkedObject.InteractableObjectUnused -= InteractableObjectUnused;
            }
        }

        protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            CancelScale();
            scaleRoutine = StartCoroutine(ScaleRoutine(objectToScale.transform.localScale, useScale));
        }

        protected virtual void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
        {
            CancelScale();
            scaleRoutine = StartCoroutine(ScaleRoutine(objectToScale.transform.localScale, unuseScale));
        }

        protected virtual void CancelScale()
        {
            if (scaleRoutine != null)
            {
                StopCoroutine(scaleRoutine);
                scaleRoutine = null;
            }
        }

        protected virtual IEnumerator ScaleRoutine(Vector3 originalScale, Vector3 targetScale)
        {
            if (objectToScale != null)
            {
                float progress = 0;
                while (progress <= 1)
                {
                    objectToScale.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                    progress += Time.deltaTime * scaleSpeed;
                    yield return null;
                }
                ForceScale(targetScale);
            }
        }

        protected virtual void ForceScale(Vector3 targetScale)
        {
            if (objectToScale != null)
            {
                objectToScale.transform.localScale = targetScale;
            }
        }
    }
}