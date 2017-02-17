![vrtk logo](https://raw.githubusercontent.com/thestonefox/VRTK/master/Assets/VRTK/Examples/Resources/Images/logos/vrtk-capsule-clear.png)
> ### VRTK - Virtual Reality Toolkit
> A productive VR Toolkit for rapidly building VR solutions in Unity3d.

[![Slack](http://sysdia2.co.uk/badge.svg)](http://invite.vrtk.io)
[![Twitter Follow](https://img.shields.io/twitter/follow/vr_toolkit.svg?style=flat&label=twitter)](https://twitter.com/VR_Toolkit)
[![YouTube](https://img.shields.io/badge/youtube-channel-e52d27.svg)](http://videos.vrtk.io)
[![Waffle](https://img.shields.io/badge/project-roadmap-blue.svg)](http://tracker.vrtk.io)

[![patreon_logo](https://cloud.githubusercontent.com/assets/1029673/23074410/8c248822-f530-11e6-9156-aeef1262be86.png)](https://www.patreon.com/vrtk)

| Supported SDK | Download Link |
|---------------|---------------|
| VR Simulator | Included |
| SteamVR Unity Asset | [SteamVR Plugin] |
| Oculus Utilities Unity Package | [Oculus Utilities] |

## Documentation

The documentation for the project can be found within this
repository in [DOCUMENTATION.md] which includes the up to date
documentation for this GitHub repository.

Alternatively, the stable versions of the documentation can be viewed
online at [http://docs.vrtk.io](http://docs.vrtk.io).

## Frequently Asked Questions

If you have an issue or question then check the [FAQ] document to see
if your query has already been answered.

## Getting Started

> *VRTK requires a supported VR SDK to be imported into your Unity3d Project.*

 * Clone this repository `git clone https://github.com/thestonefox/VRTK.git`.
 * Open `VRTK` within Unity3d.
 * Add the `VRTK_SDKManager` script to a GameObject in the scene.

<details><summary>**Instructions for using the VR Simulator**</summary>

 * Drag the `VRSimulatorCameraRig` prefab from the VRTK/Prefabs into the
 scene.
 * Select the GameObject with the `VRTK_SDKManager` script attached
 to it.
  * Select `Simulator` for each of the SDK Choices.
  * Click the `Auto Populate Linked Objects` button to find the
  relevant Linked Objects.
 * Use the Left Alt to switch between mouse look and move a hand.
 * Press Tab to switch between left/right hands.
 * Hold Left Shift to change from translation to rotation for the hands.
 * Hold Left Crtl to switch between X/Y and X/Z axis.
 * All above keys can be remapped using the inspector on the
 `VRSimulatorCameraRig` prefab.
 * Button mapping for the VR control are as follows:
  * Grip: Left mouse button
  * Trigger: Right mouse button
  * Touchpad Press: Q
  * Button One: E
  * Button Two: R

</details>
 
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
[FAQ]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/FAQ.md
