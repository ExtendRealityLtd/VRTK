# Windows Mixed Reality

## Instructions for using Windows Mixed Reality

  > Compatible with Unity 2017.3 and above.
 
 * Follow the [Windows Mixed Reality Setup Guide](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools) to ensure Unity is set up correctly for Windows Mixed Reality Support.
 * Switch the Unity Platform in the Unity Build Settings to `Unified Windows Platform`.
 * Follow the initial [Getting Started](/Assets/VRTK/Documentation/GETTING_STARTED.md) steps and then add the `VRTK/Source/SDK/WindowsMR/[WindowsMR_CameraRig]` prefab as a child of the SDK Setup GameObject.
 
## Instructions for adding Windows Mixed Reality controller model support
 
 * Import the [VRTK Windows Mixed Reality Extension](https://github.com/Innoactive/VRTK-Windows-MR-Extension) to your Unity project. Follow instructions in the repository on how to do.
 * Add the `MotionControllerVisualizer` component from the [VRTK Windows Mixed Reality Extension](https://github.com/Innoactive/VRTK-Windows-MR-Extension) to the `[WindowsMR_CameraRig]/ControllerManager` GameObject of the WindowsMR SDK Setup child.