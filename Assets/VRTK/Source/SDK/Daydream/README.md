# Daydream *(experimental)*

## Instructions for using Daydream

 * Open a new or existing project in Unity that offers Daydream integration.
 * Download the [Google VR SDK](https://developers.google.com/vr/unity/download) from the Google developer website.
 * Import the `.unitypackage`.
 * Switch the build settings in `File > Build Settings...` to `Android`.
 * In `Edit > Project Settings > Player` set the following:
   * API Level to `Nougat`.
   * Bundle Identifier and other settings for use with Android.
 * In the `Hierarchy` window, create a new empty GameObject named `DaydreamCameraRig`.
 * Add the following as children of `DaydreamCameraRig`:
   * A new `Camera`.
   * The `GvrControllerPointer` prefab from `GoogleVR/Prefabs/UI`.
   * The `GvrControllerMain` prefab from `GoogleVR/Prefabs/Controller`.
   * The `GvrViewerMain` prefab from `GoogleVR/Prefabs` (enables the view in editor play mode).
 * Disable Daydream's native pointer tools by removing or disabling `DaydreamCameraRig/GvrControllerPointer/Laser`.
 * Follow the initial steps above by adding the `DaydreamCameraRig` object as a child of the SDK Setup GameObject.

  > Note: Daydream supports only one controller, the left scripting alias controller of VRTK will not be used.