namespace VRTK.Examples.Archery
{
    using UnityEngine;

    public class Arrow : MonoBehaviour
    {
        public float maxArrowLife = 10f;
        public float maxCollidedLife = 1f;
        [HideInInspector]
        public bool inFlight = false;

        private bool collided = false;
        private Rigidbody rigidBody;
        private GameObject arrowHolder;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private AudioSource source;

        public void SetArrowHolder(GameObject holder)
        {
            arrowHolder = holder;
            arrowHolder.SetActive(false);
        }

        public void OnNock()
        {
            collided = false;
            inFlight = false;
        }

        public void Fired()
        {
            if (source != null)
            {
                source.Play();
            }
            DestroyArrow(maxArrowLife);
        }

        public void ResetArrow()
        {
            DestroyArrow(maxCollidedLife);
            collided = true;
            inFlight = false;
            RecreateNotch();
            ResetTransform();
        }

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            SetOrigns();
            source = GetComponent<AudioSource>();
        }

        private void SetOrigns()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            if (!collided)
            {
                transform.LookAt(transform.position + rigidBody.velocity);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (inFlight && isActiveAndEnabled && gameObject.activeInHierarchy)
            {
                ResetArrow();
            }
        }

        private void RecreateNotch()
        {
            //swap the arrow holder to be the parent again
            arrowHolder.transform.SetParent(null);
            arrowHolder.SetActive(true);

            //make the arrow a child of the holder again
            transform.SetParent(arrowHolder.transform);

            //reset the state of the rigidbodies and colliders
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            arrowHolder.GetComponent<Rigidbody>().isKinematic = false;
        }

        private void ResetTransform()
        {
            arrowHolder.transform.position = transform.position;
            arrowHolder.transform.rotation = transform.rotation;
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            transform.localScale = originalScale;
        }

        private void DestroyArrow(float time)
        {
            Destroy(arrowHolder, time);
            Destroy(gameObject, time);
        }
    }
}