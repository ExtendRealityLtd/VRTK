# Getting Started

  > VRTK offers a VR Simulator that works without any third party SDK, but VR device support requires a supported VR SDK to be imported into the Unity project.

## Using the example project & scenes

 * Download or clone this repository.
 * Open the folder in Unity to load the project.
 * Have a look at the included example scenes.

The example scenes support all the VRTK supported VR SDKs. To make use of VR devices (besides the included VR Simulator) import the needed third party VR SDK into the project.

## Using VRTK in your own project

 * Download or clone this repository.
 * Import the `Assets/VRTK` folder into your Unity project.
 * Add the `VRTK_SDKManager` script to a GameObject in the scene.

The SDK Manager handles setting up everything to use the various supported SDKs via the relevant `VRTK_SDKSetup`. To use a VR SDK the following steps are needed:

 * Download and import the relevant SDK into the project.
 * Create a new empty game object.
   * Add `VRTK_SDKSetup` to it.
   * Add the VR SDK game objects (e.g. the Camera Rig) as children.
   * On the SDK Setup set the `SDK Selection` to the respective VR SDK.
   * Make sure all the `Object References` are set correctly by `Auto Populate` or set them manually.
 * On the SDK Manager under `Setups` add a new slot and select the SDK Setup for that slot.

Repeat the steps above to add additional SDK Setups to the SDK Manager.

If the `Auto Load` setting on the SDK Manager is enabled the SDK Setups are automatically loaded in the order they appear in the list:

 * The first Setup that is usable (i.e. initializable without errors and the HMD is connected) will be used.
 * If a Setup can't be used the next one will be tried instead.
 * If no Setup is usable VR support is disabled.

The SDK Manager allows switching the used SDK Setup at runtime. To add a simple overlay GUI to do so add the `VRTK/Prefabs/SDKSetupSwitcher` prefab to the scene.

## Supported SDKs

For further information on getting started with the supported SDKs, please refer to the specific documentation for the SDK listed below:

 * [Unity](/Assets/VRTK/Source/SDK/Unity/README.md)
 * [Simulator](/Assets/VRTK/Source/SDK/Simulator/README.md)
 * [SteamVR](/Assets/VRTK/Source/SDK/SteamVR/README.md)
 * [Oculus](/Assets/VRTK/Source/SDK/Oculus/README.md)
 * [Daydream](/Assets/VRTK/Source/SDK/Daydream/README.md)
 * [HyperealVR](/Assets/VRTK/Source/SDK/HyperealVR/README.md)
 * [Ximmerse](/Assets/VRTK/Source/SDK/Ximmerse/README.md)