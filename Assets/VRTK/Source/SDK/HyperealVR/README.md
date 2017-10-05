# HyperealVR *(experimental)*

## Instructions for using HyperealVR

 * Download the [Hypereal VR Unity Plugin](https://www.hypereal.com/#/developer/resource/download) from Hypereal official website. 
 * Import the `.unitypackage`.
 * Drag `HyperealVRMain` prefab into the scene. OR
   * Create an empty GameObject named `HyperealVRMain`.
   * Create a GameObject named `Camera` under `HyperealVRMain`.
   * Attach `HyCamera` script onto `Camera` and click `Expand`.
   * Create a GameObject named `HyTrackObjRig` under `Camera(origin)` and then create two gameobjects named `Left Controller` and `Right Controller` under it.
   * Attach `HyTrackObjRig` to `HyTrackObjRig` and assign `Left/Right Controller` to corresponding field.
   * Attach `HyTrackObj` and `HyRenderModel` script to `Left/Right Controller`. For `Left/Right Controller`, set `Device` field on the two scripts to `Device_Controller 0/1`.
     > Names of gameobjects are placeholders, you can use your own.
 * Attach `VRTK_SDKSetup` onto `HyperealVRMain` and choose `Hypereal SDK` in `SDK selection` field.

  > Note: 
  > * For some versions, `HyPointerLaser` script is attached on the controllers. Remove them before use.
  > * Since we only have one button on each controller, we could not map all VRTK buttons.
  >   * The menu button on the right controller is mapped to VRTK button-menu.
  >   * The menu button on the left is mapped to button-two of VRTK.
  >   * VRTK button-one has not been mapped to any button of the controller.
  >   * Double clicking the menu button is reserved as system menu.
  >   * Single clicking