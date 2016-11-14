namespace VRTK.Examples.Utilities
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class Setup_Scene_031 : MonoBehaviour
    {
        private bool initalised = false;

        private void Awake()
        {
            Initialise();
        }

#if UNITY_EDITOR
        private void Update()
        {
            Initialise();
        }
#endif
        private void Initialise()
        {
            if (!initalised)
            {
                var headset = VRTK_DeviceFinder.HeadsetTransform();
                if (!headset.GetComponent<VRTK_BezierPointer>())
                {
                    var pointer = headset.gameObject.AddComponent<VRTK_BezierPointer>();

                    pointer.controller = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
                    pointer.pointerVisibility = VRTK_BasePointer.pointerVisibilityStates.Always_Off;
                    pointer.pointerLength = 7f;
                    pointer.pointerDensity = 1;
                    pointer.pointerCursorRadius = 0.3f;
                    pointer.beamCurveOffset = 1f;
                }
                if (!headset.GetComponent<VRTK_PlayAreaCursor>())
                {
                    headset.gameObject.AddComponent<VRTK_PlayAreaCursor>();
                }
                initalised = true;
            }
        }
    }
}