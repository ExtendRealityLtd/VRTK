[![VRTK logo][VRTK-Image]](#)

> ### VRTK Farm Yard Example - Virtual Reality Toolkit
> A Farm Yard example scene of how to use VRTK v4 for rapidly building spatial computing solutions in the Unity software.
> #### Requires the Unity software version 2020.3.24f1.

[![License][License-Badge]][License]
[![Backlog][Backlog-Badge]][Backlog]
[![Discord][Discord-Badge]][Discord]
[![Videos][Videos-Badge]][Videos]
[![Twitter][Twitter-Badge]][Twitter]

## Beta Disclaimer

This project was built using 2020.3.24f1 and should work as expected on that version. It is feasible to downgrade this project to a previous version of the Unity software but it may cause issues in doing so.

This project uses the newer Unity software XR management system and the newer Unity Input system.

This VRTK v4 Farm Yard example project has been updated to use the latest [Tilia] packages but is still in development and is missing a number of features from the previous release that used the deprecated [VRTK.Prefabs] package.

The current missing features are:

* Locomotion
  * Drag World
* Pointers
  * PlayArea Boundary Cursor

These features will be added in due course.

If you want to get started with the Tilia repos then check out the [Bowling Tutorial].

## Introduction

VRTK aims to make building spatial computing solutions in the [Unity] software fast and easy for beginners as well as experienced developers.

> You do not need to download anything else to get this Unity project running, simply open the downloaded Unity project in the Unity software as outlined by the Getting Started guide below.

## Getting Started

### Downloading the project

* Download this project repository to your local machine using *one* of the following methods:
  * Git clone the repository with `git clone https://github.com/ExtendRealityLtd/VRTK.git`
  * Download the zip file at `https://github.com/ExtendRealityLtd/VRTK/archive/master.zip` and extract it.

### Opening the downloaded project in the Unity software

> *Do not* drag and drop the VRTK project download into an existing Unity project. The VRTK repository download *is a Unity project* already and you should not nest a Unity project inside another Unity project. Follow the instructions below for opening the VRTK project within the Unity software.

#### Using the Unity Hub

* Open the [Unity Hub] panel.
* Click the `Add` Button:

![image](https://user-images.githubusercontent.com/1029673/68544837-112cb180-03bf-11ea-8118-acd2640cfe30.png)

* Browse to the local directory where the repository was downloaded to and click `Select Folder`:

![image](https://user-images.githubusercontent.com/1029673/68544843-1a1d8300-03bf-11ea-9b88-60f55eddf617.png)

* The VRTK project will now show up in the Unity Hub project window, so select it to open the VRTK project in the Unity software:

![image](https://user-images.githubusercontent.com/1029673/68544856-243f8180-03bf-11ea-8890-1be86159e7f6.png)

* The VRTK project will now open within the Unity software.

#### Opening from within the Unity software

* Select `Main Menu -> File -> Open Project` within the Unity software.
* Browse to the local directory where the repository was downloaded to and click `Select Folder`.
* The VRTK project will now open within the Unity software.

### Running the example scene

* Open the `Assets/Samples/Farm/Scenes/ExampleScene` scene.
* Enable `Maximize On Play` in the Unity Game view control bar to ensure no performance issues are caused by the Unity Editor overhead.
* Play the scene in the Unity Editor (`CTRL` + `P`).
* The scene should automatically play within any Unity supported XR hardware.
* Explore the farm yard and enjoy!

## Made With VRTK

[![image](https://cloud.githubusercontent.com/assets/1029673/21553226/210e291a-cdff-11e6-8639-91a3dddb1555.png)](http://store.steampowered.com/app/489380) [![image](https://cloud.githubusercontent.com/assets/1029673/21553234/2d105e4a-cdff-11e6-95a2-7dfdf7519e17.png)](http://store.steampowered.com/app/488760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553257/5c17bf30-cdff-11e6-98ab-a017bc5cd00d.png)](http://store.steampowered.com/app/494830) [![image](https://cloud.githubusercontent.com/assets/1029673/21553262/6d82afd2-cdff-11e6-8400-882989a6252c.png)](http://store.steampowered.com/app/391640) [![image](https://cloud.githubusercontent.com/assets/1029673/21553270/7b8808f2-cdff-11e6-9adb-1e20fe557ae0.png)](http://store.steampowered.com/app/525680) [![image](https://cloud.githubusercontent.com/assets/1029673/21553293/9eef3e32-cdff-11e6-8dc7-f4a3866ac386.png)](http://store.steampowered.com/app/550360) [![image](https://user-images.githubusercontent.com/1029673/27344044-dc29bb78-55dc-11e7-80b6-a1524cb3ca14.png)](http://store.steampowered.com/app/584850) [![image](https://cloud.githubusercontent.com/assets/1029673/21553649/53ded8d8-ce01-11e6-8314-d33a873db745.png)](http://store.steampowered.com/app/510410) [![image](https://cloud.githubusercontent.com/assets/1029673/21553655/63e21e0c-ce01-11e6-90b0-477b14af993f.png)](http://store.steampowered.com/app/499760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553665/713938ce-ce01-11e6-84f3-40db254292f1.png)](http://store.steampowered.com/app/548560) [![image](https://cloud.githubusercontent.com/assets/1029673/21553680/908ae95c-ce01-11e6-989f-68c38160d528.png)](http://store.steampowered.com/app/511370) [![image](https://cloud.githubusercontent.com/assets/1029673/21553683/a0afb84e-ce01-11e6-9450-aaca567f7fc8.png)](http://store.steampowered.com/app/472720)

Many games and experiences have already been made with VRTK.

Check out the [Made With VRTK] website to see the full list.

## Contributing

If you have a cool feature you'd like to show off within the Farm Yard that can be implemented with the base Tilia packages then feel free to raise a pull request with your contribution.

Please refer to the Extend Reality [Contributing guidelines] and the [project coding conventions].

## Third Party Pacakges

The VRTK v4 Farm Yard example project uses the following 3rd party package:

* [Quick Outline] by Chris Nolet.

## License

Code released under the [MIT License][License].

## Disclaimer

These materials are not sponsored by or affiliated with Unity Technologies or its affiliates. "Unity" is a trademark or registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.

[VRTK-Image]: https://raw.githubusercontent.com/ExtendRealityLtd/related-media/main/github/readme/vrtk.png
[Unity]: https://unity3d.com/
[Made With VRTK]: https://www.vrtk.io/madewith.html
[License]: LICENSE.md
[Tilia]: https://www.vrtk.io/tilia.html
[VRTK.Prefabs]: https://github.com/ExtendRealityLtd/VRTK.Prefabs
[Unity Hub]: https://docs.unity3d.com/Manual/GettingStartedUnityHub.html

[License-Badge]: https://img.shields.io/github/license/ExtendRealityLtd/VRTK.svg
[Backlog-Badge]: https://img.shields.io/badge/project-backlog-78bdf2.svg

[Discord-Badge]: https://img.shields.io/badge/discord--7289DA.svg?style=social&logo=discord
[Videos-Badge]: https://img.shields.io/badge/youtube--e52d27.svg?style=social&logo=youtube
[Twitter-Badge]: https://img.shields.io/badge/twitter--219eeb.svg?style=social&logo=twitter

[License]: LICENSE.md
[Backlog]: http://tracker.vrtk.io

[Discord]: https://discord.com/invite/bRNS6hr
[Videos]: http://videos.vrtk.io
[Twitter]: https://twitter.com/VR_Toolkit
[Bowling Tutorial]: https://github.com/ExtendRealityLtd/VRTK.Tutorials.VRBowling

[Quick Outline]: https://github.com/chrisnolet/QuickOutline

[Contributing guidelines]: https://github.com/ExtendRealityLtd/.github/blob/master/CONTRIBUTING.md
[project coding conventions]: https://github.com/ExtendRealityLtd/.github/blob/master/CONVENTIONS/UNITY3D.md
