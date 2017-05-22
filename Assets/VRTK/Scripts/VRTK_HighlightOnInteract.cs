//=====================================================================================
//
// Purpose: Provide a mechanism for highlighting interactable like in IKEA Experience
//
// This script should be attached to any object that needs touch, use or grab
//
// An optional highlight color can be set to change the object's appearance if it is
// invoked.
//
//=====================================================================================
namespace VRTK
{
    using UnityEngine;

    public class VRTK_HighlightOnInteract : MonoBehaviour
    {
        [Header("Colours")]
        public Color touchedOutlineColor = new Color(200, 200, 0, 200);
        public Color idleOutlineColor = new Color(255, 255, 255, 200);
        public Color grabbedOutlineColor = new Color(0, 0, 0, 0);        
        public Color inactiveOutlineColor = new Color(0, 0, 0, 0);

        [Space(10)]
        [Tooltip("Dont affects on runtime")]
        [SerializeField]
        private bool _isActiveAtStart = false;
        [Header("Dependencies")]
        [Tooltip("Let None if VRTK_InteractableObject already attached to this GameObject")]
        [SerializeField]
        private VRTK_InteractableObject _interactableObject;

        [Tooltip("Tooltip associated with this object that will be hidden and shown with the outline")]
        [SerializeField]
        private GameObject _toolTip;

        

        private Renderer _renderer;
        private VRTK_InteractableObject _interactable;

        public void SetHighlightActive(bool active)
        {
            if (active)
            {
                SetStateIdle(true);
            }
            else
            {
                SetStateInactive();
            }
            
        }

        void Awake()
        {
            if (_interactable == null)
            {
                _interactable = GetComponent<VRTK_InteractableObject>();
            }
            if (_interactable == null)
            {
                throw new MissingReferenceException("VRTK_HighlightOnInteract needs a VRTK_Interactable object to work properly");
            }

            _interactable.InteractableObjectTouched += (s, e) => SetStateTouched();
            _interactable.InteractableObjectGrabbed += (s, e) => SetStateGrabbed();
            _interactable.InteractableObjectUntouched += (s, e) => SetStateIdle();
            _interactable.InteractableObjectUngrabbed += (s, e) => SetStateIdle(false, true);

            // Adding material with custom shader to make the outline effect
            _renderer = GetComponent<Renderer>();
            var materials = _renderer.materials;
            var length = materials.Length;
            Material[] newmaterials = new Material[length + 1];
            materials.CopyTo(newmaterials, 0);
            var outlineMaterial = Resources.Load("OutlineHighlighter") as Material;
            newmaterials[newmaterials.Length - 1] = outlineMaterial;
            _renderer.materials = newmaterials;

            // Resetting shader color by the checkbox state
            if (_isActiveAtStart)
            {
                SetStateIdle(true);
            }
            else
            {
                SetStateInactive();
            }
        }

        private OutlineState _currentState;
        public enum OutlineState
        {
            IDLE, INACTIVE, TOUCHED, GRABBED
        }

        private void SetState(OutlineState nextState)
        {
            _currentState = nextState;
        }

        private void SetStateIdle(bool forced = false, bool grabbed = false)
        {
            if (_currentState != OutlineState.TOUCHED && !grabbed)
            {
                if (!forced)
                {
                    return;
                }
            }
            if (_currentState == OutlineState.INACTIVE && !forced)
            {
                return;
            }
            SetState(OutlineState.IDLE);
            SetColor(idleOutlineColor);
            if (_toolTip != null)
            {
                _toolTip.SetActive(true);
            }

        }

        private void SetStateInactive()
        {
            SetState(OutlineState.INACTIVE);
            SetColor(inactiveOutlineColor);
            if (_toolTip != null)
            {
                _toolTip.SetActive(false);
            }
        }

        private void SetStateTouched()
        {
            if (_currentState == OutlineState.INACTIVE || _currentState == OutlineState.GRABBED)
            {
                return;
            }
            SetState(OutlineState.TOUCHED);
            SetColor(touchedOutlineColor);
            if (_toolTip != null)
            {
                _toolTip.SetActive(false);
            }
        }

        private void SetStateGrabbed()
        {
            if (_currentState != OutlineState.TOUCHED)
            {
                return;
            }
            SetState(OutlineState.GRABBED);
            SetColor(grabbedOutlineColor);
            if (_toolTip != null)
            {
                _toolTip.SetActive(false);
            }
        }

        

        private void SetColor(Color color)
        {
            var materials = _renderer.materials;
            foreach (Material material in materials)
            {
                if (material.name == "OutlineHighlighter (Instance)")
                { 
                    material.SetColor("_OutlineColor", color);
                }
            }
        }
    }

}

}

}
