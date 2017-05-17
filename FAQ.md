# Frequently Asked Questions

For questions that are not answered here, be sure to check out the [VRTK GitHub Issues](https://github.com/thestonefox/VRTK/issues) page and raise any bugs or feature requests.

Alternatively, if you have any general setup issues or other questions relating to VRTK then visit the official Slack channel at http://invite.vrtk.io to join a vibrant and helpful community.

## Troubleshooting

### How can I get my environment information?
  > To give more info when you need help make sure to use the `Window > VRTK > Support Info` window to copy and paste your info in here!

### I'm using Unity 5.6 and SteamVR but I cannot see my controllers in any of the example scenes.
  > This is a bug with SteamVR and Unity 5.6 so you will need to add the `SteamVR_UpdatePoses` script to the main camera (which is at `[CameraRig]->Camera (head)->Camera(eye)` for the Vive controllers to show up and track correctly. (Ref: https://steamcommunity.com/app/358720/discussions/0/133258092242841297/) - Also, this is automatically fixed by VRTK in the latest GitHub master version.

### I'm using VRTK version 3.0.0 and I've just upgraded my Unity SteamVR Plugin to version 1.2.0 and now I'm getting errors.
  > You will need to upgrade to version 3.0.1 to fix the issue caused by SteamVR 1.2.0.
  It is also required that to do the update, you delete the VRTK directory and re-add it in otherwise Unity may fail to update the files correctly if they are simply copied over and overwrite the old VRTK version 3.0.0 files.

### I've upgraded to VRTK version 3 and I'm getting errors using Unity 5.3.
  > VRTK version 3 does not support Unity 5.3 so you will need to update to Unity 5.4 or above. VRTK aims to only provide support for the recent and current version of Unity (e.g. recent is 5.4 and current is 5.5). VRTK also does not support Unity beta versions.

### I'm using Unity version xx or a beta version of Unity and VRTK is throwing errors.
  > VRTK aims to only provide support for the recent and current version of Unity (e.g. recent is 5.5 and current is 5.6). VRTK also does not support Unity beta versions. If you are on a supported version and still have an issue then raise a bug report on the [VRTK GitHub Issues](https://github.com/thestonefox/VRTK/issues) page.

### I've upgraded from VRTK version 2.2.1 to version 3 and many of the VRTK scripts are missing or have changed.
  > Watch [Road to Version 3.0](https://www.youtube.com/watch?v=tMz04CqAYjw)
  
### I've upgraded my version of VRTK and I'm getting errors about classes already existing and duplicate files.
  > When performing an upgrade, be sure to delete the previous `Assets/VRTK` directory before copying in the new version of the `Assets/VRTK` directory as Unity does not handle in place updates very well.

## Getting Started

### Should I be using the [Unity Asset Store version](https://www.assetstore.unity3d.com/en/#!/content/64131) or the [GitHub master version](https://github.com/thestonefox/VRTK) of VRTK?
  > The [GitHub master version](https://github.com/thestonefox/VRTK) of VRTK is always the most up to date version with more features and bug fixes, however it is not as stable as the [GitHub releases](https://github.com/thestonefox/VRTK/releases) or the [Unity Asset Store version](https://www.assetstore.unity3d.com/en/#!/content/64131). It is recommended that to keep up to date with the latest features, the [GitHub master version](https://github.com/thestonefox/VRTK) is used.

### How can I use VRTK with the Unity SteamVR Plugin?
  > Watch [Getting Started with SteamVR](https://www.youtube.com/watch?v=tyFV9oBReqg) or read the [GETTING_STARTED.md](https://github.com/thestonefox/VRTK/blob/master/GETTING_STARTED.md) guide.

### How can I use VRTK with the Oculus Utilities for Unity?
  > Watch [Getting Started with Oculus SDK](https://www.youtube.com/watch?v=psPVNddjgGw) or read the [GETTING_STARTED.md](https://github.com/thestonefox/VRTK/blob/master/GETTING_STARTED.md) guide.

### How can I implement Oculus Avatar Unity package with VRTK?
  > Watch [Getting Started with Oculus Avatar](https://www.youtube.com/watch?v=N7F0KqgNrAk)

### How do I upgrade from version 2.2.1 to version 3?
  > Watch [Road to Version 3.0](https://www.youtube.com/watch?v=tMz04CqAYjw)

### Where can I find the documentation for VRTK?
  > The documentation can be found online at http://docs.vrtk.io and are also included in the repository at [DOCUMENTATION.md](https://github.com/thestonefox/VRTK/blob/master/DOCUMENTATION.md) file and [Documentation PDF](https://github.com/thestonefox/VRTK/blob/master/Assets/VRTK/DOCUMENTATION.pdf) file.

### Are there any video tutorials to help get started?
  > Yes, visit the [Official VRTK YouTube Channel](http://videos.vrtk.io)

### Does VRTK provide `x` feature?
  > Check the [Example Scenes](https://github.com/thestonefox/VRTK/tree/master/Assets/VRTK/Examples) first and see if the feature you're after is demonstrated, if not then ask in [Slack](http://invite.vrtk.io) and finally raise a new [GitHub Issue](https://github.com/thestonefox/VRTK/issues).

## Development

### How do I automatically grab an object to a controller either when the scene starts or just on touch?
  > Watch [How to automatically grab objects](https://www.youtube.com/watch?v=YkeWIAonku0)

### Which grab mechanic should I be using?
  > Watch [Grab attach mechanics](https://www.youtube.com/watch?v=KPJBFpl2bPI)
  
### How can I remove the VRTK gizmo icons?
  > The VRTK icons in the Unity Editor can be disabled or resized via Unity's Gizmo menu in the top right corner of the Scene window. Alternatively you can also just delete the icons from your copy of VRTK.

## General

### Where can I get the VRTK logo to use in my game on a splash screen or elsewhere?
  > There are a number of graphic assets containing the VRTK logo and pig icon in the [Image Resources](https://github.com/thestonefox/VRTK/tree/master/Assets/VRTK/Examples/Resources/Images/logos) directory.

### Where can I see which features are being worked on and when they are likely to be released?
  > You can check out the project roadmap at http://tracker.vrtk.io which shows the current issues being worked on and what's also in the pipeline for the future.
