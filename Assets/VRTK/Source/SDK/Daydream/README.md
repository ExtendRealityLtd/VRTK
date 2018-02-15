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
   * A new `Camera` and set its tag to `MainCamera`
   * The `GvrControllerPointer` prefab from `GoogleVR/Prefabs/Controller`.
   * The `GvrControllerMain` prefab from `GoogleVR/Prefabs/Controller`.
 * Disable Daydream's native pointer tools by deleting `DaydreamCameraRig/GvrControllerPointer/Laser`.
 * Follow the initial steps above by adding the `DaydreamCameraRig` object as a child of the SDK Setup GameObject.

  > Note: Daydream supports only one controller, the left scripting alias controller of VRTK will not be used.