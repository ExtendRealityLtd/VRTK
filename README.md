# SteamVR Unity Toolkit - [![Slack](https://vrtk-slack-invite.herokuapp.com/badge.svg)](https://vrtk-slack-invite.herokuapp.com) [![Subreddit](https://img.shields.io/badge/subreddit-discussions-red.svg?style=flat-square)](https://www.reddit.com/r/SteamVRUnityToolkit/) [![Trello](https://img.shields.io/badge/trello-work%20board-blue.svg?style=flat-square)](https://trello.com/b/sU0vRWUz/steamvr-unity-toolkit)

A collection of useful scripts and prefabs for building SteamVR titles
in Unity 5.

**This Toolkit requires the [SteamVR Plugin]
from the Unity Asset Store to be imported into your Unity project.**

  > _To all those lovely people who want to give donations, instead of_
  > _donating, consider supporting me by buying my latest game on_
  > _Steam - `Holodaze` for HTC Vive. At least this way, I make a bit of_
  > _money and you get something to play!_
  >
  > **[Buy Holodaze from the Steam Store](http://store.steampowered.com/app/475520)**
  
## Games, Apps and Experiences that use this Toolkit

 * Games
  * QuiVR | [Steam Store Page](http://store.steampowered.com/app/489380/)
  * Left-Hand Path | [Steam Store Page](http://store.steampowered.com/app/488760/)
  * Holodaze | [Steam Store Page](http://store.steampowered.com/app/475520/)
  * ViveSpray | [Steam Store Page](http://store.steampowered.com/app/494830/)
  * VeeR Pong | [Steam Store Page](http://store.steampowered.com/app/494850)
  * Emergence Fractal Universe | [Steam Store Page](http://store.steampowered.com/app/500470)
  * Ocarina of Vive | [Itch.io Store Page](https://tomcat94.itch.io/ocarina-of-vive-shooting-gallery)
  * Danc<R | [Itch.io Store Page](https://tomcat94.itch.io/dancr-alpha)
  * Tower Island: Explore, Discover and Disassemble | [Steam Store Page](http://store.steampowered.com/app/487740/)
  * Virtual Warfighter | [Game website](http://virtual-warfighter.com/)
  * VR Regatta | [Steam Store Page](http://store.steampowered.com/app/468240/)
  * Car Car Crash Hands | [Steam Store Page](http://store.steampowered.com/app/472720)
  * MegaPolice | [Youtube Trailer](https://www.youtube.com/watch?v=d6hCgfMxldY)
  * The Crystal Nebula | [Steam Store Page](http://store.steampowered.com/app/505660)
  * Drone Training VR | [Youtube Trailer](https://www.youtube.com/watch?v=A5MFT2JsySc)

## Quick Start

  * Clone this repository `git clone https://github.com/thestonefox/SteamVR_Unity_Toolkit.git`
  * Open the `SteamVR_Unity_Toolkit` within Unity3d
  * Import the [SteamVR Plugin] from the Unity Asset Store
  * Browse the `Examples` scenes for example usage of the scripts

## FAQ/Troubleshooting

  * How to create a new project using this toolkit along with the
  SteamVR Unity Plugin:
   * [View answer video on YouTube](https://www.youtube.com/watch?v=oFkgTZ4LXEo)
  * Pointer beams/teleporting no longer works after a project build
  and running from that build:
   * [View answer video on YouTube](https://www.youtube.com/watch?v=IsmYoLTmX4c)

## Summary

This toolkit provides many common VR functionality within Unity3d such
as (but not limited to):

  * Controller button events with common aliases
  * Controller world pointers (e.g. laser pointers)
  * Player teleportation
  * Grabbing/holding objects using the controllers
  * Interacting with objects using the controllers
  * Transforming game objects into interactive UI elements

The toolkit is heavily inspired and based upon the
[SteamVR Plugin for Unity3d Github Repo].

The reason this toolkit exists is because I found the SteamVR plugin
to contain confusing to use or broken code and I decided to build a
collection of scripts/assets that I would find useful when building for
VR within Unity3d.

## What's In The Box

This toolkit project is split into three main sections:

  * Prefabs - `SteamVR_Unity_Toolkit/Prefabs/`
  * Scripts - `SteamVR_Unity_Toolkit/Scripts/`
  * Examples - `SteamVR_Unity_Toolkit/Examples/`

The `SteamVR_Unity_Toolkit` directory is where all of the relevant
files are kept and this directory can be simply copied over to an
existing project. The `Examples` directory contains useful scenes
showing the `SteamVR_Unity_Toolkit` in action.

## Documentation

The documentation for the project can be found within this
repository in [DOCUMENTATION.md] which includes the up to date
documentation for this GitHub repository. Alternatively, the
stable versions of the documentation can be viewed online at
[https://steamvr-unity-toolkit.readme.io/](https://steamvr-unity-toolkit.readme.io/).

## Contributing

I would love to get contributions from you! Follow the instructions
below on how to make pull requests. For the full contribution
guidelines see the [Contribution Document].

## Pull requests

  1. [Fork] the project, clone your fork, and configure the remotes.
  2. Create a new topic branch (from `master`) to contain your feature,
  chore, or fix.
  3. Commit your changes in logical units.
  4. Make sure all the example scenes are still working.
  5. Push your topic branch up to your fork.
  6. [Open a Pull Request] with a clear title and description.

## License

Code released under the MIT license.

[SteamVR Plugin]: https://www.assetstore.unity3d.com/en/#!/content/32647
[SteamVR Plugin for Unity3d Github Repo]: https://github.com/ValveSoftware/openvr/tree/master/unity_package/Assets/SteamVR
[Catlike Coding]: http://catlikecoding.com/unity/tutorials/curves-and-splines/
[Contribution Document]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/CONTRIBUTING.md
[DOCUMENTATION.md]: https://github.com/thestonefox/SteamVR_Unity_Toolkit/blob/master/DOCUMENTATION.md
[Fork]: http://help.github.com/fork-a-repo/
[Open a Pull Request]: https://help.github.com/articles/using-pull-requests/
