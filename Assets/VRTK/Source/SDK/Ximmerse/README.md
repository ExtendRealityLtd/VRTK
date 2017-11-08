# Ximmerse *(experimental)*

## Instructions for using Ximmerse

 * Download the [Ximmerse Unity SDK](https://github.com/Ximmerse/SDK/tree/master/Unity) from the Ximmerse SDK Github page.
 * Import the `XIMxxx.unitypackage`.
 * Follow the initial [Getting Started](/Assets/VRTK/Documentation/GETTING_STARTED.md) steps and then add the `VRCameraRig` prefab from the [Ximmerse Unity SDK](https://github.com/Ximmerse/SDK/tree/master/Unity) as a child of the SDK Setup GameObject.
   * It is recommened to use `Floor Level` as the `Tracking Origin Type`, with the position of `VRCameraRig` set to `(0f, 0f, 0f)`. `Eye Level` can also be used, in this case it is recommended to set the position to `(0f, 1.675f, 0f)`.
   * Make sure `SimplePicker` is **not** attached to any of the GameObjects `cobra02-L` and `cobra02-R`. `SimplePicker` is provided by the Ximmerse SDK but using the script may break VRTK's grab functionality.
 * Switch the build settings in `File > Build Settings...` to `Android`.

  > Currently Ximmerse 6DOF tracking is only supported on Android. 3DOF tracking is supported on both iOS and Android. Ximmerse are getting MFI certification from Apple at the moment.