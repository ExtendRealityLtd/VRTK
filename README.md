![vrtk logo](https://user-images.githubusercontent.com/1029673/29922542-f8ccb81c-8e4d-11e7-8fa3-b8607c550d77.png)
> ### VRTK - Virtual Reality Toolkit
> A productive VR Toolkit for rapidly building VR solutions in Unity3d.

[![Slack](https://img.shields.io/badge/slack-chat-E24663.svg)](http://invite.vrtk.io)
[![Documentation](https://img.shields.io/badge/readme-docs-3484C6.svg)](http://docs.vrtk.io)
[![YouTube](https://img.shields.io/badge/youtube-channel-e52d27.svg)](http://videos.vrtk.io)
[![Twitter Follow](https://img.shields.io/twitter/follow/vr_toolkit.svg?style=flat&label=twitter)](https://twitter.com/VR_Toolkit)
[![Waffle](https://img.shields.io/badge/project-backlog-78bdf2.svg)](http://tracker.vrtk.io)

| Supported SDK | Download Link |
|---------------|---------------|
| UnityEngine.VR | _Core Unity3d library_ |
| VR Simulator | _Included_ |
| SteamVR 1.2.3 | [SteamVR Plugin] |
| Oculus | [Oculus Integration] |
| Windows Mixed Reality | [Windows Mixed Reality For Unity] |
| * Ximmerse | [Ximmerse Unity SDK] |
| * Daydream | [Google VR SDK for Unity]
| * HyperealVR | [Hypereal VR Unity Plugin]

_* unsupported/experimental_

> *NOTE:* SteamVR 2 is not supported.

## Documentation

The API documentation for the project can be found within this
repository in [API.md] which includes the up to date documentation
for this GitHub repository.

Alternatively, the stable versions of the documentation can be viewed
online at [http://docs.vrtk.io](http://docs.vrtk.io).

## Frequently Asked Questions

If you have an issue or question then check the [FAQ.md] document to
see if your query has already been answered.

## Getting Started

  > *VRTK offers a VR Simulator that works without any third party SDK,
  >  but VR device support requires a supported VR SDK to be imported
  >  into the Unity project.*

 * Download or clone this repository.
 * Open the folder in Unity to load the project.
 * Have a look at the included example scenes.

The example scenes support all the VRTK supported VR SDKs. To make use
of VR devices (besides the included VR Simulator) import the needed
third party VR SDK into the project.

For further information about setting up a specific SDK and using VRTK
in your own project, check out the [GETTING_STARTED.md] document.

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

A list of the examples can be viewed in [Examples/README.md] which includes
an up to date list of examples showcasing the features of VRTK.

The example scenes support all the VRTK supported VR SDKs. To make use
of VR devices (besides the included VR Simulator) import the needed
third party VR SDK into the project.

## Made With VRTK

[![image](https://cloud.githubusercontent.com/assets/1029673/21553226/210e291a-cdff-11e6-8639-91a3dddb1555.png)](http://store.steampowered.com/app/489380) [![image](https://cloud.githubusercontent.com/assets/1029673/21553234/2d105e4a-cdff-11e6-95a2-7dfdf7519e17.png)](http://store.steampowered.com/app/488760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553257/5c17bf30-cdff-11e6-98ab-a017bc5cd00d.png)](http://store.steampowered.com/app/494830) [![image](https://cloud.githubusercontent.com/assets/1029673/21553262/6d82afd2-cdff-11e6-8400-882989a6252c.png)](http://store.steampowered.com/app/391640) [![image](https://cloud.githubusercontent.com/assets/1029673/21553270/7b8808f2-cdff-11e6-9adb-1e20fe557ae0.png)](http://store.steampowered.com/app/525680) [![image](https://cloud.githubusercontent.com/assets/1029673/21553293/9eef3e32-cdff-11e6-8dc7-f4a3866ac386.png)](http://store.steampowered.com/app/550360) [![image](https://user-images.githubusercontent.com/1029673/27344044-dc29bb78-55dc-11e7-80b6-a1524cb3ca14.png)](http://store.steampowered.com/app/584850) [![image](https://cloud.githubusercontent.com/assets/1029673/21553649/53ded8d8-ce01-11e6-8314-d33a873db745.png)](http://store.steampowered.com/app/510410) [![image](https://cloud.githubusercontent.com/assets/1029673/21553655/63e21e0c-ce01-11e6-90b0-477b14af993f.png)](http://store.steampowered.com/app/499760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553665/713938ce-ce01-11e6-84f3-40db254292f1.png)](http://store.steampowered.com/app/548560) [![image](https://cloud.githubusercontent.com/assets/1029673/21553680/908ae95c-ce01-11e6-989f-68c38160d528.png)](http://store.steampowered.com/app/511370) [![image](https://cloud.githubusercontent.com/assets/1029673/21553683/a0afb84e-ce01-11e6-9450-aaca567f7fc8.png)](http://store.steampowered.com/app/472720)

Many games and experiences have already been made with VRTK.

Check out the [MADE_WITH_VRTK.md] document to see the full list.

## Contributing

I would love to get contributions from you! Follow the instructions
below on how to make pull requests.

For the full contribution guidelines see the [CONTRIBUTING.md] document.

## Pull requests

 1. [Fork] the project, clone your fork, and configure the remotes.
    1. If you're submitting a bug fix or refactor pull request then
    target the repository `master` branch.
    2. If you're submitting a new feature or enhancement that changes
    functionality then target the next release branch in the
    repository (which is currently `3.3.0-alpha`).
 3. Commit your changes in logical units.
 4. Make sure all the example scenes are still working.
 5. Push your topic branch up to your fork.
 6. [Open a Pull Request] with a clear title and description.

## License

Code released under the [MIT License].

Any Third Party Licenses can be viewed in [THIRD_PARTY_NOTICES.md].

[SteamVR Plugin]: https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/1.2.3/SteamVR.Plugin.unitypackage
[SteamVR Plugin for Unity3d Github Repo]: https://github.com/ValveSoftware/openvr/tree/master/unity_package/Assets/SteamVR
[Oculus Integration]: https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022
[Ximmerse Unity SDK]: https://github.com/Ximmerse/SDK/tree/master/Unity
[Google VR SDK for Unity]: https://developers.google.com/vr/unity/download
[Hypereal VR Unity Plugin]: https://www.hypereal.com/#/developer/resource/download
[Windows Mixed Reality For Unity]: https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools

[MIT License]: LICENSE.md
[THIRD_PARTY_NOTICES.md]: THIRD_PARTY_NOTICES.md
[CONTRIBUTING.md]: /.github/CONTRIBUTING.md

[FAQ.md]: /Assets/VRTK/Documentation/FAQ.md
[MADE_WITH_VRTK.md]: /Assets/VRTK/Documentation/MADE_WITH_VRTK.md
[API.md]: /Assets/VRTK/Documentation/API.md
[GETTING_STARTED.md]: /Assets/VRTK/Documentation/GETTING_STARTED.md
[THANK_YOU_CREDITS.md]: /Assets/VRTK/Documentation/THANK_YOU_CREDITS.md
[Examples/README.md]: /Assets/VRTK/Examples/README.md

[Fork]: http://help.github.com/fork-a-repo/
[Open a Pull Request]: https://help.github.com/articles/using-pull-requests/
