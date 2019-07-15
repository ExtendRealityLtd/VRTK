[![VRTK logo][VRTK-Image]](#)

> ### VRTK v4 beta - Virtual Reality Toolkit
> A productive toolkit for rapidly building spatial computing solutions in the Unity software.

[![Slack][Slack-Badge]][Slack]
[![Documentation][Academy-Badge]][VRTK.Academy]
[![Videos][Videos-Badge]][Videos]
[![Twitter][Twitter-Badge]][Twitter]
[![License][License-Badge]][License]
[![Backlog][Backlog-Badge]][Backlog]

## Beta Disclaimer

VRTK v4 has not yet been released fully is still in the beta phase meaning there may still be undiscovered bugs, missing features and a lack of usable documentation. All of these things are being worked on but progress is slow as there is only one person actively working on VRTK v4 in their spare time so please be patient and respectful. You are free to use VRTK v4 in any way you wish, but if you feel you will struggle figuring stuff out for yourself without detailed documentation then it may be better off if you wait until the full release of VRTK v4. 

## Introduction

VRTK aims to make building spatial computing solutions in the [Unity] software fast and easy for beginners as well as experienced developers.

> **Requires** the Unity software version 2018.3.10f1 (or above).

## Getting Started

### Documentation

Visit the [VRTK.Academy] for a collection of educational content to help you get the most out of building spatial computing solutions with VRTK.

### Setting up the project

* Create a new project in the Unity software version 2018.3.10f1 (or above) using 3D Template or open an existing project.
* Ensure `Virtual Reality Supported` is checked:
  * In the Unity software select `Main Menu -> Edit -> Project Settings` to open the `Project Settings` window.
  * Select `Player` from the left hand menu in the `Project Settings` window.
  * In the `Player` settings panel expand `XR Settings`.
  * In `XR Settings` ensure the `Virtual Reality Supported` option is checked.
* Ensure the project `Scripting Runtime Version` is set to `.NET 4.x Equivalent`:
  * In the Unity software select `Main Menu -> Edit -> Project Settings` to open the `Project Settings` inspector.
  * Select `Player` from the left hand menu in the `Project Settings` window.
  * In the `Player` settings panel expand `Other Settings`.
  * Ensure the `Scripting Runtime Version` is set to `.NET 4.x Equivalent`.

> Note: Unity 2019.1 requires additional project setup before importing VRTK.

* Download and install the `XR Legacy Input Helpers` from the Unity Package Manager.
  * In the Unity software select `Main Menu -> Window -> Package Manager` to open the `Package Manager` window.
  * Select `XR Legacy Input Helpers` from the `Packages` tab in the `Package Manager` window.
  * Click the `Install` button located in the bottom right of the `Package Manager` window.
  * The `XR Legacy Input Helpers` package will now download and install into the project.

### Cloning the repo

* Navigate to the project `Assets/` directory.
* Git clone with required submodules into the `Assets/` directory:
  * `git clone --recurse-submodules https://github.com/ExtendRealityLtd/VRTK.git`
  * change to the newly cloned directory: `cd VRTK/`
  * `git submodule init && git submodule update`
* The Unity software will now import and compile the new files.

### Running the example scene

* Open the `VRTK/Samples/Farm/Scenes/ExampleScene` scene.
* Enable `Maximize On Play` in the Unity Game view control bar to ensure no performance issues are caused by the Unity Editor overhead.
* Play the scene in the Unity Editor (`CTRL` + `P`).
* The scene should automatically play within any Unity supported XR hardware.
* Explore the farm yard and enjoy!

## Made With VRTK

[![image](https://cloud.githubusercontent.com/assets/1029673/21553226/210e291a-cdff-11e6-8639-91a3dddb1555.png)](http://store.steampowered.com/app/489380) [![image](https://cloud.githubusercontent.com/assets/1029673/21553234/2d105e4a-cdff-11e6-95a2-7dfdf7519e17.png)](http://store.steampowered.com/app/488760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553257/5c17bf30-cdff-11e6-98ab-a017bc5cd00d.png)](http://store.steampowered.com/app/494830) [![image](https://cloud.githubusercontent.com/assets/1029673/21553262/6d82afd2-cdff-11e6-8400-882989a6252c.png)](http://store.steampowered.com/app/391640) [![image](https://cloud.githubusercontent.com/assets/1029673/21553270/7b8808f2-cdff-11e6-9adb-1e20fe557ae0.png)](http://store.steampowered.com/app/525680) [![image](https://cloud.githubusercontent.com/assets/1029673/21553293/9eef3e32-cdff-11e6-8dc7-f4a3866ac386.png)](http://store.steampowered.com/app/550360) [![image](https://user-images.githubusercontent.com/1029673/27344044-dc29bb78-55dc-11e7-80b6-a1524cb3ca14.png)](http://store.steampowered.com/app/584850) [![image](https://cloud.githubusercontent.com/assets/1029673/21553649/53ded8d8-ce01-11e6-8314-d33a873db745.png)](http://store.steampowered.com/app/510410) [![image](https://cloud.githubusercontent.com/assets/1029673/21553655/63e21e0c-ce01-11e6-90b0-477b14af993f.png)](http://store.steampowered.com/app/499760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553665/713938ce-ce01-11e6-84f3-40db254292f1.png)](http://store.steampowered.com/app/548560) [![image](https://cloud.githubusercontent.com/assets/1029673/21553680/908ae95c-ce01-11e6-989f-68c38160d528.png)](http://store.steampowered.com/app/511370) [![image](https://cloud.githubusercontent.com/assets/1029673/21553683/a0afb84e-ce01-11e6-9450-aaca567f7fc8.png)](http://store.steampowered.com/app/472720)

Many games and experiences have already been made with VRTK.

Check out the [Made With VRTK] website to see the full list.

## Contributing

We're not currently in a place where accepting contributions would be helpful. But as soon as we're ready we'll let you know!

## License

Code released under the [MIT License][License].

## Disclaimer

These materials are not sponsored by or affiliated with Unity Technologies or its affiliates. "Unity" is a trademark or registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.

[VRTK-Image]: https://user-images.githubusercontent.com/1029673/40060519-bb122e8c-584e-11e8-8402-ca168b327671.png
[Unity]: https://unity3d.com/
[Made With VRTK]: https://www.vrtk.io/madewith.html
[License]: LICENSE.md

[Slack-Badge]: https://img.shields.io/badge/slack-chat-E24663.svg
[Academy-Badge]: https://img.shields.io/badge/vrtk-academy-3484C6.svg
[Videos-Badge]: https://img.shields.io/badge/youtube-channel-e52d27.svg
[Twitter-Badge]: https://img.shields.io/twitter/follow/vr_toolkit.svg?style=flat&label=twitter
[Backlog-Badge]: https://img.shields.io/badge/project-backlog-78bdf2.svg
[License-Badge]: https://img.shields.io/github/license/ExtendRealityLtd/VRTK.svg

[Slack]: http://invite.vrtk.io
[VRTK.Academy]: https://academy.vrtk.io
[Videos]: http://videos.vrtk.io
[Twitter]: https://twitter.com/VR_Toolkit
[Backlog]: http://tracker.vrtk.io
