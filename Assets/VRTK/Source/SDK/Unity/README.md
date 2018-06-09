# UnityEngine.VR

## Instructions for using the built in Unity3d VR support

 * Follow the initial [Getting Started](/Assets/VRTK/Documentation/GETTING_STARTED.md) steps and then add the `VRTK/Source/SDK/Unity/[UnityBase_CameraRig]` prefab as a child of the SDK Setup GameObject.
 * The Unity Input Manager must also be configured to utilise the axes on the VR controller.
 * Open up the Unity Input Manager tab `Edit -> Project Settings -> Input`.
 * Increase the size of the `Axes` array to the number of required new axes (usually increase by 8, 2 for trigger, 2 for grip and 4 for touchpad [x,y] which is for both hands).
 * For each new axis created, enter a name for the axis that corresponds to the name specified on the `SDK_UnityControllerTrackerScript` which is found on `[UnityBase_CameraRig/<xxxx>HandAnchor]`.
 * The axis mapping information can be found in the [Unity3d Docs](https://docs.unity3d.com/Manual/OpenVRControllers.html).
 * The touchpad y axis is reversed from other SDKs, so they should be inverted in the Input Manager in order for them to behave the same.