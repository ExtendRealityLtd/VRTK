# SteamVR Unity Toolkit

A collection of useful scripts and prefabs for building SteamVR titles
in Unity 5

  > #### Note:
  > This is very early alpha and does not offer much functionality at
  > present. I'm open to suggestions, ideas and bug finding/fixing.
  > Also, expect builds to break older versions as things are changing
  > fast at this stage, it will settle down when the project reaches
  > a beta stage.

## Quick Start

  * Clone this repository `git clone https://github.com/thestonefox/SteamVR_Unity_Toolkit.git`
  * Open the `SteamVR_Unity_Toolkit` within Unity3d
  * Browse the `Examples` scenes for example usage of the scripts

## Summary

This toolkit provides many common VR functionality within Unity3d such
as (but not limited to):

  * Controller button events with common aliases
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

This toolkit project is split into two main sections:

  * SteamVR_Unity_Toolkit - `SteamVR_Unity_Toolkit/`
    * Prefabs - `SteamVR_Unity_Toolkit/Prefabs/`
    * Scripts - `SteamVR_Unity_Toolkit/Scripts/`
    * Required Includes - `SteamVR_Unity_Toolkit/Required Includes/`
  * Examples - `Examples/`

The `SteamVR_Unity_Toolkit` directory is where all of the relevant
files are kept and this directory can be simply copied over to an
existing project. The `Examples` directory contains useful scenes
showing the `SteamVR_Unity_Toolkit` in action.

### Prefabs

