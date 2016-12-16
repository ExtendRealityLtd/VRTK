![vrtk logo](https://raw.githubusercontent.com/thestonefox/VRTK/master/Assets/VRTK/Examples/Resources/Images/logos/vrtk-capsule-clear.png)
> ### VRTK - Virtual Reality Toolkit
> A productive VR Toolkit for rapidly building VR solutions in Unity3d.

[![Slack](http://sysdia2.co.uk/badge.svg)](http://invite.vrtk.io)
[![Waffle](https://img.shields.io/badge/waffle-tracker-blue.svg)](http://tracker.vrtk.io)

<<<<<<< d7e2cab01bb33a336bebd43c055ea4658670bcb5
| Supported SDK | Download Link |
|---------------|---------------|
| SteamVR Unity Asset | [SteamVR Plugin] |
| Oculus Utilities Unity Package | [Oculus Utilities] |
=======
**This Toolkit requires a compatible VR SDK to be imported into your Unity Project**

  * Compatible SDKs
   * [SteamVR Plugin]
   * [Simulator]

## Quick Start for SteamVR

  * Clone this repository `git clone https://github.com/thestonefox/VRTK.git`.
  * Open `VRTK` within Unity3d.
  * Import the [SteamVR Plugin] from the Unity Asset Store.
  * Drag the `[CameraRig]` prefab from the SteamVR plugin into the
  scene.
  * Add the `VRTK_SDKManager` script a GameObject in the scene.
   * Select `Steam VR` for each of the SDK Choices.
   * Drag the `[CameraRig]` GameObject to the `Actual Boundaries`
   parameter in the `VRTK_SDKManager`.
   * Drag the `[CameraRig] -> Camera (head) -> Camera (eye)` GameObject
   to the `Actual Headset` parameter in the `VRTK_SDKManager`.
   * Drag the `[CameraRig] -> Controller (left)` GameObject to the
   `Actual Left Controller` parameter in the `VRTK_SDKManager`.
   * Drag the `[CameraRig] -> Controller (right)` GameObject to the
   `Actual Right Controller` parameter in the `VRTK_SDKManager`.
  * Optionally, browse the `Examples` scenes for example usage of the
  scripts.

## Quick Start for Simulator
  Simulator SDK is as the name suggests a simulator to be able to use VRTK without
  the need of a VR controller and/or headset. It uses the mouse and some modifier keys
  to move and rotate the "hands" around in 3d space to enable the user to grab and use
  objects the same way as if they had a controller. 

  * Clone this repository `git clone https://github.com/thestonefox/VRTK.git`.
  * Open `VRTK` within Unity3d.
  * Drag the `VRTK_SimPlayer` prefab from the VRTK/Prefabs into the
  scene.
  * Add the `VRTK_SDKManager` script a GameObject in the scene.
   * Select `Simulator` for each of the SDK Choices.
   * Drag the `VRTK_SimPlayer` GameObject to the `Actual Boundaries`
   parameter in the `VRTK_SDKManager`.
   * Drag the `VRTK_SimPlayer -> Camera` GameObject
   to the `Actual Headset` parameter in the `VRTK_SDKManager`.
   * Drag the `VRTK_SimPlayer -> LeftHand` GameObject to the
   `Actual Left Controller` parameter in the `VRTK_SDKManager`.
   * Drag the `VRTK_SimPlayer -> RightHand` GameObject to the
   `Actual Right Controller` parameter in the `VRTK_SDKManager`.
  * Go to the `VRTK_ControllerEvents` scripts and set `Use Toggle Button` to `Trigger_Press`
  * Button mapping are as follows:
   * Grip: Left mouse button
   * Trigger: Right mouse button
   * Touchpad Press: Q
   * Application Menu: E

## Summary

This toolkit provides many common VR functionality within Unity3d such
as (but not limited to):

  * Controller button events with common aliases
  * Controller world pointers (e.g. laser pointers)
  * Player Locomotion
  * Grabbing/holding objects using the controllers
  * Interacting with objects using the controllers
  * Transforming game objects into interactive UI elements

The toolkit is heavily inspired by the [SteamVR Plugin for Unity3d Github Repo].

## What's In The Box

This toolkit project is split into three main sections:

  * Prefabs - `VRTK/Prefabs/`
  * Scripts - `VRTK/Scripts/`
  * Examples - `VRTK/Examples/`
  * SDK - `VRTK/SDK`

The `VRTK` directory is where all of the relevant files are kept
and this directory can be simply copied over to an existing project.
The `Examples` directory contains useful scenes showing the VR Toolkit
in action.
>>>>>>> feat(SDK): Add Simulator

## Documentation

The documentation for the project can be found within this
repository in [DOCUMENTATION.md] which includes the up to date
documentation for this GitHub repository.

Alternatively, the stable versions of the documentation can be viewed
online at [http://docs.vrtk.io](http://docs.vrtk.io).

## Getting Started

> *VRTK requires a supported VR SDK to be imported into your Unity3d Project.*

 * Clone this repository `git clone https://github.com/thestonefox/VRTK.git`.
 * Open `VRTK` within Unity3d.
 * Add the `VRTK_SDKManager` script to a GameObject in the scene.

<details><summary>**Instructions for using the SteamVR Unity3d asset**</summary>

 * Import the [SteamVR Plugin] from the Unity Asset Store.
 * Drag the `[CameraRig]` prefab from the SteamVR plugin into the
 scene.
 * Check that `Virtual Reality Supported` is ticked in the
 `Edit -> Project Settings -> Player` menu.
 * Ensure that `OpenVR` is added in the `Virtual Reality SDKs` list
 in the `Edit -> Project Settings -> Player` menu.
 * Select the GameObject with the `VRTK_SDKManager` script attached
 to it.
  * Select `Steam VR` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the
  relevant Linked Objects.
 * Optionally, browse the `Examples` scenes for example usage of the
 scripts.

</details>

<details><summary>**Instructions for using the Oculus Utilities Unity3d package**</summary>

 * Download the [Oculus Utilities] from the Oculus developer website.
 * Import the `OculusUtilities.unitypackage` into the project.
 * Drag the `OVRCameraRig` prefab from the Oculus package into the
 scene.
 * Check that `Virtual Reality Supported` is ticked in the
 `Edit -> Project Settings -> Player` menu.
 * Ensure that `Oculus` is added in the `Virtual Reality SDKs` list
 in the `Edit -> Project Settings -> Player` menu.
 * Select the GameObject with the `VRTK_SDKManager` script attached
 to it.
  * Select `Oculus VR` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the
  relevant Linked Objects.

</details>
   
## What's In The Box

VRTK is a collection of useful scripts and concepts to aid building VR
solutions rapidly and easily in Unity3d 5+.

It covers a number of common solutions such as:

 * Locomotion within virtual space.
 * Interactions like touching, grabbing and using objects
 * Interacting with Unity3d UI elements through pointers or touch.
 * Body physics within virtual space.
 * 2D and 3D controls like buttons, levers, doors, drawers, etc.
 * And much more...

## Examples

A collection of example scenes have been created to aid with
understanding the different aspects of VRTK.

A list of the examples can be viewed in [EXAMPLES.md] which includes
an up to date list of examples showcasing the features of VRTK.

The examples have all been built to work with the [SteamVR Plugin] by
default, but they can be converted over to using the [Oculus Utilities]
package by following the instructions for using the Oculus Utilities
package above.

> *If the examples are not working on first load, click the `[VRTK]`
> GameObject in the scene hierarchy to ensure the SDK Manager editor
> script successfully sets up the project and scene.*

## Made With VRTK

[![image](https://cloud.githubusercontent.com/assets/1029673/21553226/210e291a-cdff-11e6-8639-91a3dddb1555.png)](http://store.steampowered.com/app/489380) [![image](https://cloud.githubusercontent.com/assets/1029673/21553234/2d105e4a-cdff-11e6-95a2-7dfdf7519e17.png)](http://store.steampowered.com/app/488760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553257/5c17bf30-cdff-11e6-98ab-a017bc5cd00d.png)](http://store.steampowered.com/app/494830) [![image](https://cloud.githubusercontent.com/assets/1029673/21553262/6d82afd2-cdff-11e6-8400-882989a6252c.png)](http://store.steampowered.com/app/391640) [![image](https://cloud.githubusercontent.com/assets/1029673/21553270/7b8808f2-cdff-11e6-9adb-1e20fe557ae0.png)](http://store.steampowered.com/app/525680) [![image](https://cloud.githubusercontent.com/assets/1029673/21553293/9eef3e32-cdff-11e6-8dc7-f4a3866ac386.png)](http://store.steampowered.com/app/550360) [![image](https://cloud.githubusercontent.com/assets/1029673/21553635/3acbed36-ce01-11e6-80cd-4fe8d28d6b38.png)](http://store.steampowered.com/app/475520) [![image](https://cloud.githubusercontent.com/assets/1029673/21553649/53ded8d8-ce01-11e6-8314-d33a873db745.png)](http://store.steampowered.com/app/510410) [![image](https://cloud.githubusercontent.com/assets/1029673/21553655/63e21e0c-ce01-11e6-90b0-477b14af993f.png)](http://store.steampowered.com/app/499760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553665/713938ce-ce01-11e6-84f3-40db254292f1.png)](http://store.steampowered.com/app/548560) [![image](https://cloud.githubusercontent.com/assets/1029673/21553680/908ae95c-ce01-11e6-989f-68c38160d528.png)](http://store.steampowered.com/app/511370) [![image](https://cloud.githubusercontent.com/assets/1029673/21553683/a0afb84e-ce01-11e6-9450-aaca567f7fc8.png)](http://store.steampowered.com/app/472720)

Many games and experiences have already been made with VRTK.

Check out the [Made With VRTK Document] to see the full list.

## Contributing

I would love to get contributions from you! Follow the instructions
below on how to make pull requests.

For the full contribution guidelines see the [Contribution Document].

## Pull requests

 1. [Fork] the project, clone your fork, and configure the remotes.
 2. Create a new topic branch (from `master`) to contain your feature,
 chore, or fix.
 3. Commit your changes in logical units.
 4. Make sure all the example scenes are still working.
 5. Push your topic branch up to your fork.
 6. [Open a Pull Request] with a clear title and description.

## License

Code released under the [MIT License].

[SteamVR Plugin]: https://www.assetstore.unity3d.com/en/#!/content/32647
[SteamVR Plugin for Unity3d Github Repo]: https://github.com/ValveSoftware/openvr/tree/master/unity_package/Assets/SteamVR
[Oculus Utilities]: https://developer3.oculus.com/downloads/game-engines/1.10.0/Oculus_Utilities_for_Unity_5/
[MIT License]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/LICENSE
[Contribution Document]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/CONTRIBUTING.md
[Made With VRTK Document]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/MADEWITHVRTK.md
[DOCUMENTATION.md]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/DOCUMENTATION.md
[EXAMPLES.md]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/EXAMPLES.md
[Fork]: http://help.github.com/fork-a-repo/
[Open a Pull Request]: https://help.github.com/articles/using-pull-requests/
