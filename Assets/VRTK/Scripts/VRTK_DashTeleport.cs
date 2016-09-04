namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_DashTeleport : VRTK_HeightAdjustTeleport
    {
        public delegate void DashAction(RaycastHit[] hit);
        public static event DashAction OnWillDashThruObjects;
        public static event DashAction OnDashedThruObjects;
        public float normalLerpTime = 0.1f; // 100ms for every dash above minDistanceForNormalLerp
        public float minSpeedMps = 50.0f; // clamped to minimum speed 50m/s to avoid sickness
        public float capsuleTopOffset = 0.2f;
        public float capsuleBottomOffset = 0.5f;
        public float capsuleRadius = 0.5f;

        // The minimum distance for fixed time lerp is determined by the minSpeed and the normalLerpTime
        // If you want to always lerp with a fixed mps speed, set the normalLerpTime to a high value
        private float minDistanceForNormalLerp;
        private float lerpTime = 0.1f;

        protected override void Awake()
        {
            base.Awake();
            minDistanceForNormalLerp = minSpeedMps * normalLerpTime; // default values give 5.0f
        }

        protected override void SetNewPosition(Vector3 position, Transform target)
        {
            Vector3 targetPosition = CheckTerrainCollision(position, target);
            StartCoroutine(lerpToPosition(targetPosition, target));
        }

        private IEnumerator lerpToPosition(Vector3 targetPosition, Transform target)
        {
            base.enableTeleport = false;
            bool gameObjectInTheWay = false;

            // Find the objects we will be dashing through and broadcast them via events
            Vector3 eyeCameraPosition = eyeCamera.transform.position;
            Vector3 eyeCameraPositionOnGround = new Vector3(eyeCameraPosition.x, transform.position.y, eyeCameraPosition.z);
            Vector3 eyeCameraRelativeToRig = eyeCameraPosition - transform.position;
            Vector3 targetEyeCameraPosition = targetPosition + eyeCameraRelativeToRig;
            Vector3 direction = (targetEyeCameraPosition - eyeCameraPosition).normalized;
            Vector3 bottomPoint = eyeCameraPositionOnGround + (Vector3.up * capsuleBottomOffset) + direction;
            Vector3 topPoint = eyeCameraPosition + (Vector3.up * capsuleTopOffset) + direction;
            float maxDistance = Vector3.Distance(transform.position, targetPosition - direction * 0.5f);
            RaycastHit[] allHits = Physics.CapsuleCastAll(bottomPoint, topPoint, capsuleRadius, direction, maxDistance);

            foreach (RaycastHit hit in allHits)
            {
                gameObjectInTheWay = hit.collider.gameObject != target.gameObject ? true : false;
            }

            if (gameObjectInTheWay && OnWillDashThruObjects != null)
            {
                OnWillDashThruObjects(allHits);
            }

            if (maxDistance >= minDistanceForNormalLerp)
            {
                lerpTime = normalLerpTime; // fixed time for all bigger dashes
            }
            else
            {
                lerpTime = (1 / minSpeedMps) * maxDistance; // clamped to speed for small dashes
            }

            Vector3 startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            float elapsedTime = 0;
            float t = 0;

            while (t < 1)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                t = elapsedTime / lerpTime;
                if (t > 1)
                {
                    t = 1;
                }
                yield return new WaitForEndOfFrame();
            }

            if (gameObjectInTheWay && OnDashedThruObjects != null)
            {
                OnDashedThruObjects(allHits);
            }

            gameObjectInTheWay = false;
            base.enableTeleport = true;
        }
    }
}
