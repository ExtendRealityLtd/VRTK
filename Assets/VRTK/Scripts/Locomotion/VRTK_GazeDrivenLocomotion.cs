// Gaze Driven Movement|Scripts|0070
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    /// <summary>
    /// The Gaze Driven Movement script creations a directional "joystick" underneath a given game object within the seen. Movement is driven by moving the joystick using the HMD to drag across the gamepad.
    /// </summary>
    /// <remarks>
    /// This script is meant to be used for 3rd person VR games.
    /// Attach this script to the `[CameraRig]` prefab. Attach the VRTK_DirectionPad prefab to get the UI for the directional pad.
    /// </remarks>
    /// <example> 
    /// `VRTK/Examples/044_Controller_Events` shows an example of 3rd person locomotion using the HMD, along with the provided camera script. This method was developed with use of `VRTK_ThirdPersonCamera` in mind
    /// </example>
    public class VRTK_GazeDrivenLocomotion : MonoBehaviour
    {
        [Tooltip("The speed of the character when HMD gaze is at the edge of the game pad")]
        public float maxSpeed = 2.0f;
        [Tooltip("The directionpad UI that will show the user where their gaze is and how it's affected the locomotion of the character. Use the DirectionPad Prefab in VRTK/Prefabs")]
        [SerializeField]
        private GameObject directionPad;
        [Tooltip("The character game object that you want to locomote.")]
        [SerializeField]
        private GameObject characterModel;
        [Tooltip("Sets the size of the gamepad that spawns underneath the player, which affects the sensitivity and where users look to drive movement of character")]
        [SerializeField]
        private float gamePadSize;
        [Tooltip("The character should be attached to an empty game object, so that certain objects that should not rotate with the character can be attached the empty root object")]
        [SerializeField]
        private GameObject characterController;
        private Quaternion characterLookRotation;
        private Vector3 directionVector = new Vector3(0, 0, 0);
        private GameObject interactionPlane;
        private Slider slider;
        private Transform hmd;

        protected void Start()
        {
            hmd = VRTK_DeviceFinder.HeadsetCamera();
            CreateInteractionPlane();
            CreateDirectionPadUI();
        }

        protected void Update()
        {
            Ray ray = new Ray(hmd.position, hmd.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject collisionObject = hit.transform.gameObject;
                if (hit.collider.gameObject == interactionPlane)
                {
                    Vector3 localizedHitCoordinates = hit.transform.InverseTransformPoint(hit.point);
                    float x = localizedHitCoordinates.x / ((.5f));
                    float y = localizedHitCoordinates.y / ((.5f));
                    directionVector = new Vector3(x, 0, y);
                    Vector3 directionalUnitVector = (hit.point - directionPad.transform.position).normalized;
                    UpdateDirectionPadUI(directionalUnitVector, Mathf.Clamp(directionVector.magnitude, 0, 1));
                    Vector3 CharacterLookVector = (hit.point - characterModel.transform.position).normalized;
                    CharacterLookVector.y = 0;
                    characterLookRotation = Quaternion.LookRotation(CharacterLookVector);
                    characterController.transform.Translate(CalculateSpeedVector(directionVector));
                }
                else
                {
                    directionVector = Vector3.zero;
                }
            }
        }

        protected void LateUpdate()
        {
            characterModel.transform.rotation = characterLookRotation;
        }

        private void CreateDirectionPadUI()
        {
            directionPad = Instantiate(directionPad, characterModel.transform);
            directionPad.transform.localScale = new Vector3(gamePadSize, .1f, gamePadSize);
            directionPad.transform.position = characterModel.transform.position;
            directionPad.transform.position = new Vector3(directionPad.transform.position.x, directionPad.transform.position.y - 0.25f, directionPad.transform.position.z);
            slider = directionPad.GetComponentInChildren<Slider>();
        }

        private void UpdateDirectionPadUI(Vector3 pointDirection, float magnitude)
        {
            pointDirection.y = 0;
            directionPad.transform.rotation = Quaternion.LookRotation(pointDirection);
            slider.value = magnitude;
        }

        private void CreateInteractionPlane()
        {
            interactionPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interactionPlane.GetComponent<MeshRenderer>().enabled = false;
            interactionPlane.transform.localScale = new Vector3(gamePadSize, gamePadSize, 0.1f);
            interactionPlane.transform.parent = characterController.transform;
            interactionPlane.transform.position = characterController.transform.position;
            interactionPlane.transform.position = new Vector3(interactionPlane.transform.position.x, interactionPlane.transform.position.y - 0.25f, interactionPlane.transform.position.z);
            interactionPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        private Vector3 CalculateSpeedVector(Vector3 directionVector)
        {
            return maxSpeed * directionVector * Time.deltaTime;
        }
    }
}