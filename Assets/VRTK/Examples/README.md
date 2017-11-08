# Examples

This directory contains Unity3d scenes that demonstrate the scripts and prefabs being used in the game world to create desired functionality.

> *VRTK offers a VR Simulator that works without any third party SDK, but VR device support requires a supported VR SDK to be imported into the Unity project.*

The example scenes support all the VRTK supported VR SDKs. To make use of VR devices (besides the included VR Simulator) import the needed third party VR SDK into the project.

For further information about setting up a specific SDK and using VRTK in your own project, check out the
[GETTING_STARTED.md](/Assets/VRTK/Documentation/GETTING_STARTED.md) document.

There is also a `/ExampleResources/Scripts` directory within the `VRTK/Examples` directory that contains helper scripts utilised by the example scenes to highlight certain functionality (such as event listeners). These example scripts are not required for real world usage.

## Current Examples

### 001_CameraRig_VRPlayArea

A simple scene showing the `[CameraRig]` prefab usage.

### 002_Controller_Events

A simple scene displaying the events from the controller in the console window.

### 003_Controller_SimplePointer

A scene with basic objects that can be pointed at with the laser beam from the controller activated by pressing the `Touchpad`. The pointer events are also displayed in the console window.

### 004_CameraRig_BasicTeleport

A scene with basic objects that can be traversed using the controller laser beam to point at an object in the game world where the user is to be teleported to by pressing `Touchpad` on the controller. When the `Touchpad` is released, the user is teleported to the laser beam end location.

### 005_Controller_BasicObjectGrabbing

A scene with a selection of objects that can be grabbed by touching them with the controller and pressing the `Grip` button down. Releasing the grip button will propel the object in the direction and velocity of the grabbing controller. The scene also demonstrates simple highlighting of objects when the controller touches them. The interaction events are also displayed in the console window.

### 006_Controller_UsingADoor

