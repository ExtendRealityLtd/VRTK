# Daydream *(experimental)*

## Instructions for using Daydream

 * Open a new or existing project in Unity that offers Daydream integration.
 * Download the [Google VR SDK](https://developers.google.com/vr/unity/download) from the Google developer website.
 * Import the `.unitypackage`.
 * Switch the build settings in `File > Build Settings...` to `Android`.
 * In `Edit > Project Settings > Player` set the following:
   * API Level to `Nougat`.
   * Bundle Identifier and other settings for use with Android.
 * In the `Hierarchy` window, create a new empty GameObject named `[VRTK_SDK_Manager]`.
 * Add Component `VRTK_SDKManager`.
 * Add `SDKSetups` prefab from Assets/VRTK/Examples/ExampleResources/Prefabs/ as a child of `[VRTK_SDK_Manager]`.
 * Delete all the SDKs except `Daydream` from children of `SDKSetups`.
 * Click Auto Populate in the inspector of `[VRTK_SDK_Manager]`.

  > Note: Daydream supports only one controller, the left scripting alias controller of VRTK will not be used.