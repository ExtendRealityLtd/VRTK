namespace VRTK.Examples.Prefabs.Items.Confetti
{
    using UnityEngine;

    public class ConfettiCloner : MonoBehaviour
    {
        public GameObject confettiParticle;

        public virtual void MakeClone(float lifetime)
        {
            if (confettiParticle == null)
            {
                return;
            }

            GameObject clone = Instantiate(confettiParticle, confettiParticle.transform.position, confettiParticle.transform.rotation);
            clone.SetActive(true);
            Destroy(clone, lifetime);
        }
    }
}