﻿namespace VRTK {
    using System;
    using UnityEngine;

    public class VRTK_Lever : VRTK_Control {
        public enum Direction {
            x, y, z
        }

        public Direction direction = Direction.y;
        public float min = 0f;
        public float max = 100f;
        public float stepSize = 1f;

        private Rigidbody rb;
        private VRTK_InteractableObject io;
        private HingeJoint hj;

        protected override void initRequiredComponents() {
            initRigidBody();
            initInteractable();
            initJoint();
        }

        protected override bool detectSetup() {
            return true;
        }

        private void initRigidBody() {
            rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.angularDrag = 30; // otherwise lever will continue to move too far on its own
        }

        private void initInteractable() {
            io = GetComponent<VRTK_InteractableObject>();
            if (io == null) {
                io = gameObject.AddComponent<VRTK_InteractableObject>();
            }
            io.isGrabbable = true;
            io.grabSnapType = VRTK_InteractableObject.GrabSnapType.Precision_Snap;
            io.grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Track_Object;
        }

        private void initJoint() {
            hj = GetComponent<HingeJoint>();
            if (hj == null) {
                hj = gameObject.AddComponent<HingeJoint>();
                hj.useLimits = true;
                hj.anchor = new Vector3(0, -0.5f, 0);
                JointLimits limits = hj.limits;

                // this involves quite some guesswork. It is very hard to find general purpose settings but we can try. The user can still create the hingejoint himself.
                switch (direction) {
                    case Direction.x:
                        hj.axis = new Vector3(0, 1, 0);
                        limits.min = -130;
                        break;
                    case Direction.y:
                        hj.axis = new Vector3(0, 0, 1);
                        limits.min = -130;
                        break;
                    case Direction.z:
                        hj.axis = new Vector3(1, 0, 0);
                        limits.min = -130;
                        break;
                }
                hj.limits = limits;
            }
        }

        protected override void handleUpdate() {
            value = calculateValue();
            snapToValue(value);
        }

        private float calculateValue() {
            return Mathf.Round((min + Mathf.Clamp01(Mathf.Abs(hj.angle / (hj.limits.max - hj.limits.min))) * (max - min)) / stepSize) * stepSize;
        }

        private void snapToValue(float value) {
            float angle = ((value - min) / (max - min)) * (hj.limits.max - hj.limits.min);

            // TODO: there is no direct setter, one recommendation by Unity staff is to "abuse" min/max which seems the most reliable but not working so far
            JointLimits oldLimits = hj.limits;
            JointLimits tempLimits = hj.limits;
            tempLimits.min = angle;
            tempLimits.max = angle;
            hj.limits = tempLimits;
            hj.limits = oldLimits;
        }
    }
}