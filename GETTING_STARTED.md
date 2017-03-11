# Getting Started

> *VRTK requires a supported VR SDK to be imported into your Unity3d Project.*

 * Clone this repository `git clone https://github.com/thestonefox/VRTK.git`.
 * Open `VRTK` within Unity3d.
 * Add the `VRTK_SDKManager` script to a GameObject in the scene.

## VR Simulator
 
<details><summary>**Instructions for using the VR Simulator**</summary>

 * Drag the `VRSimulatorCameraRig` prefab from the VRTK/Prefabs into the scene.
 * Select the GameObject with the `VRTK_SDKManager` script attached to it.
  * Select `Simulator` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the relevant Linked Objects.
 * Use the Left Alt to switch between mouse look and move a hand.
 * Press Tab to switch between left/right hands.
 * Hold Left Shift to change from translation to rotation for the hands.
 * Hold Left Ctrl to switch between X/Y and X/Z axis.
 * All above keys can be remapped using the inspector on the `VRSimulatorCameraRig` prefab.
 * Button mapping for the VR control are as follows:
  * Grip: Left mouse button
  * Trigger: Right mouse button
  * Touchpad Press: Q
  * Button One: E
  * Button Two: R

</details>
 
## SteamVR Unity Asset

<details><summary>**Instructions for using the SteamVR Unity3d asset**</summary>

 * Import the [SteamVR Plugin] from the Unity Asset Store.
 * Drag the `[CameraRig]` prefab from the SteamVR plugin into the scene.
 * Check that `Virtual Reality Supported` is ticked in the `Edit -> Project Settings -> Player` menu.
 * Ensure that `OpenVR` is added in the `Virtual Reality SDKs` list in the `Edit -> Project Settings -> Player` menu.
 * Select the GameObject with the `VRTK_SDKManager` script attached to it.
  * Select `Steam VR` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the relevant Linked Objects.
 * Optionally, browse the `Examples` scenes for example usage of the scripts.

</details>

## Oculus Utilities Unity Package

<details><summary>**Instructions for using the Oculus Utilities Unity3d package**</summary>

 * Download the [Oculus Utilities] from the Oculus developer website.
 * Import the `OculusUtilities.unitypackage` into the project.
 * Drag the `OVRCameraRig` prefab from the Oculus package into the scene.
 * Check that `Virtual Reality Supported` is ticked in the `Edit -> Project Settings -> Player` menu.
 * Ensure that `Oculus` is added in the `Virtual Reality SDKs` list in the `Edit -> Project Settings -> Player` menu.
 * Select the GameObject with the `VRTK_SDKManager` script attached to it.
  * Select `Oculus VR` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the relevant Linked Objects.

</details>

## XimmerseSDK for Unity *(experimental)*

<details><summary>**Instructions for using the Ximmerse Unity SDK**</summary>

 * Download the [Ximmerse Unity SDK] from the Ximmerse SDK Github page.
 * Import the `XIM01-v2.0.1.unitypackage` into the project.
 * Drag the `VRCameraRig` prefab from the Ximmerse Unity SDK into the scene.

  > * It is recommened to use "Floor Level" as the Tracking Origin Type, with `VRCameraRig` positon's set to `(0f,0f,0f)`.
  > * "Eye Level" can also be used as the Tracking Origin Type. However, the positons of `VRCameraRig` is recommended to set to `(0f,1.675f,0f)` in this case.
  > * Please make sure SimplePicker.cs is NOT attached on gameobject "cobra02-L" and "cobra02-R". SimplePicker script is provided by Ximmerse SDK, while having the script on the profab may break VRTK grab functionality.

 * Change platform to Android.
  > Currently Ximmerse 6DOF tracking is only supported on Android. 3DOF tracking is supported on both iOS and Android. We are getting MFI cert from Apple at the moment.
 * Check that `Virtual Reality Supported` is ticked in the `Edit -> Project Settings -> Player` menu.
 * Ensure that `Oculus` is added in the `Virtual Reality SDKs` list in the `Edit -> Project Settings -> Player` menu.
 * Make sure `VRTK_SDK_XIMMERSEVR` is defined in Scripting Define Symbols.
 * Select the GameObject with the `VRTK_SDKManager` script attached to it.
 * Select `Ximmerse VR` for each of the SDK Choices.
 * Config Linked Objects:
  * Actual Boundaries = VRCameraRig
  * Actual Headset = CenterEyeAnchor
  * Actual Left Controller = LeftHandAnchor
  * Actual Right Controller = RightHandAnchor
  * Model Alias Left Controller = _VisibleObject (child of LeftHandAnchor)
  * Model Right Left Controller = _VisibleObject (child of RightHandAnchor)

</details>

## Google VR SDK for Unity *(experimental)*

<details><summary>**Instructions for using the Google VR SDK for Unity**</summary>

 * Open a new or existing project in Unity (5.4.2f2-GVR13 or other version with Daydream integration).
 * Import asset package GoogleVRForUnity you downloaded from Google.
 * Build Settings:
  * Target platform: `Android`
 * Player settings:
  * Virtual Reality Supported > Daydream
  * API Level: `Nougat`
  * Bundle Identifier and other settings for use with Android.
 * In Hierarchy, create empty GameObject named `DaydreamCameraRig`.
  * Move or create a Camera as child of `DaydreamCameraRig`, reset its transform `position: 0,0,0`.
  * Add `GvrControllerPointer` prefab from `Assets/GoogleVR/Prefabs/UI`.
  * Add `GvrControllerMain` prefab from `Assets/GoogleVR/Prefabs/Controller/`.
  * Add `GvrViewerMain` prefab (enables view in editor play mode).
 * Disable Daydream's native pointer tools.
  * Camera object, disable or remove GvrPointerPhysicsRaycaster component (if present).
  * GvrControllerPointer/Laser, disable or delete.

### Setup VRTK Components
 * In Hierarchy, create an empty GameObject named `[VRTK]`.
 * Add component `VRTK_SDKManager`
 * Add a child empty GameObject named `RightController`.
  *  > Note: Daydream supports only one controller, LeftController will not be used. If present, can be disabled or deleted.
 * SDK Selection
  * In Inspector, choose Quick Select SDK: Daydream
  * In Player Settings, ensure Scripting Define Symbols: `VRTK_SDK_DAYDREAM`
 * Linked Objects:
  * Click `Auto Populate Linked Objects`, that should set:
    * Actual Boundaries: `DaydreamCameraRig`
    * Actual Headset: `DaydreamCameraRig/Camera`
    * Actual Left Controller: `empty`
    * Actual Right Controller: `DaydreamCameraRig/GvrControllerPointer/Controller`
 * Controler Aliases:
  * Model Alias Left Controller: `empty`
  * Model Alias Right Controller: `DaydreamCameraRig/GvrControllerPoints/Controller`
  * Script Alias Left Controller: `empty`
  * Script Alias Right Controller: `[VRTK]/RightController`

</details>

[SteamVR Plugin]: https://www.assetstore.unity3d.com/en/#!/content/32647
[SteamVR Plugin for Unity3d Github Repo]: https://github.com/ValveSoftware/openvr/tree/master/unity_package/Assets/SteamVR
[Oculus Utilities]: https://developer3.oculus.com/downloads/game-engines/1.10.0/Oculus_Utilities_for_Unity_5/
[Google VR SDK for Unity]: https://developers.google.com/vr/unity/download
[Ximmerse Unity SDK]: https://github.com/Ximmerse/SDK/tree/master/Unity