A scene with a door interactable object that is set to `usable` and when the door is used by pressing the controller `Trigger` button, the door swings open (or closes if it's already open).

### 007_CameraRig_HeightAdjustTeleport

A scene with a selection of varying height objects that can be traversed using the controller laser beam to point at an object and if the laser beam is pointing on top of the object then the user is teleported to the top of the object. Also, it shows that if the user steps into a part of the play area that is not on the object then the user will fall to the nearest object. This also enables the user to climb objects just by standing over them as the floor detection is done from the position of the headset.

### 008_Controller_UsingAGrabbedObject

A scene with interactable objects that can be grabbed (pressing the `Grip` controller button) and then used (pressing the `Trigger` controller button). There is a gun on a table that can be picked up and fired, or a strange box that when picked up and used the top spins.

### 009_Controller_BezierPointer

A scene with a selection of varying height objects that can be traversed using the controller however, rather than just pointing a straight beam, the beam is curved (over a bezier curve) which allows climbing on top of items that the user cannot visibly see.

### 010_CameraRig_TerrainTeleporting

A scene with a terrain object and a selection of varying height 3d objects that can be traversed using the controller laser beam pointer. It shows how the Height Adjust Teleporter can be used to climb up and down game objects as well as traversing terrains as well.

### 011_Camera_HeadSetCollisionFading

A scene with three walls around the play area and if the user puts their head into any of the collidable walls then the headset fades to black to prevent seeing unwanted object clipping artefacts.

### 012_Controller_PointerWithAreaCollision

A scene which demonstrates how to use a controller pointer to traverse a world but where the beam shows the projected play area space and if the space collides with any objects then the teleportation action is disabled. This means it's possible to create a level with areas where the user cannot teleport to because they would allow the user to clip into objects.

### 013_Controller_UsingAndGrabbingMultipleObjects

A scene which demonstrates how interactable objects can be grabbed by holding down the grab button continuously or by pressing the grab button once to pick up and once again to release. The scene also shows that the use button can have a hold down to keep using or a press use button once to start using and press again to stop using. This allows multiple objects to be put into their Using state at the same time as also demonstrated in this example scene.

### 014_Controller_SnappingObjectsOnGrab

A scene with a selection of objects that demonstrate the different snap to controller mechanics. The two green guns, light saber and sword utilise a Snap Handle which uses an empty game object as a child of the interactable object as the orientation point at grab, so the rotation and position of the object matches that of the given `Snap Handle`. The red gun utilises a basic snap where no precision is required and no Snap Handles are provided which does not affect the object's rotation but positions the centre of the object to the snap point on the controller. The red/green gun utilises the `Precision Snap` which does not affect the rotation or position of the grabbed object and picks the object up at the point that the controller snap point is touching the object.

### 015_Controller_TouchpadAxisControl

A scene with an R/C car that is controlled by using the Controller Touchpad. Moving a finger up and down on the Touchpad will cause the car to drive forward or backward. Moving a finger to the left or right of the `Touchpad` will cause the car to turn in that direction. Pressing the `Trigger` will cause the car to jump, this utilises the Trigger axis and the more the trigger is depressed, the higher the car will jump.

### 016_Controller_HapticRumble

A scene with a collection of breakable boxes and a sword. The sword can be picked up and swung at the boxes. The controller rumbles at an appropriate vibration depending on how hard the sword hits the box. The box also breaks apart if it is hit hard enough by the sword.

### 017_CameraRig_TouchpadWalking

A scene which demonstrates how to move around the game world using the touchpad by sliding a finger forward and backwards to move in that direction. Sliding a finger left and right across the touchpad strafes in that direction. The rotation is done via the user in game physically rotating their body in the place space and whichever way the headset is looking will be the way the user walks forward. Crouching is also possible as demonstrated in this scene and in conjunction with the Headset Collision Fade script it can detect unwanted collisions (e.g. if the user stands up whilst walking as crouched) and reset their position to the last good known position.

### 018_CameraRig_FramesPerSecondCounter

A scene which displays the frames per second in the centre of the headset view. Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres. Eventually when lots of spheres are present the FPS will drop and demonstrate the prefab.

### 019_Controller_InteractingWithPointer

A scene which shows how the controller pointer beam can be used to toggle the use actions on interactable objects. Pressing the touchpad activates the beam and aiming it at objects will toggle their use state. It also demonstrates how a game menu could be created by using interactable objects snapped to a game object can trigger actions. Pressing the Application Menu button displays a cube connected to the controller which has menu options. Pointing the beam with the other controller at the cube will select the menu options accordingly.

### 020_CameraRig_MeshTeleporting

A scene with an object with a mesh collider to demonstrate how the Height Adjust Teleporter can be used to climb up and down objects with a mesh collider.

### 021_Controller_GrabbingObjectsWithJoints

A scene with a collection of Interactable Objects that are attached to other objects with joints. The example shows that Interactable Objects can have different attach mechanics to determine the best way of connecting the object to the controller. Fixed Joint works well for holding objects like cubes as they track perfectly to the controller whereas a Spring Joint works well on the drawer to give it a natural slide when operating. Finally, the Rotator Track works well on the door to give a natural control over the swing of the door. There is also a Character Joint object that can be manipulated into different shapes by pulling each of the relevant sections.

### 022_Controller_CustomBezierPointer

A scene that demonstrates how the Bezier Pointer can have complex objects passed to it to generate the tracer beam and the cursor of the pointer. In the scene, particle objects with rotations are used to demonstrate a different look to the bezier pointer beam.

### 023_Controller_ChildOfControllerOnGrab

A scene that demonstrates the grab mechanic where the object being grabbed becomes a child of the controller doing the grabbing. This works well for objects that need absolute tracking of the controller and do not want to be disjointed under any circumstances. The object becomes an extension of the controller. The scene demonstrates this with a bow and arrow example, where the bow can be picked up and tracked to the controller, whilst the other controller is responsible for picking up arrows to fire in the bow.

### 024_CameraRig_ExcludeTeleportLocation

A scene that shows how to exclude certain objects from being teleportable by either applying a named Tag to the object or by applying a Script of a certain name. In the scene, the yellow objects are excluded from teleport locations by having an `ExcludeTeleport` tag set on them and the black objects are excluded by having a script called `ExcludeTeleport` attached to them. The `ExcludeTeleport` script has no methods and is just used as a placeholder.

### 025_Controls_Overview

A scene that showcases the existing interactive controls, different ways how they can be set up and how to react to events sent by them.

### 026_Controller_ForceHoldObject

A scene that shows how to grab an object on game start and prevent the user from dropping that object. The scene auto grabs two swords to each of the controllers and it's not possible to drop either of the swords.

### 027_CameraRig_TeleportByModelVillage

A scene that demonstrates how to teleport to different locations without needing a world pointer and using the Destination Events abstract class on objects that represent a mini map of the game world. Touching and using an object on the map teleports the user to the specified location.

### 028_CameraRig_RoomExtender

A scene that demonstrates the concept of extending the physical room scale space by multiplying the physical steps taken in the chaperone bounds. A higher multiplier will mean the user can walk further in the play area and the walk multiplier can be toggled by a button press.

### 029_Controller_Tooltips

A scene that demonstrates adding tooltips to game objects and to the controllers using the prefabs `ObjectTooltip` and `ControllerTooltips`.

### 030_Controls_RadialTouchpadMenu

A scene that demonstrates adding dynamic radial menus to controllers and other objects using the prefab `RadialMenu`.

### 031_CameraRig_HeadsetGazePointer

A scene that demonstrates the ability to attach a pointer to the headset to allow for a gaze pointer for teleporting or other interactions supported by the World Pointers. The `Touchpad` on the right controller activates the gaze beam, whereas the `Touchpad` on the left controller activates a beam projected from a drone in the sky as the World Pointers can be attached to any object.

### 032_Controller_CustomControllerModel

A scene that demonstrates how to use custom models for the controllers instead of the default HTC Vive controllers. It uses two simple hands in place of the default controllers and shows simple state changes based on whether the grab button or use button are being pressed.

### 033_CameraRig_TeleportingInNavMesh

A scene that demonstrates how a baked NavMesh can be used to define the regions that a user is allowed to teleport into.

### 034_Controls_InteractingWithUnityUI

A scene that demonstrates how to interact with Unity UI elements. The scene uses the `VRTK_UIPointer` script on the right Controller to allow for the interaction with Unity UI elements using a Simple Pointer beam. The left Controller controls a Simple Pointer on the headset to demonstrate gaze interaction with Unity UI elements.

### 035_Controller_OpacityAndHighlighting

A scene that demonstrates how to change the opacity of the controller and how to highlight elements of the controller such as the buttons or even the entire controller model.

### 036_Controller_CustomCompoundPointer

A scene that demonstrates how the Bezier Pointer can display an object (teleport beam) only if the teleport location is valid, and can create an animated trail along the tracer curve. This scene provides a textured environment for testing the teleport, some active "plasma" spheres on the wall that can be activated with the pointer and another sphere that can be also grabbed and launched around.

### 037_CameraRig_ClimbingFalling

A scene that demonstrates how to set up the climbing mechanism with different activities to try it with. Various objects with a `VRTK_InteractableObject` component are scattered throughout the level. They all have the `GrabAttachMechanic` set to `Climbable`.

### 038_CameraRig_CameraRig_DashTeleport

A scene that shows the teleporting behaviour and also demonstrates a way to use the broadcasted RaycastHit array. In the example obstacles in the way of the dash turn off their mesh renderers while the dash is in progress.

### 039_CameraRig_AdaptiveQuality

  > **Only Compatible With Unity 5.4 and above**

A scene displays the frames per second in the centre of the headset view. The debug visualization of this script is displayed near the top edge of the headset view. Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres. Eventually when lots of spheres are present the FPS will drop and demonstrate the script.

### 040_Controls_PanelMenu

A scene that demonstrates how to attach interactable panel prefabs to game objects to provide additional settings.

### 041_Controller_ObjectSnappingToDropZones

A scene that uses the `VRTK_SnapDropZone` prefab to set up pre-determined snap zones for a range of objects and demonstrates how only objects of certain types can be snapped into certain areas.

### 042_CameraRig_MoveInPlace

A scene that demonstrates how the user can move and traverse colliders by either swinging the controllers in a walking fashion or by running on the spot utilising the head bob for movement.

### 043_Controller_SecondaryControllerActions

A scene that demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.

### 044_CameraRig_RestrictedTeleportZones

A scene that uses the `VRTK_DestinationPoint` prefab to set up a collection of pre-defined teleport locations.