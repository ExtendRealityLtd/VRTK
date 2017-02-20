# Introduction

This file provides documentation on how to use the included prefabs and scripts.

 * [Prefabs](#prefabs-vrtkprefabs)
 * [Pointers](#pointers-vrtkscriptspointers)
  * [Pointer Renderers](#pointer-renderers-vrtkscriptspointerspointerrenderers)
 * [Locomotion](#locomotion-vrtkscriptslocomotion)
  * [Touchpad Control Actions](#touchpad-control-actions-vrtkscriptslocomotiontouchpadcontrolactions)
 * [Interactions](#interactions-vrtkscriptsinteractions)
  * [Highlighters](#highlighters-vrtkscriptsinteractionshighlighters)
  * [Grab Attach Mechanics](#grab-attach-mechanics-vrtkscriptsinteractionsgrabattachmechanics)
  * [Secondary Controller Grab Actions](#secondary-controller-grab-actions-vrtkscriptsinteractionssecondarycontrollergrabactions)
 * [Presence](#presence-vrtkscriptspresence)
 * [UI](#ui-vrtkscriptsui)
 * [3D Controls](#3d-controls-vrtkscriptscontrols3d)
 * [Utilities](#utilities-vrtkscriptsutilities)
 * [Base SDK](#base-sdk-vrtksdkbase)
  * [Fallback SDK](#fallback-sdk-vrtksdkfallback)
  * [Simulator SDK](#simulator-sdk-vrtksdksimulator)
  * [SteamVR SDK](#steamvr-sdk-vrtksdksteamvr)
  * [OculusVR SDK](#oculusvr-sdk-vrtksdkoculusvr)

---

# Prefabs (VRTK/Prefabs)

A collection of pre-defined usable prefabs have been included to allow for each drag-and-drop set up of common elements.

 * [VR Simulator](#vr-simulator-sdk_inputsimulator)
 * [Frames Per Second Canvas](#frames-per-second-canvas-vrtk_framespersecondviewer)
 * [Object Tooltip](#object-tooltip-vrtk_objecttooltip)
 * [Controller Tooltips](#controller-tooltips-vrtk_controllertooltips)
 * [Controller Rigidbody Activator](#controller-rigidbody-activator-vrtk_controllerrigidbodyactivator)
 * [Snap Drop Zone](#snap-drop-zone-vrtk_snapdropzone)
 * [Radial Menu](#radial-menu-radialmenu)
 * [Independent Radial Menu Controller](#independent-radial-menu-controller-vrtk_independentradialmenucontroller)
 * [Destination Point](#destination-point-vrtk_destinationpoint)
 * [Console Viewer Canvas](#console-viewer-canvas-vrtk_consoleviewer)
 * [Panel Menu Controller](#panel-menu-controller-panelmenucontroller)
 * [Panel Menu Item Controller](#panel-menu-item-controller-panelmenuitemcontroller)

---

## VR Simulator (SDK_InputSimulator)

### Overview

The `VRSimulatorCameraRig` prefab is a mock Camera Rig set up that can be used to develop with VRTK without the need for VR Hardware.

Use the mouse and keyboard to move around both play area and hands and interacting with objects without the need of a hmd or VR controls.

### Inspector Parameters

 * **Hide Hands At Switch:** Hide hands when disabling them.
 * **Reset Hands At Switch:** Reset hand position and rotation when enabling them.
 * **Hand Move Multiplier:** Adjust hand movement speed.
 * **Hand Rotation Multiplier:** Adjust hand rotation speed.
 * **Player Move Multiplier:** Adjust player movement speed.
 * **Player Rotation Multiplier:** Adjust player rotation speed.
 * **Change Hands:** Key used to switch between left and righ hand.
 * **Hands On Off:** Key used to switch hands On/Off.
 * **Rotation Position:** Key used to switch between positional and rotational movement.
 * **Change Axis:** Key used to switch between X/Y and X/Z axis.
 * **Trigger Alias:** Key used to simulate trigger button.
 * **Grip Alias:** Key used to simulate grip button.
 * **Touchpad Alias:** Key used to simulate touchpad button.
 * **Button One Alias:** Key used to simulate button one.
 * **Button Two Alias:** Key used to simulate button two.
 * **Start Menu Alias:** Key used to simulate start menu button.
 * **Touch Modifier:** Key used to switch between button touch and button press mode.
 * **Hair Touch Modifier:** Key used to switch between hair touch mode.

### Class Methods

#### FindInScene/0

  > `public static GameObject FindInScene()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - Returns the found `VRSimulatorCameraRig` GameObject if it is found. If it is not found then it prints a debug log error.

The FindInScene method is used to find the `VRSimulatorCameraRig` GameObject within the current scene.

---

## Frames Per Second Canvas (VRTK_FramesPerSecondViewer)

### Overview

This canvas adds a frames per second text element to the headset. To use the prefab it must be placed into the scene then the headset camera needs attaching to the canvas:

* Select `FramesPerSecondCanvas` object from the scene objects
* Find the `Canvas` component
* Set the `Render Camera` parameter to the camera used by the VR Headset (e.g. SteamVR: [CameraRig]-> Camera(Head) -> Camera(eye)])

This script is pretty much a copy and paste from the script at: http://talesfromtherift.com/vr-fps-counter/ So all credit to Peter Koch for his work. Twitter: @peterept

### Inspector Parameters

 * **Display FPS:** Toggles whether the FPS text is visible.
 * **Target FPS:** The frames per second deemed acceptable that is used as the benchmark to change the FPS text colour.
 * **Font Size:** The size of the font the FPS is displayed in.
 * **Position:** The position of the FPS text within the headset view.
 * **Good Color:** The colour of the FPS text when the frames per second are within reasonable limits of the Target FPS.
 * **Warn Color:** The colour of the FPS text when the frames per second are falling short of reasonable limits of the Target FPS.
 * **Bad Color:** The colour of the FPS text when the frames per second are at an unreasonable level of the Target FPS.

### Example

`VRTK/Examples/018_CameraRig_FramesPerSecondCounter` displays the frames per second in the centre of the headset view. Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres. Eventually when lots of spheres are present the FPS will drop and demonstrate the prefab.

---

## Object Tooltip (VRTK_ObjectTooltip)

### Overview

This adds a UI element into the World Space that can be used to provide additional information about an object by providing a piece of text with a line drawn to a destination point.

There are a number of parameters that can be set on the Prefab which are provided by the `VRTK_ObjectTooltip` script which is applied to the prefab.

### Inspector Parameters

 * **Display Text:** The text that is displayed on the tooltip.
 * **Font Size:** The size of the text that is displayed.
 * **Draw Line From:** An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.
 * **Draw Line To:** A transform of another object in the scene that a line will be drawn from the tooltip to, this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, then the parent object's transform will be used as this destination position.
 * **Line Width:** The width of the line drawn between the tooltip and the destination transform.
 * **Font Color:** The colour to use for the text on the tooltip.
 * **Container Color:** The colour to use for the background container of the tooltip.
 * **Line Color:** The colour to use for the line drawn between the tooltip and the destination transform.

### Class Methods

#### ResetTooltip/0

  > `public void ResetTooltip()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetTooltip method resets the tooltip back to its initial state.

#### UpdateText/1

  > `public void UpdateText(string newText)`

  * Parameters
   * `string newText` - A string containing the text to update the tooltip to display.
  * Returns
   * _none_

The UpdateText method allows the tooltip text to be updated at runtime.

### Example

`VRTK/Examples/029_Controller_Tooltips` displays two cubes that have an object tooltip added to them along with tooltips that have been added to the controllers.

---

## Controller Tooltips (VRTK_ControllerTooltips)

### Overview

This adds a collection of Object Tooltips to the Controller that give information on what the main controller buttons may do. To add the prefab, it just needs to be added as a child of the relevant alias controller GameObject.

If the transforms for the buttons are not provided, then the script will attempt to find the attach transforms on the default controller model.

If no text is provided for one of the elements then the tooltip for that element will be set to disabled.

There are a number of parameters that can be set on the Prefab which are provided by the `VRTK_ControllerTooltips` script which is applied to the prefab.

### Inspector Parameters

 * **Trigger Text:** The text to display for the trigger button action.
 * **Grip Text:** The text to display for the grip button action.
 * **Touchpad Text:** The text to display for the touchpad action.
 * **Button One Text:** The text to display for button one action.
 * **Button Two Text:** The text to display for button two action.
 * **Start Menu Text:** The text to display for the start menu action.
 * **Tip Background Color:** The colour to use for the tooltip background container.
 * **Tip Text Color:** The colour to use for the text within the tooltip.
 * **Tip Line Color:** The colour to use for the line between the tooltip and the relevant controller button.
 * **Trigger:** The transform for the position of the trigger button on the controller.
 * **Grip:** The transform for the position of the grip button on the controller.
 * **Touchpad:** The transform for the position of the touchpad button on the controller.
 * **Button One:** The transform for the position of button one on the controller.
 * **Button Two:** The transform for the position of button two on the controller.
 * **Start Menu:** The transform for the position of the start menu on the controller.

### Class Methods

#### ResetTooltip/0

  > `public void ResetTooltip()`

  * Parameters
   * _none_
  * Returns
   * _none_

The Reset method reinitalises the tooltips on all of the controller elements.

#### UpdateText/2

  > `public void UpdateText(TooltipButtons element, string newText)`

  * Parameters
   * `TooltipButtons element` - The specific controller element to change the tooltip text on.
   * `string newText` - A string containing the text to update the tooltip to display.
  * Returns
   * _none_

The UpdateText method allows the tooltip text on a specific controller element to be updated at runtime.

#### ToggleTips/2

  > `public void ToggleTips(bool state, TooltipButtons element = TooltipButtons.None)`

  * Parameters
   * `bool state` - The state of whether to display or hide the controller tooltips, true will display and false will hide.
   * `TooltipButtons element` - The specific element to hide the tooltip on, if it is `TooltipButtons.None` then it will hide all tooltips. Optional parameter defaults to `TooltipButtons.None`
  * Returns
   * _none_

The ToggleTips method will display the controller tooltips if the state is `true` and will hide the controller tooltips if the state is `false`. An optional `element` can be passed to target a specific controller tooltip to toggle otherwise all tooltips are toggled.

### Example

`VRTK/Examples/029_Controller_Tooltips` displays two cubes that have an object tooltip added to them along with tooltips that have been added to the controllers.

---

## Controller Rigidbody Activator (VRTK_ControllerRigidbodyActivator)

### Overview

This adds a simple trigger collider volume that when a controller enters will enable the rigidbody on the controller.

The prefab game object should be placed in the scene where another interactable game object (such as a button control) is located to turn the controller rigidbody on at the appropriate time for interaction with the control without needing to manually activate by pressing the grab.

If the prefab is placed as a child of the target interactable game object then the collider volume on the prefab will trigger collisions on the interactable object.

The sphere collider on the prefab can have the radius adjusted to determine how close the controller needs to be to the object before the rigidbody is activated.

It's also possible to replace the sphere trigger collider with an alternative trigger collider for customised collision detection.

### Inspector Parameters

 * **Is Enabled:** If this is checked then the collider will have it's rigidbody toggled on and off during a collision.

---

## Snap Drop Zone (VRTK_SnapDropZone)

### Overview

This sets up a predefined zone where an existing interactable object can be dropped and upon dropping it snaps to the set snap drop zone transform position, rotation and scale.

The position, rotation and scale of the `SnapDropZone` Game Object will be used to determine the final position of the dropped interactable object if it is dropped within the drop zone collider volume.

The provided Highlight Object Prefab is used to create the highlighting object (also within the Editor for easy placement) and by default the standard Material Color Swap highlighter is used.

An alternative highlighter can also be added to the `SnapDropZone` Game Object and this new highlighter component will be used to show the interactable object position on release.

The prefab is a pre-built game object that contains a default trigger collider (Sphere Collider) and a kinematic rigidbody (to ensure collisions occur).

If an alternative collider is required, then the default Sphere Collider can be removed and another collider added.

If the `Use Joint` Snap Type is selected then a custom Joint component is required to be added to the `SnapDropZone` Game Object and upon release the interactable object's rigidbody will be linked to this joint as the `Connected Body`.

### Inspector Parameters

 * **Highlight Object Prefab:** A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.
 * **Snap Type:** The Snap Type to apply when a valid interactable object is dropped within the snap zone.
 * **Snap Duration:** The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.
 * **Apply Scaling On Snap:** If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.
 * **Highlight Color:** The colour to use when showing the snap zone is active.
 * **Highlight Always Active:** The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.
 * **Valid Object List Policy:** A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.
 * **Display Drop Zone In Editor:** If this is checked then the drop zone highlight section will be displayed in the scene editor window.

### Class Variables

 * `public enum SnapTypes` - The types of snap on release available.
  * `Use_Kinematic` - Will set the interactable object rigidbody to `isKinematic = true`.
  * `Use_Joint` - Will attach the interactable object's rigidbody to the provided joint as it's `Connected Body`.
  * `Use_Parenting` - Will set the SnapDropZone as the interactable object's parent and set it's rigidbody to `isKinematic = true`.

### Class Events

 * `ObjectEnteredSnapDropZone` - Emitted when a valid interactable object enters the snap drop zone trigger collider.
 * `ObjectExitedSnapDropZone` - Emitted when a valid interactable object exists the snap drop zone trigger collider.
 * `ObjectSnappedToDropZone` - Emitted when an interactable object is successfully snapped into a drop zone.
 * `ObjectUnsnappedFromDropZone` - Emitted when an interactable object is removed from a snapped drop zone.

### Unity Events

Adding the `VRTK_SnapDropZone_UnityEvents` component to `VRTK_SnapDropZone` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnObjectEnteredSnapDropZone` - Emits the ObjectEnteredSnapDropZone class event.
 * `OnObjectExitedSnapDropZone` - Emits the ObjectExitedSnapDropZone class event.
 * `OnObjectSnappedToDropZone` - Emits the ObjectSnappedToDropZone class event.
 * `OnObjectUnsnappedFromDropZone` - Emits the ObjectUnsnappedFromDropZone class event.

### Event Payload

 * `GameObject snappedObject` - The interactable object that is dealing with the snap drop zone.

### Class Methods

#### InitaliseHighlightObject/1

  > `public virtual void InitaliseHighlightObject(bool removeOldObject = false)`

  * Parameters
   * `bool removeOldObject` - If this is set to true then it attempts to delete the old highlight object if it exists. Defaults to `false`
  * Returns
   * _none_

The InitaliseHighlightObject method sets up the highlight object based on the given Highlight Object Prefab.

#### ForceSnap/1

  > `public virtual void ForceSnap(GameObject objectToSnap)`

  * Parameters
   * `GameObject objectToSnap` - The GameObject to attempt to snap.
  * Returns
   * _none_

the ForceSnap method attempts to automatically attach a valid game object to the snap drop zone.

#### ForceUnsnap/0

  > `public virtual void ForceUnsnap()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.

### Example

`VRTK/Examples/041_Controller_ObjectSnappingToDropZones` uses the `VRTK_SnapDropZone` prefab to set up pre-determined snap zones for a range of objects and demonstrates how only objects of certain types can be snapped into certain areas.

---

## Radial Menu (RadialMenu)

### Overview

This adds a UI element into the world space that can be dropped into a Controller object and used to create and use Radial Menus from the touchpad.

If the RadialMenu is placed inside a controller, it will automatically find a `VRTK_ControllerEvents` in its parent to use at the input. However, a `VRTK_ControllerEvents` can be defined explicitly by setting the `Events` parameter of the `Radial Menu Controller` script also attached to the prefab.

The RadialMenu can also be placed inside a `VRTK_InteractableObject` for the RadialMenu to be anchored to a world object instead of the controller. The `Events Manager` parameter will automatically be set if the RadialMenu is a child of an InteractableObject, but it can also be set manually in the inspector. Additionally, for the RadialMenu to be anchored in the world, the `RadialMenuController` script in the prefab must be replaced with `VRTK_IndependentRadialMenuController`. See the script information for further details on making the RadialMenu independent of the controllers.

### Inspector Parameters

 * **Buttons:** An array of Buttons that define the interactive buttons required to be displayed as part of the radial menu.
 * **Button Prefab:** The base for each button in the menu, by default set to a dynamic circle arc that will fill up a portion of the menu.
 * **Generate On Awake:** If checked, then the buttons will be auto generated on awake.
 * **Button Thickness:** Percentage of the menu the buttons should fill, 1.0 is a pie slice, 0.1 is a thin ring.
 * **Button Color:** The background colour of the buttons, default is white.
 * **Offset Distance:** The distance the buttons should move away from the centre. This creates space between the individual buttons.
 * **Offset Rotation:** The additional rotation of the Radial Menu.
 * **Rotate Icons:** Whether button icons should rotate according to their arc or be vertical compared to the controller.
 * **Icon Margin:** The margin in pixels that the icon should keep within the button.
 * **Is Shown:** Whether the buttons are shown
 * **Hide On Release:** Whether the buttons should be visible when not in use.
 * **Execute On Unclick:** Whether the button action should happen when the button is released, as opposed to happening immediately when the button is pressed.
 * **Base Haptic Strength:** The base strength of the haptic pulses when the selected button is changed, or a button is pressed. Set to zero to disable.
 * **Menu Buttons:** The actual GameObjects that make up the radial menu.

### Example

`VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.

---

## Independent Radial Menu Controller (VRTK_IndependentRadialMenuController)
 > extends RadialMenuController

### Overview

This script inherited from `RadialMenuController` and therefore can be used instead of `RadialMenuController` to allow the RadialMenu to be anchored to any object, not just a controller. The RadialMenu will show when a controller is near the object and the buttons can be clicked with the `Use Alias` button. The menu also automatically rotates towards the user.

To convert the default `RadialMenu` prefab to be independent of the controllers:

* Make the `RadialMenu` a child of an object other than a controller.
* Position and scale the menu by adjusting the transform of the `RadialMenu` empty.
* Replace `RadialMenuController` with `VRTK_IndependentRadialMenuController`.
* Ensure the parent object has the `VRTK_InteractableObject` script.
* Verify that `Is Usable` and `Hold Button to Use` are both checked.
* Attach `VRTK_InteractTouch` and `VRTK_InteractUse` scripts to the controllers.

### Inspector Parameters

 * **Events Manager:** If the RadialMenu is the child of an object with VRTK_InteractableObject attached, this will be automatically obtained. It can also be manually set.
 * **Add Menu Collider:** Whether or not the script should dynamically add a SphereCollider to surround the menu.
 * **Collider Radius Multiplier:** This times the size of the RadialMenu is the size of the collider.
 * **Hide After Execution:** If true, after a button is clicked, the RadialMenu will hide.
 * **Offset Multiplier:** How far away from the object the menu should be placed, relative to the size of the RadialMenu.
 * **Rotate Towards:** The object the RadialMenu should face towards. If left empty, it will automatically try to find the Headset Camera.

---

## Destination Point (VRTK_DestinationPoint)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

The Destination Point allows for a specific scene marker that can be teleported to.

The destination points can provide a useful way of having specific teleport locations in a scene.

The destination points can also have a locked state if the `Enable Teleport` flag is disabled.

### Inspector Parameters

 * **Default Cursor Object:** The GameObject to use to represent the default cursor state.
 * **Hover Cursor Object:** The GameObject to use to represent the hover cursor state.
 * **Locked Cursor Object:** The GameObject to use to represent the locked cursor state.
 * **Snap To Point:** If this is checked then after teleporting, the play area will be snapped to the origin of the destination point. If this is false then it's possible to teleport to anywhere within the destination point collider.
 * **Hide Pointer Cursor On Hover:** If this is checked, then the pointer cursor will be hidden when a valid destination point is hovered over.

### Class Methods

#### ResetDestinationPoint/0

  > `public virtual void ResetDestinationPoint()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetDestinationPoint resets the destination point back to the default state.

### Example

`044_CameraRig_RestrictedTeleportZones` uses the `VRTK_DestinationPoint` prefab to set up a collection of pre-defined teleport locations.

---

## Console Viewer Canvas (VRTK_ConsoleViewer)

### Overview

This canvas adds the unity console log to a world game object. To use the prefab, it simply needs to be placed into the scene and it will be visible in world space. It's also possible to child it to other objects such as the controller so it can track where the user is.

It's also recommended to use the Simple Pointer and UI Pointer on a controller to interact with the Console Viewer Canvas as it has a scrollable text area, a button to clear the log and a checkbox to toggle whether the log messages are collapsed.

### Inspector Parameters

 * **Font Size:** The size of the font the log text is displayed in.
 * **Info Message:** The colour of the text for an info log message.
 * **Assert Message:** The colour of the text for an assertion log message.
 * **Warning Message:** The colour of the text for a warning log message.
 * **Error Message:** The colour of the text for an error log message.
 * **Exception Message:** The colour of the text for an exception log message.

### Class Methods

#### SetCollapse/1

  > `public void SetCollapse(bool state)`

  * Parameters
   * `bool state` - The state of whether to collapse the output messages, true will collapse and false will not collapse.
  * Returns
   * _none_

The SetCollapse method determines whether the console will collapse same message output into the same line. A state of `true` will collapse messages and `false` will print the same message for each line.

#### ClearLog/0

  > `public void ClearLog()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ClearLog method clears the current log view of all messages

---

## Panel Menu Controller (PanelMenuController)

### Overview

Purpose: top-level controller class to handle the display of up to four child PanelMenuItemController items which are displayed as a canvas UI panel.

This script should be attached to a VRTK_InteractableObject > first child GameObject [PanelMenuController].
The [PanelMenuController] must have a child GameObject [panel items container].
The [panel items container] must have a Canvas component.
A [panel items container] can have up to four child GameObject, each of these contains the UI for a panel that can be displayed by [PanelMenuController].
They also have the [PanelMenuItemController] script attached to them. The [PanelMenuItemController] script intercepts the controller events sent from this [PanelMenuController] and passes them onto additional custom event subscriber scripts, which then carry out the required custom UI actions.
To show / hide a UI panel, you must first pick up the VRTK_InteractableObject and then by pressing the touchpad top/bottom/left/right you can open/close the child UI panel that has been assigned via the Unity Editor panel. Button type UI actions are handled by a trigger press when the panel is open.

### Inspector Parameters

 * **Rotate Towards:** The GameObject the panel should rotate towards, which is the Camera (eye) by default.
 * **Zoom Scale Multiplier:** The scale multiplier, which relates to the scale of parent interactable object.
 * **Top Panel Menu Item Controller:** The top PanelMenuItemController, which is triggered by pressing up on the controller touchpad.
 * **Bottom Panel Menu Item Controller:** The bottom PanelMenuItemController, which is triggered by pressing down on the controller touchpad.
 * **Left Panel Menu Item Controller:** The left PanelMenuItemController, which is triggered by pressing left on the controller touchpad.
 * **Right Panel Menu Item Controller:** The right PanelMenuItemController, which is triggered by pressing right on the controller touchpad.

### Example

`040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.

---

## Panel Menu Item Controller (PanelMenuItemController)

### Overview

Purpose: panel item controller class that intercepts the controller events sent from a [PanelMenuController] and passes them onto additional custom event subscriber scripts, which then carry out the required custom UI actions.

This script should be attached to a VRTK_InteractableObject > [PanelMenuController] > [panel items container] > child GameObject (See the [PanelMenuController] class for more details on setup structure.).
To show / hide a UI panel, you must first pick up the VRTK_InteractableObject and then by pressing the touchpad top/bottom/left/right you can open/close the child UI panel that has been assigned via the Unity Editor panel.

### Class Events

 * `PanelMenuItemShowing` - Emitted when the panel menu item is showing.
 * `PanelMenuItemHiding` - Emitted when the panel menu item is hiding.
 * `PanelMenuItemSwipeLeft` - Emitted when the panel menu item is open and the user swipes left on the controller touchpad.
 * `PanelMenuItemSwipeRight` - Emitted when the panel menu item is open and the user swipes right on the controller touchpad.
 * `PanelMenuItemSwipeTop` - Emitted when the panel menu item is open and the user swipes top on the controller touchpad.
 * `PanelMenuItemSwipeBottom` - Emitted when the panel menu item is open and the user swipes bottom on the controller touchpad.
 * `PanelMenuItemTriggerPressed` - Emitted when the panel menu item is open and the user presses the trigger of the controller holding the interactable object.

### Event Payload

 * `GameObject interactableObject` - The GameObject for the interactable object the PanelMenu is attached to.

### Example

`040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.

---

# Pointers (VRTK/Scripts/Pointers)

A collection of scripts that provide the ability to create pointers and set destination markers in the scene.

 * [Destination Marker](#destination-marker-vrtk_destinationmarker)
 * [Pointer](#pointer-vrtk_pointer)
 * [Play Area Cursor](#play-area-cursor-vrtk_playareacursor)

---

## Destination Marker (VRTK_DestinationMarker)

### Overview

This abstract class provides the ability to emit events of destination markers within the game world. It can be useful for tagging locations for specific purposes such as teleporting.

It is utilised by the `VRTK_BasePointer` for dealing with pointer events when the pointer cursor touches areas within the game world.

### Inspector Parameters

 * **Enable Teleport:** If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.

### Class Events

 * `DestinationMarkerEnter` - Emitted when a collision with another game object has occurred.
 * `DestinationMarkerExit` - Emitted when the collision with the other game object finishes.
 * `DestinationMarkerSet` - Emitted when the destination marker is active in the scene to determine the last destination position (useful for selecting and teleporting).

### Unity Events

Adding the `VRTK_DestinationMarker_UnityEvents` component to `VRTK_DestinationMarker` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnDestinationMarkerEnter` - Emits the DestinationMarkerEnter class event.
 * `OnDestinationMarkerExit` - Emits the DestinationMarkerExit class event.
 * `OnDestinationMarkerSet` - Emits the DestinationMarkerSet class event.

### Event Payload

 * `float distance` - The distance between the origin and the collided destination.
 * `Transform target` - The Transform of the collided destination object.
 * `RaycastHit raycastHit` - The optional RaycastHit generated from when the ray collided.
 * `Vector3 destinationPosition` - The world position of the destination marker.
 * `bool forceDestinationPosition` - If true then the given destination position should not be altered by anything consuming the payload.
 * `bool enableTeleport` - Whether the destination set event should trigger teleport.
 * `uint controllerIndex` - The optional index of the controller emitting the beam.

### Class Methods

#### SetInvalidTarget/1

  > `public virtual void SetInvalidTarget(VRTK_PolicyList list = null)`

  * Parameters
   * `VRTK_PolicyList list` - The Tag Or Script list policy to check the set operation on.
  * Returns
   * _none_

The SetInvalidTarget method is used to set objects that contain the given tag or class matching the name as invalid destination targets. It accepts a VRTK_PolicyList for a custom level of policy management.

#### SetNavMeshCheckDistance/1

  > `public virtual void SetNavMeshCheckDistance(float distance)`

  * Parameters
   * `float distance` - The max distance the nav mesh can be from the sample point to be valid.
  * Returns
   * _none_

The SetNavMeshCheckDistance method sets the max distance the destination marker position can be from the edge of a nav mesh to be considered a valid destination.

#### SetHeadsetPositionCompensation/1

  > `public virtual void SetHeadsetPositionCompensation(bool state)`

  * Parameters
   * `bool state` - The state of whether to take the position of the headset within the play area into account when setting the destination marker.
  * Returns
   * _none_

The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.

---

## Pointer (VRTK_Pointer)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

The VRTK Pointer class forms the basis of being able to emit a pointer from a game object (e.g. controller).

The concept of the pointer is it can be activated and deactivated and used to select elements utilising different button combinations if required.

The Pointer requires a Pointer Renderer which is the visualisation of the pointer in the scene.

A Pointer can also be used to extend the interactions of an interacting object such as a controller. This enables pointers to touch (and highlight), grab and use interactable objects.

The Pointer script does not need to go on a controller game object, but if it's placed on another object then a controller must be provided to determine what activates the pointer.

It extends the `VRTK_DestinationMarker` to allow for destination events to be emitted when the pointer cursor collides with objects.

### Inspector Parameters

 * **Pointer Renderer:** The specific renderer to use when the pointer is activated. The renderer also determines how the pointer reaches it's destination (e.g. straight line, bezier curve).
 * **Activation Button:** The button used to activate/deactivate the pointer.
 * **Hold Button To Activate:** If this is checked then the Activation Button needs to be continuously held down to keep the pointer active. If this is unchecked then the Activation Button works as a toggle, the first press/release enables the pointer and the second press/release disables the pointer.
 * **Activation Delay:** The time in seconds to delay the pointer being able to be active again.
 * **Selection Button:** The button used to execute the select action at the pointer's target position.
 * **Select On Press:** If this is checked then the pointer selection action is executed when the Selection Button is pressed down. If this is unchecked then the selection action is executed when the Selection Button is released.
 * **Interact With Objects:** If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.
 * **Grab To Pointer Tip:** If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.
 * **Controller:** The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Custom Origin:** A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.

### Class Methods

#### PointerEnter/1

  > `public virtual void PointerEnter(RaycastHit givenHit)`

  * Parameters
   * `RaycastHit givenHit` - The valid collision.
  * Returns
   * _none_

The PointerEnter method emits a DestinationMarkerEnter event when the pointer enters a valid object.

#### PointerExit/1

  > `public virtual void PointerExit(RaycastHit givenHit)`

  * Parameters
   * `RaycastHit givenHit` - The previous valid collision.
  * Returns
   * _none_

The PointerExit method emits a DestinationMarkerExit event when the pointer leaves a previously entered object.

#### CanActivate/0

  > `public virtual bool CanActivate()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the pointer can be activated.

The CanActivate method is used to determine if the pointer has passed the activation time limit.

#### IsPointerActive/0

  > `public virtual bool IsPointerActive()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the pointer is currently active.

The IsPointerActive method is used to determine if the pointer's current state is active or not.

#### ResetActivationTimer/1

  > `public virtual void ResetActivationTimer(bool forceZero = false)`

  * Parameters
   * `bool forceZero` - If this is true then the next activation time will be 0.
  * Returns
   * _none_

The ResetActivationTimer method is used to reset the pointer activation timer to the next valid activation time.

#### Toggle/1

  > `public virtual void Toggle(bool state)`

  * Parameters
   * `bool state` - If true the pointer will be enabled if possible, if false the pointer will be disabled if possible.
  * Returns
   * _none_

The Toggle method is used to enable or disable the pointer.

---

## Play Area Cursor (VRTK_PlayAreaCursor)

### Overview

The Play Area Cursor is used in conjunction with a Pointer script and displays a representation of the play area where the pointer cursor hits.

### Inspector Parameters

 * **Play Area Cursor Dimensions:** Determines the size of the play area cursor and collider. If the values are left as zero then the Play Area Cursor will be sized to the calibrated Play Area space.
 * **Handle Play Area Cursor Collisions:** If this is ticked then if the play area cursor is colliding with any other object then the pointer colour will change to the `Pointer Miss Color` and the `DestinationMarkerSet` event will not be triggered, which will prevent teleporting into areas where the play area will collide.
 * **Headset Out Of Bounds Is Collision:** If this is ticked then if the user's headset is outside of the play area cursor bounds then it is considered a collision even if the play area isn't colliding with anything.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether the play area cursor collisions will be acted upon.

### Class Methods

#### HasCollided/0

  > `public virtual bool HasCollided()`

  * Parameters
   * _none_
  * Returns
   * `bool` - A bool to determine the state of collision. `true` if the play area is colliding with a valid object and `false` if not.

The HasCollided method returns the state of whether the play area cursor has currently collided with another valid object.

#### SetHeadsetPositionCompensation/1

  > `public virtual void SetHeadsetPositionCompensation(bool state)`

  * Parameters
   * `bool state` - The state of whether to take the position of the headset within the play area into account when setting the destination marker.
  * Returns
   * _none_

The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.

#### SetPlayAreaCursorCollision/1

  > `public virtual void SetPlayAreaCursorCollision(bool state)`

  * Parameters
   * `bool state` - The state of whether to check for play area collisions.
  * Returns
   * _none_

The SetPlayAreaCursorCollision method determines whether play area collisions should be taken into consideration with the play area cursor.

#### SetMaterialColor/1

  > `public virtual void SetMaterialColor(Color color)`

  * Parameters
   * `Color color` - The colour to update the play area cursor material to.
  * Returns
   * _none_

The SetMaterialColor method sets the current material colour on the play area cursor.

#### SetPlayAreaCursorTransform/1

  > `public virtual void SetPlayAreaCursorTransform(Vector3 location)`

  * Parameters
   * `Vector3 location` - The location where to draw the play area cursor.
  * Returns
   * _none_

The SetPlayAreaCursorTransform method is used to update the position of the play area cursor in world space to the given location.

#### ToggleState/1

  > `public virtual void ToggleState(bool state)`

  * Parameters
   * `bool state` - The state of whether to show or hide the play area cursor.
  * Returns
   * _none_

The ToggleState method enables or disables the visibility of the play area cursor.

#### GetPlayAreaContainer/0

  > `public virtual GameObject GetPlayAreaContainer()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The GameObject that is the container of the play area cursor.

The GetPlayAreaContainer method returns the created game object that holds the play area cursor representation.

#### ToggleVisibility/1

  > `public virtual void ToggleVisibility(bool state)`

  * Parameters
   * `bool state` - The state of the cursor visibility. True will show the renderers and false will hide the renderers.
  * Returns
   * _none_

The ToggleVisibility method enables or disables the play area cursor renderers to allow the cursor to be seen or hidden.

### Example

`VRTK/Examples/012_Controller_PointerWithAreaCollision` shows how a Bezier Pointer with the Play Area Cursor and Collision Detection enabled can be used to traverse a game area but not allow teleporting into areas where the walls or other objects would fall into the play area space enabling the user to enter walls.

---

# Pointer Renderers (VRTK/Scripts/Pointers/PointerRenderers)

This directory contains scripts that are used to provide different renderers for the VRTK_Pointer.

 * [Base Pointer Renderer](#base-pointer-renderer-vrtk_basepointerrenderer)
 * [Straight Pointer Renderer](#straight-pointer-renderer-vrtk_straightpointerrenderer)
 * [Bezier Pointer Renderer](#bezier-pointer-renderer-vrtk_bezierpointerrenderer)

---

## Base Pointer Renderer (VRTK_BasePointerRenderer)

### Overview

The Base Pointer Renderer script is an abstract class that handles the set up and operation of how a pointer renderer works.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Inspector Parameters

 * **Playarea Cursor:** An optional Play Area Cursor generator to add to the destination position of the pointer tip.
 * **Layers To Ignore:** The layers for the pointer's raycasts to ignore.
 * **Valid Collision Color:** The colour to change the pointer materials when the pointer collides with a valid object. Set to `Color.clear` to bypass changing material colour on valid collision.
 * **Invalid Collision Color:** The colour to change the pointer materials when the pointer is not colliding with anything or with an invalid object. Set to `Color.clear` to bypass changing material colour on invalid collision.
 * **Tracer Visibility:** Determines when the main tracer of the pointer renderer will be visible.
 * **Cursor Visibility:** Determines when the cursor/tip of the pointer renderer will be visible.

### Class Variables

 * `public enum VisibilityStates` - States of Pointer Visibility.
  * `OnWhenActive` - Only shows the object when the pointer is active.
  * `AlwaysOn` - Ensures the object is always.
  * `AlwaysOff` - Ensures the object beam is never visible.

### Class Methods

#### InitalizePointer/4

  > `public virtual void InitalizePointer(VRTK_Pointer givenPointer, VRTK_PolicyList givenInvalidListPolicy, float givenNavMeshCheckDistance, bool givenHeadsetPositionCompensation)`

  * Parameters
   * `VRTK_Pointer givenPointer` - The VRTK_Pointer that is controlling the pointer renderer.
   * `VRTK_PolicyList givenInvalidListPolicy` - The VRTK_PolicyList for managing valid and invalid pointer locations.
   * `float givenNavMeshCheckDistance` - The given distance from a nav mesh that the pointer can be to be valid.
   * `bool givenHeadsetPositionCompensation` - Determines whether the play area cursor will take the headset position within the play area into account when being displayed.
  * Returns
   * _none_

The InitalizePointer method is used to set up the state of the pointer renderer.

#### Toggle/2

  > `public virtual void Toggle(bool pointerState, bool actualState)`

  * Parameters
   * `bool pointerState` - The activation state of the pointer.
   * `bool actualState` - The actual state of the activation button press.
  * Returns
   * _none_

The Toggle Method is used to enable or disable the pointer renderer.

#### ToggleInteraction/1

  > `public virtual void ToggleInteraction(bool state)`

  * Parameters
   * `bool state` - If true then the object interactor will be enabled.
  * Returns
   * _none_

The ToggleInteraction method is used to enable or disable the controller extension interactions.

#### UpdateRenderer/0

  > `public virtual void UpdateRenderer()`

  * Parameters
   * _none_
  * Returns
   * _none_

The UpdateRenderer method is used to run an Update routine on the pointer.

#### GetDestinationHit/0

  > `public virtual RaycastHit GetDestinationHit()`

  * Parameters
   * _none_
  * Returns
   * `RaycastHit` - The RaycastHit containing the information where the pointer is hitting.

The GetDestinationHit method is used to get the RaycastHit of the pointer destination.

#### ValidPlayArea/0

  > `public virtual bool ValidPlayArea()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if there is a valid play area and no collisions. Returns false if there is no valid play area or there is but with a collision detected.

The ValidPlayArea method is used to determine if there is a valid play area and if it has had any collisions.

---

## Straight Pointer Renderer (VRTK_StraightPointerRenderer)
 > extends [VRTK_BasePointerRenderer](#base-pointer-renderer-vrtk_basepointerrenderer)

### Overview

The Straight Pointer Renderer emits a coloured beam from the end of the object it is attached to and simulates a laser beam.

It can be useful for pointing to objects within a scene and it can also determine the object it is pointing at and the distance the object is from the controller the beam is being emitted from.

### Inspector Parameters

 * **Maximum Length:** The maximum length the pointer tracer can reach.
 * **Scale Factor:** The scale factor to scale the pointer tracer object by.
 * **Cursor Scale Multiplier:** The scale multiplier to scale the pointer cursor object by in relation to the `Scale Factor`.
 * **Cursor Match Target Rotation:** The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.
 * **Cursor Distance Rescale:** Rescale the cursor proportionally to the distance from the tracer origin.
 * **Custom Tracer:** A custom game object to use as the appearance for the pointer tracer. If this is empty then a Box primitive will be created and used.
 * **Custom Cursor:** A custom game object to use as the appearance for the pointer cursor. If this is empty then a Sphere primitive will be created and used.

### Class Methods

#### UpdateRenderer/0

  > `public override void UpdateRenderer()`

  * Parameters
   * _none_
  * Returns
   * _none_

The UpdateRenderer method is used to run an Update routine on the pointer.

### Example

`VRTK/Examples/003_Controller_SimplePointer` shows the simple pointer in action and code examples of how the events are utilised and listened to can be viewed in the script `VRTK/Examples/Resources/Scripts/VRTK_ControllerPointerEvents_ListenerExample.cs`

---

## Bezier Pointer Renderer (VRTK_BezierPointerRenderer)
 > extends [VRTK_BasePointerRenderer](#base-pointer-renderer-vrtk_basepointerrenderer)

### Overview

The Bezier Pointer Renderer emits a curved line (made out of game objects) from the end of the attached object to a point on a ground surface (at any height).

It is more useful than the Simple Pointer Renderer for traversing objects of various heights as the end point can be curved on top of objects that are not visible to the user.

> The bezier curve generation code is in another script located at `VRTK/Scripts/Internal/VRTK_CurveGenerator.cs` and was heavily inspired by the tutorial and code from [Catlike Coding](http://catlikecoding.com/unity/tutorials/curves-and-splines/).

### Inspector Parameters

 * **Maximum Length:** The maximum length of the projected forward beam.
 * **Tracer Density:** The number of items to render in the bezier curve tracer beam. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.
 * **Cursor Radius:** The size of the ground cursor. This number also affects the size of the objects in the bezier curve tracer beam. The larger the radius, the larger the objects will be.
 * **Height Limit Angle:** The maximum angle in degrees of the origin before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.
 * **Curve Offset:** The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.
 * **Rescale Tracer:** Rescale each tracer element according to the length of the Bezier curve.
 * **Cursor Match Target Rotation:** The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.
 * **Collision Check Frequency:** The number of points along the bezier curve to check for an early beam collision. Useful if the bezier curve is appearing to clip through teleport locations. 0 won't make any checks and it will be capped at `Pointer Density`. The higher the number, the more CPU intensive the checks become.
 * **Custom Tracer:** A custom game object to use as the appearance for the pointer tracer. If this is empty then a collection of Sphere primitives will be created and used.
 * **Custom Cursor:** A custom game object to use as the appearance for the pointer cursor. If this is empty then a Cylinder primitive will be created and used.
 * **Valid Location Object:** A custom game object can be applied here to appear only if the location is valid.
 * **Invalid Location Object:** A custom game object can be applied here to appear only if the location is invalid.

### Class Methods

#### UpdateRenderer/0

  > `public override void UpdateRenderer()`

  * Parameters
   * _none_
  * Returns
   * _none_

The UpdateRenderer method is used to run an Update routine on the pointer.

### Example

`VRTK/Examples/009_Controller_BezierPointer` is used in conjunction with the Height Adjust Teleporter shows how it is possible to traverse different height objects using the curved pointer without needing to see the top of the object.

`VRTK/Examples/036_Controller_CustomCompoundPointer' shows how to display an object (a teleport beam) only if the teleport location is valid, and can create an animated trail along the tracer curve.

---

# Locomotion (VRTK/Scripts/Locomotion)

A collection of scripts that provide varying methods of moving the user around the scene.

 * [Basic Teleport](#basic-teleport-vrtk_basicteleport)
 * [Height Adjust Teleport](#height-adjust-teleport-vrtk_heightadjustteleport)
 * [Dash Teleport](#dash-teleport-vrtk_dashteleport)
 * [Teleport Disable On Headset Collision](#teleport-disable-on-headset-collision-vrtk_teleportdisableonheadsetcollision)
 * [Teleport Disable On Controller Obscured](#teleport-disable-on-controller-obscured-vrtk_teleportdisableoncontrollerobscured)
 * [Touchpad Control](#touchpad-control-vrtk_touchpadcontrol)
 * [Move In Place](#move-in-place-vrtk_moveinplace)
 * [Player Climb](#player-climb-vrtk_playerclimb)
 * [Room Extender](#room-extender-vrtk_roomextender)

---

## Basic Teleport (VRTK_BasicTeleport)

### Overview

The basic teleporter updates the user's x/z position in the game world to the position of a Base Pointer's tip location which is set via the `DestinationMarkerSet` event.

The y position is never altered so the basic teleporter cannot be used to move up and down game objects as it only allows for travel across a flat plane.

### Inspector Parameters

 * **Blink Transition Speed:** The fade blink speed can be changed on the basic teleport script to provide a customised teleport experience. Setting the speed to 0 will mean no fade blink effect is present.
 * **Distance Blink Delay:** A range between 0 and 32 that determines how long the blink transition will stay blacked out depending on the distance being teleported. A value of 0 will not delay the teleport blink effect over any distance, a value of 32 will delay the teleport blink fade in even when the distance teleported is very close to the original position. This can be used to simulate time taking longer to pass the further a user teleports. A value of 16 provides a decent basis to simulate this to the user.
 * **Headset Position Compensation:** If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether destination targets will be acted upon by the Teleporter.
 * **Nav Mesh Limit Distance:** The max distance the teleport destination can be outside the nav mesh to be considered valid. If a value of `0` is given then the nav mesh restrictions will be ignored.

### Class Events

 * `Teleporting` - Emitted when the teleport process has begun.
 * `Teleported` - Emitted when the teleport process has successfully completed.

### Unity Events

Adding the `VRTK_BasicTeleport_UnityEvents` component to `VRTK_BasicTeleport` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnTeleporting` - Emits the Teleporting class event.
 * `OnTeleported` - Emits the Teleported class event.

### Event Payload

 * `float distance` - The distance between the origin and the collided destination.
 * `Transform target` - The Transform of the collided destination object.
 * `RaycastHit raycastHit` - The optional RaycastHit generated from when the ray collided.
 * `Vector3 destinationPosition` - The world position of the destination marker.
 * `bool forceDestinationPosition` - If true then the given destination position should not be altered by anything consuming the payload.
 * `bool enableTeleport` - Whether the destination set event should trigger teleport.
 * `uint controllerIndex` - The optional index of the controller emitting the beam.

### Class Methods

#### InitDestinationSetListener/2

  > `public virtual void InitDestinationSetListener(GameObject markerMaker, bool register)`

  * Parameters
   * `GameObject markerMaker` - The game object that is used to generate destination marker events, such as a controller.
   * `bool register` - Determines whether to register or unregister the listeners.
  * Returns
   * _none_

The InitDestinationSetListener method is used to register the teleport script to listen to events from the given game object that is used to generate destination markers. Any destination set event emitted by a registered game object will initiate the teleport to the given destination location.

#### ToggleTeleportEnabled/1

  > `public virtual void ToggleTeleportEnabled(bool state)`

  * Parameters
   * `bool state` - Toggles whether the teleporter is enabled or disabled.
  * Returns
   * _none_

The ToggleTeleportEnabled method is used to determine whether the teleporter will initiate a teleport on a destination set event, if the state is true then the teleporter will work as normal, if the state is false then the teleporter will not be operational.

#### ValidLocation/2

  > `public virtual bool ValidLocation(Transform target, Vector3 destinationPosition)`

  * Parameters
   * `Transform target` - The Transform that the destination marker is touching.
   * `Vector3 destinationPosition` - The position in world space that is the destination.
  * Returns
   * `bool` - Returns true if the target is a valid location.

The ValidLocation method determines if the given target is a location that can be teleported to

### Example

`VRTK/Examples/004_CameraRig_BasicTeleport` uses the `VRTK_SimplePointer` script on the Controllers to initiate a laser pointer by pressing the `Touchpad` on the controller and when the laser pointer is deactivated (release the `Touchpad`) then the user is teleported to the location of the laser pointer tip as this is where the pointer destination marker position is set to.

---

## Height Adjust Teleport (VRTK_HeightAdjustTeleport)
 > extends [VRTK_BasicTeleport](#basic-teleport-vrtk_basicteleport)

### Overview

The height adjust teleporter extends the basic teleporter and allows for the y position of the user's position to be altered based on whether the teleport location is on top of another object.

### Inspector Parameters

 * **Layers To Ignore:** The layers to ignore when raycasting to find floors.

### Example

`VRTK/Examples/007_CameraRig_HeightAdjustTeleport` has a collection of varying height objects that the user can either walk up and down or use the laser pointer to climb on top of them.

`VRTK/Examples/010_CameraRig_TerrainTeleporting` shows how the teleportation of a user can also traverse terrain colliders.

`VRTK/Examples/020_CameraRig_MeshTeleporting` shows how the teleportation of a user can also traverse mesh colliders.

---

## Dash Teleport (VRTK_DashTeleport)
 > extends [VRTK_HeightAdjustTeleport](#height-adjust-teleport-vrtk_heightadjustteleport)

### Overview

The dash teleporter extends the height adjust teleporter and allows to have the user's position dashing to a new teleport location.

The basic principle is to dash for a very short amount of time, to avoid sim sickness. The default value is 100 miliseconds. This value is fixed for all normal and longer distances. When the distances get very short the minimum speed is clamped to 50 mps, so the dash time becomes even shorter.

The minimum distance for the fixed time dash is determined by the minSpeed and normalLerpTime values, if you want to always lerp with a fixed mps speed instead, set the normalLerpTime to a high value. Right before the teleport a capsule is cast towards the target and registers all colliders blocking the way. These obstacles are then broadcast in an event so that for example their gameobjects or renderers can be turned off while the dash is in progress.

### Inspector Parameters

 * **Normal Lerp Time:** The fixed time it takes to dash to a new position.
 * **Min Speed Mps:** The minimum speed for dashing in meters per second.
 * **Capsule Top Offset:** The Offset of the CapsuleCast above the camera.
 * **Capsule Bottom Offset:** The Offset of the CapsuleCast below the camera.
 * **Capsule Radius:** The radius of the CapsuleCast.

### Class Events

 * `WillDashThruObjects` - Emitted when the CapsuleCast towards the target has found that obstacles are in the way.
 * `DashedThruObjects` - Emitted when obstacles have been crossed and the dash has ended.

### Unity Events

Adding the `VRTK_DashTeleport_UnityEvents` component to `VRTK_DashTeleport` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnWillDashThruObjects` - Emits the WillDashThruObjects class event.
 * `OnDashedThruObjects` - Emits the DashedThruObjects class event.

### Event Payload

 * `RaycastHit[] hits` - An array of objects that the CapsuleCast has collided with.

### Example

`SteamVR_Unity_Toolkit/Examples/038_CameraRig_DashTeleport` shows how to turn off the mesh renderers of objects that are in the way during the dash.

---

## Teleport Disable On Headset Collision (VRTK_TeleportDisableOnHeadsetCollision)

### Overview

The purpose of the Teleport Disable On Headset Collision script is to detect when the headset is colliding with a valid object and prevent teleportation from working. This is to ensure that if a user is clipping their head into a wall then they cannot teleport to an area beyond the wall.

---

## Teleport Disable On Controller Obscured (VRTK_TeleportDisableOnControllerObscured)

### Overview

The purpose of the Teleport Disable On Controller Obscured script is to detect when the headset does not have a line of sight to the controllers and prevent teleportation from working. This is to ensure that if a user is clipping their controllers through a wall then they cannot teleport to an area beyond the wall.

---

## Touchpad Control (VRTK_TouchpadControl)

### Overview

The ability to control an object with the touchpad based on the position of the finger on the touchpad axis.

The Touchpad Control script forms the stub to allow for pre-defined actions to execute when the touchpad axis changes.

This is enabled by the Touchpad Control script emitting an event each time the X axis and Y Axis on the touchpad change and the corresponding Touchpad Control Action registers with the appropriate axis event. This means that multiple Touchpad Control Actions can be triggered per axis change.

This script is placed on the Script Alias of the Controller that is required to be affected by changes in the touchpad.

If the controlled object is the play area and `VRTK_BodyPhysics` is also available, then additional logic is processed when the user is falling such as preventing the touchpad control from affecting a falling user.

### Inspector Parameters

 * **Primary Activation Button:** An optional button that has to be engaged to allow the touchpad control to activate.
 * **Action Modifier Button:** An optional button that when engaged will activate the modifier on the touchpad control action.
 * **Device For Direction:** The direction that will be moved in is the direction of this device.
 * **Disable Other Controls On Active:** If this is checked then whenever the touchpad axis on the attached controller is being changed, all other touchpad control scripts on other controllers will be disabled.
 * **Affect On Falling:** If a `VRTK_BodyPhysics` script is present and this is checked, then the touchpad control will affect the play area whilst it is falling.
 * **Control Override Object:** An optional game object to apply the touchpad control to. If this is blank then the PlayArea will be controlled.

### Class Variables

 * `public enum DirectionDevices` - Devices for providing direction.
  * `Headset` - The headset device.
  * `LeftController` - The left controller device.
  * `RightController` - The right controller device.
  * `ControlledObject` - The controlled object.

### Class Events

 * `XAxisChanged` - Emitted when the touchpad X Axis Changes.
 * `YAxisChanged` - Emitted when the touchpad Y Axis Changes.

### Unity Events

Adding the `VRTK_TouchpadControl_UnityEvents` component to `VRTK_TouchpadControl` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnXAxisChanged` - Emits the XAxisChanged class event.
 * `OnYAxisChanged` - Emits the YAxisChanged class event.

### Event Payload

 * `GameObject controlledGameObject` - The GameObject that is going to be affected.
 * `Transform directionDevice` - The device that is used for the direction.
 * `Vector3 axisDirection` - The axis that is being affected from the touchpad.
 * `Vector3 axis` - The value of the current touchpad touch point based across the axis direction.
 * `float deadzone` - The value of the deadzone based across the axis direction.
 * `bool currentlyFalling` - Whether the controlled GameObject is currently falling.
 * `bool modifierActive` - Whether the modifier button is pressed.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

---

## Move In Place (VRTK_MoveInPlace)

### Overview

Move In Place allows the user to move the play area by calculating the y-movement of the user's headset and/or controllers. The user is propelled forward the more they are moving. This simulates moving in game by moving in real life.

> This locomotion method is based on Immersive Movement, originally created by Highsight.

### Inspector Parameters

 * **Left Controller:** If this is checked then the left controller touchpad will be enabled to move the play area.
 * **Right Controller:** If this is checked then the right controller touchpad will be enabled to move the play area.
 * **Engage Button:** Select which button to hold to engage Move In Place.
 * **Control Options:** Select which trackables are used to determine movement.
 * **Speed Scale:** Lower to decrease speed, raise to increase.
 * **Max Speed:** The max speed the user can move in game units. (If 0 or less, max speed is uncapped)
 * **Deceleration:** The speed in which the play area slows down to a complete stop when the user is no longer pressing the engage button. This deceleration effect can ease any motion sickness that may be suffered.
 * **Falling Deceleration:** The speed in which the play area slows down to a complete stop when the user is falling.
 * **Direction Method:** How the user's movement direction will be determined.  The Gaze method tends to lead to the least motion sickness.  Smart decoupling is still a Work In Progress.
 * **Smart Decouple Threshold:** The degree threshold that all tracked objects (controllers, headset) must be within to change direction when using the Smart Decoupling Direction Method.
 * **Sensitivity:** The maximum amount of movement required to register in the virtual world.  Decreasing this will increase acceleration, and vice versa.

### Class Variables

 * `public enum ControlOptions` - Options for testing if a play space fall is valid.
  * `HeadsetAndControllers` - Track both headset and controllers for movement calculations.
  * `ControllersOnly` - Track only the controllers for movement calculations.
  * `HeadsetOnly` - Track only headset for movement caluclations.
 * `public enum DirectionalMethod` - Options for which method is used to determine player direction while moving.
  * `Gaze` - Player will always move in the direction they are currently looking.
  * `ControllerRotation` - Player will move in the direction that the controllers are pointing (averaged).
  * `DumbDecoupling` - Player will move in the direction they were first looking when they engaged Move In Place.
  * `SmartDecoupling` - Player will move in the direction they are looking only if their headset point the same direction as their controllers.

### Class Methods

#### SetControlOptions/1

  > `public void SetControlOptions(ControlOptions givenControlOptions)`

  * Parameters
   * `ControlOptions givenControlOptions` - The control options to set the current control options to.
  * Returns
   * _none_

Set the control options and modify the trackables to match.

#### GetMovementDirection/0

  > `public Vector3 GetMovementDirection()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - Returns a vector representing the player's current movement direction.

The GetMovementDirection method will return the direction the player is moving.

#### GetSpeed/0

  > `public float GetSpeed()`

  * Parameters
   * _none_
  * Returns
   * `float` - Returns a float representing the player's current movement speed.

The GetSpeed method will return the current speed the player is moving at.

### Example

`VRTK/Examples/042_CameraRig_MoveInPlace` demonstrates how the user can move and traverse colliders by either swinging the controllers in a walking fashion or by running on the spot utilisng the head bob for movement.

---

## Player Climb (VRTK_PlayerClimb)

### Overview

The Player Climb allows player movement based on grabbing of `VRTK_InteractableObject` objects that have a `Climbable` grab attach script. Because it works by grabbing, each controller should have a `VRTK_InteractGrab` and `VRTK_InteractTouch` component attached.

### Inspector Parameters

 * **Use Player Scale:** Will scale movement up and down based on the player transform's scale.

### Class Events

 * `PlayerClimbStarted` - Emitted when player climbing has started.
 * `PlayerClimbEnded` - Emitted when player climbing has ended.

### Unity Events

Adding the `VRTK_PlayerClimb_UnityEvents` component to `VRTK_PlayerClimb` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnPlayerClimbStarted` - Emits the PlayerClimbStarted class event.
 * `OnPlayerClimbEnded` - Emits the PlayerClimbEnded class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller doing the interaction.
 * `GameObject target` - The GameObject of the interactable object that is being interacted with by the controller.

### Example

`VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with player climbing. There are many different examples showing how the same system can be used in unique ways.

---

## Room Extender (VRTK_RoomExtender)

### Overview

This script allows the playArea to move with the user. The play area is only moved when at the edge of a defined circle.

There is an additional script `VRTK_RoomExtender_PlayAreaGizmo` which can be attached alongside to visualize the extended playArea within the Editor.

### Inspector Parameters

 * **Movement Function:** This determines the type of movement used by the extender.
 * **Additional Movement Enabled:** This is the a public variable to enable the additional movement. This can be used in other scripts to toggle the play area movement.
 * **Additional Movement Enabled On Button Press:** This configures the controls of the RoomExtender. If this is true then the touchpad needs to be pressed to enable it. If this is false then it is disabled by pressing the touchpad.
 * **Additional Movement Multiplier:** This is the factor by which movement at the edge of the circle is amplified. 0 is no movement of the play area. Higher values simulate a bigger play area but may be too uncomfortable.
 * **Head Zone Radius:** This is the size of the circle in which the playArea is not moved and everything is normal. If it is to low it becomes uncomfortable when crouching.
 * **Debug Transform:** This transform visualises the circle around the user where the play area is not moved. In the demo scene this is a cylinder at floor level. Remember to turn of collisions.

### Class Variables

 * `public enum MovementFunction` - Movement methods.
  * `Nonlinear` - Moves the head with a non-linear drift movement.
  * `LinearDirect` - Moves the headset in a direct linear movement.

### Example

`VRTK/Examples/028_CameraRig_RoomExtender` shows how the RoomExtender script is controlled by a VRTK_RoomExtender_Controller Example script located at both controllers. Pressing the `Touchpad` on the controller activates the Room Extender. The Additional Movement Multiplier is changed based on the touch distance to the centre of the touchpad.

---

# Touchpad Control Actions (VRTK/Scripts/Locomotion/TouchpadControlActions)

This directory contains scripts that are used to provide different actions when using Touchpad Control.

 * [Base Touchpad Control Action](#base-touchpad-control-action-vrtk_basetouchpadcontrolaction)
 * [Slide Touchpad Control Action](#slide-touchpad-control-action-vrtk_slidetouchpadcontrolaction)
 * [Rotate Touchpad Control Action](#rotate-touchpad-control-action-vrtk_rotatetouchpadcontrolaction)
 * [Snap Rotate Touchpad Control Action](#snap-rotate-touchpad-control-action-vrtk_snaprotatetouchpadcontrolaction)
 * [Warp Touchpad Control Action](#warp-touchpad-control-action-vrtk_warptouchpadcontrolaction)

---

## Base Touchpad Control Action (VRTK_BaseTouchpadControlAction)

### Overview

The Base Touchpad Control Action script is an abstract class that all touchpad control action scripts inherit.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Inspector Parameters

 * **Touchpad Control Script:** The Touchpad Control script to receive axis change events from.
 * **Listen On Axis Change:** Determines which Touchpad Control Axis event to listen to.

---

## Slide Touchpad Control Action (VRTK_SlideTouchpadControlAction)
 > extends [VRTK_BaseTouchpadControlAction](#base-touchpad-control-action-vrtk_basetouchpadcontrolaction)

### Overview

The Slide Touchpad Control Action script is used to slide the controlled GameObject around the scene when changing the touchpad axis.

The effect is a smooth sliding motion in forward and sideways directions to simulate touchpad walking.

### Inspector Parameters

 * **Maximum Speed:** The maximum speed the controlled object can be moved in based on the position of the touchpad axis.
 * **Deceleration:** The rate of speed deceleration when the touchpad is no longer being touched.
 * **Falling Deceleration:** The rate of speed deceleration when the touchpad is no longer being touched and the object is falling.
 * **Speed Multiplier:** The speed multiplier to be applied when the modifier button is pressed.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Slide Touchpad Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Slide` control script active.

---

## Rotate Touchpad Control Action (VRTK_RotateTouchpadControlAction)
 > extends [VRTK_BaseTouchpadControlAction](#base-touchpad-control-action-vrtk_basetouchpadcontrolaction)

### Overview

The Rotate Touchpad Control Action script is used to rotate the controlled GameObject around the up vector when changing the touchpad axis.

The effect is a smooth rotation to simulate turning.

### Inspector Parameters

 * **Maximum Rotation Speed:** The maximum speed the controlled object can be rotated based on the position of the touchpad axis.
 * **Rotation Multiplier:** The rotation multiplier to be applied when the modifier button is pressed.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Rotate Touchpad Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Rotate` control script active.

---

## Snap Rotate Touchpad Control Action (VRTK_SnapRotateTouchpadControlAction)
 > extends [VRTK_BaseTouchpadControlAction](#base-touchpad-control-action-vrtk_basetouchpadcontrolaction)

### Overview

The Snap Rotate Touchpad Control Action script is used to snap rotate the controlled GameObject around the up vector when changing the touchpad axis.

The effect is a immediate snap rotation to quickly face in a new direction.

### Inspector Parameters

 * **Angle Per Snap:** The angle to rotate for each snap.
 * **Angle Multiplier:** The snap angle multiplier to be applied when the modifier button is pressed.
 * **Snap Delay:** The amount of time required to pass before another snap rotation can be carried out.
 * **Blink Transition Speed:** The speed for the headset to fade out and back in. Having a blink between rotations can reduce nausia.
 * **Axis Threshold:** The threshold the listened axis needs to exceed before the action occurs. This can be used to limit the snap rotate to a single axis direction (e.g. pull down to flip rotate). The threshold is ignored if it is 0.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Snap Rotate Touchpad Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Snap Rotate` control script active.

---

## Warp Touchpad Control Action (VRTK_WarpTouchpadControlAction)
 > extends [VRTK_BaseTouchpadControlAction](#base-touchpad-control-action-vrtk_basetouchpadcontrolaction)

### Overview

The Warp Touchpad Control Action script is used to warp the controlled GameObject a given distance when changing the touchpad axis.

The effect is a immediate snap to a new position in the given direction.

### Inspector Parameters

 * **Warp Distance:** The distance to warp in the facing direction.
 * **Warp Multiplier:** The multiplier to be applied to the warp when the modifier button is pressed.
 * **Warp Delay:** The amount of time required to pass before another warp can be carried out.
 * **Floor Height Tolerance:** The height different in floor allowed to be a valid warp.
 * **Blink Transition Speed:** The speed for the headset to fade out and back in. Having a blink between warps can reduce nausia.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Warp Touchpad Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Warp` control script active.

---

# Interactions (VRTK/Scripts/Interactions)

A collection of scripts that provide the ability to interact with game objects with the controllers.

 * [Controller Events](#controller-events-vrtk_controllerevents)
 * [Controller Actions](#controller-actions-vrtk_controlleractions)
 * [Interactable Object](#interactable-object-vrtk_interactableobject)
 * [Interact Touch](#interact-touch-vrtk_interacttouch)
 * [Interact Grab](#interact-grab-vrtk_interactgrab)
 * [Interact Use](#interact-use-vrtk_interactuse)
 * [Interact Haptics](#interact-haptics-vrtk_interacthaptics)
 * [Interact Controller Appearance](#interact-controller-appearance-vrtk_interactcontrollerappearance)
 * [Object Auto Grab](#object-auto-grab-vrtk_objectautograb)

---

## Controller Events (VRTK_ControllerEvents)

### Overview

The Controller Events script deals with events that the game controller is sending out.

The Controller Events script requires the Controller Mapper script on the same GameObject and provides event listeners for every button press on the controller (excluding the System Menu button as this cannot be overridden and is always used by Steam).

When a controller button is pressed, the script emits an event to denote that the button has been pressed which allows other scripts to listen for this event without needing to implement any controller logic. When a controller button is released, the script also emits an event denoting that the button has been released.

The script also has a public boolean pressed state for the buttons to allow the script to be queried by other scripts to check if a button is being held down.

### Inspector Parameters

 * **Grab Toggle Button:** The button to use for the action of grabbing game objects.
 * **Use Toggle Button:** The button to use for the action of using game objects.
 * **Menu Toggle Button:** The button to use for the action of bringing up an in-game menu.
 * **Axis Fidelity:** The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.
 * **Trigger Click Threshold:** The level on the trigger axis to reach before a click is registered.
 * **Grip Click Threshold:** The level on the grip axis to reach before a click is registered.

### Class Variables

 * `public enum ButtonAlias` - Button types
  * `Undefined` - No button specified
  * `Trigger_Hairline` - The trigger is squeezed past the current hairline threshold.
  * `Trigger_Touch` - The trigger is squeezed a small amount.
  * `Trigger_Press` - The trigger is squeezed about half way in.
  * `Trigger_Click` - The trigger is squeezed all the way down.
  * `Grip_Hairline` - The grip is squeezed past the current hairline threshold.
  * `Grip_Touch` - The grip button is touched.
  * `Grip_Press` - The grip button is pressed.
  * `Grip_Click` - The grip button is pressed all the way down.
  * `Touchpad_Touch` - The touchpad is touched (without pressing down to click).
  * `Touchpad_Press` - The touchpad is pressed (to the point of hearing a click).
  * `Button_One_Touch` - The button one is touched.
  * `Button_One_Press` - The button one is pressed.
  * `Button_Two_Touch` - The button one is touched.
  * `Button_Two_Press` - The button one is pressed.
  * `Start_Menu_Press` - The button one is pressed.
 * `public bool triggerPressed` - This will be true if the trigger is squeezed about half way in. Default: `false`
 * `public bool triggerTouched` - This will be true if the trigger is squeezed a small amount. Default: `false`
 * `public bool triggerHairlinePressed` - This will be true if the trigger is squeezed a small amount more from any previous squeeze on the trigger. Default: `false`
 * `public bool triggerClicked` - This will be true if the trigger is squeezed all the way down. Default: `false`
 * `public bool triggerAxisChanged` - This will be true if the trigger has been squeezed more or less. Default: `false`
 * `public bool gripPressed` - This will be true if the grip is squeezed about half way in. Default: `false`
 * `public bool gripTouched` - This will be true if the grip is touched. Default: `false`
 * `public bool gripHairlinePressed` - This will be true if the grip is squeezed a small amount more from any previous squeeze on the grip. Default: `false`
 * `public bool gripClicked` - This will be true if the grip is squeezed all the way down. Default: `false`
 * `public bool gripAxisChanged` - This will be true if the grip has been squeezed more or less. Default: `false`
 * `public bool touchpadPressed` - This will be true if the touchpad is held down. Default: `false`
 * `public bool touchpadTouched` - This will be true if the touchpad is being touched. Default: `false`
 * `public bool touchpadAxisChanged` - This will be true if the touchpad touch position has changed. Default: `false`
 * `public bool buttonOnePressed` - This will be true if button one is held down. Default: `false`
 * `public bool buttonOneTouched` - This will be true if button one is being touched. Default: `false`
 * `public bool buttonTwoPressed` - This will be true if button two is held down. Default: `false`
 * `public bool buttonTwoTouched` - This will be true if button two is being touched. Default: `false`
 * `public bool startMenuPressed` - This will be true if start menu is held down. Default: `false`
 * `public bool pointerPressed` - This will be true if the button aliased to the pointer is held down. Default: `false`
 * `public bool grabPressed` - This will be true if the button aliased to the grab is held down. Default: `false`
 * `public bool usePressed` - This will be true if the button aliased to the use is held down. Default: `false`
 * `public bool uiClickPressed` - This will be true if the button aliased to the UI click is held down. Default: `false`
 * `public bool menuPressed` - This will be true if the button aliased to the menu is held down. Default: `false`

### Class Events

 * `TriggerPressed` - Emitted when the trigger is squeezed about half way in.
 * `TriggerReleased` - Emitted when the trigger is released under half way.
 * `TriggerTouchStart` - Emitted when the trigger is squeezed a small amount.
 * `TriggerTouchEnd` - Emitted when the trigger is no longer being squeezed at all.
 * `TriggerHairlineStart` - Emitted when the trigger is squeezed past the current hairline threshold.
 * `TriggerHairlineEnd` - Emitted when the trigger is released past the current hairline threshold.
 * `TriggerClicked` - Emitted when the trigger is squeezed all the way down.
 * `TriggerUnclicked` - Emitted when the trigger is no longer being held all the way down.
 * `TriggerAxisChanged` - Emitted when the amount of squeeze on the trigger changes.
 * `GripPressed` - Emitted when the grip is squeezed about half way in.
 * `GripReleased` - Emitted when the grip is released under half way.
 * `GripTouchStart` - Emitted when the grip is squeezed a small amount.
 * `GripTouchEnd` - Emitted when the grip is no longer being squeezed at all.
 * `GripHairlineStart` - Emitted when the grip is squeezed past the current hairline threshold.
 * `GripHairlineEnd` - Emitted when the grip is released past the current hairline threshold.
 * `GripClicked` - Emitted when the grip is squeezed all the way down.
 * `GripUnclicked` - Emitted when the grip is no longer being held all the way down.
 * `GripAxisChanged` - Emitted when the amount of squeeze on the grip changes.
 * `TouchpadPressed` - Emitted when the touchpad is pressed (to the point of hearing a click).
 * `TouchpadReleased` - Emitted when the touchpad has been released after a pressed state.
 * `TouchpadTouchStart` - Emitted when the touchpad is touched (without pressing down to click).
 * `TouchpadTouchEnd` - Emitted when the touchpad is no longer being touched.
 * `TouchpadAxisChanged` - Emitted when the touchpad is being touched in a different location.
 * `ButtonOneTouchStart` - Emitted when button one is touched.
 * `ButtonOneTouchEnd` - Emitted when button one is no longer being touched.
 * `ButtonOnePressed` - Emitted when button one is pressed.
 * `ButtonOneReleased` - Emitted when button one is released.
 * `ButtonTwoTouchStart` - Emitted when button two is touched.
 * `ButtonTwoTouchEnd` - Emitted when button two is no longer being touched.
 * `ButtonTwoPressed` - Emitted when button two is pressed.
 * `ButtonTwoReleased` - Emitted when button two is released.
 * `StartMenuPressed` - Emitted when start menu is pressed.
 * `StartMenuReleased` - Emitted when start menu is released.
 * `AliasPointerOn` - Emitted when the pointer toggle alias button is pressed.
 * `AliasPointerOff` - Emitted when the pointer toggle alias button is released.
 * `AliasPointerSet` - Emitted when the pointer set alias button is released.
 * `AliasGrabOn` - Emitted when the grab toggle alias button is pressed.
 * `AliasGrabOff` - Emitted when the grab toggle alias button is released.
 * `AliasUseOn` - Emitted when the use toggle alias button is pressed.
 * `AliasUseOff` - Emitted when the use toggle alias button is released.
 * `AliasMenuOn` - Emitted when the menu toggle alias button is pressed.
 * `AliasMenuOff` - Emitted when the menu toggle alias button is released.
 * `AliasUIClickOn` - Emitted when the UI click alias button is pressed.
 * `AliasUIClickOff` - Emitted when the UI click alias button is released.
 * `ControllerEnabled` - Emitted when the controller is enabled.
 * `ControllerDisabled` - Emitted when the controller is disabled.
 * `ControllerIndexChanged` - Emitted when the controller index changed.

### Unity Events

Adding the `VRTK_ControllerEvents_UnityEvents` component to `VRTK_ControllerEvents` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnTriggerPressed` - Emits the TriggerPressed class event.
 * `OnTriggerReleased` - Emits the TriggerReleased class event.
 * `OnTriggerTouchStart` - Emits the TriggerTouchStart class event.
 * `OnTriggerTouchEnd` - Emits the TriggerTouchEnd class event.
 * `OnTriggerHairlineStart` - Emits the TriggerHairlineStart class event.
 * `OnTriggerHairlineEnd` - Emits the TriggerHairlineEnd class event.
 * `OnTriggerClicked` - Emits the TriggerClicked class event.
 * `OnTriggerUnclicked` - Emits the TriggerUnclicked class event.
 * `OnTriggerAxisChanged` - Emits the TriggerAxisChanged class event.
 * `OnGripPressed` - Emits the GripPressed class event.
 * `OnGripReleased` - Emits the GripReleased class event.
 * `OnGripTouchStart` - Emits the GripTouchStart class event.
 * `OnGripTouchEnd` - Emits the GripTouchEnd class event.
 * `OnGripHairlineStart` - Emits the GripHairlineStart class event.
 * `OnGripHairlineEnd` - Emits the GripHairlineEnd class event.
 * `OnGripClicked` - Emits the GripClicked class event.
 * `OnGripUnclicked` - Emits the GripUnclicked class event.
 * `OnGripAxisChanged` - Emits the GripAxisChanged class event.
 * `OnTouchpadPressed` - Emits the TouchpadPressed class event.
 * `OnTouchpadReleased` - Emits the TouchpadReleased class event.
 * `OnTouchpadTouchStart` - Emits the TouchpadTouchStart class event.
 * `OnTouchpadTouchEnd` - Emits the TouchpadTouchEnd class event.
 * `OnTouchpadAxisChanged` - Emits the TouchpadAxisChanged class event.
 * `OnButtonOnePressed` - Emits the ButtonOnePressed class event.
 * `OnButtonOneReleased` - Emits the ButtonOneReleased class event.
 * `OnButtonOneTouchStart` - Emits the ButtonOneTouchStart class event.
 * `OnButtonOneTouchEnd` - Emits the ButtonOneTouchEnd class event.
 * `OnButtonTwoPressed` - Emits the ButtonTwoPressed class event.
 * `OnButtonTwoReleased` - Emits the ButtonTwoReleased class event.
 * `OnButtonTwoTouchStart` - Emits the ButtonTwoTouchStart class event.
 * `OnButtonTwoTouchEnd` - Emits the ButtonTwoTouchEnd class event.
 * `OnStartMenuPressed` - Emits the StartMenuPressed class event.
 * `OnStartMenuReleased` - Emits the StartMenuReleased class event.
 * `OnAliasPointerOn` - Emits the AliasPointerOn class event.
 * `OnAliasPointerOff` - Emits the AliasPointerOff class event.
 * `OnAliasPointerSet` - Emits the AliasPointerSet class event.
 * `OnAliasGrabOn` - Emits the AliasGrabOn class event.
 * `OnAliasGrabOff` - Emits the AliasGrabOff class event.
 * `OnAliasUseOn` - Emits the AliasUseOn class event.
 * `OnAliasUseOff` - Emits the AliasUseOff class event.
 * `OnAliasUIClickOn` - Emits the AliasMenuOn class event.
 * `OnAliasUIClickOff` - Emits the AliasMenuOff class event.
 * `OnAliasMenuOn` - Emits the AliasUIClickOn class event.
 * `OnAliasMenuOff` - Emits the AliasUIClickOff class event.
 * `OnControllerEnabled` - Emits the ControllerEnabled class event.
 * `OnControllerDisabled` - Emits the ControllerDisabled class event.
 * `OnControllerIndexChanged` - Emits the ControllerIndexChanged class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller that was used.
 * `float buttonPressure` - The amount of pressure being applied to the button pressed. `0f` to `1f`.
 * `Vector2 touchpadAxis` - The position the touchpad is touched at. `(0,0)` to `(1,1)`.
 * `float touchpadAngle` - The rotational position the touchpad is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.

### Class Methods

#### GetTouchpadAxis/0

  > `public Vector2 GetTouchpadAxis()`

  * Parameters
   * _none_
  * Returns
   * `Vector2` - A 2 dimensional vector containing the x and y position of where the touchpad is being touched. `(0,0)` to `(1,1)`.

The GetTouchpadAxis method returns the coordinates of where the touchpad is being touched and can be used for directional input via the touchpad. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.

#### GetTouchpadAxisAngle/0

  > `public float GetTouchpadAxisAngle()`

  * Parameters
   * _none_
  * Returns
   * `float` - A float representing the angle of where the touchpad is being touched. `0f` to `360f`.

The GetTouchpadAxisAngle method returns the angle of where the touchpad is currently being touched with the top of the touchpad being 0 degrees and the bottom of the touchpad being 180 degrees.

#### GetTriggerAxis/0

  > `public float GetTriggerAxis()`

  * Parameters
   * _none_
  * Returns
   * `float` - A float representing the amount of squeeze that is being applied to the trigger. `0f` to `1f`.

The GetTriggerAxis method returns a float that represents how much the trigger is being squeezed. This can be useful for using the trigger axis to perform high fidelity tasks or only activating the trigger press once it has exceeded a given press threshold.

#### GetGripAxis/0

  > `public float GetGripAxis()`

  * Parameters
   * _none_
  * Returns
   * `float` - A float representing the amount of squeeze that is being applied to the grip. `0f` to `1f`.

The GetGripAxis method returns a float that represents how much the grip is being squeezed. This can be useful for using the grip axis to perform high fidelity tasks or only activating the grip press once it has exceeded a given press threshold.

#### GetHairTriggerDelta/0

  > `public float GetHairTriggerDelta()`

  * Parameters
   * _none_
  * Returns
   * `float` - A float representing the difference in the trigger pressure from the hairline threshold start to current position.

The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.

#### GetHairGripDelta/0

  > `public float GetHairGripDelta()`

  * Parameters
   * _none_
  * Returns
   * `float` - A float representing the difference in the trigger pressure from the hairline threshold start to current position.

The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.

#### AnyButtonPressed/0

  > `public bool AnyButtonPressed()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if any of the controller buttons are currently being pressed.

The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.

#### IsButtonPressed/1

  > `public bool IsButtonPressed(ButtonAlias button)`

  * Parameters
   * `ButtonAlias button` - The button to check if it's being pressed.
  * Returns
   * `bool` - Is true if the button is being pressed.

The IsButtonPressed method takes a given button alias and returns a boolean whether that given button is currently being pressed or not.

#### SubscribeToButtonAliasEvent/3

  > `public void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)`

  * Parameters
   * `ButtonAlias givenButton` - The ButtonAlias to register the event on.
   * `bool startEvent` - If this is `true` then the start event related to the button is used (e.g. OnPress). If this is `false` then the end event related to the button is used (e.g. OnRelease).
   * `ControllerInteractionEventHandler callbackMethod` - The method to subscribe to the event.
  * Returns
   * _none_

The SubscribeToButtonAliasEvent method makes it easier to subscribe to a button event on either the start or end action. Upon the event firing, the given callback method is executed.

#### UnsubscribeToButtonAliasEvent/3

  > `public void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)`

  * Parameters
   * `ButtonAlias givenButton` - The ButtonAlias to unregister the event on.
   * `bool startEvent` - If this is `true` then the start event related to the button is used (e.g. OnPress). If this is `false` then the end event related to the button is used (e.g. OnRelease).
   * `ControllerInteractionEventHandler callbackMethod` - The method to unsubscribe from the event.
  * Returns
   * _none_

The UnsubscribeToButtonAliasEvent method makes it easier to unsubscribe to from button event on either the start or end action.

### Example

`VRTK/Examples/002_Controller_Events` shows how the events are utilised and listened to. The accompanying example script can be viewed in `VRTK/Examples/Resources/Scripts/VRTK_ControllerEvents_ListenerExample.cs`.

---

## Controller Actions (VRTK_ControllerActions)

### Overview

The Controller Actions script provides helper methods to deal with common controller actions. It deals with actions that can be done to the controller.

The highlighting of the controller is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.

### Inspector Parameters

 * **Model Element Paths:** A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.
  * The available model sub elements are:
    * `Body Model Path`: The overall shape of the controller.
    * `Trigger Model Path`: The model that represents the trigger button.
    * `Grip Left Model Path`: The model that represents the left grip button.
    * `Grip Right Model Path`: The model that represents the right grip button.
    * `Touchpad Model Path`: The model that represents the touchpad.
    * `Button One Model Path`: The model that represents button one.
    * `Button Two Model Path`: The model that represents button two.
    * `System Menu Model Path`: The model that represents the system menu button.  * `Start Menu Model Path`: The model that represents the start menu button.
 * **Element Highlighter Overrides:** A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.
  * The available model sub elements are:
    * `Body`: The highlighter to use on the overall shape of the controller.
    * `Trigger`: The highlighter to use on the trigger button.
    * `Grip Left`: The highlighter to use on the left grip button.
    * `Grip Right`: The highlighter to use on the  right grip button.
    * `Touchpad`: The highlighter to use on the touchpad.
    * `Button One`: The highlighter to use on button one.
    * `Button Two`: The highlighter to use on button two.
    * `System Menu`: The highlighter to use on the system menu button.  * `Start Menu`: The highlighter to use on the start menu button.

### Class Events

 * `ControllerModelVisible` - Emitted when the controller model is toggled to be visible.
 * `ControllerModelInvisible` - Emitted when the controller model is toggled to be invisible.

### Unity Events

Adding the `VRTK_ControllerActions_UnityEvents` component to `VRTK_ControllerActions` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnControllerModelVisible` - Emits the ControllerModelVisible class event.
 * `OnControllerModelInvisible` - Emits the ControllerModelInvisible class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller that was used.

### Class Methods

#### IsControllerVisible/0

  > `public bool IsControllerVisible()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the controller model has the renderers that are attached to it are enabled.

The IsControllerVisible method returns true if the controller is currently visible by whether the renderers on the controller are enabled.

#### ToggleControllerModel/2

  > `public virtual void ToggleControllerModel(bool state, GameObject grabbedChildObject)`

  * Parameters
   * `bool state` - The visibility state to toggle the controller to, `true` will make the controller visible - `false` will hide the controller model.
   * `GameObject grabbedChildObject` - If an object is being held by the controller then this can be passed through to prevent hiding the grabbed game object as well.
  * Returns
   * _none_

The ToggleControllerModel method is used to turn on or off the controller model by enabling or disabling the renderers on the object. It will also work for any custom controllers. It should also not disable any objects being held by the controller if they are a child of the controller object.

#### SetControllerOpacity/1

  > `public virtual void SetControllerOpacity(float alpha)`

  * Parameters
   * `float alpha` - The alpha level to apply to opacity of the controller object. `0f` to `1f`.
  * Returns
   * _none_

The SetControllerOpacity method allows the opacity of the controller model to be changed to make the controller more transparent. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.

#### HighlightControllerElement/3

  > `public virtual void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)`

  * Parameters
   * `GameObject element` - The element of the controller to apply the highlight to.
   * `Color? highlight` - The colour of the highlight.
   * `float fadeDuration` - The duration of fade from white to the highlight colour. Optional parameter defaults to `0f`.
  * Returns
   * _none_

The HighlightControllerElement method allows for an element of the controller to have its colour changed to simulate a highlighting effect of that element on the controller. It's useful for being able to draw a user's attention to a specific button on the controller.

#### UnhighlightControllerElement/1

  > `public virtual void UnhighlightControllerElement(GameObject element)`

  * Parameters
   * `GameObject element` - The element of the controller to remove the highlight from.
  * Returns
   * _none_

The UnhighlightControllerElement method is the inverse of the HighlightControllerElement method and resets the controller element to its original colour.

#### ToggleHighlightControllerElement/4

  > `public virtual void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the given element and `false` will remove the highlight from the given element.
   * `GameObject element` - The element of the controller to apply the highlight to.
   * `Color? highlight` - The colour of the highlight.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightControllerElement method is a shortcut method that makes it easier to highlight and unhighlight a controller element in a single method rather than using the HighlightControllerElement and UnhighlightControllerElement methods separately.

#### ToggleHighlightTrigger/3

  > `public virtual void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the trigger and `false` will remove the highlight from the trigger.
   * `Color? highlight` - The colour to highlight the trigger with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightTrigger method is a shortcut method that makes it easier to toggle the highlight state of the controller trigger element.

#### ToggleHighlightGrip/3

  > `public virtual void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the grip and `false` will remove the highlight from the grip.
   * `Color? highlight` - The colour to highlight the grip with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightGrip method is a shortcut method that makes it easier to toggle the highlight state of the controller grip element.

#### ToggleHighlightTouchpad/3

  > `public virtual void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the touchpad and `false` will remove the highlight from the touchpad.
   * `Color? highlight` - The colour to highlight the touchpad with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightTouchpad method is a shortcut method that makes it easier to toggle the highlight state of the controller touchpad element.

#### ToggleHighlightButtonOne/3

  > `public virtual void ToggleHighlightButtonOne(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on button one and `false` will remove the highlight from button one.
   * `Color? highlight` - The colour to highlight button one with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightButtonOne method is a shortcut method that makes it easier to toggle the highlight state of the button one controller element.

#### ToggleHighlightButtonTwo/3

  > `public virtual void ToggleHighlightButtonTwo(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on button two and `false` will remove the highlight from button two.
   * `Color? highlight` - The colour to highlight button two with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightButtonTwo method is a shortcut method that makes it easier to toggle the highlight state of the button two controller element.

#### ToggleHighlightStartMenu/3

  > `public virtual void ToggleHighlightStartMenu(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the start menu and `false` will remove the highlight from the start menu.
   * `Color? highlight` - The colour to highlight the start menu with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightStartMenu method is a shortcut method that makes it easier to toggle the highlight state of the start menu controller element.

#### ToggleHighlighBody/3

  > `public virtual void ToggleHighlighBody(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the body and `false` will remove the highlight from the body.
   * `Color? highlight` - The colour to highlight the body with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlighBody method is a shortcut method that makes it easier to toggle the highlight state of the controller body element.

#### ToggleHighlightController/3

  > `public virtual void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)`

  * Parameters
   * `bool state` - The highlight colour state, `true` will enable the highlight on the entire controller `false` will remove the highlight from the entire controller.
   * `Color? highlight` - The colour to highlight the entire controller with.
   * `float duration` - The duration of fade from white to the highlight colour.
  * Returns
   * _none_

The ToggleHighlightController method is a shortcut method that makes it easier to toggle the highlight state of the entire controller.

#### TriggerHapticPulse/1

  > `public virtual void TriggerHapticPulse(float strength)`

  * Parameters
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.

#### TriggerHapticPulse/3

  > `public virtual void TriggerHapticPulse(float strength, float duration, float pulseInterval)`

  * Parameters
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
   * `float duration` - The length of time the rumble should continue for.
   * `float pulseInterval` - The interval to wait between each haptic pulse.
  * Returns
   * _none_

The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.

#### InitaliseHighlighters/0

  > `public virtual void InitaliseHighlighters()`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitaliseHighlighters method sets up the highlighters on the controller model.

### Example

`VRTK/Examples/016_Controller_HapticRumble` demonstrates the ability to hide a controller model and make the controller vibrate for a given length of time at a given intensity.

`VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.

---

## Interactable Object (VRTK_InteractableObject)

### Overview

The Interactable Object script is attached to any game object that is required to be interacted with (e.g. via the controllers).

The basis of this script is to provide a simple mechanism for identifying objects in the game world that can be grabbed or used but it is expected that this script is the base to be inherited into a script with richer functionality.

The highlighting of an Interactable Object is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.

### Inspector Parameters

 * **Disable When Idle:** If this is checked then the interactable object script will be disabled when the object is not being interacted with. This will eliminate the potential number of calls the interactable objects make each frame.
 * **Touch Highlight Color:** The colour to highlight the object when it is touched. This colour will override any globally set colour (for instance on the `VRTK_InteractTouch` script).
 * **Allowed Touch Controllers:** Determines which controller can initiate a touch action.
 * **Is Grabbable:** Determines if the object can be grabbed.
 * **Hold Button To Grab:** If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.
 * **Stay Grabbed On Teleport:** If this is checked then the object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the object will be released when a teleport occurs.
 * **Valid Drop:** Determines in what situation the object can be dropped by the controller grab button.
 * **Grab Override Button:** If this is set to `Undefined` then the global grab alias button will grab the object, setting it to any other button will ensure the override button is used to grab this specific interactable object.
 * **Allowed Grab Controllers:** Determines which controller can initiate a grab action.
 * **Grab Attach Mechanic Script:** This determines how the grabbed item will be attached to the controller when it is grabbed. If one isn't provided then the first Grab Attach script on the GameObject will be used, if one is not found and the object is grabbable then a Fixed Joint Grab Attach script will be created at runtime.
 * **Secondary Grab Action Script:** The script to utilise when processing the secondary controller action on a secondary grab attempt. If one isn't provided then the first Secondary Controller Grab Action script on the GameObject will be used, if one is not found then no action will be taken on secondary grab.
 * **Is Usable:** Determines if the object can be used.
 * **Hold Button To Use:** If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.
 * **Use Only If Grabbed:** If this is checked the object can be used only if it is currently being grabbed.
 * **Pointer Activates Use Action:** If this is checked then when a Base Pointer beam (projected from the controller) hits the interactable object, if the object has `Hold Button To Use` unchecked then whilst the pointer is over the object it will run it's `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the pointer is deactivated. The world pointer will not throw the `Destination Set` event if it is affecting an interactable object with this setting checked as this prevents unwanted teleporting from happening when using an object with a pointer.
 * **Use Override Button:** If this is set to `Undefined` then the global use alias button will use the object, setting it to any other button will ensure the override button is used to use this specific interactable object.
 * **Allowed Use Controllers:** Determines which controller can initiate a use action.

### Class Variables

 * `public enum AllowedController` - Allowed controller type.
  * `Both` - Both controllers are allowed to interact.
  * `Left_Only` - Only the left controller is allowed to interact.
  * `Right_Only` - Only the right controller is allowed to interact.
 * `public enum ValidDropTypes` - The types of valid situations that the object can be released from grab.
  * `No_Drop` - The object cannot be dropped via the controller
  * `Drop_Anywhere` - The object can be dropped anywhere in the scene via the controller.
  * `Drop_ValidSnapDropZone` - The object can only be dropped when it is hovering over a valid snap drop zone.
 * `public int usingState` - The current using state of the object. `0` not being used, `1` being used. Default: `0`
 * `public bool isKinematic` - isKinematic is a pass through to the `isKinematic` getter/setter on the object's rigidbody component.

### Class Events

 * `InteractableObjectTouched` - Emitted when another object touches the current object.
 * `InteractableObjectUntouched` - Emitted when the other object stops touching the current object.
 * `InteractableObjectGrabbed` - Emitted when another object grabs the current object (e.g. a controller).
 * `InteractableObjectUngrabbed` - Emitted when the other object stops grabbing the current object.
 * `InteractableObjectUsed` - Emitted when another object uses the current object (e.g. a controller).
 * `InteractableObjectUnused` - Emitted when the other object stops using the current object.

### Unity Events

Adding the `VRTK_InteractableObject_UnityEvents` component to `VRTK_InteractableObject` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnTouch` - Emits the InteractableObjectTouched class event.
 * `OnUntouch` - Emits the InteractableObjectUntouched class event.
 * `OnGrab` - Emits the InteractableObjectGrabbed class event.
 * `OnUngrab` - Emits the InteractableObjectUngrabbed class event.
 * `OnUse` - Emits the InteractableObjectUsed class event.
 * `OnUnuse` - Emits the InteractableObjectUnused class event.

### Event Payload

 * `GameObject interactingObject` - The object that is initiating the interaction (e.g. a controller).

### Class Methods

#### IsTouched/0

  > `public bool IsTouched()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns `true` if the object is currently being touched.

The IsTouched method is used to determine if the object is currently being touched.

#### IsGrabbed/1

  > `public bool IsGrabbed(GameObject grabbedBy = null)`

  * Parameters
   * `GameObject grabbedBy` - An optional GameObject to check if the Interactable Object is grabbed by that specific GameObject. Defaults to `null`
  * Returns
   * `bool` - Returns `true` if the object is currently being grabbed.

The IsGrabbed method is used to determine if the object is currently being grabbed.

#### IsUsing/1

  > `public bool IsUsing(GameObject usedBy = null)`

  * Parameters
   * `GameObject usedBy` - An optional GameObject to check if the Interactable Object is used by that specific GameObject. Defaults to `null`
  * Returns
   * `bool` - Returns `true` if the object is currently being used.

The IsUsing method is used to determine if the object is currently being used.

#### StartTouching/1

  > `public virtual void StartTouching(GameObject currentTouchingObject)`

  * Parameters
   * `GameObject currentTouchingObject` - The game object that is currently touching this object.
  * Returns
   * _none_

The StartTouching method is called automatically when the object is touched initially. It is also a virtual method to allow for overriding in inherited classes.

#### StopTouching/1

  > `public virtual void StopTouching(GameObject previousTouchingObject)`

  * Parameters
   * `GameObject previousTouchingObject` - The game object that was previously touching this object.
  * Returns
   * _none_

The StopTouching method is called automatically when the object has stopped being touched. It is also a virtual method to allow for overriding in inherited classes.

#### Grabbed/1

  > `public virtual void Grabbed(GameObject currentGrabbingObject)`

  * Parameters
   * `GameObject currentGrabbingObject` - The game object that is currently grabbing this object.
  * Returns
   * _none_

The Grabbed method is called automatically when the object is grabbed initially. It is also a virtual method to allow for overriding in inherited classes.

#### Ungrabbed/1

  > `public virtual void Ungrabbed(GameObject previousGrabbingObject)`

  * Parameters
   * `GameObject previousGrabbingObject` - The game object that was previously grabbing this object.
  * Returns
   * _none_

The Ungrabbed method is called automatically when the object has stopped being grabbed. It is also a virtual method to allow for overriding in inherited classes.

#### StartUsing/1

  > `public virtual void StartUsing(GameObject currentUsingObject)`

  * Parameters
   * `GameObject currentUsingObject` - The game object that is currently using this object.
  * Returns
   * _none_

The StartUsing method is called automatically when the object is used initially. It is also a virtual method to allow for overriding in inherited classes.

#### StopUsing/1

  > `public virtual void StopUsing(GameObject previousUsingObject)`

  * Parameters
   * `GameObject previousUsingObject` - The game object that was previously using this object.
  * Returns
   * _none_

The StopUsing method is called automatically when the object has stopped being used. It is also a virtual method to allow for overriding in inherited classes.

#### ToggleHighlight/1

  > `public virtual void ToggleHighlight(bool toggle)`

  * Parameters
   * `bool toggle` - The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.
  * Returns
   * _none_

The ToggleHighlight method is used to turn on or off the colour highlight of the object.

#### ResetHighlighter/0

  > `public virtual void ResetHighlighter()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetHighlighter method is used to reset the currently attached highlighter.

#### PauseCollisions/1

  > `public virtual void PauseCollisions(float delay)`

  * Parameters
   * `float delay` - The amount of time to pause the collisions for.
  * Returns
   * _none_

The PauseCollisions method temporarily pauses all collisions on the object at grab time by removing the object's rigidbody's ability to detect collisions. This can be useful for preventing clipping when initially grabbing an item.

#### ZeroVelocity/0

  > `public virtual void ZeroVelocity()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ZeroVelocity method resets the velocity and angular velocity to zero on the rigidbody attached to the object.

#### SaveCurrentState/0

  > `public virtual void SaveCurrentState()`

  * Parameters
   * _none_
  * Returns
   * _none_

The SaveCurrentState method stores the existing object parent and the object's rigidbody kinematic setting.

#### GetTouchingObjects/0

  > `public List<GameObject> GetTouchingObjects()`

  * Parameters
   * _none_
  * Returns
   * `List<GameObject>` - A list of game object of that are currently touching the current object.

The GetTouchingObjects method is used to return the collecetion of valid game objects that are currently touching this object.

#### GetGrabbingObject/0

  > `public GameObject GetGrabbingObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of what is grabbing the current object.

The GetGrabbingObject method is used to return the game object that is currently grabbing this object.

#### GetSecondaryGrabbingObject/0

  > `public GameObject GetSecondaryGrabbingObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of the secondary controller influencing the current grabbed object.

The GetSecondaryGrabbingObject method is used to return the game object that is currently being used to influence this object whilst it is being grabbed by a secondary controller.

#### GetUsingObject/0

  > `public GameObject GetUsingObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of what is using the current object.

The GetUsingObject method is used to return the game object that is currently using this object.

#### IsValidInteractableController/2

  > `public virtual bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)`

  * Parameters
   * `GameObject actualController` - The game object of the controller that is being checked.
   * `AllowedController controllerCheck` - The value of which controller is allowed to interact with this object.
  * Returns
   * `bool` - Is true if the interacting controller is allowed to grab the object.

The IsValidInteractableController method is used to check to see if a controller is allowed to perform an interaction with this object as sometimes controllers are prohibited from grabbing or using an object depedning on the use case.

#### ForceStopInteracting/0

  > `public virtual void ForceStopInteracting()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceStopInteracting method forces the object to no longer be interacted with and will cause a controller to drop the object and stop touching it. This is useful if the controller is required to auto interact with another object.

#### ForceStopSecondaryGrabInteraction/0

  > `public virtual void ForceStopSecondaryGrabInteraction()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceStopSecondaryGrabInteraction method forces the object to no longer be influenced by the second controller grabbing it.

#### RegisterTeleporters/0

  > `public void RegisterTeleporters()`

  * Parameters
   * _none_
  * Returns
   * _none_

The RegisterTeleporters method is used to find all objects that have a teleporter script and register the object on the `OnTeleported` event. This is used internally by the object for keeping Tracked objects positions updated after teleporting.

#### UnregisterTeleporters/0

  > `public void UnregisterTeleporters()`

  * Parameters
   * _none_
  * Returns
   * _none_

The UnregisterTeleporters method is used to unregister all teleporter events that are active on this object.

#### StoreLocalScale/0

  > `public virtual void StoreLocalScale()`

  * Parameters
   * _none_
  * Returns
   * _none_

the StoreLocalScale method saves the current transform local scale values.

#### ToggleSnapDropZone/2

  > `public virtual void ToggleSnapDropZone(VRTK_SnapDropZone snapDropZone, bool state)`

  * Parameters
   * `VRTK_SnapDropZone snapDropZone` - The Snap Drop Zone object that is being interacted with.
   * `bool state` - The state of whether the interactable object is fixed in or removed from the Snap Drop Zone. True denotes the interactable object is fixed to the Snap Drop Zone and false denotes it has been removed from the Snap Drop Zone.
  * Returns
   * _none_

The ToggleSnapDropZone method is used to set the state of whether the interactable object is in a Snap Drop Zone or not.

#### IsInSnapDropZone/0

  > `public bool IsInSnapDropZone()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the interactable object is currently snapped in a drop zone and returns false if it is not.

The IsInSnapDropZone method determines whether the interactable object is currently snapped to a drop zone.

#### SetSnapDropZoneHover/1

  > `public void SetSnapDropZoneHover(bool state)`

  * Parameters
   * `bool state` - The state of whether the object is being hovered or not.
  * Returns
   * _none_

The SetSnapDropZoneHover method sets whether the interactable object is currently being hovered over a valid Snap Drop Zone.

#### GetStoredSnapDropZone/0

  > `public VRTK_SnapDropZone GetStoredSnapDropZone()`

  * Parameters
   * _none_
  * Returns
   * `VRTK_SnapDropZone` - The SnapDropZone that the interactable object is currently snapped to.

The GetStoredSnapDropZone method returns the snap drop zone that the interactable object is currently snapped to.

#### IsDroppable/0

  > `public bool IsDroppable()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the object can currently be dropped and returns false if it is not currently possible to drop.

The IsDroppable method returns whether the object can be dropped or not in it's current situation.

#### IsSwappable/0

  > `public bool IsSwappable()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the object can be grabbed by a secondary controller whilst already being grabbed and the object will swap controllers. Returns false if the object cannot be swapped.

The IsSwappable method returns whether the object can be grabbed with one controller and then swapped to another controller by grabbing with the secondary controller.

#### PerformSecondaryAction/0

  > `public bool PerformSecondaryAction()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the obejct has a secondary action, returns false if it has no secondary action or is swappable.

The PerformSecondaryAction method returns whether the object has a secondary action that can be performed when grabbing the object with a secondary controller.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` uses the `VRTK_InteractTouch` and `VRTK_InteractGrab` scripts on the controllers to show how an interactable object can be grabbed and snapped to the controller and thrown around the game world.

`VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` shows multiple objects that can be grabbed by holding the buttons or grabbed by toggling the button click and also has objects that can have their Using state toggled to show how multiple items can be turned on at the same time.

---

## Interact Touch (VRTK_InteractTouch)

### Overview

The Interact Touch script is usually applied to a Controller and provides a collider to know when the controller is touching something.

Colliders are created for the controller and by default the selected controller SDK will have a set of colliders for the given default controller of that SDK.

A custom collider can be provided by the Custom Rigidbody Object parameter.

### Inspector Parameters

 * **Custom Rigidbody Object:** If a custom rigidbody and collider for the rigidbody are required, then a gameobject containing a rigidbody and collider can be passed into this parameter. If this is empty then the rigidbody and collider will be auto generated at runtime to match the SDK default controller.

### Class Events

 * `ControllerTouchInteractableObject` - Emitted when a valid object is touched.
 * `ControllerUntouchInteractableObject` - Emitted when a valid object is no longer being touched.

### Unity Events

Adding the `VRTK_InteractTouch_UnityEvents` component to `VRTK_InteractTouch` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnControllerTouchInteractableObject` - Emits the ControllerTouchInteractableObject class event.
 * `OnControllerUntouchInteractableObject` - Emits the ControllerUntouchInteractableObject class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller doing the interaction.
 * `GameObject target` - The GameObject of the interactable object that is being interacted with by the controller.

### Class Methods

#### ForceTouch/1

  > `public void ForceTouch(GameObject obj)`

  * Parameters
   * `GameObject obj` - The game object to attempt to force touch.
  * Returns
   * _none_

The ForceTouch method will attempt to force the controller to touch the given game object. This is useful if an object that isn't being touched is required to be grabbed or used as the controller doesn't physically have to be touching it to be forced to interact with it.

#### GetTouchedObject/0

  > `public GameObject GetTouchedObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of what is currently being touched by this controller.

The GetTouchedObject method returns the current object being touched by the controller.

#### IsObjectInteractable/1

  > `public bool IsObjectInteractable(GameObject obj)`

  * Parameters
   * `GameObject obj` - The game object to check to see if it's interactable.
  * Returns
   * `bool` - Is true if the given object is of type `VRTK_InteractableObject`.

The IsObjectInteractable method is used to check if a given game object is of type `VRTK_InteractableObject` and whether the object is enabled.

#### ToggleControllerRigidBody/2

  > `public void ToggleControllerRigidBody(bool state, bool forceToggle = false)`

  * Parameters
   * `bool state` - The state of whether the rigidbody is on or off. `true` toggles the rigidbody on and `false` turns it off.
   * `bool forceToggle` - Determines if the rigidbody has been forced into it's new state by another script. This can be used to override other non-force settings. Defaults to `false`
  * Returns
   * _none_

The ToggleControllerRigidBody method toggles the controller's rigidbody's ability to detect collisions. If it is true then the controller rigidbody will collide with other collidable game objects.

#### IsRigidBodyActive/0

  > `public bool IsRigidBodyActive()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the rigidbody on the controller is currently active and able to affect other scene rigidbodies.

The IsRigidBodyActive method checks to see if the rigidbody on the controller object is active and can affect other rigidbodies in the scene.

#### IsRigidBodyForcedActive/0

  > `public bool IsRigidBodyForcedActive()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the rigidbody is active and has been forced into the active state.

The IsRigidBodyForcedActive method checks to see if the rigidbody on the controller object has been forced into the active state.

#### ForceStopTouching/0

  > `public void ForceStopTouching()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceStopTouching method will stop the controller from touching an object even if the controller is physically touching the object still.

#### ControllerColliders/0

  > `public Collider[] ControllerColliders()`

  * Parameters
   * _none_
  * Returns
   * `Collider[]` - An array of colliders that are associated with the controller.

The ControllerColliders method retrieves all of the associated colliders on the controller.

### Example

`VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the highlighting of objects that have the `VRTK_InteractableObject` script added to them to show the ability to highlight interactable objects when they are touched by the controllers.

---

## Interact Grab (VRTK_InteractGrab)

### Overview

The Interact Grab script is attached to a Controller object and requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for grabbing and releasing interactable game objects.

It listens for the `AliasGrabOn` and `AliasGrabOff` events to determine when an object should be grabbed and should be released.

The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be grabbed.

An object can be grabbed if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isGrabbable` set to `true`.

If a valid interactable object is grabbable then pressing the set `Grab` button on the Controller (default is `Grip`) will grab and snap the object to the controller and will not release it until the `Grab` button is released.

When the Controller `Grab` button is released, if the interactable game object is grabbable then it will be propelled in the direction and at the velocity the controller was at, which can simulate object throwing.

The interactable objects require a collider to activate the trigger and a rigidbody to pick them up and move them around the game world.

### Inspector Parameters

 * **Controller Attach Point:** The rigidbody point on the controller model to snap the grabbed object to. If blank it will be set to the SDK default.
 * **Grab Precognition:** An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.
 * **Throw Multiplier:** An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the play area to simulate being able to throw items further.
 * **Create Rigid Body When Not Touching:** If this is checked and the controller is not touching an Interactable Object when the grab button is pressed then a rigid body is added to the controller to allow the controller to push other rigid body objects around.

### Class Events

 * `ControllerGrabInteractableObject` - Emitted when a valid object is grabbed.
 * `ControllerUngrabInteractableObject` - Emitted when a valid object is released from being grabbed.

### Unity Events

Adding the `VRTK_InteractGrab_UnityEvents` component to `VRTK_InteractGrab` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnControllerGrabInteractableObject` - Emits the ControllerGrabInteractableObject class event.
 * `OnControllerUngrabInteractableObject` - Emits the ControllerUngrabInteractableObject class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller doing the interaction.
 * `GameObject target` - The GameObject of the interactable object that is being interacted with by the controller.

### Class Methods

#### ForceRelease/1

  > `public void ForceRelease(bool applyGrabbingObjectVelocity = false)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If this is true then upon releasing the object any velocity on the grabbing object will be applied to the object to essentiall throw it. Defaults to `false`.
  * Returns
   * _none_

The ForceRelease method will force the controller to stop grabbing the currently grabbed object.

#### AttemptGrab/0

  > `public void AttemptGrab()`

  * Parameters
   * _none_
  * Returns
   * _none_

The AttemptGrab method will attempt to grab the currently touched object without needing to press the grab button on the controller.

#### GetGrabbedObject/0

  > `public GameObject GetGrabbedObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of what is currently being grabbed by this controller.

The GetGrabbedObject method returns the current object being grabbed by the controller.

### Example

`VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the grabbing of interactable objects that have the `VRTK_InteractableObject` script attached to them. The objects can be picked up and thrown around.

`VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` demonstrates that each controller can grab and use objects independently and objects can also be toggled to their use state simultaneously.

`VRTK/Examples/014_Controller_SnappingObjectsOnGrab` demonstrates the different mechanisms for snapping a grabbed object to the controller.

---

## Interact Use (VRTK_InteractUse)

### Overview

The Interact Use script is attached to a Controller object and requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for using and stop using interactable game objects.

It listens for the `AliasUseOn` and `AliasUseOff` events to determine when an object should be used and should stop using.

The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be used.

An object can be used if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isUsable` set to `true`.

If a valid interactable object is usable then pressing the set `Use` button on the Controller (default is `Trigger`) will call the `StartUsing` method on the touched interactable object.

### Class Events

 * `ControllerUseInteractableObject` - Emitted when a valid object starts being used.
 * `ControllerUnuseInteractableObject` - Emitted when a valid object stops being used.

### Unity Events

Adding the `VRTK_InteractUse_UnityEvents` component to `VRTK_InteractUse` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnControllerUseInteractableObject` - Emits the ControllerUseInteractableObject class event.
 * `OnControllerUnuseInteractableObject` - Emits the ControllerUnuseInteractableObject class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller doing the interaction.
 * `GameObject target` - The GameObject of the interactable object that is being interacted with by the controller.

### Class Methods

#### GetUsingObject/0

  > `public GameObject GetUsingObject()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The game object of what is currently being used by this controller.

The GetUsingObject method returns the current object being used by the controller.

#### ForceStopUsing/0

  > `public void ForceStopUsing()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceStopUsing method will force the controller to stop using the currently touched object and will also stop the object's using action.

#### ForceResetUsing/0

  > `public void ForceResetUsing()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ForceResetUsing will force the controller to stop using the currently touched object but the object will continue with it's existing using action.

### Example

`VRTK/Examples/006_Controller_UsingADoor` simulates using a door object to open and close it. It also has a cube on the floor that can be grabbed to show how interactable objects can be usable or grabbable.

`VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that objects can be grabbed with one button and used with another (e.g. firing a gun).

---

## Interact Haptics (VRTK_InteractHaptics)

### Overview

The Interact Haptics script is attached on the same GameObject as an Interactable Object script and provides controller haptics on touch, grab and use of the object.

### Inspector Parameters

 * **Strength On Touch:** Denotes how strong the rumble in the controller will be on touch.
 * **Duration On Touch:** Denotes how long the rumble in the controller will last on touch.
 * **Interval On Touch:** Denotes interval betweens rumble in the controller on touch.
 * **Strength On Grab:** Denotes how strong the rumble in the controller will be on grab.
 * **Duration On Grab:** Denotes how long the rumble in the controller will last on grab.
 * **Interval On Grab:** Denotes interval betweens rumble in the controller on grab.
 * **Strength On Use:** Denotes how strong the rumble in the controller will be on use.
 * **Duration On Use:** Denotes how long the rumble in the controller will last on use.
 * **Interval On Use:** Denotes interval betweens rumble in the controller on use.

### Class Methods

#### HapticsOnTouch/1

  > `public virtual void HapticsOnTouch(VRTK_ControllerActions controllerActions)`

  * Parameters
   * `VRTK_ControllerActions controllerActions` - The controller to activate the haptic feedback on.
  * Returns
   * _none_

The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.

#### HapticsOnGrab/1

  > `public virtual void HapticsOnGrab(VRTK_ControllerActions controllerActions)`

  * Parameters
   * `VRTK_ControllerActions controllerActions` - The controller to activate the haptic feedback on.
  * Returns
   * _none_

The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.

#### HapticsOnUse/1

  > `public virtual void HapticsOnUse(VRTK_ControllerActions controllerActions)`

  * Parameters
   * `VRTK_ControllerActions controllerActions` - The controller to activate the haptic feedback on.
  * Returns
   * _none_

The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.

---

## Interact Controller Appearance (VRTK_InteractControllerAppearance)

### Overview

The Interact Controller Appearance script is attached on the same GameObject as an Interactable Object script and is used to determine whether the controller model should be visible or hidden on touch, grab or use.

### Inspector Parameters

 * **Hide Controller On Touch:** Hides the controller model when a valid touch occurs.
 * **Hide Delay On Touch:** The amount of seconds to wait before hiding the controller on touch.
 * **Hide Controller On Grab:** Hides the controller model when a valid grab occurs.
 * **Hide Delay On Grab:** The amount of seconds to wait before hiding the controller on grab.
 * **Hide Controller On Use:** Hides the controller model when a valid use occurs.
 * **Hide Delay On Use:** The amount of seconds to wait before hiding the controller on use.

### Class Methods

#### ToggleControllerOnTouch/3

  > `public virtual void ToggleControllerOnTouch(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)`

  * Parameters
   * `bool showController` - If true then the controller will attempt to be made visible when no longer touching, if false then the controller will be hidden on touch.
   * `VRTK_ControllerActions controllerActions` - The controller to apply the visibility state to.
   * `GameObject obj` - The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.
  * Returns
   * _none_

The ToggleControllerOnTouch method determines whether the controller should be shown or hidden when touching an interactable object.

#### ToggleControllerOnGrab/3

  > `public virtual void ToggleControllerOnGrab(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)`

  * Parameters
   * `bool showController` - If true then the controller will attempt to be made visible when no longer grabbing, if false then the controller will be hidden on grab.
   * `VRTK_ControllerActions controllerActions` - The controller to apply the visibility state to.
   * `GameObject obj` - The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.
  * Returns
   * _none_

The ToggleControllerOnGrab method determines whether the controller should be shown or hidden when grabbing an interactable object.

#### ToggleControllerOnUse/3

  > `public virtual void ToggleControllerOnUse(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)`

  * Parameters
   * `bool showController` - If true then the controller will attempt to be made visible when no longer using, if false then the controller will be hidden on use.
   * `VRTK_ControllerActions controllerActions` - The controller to apply the visibility state to.
   * `GameObject obj` - The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.
  * Returns
   * _none_

The ToggleControllerOnUse method determines whether the controller should be shown or hidden when using an interactable object.

### Example

`VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that the controller can be hidden when touching, grabbing and using an object.

---

## Object Auto Grab (VRTK_ObjectAutoGrab)

### Overview

It is possible to automatically grab an Interactable Object to a specific controller by applying the Object Auto Grab script to the controller that the object should be grabbed by default.

### Inspector Parameters

 * **Object To Grab:** A game object (either within the scene or a prefab) that will be grabbed by the controller on game start.
 * **Object Is Prefab:** If the `Object To Grab` is a prefab then this needs to be checked, if the `Object To Grab` already exists in the scene then this needs to be unchecked.
 * **Clone Grabbed Object:** If this is checked then the Object To Grab will be cloned into a new object and attached to the controller leaving the existing object in the scene. This is required if the same object is to be grabbed to both controllers as a single object cannot be grabbed by different controllers at the same time. It is also required to clone a grabbed object if it is a prefab as it needs to exist within the scene to be grabbed.
 * **Always Clone On Enable:** If `Clone Grabbed Object` is checked and this is checked, then whenever this script is disabled and re-enabled, it will always create a new clone of the object to grab. If this is false then the original cloned object will attempt to be grabbed again. If the original cloned object no longer exists then a new clone will be created.

### Class Methods

#### ClearPreviousClone/0

  > `public void ClearPreviousClone()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ClearPreviousClone method resets the previous cloned object to null to ensure when the script is re-enabled that a new cloned object is created, rather than the original clone being grabbed again.

### Example

`VRTK/Examples/026_Controller_ForceHoldObject` shows how to automatically grab a sword to each controller and also prevents the swords from being dropped so they are permanently attached to the user's controllers.

---

# Highlighters (VRTK/Scripts/Interactions/Highlighters)

This directory contains scripts that are used to provide different object highlighting.

 * [Base Highlighter](#base-highlighter-vrtk_basehighlighter)
 * [Material Colour Swap](#material-colour-swap-vrtk_materialcolorswaphighlighter)
 * [Material Property Block Colour Swap](#material-property-block-colour-swap-vrtk_materialpropertyblockcolorswaphighlighter)
 * [Outline Object Copy](#outline-object-copy-vrtk_outlineobjectcopyhighlighter)

---

## Base Highlighter (VRTK_BaseHighlighter)

### Overview

The Base Highlighter is an abstract class that all other highlighters inherit and are required to implement the public methods.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Inspector Parameters

 * **Active:** Determines if this highlighter is the active highlighter for the object the component is attached to. Only 1 active highlighter can be applied to a game object.
 * **Unhighlight On Disable:** Determines if the highlighted object should be unhighlighted when it is disabled.

### Class Methods

#### Initialise/2

  > `public abstract void Initialise(Color? color = null, Dictionary<string, object> options = null);`

  * Parameters
   * `Color? color` - An optional colour may be passed through at point of initialisation in case the highlighter requires it.
   * `Dictionary<string, object> options` - An optional dictionary of highlighter specific options that may be differ with highlighter implementations.
  * Returns
   * _none_

The Initalise method is used to set up the state of the highlighter.

#### ResetHighlighter/0

  > `public abstract void ResetHighlighter();`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetHighlighter method is used to reset the highlighter if anything on the object has changed. It should be called by any scripts changing object materials or colours.

#### Highlight/2

  > `public abstract void Highlight(Color? color = null, float duration = 0f);`

  * Parameters
   * `Color? color` - An optional colour to highlight the game object to. The highlight colour may already have been set in the `Initialise` method so may not be required here.
   * `float duration` - An optional duration of how long before the highlight has occured. It can be used by highlighters to fade the colour if possible.
  * Returns
   * _none_

The Highlight method is used to initiate the highlighting logic to apply to an object.

#### Unhighlight/2

  > `public abstract void Unhighlight(Color? color = null, float duration = 0f);`

  * Parameters
   * `Color? color` - An optional colour that could be used during the unhighlight phase. Usually will be left as null.
   * `float duration` - An optional duration of how long before the unhighlight has occured.
  * Returns
   * _none_

The Unhighlight method is used to initiate the logic that returns an object back to it's original appearance.

#### GetOption<T>/2

  > `public virtual T GetOption<T>(Dictionary<string, object> options, string key)`

  * Type Params
   * `T` - The system type that is expected to be returned.
  * Parameters
   * `Dictionary<string, object> options` - The dictionary of options to check in.
   * `string key` - The identifier key to look for.
  * Returns
   * `T` - The value in the options at the given key returned in the provided system type.

The GetOption method is used to return a value from the options array if the given key exists.

#### UsesClonedObject/0

  > `public virtual bool UsesClonedObject()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the highlighter creates a cloned object to apply the highlighter on, returns false if no additional object is created.

The UsesClonedObject method is used to return whether the current highlighter creates a cloned object to do the highlighting with.

#### GetActiveHighlighter/1

  > `public static VRTK_BaseHighlighter GetActiveHighlighter(GameObject obj)`

  * Parameters
   * `GameObject obj` - The game object to check for a highlighter on.
  * Returns
   * `VRTK_BaseHighlighter` - A valid and active highlighter.

The GetActiveHighlighter method checks the given game object for a valid and active highlighter.

---

## Material Colour Swap (VRTK_MaterialColorSwapHighlighter)
 > extends [VRTK_BaseHighlighter](#base-highlighter-vrtk_basehighlighter)

### Overview

The Material Colour Swap Highlighter is a basic implementation that simply swaps the texture colour for the given highlight colour.

Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted.

The Draw Call Batching will resume on the original material when the item is no longer highlighted.

This is the default highlighter that is applied to any script that requires a highlighting component (e.g. `VRTK_Interactable_Object` or `VRTK_ControllerActions`).

### Inspector Parameters

 * **Emission Darken:** The emission colour of the texture will be the highlight colour but this percent darker.
 * **Custom Material:** A custom material to use on the highlighted object.

### Class Methods

#### Initialise/2

  > `public override void Initialise(Color? color = null, Dictionary<string, object> options = null)`

  * Parameters
   * `Color? color` - Not used.
   * `Dictionary<string, object> options` - A dictionary array containing the highlighter options:
     * `<'resetMainTexture', bool>` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.
  * Returns
   * _none_

The Initialise method sets up the highlighter for use.

#### ResetHighlighter/0

  > `public override void ResetHighlighter()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetHighlighter method stores the object's materials and shared materials prior to highlighting.

#### Highlight/2

  > `public override void Highlight(Color? color, float duration = 0f)`

  * Parameters
   * `Color? color` - The colour to highlight to.
   * `float duration` - The time taken to fade to the highlighted colour.
  * Returns
   * _none_

The Highlight method initiates the change of colour on the object and will fade to that colour (from a base white colour) for the given duration.

#### Unhighlight/2

  > `public override void Unhighlight(Color? color = null, float duration = 0f)`

  * Parameters
   * `Color? color` - Not used.
   * `float duration` - Not used.
  * Returns
   * _none_

The Unhighlight method returns the object back to it's original colour.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the solid highlighting on the green cube, red cube and flying saucer when the controller touches it.

`VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the solid highlighting if the right controller collides with the green box or if any of the buttons are pressed.

---

## Material Property Block Colour Swap (VRTK_MaterialPropertyBlockColorSwapHighlighter)
 > extends [VRTK_MaterialColorSwapHighlighter](#material-colour-swap-vrtk_materialcolorswaphighlighter)

### Overview

This highlighter swaps the texture colour for the given highlight colour using MaterialPropertyBlocks.
The effect of this highlighter is the same as of the VRTK_MaterialColorSwapHighlighter.cs but this highlighter
can additionally handle objects which make use material instances.

Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted.

The Draw Call Batching will resume on the original material when the item is no longer highlighted.

### Class Methods

#### Initialise/2

  > `public override void Initialise(Color? color = null, Dictionary<string, object> options = null)`

  * Parameters
   * `Color? color` - Not used.
   * `Dictionary<string, object> options` - A dictionary array containing the highlighter options:
     * `<'resetMainTexture', bool>` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.
  * Returns
   * _none_

The Initialise method sets up the highlighter for use.

#### Unhighlight/2

  > `public override void Unhighlight(Color? color = null, float duration = 0f)`

  * Parameters
   * `Color? color` - Not used.
   * `float duration` - Not used.
  * Returns
   * _none_

The Unhighlight method returns the object back to it's original colour.

---

## Outline Object Copy (VRTK_OutlineObjectCopyHighlighter)
 > extends [VRTK_BaseHighlighter](#base-highlighter-vrtk_basehighlighter)

### Overview

The Outline Object Copy Highlighter works by making a copy of a mesh and adding an outline shader to it and toggling the appearance of the highlighted object.

### Inspector Parameters

 * **Thickness:** The thickness of the outline effect
 * **Custom Outline Model:** The GameObject to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.
 * **Custom Outline Model Path:** A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.
 * **Enable Submesh Highlight:** If the mesh has multiple sub-meshes to highlight then this should be checked, otherwise only the first mesh will be highlighted.

### Class Methods

#### Initialise/2

  > `public override void Initialise(Color? color = null, Dictionary<string, object> options = null)`

  * Parameters
   * `Color? color` - Not used.
   * `Dictionary<string, object> options` - A dictionary array containing the highlighter options:
     * `<'thickness', float>` - Same as `thickness` inspector parameter.
     * `<'customOutlineModel', GameObject>` - Same as `customOutlineModel` inspector parameter.
     * `<'customOutlineModelPath', string>` - Same as `customOutlineModelPath` inspector parameter.
  * Returns
   * _none_

The Initialise method sets up the highlighter for use.

#### ResetHighlighter/0

  > `public override void ResetHighlighter()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetHighlighter method creates the additional model to use as the outline highlighted object.

#### Highlight/2

  > `public override void Highlight(Color? color, float duration = 0f)`

  * Parameters
   * `Color? color` - The colour to outline with.
   * `float duration` - Not used.
  * Returns
   * _none_

The Highlight method initiates the outline object to be enabled and display the outline colour.

#### Unhighlight/2

  > `public override void Unhighlight(Color? color = null, float duration = 0f)`

  * Parameters
   * `Color? color` - Not used.
   * `float duration` - Not used.
  * Returns
   * _none_

The Unhighlight method hides the outline object and removes the outline colour.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the outline highlighting on the green sphere when the controller touches it.

`VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the outline highlighting if the left controller collides with the green box.

---

# Grab Attach Mechanics (VRTK/Scripts/Interactions/GrabAttachMechanics)

This directory contains scripts that are used to provide different mechanics to apply when grabbing an interactable object.

 * [Base Grab Attach](#base-grab-attach-vrtk_basegrabattach)
 * [Base Joint Grab Attach](#base-joint-grab-attach-vrtk_basejointgrabattach)
 * [Fixed Joint Grab Attach](#fixed-joint-grab-attach-vrtk_fixedjointgrabattach)
 * [Spring Joint Grab Attach](#spring-joint-grab-attach-vrtk_springjointgrabattach)
 * [Custom Joint Grab Attach](#custom-joint-grab-attach-vrtk_customjointgrabattach)
 * [Child Of Controller Grab Attach](#child-of-controller-grab-attach-vrtk_childofcontrollergrabattach)
 * [Track Object Grab Attach](#track-object-grab-attach-vrtk_trackobjectgrabattach)
 * [Rotator Track Grab Attach](#rotator-track-grab-attach-vrtk_rotatortrackgrabattach)
 * [Climbable Grab Attach](#climbable-grab-attach-vrtk_climbablegrabattach)

---

## Base Grab Attach (VRTK_BaseGrabAttach)

### Overview

The Base Grab Attach script is an abstract class that all grab attach script inherit.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Inspector Parameters

 * **Precision Grab:** If this is checked then when the controller grabs the object, it will grab it with precision and pick it up at the particular point on the object the controller is touching.
 * **Right Snap Handle:** A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the right handed controller. If no Right Snap Handle is provided but a Left Snap Handle is provided, then the Left Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.
 * **Left Snap Handle:** A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the left handed controller. If no Left Snap Handle is provided but a Right Snap Handle is provided, then the Right Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.
 * **Throw Velocity With Attach Distance:** If checked then when the object is thrown, the distance between the object's attach point and the controller's attach point will be used to calculate a faster throwing velocity.
 * **Throw Multiplier:** An amount to multiply the velocity of the given object when it is thrown. This can also be used in conjunction with the Interact Grab Throw Multiplier to have certain objects be thrown even further than normal (or thrown a shorter distance if a number below 1 is entered).
 * **On Grab Collision Delay:** The amount of time to delay collisions affecting the object when it is first grabbed. This is useful if a game object may get stuck inside another object when it is being grabbed.

### Class Methods

#### IsTracked/0

  > `public bool IsTracked()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the mechanic is of type tracked.

The IsTracked method determines if the grab attach mechanic is a track object type.

#### IsClimbable/0

  > `public bool IsClimbable()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the mechanic is of type climbable.

The IsClimbable method determines if the grab attach mechanic is a climbable object type.

#### IsKinematic/0

  > `public bool IsKinematic()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the mechanic is of type kinematic.

The IsKinematic method determines if the grab attach mechanic is a kinematic object type.

#### ValidGrab/1

  > `public virtual bool ValidGrab(Rigidbody checkAttachPoint)`

  * Parameters
   * `Rigidbody checkAttachPoint` -
  * Returns
   * `bool` - Always returns true for the base check.

The ValidGrab method determines if the grab attempt is valid.

#### SetTrackPoint/1

  > `public virtual void SetTrackPoint(Transform givenTrackPoint)`

  * Parameters
   * `Transform givenTrackPoint` - The track point to set on the grabbed object.
  * Returns
   * _none_

The SetTrackPoint method sets the point on the grabbed object where the grab is happening.

#### SetInitialAttachPoint/1

  > `public virtual void SetInitialAttachPoint(Transform givenInitialAttachPoint)`

  * Parameters
   * `Transform givenInitialAttachPoint` - The point where the initial grab took place.
  * Returns
   * _none_

The SetInitialAttachPoint method sets the point on the grabbed object where the initial grab happened.

#### StartGrab/3

  > `public virtual bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

  * Parameters
   * `GameObject grabbingObject` - The object that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The object that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
  * Returns
   * `bool` - Is true if the grab is successful, false if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an object is grabbed.

#### StopGrab/1

  > `public virtual void StopGrab(bool applyGrabbingObjectVelocity)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If true will apply the current velocity of the grabbing object to the grabbed object on release.
  * Returns
   * _none_

The StopGrab method ends the grab of the current object and cleans up the state.

#### CreateTrackPoint/4

  > `public virtual Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

  * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The object that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The object that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
  * Returns
   * `Transform` - The transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public virtual void ProcessUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the interactable object.

#### ProcessFixedUpdate/0

  > `public virtual void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object.

---

## Base Joint Grab Attach (VRTK_BaseJointGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

The Base Joint Grab Attach script is an abstract class that all joint grab attach types inherit.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Inspector Parameters

 * **Destroy Immediately On Throw:** Determines whether the joint should be destroyed immediately on release or whether to wait till the end of the frame before being destroyed.

### Class Methods

#### ValidGrab/1

  > `public override bool ValidGrab(Rigidbody checkAttachPoint)`

  * Parameters
   * `Rigidbody checkAttachPoint` -
  * Returns
   * `bool` - Returns true if there is no current grab happening, or the grab is initiated by another grabbing object.

The ValidGrab method determines if the grab attempt is valid.

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

  * Parameters
   * `GameObject grabbingObject` - The object that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The object that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
  * Returns
   * `bool` - Is true if the grab is successful, false if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an object is grabbed. It is also responsible for creating the joint on the grabbed object.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If true will apply the current velocity of the grabbing object to the grabbed object on release.
  * Returns
   * _none_

The StopGrab method ends the grab of the current object and cleans up the state. It is also responsible for removing the joint from the grabbed object.

---

## Fixed Joint Grab Attach (VRTK_FixedJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

The Fixed Joint Grab Attach script is used to create a simple Fixed Joint connection between the object and the grabbing object.

### Inspector Parameters

 * **Break Force:** Maximum force the joint can withstand before breaking. Infinity means unbreakable.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates this grab attach mechanic all of the grabbable objects in the scene.

---

## Spring Joint Grab Attach (VRTK_SpringJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

The Spring Joint Grab Attach script is used to create a simple Spring Joint connection between the object and the grabbing object.

### Inspector Parameters

 * **Break Force:** Maximum force the joint can withstand before breaking. Infinity means unbreakable.
 * **Strength:** The strength of the spring.
 * **Damper:** The amount of dampening to apply to the spring.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Drawer object in the scene.

---

## Custom Joint Grab Attach (VRTK_CustomJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

The Custom Joint Grab Attach script allows a custom joint to be provided for the grab attach mechanic.

The custom joint is placed on the interactable object and at runtime the joint is copied into a `JointHolder` game object that becomes a child of the interactable object.

The custom joint is then copied from this `JointHolder` to the interactable object when a grab happens and is removed when a grab ends.

### Inspector Parameters

 * **Custom Joint:** The joint to use for the grab attach joint.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Lamp object in the scene.

---

## Child Of Controller Grab Attach (VRTK_ChildOfControllerGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

The Child Of Controller Grab Attach script is used to make the grabbed object a child of the grabbing object upon grab.

The object upon grab will naturally track the position and rotation of the grabbing object as it is a child of the grabbing game object.

The rigidbody of the object will be set to kinematic upon grab and returned to it's original state on release.

### Class Methods

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

  * Parameters
   * `GameObject grabbingObject` - The object that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The object that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
  * Returns
   * `bool` - Is true if the grab is successful, false if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an object is grabbed. It is also responsible for creating the joint on the grabbed object.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If true will apply the current velocity of the grabbing object to the grabbed object on release.
  * Returns
   * _none_

The StopGrab method ends the grab of the current object and cleans up the state.

### Example

`VRTK/Examples/023_Controller_ChildOfControllerOnGrab` uses this grab attach mechanic for the bow and the arrow.

---

## Track Object Grab Attach (VRTK_TrackObjectGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

The Track Object Grab Attach script doesn't attach the object to the controller via a joint, instead it ensures the object tracks the direction of the controller.

This works well for items that are on hinged joints or objects that require to interact naturally with other scene rigidbodies.

### Inspector Parameters

 * **Detach Distance:** The maximum distance the grabbing controller is away from the object before it is automatically dropped.
 * **Velocity Limit:** The maximum amount of velocity magnitude that can be applied to the object. Lowering this can prevent physics glitches if objects are moving too fast.
 * **Angular Velocity Limit:** The maximum amount of angular velocity magnitude that can be applied to the object. Lowering this can prevent physics glitches if objects are moving too fast.

### Class Methods

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If true will apply the current velocity of the grabbing object to the grabbed object on release.
  * Returns
   * _none_

The StopGrab method ends the grab of the current object and cleans up the state.

#### CreateTrackPoint/4

  > `public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

  * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The object that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The object that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
  * Returns
   * `Transform` - The transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the interactable object. It is responsible for checking if the tracked object has exceeded it's detach distance.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object. It applies velocity to the object to ensure it is tracking the grabbing object.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Chest handle and Fire Extinguisher body.

---

## Rotator Track Grab Attach (VRTK_RotatorTrackGrabAttach)
 > extends [VRTK_TrackObjectGrabAttach](#track-object-grab-attach-vrtk_trackobjectgrabattach)

### Overview

The Rotator Track Grab Attach script is used to track the object but instead of the object tracking the direction of the controller, a force is applied to the object to cause it to rotate.

This is ideal for hinged joints on items such as wheels or doors.

### Class Methods

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

  * Parameters
   * `bool applyGrabbingObjectVelocity` - If true will apply the current velocity of the grabbing object to the grabbed object on release.
  * Returns
   * _none_

The StopGrab method ends the grab of the current object and cleans up the state.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object. It applies a force to the grabbed object to move it in the direction of the grabbing object.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Wheel and Door objects in the scene.

---

## Climbable Grab Attach (VRTK_ClimbableGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

The Climbable Grab Attach script is used to mark the object as a climbable interactable object.

### Inspector Parameters

 * **Use Object Rotation:** Will respect the grabbed climbing object's rotation if it changes dynamically

### Example

`VRTK/Examples/037_CameraRig_ClimbingFalling` uses this grab attach mechanic for each item that is climbable in the scene.

---

# Secondary Controller Grab Actions (VRTK/Scripts/Interactions/SecondaryControllerGrabActions)

This directory contains scripts that are used to provide different actions when a secondary controller grabs a grabbed object.

 * [Base Grab Action](#base-grab-action-vrtk_basegrabaction)
 * [Swap Controller Grab Action](#swap-controller-grab-action-vrtk_swapcontrollergrabaction)
 * [Axis Scale Grab Action](#axis-scale-grab-action-vrtk_axisscalegrabaction)
 * [Control Direction Grab Action](#control-direction-grab-action-vrtk_controldirectiongrabaction)

---

## Base Grab Action (VRTK_BaseGrabAction)

### Overview

The Base Grab Action is an abstract class that all other secondary controller actions inherit and are required to implement the public abstract methods.

As this is an abstract class, it cannot be applied directly to a game object and performs no logic.

### Class Methods

#### Initialise/5

  > `public virtual void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

  * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary controller.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary controller.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary controller.
   * `Transform primaryGrabPoint` - The point on the object where the primary controller initially grabbed the object.
   * `Transform secondaryGrabPoint` - The point on the object where the secondary controller initially grabbed the object.
  * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.

#### ResetAction/0

  > `public virtual void ResetAction()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetAction method is used to reset the secondary action when the object is no longer grabbed by a secondary controller.

#### IsActionable/0

  > `public bool IsActionable()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the secondary grab action does perform an action on secondary grab.

The IsActionable method is used to determine if the secondary grab action performs an action on grab.

#### IsSwappable/0

  > `public bool IsSwappable()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Is true if the grab action allows swapping to another grabbing object.

The IsSwappable method is used to determine if the secondary grab action allows to swab the grab state to another grabbing object.

#### ProcessUpdate/0

  > `public virtual void ProcessUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.

#### ProcessFixedUpdate/0

  > `public virtual void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller.

#### OnDropAction/0

  > `public virtual void OnDropAction()`

  * Parameters
   * _none_
  * Returns
   * _none_

The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.

---

## Swap Controller Grab Action (VRTK_SwapControllerGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

The Swap Controller Grab Action provides a mechanism to allow grabbed objects to be swapped between controllers.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the ability to swap objects between controllers on grab.

---

## Axis Scale Grab Action (VRTK_AxisScaleGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

The Axis Scale Grab Action provides a mechanism to scale objects when they are grabbed with a secondary controller.

### Inspector Parameters

 * **Ungrab Distance:** The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.
 * **Lock X Axis:** If checked the current X Axis of the object won't be scaled
 * **Lock Y Axis:** If checked the current Y Axis of the object won't be scaled
 * **Lock Z Axis:** If checked the current Z Axis of the object won't be scaled
 * **Uniform Scaling:** If checked all the axes will be scaled together (unless locked)

### Class Methods

#### Initialise/5

  > `public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

  * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary controller.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary controller.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary controller.
   * `Transform primaryGrabPoint` - The point on the object where the primary controller initially grabbed the object.
   * `Transform secondaryGrabPoint` - The point on the object where the secondary controller initially grabbed the object.
  * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and performs the scaling action.

### Example

`VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.

---

## Control Direction Grab Action (VRTK_ControlDirectionGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

The Control Direction Grab Action provides a mechanism to control the facing direction of the object when they are grabbed with a secondary controller.

For an object to correctly be rotated it must be created with the front of the object pointing down the z-axis (forward) and the upwards of the object pointing up the y-axis (up).

It's not possible to control the direction of an interactable object with a `Fixed_Joint` as the joint fixes the rotation of the object.

### Inspector Parameters

 * **Ungrab Distance:** The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.
 * **Release Snap Speed:** The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.
 * **Lock Z Rotation:** Prevent the secondary controller rotating the grabbed object through it's z-axis.

### Class Methods

#### Initialise/5

  > `public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

  * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary controller.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary controller.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary controller.
   * `Transform primaryGrabPoint` - The point on the object where the primary controller initially grabbed the object.
   * `Transform secondaryGrabPoint` - The point on the object where the secondary controller initially grabbed the object.
  * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.

#### ResetAction/0

  > `public override void ResetAction()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ResetAction method is used to reset the secondary action when the object is no longer grabbed by a secondary controller.

#### OnDropAction/0

  > `public override void OnDropAction()`

  * Parameters
   * _none_
  * Returns
   * _none_

The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

  * Parameters
   * _none_
  * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and influences the rotation of the object.

### Example

`VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and control their direction with the second controller.

---

# Presence (VRTK/Scripts/Presence)

A collection of scripts that provide the ability to deal with tracking the world around the user in the scene.

 * [Headset Collision](#headset-collision-vrtk_headsetcollision)
 * [Headset Fade](#headset-fade-vrtk_headsetfade)
 * [Headset Collision Fade](#headset-collision-fade-vrtk_headsetcollisionfade)
 * [Headset Controller Aware](#headset-controller-aware-vrtk_headsetcontrolleraware)
 * [Hip Tracking](#hip-tracking-vrtk_hiptracking)
 * [Body Physics](#body-physics-vrtk_bodyphysics)
 * [Position Rewind](#position-rewind-vrtk_positionrewind)

---

## Headset Collision (VRTK_HeadsetCollision)

### Overview

The purpose of the Headset Collision is to detect when the user's VR headset collides with another game object.

The Headset Collision script will automatically create a script on the headset to deal with the collision events.

### Inspector Parameters

 * **Collider Radius:** The radius of the auto generated sphere collider for detecting collisions on the headset.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision.

### Class Variables

 * `public bool headsetColliding` - Determines if the headset is currently colliding with another object. Default: `false`
 * `public Collider collidingWith` - Stores the collider of what the headset is colliding with. Default: `null`

### Class Events

 * `HeadsetCollisionDetect` - Emitted when the user's headset collides with another game object.
 * `HeadsetCollisionEnded` - Emitted when the user's headset stops colliding with a game object.

### Unity Events

Adding the `VRTK_HeadsetCollision_UnityEvents` component to `VRTK_HeadsetCollision` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnHeadsetCollisionDetect` - Emits the HeadsetCollisionDetect class event.
 * `OnHeadsetCollisionEnded` - Emits the HeadsetCollisionEnded class event.

### Event Payload

 * `Collider collider` - The Collider of the game object the headset has collided with.
 * `Transform currentTransform` - The current Transform of the object that the Headset Collision Fade script is attached to (Camera).

### Class Methods

#### IsColliding/0

  > `public virtual bool IsColliding()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the headset is currently colliding with a valid game object.

The IsColliding method is used to determine if the headset is currently colliding with a valid game object and returns true if it is and false if it is not colliding with anything or an invalid game object.

### Example

`VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.

---

## Headset Fade (VRTK_HeadsetFade)

### Overview

The purpose of the Headset Fade is to change the colour of the headset view to a specified colour over a given duration and to also unfade it back to being transparent.

The `Fade` and `Unfade` methods can only be called via another script and this Headset Fade script does not do anything on initialisation to fade or unfade the headset view.

### Class Events

 * `HeadsetFadeStart` - Emitted when the user's headset begins to fade to a given colour.
 * `HeadsetFadeComplete` - Emitted when the user's headset has completed the fade and is now fully at the given colour.
 * `HeadsetUnfadeStart` - Emitted when the user's headset begins to unfade back to a transparent colour.
 * `HeadsetUnfadeComplete` - Emitted when the user's headset has completed unfading and is now fully transparent again.

### Unity Events

Adding the `VRTK_HeadsetFade_UnityEvents` component to `VRTK_HeadsetFade` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnHeadsetFadeStart` - Emits the HeadsetFadeStart class event.
 * `OnHeadsetFadeComplete` - Emits the HeadsetFadeComplete class event.
 * `OnHeadsetUnfadeStart` - Emits the HeadsetUnfadeStart class event.
 * `OnHeadsetUnfadeComplete` - Emits the HeadsetUnfadeComplete class event.

### Event Payload

 * `float timeTillComplete` - A float that is the duration for the fade/unfade process has remaining.
 * `Transform currentTransform` - The current Transform of the object that the Headset Fade script is attached to (Camera).

### Class Methods

#### IsFaded/0

  > `public virtual bool IsFaded()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the headset is currently fading or faded.

The IsFaded method returns true if the headset is currently fading or has completely faded and returns false if it is completely unfaded.

#### IsTransitioning/0

  > `public virtual bool IsTransitioning()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the headset is currently in the process of fading or unfading.

The IsTransitioning method returns true if the headset is currently fading or unfading and returns false if it is completely faded or unfaded.

#### Fade/2

  > `public virtual void Fade(Color color, float duration)`

  * Parameters
   * `Color color` - The colour to fade the headset view to.
   * `float duration` - The time in seconds to take to complete the fade transition.
  * Returns
   * _none_

The Fade method initiates a change in the colour of the headset view to the given colour over a given duration.

#### Unfade/1

  > `public virtual void Unfade(float duration)`

  * Parameters
   * `float duration` - The time in seconds to take to complete the unfade transition.
  * Returns
   * _none_

The Unfade method initiates the headset to change colour back to a transparent colour over a given duration.

### Example

`VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.

---

## Headset Collision Fade (VRTK_HeadsetCollisionFade)

### Overview

The purpose of the Headset Collision Fade is to detect when the user's VR headset collides with another game object and fades the screen to a solid colour.

This is to deal with a user putting their head into a game object and seeing the inside of the object clipping, which is an undesired effect. The reasoning behind this is if the user puts their head where it shouldn't be, then fading to a colour (e.g. black) will make the user realise they've done something wrong and they'll probably naturally step backwards.

The Headset Collision Fade uses a composition of the Headset Collision and Headset Fade scripts to derive the desired behaviour.

### Inspector Parameters

 * **Blink Transition Speed:** The fade blink speed on collision.
 * **Fade Color:** The colour to fade the headset to on collision.

### Example

`VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.

---

## Headset Controller Aware (VRTK_HeadsetControllerAware)

### Overview

The purpose of Headset Controller Aware is to allow the headset to know if something is blocking the path between the headset and controllers and to know if the headset is looking at a controller.

### Inspector Parameters

 * **Track Left Controller:** If this is checked then the left controller will be checked if items obscure it's path from the headset.
 * **Track Right Controller:** If this is checked then the right controller will be checked if items obscure it's path from the headset.
 * **Controller Glance Radius:** The radius of the accepted distance from the controller origin point to determine if the controller is being looked at.
 * **Custom Right Controller Origin:** A custom transform to provide the world space position of the right controller.
 * **Custom Left Controller Origin:** A custom transform to provide the world space position of the left controller.

### Class Events

 * `ControllerObscured` - Emitted when the controller is obscured by another object.
 * `ControllerUnobscured` - Emitted when the controller is no longer obscured by an object.
 * `ControllerGlanceEnter` - Emitted when the controller is seen by the headset view.
 * `ControllerGlanceExit` - Emitted when the controller is no longer seen by the headset view.

### Unity Events

Adding the `VRTK_HeadsetControllerAware_UnityEvents` component to `VRTK_HeadsetControllerAware` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnControllerObscured` - Emits the ControllerObscured class event.
 * `OnControllerUnobscured` - Emits the ControllerUnobscured class event.
 * `OnControllerGlanceEnter` - Emits the ControllerGlanceEnter class event.
 * `OnControllerGlanceExit` - Emits the ControllerGlanceExit class event.

### Event Payload

 * `RaycastHit raycastHit` - The Raycast Hit struct of item that is obscuring the path to the controller.
 * `uint controllerIndex` - The index of the controller that is being or has been obscured or being or has been glanced.

### Class Methods

#### LeftControllerObscured/0

  > `public bool LeftControllerObscured()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the path between the headset and the controller is obscured.

The LeftControllerObscured method returns the state of if the left controller is being obscured from the path of the headset.

#### RightControllerObscured/0

  > `public bool RightControllerObscured()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the path between the headset and the controller is obscured.

The RightControllerObscured method returns the state of if the right controller is being obscured from the path of the headset.

#### LeftControllerGlanced/0

  > `public bool LeftControllerGlanced()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the headset can currently see the controller within the given radius threshold.

the LeftControllerGlanced method returns the state of if the headset is currently looking at the left controller or not.

#### RightControllerGlanced/0

  > `public bool RightControllerGlanced()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the headset can currently see the controller within the given radius threshold.

the RightControllerGlanced method returns the state of if the headset is currently looking at the right controller or not.

### Example

`VRTK/Examples/029_Controller_Tooltips` displays tooltips that have been added to the controllers and are only visible when the controller is being looked at.

---

## Hip Tracking (VRTK_HipTracking)

### Overview

Hip Tracking attempts to reasonably track hip position in the absence of a hip position sensor.

The Hip Tracking script is placed on an empty GameObject which will be positioned at the estimated hip position.

### Inspector Parameters

 * **Head Offset:** Distance underneath Player Head for hips to reside.
 * **Head Override:** Optional Transform to use as the Head Object for calculating hip position. If none is given one will try to be found in the scene.
 * **Reference Up:** Optional Transform to use for calculating which way is 'Up' relative to the player for hip positioning.

---

## Body Physics (VRTK_BodyPhysics)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

The body physics script deals with how a user's body in the scene reacts to world physics and how to handle drops.

The body physics creates a rigidbody and collider for where the user is standing to allow physics interactions and prevent walking through walls.

Upon actually moving in the play area, the rigidbody is set to kinematic to prevent the world from being pushed back in the user's view reducing sickness.

The body physics script also deals with snapping a user to the nearest floor if they look over a ledge or walk up stairs then it will move the play area to simulate movement in the scene.

To allow for peeking over a ledge and not falling, a fall restiction can happen by keeping a controller over the existing floor and the snap to the nearest floor will not happen until the controllers are also over the floor.

### Inspector Parameters

 * **Enable Body Collisions:** If checked then the body collider and rigidbody will be used to check for rigidbody collisions.
 * **Ignore Grabbed Collisions:** If this is checked then any items that are grabbed with the controller will not collide with the body collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.
 * **Headset Y Offset:** The collider which is created for the user is set at a height from the user's headset position. If the collider is required to be lower to allow for room between the play area collider and the headset then this offset value will shorten the height of the generated collider.
 * **Movement Threshold:** The amount of movement of the headset between the headset's current position and the current standing position to determine if the user is walking in play space and to ignore the body physics collisions if the movement delta is above this threshold.
 * **Standing History Samples:** The maximum number of samples to collect of headset position before determining if the current standing position within the play space has changed.
 * **Lean Y Threshold:** The `y` distance between the headset and the object being leaned over, if object being leaned over is taller than this threshold then the current standing position won't be updated.
 * **Layers To Ignore:** The layers to ignore when raycasting to find floors.
 * **Fall Restriction:** A check to see if the drop to nearest floor should take place. If the selected restrictor is still over the current floor then the drop to nearest floor will not occur. Works well for being able to lean over ledges and look down. Only works for falling down not teleporting up.
 * **Gravity Fall Y Threshold:** When the `y` distance between the floor and the headset exceeds this distance and `Enable Body Collisions` is true then the rigidbody gravity will be used instead of teleport to drop to nearest floor.
 * **Blink Y Threshold:** The `y` distance between the floor and the headset that must change before a fade transition is initiated. If the new user location is at a higher distance than the threshold then the headset blink transition will activate on teleport. If the new user location is within the threshold then no blink transition will happen, which is useful for walking up slopes, meshes and terrains to prevent constant blinking.
 * **Floor Height Tolerance:** The amount the `y` position needs to change by between the current floor `y` position and the previous floor `y` position before a change in floor height is considered to have occurred. A higher value here will mean that a `Drop To Floor` will be less likely to happen if the `y` of the floor beneath the user hasn't changed as much as the given threshold.

### Class Variables

 * `public enum FallingRestrictors` - Options for testing if a play space fall is valid
  * `No_Restriction` - Always drop to nearest floor when the headset is no longer over the current standing object.
  * `Left_Controller` - Don't drop to nearest floor  if the Left Controller is still over the current standing object even if the headset isn't.
  * `Right_Controller` - Don't drop to nearest floor  if the Right Controller is still over the current standing object even if the headset isn't.
  * `Either_Controller` - Don't drop to nearest floor  if Either Controller is still over the current standing object even if the headset isn't.
  * `Both_Controllers` - Don't drop to nearest floor only if Both Controllers are still over the current standing object even if the headset isn't.

### Class Events

 * `StartFalling` - Emitted when a fall begins.
 * `StopFalling` - Emitted when a fall ends.
 * `StartMoving` - Emitted when movement in the play area begins.
 * `StopMoving` - Emitted when movement in the play area ends
 * `StartColliding` - Emitted when the body collider starts colliding with another game object
 * `StopColliding` - Emitted when the body collider stops colliding with another game object

### Unity Events

Adding the `VRTK_BodyPhysics_UnityEvents` component to `VRTK_BodyPhysics` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnStartFalling` - Emits the StartFalling class event.
 * `OnStopFalling` - Emits the StopFalling class event.
 * `OnStartMoving` - Emits the StartMoving class event.
 * `OnStopMoving` - Emits the StopMoving class event.
 * `OnStartColliding` - Emits the StartColliding class event.
 * `OnStopColliding` - Emits the StopColliding class event.

### Event Payload

 * `GameObject target` - The target the event is dealing with.

### Class Methods

#### ArePhysicsEnabled/0

  > `public bool ArePhysicsEnabled()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the body physics will interact with other scene physics objects and false if the body physics will ignore other scene physics objects.

The ArePhysicsEnabled method determines whether the body physics are set to interact with other scene physics objects.

#### ApplyBodyVelocity/3

  > `public void ApplyBodyVelocity(Vector3 velocity, bool forcePhysicsOn = false, bool applyMomentum = false)`

  * Parameters
   * `Vector3 velocity` - The velocity to apply.
   * `bool forcePhysicsOn` - If true will toggle the body collision physics back on if enable body collisions is true.
   * `bool applyMomentum` - If true then the existing momentum of the play area will be applied as a force to the resulting velocity.
  * Returns
   * _none_

The ApplyBodyVelocity method applies a given velocity to the rigidbody attached to the body physics.

#### ToggleOnGround/1

  > `public void ToggleOnGround(bool state)`

  * Parameters
   * `bool state` - If true then body physics are set to being on the ground.
  * Returns
   * _none_

The ToggleOnGround method sets whether the body is considered on the ground or not.

#### TogglePreventSnapToFloor/1

  > `public void TogglePreventSnapToFloor(bool state)`

  * Parameters
   * `bool state` - If true the the snap to floor mechanic will not execute.
  * Returns
   * _none_

The PreventSnapToFloor method sets whether the snap to floor mechanic should be used.

#### IsFalling/0

  > `public bool IsFalling()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the body is currently falling via gravity or via teleport.

The IsFalling method returns the falling state of the body.

#### IsMoving/0

  > `public bool IsMoving()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the user is currently walking around their play area space.

The IsMoving method returns the moving within play area state of the body.

#### IsLeaning/0

  > `public bool IsLeaning()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the user is considered to be leaning over an object.

The IsLeaning method returns the leaning state of the user.

#### OnGround/0

  > `public bool OnGround()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the play area is on the ground and false if the play area is in the air.

The OnGround method returns whether the user is currently standing on the ground or not.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad but the user cannot pass through the objects as they are collidable and the rigidbody physics won't allow the intersection to occur.

---

## Position Rewind (VRTK_PositionRewind)

### Overview

The Position Rewind script is used to reset the user back to a good known standing position upon receiving a headset collision event.

### Inspector Parameters

 * **Rewind Delay:** The amount of time from original headset collision until the rewind to the last good known position takes place.
 * **Pushback Distance:** The additional distance to push the play area back upon rewind to prevent being right next to the wall again.
 * **Crouch Threshold:** The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has the position rewind script to reset the user's position if they walk into objects.

---

# UI (VRTK/Scripts/UI)

A collection of scripts that provide the ability to utilise and interact with Unity UI elements.

 * [UI Canvas](#ui-canvas-vrtk_uicanvas)
 * [UI Pointer](#ui-pointer-vrtk_uipointer)
 * [UI Draggable Item](#ui-draggable-item-vrtk_uidraggableitem)
 * [UI Drop Zone](#ui-drop-zone-vrtk_uidropzone)

---

## UI Canvas (VRTK_UICanvas)

### Overview

The UI Canvas is used to denote which World Canvases are interactable by a UI Pointer.

When the script is enabled it will disable the `Graphic Raycaster` on the canvas and create a custom `UI Graphics Raycaster` and the Blocking Objects and Blocking Mask settings are copied over from the `Graphic Raycaster`.

### Inspector Parameters

 * **Click On Pointer Collision:** Determines if a UI Click action should happen when a UI Pointer game object collides with this canvas.
 * **Auto Activate Within Distance:** Determines if a UI Pointer will be auto activated if a UI Pointer game object comes within the given distance of this canvas. If a value of `0` is given then no auto activation will occur.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UICanvas` script on two of the canvases to show how the UI Pointer can interact with them.

---

## UI Pointer (VRTK_UIPointer)

### Overview

The UI Pointer provides a mechanism for interacting with Unity UI elements on a world canvas. The UI Pointer can be attached to any game object the same way in which a Base Pointer can be and the UI Pointer also requires a controller to initiate the pointer activation and pointer click states.

The simplest way to use the UI Pointer is to attach the script to a game controller along with a Simple Pointer as this provides visual feedback as to where the UI ray is pointing.

The UI pointer is activated via the `Pointer` alias on the `Controller Events` and the UI pointer click state is triggered via the `UI Click` alias on the `Controller Events`.

### Inspector Parameters

 * **Activation Button:** The button used to activate/deactivate the UI raycast for the pointer.
 * **Activation Mode:** Determines when the UI pointer should be active.
 * **Selection Button:** The button used to execute the select action at the pointer's target position.
 * **Click Method:** Determines when the UI Click event action should happen.
 * **Attempt Click On Deactivate:** Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`
 * **Click After Hover Duration:** The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.
 * **Controller:** The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Pointer Origin Transform:** A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.

### Class Variables

 * `public enum ActivationMethods` - Methods of activation.
  * `HoldButton` - Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.
  * `ToggleButton` - Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.
  * `AlwaysOn` - The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.
 * `public enum ClickMethods` - Methods of when to consider a UI Click action
  * `ClickOnButtonUp` - Consider a UI Click action has happened when the UI Click alias button is released.
  * `ClickOnButtonDown` - Consider a UI Click action has happened when the UI Click alias button is pressed.
 * `public GameObject autoActivatingCanvas` - The GameObject of the front trigger activator of the canvas currently being activated by this pointer. Default: `null`
 * `public bool collisionClick` - Determines if the UI Pointer has collided with a valid canvas that has collision click turned on. Default: `false`

### Class Events

 * `UIPointerElementEnter` - Emitted when the UI Pointer is colliding with a valid UI element.
 * `UIPointerElementExit` - Emitted when the UI Pointer is no longer colliding with any valid UI elements.
 * `UIPointerElementClick` - Emitted when the UI Pointer has clicked the currently collided UI element.
 * `UIPointerElementDragStart` - Emitted when the UI Pointer begins dragging a valid UI element.
 * `UIPointerElementDragEnd` - Emitted when the UI Pointer stops dragging a valid UI element.

### Unity Events

Adding the `VRTK_UIPointer_UnityEvents` component to `VRTK_UIPointer` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnUIPointerElementEnter` - Emits the UIPointerElementEnter class event.
 * `OnUIPointerElementExit` - Emits the UIPointerElementExit class event.
 * `OnUIPointerElementClick` - Emits the UIPointerElementClick class event.
 * `OnUIPointerElementDragStart` - Emits the UIPointerElementDragStart class event.
 * `OnUIPointerElementDragEnd` - Emits the UIPointerElementDragEnd class event.

### Event Payload

 * `uint controllerIndex` - The index of the controller that was used.
 * `bool isActive` - The state of whether the UI Pointer is currently active or not.
 * `GameObject currentTarget` - The current UI element that the pointer is colliding with.
 * `GameObject previousTarget` - The previous UI element that the pointer was colliding with.

### Class Methods

#### SetEventSystem/1

  > `public virtual VRTK_EventSystemVRInput SetEventSystem(EventSystem eventSystem)`

  * Parameters
   * `EventSystem eventSystem` - The global Unity event system to be used by the UI pointers.
  * Returns
   * `VRTK_EventSystemVRInput` - A custom event system input class that is used to detect input from VR pointers.

The SetEventSystem method is used to set up the global Unity event system for the UI pointer. It also handles disabling the existing Standalone Input Module that exists on the EventSystem and adds a custom VRTK Event System VR Input component that is required for interacting with the UI with VR inputs.

#### RemoveEventSystem/0

  > `public virtual void RemoveEventSystem()`

  * Parameters
   * _none_
  * Returns
   * _none_

The RemoveEventSystem resets the Unity EventSystem back to the original state before the VRTK_EventSystemVRInput was swapped for it.

#### PointerActive/0

  > `public virtual bool PointerActive()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the ui pointer should be currently active.

The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.

#### SelectionButtonActive/0

  > `public virtual bool SelectionButtonActive()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the selection button is active.

The SelectionButtonActive method is used to determine if the configured selection button is currently in the active state.

#### ValidClick/2

  > `public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)`

  * Parameters
   * `bool checkLastClick` - If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.
   * `bool lastClickState` - This determines what the last frame's state of the UI Click button should be in for it to be a valid click.
  * Returns
   * `bool` - Returns true if the UI Click button is in a valid state to action a click, returns false if it is not in a valid state.

The ValidClick method determines if the UI Click button is in a valid state to register a click action.

#### GetOriginPosition/0

  > `public virtual Vector3 GetOriginPosition()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 of the pointer transform position

The GetOriginPosition method returns the relevant transform position for the pointer based on whether the pointerOriginTransform variable is valid.

#### GetOriginForward/0

  > `public virtual Vector3 GetOriginForward()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 of the pointer transform forward

The GetOriginPosition method returns the relevant transform forward for the pointer based on whether the pointerOriginTransform variable is valid.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UIPointer` script on the right Controller to allow for the interaction with Unity UI elements using a Simple Pointer beam. The left Controller controls a Simple Pointer on the headset to demonstrate gaze interaction with Unity UI elements.

---

## UI Draggable Item (VRTK_UIDraggableItem)
 > extends MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler

### Overview

The UI Draggable item will make any UI element draggable on the canvas.

If a UI Draggable item is set to `Restrict To Drop Zone = true` then the UI Draggable item must be a child of an element that has the VRTK_UIDropZone script applied to it to ensure it starts in a valid drop zone.

### Inspector Parameters

 * **Restrict To Drop Zone:** If checked then the UI element can only be dropped in valid a VRTK_UIDropZone object and must start as a child of a VRTK_UIDropZone object. If unchecked then the UI element can be dropped anywhere on the canvas.
 * **Restrict To Original Canvas:** If checked then the UI element can only be dropped on the original parent canvas. If unchecked the UI element can be dropped on any valid VRTK_UICanvas.
 * **Forward Offset:** The offset to bring the UI element forward when it is being dragged.

### Class Variables

 * `public GameObject validDropZone` - The current valid drop zone the dragged element is hovering over.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI elements that are draggable

---

## UI Drop Zone (VRTK_UIDropZone)
 > extends MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

### Overview

A UI Drop Zone is applied to any UI element that is to be considered a valid parent for any UI Draggable element to be dropped into it.

It's usually appropriate to use a Panel UI element as a drop zone with a layout group applied so new children dropped into the drop zone automatically align.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI Drop Zones.

---

# 3D Controls (VRTK/Scripts/Controls/3D)

In order to interact with the world beyond grabbing and throwing, controls can be used to mimic real-life objects.

A number of controls are available which partially support auto-configuration. So can a slider for example detect its min and max points or a button the distance until a push event should be triggered. In the editor gizmos will be drawn that show the current settings. A yellow gizmo signals a valid configuration. A red one shows that the position of the object should change or switch to manual configuration mode.

All 3D controls extend the `VRTK_Control` abstract class which provides common methods and events.

 * [Control](#control-vrtk_control)
 * [Button](#button-vrtk_button)
 * [Chest](#chest-vrtk_chest)
 * [Door](#door-vrtk_door)
 * [Drawer](#drawer-vrtk_drawer)
 * [Knob](#knob-vrtk_knob)
 * [Wheel](#wheel-vrtk_wheel)
 * [Lever](#lever-vrtk_lever)
 * [Spring Lever](#spring-lever-vrtk_springlever)
 * [Slider](#slider-vrtk_slider)
 * [Content Handler](#content-handler-vrtk_contenthandler)

---

## Control (VRTK_Control)

### Overview

All 3D controls extend the `VRTK_Control` abstract class which provides a default set of methods and events that all of the subsequent controls expose.

### Inspector Parameters

 * **Interact Without Grab:** If active the control will react to the controller without the need to push the grab button.

### Class Variables

 * `public ValueChangedEvent OnValueChanged` - Emitted when the control is interacted with.
 * `public enum Direction` - 3D Control Directions
  * `autodetect` - Attempt to auto detect the axis
  * `x` - X axis
  * `y` - Y axis
  * `z` - Z axis

### Class Events

 * `ValueChanged` - Emitted when the 3D Control value has changed.

### Unity Events

Adding the `VRTK_Control_UnityEvents` component to `VRTK_Control` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnValueChanged` - Emits the ValueChanged class event.

### Event Payload

 * `float value` - The current value being reported by the control.
 * `float normalizedValue` - The normalized value being reported by the control.

### Class Methods

#### GetValue/0

  > `public float GetValue()`

  * Parameters
   * _none_
  * Returns
   * `float` - The current value of the control.

The GetValue method returns the current value/position/setting of the control depending on the control that is extending this abstract class.

#### GetNormalizedValue/0

  > `public float GetNormalizedValue()`

  * Parameters
   * _none_
  * Returns
   * `float` - The current normalized value of the control.

The GetNormalizedValue method returns the current value mapped onto a range between 0 and 100.

#### SetContent/2

  > `public void SetContent(GameObject content, bool hideContent)`

  * Parameters
   * `GameObject content` - The content to be considered within the control.
   * `bool hideContent` - When true the content will be hidden in addition to being non-interactable in case the control is fully closed.
  * Returns
   * _none_

The SetContent method sets the given game object as the content of the control. This will then disable and optionally hide the content when a control is obscuring its view to prevent interacting with content within a control.

#### GetContent/0

  > `public GameObject GetContent()`

  * Parameters
   * _none_
  * Returns
   * `GameObject` - The currently stored content for the control.

The GetContent method returns the current game object of the control's content.

---

## Button (VRTK_Button)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Attaching the script to a game object will allow the user to interact with it as if it were a push button. The direction into which the button should be pushable can be freely set and auto-detection is supported. Since this is physics-based there needs to be empty space in the push direction so that the button can move.

The script will instantiate the required Rigidbody and ConstantForce components automatically in case they do not exist yet.

### Inspector Parameters

 * **Connected To:** An optional game object to which the button will be connected. If the game object moves the button will follow along.
 * **Direction:** The axis on which the button should move. All other axis will be frozen.
 * **Activation Distance:** The local distance the button needs to be pushed until a push event is triggered.
 * **Button Strength:** The amount of force needed to push the button down as well as the speed with which it will go back into its original position.

### Class Variables

 * `public enum ButtonDirection` - 3D Control Button Directions
  * `autodetect` - Attempt to auto detect the axis
  * `x` - X axis
  * `y` - Y axis
  * `z` - Z axis
  * `negX` - Negative X axis
  * `negY` - Negative Y axis
  * `negZ` - Negative Z axis

### Class Events

 * `Pushed` - Emitted when the 3D Button has reached it's activation distance.

### Unity Events

Adding the `VRTK_Button_UnityEvents` component to `VRTK_Button` object allows access to `UnityEvents` that will react identically to the Class Events.

 * `OnPushed` - Emits the Pushed class event.

### Event Payload

 * `float value` - The current value being reported by the control.
 * `float normalizedValue` - The normalized value being reported by the control.

### Example

`VRTK/Examples/025_Controls_Overview` shows a collection of pressable buttons that are interacted with by activating the rigidbody on the controller by pressing the grab button without grabbing an object.

---

## Chest (VRTK_Chest)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Transforms a game object into a chest with a lid. The direction can be auto-detected with very high reliability or set manually.

The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. It will expect three distinct game objects: a body, a lid and a handle. These should be independent and not children of each other.

### Inspector Parameters

 * **Direction:** The axis on which the chest should open. All other axis will be frozen.
 * **Lid:** The game object for the lid.
 * **Body:** The game object for the body.
 * **Handle:** The game object for the handle.
 * **Content:** The parent game object for the chest content elements.
 * **Hide Content:** Makes the content invisible while the chest is closed.
 * **Max Angle:** The maximum opening angle of the chest.

### Example

`VRTK/Examples/025_Controls_Overview` shows a chest that can be open and closed, it also displays the current opening angle of the chest.

---

## Door (VRTK_Door)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Transforms a game object into a door with an optional handle attached to an optional frame. The direction can be freely set and also very reliably auto-detected.

There are situations when it can be very hard to automatically calculate the correct axis and anchor values for the hinge joint. If this situation is encountered then simply add the hinge joint manually and set these two values. All the rest will still be handled by the script.

The script will instantiate the required Rigidbodies, Interactable and HingeJoint components automatically in case they do not exist yet. Gizmos will indicate the direction.

### Inspector Parameters

 * **Direction:** The axis on which the door should open.
 * **Door:** The game object for the door. Can also be an empty parent or left empty if the script is put onto the actual door mesh. If no colliders exist yet a collider will tried to be automatically attached to all children that expose renderers.
 * **Handles:** The game object for the handles. Can also be an empty parent or left empty. If empty the door can only be moved using the rigidbody mode of the controller. If no collider exists yet a compound collider made up of all children will try to be calculated but this will fail if the door is rotated. In that case a manual collider will need to be assigned.
 * **Frame:** The game object for the frame to which the door is attached. Should only be set if the frame will move as well to ensure that the door moves along with the frame.
 * **Content:** The parent game object for the door content elements.
 * **Hide Content:** Makes the content invisible while the door is closed.
 * **Max Angle:** The maximum opening angle of the door.
 * **Open Inward:** Can the door be pulled to open.
 * **Open Outward:** Can the door be pushed to open.
 * **Min Snap Close:** The range at which the door must be to being closed before it snaps shut. Only works if either inward or outward is selected, not both.
 * **Released Friction:** The amount of friction the door will have whilst swinging when it is not grabbed.
 * **Grabbed Friction:** The amount of friction the door will have whilst swinging when it is grabbed.
 * **Handle Interactable Only:** If this is checked then only the door handle is grabbale to operate the door.

### Example

`VRTK/Examples/025_Controls_Overview` shows a selection of door types, from a normal door and trapdoor, to a door with a cat-flap in the middle.

---

## Drawer (VRTK_Drawer)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Transforms a game object into a drawer. The direction can be freely set and also auto-detected with very high reliability.

The script will instantiate the required Rigidbody, Interactable and Joint components automatically in case they do not exist yet. There are situations when it can be very hard to automatically calculate the correct axis for the joint. If this situation is encountered simply add the configurable joint manually and set the axis. All the rest will still be handled by the script.

It will expect two distinct game objects: a body and a handle. These should be independent and not children of each other. The distance to which the drawer can be pulled out will automatically set depending on the length of it. If no body is specified the current object is assumed to be the body.

It is possible to supply a third game object which is the root of the contents inside the drawer. When this is specified the VRTK_InteractableObject components will be automatically deactivated in case the drawer is closed or not yet far enough open. This eliminates the issue that a user could grab an object inside a drawer although it is closed.

### Inspector Parameters

 * **Connected To:** An optional game object to which the drawer will be connected. If the game object moves the drawer will follow along.
 * **Direction:** The axis on which the drawer should open. All other axis will be frozen.
 * **Body:** The game object for the body.
 * **Handle:** The game object for the handle.
 * **Content:** The parent game object for the drawer content elements.
 * **Hide Content:** Makes the content invisible while the drawer is closed.
 * **Min Snap Close:** If the extension of the drawer is below this percentage then the drawer will snap shut.
 * **Max Extend:** The maximum percentage of the drawer's total length that the drawer will open to.

### Example

`VRTK/Examples/025_Controls_Overview` shows a drawer with contents that can be opened and closed freely and the contents can be removed from the drawer.

---

## Knob (VRTK_Knob)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Attaching the script to a game object will allow the user to interact with it as if it were a radial knob. The direction can be freely set.

The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.

### Inspector Parameters

 * **Connected To:** An optional game object to which the knob will be connected. If the game object moves the knob will follow along.
 * **Direction:** The axis on which the knob should rotate. All other axis will be frozen.
 * **Min:** The minimum value of the knob.
 * **Max:** The maximum value of the knob.
 * **Step Size:** The increments in which knob values can change.

### Example

`VRTK/Examples/025_Controls_Overview` has a couple of rotator knobs that can be rotated by grabbing with the controller and then rotating the controller in the desired direction.

---

## Wheel (VRTK_Wheel)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Attaching the script to a game object will allow the user to interact with it as if it were a spinnable wheel.

The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.

### Inspector Parameters

 * **Connected To:** An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.
 * **Grab Type:** The grab attach mechanic to use. Track Object allows for rotations of the controller, Rotator Track allows for grabbing the wheel and spinning it.
 * **Detatch Distance:** The maximum distance the grabbing controller is away from the wheel before it is automatically released.
 * **Minimum Value:** The minimum value the wheel can be set to.
 * **Maximum Value:** The maximum value the wheel can be set to.
 * **Step Size:** The increments in which values can change.
 * **Snap To Step:** If this is checked then when the wheel is released, it will snap to the step rotation.
 * **Grabbed Friction:** The amount of friction the wheel will have when it is grabbed.
 * **Released Friction:** The amount of friction the wheel will have when it is released.
 * **Max Angle:** The maximum angle the wheel has to be turned to reach it's maximum value.
 * **Lock At Limits:** If this is checked then the wheel cannot be turned beyond the minimum and maximum value.

### Example

`VRTK/Examples/025_Controls_Overview` has a collection of wheels that can be rotated by grabbing with the controller and then rotating the controller in the desired direction.

---

## Lever (VRTK_Lever)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Attaching the script to a game object will allow the user to interact with it as if it were a lever. The direction can be freely set.

The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. The joint is very tricky to setup automatically though and will only work in straight forward cases. If there are any issues, then create the HingeJoint component manually and configure it as needed.

### Inspector Parameters

 * **Connected To:** An optional game object to which the lever will be connected. If the game object moves the lever will follow along.
 * **Direction:** The axis on which the lever should rotate. All other axis will be frozen.
 * **Min Angle:** The minimum angle of the lever counted from its initial position.
 * **Max Angle:** The maximum angle of the lever counted from its initial position.
 * **Step Size:** The increments in which lever values can change.
 * **Released Friction:** The amount of friction the lever will have whilst swinging when it is not grabbed.
 * **Grabbed Friction:** The amount of friction the lever will have whilst swinging when it is grabbed.

### Example

`VRTK/Examples/025_Controls_Overview` has a couple of levers that can be grabbed and moved. One lever is horizontal and the other is vertical.

---

## Spring Lever (VRTK_SpringLever)
 > extends [VRTK_Lever](#lever-vrtk_lever)

### Overview

This script extends VRTK_Lever to add spring force toward whichever end of the lever's range it is closest to.

The script will instantiate the required Rigidbody, Interactable and HingeJoint components automatically in case they do not exist yet. The joint is very tricky to setup automatically though and will only work in straight forward cases. If there are any issues, then create the HingeJoint component manually and configure it as needed.

### Inspector Parameters

 * **Spring Strength:** The strength of the spring force that will be applied upon the lever.
 * **Spring Damper:** The damper of the spring force that will be applied upon the lever.
 * **Snap To Nearest Limit:** If this is checked then the spring will snap the lever to the nearest end point (either min or max angle). If it is unchecked, the lever will always snap to the min angle position.
 * **Always Active:** If this is checked then the spring will always be active even when grabbing the lever.

---

## Slider (VRTK_Slider)
 > extends [VRTK_Control](#control-vrtk_control)

### Overview

Attaching the script to a game object will allow the user to interact with it as if it were a horizontal or vertical slider. The direction can be freely set and auto-detection is supported.

The script will instantiate the required Rigidbody and Interactable components automatically in case they do not exist yet.

### Inspector Parameters

 * **Connected To:** An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.
 * **Direction:** The axis on which the slider should move. All other axis will be frozen.
 * **Minimum Limit:** The collider to specify the minimum limit of the slider.
 * **Maximum Limit:** The collider to specify the maximum limit of the slider.
 * **Minimum Value:** The minimum value of the slider.
 * **Maximum Value:** The maximum value of the slider.
 * **Step Size:** The increments in which slider values can change.
 * **Snap To Step:** If this is checked then when the slider is released, it will snap to the nearest value position.
 * **Released Friction:** The amount of friction the slider will have when it is released.

### Example

`VRTK/Examples/025_Controls_Overview` has a selection of sliders at various angles with different step values to demonstrate their usage.

---

## Content Handler (VRTK_ContentHandler)

### Overview

Manages objects defined as content. When taking out an object from a drawer and closing the drawer this object would otherwise disappear even if outside the drawer.

The script will use the boundaries of the control to determine if it is in or out and re-parent the object as necessary. It can be put onto individual objects or the parent of multiple objects. Using the latter way all interactable objects under that parent will become managed by the script.

### Inspector Parameters

 * **Control:** The 3D control responsible for the content.
 * **Inside:** The transform containing the meshes or colliders that define the inside of the control.
 * **Outside:** Any transform that will act as the parent while the object is not inside the control.

### Example

`VRTK/Examples/025_Controls_Overview` has a drawer with a collection of items that adhere to this concept.

---

# Utilities (VRTK/Scripts/Utilities)

A collection of scripts that provide useful functionality to aid the creation process.

 * [SDK Manager](#sdk-manager-vrtk_sdkmanager)
 * [Device Finder](#device-finder-vrtk_devicefinder)
 * [Shared Methods](#shared-methods-vrtk_sharedmethods)
 * [Policy List](#policy-list-vrtk_policylist)
 * [Adaptive Quality](#adaptive-quality-vrtk_adaptivequality)
 * [Object Transform Follow](#object-transform-follow-vrtk_objectfollow)
 * [Simulating Headset Movement](#simulating-headset-movement-vrtk_simulator)

---

## SDK Manager (VRTK_SDKManager)

### Overview

The SDK Manager script provides configuration of supported SDKs

### Inspector Parameters

 * **Persist On Load:** If this is true then the instance of the SDK Manager won't be destroyed on every scene load.
 * **System SDK:** The SDK to use to deal with all system actions.
 * **Boundaries SDK:** The SDK to use to utilise room scale boundaries.
 * **Headset SDK:** The SDK to use to utilise the VR headset.
 * **Controller SDK:** The SDK to use to utilise the input devices.
 * **Auto Manage Script Defines:** This determines whether the scripting define symbols required by the selected SDKs are automatically added to the player settings when using the SDK Manager inspector window.
 * **Actual Boundaries:** A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.
 * **Actual Headset:** A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.
 * **Actual Left Controller:** A reference to the GameObject that contains the SDK Left Hand Controller.
 * **Actual Right Controller:** A reference to the GameObject that contains the SDK Right Hand Controller.
 * **Model Alias Left Controller:** A reference to the GameObject that models for the Left Hand Controller.
 * **Model Alias Right Controller:** A reference to the GameObject that models for the Right Hand Controller
 * **Script Alias Left Controller:** A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.
 * **Script Alias Right Controller:** A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.

### Class Variables

 * `public enum SupportedSDKs` - The supported SDKs
 * `public static VRTK_SDKManager instance` - The singleton instance to access the SDK Manager variables from. Default: `null`

### Class Methods

#### GetSystemSDK/0

  > `public SDK_BaseSystem GetSystemSDK()`

  * Parameters
   * _none_
  * Returns
   * `SDK_BaseSystem` - The currently selected System SDK

The GetSystemSDK method returns the selected system SDK

#### GetHeadsetSDK/0

  > `public SDK_BaseHeadset GetHeadsetSDK()`

  * Parameters
   * _none_
  * Returns
   * `SDK_BaseHeadset` - The currently selected Headset SDK

The GetHeadsetSDK method returns the selected headset SDK

#### GetControllerSDK/0

  > `public SDK_BaseController GetControllerSDK()`

  * Parameters
   * _none_
  * Returns
   * `SDK_BaseController` - The currently selected Controller SDK

The GetControllerSDK method returns the selected controller SDK

#### GetBoundariesSDK/0

  > `public SDK_BaseBoundaries GetBoundariesSDK()`

  * Parameters
   * _none_
  * Returns
   * `SDK_BaseBoundaries` - The currently selected Boundaries SDK

The GetBoundariesSDK method returns the selected boundaries SDK

---

## Device Finder (VRTK_DeviceFinder)

### Overview

The Device Finder offers a collection of static methods that can be called to find common game devices such as the headset or controllers, or used to determine key information about the connected devices.

### Class Variables

 * `public enum Devices` - Possible devices.
  * `Headset` - The headset.
  * `Left_Controller` - The left hand controller.
  * `Right_Controller` - The right hand controller.
 * `public enum Headsets` - Possible headsets
  * `Unknown` - An unknown headset.
  * `OculusRift` - A summary of all Oculus Rift headset versions.
  * `OculusRiftCV1` - A specific version of the Oculus Rift headset, the Consumer Version 1.
  * `Vive` - A summary of all HTC Vive headset versions.
  * `ViveMV` - A specific version of the HTC Vive headset, the first consumer version.

### Class Methods

#### GetControllerIndex/1

  > `public static uint GetControllerIndex(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller object to get the index of a controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method is used to find the index of a given controller object.

#### GetControllerByIndex/2

  > `public static GameObject GetControllerByIndex(uint index, bool getActual)`

  * Parameters
   * `uint index` - The index of the actual controller to find.
   * `bool getActual` - An optional parameter that if true will return the game object that the SDK controller is attached to.
  * Returns
   * `GameObject` - The actual controller GameObject that matches the given index.

The GetControllerByIndex method is used to find a controller based on it's unique index.

#### GetControllerOrigin/1

  > `public static Transform GetControllerOrigin(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to get the origin for.
  * Returns
   * `Transform` - The transform of the controller origin or if an origin is not set then the transform parent.

The GetControllerOrigin method is used to find the controller's origin.

#### DeviceTransform/1

  > `public static Transform DeviceTransform(Devices device)`

  * Parameters
   * `Devices device` - The Devices enum to get the transform for.
  * Returns
   * `Transform` - The transform for the given Devices enum.

The DeviceTransform method returns the transform for a given Devices enum.

#### GetControllerHandType/1

  > `public static SDK_BaseController.ControllerHand GetControllerHandType(string hand)`

  * Parameters
   * `string hand` - The string representation of the hand to retrieve the type of. `left` or `right`.
  * Returns
   * `SDK_BaseController.ControllerHand` - A ControllerHand representing either the Left or Right hand.

The GetControllerHandType method is used for getting the enum representation of ControllerHand from a given string.

#### GetControllerHand/1

  > `public static SDK_BaseController.ControllerHand GetControllerHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller game object to check the hand of.
  * Returns
   * `SDK_BaseController.ControllerHand` - A ControllerHand representing either the Left or Right hand.

The GetControllerHand method is used for getting the enum representation of ControllerHand for the given controller game object.

#### GetControllerLeftHand/1

  > `public static GameObject GetControllerLeftHand(bool getActual = false)`

  * Parameters
   * `bool getActual` - An optional parameter that if true will return the game object that the SDK controller is attached to.
  * Returns
   * `GameObject` - The left hand controller.

The GetControllerLeftHand method retrieves the game object for the left hand controller.

#### GetControllerRightHand/1

  > `public static GameObject GetControllerRightHand(bool getActual = false)`

  * Parameters
   * `bool getActual` - An optional parameter that if true will return the game object that the SDK controller is attached to.
  * Returns
   * `GameObject` - The right hand controller.

The GetControllerRightHand method retrieves the game object for the right hand controller.

#### IsControllerOfHand/2

  > `public static bool IsControllerOfHand(GameObject checkController, SDK_BaseController.ControllerHand hand)`

  * Parameters
   * `GameObject checkController` - The actual controller object that is being checked.
   * `SDK_BaseController.ControllerHand hand` - The representation of a hand to check if the given controller matches.
  * Returns
   * `bool` - Is true if the given controller matches the given hand.

The IsControllerOfHand method is used to check if a given controller game object is of the hand type provided.

#### IsControllerLeftHand/1

  > `public static bool IsControllerLeftHand(GameObject checkController)`

  * Parameters
   * `GameObject checkController` - The controller object that is being checked.
  * Returns
   * `bool` - Is true if the given controller is the left controller.

The IsControllerLeftHand method is used to check if a given controller game object is the left handed controller.

#### IsControllerRightHand/1

  > `public static bool IsControllerRightHand(GameObject checkController)`

  * Parameters
   * `GameObject checkController` - The controller object that is being checked.
  * Returns
   * `bool` - Is true if the given controller is the right controller.

The IsControllerRightHand method is used to check if a given controller game object is the right handed controller.

#### GetActualController/1

  > `public static GameObject GetActualController(GameObject givenController)`

  * Parameters
   * `GameObject givenController` - The GameObject of the controller.
  * Returns
   * `GameObject` - The GameObject that is the actual controller.

The GetActualController method will attempt to get the actual SDK controller object.

#### GetScriptAliasController/1

  > `public static GameObject GetScriptAliasController(GameObject givenController)`

  * Parameters
   * `GameObject givenController` - The GameObject of the controller.
  * Returns
   * `GameObject` - The GameObject that is the alias controller containing the scripts.

The GetScriptAliasController method will attempt to get the object that contains the scripts for the controller.

#### GetModelAliasController/1

  > `public static GameObject GetModelAliasController(GameObject givenController)`

  * Parameters
   * `GameObject givenController` - The GameObject of the controller.
  * Returns
   * `GameObject` - The GameObject that is the alias controller containing the controller model.

The GetModelAliasController method will attempt to get the object that contains the model for the controller.

#### GetControllerVelocity/1

  > `public static Vector3 GetControllerVelocity(GameObject givenController)`

  * Parameters
   * `GameObject givenController` - The GameObject of the controller.
  * Returns
   * `Vector3` - A 3 dimensional vector containing the current real world physical controller velocity.

The GetControllerVelocity method is used for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.

#### GetControllerAngularVelocity/1

  > `public static Vector3 GetControllerAngularVelocity(GameObject givenController)`

  * Parameters
   * `GameObject givenController` - The GameObject of the controller.
  * Returns
   * `Vector3` - A 3 dimensional vector containing the current real world physical controller angular (rotational) velocity.

The GetControllerAngularVelocity method is used for getting the current rotational velocity of the physical game controller. This can be useful for determining which way the controller is being rotated and at what speed the rotation is occurring.

#### GetHeadsetVelocity/0

  > `public static Vector3 GetHeadsetVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public static Vector3 GetHeadsetAngularVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetTransform/0

  > `public static Transform HeadsetTransform()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - The transform of the VR Headset component.

The HeadsetTransform method is used to retrieve the transform for the VR Headset in the scene. It can be useful to determine the position of the user's head in the game world.

#### HeadsetCamera/0

  > `public static Transform HeadsetCamera()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - The transform of the VR Camera component.

The HeadsetCamera method is used to retrieve the transform for the VR Camera in the scene.

#### GetHeadsetType/1

  > `public static Headsets GetHeadsetType(bool summary = false)`

  * Parameters
   * `bool summary` - If this is true, then the generic name for the headset is returned not including the version type (e.g. OculusRift will be returned for DK2 and CV1).
  * Returns
   * `Headsets` - The Headset type that is connected.

The GetHeadsetType method returns the type of headset connected to the computer.

#### PlayAreaTransform/0

  > `public static Transform PlayAreaTransform()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - The transform of the VR Play Area component.

The PlayAreaTransform method is used to retrieve the transform for the play area in the scene.

---

## Shared Methods (VRTK_SharedMethods)

### Overview

The Shared Methods script is a collection of reusable static methods that are used across a range of different scripts.

### Class Methods

#### GetBounds/3

  > `public static Bounds GetBounds(Transform transform, Transform excludeRotation = null, Transform excludeTransform = null)`

  * Parameters
   * `Transform transform` -
   * `Transform excludeRotation` - Resets the rotation of the transform temporarily to 0 to eliminate skewed bounds.
   * `Transform excludeTransform` - Does not consider the stated object when calculating the bounds.
  * Returns
   * `Bounds` - The bounds of the transform.

The GetBounds methods returns the bounds of the transform including all children in world space.

#### IsLowest/2

  > `public static bool IsLowest(float value, float[] others)`

  * Parameters
   * `float value` - The value to check to see if it is lowest.
   * `float[] others` - The array of values to check against.
  * Returns
   * `bool` - Returns true if the value is lower than all numbers in the given array, returns false if it is not the lowest.

The IsLowest method checks to see if the given value is the lowest number in the given array of values.

#### AddCameraFade/0

  > `public static Transform AddCameraFade()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - The transform of the headset camera.

The AddCameraFade method finds the headset camera and adds a headset fade script to it.

#### CreateColliders/1

  > `public static void CreateColliders(GameObject obj)`

  * Parameters
   * `GameObject obj` - The game object to attempt to add the colliders to.
  * Returns
   * _none_

The CreateColliders method attempts to add box colliders to all child objects in the given object that have a renderer but no collider.

#### CloneComponent/3

  > `public static Component CloneComponent(Component source, GameObject destination, bool copyProperties = false)`

  * Parameters
   * `Component source` - The component to copy.
   * `GameObject destination` - The game object to copy the component to.
   * `bool copyProperties` - Determines whether the properties of the component as well as the fields should be copied.
  * Returns
   * `Component` - The component that has been cloned onto the given game object.

The CloneComponent method takes a source component and copies it to the given destination game object.

#### ColorDarken/2

  > `public static Color ColorDarken(Color color, float percent)`

  * Parameters
   * `Color color` - The source colour to apply the darken to.
   * `float percent` - The percent to darken the colour by.
  * Returns
   * `Color` - The new colour with the darken applied.

The ColorDarken method takes a given colour and darkens it by the given percentage.

#### IsEditTime/0

  > `public static bool IsEditTime()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if Unity is in the Unity Editor and not in play mode.

The IsEditTime method determines if the state of Unity is in the Unity Editor and the scene is not in play mode.

---

## Policy List (VRTK_PolicyList)

### Overview

The Policy List allows to create a list of either tag names, script names or layer names that can be checked against to see if another operation is permitted.

A number of other scripts can use a Policy List to determine if an operation is permitted based on whether a game object has a tag applied, a script component on it or whether it's on a given layer.

For example, the Teleporter scripts can ignore game object targets as a teleport location if the game object contains a tag that is in the identifiers list and the policy is set to ignore.

Or the teleporter can only allow teleport to targets that contain a tag that is in the identifiers list and the policy is set to include.

Add the Policy List script to a game object (preferably the same component utilising the list) and then configure the list accordingly.

Then in the component that has a Policy List paramter (e.g. BasicTeleporter has `Target List Policy`) simply select the list that has been created and defined.

### Inspector Parameters

 * **Operation:** The operation to apply on the list of identifiers.
 * **Check Type:** The element type on the game object to check against.

### Class Variables

 * `public enum OperationTypes` - The operation to apply on the list of identifiers.
  * `Ignore` - Will ignore any game objects that contain either a tag or script component that is included in the identifiers list.
  * `Include` - Will only include game objects that contain either a tag or script component that is included in the identifiers list.
 * `public enum CheckTypes` - The types of element that can be checked against.
  * `Tag` - The tag applied to the game object.
  * `Script` - A script component added to the game object.
  * `Layer` - A layer applied to the game object.

### Class Methods

#### Find/1

  > `public virtual bool Find(GameObject obj)`

  * Parameters
   * `GameObject obj` - The game object to check if it has a tag or script that is listed in the identifiers list.
  * Returns
   * `bool` - If the operation is `Ignore` and the game object is matched by an identifier from the list then it returns true. If the operation is `Include` and the game object is not matched by an identifier from the list then it returns true.

The Find method performs the set operation to determine if the given game object contains one of the identifiers on the set check type. For instance, if the Operation is `Ignore` and the Check Type is `Tag` then the Find method will attempt to see if the given game object has a tag that matches one of the identifiers.

#### Check/2

  > `public static bool Check(GameObject obj, VRTK_PolicyList list)`

  * Parameters
   * `GameObject obj` - The game object to check.
   * `VRTK_PolicyList list` - The policy list to use for checking.
  * Returns
   * `bool` - Returns true of the given game object matches the policy list or given string logic.

The Check method is used to check if a game object should be ignored based on a given string or policy list.

---

## Adaptive Quality (VRTK_AdaptiveQuality)

### Overview

Adaptive Quality dynamically changes rendering settings to maintain VR framerate while maximizing GPU utilization.

> **Only Compatible With Unity 5.4 and above**

There are two goals:
 * Reduce the chances of dropping frames and reprojecting
 * Increase quality when there are idle GPU cycles

This script currently changes the following to reach these goals:
 * Rendering resolution and viewport size (aka Dynamic Resolution)

In the future it could be changed to also change the following:
 * MSAA level
 * Fixed Foveated Rendering
 * Radial Density Masking
 * (Non-fixed) Foveated Rendering (once HMDs support eye tracking)

Some shaders, especially Image Effects, need to be modified to work with the changed render scale. To fix them
pass `1.0f / VRSettings.renderViewportScale` into the shader and scale all incoming UV values with it in the vertex
program. Do this by using `Material.SetFloat` to set the value in the script that configures the shader.

In more detail:
 * In the `.shader` file: Add a new runtime-set property value `float _InverseOfRenderViewportScale` and add `vertexInput.texcoord *= _InverseOfRenderViewportScale` to the start of the vertex program
 * In the `.cs` file: Before using the material (eg. `Graphics.Blit`) add `material.SetFloat("_InverseOfRenderViewportScale", 1.0f / VRSettings.renderViewportScale)`

### Inspector Parameters

 * **Draw Debug Visualization:** Toggles whether to show the debug overlay. Each square represents a different level on the quality scale. Levels increase from left to right,  the first green box that is lit above represents the recommended render target resolution provided by the  current `VRDevice`, the box that is lit below in cyan represents the current resolution and the filled box  represents the current viewport scale. The yellow boxes represent resolutions below the recommended render target resolution. The currently lit box becomes red whenever the user is likely seeing reprojection in the HMD since the  application isn't maintaining VR framerate. If lit, the box all the way on the left is almost always lit  red because it represents the lowest render scale with reprojection on.
 * **Allow Keyboard Shortcuts:** Toggles whether to allow keyboard shortcuts to control this script.
  * The supported shortcuts are:
    * `Shift+F1`: Toggle debug visualization on/off
    * `Shift+F2`: Toggle usage of override render scale on/off
    * `Shift+F3`: Decrease override render scale level
    * `Shift+F4`: Increase override render scale level
 * **Allow Command Line Arguments:** Toggles whether to allow command line arguments to control this script at startup of the standalone build.
  * The supported command line arguments all begin with '-' and are:
    * `-noaq`: Disable adaptive quality
    * `-aqminscale X`: Set minimum render scale to X
    * `-aqmaxscale X`: Set maximum render scale to X
    * `-aqmaxres X`: Set maximum render target dimension to X
    * `-aqfillratestep X`: Set render scale fill rate step size in percent to X (X from 1 to 100)
    * `-aqoverride X`: Set override render scale level to X
    * `-vrdebug`: Enable debug visualization
    * `-msaa X`: Set MSAA level to X
 * **Msaa Level:** The MSAA level to use.
 * **Scale Render Viewport:** Toggles whether the render viewport scale is dynamically adjusted to maintain VR framerate. If unchecked, the renderer will render at the recommended resolution provided by the current `VRDevice`.
 * **Minimum Render Scale:** The minimum allowed render scale.
 * **Maximum Render Scale:** The maximum allowed render scale.
 * **Maximum Render Target Dimension:** The maximum allowed render target dimension. This puts an upper limit on the size of the render target regardless of the maximum render scale.
 * **Render Scale Fill Rate Step Size In Percent:** The fill rate step size in percent by which the render scale levels will be calculated.
 * **Scale Render Target Resolution:** Toggles whether the render target resolution is dynamically adjusted to maintain VR framerate. If unchecked, the renderer will use the maximum target resolution specified by `maximumRenderScale`.
 * **Override Render Viewport Scale:** Toggles whether to override the used render viewport scale level.
 * **Override Render Viewport Scale Level:** The render viewport scale level to override the current one with.

### Class Variables

 * `public readonly ReadOnlyCollection<float> renderScales` - All the calculated render scales. The elements of this collection are to be interpreted as modifiers to the recommended render target resolution provided by the current `VRDevice`.
 * `public static float CurrentRenderScale` - The current render scale. A render scale of `1.0` represents the recommended render target resolution provided by the current `VRDevice`.
 * `public Vector2 defaultRenderTargetResolution` - The recommended render target resolution provided by the current `VRDevice`.
 * `public Vector2 currentRenderTargetResolution` - The current render target resolution.

### Class Methods

#### RenderTargetResolutionForRenderScale/1

  > `public static Vector2 RenderTargetResolutionForRenderScale(float renderScale)`

  * Parameters
   * `float renderScale` - The render scale to calculate the render target resolution with.
  * Returns
   * `Vector2` - The render target resolution for `renderScale`.

Calculates and returns the render target resolution for a given render scale.

#### BiggestAllowedMaximumRenderScale/0

  > `public float BiggestAllowedMaximumRenderScale()`

  * Parameters
   * _none_
  * Returns
   * `float` - The biggest allowed maximum render scale.

Calculates and returns the biggest allowed maximum render scale to be used for `maximumRenderScale` given the current `maximumRenderTargetDimension`.

#### ToString/0

  > `public override string ToString()`

  * Parameters
   * _none_
  * Returns
   * `string` - The summary.

A summary of this script by listing all the calculated render scales with their corresponding render target resolution.

### Example

`VRTK/Examples/039_CameraRig_AdaptiveQuality` displays the frames per second in the centre of the headset view.
The debug visualization of this script is displayed near the top edge of the headset view.
Pressing the trigger generates a new sphere and pressing the touchpad generates ten new spheres.
Eventually when lots of spheres are present the FPS will drop and demonstrate the script.

---

## Object Transform Follow (VRTK_ObjectFollow)

### Overview

A simple script that when attached to a GameObject will follow the position, scale and rotation of the given Transform.

### Inspector Parameters

 * **Object To Follow:** A transform of an object to follow the position, scale and rotation of.
 * **Follow Position:** Follow the position of the given object.
 * **Follow Rotation:** Follow the rotation of the given object.
 * **Follow Scale:** Follow the scale of the given object.

---

## Simulating Headset Movement (VRTK_Simulator)

### Overview

To test a scene it is often necessary to use the headset to move to a location. This increases turn-around times and can become cumbersome.

The simulator allows navigating through the scene using the keyboard instead, without the need to put on the headset. One can then move around (also through walls) while looking at the monitor and still use the controllers to interact.

Supported movements are: forward, backward, strafe left, strafe right, turn left, turn right, up, down.

### Inspector Parameters

 * **Keys:** Per default the keys on the left-hand side of the keyboard are used (WASD). They can be individually set as needed. The reset key brings the camera to its initial location.
 * **Only In Editor:** Typically the simulator should be turned off when not testing anymore. This option will do this automatically when outside the editor.
 * **Step Size:** Depending on the scale of the world the step size can be defined to increase or decrease movement speed.
 * **Cam Start:** An optional game object marking the position and rotation at which the camera should be initially placed.

---

# Base SDK (VRTK/SDK/Base)

The base scripts used to determine the interface for interacting with a Unity VR SDK.

 * [Base System](#base-system-sdk_basesystem)
 * [Base Headset](#base-headset-sdk_baseheadset)
 * [Base Controller](#base-controller-sdk_basecontroller)
 * [Base Boundaries](#base-boundaries-sdk_baseboundaries)

---

## Base System (SDK_BaseSystem)
 > extends ScriptableObject

### Overview

The Base System SDK script provides a bridge to core system SDK methods.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Methods

#### IsDisplayOnDesktop/0

  > `public abstract bool IsDisplayOnDesktop();`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the display is extending the desktop

The IsDisplayOnDesktop method returns true if the display is extending the desktop.

#### ShouldAppRenderWithLowResources/0

  > `public abstract bool ShouldAppRenderWithLowResources();`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the Unity app should render with low resources.

The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.

#### ForceInterleavedReprojectionOn/1

  > `public abstract void ForceInterleavedReprojectionOn(bool force);`

  * Parameters
   * `bool force` - If true then Interleaved Reprojection will be forced on, if false it will not be forced on.
  * Returns
   * _none_

The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.

---

## Base Headset (SDK_BaseHeadset)
 > extends ScriptableObject

### Overview

The Base Headset SDK script provides a bridge to SDK methods that deal with the VR Headset.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Methods

#### ProcessUpdate/1

  > `public abstract void ProcessUpdate(Dictionary<string, object> options);`

  * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetHeadset/0

  > `public abstract Transform GetHeadset();`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the headset in the scene.

The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.

#### GetHeadsetCamera/0

  > `public abstract Transform GetHeadsetCamera();`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object holding the headset camera in the scene.

The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.

#### GetHeadsetVelocity/0

  > `public abstract Vector3 GetHeadsetVelocity();`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public abstract Vector3 GetHeadsetAngularVelocity();`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetFade/3

  > `public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);`

  * Parameters
   * `Color color` - The colour to fade to.
   * `float duration` - The amount of time the fade should take to reach the given colour.
   * `bool fadeOverlay` - Determines whether to use an overlay on the fade.
  * Returns
   * _none_

The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.

#### HasHeadsetFade/1

  > `public abstract bool HasHeadsetFade(Transform obj);`

  * Parameters
   * `Transform obj` - The Transform to check to see if a camera fade is available on.
  * Returns
   * `bool` - Returns true if the headset has fade functionality on it.

The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.

#### AddHeadsetFade/1

  > `public abstract void AddHeadsetFade(Transform camera);`

  * Parameters
   * `Transform camera` - The Transform to with the camera on to add the fade functionality to.
  * Returns
   * _none_

The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.

---

## Base Controller (SDK_BaseController)
 > extends ScriptableObject

### Overview

The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Variables

 * `public enum ButtonPressTypes` - Concepts of controller button press
  * `Press` - The button is currently being pressed.
  * `PressDown` - The button has just been pressed down.
  * `PressUp` - The button has just been released.
  * `Touch` - The button is currently being touched.
  * `TouchDown` - The button has just been touched.
  * `TouchUp` - The button is no longer being touched.
 * `public enum ControllerElements` - The elements of a generic controller
  * `AttachPoint` - The default point on the controller to attach grabbed objects to.
  * `Trigger` - The trigger button.
  * `GripLeft` - The left part of the grip button collection.
  * `GripRight` - The right part of the grip button collection.
  * `Touchpad` - The touch pad/stick.
  * `ButtonOne` - The first generic button.
  * `ButtonTwo` - The second generic button.
  * `SystemMenu` - The system menu button.
  * `Body` - The encompassing mesh of the controller body.
  * `StartMenu` - The start menu button.
 * `public enum ControllerHand` - Controller hand reference.
  * `None` - No hand is assigned.
  * `Left` - The left hand is assigned.
  * `Right` - The right hand is assigned.

### Class Methods

#### ProcessUpdate/2

  > `public abstract void ProcessUpdate(uint index, Dictionary<string, object> options);`

  * Parameters
   * `uint index` - The index of the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetControllerDefaultColliderPath/1

  > `public abstract string GetControllerDefaultColliderPath(ControllerHand hand);`

  * Parameters
   * `ControllerHand hand` - The controller hand to check for
  * Returns
   * `string` - A path to the resource that contains the collider GameObject.

The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.

#### GetControllerElementPath/3

  > `public abstract string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false);`

  * Parameters
   * `ControllerElements element` - The controller element to look up.
   * `ControllerHand hand` - The controller hand to look up.
   * `bool fullPath` - Whether to get the initial path or the full path to the element.
  * Returns
   * `string` - A string containing the path to the game object that the controller element resides in.

The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.

#### GetControllerIndex/1

  > `public abstract uint GetControllerIndex(GameObject controller);`

  * Parameters
   * `GameObject controller` - The GameObject containing the controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method returns the index of the given controller.

#### GetControllerByIndex/2

  > `public abstract GameObject GetControllerByIndex(uint index, bool actual = false);`

  * Parameters
   * `uint index` - The index of the controller to find.
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` -

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public abstract Transform GetControllerOrigin(GameObject controller);`

  * Parameters
   * `GameObject controller` - The controller to retrieve the origin from.
  * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

#### GenerateControllerPointerOrigin/1

  > `public abstract Transform GenerateControllerPointerOrigin(GameObject parent);`

  * Parameters
   * `GameObject parent` - The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.
  * Returns
   * `Transform` - A generated Transform that contains the custom pointer origin.

The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.

#### GetControllerLeftHand/1

  > `public abstract GameObject GetControllerLeftHand(bool actual = false);`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the left hand controller.

The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.

#### GetControllerRightHand/1

  > `public abstract GameObject GetControllerRightHand(bool actual = false);`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the right hand controller.

The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.

#### IsControllerLeftHand/1

  > `public abstract bool IsControllerLeftHand(GameObject controller);`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/1

  > `public abstract bool IsControllerRightHand(GameObject controller);`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.

#### IsControllerLeftHand/2

  > `public abstract bool IsControllerLeftHand(GameObject controller, bool actual);`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/2

  > `public abstract bool IsControllerRightHand(GameObject controller, bool actual);`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.

#### GetControllerModel/1

  > `public abstract GameObject GetControllerModel(GameObject controller);`

  * Parameters
   * `GameObject controller` - The GameObject to get the model alias for.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given GameObject.

#### GetControllerModel/1

  > `public abstract GameObject GetControllerModel(ControllerHand hand);`

  * Parameters
   * `ControllerHand hand` - The hand enum of which controller model to retrieve.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given controller hand.

#### GetControllerRenderModel/1

  > `public abstract GameObject GetControllerRenderModel(GameObject controller);`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `GameObject` - A GameObject containing the object that has a render model for the controller.

The GetControllerRenderModel method gets the game object that contains the given controller's render model.

#### SetControllerRenderModelWheel/2

  > `public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);`

  * Parameters
   * `GameObject renderModel` - The GameObject containing the controller render model.
   * `bool state` - If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.
  * Returns
   * _none_

The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.

#### HapticPulseOnIndex/2

  > `public abstract void HapticPulseOnIndex(uint index, float strength = 0.5f);`

  * Parameters
   * `uint index` - The index of the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.

#### GetHapticModifiers/0

  > `public abstract SDK_ControllerHapticModifiers GetHapticModifiers();`

  * Parameters
   * _none_
  * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocityOnIndex/1

  > `public abstract Vector3 GetVelocityOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.

#### GetAngularVelocityOnIndex/1

  > `public abstract Vector3 GetAngularVelocityOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.

#### GetTouchpadAxisOnIndex/1

  > `public abstract Vector2 GetTouchpadAxisOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current x,y position of where the touchpad is being touched.

The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.

#### GetTriggerAxisOnIndex/1

  > `public abstract Vector2 GetTriggerAxisOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the trigger.

The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.

#### GetGripAxisOnIndex/1

  > `public abstract Vector2 GetGripAxisOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the grip.

The GetGripAxisOnIndex method is used to get the current grip position on the controller.

#### GetTriggerHairlineDeltaOnIndex/1

  > `public abstract float GetTriggerHairlineDeltaOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the trigger presses.

The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.

#### GetGripHairlineDeltaOnIndex/1

  > `public abstract float GetGripHairlineDeltaOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the grip presses.

The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.

#### IsTriggerPressedOnIndex/1

  > `public abstract bool IsTriggerPressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTriggerPressedDownOnIndex/1

  > `public abstract bool IsTriggerPressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTriggerPressedUpOnIndex/1

  > `public abstract bool IsTriggerPressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTriggerTouchedOnIndex/1

  > `public abstract bool IsTriggerTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTriggerTouchedDownOnIndex/1

  > `public abstract bool IsTriggerTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTriggerTouchedUpOnIndex/1

  > `public abstract bool IsTriggerTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairTriggerDownOnIndex/1

  > `public abstract bool IsHairTriggerDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairTriggerUpOnIndex/1

  > `public abstract bool IsHairTriggerUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsGripPressedOnIndex/1

  > `public abstract bool IsGripPressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsGripPressedDownOnIndex/1

  > `public abstract bool IsGripPressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsGripPressedUpOnIndex/1

  > `public abstract bool IsGripPressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsGripTouchedOnIndex/1

  > `public abstract bool IsGripTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsGripTouchedDownOnIndex/1

  > `public abstract bool IsGripTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsGripTouchedUpOnIndex/1

  > `public abstract bool IsGripTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairGripDownOnIndex/1

  > `public abstract bool IsHairGripDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairGripUpOnIndex/1

  > `public abstract bool IsHairGripUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsTouchpadPressedOnIndex/1

  > `public abstract bool IsTouchpadPressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTouchpadPressedDownOnIndex/1

  > `public abstract bool IsTouchpadPressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTouchpadPressedUpOnIndex/1

  > `public abstract bool IsTouchpadPressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTouchpadTouchedOnIndex/1

  > `public abstract bool IsTouchpadTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTouchpadTouchedDownOnIndex/1

  > `public abstract bool IsTouchpadTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTouchpadTouchedUpOnIndex/1

  > `public abstract bool IsTouchpadTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOnePressedOnIndex/1

  > `public abstract bool IsButtonOnePressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonOnePressedDownOnIndex/1

  > `public abstract bool IsButtonOnePressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonOnePressedUpOnIndex/1

  > `public abstract bool IsButtonOnePressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOneTouchedOnIndex/1

  > `public abstract bool IsButtonOneTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonOneTouchedDownOnIndex/1

  > `public abstract bool IsButtonOneTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonOneTouchedUpOnIndex/1

  > `public abstract bool IsButtonOneTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoPressedOnIndex/1

  > `public abstract bool IsButtonTwoPressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonTwoPressedDownOnIndex/1

  > `public abstract bool IsButtonTwoPressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonTwoPressedUpOnIndex/1

  > `public abstract bool IsButtonTwoPressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoTouchedOnIndex/1

  > `public abstract bool IsButtonTwoTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonTwoTouchedDownOnIndex/1

  > `public abstract bool IsButtonTwoTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonTwoTouchedUpOnIndex/1

  > `public abstract bool IsButtonTwoTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuPressedOnIndex/1

  > `public abstract bool IsStartMenuPressedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsStartMenuPressedDownOnIndex/1

  > `public abstract bool IsStartMenuPressedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsStartMenuPressedUpOnIndex/1

  > `public abstract bool IsStartMenuPressedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuTouchedOnIndex/1

  > `public abstract bool IsStartMenuTouchedOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsStartMenuTouchedDownOnIndex/1

  > `public abstract bool IsStartMenuTouchedDownOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsStartMenuTouchedUpOnIndex/1

  > `public abstract bool IsStartMenuTouchedUpOnIndex(uint index);`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.

---

## Base Boundaries (SDK_BaseBoundaries)
 > extends ScriptableObject

### Overview

The Base Boundaries SDK script provides a bridge to SDK methods that deal with the play area of SDKs that support room scale play spaces.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Methods

#### InitBoundaries/0

  > `public abstract void InitBoundaries();`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.

#### GetPlayArea/0

  > `public abstract Transform GetPlayArea();`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the play area in the scene.

The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.

#### GetPlayAreaVertices/1

  > `public abstract Vector3[] GetPlayAreaVertices(GameObject playArea);`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/1

  > `public abstract float GetPlayAreaBorderThickness(GameObject playArea);`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/1

  > `public abstract bool IsPlayAreaSizeCalibrated(GameObject playArea);`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

---

# Fallback SDK (VRTK/SDK/Fallback)

The scripts used to provide a default/null fallback for any implemented SDK.

 * [Fallback System](#fallback-system-sdk_fallbacksystem)
 * [Fallback Headset](#fallback-headset-sdk_fallbackheadset)
 * [Fallback Controller](#fallback-controller-sdk_fallbackcontroller)
 * [Fallback Boundaries](#fallback-boundaries-sdk_fallbackboundaries)

---

## Fallback System (SDK_FallbackSystem)
 > extends [SDK_BaseSystem](#base-system-sdk_basesystem)

### Overview

The Fallback System SDK script provides a fallback collection of methods that return null or default system values.

This is the fallback class that will just return default values.

### Class Methods

#### IsDisplayOnDesktop/0

  > `public override bool IsDisplayOnDesktop()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the display is extending the desktop

The IsDisplayOnDesktop method returns true if the display is extending the desktop.

#### ShouldAppRenderWithLowResources/0

  > `public override bool ShouldAppRenderWithLowResources()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the Unity app should render with low resources.

The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.

#### ForceInterleavedReprojectionOn/1

  > `public override void ForceInterleavedReprojectionOn(bool force)`

  * Parameters
   * `bool force` - If true then Interleaved Reprojection will be forced on, if false it will not be forced on.
  * Returns
   * _none_

The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.

---

## Fallback Headset (SDK_FallbackHeadset)
 > extends [SDK_BaseHeadset](#base-headset-sdk_baseheadset)

### Overview

The Fallback System SDK script provides a fallback collection of methods that return null or default Headset values.

This is the fallback class that will just return default values.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

  * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetHeadset/0

  > `public override Transform GetHeadset()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the headset in the scene.

The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.

#### GetHeadsetCamera/0

  > `public override Transform GetHeadsetCamera()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object holding the headset camera in the scene.

The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.

#### GetHeadsetVelocity/0

  > `public override Vector3 GetHeadsetVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public override Vector3 GetHeadsetAngularVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetFade/3

  > `public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)`

  * Parameters
   * `Color color` - The colour to fade to.
   * `float duration` - The amount of time the fade should take to reach the given colour.
   * `bool fadeOverlay` - Determines whether to use an overlay on the fade.
  * Returns
   * _none_

The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.

#### HasHeadsetFade/1

  > `public override bool HasHeadsetFade(Transform obj)`

  * Parameters
   * `Transform obj` - The Transform to check to see if a camera fade is available on.
  * Returns
   * `bool` - Returns true if the headset has fade functionality on it.

The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.

#### AddHeadsetFade/1

  > `public override void AddHeadsetFade(Transform camera)`

  * Parameters
   * `Transform camera` - The Transform to with the camera on to add the fade functionality to.
  * Returns
   * _none_

The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.

---

## Fallback Controller (SDK_FallbackController)
 > extends [SDK_BaseController](#base-controller-sdk_basecontroller)

### Overview

The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.

This is the fallback class that will just return default values.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(uint index, Dictionary<string, object> options)`

  * Parameters
   * `uint index` - The index of the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetControllerDefaultColliderPath/1

  > `public override string GetControllerDefaultColliderPath(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The controller hand to check for
  * Returns
   * `string` - A path to the resource that contains the collider GameObject.

The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.

#### GetControllerElementPath/3

  > `public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)`

  * Parameters
   * `ControllerElements element` - The controller element to look up.
   * `ControllerHand hand` - The controller hand to look up.
   * `bool fullPath` - Whether to get the initial path or the full path to the element.
  * Returns
   * `string` - A string containing the path to the game object that the controller element resides in.

The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.

#### GetControllerIndex/1

  > `public override uint GetControllerIndex(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject containing the controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method returns the index of the given controller.

#### GetControllerByIndex/2

  > `public override GameObject GetControllerByIndex(uint index, bool actual = false)`

  * Parameters
   * `uint index` - The index of the controller to find.
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` -

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller to retrieve the origin from.
  * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

#### GenerateControllerPointerOrigin/1

  > `public override Transform GenerateControllerPointerOrigin(GameObject parent)`

  * Parameters
   * `GameObject parent` - The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.
  * Returns
   * `Transform` - A generated Transform that contains the custom pointer origin.

The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.

#### GetControllerLeftHand/1

  > `public override GameObject GetControllerLeftHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the left hand controller.

The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.

#### GetControllerRightHand/1

  > `public override GameObject GetControllerRightHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the right hand controller.

The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.

#### IsControllerLeftHand/1

  > `public override bool IsControllerLeftHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/1

  > `public override bool IsControllerRightHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.

#### IsControllerLeftHand/2

  > `public override bool IsControllerLeftHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/2

  > `public override bool IsControllerRightHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to get the model alias for.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given GameObject.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The hand enum of which controller model to retrieve.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given controller hand.

#### GetControllerRenderModel/1

  > `public override GameObject GetControllerRenderModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `GameObject` - A GameObject containing the object that has a render model for the controller.

The GetControllerRenderModel method gets the game object that contains the given controller's render model.

#### SetControllerRenderModelWheel/2

  > `public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)`

  * Parameters
   * `GameObject renderModel` - The GameObject containing the controller render model.
   * `bool state` - If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.
  * Returns
   * _none_

The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.

#### HapticPulseOnIndex/2

  > `public override void HapticPulseOnIndex(uint index, float strength = 0.5f)`

  * Parameters
   * `uint index` - The index of the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

  * Parameters
   * _none_
  * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocityOnIndex/1

  > `public override Vector3 GetVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.

#### GetAngularVelocityOnIndex/1

  > `public override Vector3 GetAngularVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.

#### GetTouchpadAxisOnIndex/1

  > `public override Vector2 GetTouchpadAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current x,y position of where the touchpad is being touched.

The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.

#### GetTriggerAxisOnIndex/1

  > `public override Vector2 GetTriggerAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the trigger.

The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.

#### GetGripAxisOnIndex/1

  > `public override Vector2 GetGripAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the grip.

The GetGripAxisOnIndex method is used to get the current grip position on the controller.

#### GetTriggerHairlineDeltaOnIndex/1

  > `public override float GetTriggerHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the trigger presses.

The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.

#### GetGripHairlineDeltaOnIndex/1

  > `public override float GetGripHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the grip presses.

The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.

#### IsTriggerPressedOnIndex/1

  > `public override bool IsTriggerPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTriggerPressedDownOnIndex/1

  > `public override bool IsTriggerPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTriggerPressedUpOnIndex/1

  > `public override bool IsTriggerPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTriggerTouchedOnIndex/1

  > `public override bool IsTriggerTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTriggerTouchedDownOnIndex/1

  > `public override bool IsTriggerTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTriggerTouchedUpOnIndex/1

  > `public override bool IsTriggerTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairTriggerDownOnIndex/1

  > `public override bool IsHairTriggerDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairTriggerUpOnIndex/1

  > `public override bool IsHairTriggerUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsGripPressedOnIndex/1

  > `public override bool IsGripPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsGripPressedDownOnIndex/1

  > `public override bool IsGripPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsGripPressedUpOnIndex/1

  > `public override bool IsGripPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsGripTouchedOnIndex/1

  > `public override bool IsGripTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsGripTouchedDownOnIndex/1

  > `public override bool IsGripTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsGripTouchedUpOnIndex/1

  > `public override bool IsGripTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairGripDownOnIndex/1

  > `public override bool IsHairGripDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairGripUpOnIndex/1

  > `public override bool IsHairGripUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsTouchpadPressedOnIndex/1

  > `public override bool IsTouchpadPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTouchpadPressedDownOnIndex/1

  > `public override bool IsTouchpadPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTouchpadPressedUpOnIndex/1

  > `public override bool IsTouchpadPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTouchpadTouchedOnIndex/1

  > `public override bool IsTouchpadTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTouchpadTouchedDownOnIndex/1

  > `public override bool IsTouchpadTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTouchpadTouchedUpOnIndex/1

  > `public override bool IsTouchpadTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOnePressedOnIndex/1

  > `public override bool IsButtonOnePressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonOnePressedDownOnIndex/1

  > `public override bool IsButtonOnePressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonOnePressedUpOnIndex/1

  > `public override bool IsButtonOnePressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOneTouchedOnIndex/1

  > `public override bool IsButtonOneTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonOneTouchedDownOnIndex/1

  > `public override bool IsButtonOneTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonOneTouchedUpOnIndex/1

  > `public override bool IsButtonOneTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoPressedOnIndex/1

  > `public override bool IsButtonTwoPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonTwoPressedDownOnIndex/1

  > `public override bool IsButtonTwoPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonTwoPressedUpOnIndex/1

  > `public override bool IsButtonTwoPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoTouchedOnIndex/1

  > `public override bool IsButtonTwoTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonTwoTouchedDownOnIndex/1

  > `public override bool IsButtonTwoTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonTwoTouchedUpOnIndex/1

  > `public override bool IsButtonTwoTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuPressedOnIndex/1

  > `public override bool IsStartMenuPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsStartMenuPressedDownOnIndex/1

  > `public override bool IsStartMenuPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsStartMenuPressedUpOnIndex/1

  > `public override bool IsStartMenuPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuTouchedOnIndex/1

  > `public override bool IsStartMenuTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsStartMenuTouchedDownOnIndex/1

  > `public override bool IsStartMenuTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsStartMenuTouchedUpOnIndex/1

  > `public override bool IsStartMenuTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.

---

## Fallback Boundaries (SDK_FallbackBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The Base Boundaries SDK script provides a bridge to SDK methods that deal with the play area of SDKs that support room scale play spaces.

This is the fallback class that will just return default values.

### Class Methods

#### InitBoundaries/0

  > `public override void InitBoundaries()`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.

#### GetPlayArea/0

  > `public override Transform GetPlayArea()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the play area in the scene.

The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.

#### GetPlayAreaVertices/1

  > `public override Vector3[] GetPlayAreaVertices(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/1

  > `public override float GetPlayAreaBorderThickness(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/1

  > `public override bool IsPlayAreaSizeCalibrated(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

---

# Simulator SDK (VRTK/SDK/Simulator)

The scripts used to utilise the VR Simulator to build without VR Hardware.

 * [Simulator System](#simulator-system-sdk_simsystem)
 * [Simulator Headset](#simulator-headset-sdk_simheadset)
 * [Simulator Controller](#simulator-controller-sdk_simcontroller)
 * [Simulator Boundaries](#simulator-boundaries-sdk_simboundaries)

---

## Simulator System (SDK_SimSystem)
 > extends [SDK_BaseSystem](#base-system-sdk_basesystem)

### Overview

The Sim System SDK script provides dummy functions for system functions.

### Class Methods

#### IsDisplayOnDesktop/0

  > `public override bool IsDisplayOnDesktop()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the display is extending the desktop

The IsDisplayOnDesktop method returns true if the display is extending the desktop.

#### ShouldAppRenderWithLowResources/0

  > `public override bool ShouldAppRenderWithLowResources()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the Unity app should render with low resources.

The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.

#### ForceInterleavedReprojectionOn/1

  > `public override void ForceInterleavedReprojectionOn(bool force)`

  * Parameters
   * `bool force` - If true then Interleaved Reprojection will be forced on, if false it will not be forced on.
  * Returns
   * _none_

The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.

---

## Simulator Headset (SDK_SimHeadset)
 > extends [SDK_BaseHeadset](#base-headset-sdk_baseheadset)

### Overview

The Sim Headset SDK script  provides dummy functions for the headset.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

  * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetHeadset/0

  > `public override Transform GetHeadset()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the headset in the scene.

The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.

#### GetHeadsetCamera/0

  > `public override Transform GetHeadsetCamera()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object holding the headset camera in the scene.

The GetHeadsetCamera/0 method returns the Transform of the object that is used to hold the headset camera in the scene.

#### GetHeadsetVelocity/0

  > `public override Vector3 GetHeadsetVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public override Vector3 GetHeadsetAngularVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetFade/3

  > `public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)`

  * Parameters
   * `Color color` - The colour to fade to.
   * `float duration` - The amount of time the fade should take to reach the given colour.
   * `bool fadeOverlay` - Determines whether to use an overlay on the fade.
  * Returns
   * _none_

The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.

#### HasHeadsetFade/1

  > `public override bool HasHeadsetFade(Transform obj)`

  * Parameters
   * `Transform obj` - The Transform to check to see if a camera fade is available on.
  * Returns
   * `bool` - Returns true if the headset has fade functionality on it.

The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.

#### AddHeadsetFade/1

  > `public override void AddHeadsetFade(Transform camera)`

  * Parameters
   * `Transform camera` - The Transform to with the camera on to add the fade functionality to.
  * Returns
   * _none_

The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.

---

## Simulator Controller (SDK_SimController)
 > extends [SDK_BaseController](#base-controller-sdk_basecontroller)

### Overview

The Sim Controller SDK script provides functions to help simulate VR controllers.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(uint index, Dictionary<string, object> options)`

  * Parameters
   * `uint index` - The index of the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetControllerDefaultColliderPath/1

  > `public override string GetControllerDefaultColliderPath(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The controller hand to check for
  * Returns
   * `string` - A path to the resource that contains the collider GameObject.

The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.

#### GetControllerElementPath/3

  > `public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)`

  * Parameters
   * `ControllerElements element` - The controller element to look up.
   * `ControllerHand hand` - The controller hand to look up.
   * `bool fullPath` - Whether to get the initial path or the full path to the element.
  * Returns
   * `string` - A string containing the path to the game object that the controller element resides in.

The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.

#### GetControllerIndex/1

  > `public override uint GetControllerIndex(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject containing the controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method returns the index of the given controller.

#### GetControllerByIndex/2

  > `public override GameObject GetControllerByIndex(uint index, bool actual = false)`

  * Parameters
   * `uint index` - The index of the controller to find.
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` -

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller to retrieve the origin from.
  * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

#### GenerateControllerPointerOrigin/1

  > `public override Transform GenerateControllerPointerOrigin(GameObject parent)`

  * Parameters
   * `GameObject parent` - The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.
  * Returns
   * `Transform` - A generated Transform that contains the custom pointer origin.

The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.

#### GetControllerLeftHand/1

  > `public override GameObject GetControllerLeftHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the left hand controller.

The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.

#### GetControllerRightHand/1

  > `public override GameObject GetControllerRightHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the right hand controller.

The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.

#### IsControllerLeftHand/1

  > `public override bool IsControllerLeftHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/1

  > `public override bool IsControllerRightHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.

#### IsControllerLeftHand/2

  > `public override bool IsControllerLeftHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/2

  > `public override bool IsControllerRightHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to get the model alias for.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given GameObject.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The hand enum of which controller model to retrieve.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given controller hand.

#### GetControllerRenderModel/1

  > `public override GameObject GetControllerRenderModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `GameObject` - A GameObject containing the object that has a render model for the controller.

The GetControllerRenderModel method gets the game object that contains the given controller's render model.

#### SetControllerRenderModelWheel/2

  > `public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)`

  * Parameters
   * `GameObject renderModel` - The GameObject containing the controller render model.
   * `bool state` - If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.
  * Returns
   * _none_

The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.

#### HapticPulseOnIndex/2

  > `public override void HapticPulseOnIndex(uint index, float strength = 0.5f)`

  * Parameters
   * `uint index` - The index of the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

  * Parameters
   * _none_
  * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocityOnIndex/1

  > `public override Vector3 GetVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.

#### GetAngularVelocityOnIndex/1

  > `public override Vector3 GetAngularVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.

#### GetTouchpadAxisOnIndex/1

  > `public override Vector2 GetTouchpadAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current x,y position of where the touchpad is being touched.

The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.

#### GetTriggerAxisOnIndex/1

  > `public override Vector2 GetTriggerAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the trigger.

The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.

#### GetGripAxisOnIndex/1

  > `public override Vector2 GetGripAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the grip.

The GetGripAxisOnIndex method is used to get the current grip position on the controller.

#### GetTriggerHairlineDeltaOnIndex/1

  > `public override float GetTriggerHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the trigger presses.

The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.

#### GetGripHairlineDeltaOnIndex/1

  > `public override float GetGripHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the grip presses.

The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.

#### IsTriggerPressedOnIndex/1

  > `public override bool IsTriggerPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTriggerPressedDownOnIndex/1

  > `public override bool IsTriggerPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTriggerPressedUpOnIndex/1

  > `public override bool IsTriggerPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTriggerTouchedOnIndex/1

  > `public override bool IsTriggerTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTriggerTouchedDownOnIndex/1

  > `public override bool IsTriggerTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTriggerTouchedUpOnIndex/1

  > `public override bool IsTriggerTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairTriggerDownOnIndex/1

  > `public override bool IsHairTriggerDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairTriggerUpOnIndex/1

  > `public override bool IsHairTriggerUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsGripPressedOnIndex/1

  > `public override bool IsGripPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsGripPressedDownOnIndex/1

  > `public override bool IsGripPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsGripPressedUpOnIndex/1

  > `public override bool IsGripPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsGripTouchedOnIndex/1

  > `public override bool IsGripTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsGripTouchedDownOnIndex/1

  > `public override bool IsGripTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsGripTouchedUpOnIndex/1

  > `public override bool IsGripTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairGripDownOnIndex/1

  > `public override bool IsHairGripDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairGripUpOnIndex/1

  > `public override bool IsHairGripUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsTouchpadPressedOnIndex/1

  > `public override bool IsTouchpadPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTouchpadPressedDownOnIndex/1

  > `public override bool IsTouchpadPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTouchpadPressedUpOnIndex/1

  > `public override bool IsTouchpadPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTouchpadTouchedOnIndex/1

  > `public override bool IsTouchpadTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTouchpadTouchedDownOnIndex/1

  > `public override bool IsTouchpadTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTouchpadTouchedUpOnIndex/1

  > `public override bool IsTouchpadTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOnePressedOnIndex/1

  > `public override bool IsButtonOnePressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonOnePressedDownOnIndex/1

  > `public override bool IsButtonOnePressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonOnePressedUpOnIndex/1

  > `public override bool IsButtonOnePressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOneTouchedOnIndex/1

  > `public override bool IsButtonOneTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonOneTouchedDownOnIndex/1

  > `public override bool IsButtonOneTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonOneTouchedUpOnIndex/1

  > `public override bool IsButtonOneTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoPressedOnIndex/1

  > `public override bool IsButtonTwoPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonTwoPressedDownOnIndex/1

  > `public override bool IsButtonTwoPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonTwoPressedUpOnIndex/1

  > `public override bool IsButtonTwoPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoTouchedOnIndex/1

  > `public override bool IsButtonTwoTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonTwoTouchedDownOnIndex/1

  > `public override bool IsButtonTwoTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonTwoTouchedUpOnIndex/1

  > `public override bool IsButtonTwoTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuPressedOnIndex/1

  > `public override bool IsStartMenuPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsStartMenuPressedDownOnIndex/1

  > `public override bool IsStartMenuPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsStartMenuPressedUpOnIndex/1

  > `public override bool IsStartMenuPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuTouchedOnIndex/1

  > `public override bool IsStartMenuTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsStartMenuTouchedDownOnIndex/1

  > `public override bool IsStartMenuTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsStartMenuTouchedUpOnIndex/1

  > `public override bool IsStartMenuTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.

---

## Simulator Boundaries (SDK_SimBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The Sim Boundaries SDK script provides dummy functions for the play area bounderies.

### Class Methods

#### InitBoundaries/0

  > `public override void InitBoundaries()`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.

#### GetPlayArea/0

  > `public override Transform GetPlayArea()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the play area in the scene.

The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.

#### GetPlayAreaVertices/1

  > `public override Vector3[] GetPlayAreaVertices(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/1

  > `public override float GetPlayAreaBorderThickness(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/1

  > `public override bool IsPlayAreaSizeCalibrated(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

---

# SteamVR SDK (VRTK/SDK/SteamVR)

The scripts used to utilise the SteamVR Unity Plugin SDK.

 * [SteamVR System](#steamvr-system-sdk_steamvrsystem)
 * [SteamVR Headset](#steamvr-headset-sdk_steamvrheadset)
 * [SteamVR Controller](#steamvr-controller-sdk_steamvrcontroller)
 * [SteamVR Boundaries](#steamvr-boundaries-sdk_steamvrboundaries)

---

## SteamVR System (SDK_SteamVRSystem)
 > extends [SDK_BaseSystem](#base-system-sdk_basesystem)

### Overview

The SteamVR System SDK script provides a bridge to the SteamVR SDK.

### Class Methods

#### IsDisplayOnDesktop/0

  > `public override bool IsDisplayOnDesktop()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the display is extending the desktop

The IsDisplayOnDesktop method returns true if the display is extending the desktop.

#### ShouldAppRenderWithLowResources/0

  > `public override bool ShouldAppRenderWithLowResources()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the Unity app should render with low resources.

The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.

#### ForceInterleavedReprojectionOn/1

  > `public override void ForceInterleavedReprojectionOn(bool force)`

  * Parameters
   * `bool force` - If true then Interleaved Reprojection will be forced on, if false it will not be forced on.
  * Returns
   * _none_

The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.

---

## SteamVR Headset (SDK_SteamVRHeadset)
 > extends [SDK_BaseHeadset](#base-headset-sdk_baseheadset)

### Overview

The SteamVR Headset SDK script provides a bridge to the SteamVR SDK.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

  * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetHeadset/0

  > `public override Transform GetHeadset()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the headset in the scene.

The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.

#### GetHeadsetCamera/0

  > `public override Transform GetHeadsetCamera()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object holding the headset camera in the scene.

The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.

#### GetHeadsetVelocity/0

  > `public override Vector3 GetHeadsetVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public override Vector3 GetHeadsetAngularVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetFade/3

  > `public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)`

  * Parameters
   * `Color color` - The colour to fade to.
   * `float duration` - The amount of time the fade should take to reach the given colour.
   * `bool fadeOverlay` - Determines whether to use an overlay on the fade.
  * Returns
   * _none_

The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.

#### HasHeadsetFade/1

  > `public override bool HasHeadsetFade(Transform obj)`

  * Parameters
   * `Transform obj` - The Transform to check to see if a camera fade is available on.
  * Returns
   * `bool` - Returns true if the headset has fade functionality on it.

The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.

#### AddHeadsetFade/1

  > `public override void AddHeadsetFade(Transform camera)`

  * Parameters
   * `Transform camera` - The Transform to with the camera on to add the fade functionality to.
  * Returns
   * _none_

The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.

---

## SteamVR Controller (SDK_SteamVRController)
 > extends [SDK_BaseController](#base-controller-sdk_basecontroller)

### Overview

The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(uint index, Dictionary<string, object> options)`

  * Parameters
   * `uint index` - The index of the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetControllerDefaultColliderPath/1

  > `public override string GetControllerDefaultColliderPath(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The controller hand to check for
  * Returns
   * `string` - A path to the resource that contains the collider GameObject.

The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.

#### GetControllerElementPath/3

  > `public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)`

  * Parameters
   * `ControllerElements element` - The controller element to look up.
   * `ControllerHand hand` - The controller hand to look up.
   * `bool fullPath` - Whether to get the initial path or the full path to the element.
  * Returns
   * `string` - A string containing the path to the game object that the controller element resides in.

The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.

#### GetControllerIndex/1

  > `public override uint GetControllerIndex(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject containing the controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method returns the index of the given controller.

#### GetControllerByIndex/2

  > `public override GameObject GetControllerByIndex(uint index, bool actual = false)`

  * Parameters
   * `uint index` - The index of the controller to find.
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` -

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller to retrieve the origin from.
  * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

#### GenerateControllerPointerOrigin/1

  > `public override Transform GenerateControllerPointerOrigin(GameObject parent)`

  * Parameters
   * `GameObject parent` - The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.
  * Returns
   * `Transform` - A generated Transform that contains the custom pointer origin.

The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.

#### GetControllerLeftHand/1

  > `public override GameObject GetControllerLeftHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the left hand controller.

The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.

#### GetControllerRightHand/1

  > `public override GameObject GetControllerRightHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the right hand controller.

The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.

#### IsControllerLeftHand/1

  > `public override bool IsControllerLeftHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/1

  > `public override bool IsControllerRightHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.

#### IsControllerLeftHand/2

  > `public override bool IsControllerLeftHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/2

  > `public override bool IsControllerRightHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to get the model alias for.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given GameObject.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The hand enum of which controller model to retrieve.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given controller hand.

#### GetControllerRenderModel/1

  > `public override GameObject GetControllerRenderModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `GameObject` - A GameObject containing the object that has a render model for the controller.

The GetControllerRenderModel method gets the game object that contains the given controller's render model.

#### SetControllerRenderModelWheel/2

  > `public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)`

  * Parameters
   * `GameObject renderModel` - The GameObject containing the controller render model.
   * `bool state` - If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.
  * Returns
   * _none_

The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.

#### HapticPulseOnIndex/2

  > `public override void HapticPulseOnIndex(uint index, float strength = 0.5f)`

  * Parameters
   * `uint index` - The index of the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

  * Parameters
   * _none_
  * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocityOnIndex/1

  > `public override Vector3 GetVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.

#### GetAngularVelocityOnIndex/1

  > `public override Vector3 GetAngularVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.

#### GetTouchpadAxisOnIndex/1

  > `public override Vector2 GetTouchpadAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current x,y position of where the touchpad is being touched.

The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.

#### GetTriggerAxisOnIndex/1

  > `public override Vector2 GetTriggerAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the trigger.

The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.

#### GetGripAxisOnIndex/1

  > `public override Vector2 GetGripAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the grip.

The GetGripAxisOnIndex method is used to get the current grip position on the controller.

#### GetTriggerHairlineDeltaOnIndex/1

  > `public override float GetTriggerHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the trigger presses.

The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.

#### GetGripHairlineDeltaOnIndex/1

  > `public override float GetGripHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the grip presses.

The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.

#### IsTriggerPressedOnIndex/1

  > `public override bool IsTriggerPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTriggerPressedDownOnIndex/1

  > `public override bool IsTriggerPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTriggerPressedUpOnIndex/1

  > `public override bool IsTriggerPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTriggerTouchedOnIndex/1

  > `public override bool IsTriggerTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTriggerTouchedDownOnIndex/1

  > `public override bool IsTriggerTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTriggerTouchedUpOnIndex/1

  > `public override bool IsTriggerTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairTriggerDownOnIndex/1

  > `public override bool IsHairTriggerDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairTriggerUpOnIndex/1

  > `public override bool IsHairTriggerUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsGripPressedOnIndex/1

  > `public override bool IsGripPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsGripPressedDownOnIndex/1

  > `public override bool IsGripPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsGripPressedUpOnIndex/1

  > `public override bool IsGripPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsGripTouchedOnIndex/1

  > `public override bool IsGripTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsGripTouchedDownOnIndex/1

  > `public override bool IsGripTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsGripTouchedUpOnIndex/1

  > `public override bool IsGripTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairGripDownOnIndex/1

  > `public override bool IsHairGripDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairGripUpOnIndex/1

  > `public override bool IsHairGripUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsTouchpadPressedOnIndex/1

  > `public override bool IsTouchpadPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTouchpadPressedDownOnIndex/1

  > `public override bool IsTouchpadPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTouchpadPressedUpOnIndex/1

  > `public override bool IsTouchpadPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTouchpadTouchedOnIndex/1

  > `public override bool IsTouchpadTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTouchpadTouchedDownOnIndex/1

  > `public override bool IsTouchpadTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTouchpadTouchedUpOnIndex/1

  > `public override bool IsTouchpadTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOnePressedOnIndex/1

  > `public override bool IsButtonOnePressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonOnePressedDownOnIndex/1

  > `public override bool IsButtonOnePressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonOnePressedUpOnIndex/1

  > `public override bool IsButtonOnePressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOneTouchedOnIndex/1

  > `public override bool IsButtonOneTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonOneTouchedDownOnIndex/1

  > `public override bool IsButtonOneTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonOneTouchedUpOnIndex/1

  > `public override bool IsButtonOneTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoPressedOnIndex/1

  > `public override bool IsButtonTwoPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonTwoPressedDownOnIndex/1

  > `public override bool IsButtonTwoPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonTwoPressedUpOnIndex/1

  > `public override bool IsButtonTwoPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoTouchedOnIndex/1

  > `public override bool IsButtonTwoTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonTwoTouchedDownOnIndex/1

  > `public override bool IsButtonTwoTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonTwoTouchedUpOnIndex/1

  > `public override bool IsButtonTwoTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuPressedOnIndex/1

  > `public override bool IsStartMenuPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsStartMenuPressedDownOnIndex/1

  > `public override bool IsStartMenuPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsStartMenuPressedUpOnIndex/1

  > `public override bool IsStartMenuPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuTouchedOnIndex/1

  > `public override bool IsStartMenuTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsStartMenuTouchedDownOnIndex/1

  > `public override bool IsStartMenuTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsStartMenuTouchedUpOnIndex/1

  > `public override bool IsStartMenuTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.

---

## SteamVR Boundaries (SDK_SteamVRBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The SteamVR Boundaries SDK script provides a bridge to the SteamVR SDK play area.

### Class Methods

#### InitBoundaries/0

  > `public override void InitBoundaries()`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.

#### GetPlayArea/0

  > `public override Transform GetPlayArea()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the play area in the scene.

The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.

#### GetPlayAreaVertices/1

  > `public override Vector3[] GetPlayAreaVertices(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/1

  > `public override float GetPlayAreaBorderThickness(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/1

  > `public override bool IsPlayAreaSizeCalibrated(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

---

# OculusVR SDK (VRTK/SDK/OculusVR)

The scripts used to utilise the Oculus Utilities Unity Package SDK.

 * [OculusVR System](#oculusvr-system-sdk_oculusvrsystem)
 * [OculusVR Headset](#oculusvr-headset-sdk_oculusvrheadset)
 * [OculusVR Controller](#oculusvr-controller-sdk_oculusvrcontroller)
 * [OculusVR Boundaries](#oculusvr-boundaries-sdk_oculusvrboundaries)

---

## OculusVR System (SDK_OculusVRSystem)
 > extends [SDK_BaseSystem](#base-system-sdk_basesystem)

### Overview

The OculusVR System SDK script provides a bridge to the OculusVR SDK.

### Class Methods

#### IsDisplayOnDesktop/0

  > `public override bool IsDisplayOnDesktop()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the display is extending the desktop

The IsDisplayOnDesktop method returns true if the display is extending the desktop.

#### ShouldAppRenderWithLowResources/0

  > `public override bool ShouldAppRenderWithLowResources()`

  * Parameters
   * _none_
  * Returns
   * `bool` - Returns true if the Unity app should render with low resources.

The ShouldAppRenderWithLowResources method is used to determine if the Unity app should use low resource mode. Typically true when the dashboard is showing.

#### ForceInterleavedReprojectionOn/1

  > `public override void ForceInterleavedReprojectionOn(bool force)`

  * Parameters
   * `bool force` - If true then Interleaved Reprojection will be forced on, if false it will not be forced on.
  * Returns
   * _none_

The ForceInterleavedReprojectionOn method determines whether Interleaved Reprojection should be forced on or off.

---

## OculusVR Headset (SDK_OculusVRHeadset)
 > extends [SDK_BaseHeadset](#base-headset-sdk_baseheadset)

### Overview

The OculusVR Headset SDK script provides a bridge to the OculusVR SDK.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

  * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetHeadset/0

  > `public override Transform GetHeadset()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the headset in the scene.

The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.

#### GetHeadsetCamera/0

  > `public override Transform GetHeadsetCamera()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object holding the headset camera in the scene.

The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.

#### GetHeadsetVelocity/0

  > `public override Vector3 GetHeadsetVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the headset.

The GetHeadsetVelocity method is used to determine the current velocity of the headset.

#### GetHeadsetAngularVelocity/0

  > `public override Vector3 GetHeadsetAngularVelocity()`

  * Parameters
   * _none_
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the headset.

The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.

#### HeadsetFade/3

  > `public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)`

  * Parameters
   * `Color color` - The colour to fade to.
   * `float duration` - The amount of time the fade should take to reach the given colour.
   * `bool fadeOverlay` - Determines whether to use an overlay on the fade.
  * Returns
   * _none_

The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.

#### HasHeadsetFade/1

  > `public override bool HasHeadsetFade(Transform obj)`

  * Parameters
   * `Transform obj` - The Transform to check to see if a camera fade is available on.
  * Returns
   * `bool` - Returns true if the headset has fade functionality on it.

The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.

#### AddHeadsetFade/1

  > `public override void AddHeadsetFade(Transform camera)`

  * Parameters
   * `Transform camera` - The Transform to with the camera on to add the fade functionality to.
  * Returns
   * _none_

The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.

---

## OculusVR Controller (SDK_OculusVRController)
 > extends [SDK_BaseController](#base-controller-sdk_basecontroller)

### Overview

The OculusVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(uint index, Dictionary<string, object> options)`

  * Parameters
   * `uint index` - The index of the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
  * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### GetControllerDefaultColliderPath/1

  > `public override string GetControllerDefaultColliderPath(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The controller hand to check for
  * Returns
   * `string` - A path to the resource that contains the collider GameObject.

The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.

#### GetControllerElementPath/3

  > `public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)`

  * Parameters
   * `ControllerElements element` - The controller element to look up.
   * `ControllerHand hand` - The controller hand to look up.
   * `bool fullPath` - Whether to get the initial path or the full path to the element.
  * Returns
   * `string` - A string containing the path to the game object that the controller element resides in.

The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.

#### GetControllerIndex/1

  > `public override uint GetControllerIndex(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject containing the controller.
  * Returns
   * `uint` - The index of the given controller.

The GetControllerIndex method returns the index of the given controller.

#### GetControllerByIndex/2

  > `public override GameObject GetControllerByIndex(uint index, bool actual = false)`

  * Parameters
   * `uint index` - The index of the controller to find.
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` -

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(GameObject controller)`

  * Parameters
   * `GameObject controller` - The controller to retrieve the origin from.
  * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

#### GenerateControllerPointerOrigin/1

  > `public override Transform GenerateControllerPointerOrigin(GameObject parent)`

  * Parameters
   * `GameObject parent` - The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.
  * Returns
   * `Transform` - A generated Transform that contains the custom pointer origin.

The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.

#### GetControllerLeftHand/1

  > `public override GameObject GetControllerLeftHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the left hand controller.

The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.

#### GetControllerRightHand/1

  > `public override GameObject GetControllerRightHand(bool actual = false)`

  * Parameters
   * `bool actual` - If true it will return the actual controller, if false it will return the script alias controller GameObject.
  * Returns
   * `GameObject` - The GameObject containing the right hand controller.

The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.

#### IsControllerLeftHand/1

  > `public override bool IsControllerLeftHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/1

  > `public override bool IsControllerRightHand(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.

#### IsControllerLeftHand/2

  > `public override bool IsControllerLeftHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the left hand controller.

The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.

#### IsControllerRightHand/2

  > `public override bool IsControllerRightHand(GameObject controller, bool actual)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
   * `bool actual` - If true it will check the actual controller, if false it will check the script alias controller.
  * Returns
   * `bool` - Returns true if the given controller is the right hand controller.

The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to get the model alias for.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given GameObject.

#### GetControllerModel/1

  > `public override GameObject GetControllerModel(ControllerHand hand)`

  * Parameters
   * `ControllerHand hand` - The hand enum of which controller model to retrieve.
  * Returns
   * `GameObject` - The GameObject that has the model alias within it.

The GetControllerModel method returns the model alias for the given controller hand.

#### GetControllerRenderModel/1

  > `public override GameObject GetControllerRenderModel(GameObject controller)`

  * Parameters
   * `GameObject controller` - The GameObject to check.
  * Returns
   * `GameObject` - A GameObject containing the object that has a render model for the controller.

The GetControllerRenderModel method gets the game object that contains the given controller's render model.

#### SetControllerRenderModelWheel/2

  > `public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)`

  * Parameters
   * `GameObject renderModel` - The GameObject containing the controller render model.
   * `bool state` - If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.
  * Returns
   * _none_

The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.

#### HapticPulseOnIndex/2

  > `public override void HapticPulseOnIndex(uint index, float strength = 0.5f)`

  * Parameters
   * `uint index` - The index of the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
  * Returns
   * _none_

The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

  * Parameters
   * _none_
  * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocityOnIndex/1

  > `public override Vector3 GetVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.

#### GetAngularVelocityOnIndex/1

  > `public override Vector3 GetAngularVelocityOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.

#### GetTouchpadAxisOnIndex/1

  > `public override Vector2 GetTouchpadAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current x,y position of where the touchpad is being touched.

The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.

#### GetTriggerAxisOnIndex/1

  > `public override Vector2 GetTriggerAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the trigger.

The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.

#### GetGripAxisOnIndex/1

  > `public override Vector2 GetGripAxisOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `Vector2` - A Vector2 containing the current position of the grip.

The GetGripAxisOnIndex method is used to get the current grip position on the controller.

#### GetTriggerHairlineDeltaOnIndex/1

  > `public override float GetTriggerHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the trigger presses.

The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.

#### GetGripHairlineDeltaOnIndex/1

  > `public override float GetGripHairlineDeltaOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `float` - The delta between the grip presses.

The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.

#### IsTriggerPressedOnIndex/1

  > `public override bool IsTriggerPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTriggerPressedDownOnIndex/1

  > `public override bool IsTriggerPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTriggerPressedUpOnIndex/1

  > `public override bool IsTriggerPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTriggerTouchedOnIndex/1

  > `public override bool IsTriggerTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTriggerTouchedDownOnIndex/1

  > `public override bool IsTriggerTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTriggerTouchedUpOnIndex/1

  > `public override bool IsTriggerTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairTriggerDownOnIndex/1

  > `public override bool IsHairTriggerDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairTriggerUpOnIndex/1

  > `public override bool IsHairTriggerUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsGripPressedOnIndex/1

  > `public override bool IsGripPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsGripPressedDownOnIndex/1

  > `public override bool IsGripPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsGripPressedUpOnIndex/1

  > `public override bool IsGripPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsGripTouchedOnIndex/1

  > `public override bool IsGripTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsGripTouchedDownOnIndex/1

  > `public override bool IsGripTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsGripTouchedUpOnIndex/1

  > `public override bool IsGripTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsHairGripDownOnIndex/1

  > `public override bool IsHairGripDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has passed it's press threshold.

The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.

#### IsHairGripUpOnIndex/1

  > `public override bool IsHairGripUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released from it's press threshold.

The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.

#### IsTouchpadPressedOnIndex/1

  > `public override bool IsTouchpadPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsTouchpadPressedDownOnIndex/1

  > `public override bool IsTouchpadPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsTouchpadPressedUpOnIndex/1

  > `public override bool IsTouchpadPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsTouchpadTouchedOnIndex/1

  > `public override bool IsTouchpadTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsTouchpadTouchedDownOnIndex/1

  > `public override bool IsTouchpadTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsTouchpadTouchedUpOnIndex/1

  > `public override bool IsTouchpadTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOnePressedOnIndex/1

  > `public override bool IsButtonOnePressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonOnePressedDownOnIndex/1

  > `public override bool IsButtonOnePressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonOnePressedUpOnIndex/1

  > `public override bool IsButtonOnePressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonOneTouchedOnIndex/1

  > `public override bool IsButtonOneTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonOneTouchedDownOnIndex/1

  > `public override bool IsButtonOneTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonOneTouchedUpOnIndex/1

  > `public override bool IsButtonOneTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoPressedOnIndex/1

  > `public override bool IsButtonTwoPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsButtonTwoPressedDownOnIndex/1

  > `public override bool IsButtonTwoPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsButtonTwoPressedUpOnIndex/1

  > `public override bool IsButtonTwoPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsButtonTwoTouchedOnIndex/1

  > `public override bool IsButtonTwoTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsButtonTwoTouchedDownOnIndex/1

  > `public override bool IsButtonTwoTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsButtonTwoTouchedUpOnIndex/1

  > `public override bool IsButtonTwoTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuPressedOnIndex/1

  > `public override bool IsStartMenuPressedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being pressed.

The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.

#### IsStartMenuPressedDownOnIndex/1

  > `public override bool IsStartMenuPressedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been pressed down.

The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.

#### IsStartMenuPressedUpOnIndex/1

  > `public override bool IsStartMenuPressedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.

#### IsStartMenuTouchedOnIndex/1

  > `public override bool IsStartMenuTouchedOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button is continually being touched.

The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.

#### IsStartMenuTouchedDownOnIndex/1

  > `public override bool IsStartMenuTouchedDownOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been touched down.

The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.

#### IsStartMenuTouchedUpOnIndex/1

  > `public override bool IsStartMenuTouchedUpOnIndex(uint index)`

  * Parameters
   * `uint index` - The index of the tracked object to check for.
  * Returns
   * `bool` - Returns true if the button has just been released.

The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.

---

## OculusVR Boundaries (SDK_OculusVRBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The OculusVR Boundaries SDK script provides a bridge to the OculusVR SDK play area.

### Class Methods

#### InitBoundaries/0

  > `public override void InitBoundaries()`

  * Parameters
   * _none_
  * Returns
   * _none_

The InitBoundaries method is run on start of scene and can be used to initialse anything on game start.

#### GetPlayArea/0

  > `public override Transform GetPlayArea()`

  * Parameters
   * _none_
  * Returns
   * `Transform` - A transform of the object representing the play area in the scene.

The GetPlayArea method returns the Transform of the object that is used to represent the play area in the scene.

#### GetPlayAreaVertices/1

  > `public override Vector3[] GetPlayAreaVertices(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/1

  > `public override float GetPlayAreaBorderThickness(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/1

  > `public override bool IsPlayAreaSizeCalibrated(GameObject playArea)`

  * Parameters
   * `GameObject playArea` - The GameObject containing the play area representation.
  * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetAvatar/0

  > `public virtual OvrAvatar GetAvatar()`

  * Parameters
   * _none_
  * Returns
   * `OvrAvatar` - The OvrAvatar script for managing the Oculus Avatar.

The GetAvatar method is used to retrieve the Oculus Avatar object if it exists in the scene. This method is only available if the Oculus Avatar package is installed.

---

