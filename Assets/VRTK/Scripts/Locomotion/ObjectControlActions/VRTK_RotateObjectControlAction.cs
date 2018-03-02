// Rotate Object Control Action|ObjectControlActions|25020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Rotate Object Control Action script is used to rotate the controlled GameObject around the up vector when changing the axis.
    /// </summary>
    /// <remarks>
    /// The effect is a smooth rotation to simulate turning.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Rotate Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Rotate` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_RotateObjectControlAction")]
    public class VRTK_RotateObjectControlAction : VRTK_BaseObjectControlAction
    {
        [Tooltip("The maximum speed the controlled object can be rotated based on the position of the axis.")]
        public float maximumRotationSpeed = 3f;
        [Tooltip("The rotation multiplier to be applied when the modifier button is pressed.")]
        public float rotationMultiplier = 1.5f;

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            //Since this script rotates the Gameobject "[CameraRig]" and the player's head does not always have the localPosition (0,0,0), the rotation will result in a position offset of player's head. The Camera will moved relativly to compensate that. Therefore it will save the player's head position. Then the Rotation will be executed. Afterwards the player's head position will be corrected.
            Vector3 PlayerHeadPositionBeforeRotation = new Vector3();
            //This script can be used to rotate other Objects, therefore it will be checked, whether it is the CameraRig.
            VRTK_PlayerObject vrtk_PlayerObject = controlledGameObject.GetComponent<VRTK_PlayerObject>();
            if(vrtk_PlayerObject != null && vrtk_PlayerObject.objectType == VRTK_PlayerObject.ObjectTypes.CameraRig && VRTK_DeviceFinder.HeadsetTransform() != null)
            {
                //Save the player's head position.
                PlayerHeadPositionBeforeRotation = VRTK_DeviceFinder.HeadsetTransform().position;
            }

            float angle = Rotate(axis, modifierActive);
            if (angle != 0f)
            {
                RotateAroundPlayer(controlledGameObject, angle);
            }

            //If necessary the player's head position will be corrected by translate the Gameobject [CameraRig] relativly.
            if (vrtk_PlayerObject != null && vrtk_PlayerObject.objectType == VRTK_PlayerObject.ObjectTypes.CameraRig && VRTK_DeviceFinder.HeadsetTransform() != null)
            {
                Vector3 PositionCorrection = PlayerHeadPositionBeforeRotation - VRTK_DeviceFinder.HeadsetTransform().position;
                //The Gameobject [CameraRig] translate relativly to compensate unintentionally rotation (controlledGameObject.transform == [CameraRig]). 
                controlledGameObject.transform.position += PositionCorrection;
            }
        }

        protected virtual float Rotate(float axis, bool modifierActive)
        {
            return axis * maximumRotationSpeed * Time.deltaTime * (modifierActive ? rotationMultiplier : 1) * 10;
        }
    }
}