At present there is only one Prefab included which is the `[CameraRig]`
and it has been taken directly from the SteamVR Unity plugin example:
`Extras/SteamVR_TestThrow` scene as it includes the relevant `Model`
children on the controller (which seem to be missing from the default
prefab in the SteamVR plugin `Prefabs/[CameraRig].prefab`.

The `SteamVR_Unity_Toolkit/Prefabs/[CameraRig]` can be dropped into any scene to
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

There are also common action aliases that are emitted when controller
buttons are pressed. These action aliases can be mapped to a
preferred controller button. The aliases are:

  * Toggle Pointer - Common action of turning a laser pointer on/off
  * Toggle Interact - Common action of grabbing or using game objects
  * Toggle Menu - Common action of bringing up an in-game menu

Each of the above aliases can have the preferred controller button
mapped to their usage by selecting it from the drop down on the script
parameters window.

When the set button is pressed it will emit the actual button event as
well as an additional event that the alias is "On". When the set button
is released it will emit the actual button event as well as an
additional event that the alias button is "Off".

Listening for these alias events rather than the actual button events
means it's easier to customise the controller buttons to the actions
they should perform.

An example of the `SteamVR_ControllerEvents` script can be viewed in
the scene `Examples/002_Controller_Events` and code examples
of how the events are utilised and listened to can be viewed in the
script `Examples/Scripts/SteamVR_ControllerEvents_ListenerExample.cs`

#### Simple Laser Pointer (SteamVR_SimplePointer)

The simple pointer emits a coloured beam from the end of the controller
to simulate a laser beam. It can be useful for pointing to objects
within a scene and it can also determine the object it is pointing at
and the distance the object is from the controller the beam is being
emitted from.

The laser beam is activated by default by pressing the `Grip` on the
controller. The event it is listening for is the `AliasPointer` events
so the pointer toggle button can be set by changing the
`Pointer Toggle` button on the `SteamVR_ControllerEvents` script
parameters.

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
the scene `Examples/003_Controller_SimplePointer` and
code examples of how the events are utilised and listened to can be
viewed in the script
`Examples/Scripts/SteamVR_ControllerPointerEvents_ListenerExample.cs`

#### Basic Teleporter (SteamVR_BasicTeleport)

The basic teleporter updates the `[CameraRig]` x/z position in the
game world to the position of a World Pointer's tip location which is
set via the `WorldPointerDestinationSet` event. The y position is never
altered so the basic teleporter cannot be used to move up and down
game objects as it only allows for travel across a flat plane.

The Basic Teleport script is attached to the `[CameraRig]` prefab and
requires an implementation of the WorldPointer script to be attached
to another game object (e.g. SteamVR_SimplePointer attached to
the Controller object).

The fade blink speed can be changed on the basic Teleport script to
provide a customised teleport experience. Setting the speed to 0 will
mean no fade blink effect is present. The fade is achieved via the
`SteamVR_Fade.cs` script in the SteamVR Unity Plugin scripts.

An example of the `SteamVR_BasicTeleport` script can be viewed in the
scene `Examples/004_CameraRig_BasicTeleport`. The scene uses
the `SteamVR_SimplePointer` script on the Controllers to initiate a
laser pointer with the Controller `Grip` button and when the laser
pointer is deactivated (release the `Grip`) then the player is
teleported to the location of the laser pointer tip.

#### Height Adjustable Teleporter (SteamVR_HeightAdjustTeleport)

The height adjust teleporter extends the basic teleporter and allows
for the y position of the `[CameraRig]` to be altered based on whether
the teleport location is on top of another object.

Like the basic teleporter the Height Adjust Teleport script is attached
to the `[CameraRig]` prefab and requires a World Pointer to be
available.

There is an additional script parameter of `Play Space Falling` and
when this is checked it means if the player steps off an object into
a part of their play area that is not on the object then they are
automatically teleported down to the nearest floor.

This also works in the opposite way that if the player's headset is
above an object then the player is teleported automatically on top of
that object, which is useful for simulating climbing stairs without
needing to use the pointer beam location.

If this option is turned off then the player can hover in mid air at
the same y position of the object they are standing on.

An example of the `SteamVR_HeightAdjustTeleport` script can be viewed
in the scene `Examples/007_CameraRig_HeightAdjustTeleport`. The scene
has a collection of varying height objects that the player can either
walk up and down or use the laser pointer to climb on top of them.

#### Interactable Object (SteamVR_InteractableObject)

The Interactable Object script is attached to any game object that is
required to be interacted with (e.g. via the controllers).

It currently has states to determine if the object:

  * Can be grabbed
  * Is currently being grabbed
  * Can be used
  * Is currently being used

It also allows for a highlight colour to change the object to when
the controller touches it (it resets the colour on untouch). This
colour can be set to a different colour on each object or a default
colour can be set on the `ControllerInteract` script. However, the
local colour on the interactable object will always take precendence.

The basis of this script is to provide a simple mechanism for
identifying objects in the game world that can be grabbed or used
but it is expected that this script is the base to be inherited into a
script with richer functionality.

An example of the `SteamVR_InteractableObject` can be viewed in the
scene `Examples/005_Controller_BasicObjectGrabbing`. The scene
also uses the `SteamVR_ControllerInteract` script on the controllers to
show how an interactable object can be grabbed and snapped to the
controller and thrown around the game world.

Another example can be viewed in the scene
`Examples/006_ControllerInteract_UsingADoor`.

#### Controller Interation With Objects (SteamVR_ControllerInteract)

The Controller Interact script is attached to a Controller object
within the `[CameraRig]` prefab and the Controller object also
requires the `SteamVR_ControllerEvents` script to be attached as it
uses this for listening to the controller button events for grabbing
and releasing interactable game objects.

An object can be grabbed if the Controller touches a game object which
contains the `SteamVR_InteractableObject` script and has the flag
`isGrabbable` set to `true`.

An object can also be used if the Controller touches an interactable
object with the flag `isUsable` set to `true`.

A valid interactable object is grabbable/usable when it is touched by
the controller and by default pressing the `Trigger` on the controller
will grab the object and snap it to the Controller attach point (default
is the controller tip). The event it is listening for is the
`AliasInteract` events so the interact toggle button can be set by
changing the `Interact Toggle` button on the `SteamVR_ControllerEvents`
script parameters.

When the Controller Interact button is released, if the interactable
game object is grabbable then it will be propelled in the direction
and at the velocity the controller was at, which can simulate object
throwing.

The interactable objects require a collider to activate the trigger and
a rigidbody to pick them up and move them around the game world.

It is possible to set a default Touch Highlight Colour on the
Controller, so any interactable object that is set to Highlight On
Touch and doesn't have a local highlight colour will be highlighted
with this global colour. It's also possible to have a different global
touch colour per controller.

The controller model can be hidden upon an interaction with an object,
ticking one of the `Hide Controller On <action>` parameters on the
script will ensure the controller model is hidden when the interactable
object is being touched, grabbed or used.

There are also a number of events emitted by the Controller Interaction
script:

  * Touching/Untouching an interactable object will emit an event
  * Grabbing/Ungrabbing an interactable object will emit an event
  * Start Using/Stop Using an interactable object will emit an event

The Controller Interaction events are emitted with this payload:

  * ControllerIndex: The index of the controller doing the interaction
  * Target: The GameObject of the interactable object that is being
  interacted with by the controller

An example of the `SteamVR_ControllerInteract` can be viewed in the
scene `Examples/005_Controller/BasicObjectGrabbing`. The scene
demonstrates the grabbing of objects that have the
`SteamVR_InteractableObject` script added to them and also shows the
ability to highlight interactable objects when they are touched by
the controllers.

Another example can be viewed in the scene
`Examples/006_ControllerInteract_UsingADoor`. Which simulates using
a door object to open and close it. It also has a cube on the floor
that can be grabbed to show how interactable objects can be usable
or grabbable.

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
  * Target - The Transform of the object that the pointer is touching
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
  * 004_CameraRig_BasicTeleport: a scene with basic objects that can
  be traversed using the controller laser beam to point at an object
  in the game world where the player is to be teleported to by
  pressing the controller `Grip` button. When the `Grip` button is
  released, the player is teleported to the laser beam end location.
  * 005_Controller_BasicObjectGrabbing: a scene with a selection of
  objects that can be grabbed by touching them with the controller and
  pressing the `Trigger` button down. Releasing the trigger button
  will propel the object in the direction and velocity of the grabbing
  controller. The scene also demonstrates simple highlighting of
  objects when the controller touches them. The interaction events are
  also displayed in the console window.
  * 006_ControllerInteract_UsingADoor: a scene with a door interactable
  object that is set to `usable` and when the door is used by pressing
  the controller `Trigger` button, the door swings open (or closes if
  it's already open)
  * 007_CameraRig_HeightAdjustTeleport: a scene with a selection of
  varying height objects that can be traversed using the controller
  laser beam to point at an object and if the laser beam is pointing
  on top of the object then the player is teleported to the top of the
  object. Also, it shows that if the player steps into a part of the
  play area that is not on the object then the player will fall to
  the nearest object. This also enables the player to climb objects
  just by standing over them as the floor detection is done from the
  position of the headset.

## Contributing

I'd love this to be a community effort, but as I'm just getting
started on this, it may be best to leave pull requests for now on
new features until I get my head around how this is going to
progress. I'm happy for bug fix pull requests though :)

Also, if you find any issues or have any suggestions then please
raise an issue on GitHub and I'll take a look when I can.

## License

Code released under the MIT license.
