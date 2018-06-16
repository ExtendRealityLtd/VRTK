![vrtk logo](https://user-images.githubusercontent.com/1029673/40060519-bb122e8c-584e-11e8-8402-ca168b327671.png)

> ### VRTK - Virtual Reality Toolkit
> A productive VR Toolkit for rapidly building VR solutions in Unity3d 2018.1 or above.

[![Slack](http://sysdia2.co.uk/badge.svg)](http://invite.vrtk.io)
[![Documentation](https://img.shields.io/badge/readme-docs-3484C6.svg)](http://docs.vrtk.io)
[![Twitter Follow](https://img.shields.io/twitter/follow/vr_toolkit.svg?style=flat&label=twitter)](https://twitter.com/VR_Toolkit)
[![YouTube](https://img.shields.io/badge/youtube-channel-e52d27.svg)](http://videos.vrtk.io)
[![Waffle](https://img.shields.io/badge/project-backlog-78bdf2.svg)](http://tracker.vrtk.io)

## Introduction

VRTK aims to make building VR solutions in Unity3d fast and easy for
beginners and seasoned developers alike.

## Getting Started

### Setting up the project

  * Create a new project in Unity3d 2018.1 or above using 3D Template
  * Ensure `Virtual Reality Supported` is checked
    * Click in Unity3d main menu `Edit -> Project Settings -> Player`
    * In the `PlayerSettings` inspector panel, expand `XR Settings`
    * Check the `Virtual Reality Supported` option
  * Update the project to the supported `Scripting Runtime Version`
    * Click in Unity3d main menu `Edit -> Project Settings -> Player`
    * In the `PlayerSettings` inspector panel, expand `Other Settings`
    * Change `Scripting Runtime Version` to `.NET 4.x Equivalent`
    * Unity will now restart in the supported scripting runtime

### Cloning the repo

  * Navigate to the project `Assets/` directory
  * Git clone with required submodules into the `Assets/` directory:
    * `git clone --recurse-submodules https://github.com/thestonefox/VRTK.git`
    * `git submodule init && git submodule update`

### Running the tests

  * Open the `VRTK/Scenes/Internal/TestRunner` scene
  * Click in the Unity3d main menu `Window -> Test Runner`
  * On the `EditMode` tab click `Run All`
  * If all the tests pass then installation was successful.

## Made With VRTK

[![image](https://cloud.githubusercontent.com/assets/1029673/21553226/210e291a-cdff-11e6-8639-91a3dddb1555.png)](http://store.steampowered.com/app/489380) [![image](https://cloud.githubusercontent.com/assets/1029673/21553234/2d105e4a-cdff-11e6-95a2-7dfdf7519e17.png)](http://store.steampowered.com/app/488760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553257/5c17bf30-cdff-11e6-98ab-a017bc5cd00d.png)](http://store.steampowered.com/app/494830) [![image](https://cloud.githubusercontent.com/assets/1029673/21553262/6d82afd2-cdff-11e6-8400-882989a6252c.png)](http://store.steampowered.com/app/391640) [![image](https://cloud.githubusercontent.com/assets/1029673/21553270/7b8808f2-cdff-11e6-9adb-1e20fe557ae0.png)](http://store.steampowered.com/app/525680) [![image](https://cloud.githubusercontent.com/assets/1029673/21553293/9eef3e32-cdff-11e6-8dc7-f4a3866ac386.png)](http://store.steampowered.com/app/550360) [![image](https://user-images.githubusercontent.com/1029673/27344044-dc29bb78-55dc-11e7-80b6-a1524cb3ca14.png)](http://store.steampowered.com/app/584850) [![image](https://cloud.githubusercontent.com/assets/1029673/21553649/53ded8d8-ce01-11e6-8314-d33a873db745.png)](http://store.steampowered.com/app/510410) [![image](https://cloud.githubusercontent.com/assets/1029673/21553655/63e21e0c-ce01-11e6-90b0-477b14af993f.png)](http://store.steampowered.com/app/499760) [![image](https://cloud.githubusercontent.com/assets/1029673/21553665/713938ce-ce01-11e6-84f3-40db254292f1.png)](http://store.steampowered.com/app/548560) [![image](https://cloud.githubusercontent.com/assets/1029673/21553680/908ae95c-ce01-11e6-989f-68c38160d528.png)](http://store.steampowered.com/app/511370) [![image](https://cloud.githubusercontent.com/assets/1029673/21553683/a0afb84e-ce01-11e6-9450-aaca567f7fc8.png)](http://store.steampowered.com/app/472720)

Many games and experiences have already been made with VRTK.

Check out the [MADE_WITH_VRTK.md] document to see the full list.

## Contributing

We're not currently in a place where accepting contributions would
be helpful. But as soon as we're ready we'll let you know!

## License

Code released under the [MIT License].

[MIT License]: LICENSE.md

Any Third Party Licenses can be viewed in [THIRD_PARTY_NOTICES.md].

[MIT License]: LICENSE.md
[THIRD_PARTY_NOTICES.md]: THIRD_PARTY_NOTICES.md
[MADE_WITH_VRTK.md]: Documentation/MADE_WITH_VRTK.md
