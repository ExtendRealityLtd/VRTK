# SteamVR Unity Toolkit

A collection of useful scripts and prefabs for building SteamVR titles
in Unity 5

  > #### Note:
  > This is very early alpha and does not offer much functionality at present.
  > I'm very open to suggestions, ideas and bug finding/fixing

## Quick Start

  * Clone this repository `git clone https://github.com/thestonefox/SteamVR_Unity_Toolkit.git`
  * Open the `SteamVR_Unity_Toolkit` within Unity3d
  * Browse the `Resources/Examples` scenes for example usage of the scripts

## Summary

This toolkit provides many common VR functionality within Unity3d such
as (but not limited to):

  * Controller button events
  * Controller world pointers (e.g. laser pointers)
  * Player teleportation
  * Grabbing/holding objects using the controllers
  * Interacting with objects using the controllers

The toolkit is heavily inspired and based upon the
[SteamVR plugin for Unity3d v1.0.8](https://github.com/ValveSoftware/openvr/tree/master/unity_package/Assets/SteamVR).

The reason this toolkit exists is because I found the SteamVR plugin
to contain confusing to use or broken code and I decided to build a
collection of scripts/assets that I would find useful when building for
VR within Unity3d.

## What's In The Box

This toolkit is split into three main sections:

  * Prefabs - `Resources/Prefabs`
  * Scripts - `Resources/Scripts`
  * Examples - `Resources/Examples`

### Prefabs

At present there is only one Prefab included which is the `[CameraRig]`
and it has been taken directly from the SteamVR Unity plugin example:
`Extras/SteamVR_TestThrow` scene as it includes the relevant `Model`
children on the controller (which seem to be missing from the default
prefab in the SteamVR plugin `Prefabs/[CameraRig].prefab`.

The `Resources/Prefabs/[CameraRig]` can be dropped into any scene to
provide instant access to a VR game camera via the VR headset and
tracking of the VR controllers including model representations.

### Scripts

This directory contains all of the toolkit scripts that add VR
functionality to Unity.

The current available scripts are:

#### Controller Events (SteamVR_ControllerEvents)

The controller events script is attached to a Controller object within
the `[CameraRig]` prefab and provides event listeners for every button
press on the controller (excluding the System Menu button as this
cannot be overriden and is always used by Steam).

When a controller button is pressed, the script emits an event to
denote that the button has been pressed which allows other scripts
to listen for this event without needing to implement any controller
logic.

The script also has a public boolean pressed state for the buttons to
allow the script to be queried by other scripts to check if a button is
being held down.

When a controller button is released, the script also emits an event
denoting that the button has been released.

The controller touchpad has two states, it can either be `touched`
where the user simply presses their finger on the pressure sensitive
pad or it can be `clicked` where the user presses down on the pad until
it makes a clicking sound.

The Controller Events script deals with both touchpad touch and click
events separately.

When a controller event is emitted, it is sent with a payload containing:

  * ControllerIndex - The index of the controller that was used
  * TouchpadAxis - A Vector2 of the position the touchpad is touched at

An example of the `SteamVR_ControllerEvents` script can be viewed in
the scene `Resources/Examples/002_Controller_Events` and code examples
of how the events are utilised and listened to can be viewed in the
script `Resources/Examples/Scripts/SteamVR_ControllerEvents_ListenerExample.cs`

#### Simple Laser Pointer (SteamVR_SimplePointer)

The simple pointer emits a coloured beam from the end of the controller
to simulate a laser beam. It can be useful for pointing to objects
within a scene and it can also determine the object it is pointing at
and the distance the object is from the controller the beam is being
emitted from.

The laser beam is activated by pressing the `Grip` on the controller.
This can be changed by updating the script to listen to a different
controller button event.

The Simple Pointer script is attached to a Controller object within the
`[CameraRig]` prefab and the Controller object also requires the
`SteamVR_ControllerEvents` script to be attached as it uses this for
listening to the controller button events for enabling and disabling
the beam.

The colour of the beam can be determined by a setting on the script
and is independent for each controller meaning different controllers
can have different coloured beams.

The thickness and length of the beam can also be set on the script as
well as the ability to toggle the sphere beam tip that is displayed
at the end of the beam (to represent a cursor).

The facing axis can also be set to match the direction the
`[CameraRig`] Prefab is facing as if it is rotated then the beam will
emit out of the controller at the wrong angle, so this setting can be
adjusted to ensure the beam always projects forward.

The Simple Pointer object extends the `SteamVR_WorldPointer` abstract
class and therefore emits the same events and payload.

An example of the `SteamVR_SimplePointer` script can be viewed in
the scene `Resources/Examples/003_Controller_SimplePointer` and
code examples of how the events are utilised and listened to can be
viewed in the script
`Resources/Examples/Scripts/SteamVR_ControllerPointerEvents_ListenerExample.cs`

#### Class Interfaces

To allow for reusablity and object consistency, a collection of
abstract classes are provided as interfaces which can be used to extend
onto a concrete class providing consistent functionality across many
different scripts without needing to duplicate code.

The current abstract class interfaces are available:

##### SteamVR_WorldPointer

This abstract class provides any game pointer the ability to know the
the state of the implemented pointer and emit an event to other scripts
in the game world.

The WorldPointer class emits three events:

  * WorldPointerIn: an event when the pointer collides with another
  game object.
  * WorldPointerOut: an event when the pointer stops colliding with
  the game object.
  * WorldPointerDestinationSet: an event when the pointer is no longer
  active in the scene to determine the last destination position of
  the pointer end (useful for selecting and teleporting).

A payload is emitted with the event containing:

  * ControllerIndex - The index of the controller emitting the beam
  * Distance - The distance the target is from the controller
  * Target - The game object that the pointer is colliding with
  * TipPosition - The world position of the end of the pointer

### Examples

This directory contains Unity3d scenes that demonstrate the scripts
and prefabs being used in the game world to create desired
functionality.

There is also a `/Scripts` directory within the `/Examples` directory
that contains helper scripts utilised by the example scenes to
highlight certain functionality (such as event listeners). These
example scripts are not required for real world usage.

The current examples are:

  * 001_CameraRig_VR_PlayArea: a simple scene showing the `[CameraRig]`
  prefab usage
  * 002_Controller_Events: a simple scene displaying the events from
  the controller in the console window
  * 003_Controller_SimplePointer: a scene with basic objects that can
  be pointed at with the laser beam from the controller activated by
  the `Grip` button. The pointer events are also displayed in the
  console window.


## Contributing

I'd love this to be a community effort, but as I'm just getting
started on this, it may be best to leave pull requests for now on
new features until I get my head around how this is going to
progress. I'm happy for bug fix pull requests though :)

Also, if you find any issues or have any suggestions then please
raise an issue on GitHub and I'll take a look when I can.

## License

Code released under the MIT license.
