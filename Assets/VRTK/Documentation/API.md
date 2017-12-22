# API Documentation

This file describes all of the public methods, variables and events utilised by the VRTK prefabs and scripts.

 * [Prefabs](#prefabs-vrtkprefabs)
 * [Pointers](#pointers-vrtksourcescriptspointers)
   * [Pointer Renderers](#pointer-renderers-vrtksourcescriptspointerspointerrenderers)
 * [Locomotion](#locomotion-vrtksourcescriptslocomotion)
   * [Object Control Actions](#object-control-actions-vrtksourcescriptslocomotionobjectcontrolactions)
 * [Highlighters](#highlighters-vrtksourcescriptsinteractionshighlighters)
 * [Interactors](#interactors-vrtksourcescriptsinteractionsinteractors)
 * [Interactables](#interactables-vrtksourcescriptsinteractionsinteractables)
   * [Grab Attach Mechanics](#grab-attach-mechanics-vrtksourcescriptsinteractionsgrabattachmechanics)
   * [Secondary Controller Grab Actions](#secondary-controller-grab-actions-vrtksourcescriptsinteractionssecondarycontrollergrabactions)
   * [Controllables](#controllables-vrtksourcescriptsinteractionscontrollables)
     * [Physics Controllables](#physics-controllables-vrtksourcescriptsinteractionscontrollablesphysics)
     * [Artificial Controllables](#artificial-controllables-vrtksourcescriptsinteractionscontrollablesartificial)
 * [Presence](#presence-vrtksourcescriptspresence)
 * [UI](#ui-vrtksourcescriptsui)
 * [Utilities](#utilities-vrtksourcescriptsutilities)
 * [Base SDK](#base-sdk-vrtksourcesdkbase)
   * [Fallback SDK](#fallback-sdk-vrtksourcesdkfallback)
   * [Unity SDK](#unity-sdk-vrtksourcesdkunity)
   * [Simulator SDK](#simulator-sdk-vrtksourcesdksimulator)
   * [SteamVR SDK](#steamvr-sdk-vrtksourcesdksteamvr)
   * [Oculus SDK](#oculus-sdk-vrtksourcesdkoculus)
   * [Daydream SDK](#daydream-sdk-vrtksourcesdkdaydream)
   * [Ximmerse SDK](#ximmerse-sdk-vrtksourcesdkximmerse)
   * [HyperealVR SDK](#hyperealvr-sdk-vrtksourcesdkhyperealvr)

---

# Prefabs (VRTK/Prefabs)

A collection of pre-defined usable prefabs have been included to allow for each drag-and-drop set up of common elements.

 * [SDK Setup Switcher](#sdk-setup-switcher-vrtk_sdksetupswitcher)
 * [Console Viewer Canvas](#console-viewer-canvas-vrtk_consoleviewer)
 * [Frames Per Second Canvas](#frames-per-second-canvas-vrtk_framespersecondviewer)
 * [Desktop Camera](#desktop-camera-vrtk_desktopcamera)
 * [Controller Rigidbody Activator](#controller-rigidbody-activator-vrtk_controllerrigidbodyactivator)
 * [Object Tooltip](#object-tooltip-vrtk_objecttooltip)
 * [Controller Tooltips](#controller-tooltips-vrtk_controllertooltips)
 * [Snap Drop Zone](#snap-drop-zone-vrtk_snapdropzone)
 * [Destination Point](#destination-point-vrtk_destinationpoint)
 * [Pointer Direction Indicator](#pointer-direction-indicator-vrtk_pointerdirectionindicator)
 * [Radial Menu](#radial-menu-vrtk_radialmenu)
 * [Independent Radial Menu](#independent-radial-menu-vrtk_independentradialmenucontroller)
 * [Panel Menu](#panel-menu-vrtk_panelmenucontroller)
 * [Panel Menu Item](#panel-menu-item-vrtk_panelmenuitemcontroller)
 * [Avatar Hands](#avatar-hands-vrtk_avatarhandcontroller)

---

## SDK Setup Switcher (VRTK_SDKSetupSwitcher)

### Overview

Provides a GUI overlay to allow switching the loaded VRTK_SDKSetup of the the current VRTK_SDKManager.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/SDKSetupSwitcher/SDKSetupSwitcher` prefab into the scene hierarchy.

---

## Console Viewer Canvas (VRTK_ConsoleViewer)

### Overview

Adds an in-scene representation of the Unity console on a world space canvas.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/ConsoleViewerCanvas/ConsoleViewerCanvas` prefab into the scene hierarchy.

  > It is also possible to interact with the `ConsoleViewerCanvas` with a `VRTK_UIPointer`.

### Inspector Parameters

 * **Font Size:** The size of the font the log text is displayed in.
 * **Info Message:** The colour of the text for an info log message.
 * **Assert Message:** The colour of the text for an assertion log message.
 * **Warning Message:** The colour of the text for a warning log message.
 * **Error Message:** The colour of the text for an error log message.
 * **Exception Message:** The colour of the text for an exception log message.

### Class Methods

#### SetCollapse/1

  > `public virtual void SetCollapse(bool state)`

 * Parameters
   * `bool state` - The state of whether to collapse the output messages, true will collapse and false will not collapse.
 * Returns
   * _none_

The SetCollapse method determines whether the console will collapse same message output into the same line. A state of `true` will collapse messages and `false` will print the same message for each line.

#### ClearLog/0

  > `public virtual void ClearLog()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ClearLog method clears the current log view of all messages

---

## Frames Per Second Canvas (VRTK_FramesPerSecondViewer)

### Overview

Provides a frames per second text element to the HMD view. To use the prefab it must be placed into the scene then the headset camera needs attaching to the canvas:

**Prefab Usage:**
 * Place the `VRTK/Prefabs/FramesPerSecondCanvas/FramesPerSecondCanvas` prefab in the scene hierarchy.

  > This script is largely based on the script at: http://talesfromtherift.com/vr-fps-counter/ So all credit to Peter Koch for his work. Twitter: @peterept

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

## Desktop Camera (VRTK_DesktopCamera)

### Overview

Allows rendering a separate camera that is shown on the desktop only, without changing what's seen in VR headsets.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/DesktopCamera/DesktopCamera` prefab in the scene.

### Inspector Parameters

 * **Desktop Camera:** The camera to use for the desktop view. If left blank the camera on the game object this script is attached to or any of its children will be used.
 * **Follow Script:** The follow script to use for following the headset. If left blank the follow script on the game object this script is attached to or any of its children will be used.
 * **Headset Image:** The optional image to render the headset's view into. Can be left blank.
 * **Headset Render Texture:** The optional render texture to render the headset's view into. Can be left blank. If this is blank and `headsetImage` is set a default render texture will be created.

---

## Controller Rigidbody Activator (VRTK_ControllerRigidbodyActivator)

### Overview

Provides a simple trigger collider volume that when a controller enters will enable the rigidbody on the controller.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/ControllerRigidbodyActivator/ControllerRigidbodyActivator` prefab in the scene at the location where the controller rigidbody should be automatically activated.
 * The prefab contains a default sphere collider to determine ths collision, this collider component can be customised in the inspector or can be replaced with another collider component (set to `Is Trigger`).

  > If the prefab is placed as a child of the target Interactable Object then the collider volume on the prefab will trigger collisions on the Interactable Object.

### Inspector Parameters

 * **Is Enabled:** If this is checked then the Collider will have it's Rigidbody toggled on and off during a collision.
 * **Activate Interact Touch:** If this is checked then the Rigidbody Activator will activate the rigidbody and colliders on the Interact Touch script.
 * **Activate Tracked Collider:** If this is checked then the Rigidbody Activator will activate the rigidbody and colliders on the Controller Tracked Collider script.

### Class Events

 * `ControllerRigidbodyOn` - Emitted when the controller rigidbody is turned on.
 * `ControllerRigidbodyOff` - Emitted when the controller rigidbody is turned off.

### Unity Events

Adding the `VRTK_ControllerRigidbodyActivator_UnityEvents` component to `VRTK_ControllerRigidbodyActivator` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * ` interactingObject` - The object that touching the activator.

---

## Object Tooltip (VRTK_ObjectTooltip)

### Overview

Adds a World Space Canvas that can be used to provide additional information about an object by providing a piece of text with a line drawn to a destination point.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/ObjectTooltip/ObjectTooltip` prefab into the scene hierarchy, preferably as a child of the GameObject it is associated with.
 * Set the `Draw Line To` option to the Transform component of the GameObject the Tooltip will be assoicated with.

### Inspector Parameters

 * **Display Text:** The text that is displayed on the tooltip.
 * **Font Size:** The size of the text that is displayed.
 * **Container Size:** The size of the tooltip container where `x = width` and `y = height`.
 * **Draw Line From:** An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.
 * **Draw Line To:** A transform of another object in the scene that a line will be drawn from the tooltip to, this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, then the parent object's transform will be used as this destination position.
 * **Line Width:** The width of the line drawn between the tooltip and the destination transform.
 * **Font Color:** The colour to use for the text on the tooltip.
 * **Container Color:** The colour to use for the background container of the tooltip.
 * **Line Color:** The colour to use for the line drawn between the tooltip and the destination transform.
 * **Always Face Headset:** If this is checked then the tooltip will be rotated so it always face the headset.

### Class Events

 * `ObjectTooltipReset` - Emitted when the object tooltip is reset.
 * `ObjectTooltipTextUpdated` - Emitted when the object tooltip text is updated.

### Unity Events

Adding the `VRTK_ObjectTooltip_UnityEvents` component to `VRTK_ObjectTooltip` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `string newText` - The optional new text that is given to the tooltip.

### Class Methods

#### ResetTooltip/0

  > `public virtual void ResetTooltip()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetTooltip method resets the tooltip back to its initial state.

#### UpdateText/1

  > `public virtual void UpdateText(string newText)`

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

Adds a collection of Object Tooltips to the Controller providing information to what the controller buttons may do.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/ControllerTooltips/ControllerTooltips` prefab as a child of the relevant controller script alias GameObject in the scene hierarchy.
 * If no `Button Transform Settings` are provided in the inspector at Edit time then the button transforms will attempt to be set to the transforms of the current SDK default controller model.
 * If one of the `Button Text Settings` text options are not provided, then the tooltip for that specific button will be hidden.

  > There are a number of parameters that can be set on the Prefab which are provided by the `VRTK_ControllerTooltips` script which is applied to the prefab.

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
 * **Controller Events:** The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.
 * **Headset Controller Aware:** The headset controller aware script to use to see if the headset is looking at the controller. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.
 * **Hide When Not In View:** If this is checked then the tooltips will be hidden when the headset is not looking at the controller.

### Class Events

 * `ControllerTooltipOn` - Emitted when the controller tooltip is turned on.
 * `ControllerTooltipOff` - Emitted when the controller tooltip is turned off.

### Unity Events

Adding the `VRTK_ControllerTooltips_UnityEvents` component to `VRTK_ControllerTooltips` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerTooltips.TooltipButtons element` - The tooltip element being affected.

### Class Methods

#### ResetTooltip/0

  > `public virtual void ResetTooltip()`

 * Parameters
   * _none_
 * Returns
   * _none_

The Reset method reinitalises the tooltips on all of the controller elements.

#### UpdateText/2

  > `public virtual void UpdateText(TooltipButtons element, string newText)`

 * Parameters
   * `TooltipButtons element` - The specific controller element to change the tooltip text on.
   * `string newText` - A string containing the text to update the tooltip to display.
 * Returns
   * _none_

The UpdateText method allows the tooltip text on a specific controller element to be updated at runtime.

#### ToggleTips/2

  > `public virtual void ToggleTips(bool state, TooltipButtons element = TooltipButtons.None)`

 * Parameters
   * `bool state` - The state of whether to display or hide the controller tooltips, true will display and false will hide.
   * `TooltipButtons element` - The specific element to hide the tooltip on, if it is `TooltipButtons.None` then it will hide all tooltips. Optional parameter defaults to `TooltipButtons.None`
 * Returns
   * _none_

The ToggleTips method will display the controller tooltips if the state is `true` and will hide the controller tooltips if the state is `false`. An optional `element` can be passed to target a specific controller tooltip to toggle otherwise all tooltips are toggled.

### Example

`VRTK/Examples/029_Controller_Tooltips` displays two cubes that have an object tooltip added to them along with tooltips that have been added to the controllers.

---

## Snap Drop Zone (VRTK_SnapDropZone)

### Overview

Provides a predefined zone where a valid interactable object can be dropped and upon dropping it snaps to the set snap drop zone transform position, rotation and scale.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/SnapDropZone/SnapDropZone` prefab into the scene hierarchy.
 * Provide the SnapDropZone with an optional `Highlight Object Prefab` to generate an object outline in the scene that determines the final position, rotation and scale of the snapped object.
 * If no `VRTK_BaseHighlighter` derivative is applied to the SnapDropZone then the default MaterialColorSwap Highlighter will be used.
 * The collision zone that activates the SnapDropZone is a `Sphere Collider` by default but can be amended or replaced on the SnapDropZone GameObject.
 * If the `Use Joint` Snap Type is selected then a custom Joint component is required to be added to the `SnapDropZone` Game Object and upon release the interactable object's rigidbody will be linked to this joint as the `Connected Body`.

### Inspector Parameters

 * **Highlight Object Prefab:** A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.
 * **Snap Type:** The Snap Type to apply when a valid interactable object is dropped within the snap zone.
 * **Snap Duration:** The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.
 * **Apply Scaling On Snap:** If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.
 * **Clone New On Unsnap:** If this is checked then when the snapped object is unsnapped from the drop zone, a clone of the unsnapped object will be snapped back into the drop zone.
 * **Highlight Color:** The colour to use when showing the snap zone is active. This is used as the highlight colour when no object is hovering but `Highlight Always Active` is true.
 * **Valid Highlight Color:** The colour to use when showing the snap zone is active and a valid object is hovering. If this is `Color.clear` then the `Highlight Color` will be used.
 * **Highlight Always Active:** The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.
 * **Valid Object List Policy:** A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.
 * **Display Drop Zone In Editor:** If this is checked then the drop zone highlight section will be displayed in the scene editor window.
 * **Default Snapped Interactable Object:** The Interactable Object to snap into the dropzone when the drop zone is enabled. The Interactable Object must be valid in any given policy list to snap.

### Class Variables

 * `public enum SnapTypes` - The types of snap on release available.
   * `UseKinematic` - Will set the interactable object rigidbody to `isKinematic = true`.
   * `UseJoint` - Will attach the interactable object's rigidbody to the provided joint as it's `Connected Body`.
   * `UseParenting` - Will set the SnapDropZone as the interactable object's parent and set it's rigidbody to `isKinematic = true`.

### Class Events

 * `ObjectEnteredSnapDropZone` - Emitted when a valid interactable object enters the snap drop zone trigger collider.
 * `ObjectExitedSnapDropZone` - Emitted when a valid interactable object exists the snap drop zone trigger collider.
 * `ObjectSnappedToDropZone` - Emitted when an interactable object is successfully snapped into a drop zone.
 * `ObjectUnsnappedFromDropZone` - Emitted when an interactable object is removed from a snapped drop zone.

### Unity Events

Adding the `VRTK_SnapDropZone_UnityEvents` component to `VRTK_SnapDropZone` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

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

the ForceSnap method attempts to automatically attach a valid GameObject to the snap drop zone.

#### ForceUnsnap/0

  > `public virtual void ForceUnsnap()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.

#### ValidSnappableObjectIsHovering/0

  > `public virtual bool ValidSnappableObjectIsHovering()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if a valid object is currently in the snap drop zone area.

The ValidSnappableObjectIsHovering method determines if any valid objects are currently hovering in the snap drop zone area.

#### IsObjectHovering/1

  > `public virtual bool IsObjectHovering(GameObject checkObject)`

 * Parameters
   * `GameObject checkObject` - The GameObject to check to see if it's hovering in the snap drop zone area.
 * Returns
   * `bool` - Returns true if the given GameObject is hovering (but not snapped) in the snap drop zone area.

The IsObjectHovering method determines if the given GameObject is currently howvering (but not snapped) in the snap drop zone area.

#### IsInteractableObjectHovering/1

  > `public virtual bool IsInteractableObjectHovering(VRTK_InteractableObject checkObject)`

 * Parameters
   * `VRTK_InteractableObject checkObject` - The Interactable Object script to check to see if it's hovering in the snap drop zone area.
 * Returns
   * `bool` - Returns true if the given Interactable Object script is hovering (but not snapped) in the snap drop zone area.

The IsInteractableObjectHovering method determines if the given Interactable Object script is currently howvering (but not snapped) in the snap drop zone area.

#### GetHoveringObjects/0

  > `public virtual List<GameObject> GetHoveringObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<GameObject>` - The List of valid GameObjects that are hovering (but not snapped) in the snap drop zone area.

The GetHoveringObjects method returns a List of valid GameObjects that are currently hovering (but not snapped) in the snap drop zone area.

#### GetHoveringInteractableObjects/0

  > `public virtual List<VRTK_InteractableObject> GetHoveringInteractableObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<VRTK_InteractableObject>` - The List of valid Interactable Object scripts that are hovering (but not snapped) in the snap drop zone area.

The GetHoveringInteractableObjects method returns a List of valid Interactable Object scripts that are currently hovering (but not snapped) in the snap drop zone area.

#### GetCurrentSnappedObject/0

  > `public virtual GameObject GetCurrentSnappedObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject that is currently snapped in the snap drop zone area.

The GetCurrentSnappedObejct method returns the GameObject that is currently snapped in the snap drop zone area.

#### GetCurrentSnappedInteractableObject/0

  > `public virtual VRTK_InteractableObject GetCurrentSnappedInteractableObject()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractableObject` - The Interactable Object script that is currently snapped in the snap drop zone area.

The GetCurrentSnappedInteractableObject method returns the Interactable Object script that is currently snapped in the snap drop zone area.

### Example

`VRTK/Examples/041_Controller_ObjectSnappingToDropZones` uses the `VRTK_SnapDropZone` prefab to set up pre-determined snap zones for a range of objects and demonstrates how only objects of certain types can be snapped into certain areas.

---

## Destination Point (VRTK_DestinationPoint)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

Allows for a specific scene marker or specific area within the scene that can be teleported to.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/DestinationPoint/DestinationPoint` prefab at the desired location within the scene.
 * Uncheck the `Enable Teleport` checkbox to lock the destination point and prevent teleporting to it.
 * Uncheck the `Snap To Point` checkbox to provide a destination area rather than a specific point to teleport to.

### Inspector Parameters

 * **Default Cursor Object:** The GameObject to use to represent the default cursor state.
 * **Hover Cursor Object:** The GameObject to use to represent the hover cursor state.
 * **Locked Cursor Object:** The GameObject to use to represent the locked cursor state.
 * **Destination Location:** An optional transform to determine the destination location for the destination marker. This can be useful to offset the destination location from the destination point. If this is left empty then the destiantion point transform will be used.
 * **Snap To Point:** If this is checked then after teleporting, the play area will be snapped to the origin of the destination point. If this is false then it's possible to teleport to anywhere within the destination point collider.
 * **Hide Pointer Cursor On Hover:** If this is checked, then the pointer cursor will be hidden when a valid destination point is hovered over.
 * **Hide Direction Indicator On Hover:** If this is checked, then the pointer direction indicator will be hidden when a valid destination point is hovered over. A pointer direction indicator will always be hidden if snap to rotation is set.
 * **Snap To Rotation:** Determines if the play area will be rotated to the rotation of the destination point upon the destination marker being set.
 * **Teleporter:** The scene teleporter that is used. If this is not specified then it will be auto looked up in the scene.

### Class Variables

 * `public enum RotationTypes` - Allowed snap to rotation types.
   * `NoRotation` - No rotation information will be emitted in the destination set payload.
   * `RotateWithNoHeadsetOffset` - The destination point's rotation will be emitted without taking into consideration the current headset rotation.
   * `RotateWithHeadsetOffset` - The destination point's rotation will be emitted and will take into consideration the current headset rotation.

### Class Events

 * `DestinationPointEnabled` - Emitted when the destination point is enabled.
 * `DestinationPointDisabled` - Emitted when the destination point is disabled.
 * `DestinationPointLocked` - Emitted when the destination point is locked.
 * `DestinationPointUnlocked` - Emitted when the destination point is unlocked.
 * `DestinationPointReset` - Emitted when the destination point is reset.

### Unity Events

Adding the `VRTK_DestinationPoint_UnityEvents` component to `VRTK_DestinationPoint` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

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

## Pointer Direction Indicator (VRTK_PointerDirectionIndicator)

### Overview

Adds a Pointer Direction Indicator to a pointer renderer and determines a given world rotation that can be used by a Destiantion Marker.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/PointerDirectionIndicator/PointerDirectionIndicator` prefab into the scene hierarchy.
 * Attach the `PointerDirectionIndicator` scene GameObejct to the `Direction Indicator` inspector parameter on a `VRTK_BasePointerRenderer` component.

  > This can be useful for rotating the play area upon teleporting to face the user in a new direction without expecting them to physically turn in the play space.

### Inspector Parameters

 * **Touchpad Deadzone:** The touchpad axis needs to be above this deadzone for it to register as a valid touchpad angle.
 * **Coordinate Axis:** The axis to use for the direction coordinates.
 * **Include Headset Offset:** If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.
 * **Display On Invalid Location:** If this is checked then the direction indicator will be displayed when the location is invalid.
 * **Use Pointer Color:** If this is checked then the pointer valid/invalid colours will also be used to change the colour of the direction indicator.
 * **Indicator Visibility:** Determines when the direction indicator will be visible.

### Class Variables

 * `public enum VisibilityState` - States of Direction Indicator Visibility.
   * `OnWhenPointerActive` - Only shows the direction indicator when the pointer is active.
   * `AlwaysOnWithPointerCursor` - Only shows the direction indicator when the pointer cursor is visible or if the cursor is hidden and the pointer is active.

### Class Events

 * `PointerDirectionIndicatorPositionSet` - Emitted when the object tooltip is reset.

### Unity Events

Adding the `VRTK_PointerDirectionIndicator_UnityEvents` component to `VRTK_PointerDirectionIndicator` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### Initialize/1

  > `public virtual void Initialize(VRTK_ControllerEvents events)`

 * Parameters
   * `VRTK_ControllerEvents events` - The Controller Events script that is used to control the direction indicator's rotation.
 * Returns
   * _none_

The Initialize method is used to set up the direction indicator.

#### SetPosition/2

  > `public virtual void SetPosition(bool active, Vector3 position)`

 * Parameters
   * `bool active` - Determines if the direction indicator GameObject should be active or not.
   * `Vector3 position` - The position to set the direction indicator to.
 * Returns
   * _none_

The SetPosition method is used to set the world position of the direction indicator.

#### GetRotation/0

  > `public virtual Quaternion GetRotation()`

 * Parameters
   * _none_
 * Returns
   * `Quaternion` - The reported rotation of the direction indicator.

The GetRotation method returns the current reported rotation of the direction indicator.

#### SetMaterialColor/2

  > `public virtual void SetMaterialColor(Color color, bool validity)`

 * Parameters
   * `Color color` - The colour to update the direction indicatormaterial to.
   * `bool validity` - Determines if the colour being set is based from a valid location or invalid location.
 * Returns
   * _none_

The SetMaterialColor method sets the current material colour on the direction indicator.

#### GetControllerEvents/0

  > `public virtual VRTK_ControllerEvents GetControllerEvents()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerEvents` - The associated Controller Events script.

The GetControllerEvents method returns the associated Controller Events script with the Pointer Direction Indicator script.

---

## Radial Menu (VRTK_RadialMenu)

### Overview

Provides a UI element into the world space that can be dropped into a Controller GameObject and used to create and use Radial Menus from the touchpad.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/RadialMenu/RadialMenu` prefab as a child of a Controller script alias GameObject.

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
 * **Dead Zone:** The dead zone in the middle of the dial where the menu does not consider a button is selected. Set to zero to disable.
 * **Menu Buttons:** The actual GameObjects that make up the radial menu.

### Class Methods

#### TouchAngleDeflection/2

  > `public TouchAngleDeflection(float angle, float deflection)`

 * Parameters
   * `float angle` - The angle of the touch on the radial menu.
   * `float deflection` - Deflection of the touch, where 0 is the centre and 1 is the edge.
 * Returns
   * _none_

Constructs an object to hold the angle and deflection of the user's touch on the touchpad

#### HoverButton/1

  > `public virtual void HoverButton(TouchAngleDeflection givenTouchAngleDeflection)`

 * Parameters
   * `TouchAngleDeflection givenTouchAngleDeflection` - The angle and deflection on the radial menu.
 * Returns
   * _none_

The HoverButton method is used to set the button hover at a given angle and deflection.

#### ClickButton/1

  > `public virtual void ClickButton(TouchAngleDeflection givenTouchAngleDeflection)`

 * Parameters
   * `TouchAngleDeflection givenTouchAngleDeflection` - The angle and deflection on the radial menu.
 * Returns
   * _none_

The ClickButton method is used to set the button click at a given angle and deflection.

#### UnClickButton/1

  > `public virtual void UnClickButton(TouchAngleDeflection givenTouchAngleDeflection)`

 * Parameters
   * `TouchAngleDeflection givenTouchAngleDeflection` - The angle and deflection on the radial menu.
 * Returns
   * _none_

The UnClickButton method is used to set the button unclick at a given angle and deflection.

#### ToggleMenu/0

  > `public virtual void ToggleMenu()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ToggleMenu method is used to show or hide the radial menu.

#### StopTouching/0

  > `public virtual void StopTouching()`

 * Parameters
   * _none_
 * Returns
   * _none_

The StopTouching method is used to stop touching the menu.

#### ShowMenu/0

  > `public virtual void ShowMenu()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ShowMenu method is used to show the menu.

#### GetButton/1

  > `public virtual RadialMenuButton GetButton(int id)`

 * Parameters
   * `int id` - The id of the button to retrieve.
 * Returns
   * `RadialMenuButton` - The found radial menu button.

The GetButton method is used to get a button from the menu.

#### HideMenu/1

  > `public virtual void HideMenu(bool force)`

 * Parameters
   * `bool force` - If true then the menu is always hidden.
 * Returns
   * _none_

The HideMenu method is used to hide the menu.

#### RegenerateButtons/0

  > `public void RegenerateButtons()`

 * Parameters
   * _none_
 * Returns
   * _none_

The RegenerateButtons method creates all the button arcs and populates them with desired icons.

#### AddButton/1

  > `public void AddButton(RadialMenuButton newButton)`

 * Parameters
   * `RadialMenuButton newButton` - The button to add.
 * Returns
   * _none_

The AddButton method is used to add a new button to the menu.

### Example

`VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.

---

## Independent Radial Menu (VRTK_IndependentRadialMenuController)
 > extends VRTK_RadialMenuController

### Overview

Allows the RadialMenu to be anchored to any object, not just a controller.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/RadialMenu/RadialMenu` prefab as a child of the GameObject to associate the Radial Menu with.
 * Position and scale the menu by adjusting the transform of the `RadialMenu` empty.
 * Replace `VRTK_RadialMenuController` with `VRTK_IndependentRadialMenuController` that is located on the `RadialMenu/RadialMenuUI/Panel` GameObject.
 * Ensure the parent object has the `VRTK_InteractableObject` script.
 * Verify that `Is Usable` and `Hold Button to Use` are both checked on the `VRTK_InteractableObject`.
 * Attach `VRTK_InteractTouch` and `VRTK_InteractUse` scripts to the objects that will activate the Radial Menu (e.g. the Controllers).

### Inspector Parameters

 * **Events Manager:** If the RadialMenu is the child of an object with VRTK_InteractableObject attached, this will be automatically obtained. It can also be manually set.
 * **Add Menu Collider:** Whether or not the script should dynamically add a SphereCollider to surround the menu.
 * **Collider Radius Multiplier:** This times the size of the RadialMenu is the size of the collider.
 * **Hide After Execution:** If true, after a button is clicked, the RadialMenu will hide.
 * **Offset Multiplier:** How far away from the object the menu should be placed, relative to the size of the RadialMenu.
 * **Rotate Towards:** The object the RadialMenu should face towards. If left empty, it will automatically try to find the Headset Camera.

### Class Methods

#### UpdateEventsManager/0

  > `public virtual void UpdateEventsManager()`

 * Parameters
   * _none_
 * Returns
   * _none_

The UpdateEventsManager method is used to update the events within the menu controller.

### Example

`VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.

---

## Panel Menu (VRTK_PanelMenuController)

### Overview

Adds a top-level controller to handle the display of up to four child PanelMenuItemController items which are displayed as a canvas UI panel.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/PanelMenu/PanelMenu` prefab as a child of the `VRTK_InteractableObject` the panel menu is for.
 * Optionally remove the panel control menu item child GameObjects if they are not required, e.g. `PanelTopControls`.
 * Set the panel menu item controllers on the `VRTK_PanelMenuController` script to determine which panel control menu items are available.
 * The available panel control menu items can be activated by pressing the corresponding direction on the touchpad.

### Inspector Parameters

 * **Rotate Towards:** The GameObject the panel should rotate towards, which is the Camera (eye) by default.
 * **Zoom Scale Multiplier:** The scale multiplier, which relates to the scale of parent interactable object.
 * **Top Panel Menu Item Controller:** The top PanelMenuItemController, which is triggered by pressing up on the controller touchpad.
 * **Bottom Panel Menu Item Controller:** The bottom PanelMenuItemController, which is triggered by pressing down on the controller touchpad.
 * **Left Panel Menu Item Controller:** The left PanelMenuItemController, which is triggered by pressing left on the controller touchpad.
 * **Right Panel Menu Item Controller:** The right PanelMenuItemController, which is triggered by pressing right on the controller touchpad.

### Class Methods

#### ToggleMenu/0

  > `public virtual void ToggleMenu()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ToggleMenu method is used to show or hide the menu.

#### ShowMenu/0

  > `public virtual void ShowMenu()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ShowMenu method is used to show the menu.

#### HideMenu/1

  > `public virtual void HideMenu(bool force)`

 * Parameters
   * `bool force` - If true then the menu is always hidden.
 * Returns
   * _none_

The HideMenu method is used to hide the menu.

#### HideMenuImmediate/0

  > `public virtual void HideMenuImmediate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The HideMenuImmediate method is used to immediately hide the menu.

### Example

`040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.

---

## Panel Menu Item (VRTK_PanelMenuItemController)

### Overview

Intercepts the controller events sent from a `VRTK_PanelMenuController` and passes them onto additional custom event subscriber scripts, which then carry out the required custom UI actions.

  > This script is not directly part of a prefab but is a helper associated to the `PanelMenu` prefab.

* Place the `VRTK/Prefabs/PanelMenu/VRTK_PanelMenuItemController` script on the child GameObject of any Panel Item Container which is contained within the `PanelMenuController` prefab within the scene.
* Pick up the VRTK_InteractableObject show/hide the panel menu by pressing the touchpad top/bottom/left/right you can open/close the child UI panel that has been assigned via the Unity Editor panel.

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

### Class Methods

#### SetPanelMenuItemEvent/1

  > `public virtual PanelMenuItemControllerEventArgs SetPanelMenuItemEvent(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * `PanelMenuItemControllerEventArgs` - The payload for the event.

The SetPanelMenuItemEvent is used to build up the event payload.

#### Show/1

  > `public virtual void Show(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The Show method is used to show the menu.

#### Hide/1

  > `public virtual void Hide(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The Hide method is used to show the menu.

#### SwipeLeft/1

  > `public virtual void SwipeLeft(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The SwipeLeft method is used when the control is swiped left.

#### SwipeRight/1

  > `public virtual void SwipeRight(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The SwipeRight method is used when the control is swiped right.

#### SwipeTop/1

  > `public virtual void SwipeTop(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The SwipeTop method is used when the control is swiped up.

#### SwipeBottom/1

  > `public virtual void SwipeBottom(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The SwipeBottom method is used when the control is swiped down.

#### TriggerPressed/1

  > `public virtual void TriggerPressed(GameObject interactableObject)`

 * Parameters
   * `GameObject interactableObject` - The object the menu is attached to.
 * Returns
   * _none_

The TriggerPressed method is used when the control action button is pressed.

### Example

`040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.

---

## Avatar Hands (VRTK_AvatarHandController)

### Overview

Provides a custom controller hand model with psuedo finger functionality.

**Prefab Usage:**
 * Place the `VRTK/Prefabs/AvatarHands/BasicHands/VRTK_BasicHand` prefab as a child of either the left or right script alias.
 * If the prefab is being used in the left hand then check the `Mirror Model` parameter.
 * By default, the avatar hand controller will detect which controller is connected and represent it accordingly.
 * Optionally, use SDKTransformModify scripts to adjust the hand orientation based on different controller types.

### Inspector Parameters

 * **Ignore All Overrides:** Determines whether to ignore all of the given overrides on an Interaction event.
 * **State Value:** Sets the Animation parameter for the interaction type and can be used to change the Idle pose based on interaction type.
 * **Apply Thumb Override:** Determines when to apply the given thumb override.
 * **Thumb Override:** The axis override for the thumb on an Interact Touch event. Will only be applicable if the thumb button state is not touched.
 * **Apply Index Override:** Determines when to apply the given index finger override.
 * **Index Override:** The axis override for the index finger on an Interact Touch event. Will only be applicable if the index finger button state is not touched.
 * **Apply Middle Override:** Determines when to apply the given middle finger override.
 * **Middle Override:** The axis override for the middle finger on an Interact Touch event. Will only be applicable if the middle finger button state is not touched.
 * **Apply Ring Override:** Determines when to apply the given ring finger override.
 * **Ring Override:** The axis override for the ring finger on an Interact Touch event. Will only be applicable if the ring finger button state is not touched.
 * **Apply Pinky Override:** Determines when to apply the given pinky finger override.
 * **Pinky Override:** The axis override for the pinky finger on an Interact Touch event.  Will only be applicable if the pinky finger button state is not touched.
 * **Controller Type:** The controller type to use for default finger settings.
 * **Set Fingers For Controller Type:** Determines whether the Finger and State settings are auto set based on the connected controller type.
 * **Mirror Model:** If this is checked then the model will be mirrored, tick this if the avatar hand is for the left hand controller.
 * **Animation Snap Speed:** The speed in which a finger will transition to it's destination position if the finger state is `Digital`.
 * **Thumb Button:** The button alias to control the thumb if the thumb state is `Digital`.
 * **Index Button:** The button alias to control the index finger if the index finger state is `Digital`.
 * **Middle Button:** The button alias to control the middle finger if the middle finger state is `Digital`.
 * **Ring Button:** The button alias to control the ring finger if the ring finger state is `Digital`.
 * **Pinky Button:** The button alias to control the pinky finger if the pinky finger state is `Digital`.
 * **Three Finger Button:** The button alias to control the middle, ring and pinky finger if the three finger state is `Digital`.
 * **Thumb Axis Button:** The button type to listen for axis changes to control the thumb.
 * **Index Axis Button:** The button type to listen for axis changes to control the index finger.
 * **Middle Axis Button:** The button type to listen for axis changes to control the middle finger.
 * **Ring Axis Button:** The button type to listen for axis changes to control the ring finger.
 * **Pinky Axis Button:** The button type to listen for axis changes to control the pinky finger.
 * **Three Finger Axis Button:** The button type to listen for axis changes to control the middle, ring and pinky finger.
 * **Thumb State:** The Axis Type to utilise when dealing with the thumb state. Not all controllers support all axis types on all of the available buttons.
 * **Near Touch Overrides:** Finger axis overrides on an Interact NearTouch event.
 * **Touch Overrides:** Finger axis overrides on an Interact Touch event.
 * **Grab Overrides:** Finger axis overrides on an Interact Grab event.
 * **Use Overrides:** Finger axis overrides on an Interact Use event.
 * **Hand Model:** The Transform that contains the avatar hand model. If this is left blank then a child GameObject named `Model` will be searched for to use as the Transform.
 * **Controller Events:** The controller to listen for the events on. If this is left blank as it will be auto populated by finding the Controller Events script on the parent GameObject.
 * **Interact Near Touch:** An optional Interact NearTouch to listen for near touch events on. If this is left blank as it will attempt to be auto populated by finding the Interact NearTouch script on the parent GameObject.
 * **Interact Touch:** An optional Interact Touch to listen for touch events on. If this is left blank as it will attempt to be auto populated by finding the Interact Touch script on the parent GameObject.
 * **Interact Grab:** An optional Interact Grab to listen for grab events on. If this is left blank as it will attempt to be auto populated by finding the Interact Grab script on the parent GameObject.
 * **Interact Use:** An optional Interact Use to listen for use events on. If this is left blank as it will attempt to be auto populated by finding the Interact Use script on the parent GameObject.

### Class Variables

 * `public enum ApplyOverrideType` - Determine when to apply the override.
   * `Never` - Never apply the override.
   * `Always` - Always apply the override.
   * `DigitalState` - Only apply the override when the state is set to digital.
   * `AxisState` - Only apply the override when the state is set to axis.
   * `SenseAxisState` - Only apply the override when the state is set to sense axis.
   * `AxisAndSenseAxisState` - Only apply the override when the state is set to axis or sense axis.

### Example

`032_Controller_CustomControllerModel` uses the `VRTK_BasicHand` prefab to display custom avatar hands for the left and right controller.

---

# Pointers (VRTK/Source/Scripts/Pointers)

A collection of scripts that provide the ability to create pointers and set destination markers in the scene.

 * [Destination Marker](#destination-marker-vrtk_destinationmarker)
 * [Pointer](#pointer-vrtk_pointer)
 * [Play Area Cursor](#play-area-cursor-vrtk_playareacursor)

---

## Destination Marker (VRTK_DestinationMarker)

### Overview

Provides a base that all destination markers can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides object control action functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Enable Teleport:** If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether destination targets will be considered valid or invalid.

### Class Events

 * `DestinationMarkerEnter` - Emitted when a collision with another collider has first occurred.
 * `DestinationMarkerExit` - Emitted when the collision with the other collider ends.
 * `DestinationMarkerSet` - Emitted when the destination marker is active in the scene to determine the last destination position (useful for selecting and teleporting).

### Unity Events

Adding the `VRTK_DestinationMarker_UnityEvents` component to `VRTK_DestinationMarker` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `float distance` - The distance between the origin and the collided destination.
 * `Transform target` - The Transform of the collided destination object.
 * `RaycastHit raycastHit` - The optional RaycastHit generated from when the ray collided.
 * `Vector3 destinationPosition` - The world position of the destination marker.
 * `Quaternion? destinationRotation` - The world rotation of the destination marker.
 * `bool forceDestinationPosition` - If true then the given destination position should not be altered by anything consuming the payload.
 * `bool enableTeleport` - Whether the destination set event should trigger teleport.
 * `VRTK_ControllerReference controllerReference` - The optional reference to the controller controlling the destination marker.

### Class Methods

#### SetNavMeshData/1

  > `public virtual void SetNavMeshData(VRTK_NavMeshData givenData)`

 * Parameters
   * `VRTK_NavMeshData givenData` - The NavMeshData object that contains the NavMesh restriction settings.
 * Returns
   * _none_

The SetNavMeshData method is used to limit the destination marker to the scene NavMesh based on the settings in the given NavMeshData object.

#### SetHeadsetPositionCompensation/1

  > `public virtual void SetHeadsetPositionCompensation(bool state)`

 * Parameters
   * `bool state` - The state of whether to take the position of the headset within the play area into account when setting the destination marker.
 * Returns
   * _none_

The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.

#### SetForceHoverOnRepeatedEnter/1

  > `public virtual void SetForceHoverOnRepeatedEnter(bool state)`

 * Parameters
   * `bool state` - The state of whether to force the hover on or off.
 * Returns
   * _none_

The SetForceHoverOnRepeatedEnter method is used to set whether the Enter event will forciably call the Hover event if the existing colliding object is the same as it was the previous enter call.

---

## Pointer (VRTK_Pointer)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

Provides a basis of being able to emit a pointer from a specified GameObject.

**Required Components:**
 * `VRTK_BasePointerRenderer` - The visual representation of the pointer when activated.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller` parameter.
 * `VRTK_InteractUse` - The use component to utilise when the pointer is to activate the use action on an Interactable Object. This must be applied on the same GameObject as this script if one is not provided via the `Interact Use` parameter.

**Script Usage:**
 * Place the `VRTK_Pointer` script on either:
   * The controller script alias GameObject of the controller to emit the pointer from (e.g. Right Controller Script Alias).
   * Any other scene GameObject and provide a valid `Transform` component to the `Custom Origin` parameter of this script. This does not have to be a controller and can be any GameObject that will emit the pointer.
 * Link the required Base Pointer Renderer script to the `Pointer Renderer` parameter of this script.

### Inspector Parameters

 * **Pointer Renderer:** The specific renderer to use when the pointer is activated. The renderer also determines how the pointer reaches it's destination (e.g. straight line, bezier curve).
 * **Activation Button:** The button used to activate/deactivate the pointer.
 * **Hold Button To Activate:** If this is checked then the Activation Button needs to be continuously held down to keep the pointer active. If this is unchecked then the Activation Button works as a toggle, the first press/release enables the pointer and the second press/release disables the pointer.
 * **Activate On Enable:** If this is checked then the pointer will be toggled on when the script is enabled.
 * **Activation Delay:** The time in seconds to delay the pointer being able to be active again.
 * **Selection Button:** The button used to execute the select action at the pointer's target position.
 * **Select On Press:** If this is checked then the pointer selection action is executed when the Selection Button is pressed down. If this is unchecked then the selection action is executed when the Selection Button is released.
 * **Selection Delay:** The time in seconds to delay the pointer being able to execute the select action again.
 * **Select After Hover Duration:** The amount of time the pointer can be over the same collider before it automatically attempts to select it. 0f means no selection attempt will be made.
 * **Interact With Objects:** If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.
 * **Grab To Pointer Tip:** If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.
 * **Attached To:** An optional GameObject that determines what the pointer is to be attached to. If this is left blank then the GameObject the script is on will be used.
 * **Controller Events:** An optional Controller Events that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Interact Use:** An optional InteractUse script that will be used when using interactable objects with pointer. If this is left blank then it will attempt to get the InteractUse script from the same GameObject and if it cannot find one then it will attempt to get it from the attached controller.
 * **Custom Origin:** A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.

### Class Events

 * `ActivationButtonPressed` - Emitted when the pointer activation button is pressed.
 * `ActivationButtonReleased` - Emitted when the pointer activation button is released.
 * `SelectionButtonPressed` - Emitted when the pointer selection button is pressed.
 * `SelectionButtonReleased` - Emitted when the pointer selection button is released.
 * `PointerStateValid` - Emitted when the pointer is in a valid state.
 * `PointerStateInvalid` - Emitted when the pointer is in an invalid state.

### Unity Events

Adding the `VRTK_Pointer_UnityEvents` component to `VRTK_Pointer` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### IsActivationButtonPressed/0

  > `public virtual bool IsActivationButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the activationButton is being pressed.

The IsActivationButtonPressed method returns whether the configured activation button is being pressed.

#### IsSelectionButtonPressed/0

  > `public virtual bool IsSelectionButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the selectionButton is being pressed.

The IsSelectionButtonPressed method returns whether the configured activation button is being pressed.

#### PointerEnter/1

  > `public virtual void PointerEnter(RaycastHit givenHit)`

 * Parameters
   * `RaycastHit givenHit` - The valid collision.
 * Returns
   * _none_

The PointerEnter method emits a DestinationMarkerEnter event when the pointer first enters a valid object, it emits a DestinationMarkerHover for every following frame that the pointer stays over the valid object.

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
   * `bool` - Returns `true` if the pointer can be activated.

The CanActivate method is used to determine if the pointer has passed the activation time limit.

#### CanSelect/0

  > `public virtual bool CanSelect()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the pointer can execute the select action.

The CanSelect method is used to determine if the pointer has passed the selection time limit.

#### IsPointerActive/0

  > `public virtual bool IsPointerActive()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the pointer is currently active.

The IsPointerActive method is used to determine if the pointer's current state is active or not.

#### ResetActivationTimer/1

  > `public virtual void ResetActivationTimer(bool forceZero = false)`

 * Parameters
   * `bool forceZero` - If this is `true` then the next activation time will be 0.
 * Returns
   * _none_

The ResetActivationTimer method is used to reset the pointer activation timer to the next valid activation time.

#### ResetSelectionTimer/1

  > `public virtual void ResetSelectionTimer(bool forceZero = false)`

 * Parameters
   * `bool forceZero` - If this is `true` then the next activation time will be 0.
 * Returns
   * _none_

The ResetSelectionTimer method is used to reset the pointer selection timer to the next valid activation time.

#### Toggle/1

  > `public virtual void Toggle(bool state)`

 * Parameters
   * `bool state` - If `true` the pointer will be enabled if possible, if `false` the pointer will be disabled if possible.
 * Returns
   * _none_

The Toggle method is used to enable or disable the pointer.

#### IsStateValid/0

  > `public virtual bool IsStateValid()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the pointer is in the valid state (showing the valid colour), returns `false` if the pointer is in the invalid state (showing the invalid colour).

The IsStateValid method is used to determine if the pointer is currently in a valid state (i.e. on it's valid colour).

---

## Play Area Cursor (VRTK_PlayAreaCursor)

### Overview

Provides a visual representation of the play area boundaries that tracks to the cursor position of a pointer.

**Optional Components:**
 * `VRTK_PointerDirectionIndicator` - A Pointer Direction Indicator to set the cursor rotation to.

**Script Usage:**
 * Place the `VRTK_PlayAreaCursor` script on the same GameObject as the Pointer Renderer script it is linked to.
 * Link the required Play Area Cursor script to the `Playarea Cursor` parameter on the required Pointer Renderer script.

**Script Dependencies:**
 * A Base Pointer Renderer script attached to a valid Pointer script is required so the PlayArea Cursor script can be linked to follow the valid Base Pointer Renderer cursor GameObject.

### Inspector Parameters

 * **Use Pointer Color:** If this is checked then the pointer valid/invalid colours will also be used to change the colour of the play area cursor when colliding/not colliding.
 * **Play Area Cursor Dimensions:** Determines the size of the play area cursor and collider. If the values are left as zero then the Play Area Cursor will be sized to the calibrated Play Area space.
 * **Handle Play Area Cursor Collisions:** If this is checked then if the play area cursor is colliding with any other object then the pointer colour will change to the `Pointer Miss Color` and the `DestinationMarkerSet` event will not be triggered, which will prevent teleporting into areas where the play area will collide.
 * **Headset Out Of Bounds Is Collision:** If this is checked then if the user's headset is outside of the play area cursor bounds then it is considered a collision even if the play area isn't colliding with anything.
 * **Display On Invalid Location:** If this is checked then the play area cursor will be displayed when the location is invalid.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether the play area cursor collisions will be acted upon.
 * **Direction Indicator:** A custom Pointer Direction Indicator to use to determine the rotation of the Play Area Cursor.
 * **Valid Location Object:** A custom GameObject to use for the play area cursor representation for when the location is valid.
 * **Invalid Location Object:** A custom GameObject to use for the play area cursor representation for when the location is invalid.

### Class Events

 * `PlayAreaCursorStartCollision` - Emitted when the play area collides with another object.
 * `PlayAreaCursorEndCollision` - Emitted when the play area stops colliding with another object.

### Unity Events

Adding the `VRTK_PlayAreaCursor_UnityEvents` component to `VRTK_PlayAreaCursor` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * ` collidedWith` - The collider that is/was being collided with.

### Class Methods

#### HasCollided/0

  > `public virtual bool HasCollided()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the play area is colliding with a valid object and `false` if not.

The HasCollided method returns the state of whether the play area cursor has currently collided with another valid object.

#### SetHeadsetPositionCompensation/1

  > `public virtual void SetHeadsetPositionCompensation(bool state)`

 * Parameters
   * `bool state` - The state of whether to take the position of the headset within the play area into account when setting the destination marker.
 * Returns
   * _none_

The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.

#### SetPlayAreaCursorCollision/2

  > `public virtual void SetPlayAreaCursorCollision(bool state, Collider collider = null)`

 * Parameters
   * `bool state` - The state of whether to check for play area collisions.
   * `Collider collider` - The state of whether to check for play area collisions.
 * Returns
   * _none_

The SetPlayAreaCursorCollision method determines whether play area collisions should be taken into consideration with the play area cursor.

#### SetMaterialColor/2

  > `public virtual void SetMaterialColor(Color color, bool validity)`

 * Parameters
   * `Color color` - The colour to update the play area cursor material to.
   * `bool validity` - Determines if the colour being set is based from a valid location or invalid location.
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

#### IsActive/0

  > `public virtual bool IsActive()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the play area cursor GameObject is active.

The IsActive method returns whether the play area cursor GameObject is active or not.

#### GetPlayAreaContainer/0

  > `public virtual GameObject GetPlayAreaContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject that is the container of the play area cursor.

The GetPlayAreaContainer method returns the created GameObject that holds the play area cursor representation.

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

# Pointer Renderers (VRTK/Source/Scripts/Pointers/PointerRenderers)

A collection of scripts that are used to provide different renderers for the VRTK_Pointer.

 * [Base Pointer Renderer](#base-pointer-renderer-vrtk_basepointerrenderer)
 * [Straight Pointer Renderer](#straight-pointer-renderer-vrtk_straightpointerrenderer)
 * [Bezier Pointer Renderer](#bezier-pointer-renderer-vrtk_bezierpointerrenderer)

---

## Base Pointer Renderer (VRTK_BasePointerRenderer)

### Overview

Provides a base that all pointer renderers can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides pointer renderer functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Smooths Position:** Whether or not to smooth the position of the pointer origin when positioning the pointer tip.
 * **Max Allowed Per Frame Distance Difference:** The maximum allowed distance between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.
 * **Smooths Rotation:** Whether or not to smooth the rotation of the pointer origin when positioning the pointer tip.
 * **Max Allowed Per Frame Angle Difference:** The maximum allowed angle between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.
 * **Playarea Cursor:** An optional Play Area Cursor generator to add to the destination position of the pointer tip.
 * **Direction Indicator:** A custom VRTK_PointerDirectionIndicator to use to determine the rotation given to the destination set event.
 * **Custom Raycast:** A custom raycaster to use for the pointer's raycasts to ignore.
 * **Pointer Origin Smoothing Settings:** Specifies the smoothing to be applied to the pointer origin when positioning the pointer tip.
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

#### GetPointerObjects/0

  > `public abstract GameObject[] GetPointerObjects();`

 * Parameters
   * _none_
 * Returns
   * `GameObject[]` - An array of pointer auto generated GameObjects.

The GetPointerObjects returns an array of the auto generated GameObjects associated with the pointer.

#### InitalizePointer/4

  > `public virtual void InitalizePointer(VRTK_Pointer givenPointer, VRTK_PolicyList givenInvalidListPolicy, VRTK_NavMeshData givenNavMeshData, bool givenHeadsetPositionCompensation)`

 * Parameters
   * `VRTK_Pointer givenPointer` - The VRTK_Pointer that is controlling the pointer renderer.
   * `VRTK_PolicyList givenInvalidListPolicy` - The VRTK_PolicyList for managing valid and invalid pointer locations.
   * `VRTK_NavMeshData givenNavMeshData` - The NavMeshData object that contains the Nav Mesh restriction options.
   * `bool givenHeadsetPositionCompensation` - Determines whether the play area cursor will take the headset position within the play area into account when being displayed.
 * Returns
   * _none_

The InitalizePointer method is used to set up the state of the pointer renderer.

#### ResetPointerObjects/0

  > `public virtual void ResetPointerObjects()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetPointerObjects method is used to destroy any existing pointer objects and recreate them at runtime.

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

#### IsVisible/0

  > `public virtual bool IsVisible()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if either the tracer or cursor renderers are visible. Returns false if none are visible.

The IsVisible method determines if the pointer renderer is at all visible by checking the state of the tracer and the cursor.

#### IsTracerVisible/0

  > `public virtual bool IsTracerVisible()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the tracer renderers are visible.

The IsTracerVisible method determines if the pointer tracer renderer is visible.

#### IsCursorVisible/0

  > `public virtual bool IsCursorVisible()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the cursor renderers are visible.

The IsCursorVisible method determines if the pointer cursor renderer is visible.

#### IsValidCollision/0

  > `public virtual bool IsValidCollision()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the pointer is in a valid collision, returns false if the pointer is in an invalid collision state.

The IsValidCollision method determines if the pointer is currently in it's valid collision state.

#### GetObjectInteractor/0

  > `public virtual GameObject GetObjectInteractor()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The auto generated object interactor GameObject.
   * `GameObject` -

The GetObjectInteractor method returns the auto generated GameObject that acts as the controller extension for interacting with objects.

---

## Straight Pointer Renderer (VRTK_StraightPointerRenderer)
 > extends [VRTK_BasePointerRenderer](#base-pointer-renderer-vrtk_basepointerrenderer)

### Overview

A visual pointer representation of a straight beam with an optional cursor at the end.

**Optional Components:**
 * `VRTK_PlayAreaCursor` - A Play Area Cursor that will track the position of the pointer cursor.
 * `VRTK_PointerDirectionIndicator` - A Pointer Direction Indicator that will track the position of the pointer cursor.

**Script Usage:**
 * Place the `VRTK_StraightPointerRenderer` script on the same GameObject as the Pointer script it is linked to.
 * Link this Pointer Renderer script to the `Pointer Renderer` parameter on the required Pointer script.

**Script Dependencies:**
 * A Pointer script to control the activation of this Pointer Renderer script.

### Inspector Parameters

 * **Maximum Length:** The maximum length the pointer tracer can reach.
 * **Scale Factor:** The scale factor to scale the pointer tracer object by.
 * **Cursor Scale Multiplier:** The scale multiplier to scale the pointer cursor object by in relation to the `Scale Factor`.
 * **Cursor Match Target Rotation:** The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.
 * **Cursor Distance Rescale:** Rescale the cursor proportionally to the distance from the tracer origin.
 * **Maximum Cursor Scale:** The maximum scale the cursor is allowed to reach. This is only used when rescaling the cursor proportionally to the distance from the tracer origin.
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

#### GetPointerObjects/0

  > `public override GameObject[] GetPointerObjects()`

 * Parameters
   * _none_
 * Returns
   * `GameObject[]` - An array of pointer auto generated GameObjects.

The GetPointerObjects returns an array of the auto generated GameObjects associated with the pointer.

### Example

`VRTK/Examples/003_Controller_SimplePointer` shows the simple pointer in action and code examples of how the events are utilised and listened to can be viewed in the script `VRTK/Examples/ExampleResources/Scripts/VRTK_ControllerPointerEvents_ListenerExample.cs`

---

## Bezier Pointer Renderer (VRTK_BezierPointerRenderer)
 > extends [VRTK_BasePointerRenderer](#base-pointer-renderer-vrtk_basepointerrenderer)

### Overview

A visual pointer representation of a curved beam made from multiple objects with an optional cursor at the end.

  > The bezier curve generation code is in another script located at `VRTK/Source/Scripts/Internal/VRTK_CurveGenerator.cs` and was heavily inspired by the tutorial and code from [Catlike Coding](http://catlikecoding.com/unity/tutorials/curves-and-splines/).

**Optional Components:**
 * `VRTK_PlayAreaCursor` - A Play Area Cursor that will track the position of the pointer cursor.
 * `VRTK_PointerDirectionIndicator` - A Pointer Direction Indicator that will track the position of the pointer cursor.

**Script Usage:**
 * Place the `VRTK_BezierPointerRenderer` script on the same GameObject as the Pointer script it is linked to.
 * Link this Pointer Renderer script to the `Pointer Renderer` parameter on the required Pointer script.

**Script Dependencies:**
 * A Pointer script to control the activation of this Pointer Renderer script.

### Inspector Parameters

 * **Maximum Length:** The maximum length of the projected beam. The x value is the length of the forward beam, the y value is the length of the downward beam.
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

#### GetPointerObjects/0

  > `public override GameObject[] GetPointerObjects()`

 * Parameters
   * _none_
 * Returns
   * `GameObject[]` - An array of pointer auto generated GameObjects.

The GetPointerObjects returns an array of the auto generated GameObjects associated with the pointer.

### Example

`VRTK/Examples/009_Controller_BezierPointer` is used in conjunction with the Height Adjust Teleporter shows how it is possible to traverse different height objects using the curved pointer without needing to see the top of the object.

`VRTK/Examples/036_Controller_CustomCompoundPointer' shows how to display an object (a teleport beam) only if the teleport location is valid, and can create an animated trail along the tracer curve.

---

# Locomotion (VRTK/Source/Scripts/Locomotion)

A collection of scripts that provide varying methods of moving the user around the scene.

 * [Basic Teleport](#basic-teleport-vrtk_basicteleport)
 * [Height Adjust Teleport](#height-adjust-teleport-vrtk_heightadjustteleport)
 * [Dash Teleport](#dash-teleport-vrtk_dashteleport)
 * [Teleport Disable On Headset Collision](#teleport-disable-on-headset-collision-vrtk_teleportdisableonheadsetcollision)
 * [Teleport Disable On Controller Obscured](#teleport-disable-on-controller-obscured-vrtk_teleportdisableoncontrollerobscured)
 * [Object Control](#object-control-vrtk_objectcontrol)
 * [Touchpad Control](#touchpad-control-vrtk_touchpadcontrol)
 * [Button Control](#button-control-vrtk_buttoncontrol)
 * [Move In Place](#move-in-place-vrtk_moveinplace)
 * [Player Climb](#player-climb-vrtk_playerclimb)
 * [Slingshot Jump](#slingshot-jump-vrtk_slingshotjump)
 * [Step Multiplier](#step-multiplier-vrtk_stepmultiplier)
 * [Tunnel Overlay](#tunnel-overlay-vrtk_tunneloverlay)
 * [Drag World](#drag-world-vrtk_dragworld)

---

## Basic Teleport (VRTK_BasicTeleport)

### Overview

Updates the `x/z` position of the SDK Camera Rig with an optional screen fade.

  > The `y` position is not altered by the Basic Teleport so it only allows for movement across a 2D plane.

**Script Usage:**
 * Place the `VRTK_BasicTeleport` script on any active scene GameObject.

**Script Dependencies:**
 * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.

### Inspector Parameters

 * **Blink To Color:** The colour to fade to when fading on teleport.
 * **Blink Transition Speed:** The time taken to fade to the `Blink To Color`. Setting the speed to `0` will mean no fade effect is present.
 * **Distance Blink Delay:** Determines how long the fade will stay present out depending on the distance being teleported. A value of `0` will not delay the teleport fade effect over any distance, a max value will delay the teleport fade in even when the distance teleported is very close to the original position.
 * **Headset Position Compensation:** If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether destination targets will be acted upon by the teleporter.
 * **Nav Mesh Data:** An optional NavMeshData object that will be utilised for limiting the teleport to within any scene NavMesh.

### Class Events

 * `Teleporting` - Emitted when the teleport process has begun.
 * `Teleported` - Emitted when the teleport process has successfully completed.

### Unity Events

Adding the `VRTK_BasicTeleport_UnityEvents` component to `VRTK_BasicTeleport` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `float distance` - The distance between the origin and the collided destination.
 * `Transform target` - The Transform of the collided destination object.
 * `RaycastHit raycastHit` - The optional RaycastHit generated from when the ray collided.
 * `Vector3 destinationPosition` - The world position of the destination marker.
 * `Quaternion? destinationRotation` - The world rotation of the destination marker.
 * `bool forceDestinationPosition` - If true then the given destination position should not be altered by anything consuming the payload.
 * `bool enableTeleport` - Whether the destination set event should trigger teleport.
 * `VRTK_ControllerReference controllerReference` - The optional reference to the controller controlling the destination marker.

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
   * `bool` - Returns `true` if the target is a valid location.

The ValidLocation method determines if the given target is a location that can be teleported to

#### Teleport/1

  > `public virtual void Teleport(DestinationMarkerEventArgs teleportArgs)`

 * Parameters
   * `DestinationMarkerEventArgs teleportArgs` - The pseudo Destination Marker event for the teleport action.
 * Returns
   * _none_

The Teleport/1 method calls the teleport to update position without needing to listen for a Destination Marker event.

#### Teleport/4

  > `public virtual void Teleport(Transform target, Vector3 destinationPosition, Quaternion? destinationRotation = null, bool forceDestinationPosition = false)`

 * Parameters
   * `Transform target` - The Transform of the destination object.
   * `Vector3 destinationPosition` - The world position to teleport to.
   * `Quaternion? destinationRotation` - The world rotation to teleport to.
   * `bool forceDestinationPosition` - If `true` then the given destination position should not be altered by anything consuming the payload.
 * Returns
   * _none_

The Teleport/4 method calls the teleport to update position without needing to listen for a Destination Marker event. It will build a destination marker out of the provided parameters.

#### ForceTeleport/2

  > `public virtual void ForceTeleport(Vector3 destinationPosition, Quaternion? destinationRotation = null)`

 * Parameters
   * `Vector3 destinationPosition` - The world position to teleport to.
   * `Quaternion? destinationRotation` - The world rotation to teleport to.
 * Returns
   * _none_

The ForceTeleport method forces the position to update to a given destination and ignores any target checking or floor adjustment.

#### SetActualTeleportDestination/2

  > `public virtual void SetActualTeleportDestination(Vector3 actualPosition, Quaternion? actualRotation)`

 * Parameters
   * `Vector3 actualPosition` - The actual position that the teleport event should use as the final location.
   * `Quaternion? actualRotation` - The actual rotation that the teleport event should use as the final location.
 * Returns
   * _none_

The SetActualTeleportDestination method forces the destination of a teleport event to the given Vector3.

#### ResetActualTeleportDestination/0

  > `public virtual void ResetActualTeleportDestination()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetActualTeleportDestination method removes any previous forced destination position that was set by the SetActualTeleportDestination method.

### Example

`VRTK/Examples/004_CameraRig_BasicTeleport` uses the `VRTK_Pointer` script on the Controllers to initiate a laser pointer by pressing the `Touchpad` on the controller and when the laser pointer is deactivated (release the `Touchpad`) then the user is teleported to the location of the laser pointer tip as this is where the pointer destination marker position is set to.

---

## Height Adjust Teleport (VRTK_HeightAdjustTeleport)
 > extends [VRTK_BasicTeleport](#basic-teleport-vrtk_basicteleport)

### Overview

Updates the `x/y/z` position of the SDK Camera Rig with an optional screen fade.

  > The Camera Rig can be automatically teleported to the nearest floor `y` position when utilising this teleporter.

**Script Usage:**
 * Place the `VRTK_HeightAdjustTeleport` script on any active scene GameObject.

**Script Dependencies:**
 * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.

### Inspector Parameters

 * **Snap To Nearest Floor:** If this is checked, then the teleported Y position will snap to the nearest available below floor. If it is unchecked, then the teleported Y position will be where ever the destination Y position is.
 * **Apply Playarea Parent Offset:** If this is checked then the teleported Y position will also be offset by the play area parent Transform Y position (if the play area has a parent).
 * **Custom Raycast:** A custom raycaster to use when raycasting to find floors.

### Example

`VRTK/Examples/007_CameraRig_HeightAdjustTeleport` has a collection of varying height objects that the user can either walk up and down or use the laser pointer to climb on top of them.

`VRTK/Examples/010_CameraRig_TerrainTeleporting` shows how the teleportation of a user can also traverse terrain colliders.

`VRTK/Examples/020_CameraRig_MeshTeleporting` shows how the teleportation of a user can also traverse mesh colliders.

---

## Dash Teleport (VRTK_DashTeleport)
 > extends [VRTK_HeightAdjustTeleport](#height-adjust-teleport-vrtk_heightadjustteleport)

### Overview

Updates the `x/y/z` position of the SDK Camera Rig with a lerp to the new position creating a dash effect.

**Script Usage:**
 * Place the `VRTK_DashTeleport` script on any active scene GameObject.

**Script Dependencies:**
 * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.

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

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `RaycastHit[] hits` - An array of RaycastHits that the CapsuleCast has collided with.

### Example

`VRTK/Examples/038_CameraRig_DashTeleport` shows how to turn off the mesh renderers of objects that are in the way during the dash.

---

## Teleport Disable On Headset Collision (VRTK_TeleportDisableOnHeadsetCollision)

### Overview

Prevents teleportation when the HMD is colliding with valid geometry.

**Required Components:**
 * `VRTK_BasicTeleport` - A Teleport script to utilise for teleporting the play area.
 * `VRTK_HeadsetCollision` - A Headset Collision script for detecting when the headset has collided with valid geometry.

**Script Usage:**
 * Place the `VRTK_TeleportDisableOnHeadsetCollision` script on any active scene GameObject.

### Inspector Parameters

 * **Teleporter:** The Teleporter script to deal play area teleporting. If the script is being applied onto an object that already has a VRTK_BasicTeleport component, this parameter can be left blank as it will be auto populated by the script at runtime.
 * **Headset Collision:** The VRTK Headset Collision script to use when determining headset collisions. If this is left blank then the script will need to be applied to the same GameObject.

---

## Teleport Disable On Controller Obscured (VRTK_TeleportDisableOnControllerObscured)

### Overview

Prevents teleportation when the controllers are obscured from line of sight of the HMD.

**Required Components:**
 * `VRTK_BasicTeleport` - A Teleport script to utilise for teleporting the play area.
 * `VRTK_HeadsetControllerAware` - A Headset Controller Aware script to determine when the HMD has line of sight to the controllers.

**Script Usage:**
 * Place the `VRTK_TeleportDisableOnControllerObscured` script on any active scene GameObject.

### Inspector Parameters

 * **Teleporter:** The Teleporter script to deal play area teleporting. If the script is being applied onto an object that already has a VRTK_BasicTeleport component, this parameter can be left blank as it will be auto populated by the script at runtime.
 * **Headset Controller Aware:** The VRTK Headset Controller Aware script to use when dealing with the headset to controller awareness. If this is left blank then the script will need to be applied to the same GameObject.

---

## Object Control (VRTK_ObjectControl)

### Overview

Provides a base that all object control locomotions can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides object control locomotion functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Device For Direction:** The direction that will be moved in is the direction of this device.
 * **Disable Other Controls On Active:** If this is checked then whenever the axis on the attached controller is being changed, all other object control scripts of the same type on other controllers will be disabled.
 * **Affect On Falling:** If a `VRTK_BodyPhysics` script is present and this is checked, then the object control will affect the play area whilst it is falling.
 * **Control Override Object:** An optional game object to apply the object control to. If this is blank then the PlayArea will be controlled.
 * **Controller:** The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same GameObject.
 * **Body Physics:** An optional Body Physics script to check for potential collisions in the moving direction.

### Class Variables

 * `public enum DirectionDevices` - Devices for providing direction.
   * `Headset` - The headset device.
   * `LeftController` - The left controller device.
   * `RightController` - The right controller device.
   * `ControlledObject` - The controlled object.

### Class Events

 * `XAxisChanged` - Emitted when the X Axis Changes.
 * `YAxisChanged` - Emitted when the Y Axis Changes.

### Unity Events

Adding the `VRTK_ObjectControl_UnityEvents` component to `VRTK_ObjectControl` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject controlledGameObject` - The GameObject that is going to be affected.
 * `Transform directionDevice` - The device that is used for the direction.
 * `Vector3 axisDirection` - The axis that is being affected.
 * `Vector3 axis` - The value of the current touchpad touch point based across the axis direction.
 * `float deadzone` - The value of the deadzone based across the axis direction.
 * `bool currentlyFalling` - Whether the controlled GameObject is currently falling.
 * `bool modifierActive` - Whether the modifier button is pressed.

---

## Touchpad Control (VRTK_TouchpadControl)
 > extends [VRTK_ObjectControl](#object-control-vrtk_objectcontrol)

### Overview

Provides the ability to control a GameObject's position based on the position of the controller touchpad axis.

  > This script forms the stub of emitting the touchpad axis X and Y changes that are then digested by the corresponding Object Control Actions that are listening for the relevant event.

**Required Components:**
 * `VRTK_ControllerEvents` - The Controller Events script to listen for the touchpad events on.

**Optional Components:**
 * `VRTK_BodyPhysics` - The Body Physics script to utilise to determine if falling is occuring.

**Script Usage:**
 * Place the `VRTK_TouchpadControl` script on either:
   * The GameObject with the Controller Events script.
   * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller` parameter of this script.
 * Place a corresponding Object Control Action for the Touchpad Control script to notify of touchpad changes. Without a corresponding Object Control Action, the Touchpad Control script will do nothing.

### Inspector Parameters

 * **Coordinate Axis:** The axis to use for the direction coordinates.
 * **Primary Activation Button:** An optional button that has to be engaged to allow the touchpad control to activate.
 * **Action Modifier Button:** An optional button that when engaged will activate the modifier on the touchpad control action.
 * **Axis Deadzone:** A deadzone threshold on the touchpad that will ignore input if the touch position is within the specified deadzone. Between `0f` and `1f`.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

---

## Button Control (VRTK_ButtonControl)
 > extends [VRTK_ObjectControl](#object-control-vrtk_objectcontrol)

### Overview

Provides the ability to control a GameObject's position based the press of a controller button linked to a specific axis direction.

  > This script forms the stub of emitting the axis X and Y changes that are then digested by the corresponding Object Control Actions that are listening for the relevant event.

**Required Components:**
 * `VRTK_ControllerEvents` - The Controller Events script to listen for button presses events on.

**Optional Components:**
 * `VRTK_BodyPhysics` - The Body Physics script to utilise to determine if falling is occuring.

**Script Usage:**
 * Place the `VRTK_ButtonControl` script on either:
   * The GameObject with the Controller Events script.
   * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller` parameter of this script.
 * Place a corresponding Object Control Action for the Button Control script to notify of axis changes. Without a corresponding Object Control Action, the Button Control script will do nothing.

### Inspector Parameters

 * **Forward Button:** The button to set the y axis to +1.
 * **Backward Button:** The button to set the y axis to -1.
 * **Left Button:** The button to set the x axis to -1.
 * **Right Button:** The button to set the x axis to +1.

---

## Move In Place (VRTK_MoveInPlace)

### Overview

Moves the SDK Camera Rig based on the motion of the headset and/or the controllers. Attempts to recreate the concept of physically walking on the spot to create scene movement.

  > This locomotion method is based on Immersive Movement, originally created by Highsight. Thanks to KJack (author of Arm Swinger) for additional work.

**Optional Components:**
 * `VRTK_BodyPhysics` - A Body Physics script to help determine potential collisions in the moving direction and prevent collision tunnelling.

**Script Usage:**
 * Place the `VRTK_MoveInPlace` script on any active scene GameObject.

**Script Dependencies:**
 * The Controller Events script on the controller Script Alias to determine when the engage button is pressed.

### Inspector Parameters

 * **Left Controller:** If this is checked then the left controller engage button will be enabled to move the play area.
 * **Right Controller:** If this is checked then the right controller engage button will be enabled to move the play area.
 * **Engage Button:** The button to press to activate the movement.
 * **Control Options:** The device to determine the movement paramters from.
 * **Direction Method:** The method in which to determine the direction of forward movement.
 * **Speed Scale:** The speed in which to move the play area.
 * **Max Speed:** The maximun speed in game units. (If 0 or less, max speed is uncapped)
 * **Deceleration:** The speed in which the play area slows down to a complete stop when the engage button is released. This deceleration effect can ease any motion sickness that may be suffered.
 * **Falling Deceleration:** The speed in which the play area slows down to a complete stop when falling is occuring.
 * **Smart Decouple Threshold:** The degree threshold that all tracked objects (controllers, headset) must be within to change direction when using the Smart Decoupling Direction Method.
 * **Sensitivity:** The maximum amount of movement required to register in the virtual world.  Decreasing this will increase acceleration, and vice versa.
 * **Body Physics:** An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.

### Class Variables

 * `public enum ControlOptions` - Valid control options
   * `HeadsetAndControllers` - Track both headset and controllers for movement calculations.
   * `ControllersOnly` - Track only the controllers for movement calculations.
   * `HeadsetOnly` - Track only headset for movement caluclations.
 * `public enum DirectionalMethod` - Options for which method is used to determine direction while moving.
   * `Gaze` - Will always move in the direction they are currently looking.
   * `ControllerRotation` - Will move in the direction that the controllers are pointing (averaged).
   * `DumbDecoupling` - Will move in the direction they were first looking when they engaged Move In Place.
   * `SmartDecoupling` - Will move in the direction they are looking only if their headset point the same direction as their controllers.
   * `EngageControllerRotationOnly` - Will move in the direction that the controller with the engage button pressed is pointing.
   * `LeftControllerRotationOnly` - Will move in the direction that the left controller is pointing.
   * `RightControllerRotationOnly` - Will move in the direction that the right controller is pointing.

### Class Methods

#### SetControlOptions/1

  > `public virtual void SetControlOptions(ControlOptions givenControlOptions)`

 * Parameters
   * `ControlOptions givenControlOptions` - The control options to set the current control options to.
 * Returns
   * _none_

Set the control options and modify the trackables to match.

#### GetMovementDirection/0

  > `public virtual Vector3 GetMovementDirection()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - Returns a Vector3 representing the current movement direction.

The GetMovementDirection method will return the direction the play area is currently moving in.

#### GetSpeed/0

  > `public virtual float GetSpeed()`

 * Parameters
   * _none_
 * Returns
   * `float` - Returns a float representing the current movement speed.

The GetSpeed method will return the current speed the play area is moving at.

### Example

`VRTK/Examples/042_CameraRig_MoveInPlace` demonstrates how the user can move and traverse colliders by either swinging the controllers in a walking fashion or by running on the spot utilisng the head bob for movement.

---

## Player Climb (VRTK_PlayerClimb)

### Overview

Provides the ability for the SDK Camera Rig to be moved around based on whether an Interact Grab is interacting with a Climbable Interactable Object to simulate climbing.

**Required Components:**
 * `VRTK_BodyPhysics` - A Body Physics script to deal with the effects of physics and gravity on the play area.

**Optional Components:**
 * `VRTK_BasicTeleport` - A Teleporter script to use when snapping the play area to the nearest floor when releasing from grab.
 * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the headset is colliding with geometry to know when to reset to a valid location.
 * `VRTK_PositionRewind` - A Position Rewind script to utilise when resetting to a valid location upon ungrabbing whilst colliding with geometry.

**Script Usage:**
 * Place the `VRTK_PlayerClimb` script on any active scene GameObject.

**Script Dependencies:**
 * The controller Script Alias GameObject requires the Interact Touch and Interact Grab scripts to allow for touching and grabbing of Interactable Objects.
 * An Interactable Object in the scene that has the Climbable Grab Attach Mechanic.

### Inspector Parameters

 * **Use Player Scale:** Will scale movement up and down based on the player transform's scale.
 * **Body Physics:** The Body Physics script to use for dealing with climbing and falling. If this is left blank then the script will need to be applied to the same GameObject.
 * **Teleporter:** The Teleport script to use when snapping to nearest floor on release. If this is left blank then a Teleport script will need to be applied to the same GameObject.
 * **Headset Collision:** The Headset Collision script to use for determining if the user is climbing inside a collidable object. If this is left blank then the script will need to be applied to the same GameObject.
 * **Position Rewind:** The Position Rewind script to use for dealing resetting invalid positions. If this is left blank then the script will need to be applied to the same GameObject.

### Class Events

 * `PlayerClimbStarted` - Emitted when player climbing has started.
 * `PlayerClimbEnded` - Emitted when player climbing has ended.

### Unity Events

Adding the `VRTK_PlayerClimb_UnityEvents` component to `VRTK_PlayerClimb` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerReference controllerReference` - The reference to the controller doing the interaction.
 * `GameObject target` - The GameObject of the interactable object that is being interacted with by the controller.

### Class Methods

#### IsClimbing/0

  > `public virtual bool IsClimbing()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if climbing is currently taking place.

The IsClimbing method will return if climbing is currently taking place or not.

### Example

`VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with player climbing. There are many different examples showing how the same system can be used in unique ways.

---

## Slingshot Jump (VRTK_SlingshotJump)

### Overview

Provides the ability for the SDK Camera Rig to be thrown around with a jumping motion by slingshotting based on the pull back of each valid controller.

**Required Components:**
 * `VRTK_PlayerClimb` - A Player Climb script for dealing with the physical throwing of the play area as if throwing off an invisible climbed object.
 * `VRTK_BodyPhysics` - A Body Physics script to deal with the effects of physics and gravity on the play area.

**Optional Components:**
 * `VRTK_BasicTeleport` - A Teleporter script to use when snapping the play area to the nearest floor when releasing from grab.
 * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the headset is colliding with geometry to know when to reset to a valid location.
 * `VRTK_PositionRewind` - A Position Rewind script to utilise when resetting to a valid location upon ungrabbing whilst colliding with geometry.

**Script Usage:**
 * Place the `VRTK_SlingshotJump` script on the same GameObject as the `VRTK_PlayerClimb` script.

### Inspector Parameters

 * **Release Window Time:** How close together the button releases have to be to initiate a jump.
 * **Velocity Multiplier:** Multiplier that increases the jump strength.
 * **Velocity Max:** The maximum velocity a jump can be.

### Class Events

 * `SlingshotJumped` - Emitted when a slingshot jump occurs

### Unity Events

Adding the `VRTK_SlingshotJump_UnityEvents` component to `VRTK_SlingshotJump` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### GetActivationButton/0

  > `public virtual VRTK_ControllerEvents.ButtonAlias GetActivationButton()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerEvents.ButtonAlias` - Returns the button used for slingshot activation.

The SetActivationButton method gets the button used to activate a slingshot jump.

#### SetActivationButton/1

  > `public virtual void SetActivationButton(VRTK_ControllerEvents.ButtonAlias button)`

 * Parameters
   * `VRTK_ControllerEvents.ButtonAlias button` - The controller button to use to activate the jump.
 * Returns
   * _none_

The SetActivationButton method sets the button used to activate a slingshot jump.

#### GetCancelButton/0

  > `public virtual VRTK_ControllerEvents.ButtonAlias GetCancelButton()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerEvents.ButtonAlias` - Returns the button used to cancel a slingshot jump.

The GetCancelButton method gets the button used to cancel a slingshot jump.

#### SetCancelButton/1

  > `public virtual void SetCancelButton(VRTK_ControllerEvents.ButtonAlias button)`

 * Parameters
   * `VRTK_ControllerEvents.ButtonAlias button` - The controller button to use to cancel the jump.
 * Returns
   * _none_

The SetCancelButton method sets the button used to cancel a slingshot jump.

### Example

`VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with slingshot jumping. This script just needs to be added to the PlayArea object and the requested forces and buttons set.

---

## Step Multiplier (VRTK_StepMultiplier)

### Overview

Multiplies each real world step within the play area to enable further distances to be travelled in the virtual world.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller Events` parameter.

**Script Usage:**
 * Place the `VRTK_StepMultiplier` script on either:
   * Any GameObject in the scene if no activation button is required.
   * The GameObject with the Controller Events scripts if an activation button is required.
   * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller Events` parameter of this script if an activation button is required.

### Inspector Parameters

 * **Activation Button:** The controller button to activate the step multiplier effect. If it is `Undefined` then the step multiplier will always be active.
 * **Movement Function:** This determines the type of movement used by the extender.
 * **Additional Movement Multiplier:** This is the factor by which movement at the edge of the circle is amplified. `0` is no movement of the play area. Higher values simulate a bigger play area but may be too uncomfortable.
 * **Head Zone Radius:** This is the size of the circle in which the play area is not moved and everything is normal. If it is to low it becomes uncomfortable when crouching.
 * **Controller Events:** The Controller Events to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.

### Class Variables

 * `public enum MovementFunction` - Movement methods.
   * `Nonlinear` - Moves the head with a non-linear drift movement.
   * `LinearDirect` - Moves the headset in a direct linear movement.

### Example

`VRTK/Examples/028_CameraRig_RoomExtender` shows how the Step Multiplier can be used to move around the scene with multiplied steps.

---

## Tunnel Overlay (VRTK_TunnelOverlay)

### Overview

Applys a tunnel overlay effect to the active VR camera when the play area is moving or rotating to reduce potential nausea caused by simulation sickness.

**Script Usage:**
 * Place the `VRTK_TunnelOverlay` script on any active scene GameObject.

  > This implementation is based on a project made by SixWays at https://github.com/SixWays/UnityVrTunnelling

### Inspector Parameters

 * **Minimum Rotation:** Minimum rotation speed for the effect to activate (degrees per second).
 * **Maximum Rotation:** Maximum rotation speed for the effect have its max settings applied (degrees per second).
 * **Minimum Speed:** Minimum movement speed for the effect to activate.
 * **Maximum Speed:** Maximum movement speed where the effect will have its max settings applied.
 * **Effect Color:** The color to use for the tunnel effect.
 * **Effect Skybox:** An optional skybox texture to use for the tunnel effect.
 * **Initial Effect Size:** The initial amount of screen coverage the tunnel to consume without any movement.
 * **Maximum Effect Size:** Screen coverage at the maximum tracked values.
 * **Feather Size:** Feather effect size around the cut-off as fraction of screen.
 * **Smoothing Time:** Smooth out radius over time.

---

## Drag World (VRTK_DragWorld)

### Overview

Provides the ability to move, rotate and scale the PlayArea by dragging the world with the controllers.

**Script Usage:**
 * Place the `VRTK_DragWorld` script on any active scene GameObject.

  > If only one controller is being used to track the rotation mechanism, then the rotation will be based on the perpendicual (yaw) axis angular velocity of the tracking controller.
  > If both controllers are being used to track the rotation mechanism, then the rotation will be based on pushing one controller forward, whilst pulling the other controller backwards.

### Inspector Parameters

 * **Movement Activation Button:** The controller button to press to activate the movement mechanism.
 * **Movement Activation Requirement:** The controller(s) on which the activation button is to be pressed to consider the movement mechanism active.
 * **Movement Tracking Controller:** The controller(s) on which to track position of to determine if a valid move has taken place.
 * **Movement Multiplier:** The amount to multply the movement by.
 * **Movement Position Lock:** The axes to lock to prevent movement across.
 * **Rotation Activation Button:** The controller button to press to activate the rotation mechanism.
 * **Rotation Activation Requirement:** The controller(s) on which the activation button is to be pressed to consider the rotation mechanism active.
 * **Rotation Tracking Controller:** The controller(s) on which to determine how rotation should occur. `BothControllers` requires both controllers to be pushed/pulled to rotate, whereas any other setting will base rotation on the rotation of the activating controller.
 * **Rotation Multiplier:** The amount to multply the rotation angle by.
 * **Rotation Activation Threshold:** The threshold the rotation angle has to be above to consider a valid rotation amount.
 * **Scale Activation Button:** The controller button to press to activate the scale mechanism.
 * **Scale Activation Requirement:** The controller(s) on which the activation button is to be pressed to consider the scale mechanism active.
 * **Scale Tracking Controller:** The controller(s) on which to determine how scaling should occur.
 * **Scale Multiplier:** The amount to multply the scale factor by.
 * **Scale Activation Threshold:** The threshold the distance between the scale objects has to be above to consider a valid scale operation.
 * **Minimum Scale:** the minimum scale amount that can be applied.
 * **Maximum Scale:** the maximum scale amount that can be applied.
 * **Controlling Transform:** The transform to apply the control mechanisms to. If this is left blank then the PlayArea will be controlled.
 * **Use Offset Transform:** Uses the specified `Offset Transform` when dealing with rotational offsets.
 * **Offset Transform:** The transform to use when dealing with rotational offsets. If this is left blank then the Headset will be used as the offset.

### Class Variables

 * `public enum ActivationRequirement` - The controller on which to determine as the activation requirement for the control mechanism.
   * `LeftControllerOnly` - Only pressing the activation button on the left controller will activate the mechanism, if the right button is held down then the mechanism will not be activated.
   * `RightControllerOnly` - Only pressing the activation button on the right controller will activate the mechanism, if the left button is held down then the mechanism will not be activated.
   * `LeftController` - Pressing the activation button on the left controller is all that is required to activate the mechanism.
   * `RightController` - Pressing the activation button on the right controller is all that is required to activate the mechanism.
   * `EitherController` - Pressing the activation button on the either controller is all that is required to activate the mechanism.
   * `BothControllers` - Pressing the activation button on both controllers is required to activate the mechanism.
 * `public enum TrackingController` - The controllers which to track when performing the mechanism.
   * `LeftController` - Only track the left controller.
   * `RightController` - Only track the right controller.
   * `EitherController` - Track either the left or the right controller.
   * `BothControllers` - Only track both controllers at the same time.

---

# Object Control Actions (VRTK/Source/Scripts/Locomotion/ObjectControlActions)

A collection of scripts that are used to provide different actions when using Object Control.

 * [Base Object Control Action](#base-object-control-action-vrtk_baseobjectcontrolaction)
 * [Slide Object Control Action](#slide-object-control-action-vrtk_slideobjectcontrolaction)
 * [Rotate Object Control Action](#rotate-object-control-action-vrtk_rotateobjectcontrolaction)
 * [Snap Rotate Object Control Action](#snap-rotate-object-control-action-vrtk_snaprotateobjectcontrolaction)
 * [Warp Object Control Action](#warp-object-control-action-vrtk_warpobjectcontrolaction)

---

## Base Object Control Action (VRTK_BaseObjectControlAction)

### Overview

Provides a base that all object control actions can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides object control action functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Object Control Script:** The Object Control script to receive axis change events from.
 * **Listen On Axis Change:** Determines which Object Control Axis event to listen to.

### Class Variables

 * `public enum AxisListeners` - The axis to listen to changes on.
   * `XAxisChanged` - Listen for changes on the horizontal X axis.
   * `YAxisChanged` - Listen for changes on the vertical y axis.

---

## Slide Object Control Action (VRTK_SlideObjectControlAction)
 > extends [VRTK_BaseObjectControlAction](#base-object-control-action-vrtk_baseobjectcontrolaction)

### Overview

Provides the ability to move a GameObject around by sliding it across the `x/z` plane in the scene by updating the Transform position when the corresponding Object Control axis changes.

  > The effect is a smooth sliding motion in forward and sideways directions to simulate walking.

**Required Components:**
 * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.

**Optional Components:**
 * `VRTK_BodyPhysics` - The Body Physics script to utilise when checking for potential collisions on movement.

**Script Usage:**
 * Place the `VRTK_SlideObjectControlAction` script on any active scene GameObject.
 * Link the required Object Control script to the `Object Control Script` parameter of this script.
 * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.

### Inspector Parameters

 * **Maximum Speed:** The maximum speed the controlled object can be moved in based on the position of the axis.
 * **Deceleration:** The rate of speed deceleration when the axis is no longer being changed.
 * **Falling Deceleration:** The rate of speed deceleration when the axis is no longer being changed and the object is falling.
 * **Speed Multiplier:** The speed multiplier to be applied when the modifier button is pressed.
 * **Body Physics:** An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Slide Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Slide` control script active.

---

## Rotate Object Control Action (VRTK_RotateObjectControlAction)
 > extends [VRTK_BaseObjectControlAction](#base-object-control-action-vrtk_baseobjectcontrolaction)

### Overview

Provides the ability to rotate a GameObject through the world `y` axis in the scene by updating the Transform rotation when the corresponding Object Control axis changes.

  > The effect is a smooth rotation to simulate turning.

**Required Components:**
 * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.

**Script Usage:**
 * Place the `VRTK_RotateObjectControlAction` script on any active scene GameObject.
 * Link the required Object Control script to the `Object Control Script` parameter of this script.
 * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.

### Inspector Parameters

 * **Maximum Rotation Speed:** The maximum speed the controlled object can be rotated based on the position of the axis.
 * **Rotation Multiplier:** The rotation multiplier to be applied when the modifier button is pressed.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Rotate Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Rotate` control script active.

---

## Snap Rotate Object Control Action (VRTK_SnapRotateObjectControlAction)
 > extends [VRTK_BaseObjectControlAction](#base-object-control-action-vrtk_baseobjectcontrolaction)

### Overview

Provides the ability to snap rotate a GameObject through the world `y` axis in the scene by updating the Transform rotation in defined steps when the corresponding Object Control axis changes.

  > The effect is a immediate snap rotation to quickly face in a new direction.

**Required Components:**
 * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.

**Script Usage:**
 * Place the `VRTK_SnapRotateObjectControlAction` script on any active scene GameObject.
 * Link the required Object Control script to the `Object Control Script` parameter of this script.
 * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.

### Inspector Parameters

 * **Angle Per Snap:** The angle to rotate for each snap.
 * **Angle Multiplier:** The snap angle multiplier to be applied when the modifier button is pressed.
 * **Snap Delay:** The amount of time required to pass before another snap rotation can be carried out.
 * **Blink Transition Speed:** The speed for the headset to fade out and back in. Having a blink between rotations can reduce nausia.
 * **Axis Threshold:** The threshold the listened axis needs to exceed before the action occurs. This can be used to limit the snap rotate to a single axis direction (e.g. pull down to flip rotate). The threshold is ignored if it is 0.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Snap Rotate Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Snap Rotate` control script active.

---

## Warp Object Control Action (VRTK_WarpObjectControlAction)
 > extends [VRTK_BaseObjectControlAction](#base-object-control-action-vrtk_baseobjectcontrolaction)

### Overview

Provides the ability to move a GameObject around by warping it across the `x/z` plane in the scene by updating the Transform position in defined steps when the corresponding Object Control axis changes.

  > The effect is a immediate snap to a new position in the given direction.

**Required Components:**
 * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.

**Optional Components:**
 * `VRTK_BodyPhysics` - The Body Physics script to utilise when checking for potential collisions on movement.

**Script Usage:**
 * Place the `VRTK_WarpObjectControlAction` script on any active scene GameObject.
 * Link the required Object Control script to the `Object Control Script` parameter of this script.
 * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.

### Inspector Parameters

 * **Warp Distance:** The distance to warp in the facing direction.
 * **Warp Multiplier:** The multiplier to be applied to the warp when the modifier button is pressed.
 * **Warp Delay:** The amount of time required to pass before another warp can be carried out.
 * **Floor Height Tolerance:** The height different in floor allowed to be a valid warp.
 * **Blink Transition Speed:** The speed for the headset to fade out and back in. Having a blink between warps can reduce nausia.
 * **Body Physics:** An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.

To enable the Warp Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Warp` control script active.

---

# Highlighters (VRTK/Source/Scripts/Interactions/Highlighters)

A collection of scripts that are used to provide highlighting.

 * [Base Highlighter](#base-highlighter-vrtk_basehighlighter)
 * [Material Colour Swap](#material-colour-swap-vrtk_materialcolorswaphighlighter)
 * [Material Property Block Colour Swap](#material-property-block-colour-swap-vrtk_materialpropertyblockcolorswaphighlighter)
 * [Outline Object Copy](#outline-object-copy-vrtk_outlineobjectcopyhighlighter)

---

## Base Highlighter (VRTK_BaseHighlighter)

### Overview

Provides a base that all highlighters can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides highlight functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Active:** Determines if this highlighter is the active highlighter for the object the component is attached to. Only one active highlighter can be applied to a GameObject.
 * **Unhighlight On Disable:** Determines if the highlighted object should be unhighlighted when it is disabled.

### Class Methods

#### Initialise/3

  > `public abstract void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null);`

 * Parameters
   * `Color? color` - An optional colour may be passed through at point of initialisation in case the highlighter requires it.
   * `GameObject affectObject` - An optional GameObject to specify which object to apply the highlighting to.
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
   * `bool` - Returns `true` if the highlighter creates a cloned object to apply the highlighter on, returns `false` if no additional object is created.

The UsesClonedObject method is used to return whether the current highlighter creates a cloned object to do the highlighting with.

#### GetActiveHighlighter/1

  > `public static VRTK_BaseHighlighter GetActiveHighlighter(GameObject obj)`

 * Parameters
   * `GameObject obj` - The GameObject to check for a highlighter on.
 * Returns
   * `VRTK_BaseHighlighter` - A valid and active highlighter.

The GetActiveHighlighter method checks the given GameObject for a valid and active highlighter.

---

## Material Colour Swap (VRTK_MaterialColorSwapHighlighter)
 > extends [VRTK_BaseHighlighter](#base-highlighter-vrtk_basehighlighter)

### Overview

Swaps the texture colour on the Renderers material for the given highlight colour.

  > Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted. The Draw Call Batching will resume on the original material when the item is no longer highlighted.

**Script Usage:**
 * Place the `VRTK_MaterialColorSwapHighlighter` script on either:
   * The GameObject of the Interactable Object to highlight.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Object Highlighter` parameter to denote use of the highlighter.
 * Ensure the `Active` parameter is checked.

### Inspector Parameters

 * **Emission Darken:** The emission colour of the texture will be the highlight colour but this percent darker.
 * **Custom Material:** A custom material to use on the highlighted object.

### Class Methods

#### Initialise/3

  > `public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)`

 * Parameters
   * `Color? color` - Not used.
   * `GameObject affectObject` - An optional GameObject to specify which object to apply the highlighting to.
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

Swaps the texture colour on the Renderers material for the given highlight colour using property blocks.

  > Utilising the MaterialPropertyBlock means that Draw Call Batching in Unity is not compromised.

**Script Usage:**
 * Place the `VRTK_MaterialPropertyBlockColorSwapHighlighter` script on either:
   * The GameObject of the Interactable Object to highlight.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Object Highlighter` parameter to denote use of the highlighter.
 * Ensure the `Active` parameter is checked.

### Class Methods

#### Initialise/3

  > `public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)`

 * Parameters
   * `Color? color` - Not used.
   * `GameObject affectObject` - An optional GameObject to specify which object to apply the highlighting to.
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

Creates a mesh copy and applies an outline shader which is toggled on and off when highlighting the object.

  > A valid mesh must be found or provided for the clone mesh to be created.

**Script Usage:**
 * Place the `VRTK_OutlineObjectCopyHighlighter` script on either:
   * The GameObject of the Interactable Object to highlight.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Object Highlighter` parameter to denote use of the highlighter.
 * Ensure the `Active` parameter is checked.

### Inspector Parameters

 * **Thickness:** The thickness of the outline effect
 * **Custom Outline Models:** The GameObjects to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.
 * **Custom Outline Model Paths:** A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.
 * **Enable Submesh Highlight:** If the mesh has multiple sub-meshes to highlight then this should be checked, otherwise only the first mesh will be highlighted.

### Class Methods

#### Initialise/3

  > `public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)`

 * Parameters
   * `Color? color` - Not used.
   * `GameObject affectObject` - An optional GameObject to specify which object to apply the highlighting to.
   * `Dictionary<string, object> options` - A dictionary array containing the highlighter options:
     * `<'thickness', float>` - Same as `thickness` inspector parameter.
     * `<'customOutlineModels', GameObject[]>` - Same as `customOutlineModels` inspector parameter.
     * `<'customOutlineModelPaths', string[]>` - Same as `customOutlineModelPaths` inspector parameter.
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

# Interactors (VRTK/Source/Scripts/Interactions/Interactors)

A collection of scripts that provide the ability denote objects (such as controllers) as being able to interact with interactable objects.

 * [Controller Events](#controller-events-vrtk_controllerevents)
 * [Interact Touch](#interact-touch-vrtk_interacttouch)
 * [Interact Near Touch](#interact-near-touch-vrtk_interactneartouch)
 * [Interact Grab](#interact-grab-vrtk_interactgrab)
 * [Interact Use](#interact-use-vrtk_interactuse)
 * [Controller Tracked Collider](#controller-tracked-collider-vrtk_controllertrackedcollider)
 * [Controller Highlighter](#controller-highlighter-vrtk_controllerhighlighter)
 * [Object Auto Grab](#object-auto-grab-vrtk_objectautograb)

---

## Controller Events (VRTK_ControllerEvents)

### Overview

A relationship to a physical VR controller and emits events based on the inputs of the controller.

**Script Usage:**
 * Place the `VRTK_ControllerEvents` script on the controller script alias GameObject of the controller to track (e.g. Right Controller Script Alias).

### Inspector Parameters

 * **Axis Fidelity:** The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.
 * **Sense Axis Force Zero Threshold:** The level on a sense axis to reach before the sense axis is forced to 0f
 * **Sense Axis Press Threshold:** The amount of pressure required to be applied to a sense button before considering the sense button pressed.
 * **Trigger Click Threshold:** The level on the trigger axis to reach before a click is registered.
 * **Trigger Force Zero Threshold:** The level on the trigger axis to reach before the axis is forced to 0f.
 * **Trigger Axis Zero On Untouch:** If this is checked then the trigger axis will be forced to 0f when the trigger button reports an untouch event.
 * **Grip Click Threshold:** The level on the grip axis to reach before a click is registered.
 * **Grip Force Zero Threshold:** The level on the grip axis to reach before the axis is forced to 0f.
 * **Grip Axis Zero On Untouch:** If this is checked then the grip axis will be forced to 0f when the grip button reports an untouch event.

### Class Variables

 * `public enum ButtonAlias` - Button types
   * `Undefined` - No button specified.
   * `TriggerHairline` - The trigger is squeezed past the current hairline threshold.
   * `TriggerTouch` - The trigger is squeezed a small amount.
   * `TriggerPress` - The trigger is squeezed about half way in.
   * `TriggerClick` - The trigger is squeezed all the way down.
   * `GripHairline` - The grip is squeezed past the current hairline threshold.
   * `GripTouch` - The grip button is touched.
   * `GripPress` - The grip button is pressed.
   * `GripClick` - The grip button is pressed all the way down.
   * `TouchpadTouch` - The touchpad is touched (without pressing down to click).
   * `TouchpadPress` - The touchpad is pressed (to the point of hearing a click).
   * `TouchpadTwoTouch` - The touchpad two is touched (without pressing down to click).
   * `ButtonOneTouch` - The button one is touched.
   * `ButtonOnePress` - The button one is pressed.
   * `ButtonTwoTouch` - The button two is touched.
   * `ButtonTwoPress` - The button two is pressed.
   * `StartMenuPress` - The start menu is pressed.
   * `TouchpadSense` - The touchpad sense touch is active.
   * `TriggerSense` - The trigger sense touch is active.
   * `MiddleFingerSense` - The middle finger sense touch is active.
   * `RingFingerSense` - The ring finger sense touch is active.
   * `PinkyFingerSense` - The pinky finger sense touch is active.
 * `public enum Vector2AxisAlias` - Vector2 Axis Types.
   * `Undefined` - No axis specified.
   * `Touchpad` - Touchpad on the controller.
   * `TouchpadTwo` - Touchpad Two on the controller.
 * `public enum AxisType` - Axis Types
   * `Digital` - A digital axis with a binary result of 0f not pressed or 1f is pressed.
   * `Axis` - An analog axis ranging from no squeeze at 0f to full squeeze at 1f.
   * `SenseAxis` - A cap sens axis ranging from not near at 0f to touching at 1f.
 * `public bool triggerPressed` - This will be true if the trigger is squeezed about half way in. Default: `false`
 * `public bool triggerTouched` - This will be true if the trigger is squeezed a small amount. Default: `false`
 * `public bool triggerHairlinePressed` - This will be true if the trigger is squeezed a small amount more from any previous squeeze on the trigger. Default: `false`
 * `public bool triggerClicked` - This will be true if the trigger is squeezed all the way down. Default: `false`
 * `public bool triggerAxisChanged` - This will be true if the trigger has been squeezed more or less. Default: `false`
 * `public bool triggerSenseAxisChanged` - This will be true if the trigger sense is being touched more or less. Default: `false`
 * `public bool gripPressed` - This will be true if the grip is squeezed about half way in. Default: `false`
 * `public bool gripTouched` - This will be true if the grip is touched. Default: `false`
 * `public bool gripHairlinePressed` - This will be true if the grip is squeezed a small amount more from any previous squeeze on the grip. Default: `false`
 * `public bool gripClicked` - This will be true if the grip is squeezed all the way down. Default: `false`
 * `public bool gripAxisChanged` - This will be true if the grip has been squeezed more or less. Default: `false`
 * `public bool touchpadPressed` - This will be true if the touchpad is held down. Default: `false`
 * `public bool touchpadTouched` - This will be true if the touchpad is being touched. Default: `false`
 * `public bool touchpadAxisChanged` - This will be true if the touchpad position has changed. Default: `false`
 * `public bool touchpadSenseAxisChanged` - This will be true if the touchpad sense is being touched more or less. Default: `false`
 * `public bool touchpadTwoTouched` - This will be true if the touchpad two is being touched. Default: `false`
 * `public bool buttonOnePressed` - This will be true if button one is held down. Default: `false`
 * `public bool buttonOneTouched` - This will be true if button one is being touched. Default: `false`
 * `public bool buttonTwoPressed` - This will be true if button two is held down. Default: `false`
 * `public bool buttonTwoTouched` - This will be true if button two is being touched. Default: `false`
 * `public bool startMenuPressed` - This will be true if start menu is held down. Default: `false`
 * `public bool middleFingerSenseAxisChanged` - This will be true if the middle finger sense is being touched more or less. Default: `false`
 * `public bool ringFingerSenseAxisChanged` - This will be true if the ring finger sense is being touched more or less. Default: `false`
 * `public bool pinkyFingerSenseAxisChanged` - This will be true if the pinky finger sense is being touched more or less. Default: `false`
 * `public bool controllerVisible` - This will be true if the controller model alias renderers are visible. Default: `true`

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
 * `TriggerSenseAxisChanged` - Emitted when the amount of touch on the trigger sense changes.
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
 * `TouchpadSenseAxisChanged` - Emitted when the amount of touch on the touchpad sense changes.
 * `TouchpadTwoTouchStart` - Emitted when the touchpad two is touched (without pressing down to click).
 * `TouchpadTwoTouchEnd` - Emitted when the touchpad two is no longer being touched.
 * `TouchpadTwoAxisChanged` - Emitted when the touchpad two is being touched in a different location.
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
 * `MiddleFingerSenseAxisChanged` - Emitted when the amount of touch on the middle finger sense changes.
 * `RingFingerSenseAxisChanged` - Emitted when the amount of touch on the ring finger sense changes.
 * `PinkyFingerSenseAxisChanged` - Emitted when the amount of touch on the pinky finger sense changes.
 * `ControllerEnabled` - Emitted when the controller is enabled.
 * `ControllerDisabled` - Emitted when the controller is disabled.
 * `ControllerIndexChanged` - Emitted when the controller index changed.
 * `ControllerModelAvailable` - Emitted when the controller model becomes available.
 * `ControllerVisible` - Emitted when the controller is set to visible.
 * `ControllerHidden` - Emitted when the controller is set to hidden.

### Unity Events

Adding the `VRTK_ControllerEvents_UnityEvents` component to `VRTK_ControllerEvents` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerReference controllerReference` - The reference for the controller that initiated the event.
 * `float buttonPressure` - The amount of pressure being applied to the button pressed. `0f` to `1f`.
 * `Vector2 touchpadAxis` - The position the touchpad is touched at. `(0,0)` to `(1,1)`.
 * `float touchpadAngle` - The rotational position the touchpad is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.
 * `Vector2 touchpadTwoAxis` - The position the touchpad two is touched at. `(0,0)` to `(1,1)`.
 * `float touchpadTwoAngle` - The rotational position the touchpad two is being touched at, 0 being top, 180 being bottom and all other angles accordingly. `0f` to `360f`.

### Class Methods

#### SetControllerEvent/0

  > `public virtual ControllerInteractionEventArgs SetControllerEvent()`

 * Parameters
   * _none_
 * Returns
   * `ControllerInteractionEventArgs` - The payload for a Controller Event.

The SetControllerEvent/0 method is used to set the Controller Event payload.

#### SetControllerEvent/3

  > `public virtual ControllerInteractionEventArgs SetControllerEvent(ref bool buttonBool, bool value = false, float buttonPressure = 0f)`

 * Parameters
   * `ref bool buttonBool` - The state of the pressed button if required.
   * `bool value` - The value to set the `buttonBool` reference to.
   * `float buttonPressure` - The pressure of the button pressed if required.
 * Returns
   * `ControllerInteractionEventArgs` - The payload for a Controller Event.

The SetControllerEvent/3 method is used to set the Controller Event payload.

#### GetControllerType/0

  > `public virtual SDK_BaseController.ControllerType GetControllerType()`

 * Parameters
   * _none_
 * Returns
   * `SDK_BaseController.ControllerType` - The type of controller that the Controller Events is attached to.

The GetControllerType method is a shortcut to retrieve the current controller type the Controller Events is attached to.

#### GetAxis/1

  > `public virtual Vector2 GetAxis(Vector2AxisAlias vector2AxisType)`

 * Parameters
   * `Vector2AxisAlias vector2AxisType` - The Vector2AxisType to check the touch position of.
 * Returns
   * `Vector2` - A two dimensional vector containing the `x` and `y` position of where the given axis type is being touched. `(0,0)` to `(1,1)`.

The GetAxis method returns the coordinates of where the given axis type is being touched.

#### GetTouchpadAxis/0

  > `public virtual Vector2 GetTouchpadAxis()`

 * Parameters
   * _none_
 * Returns
   * `Vector2` - A two dimensional vector containing the `x` and `y` position of where the touchpad is being touched. `(0,0)` to `(1,1)`.

The GetTouchpadAxis method returns the coordinates of where the touchpad is being touched and can be used for directional input via the touchpad. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.

#### GetTouchpadTwoAxis/0

  > `public virtual Vector2 GetTouchpadTwoAxis()`

 * Parameters
   * _none_
 * Returns
   * `Vector2` - A two dimensional vector containing the `x` and `y` position of where the touchpad two is being touched. `(0,0)` to `(1,1)`.

The GetTouchpadTwoAxis method returns the coordinates of where the touchpad two is being touched and can be used for directional input via the touchpad two. The `x` value is the horizontal touch plane and the `y` value is the vertical touch plane.

#### GetAxisAngle/1

  > `public virtual float GetAxisAngle(Vector2AxisAlias vector2AxisType)`

 * Parameters
   * `Vector2AxisAlias vector2AxisType` - The Vector2AxisType to get the touch angle for.
 * Returns
   * `float` - A float representing the angle of where the given axis type is being touched. `0f` to `360f`.

The GetAxisAngle method returns the angle of where the given axis type is currently being touched with the top of the given axis type being `0` degrees and the bottom of the given axis type being `180` degrees.

#### GetTouchpadAxisAngle/0

  > `public virtual float GetTouchpadAxisAngle()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the angle of where the touchpad is being touched. `0f` to `360f`.

The GetTouchpadAxisAngle method returns the angle of where the touchpad is currently being touched with the top of the touchpad being `0` degrees and the bottom of the touchpad being `180` degrees.

#### GetTouchpadTwoAxisAngle/0

  > `public virtual float GetTouchpadTwoAxisAngle()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the angle of where the touchpad two is being touched. `0f` to `360f`.

The GetTouchpadTwoAxisAngle method returns the angle of where the touchpad two is currently being touched with the top of the touchpad two being `0` degrees and the bottom of the touchpad two being `180` degrees.

#### GetTriggerAxis/0

  > `public virtual float GetTriggerAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the amount of squeeze that is being applied to the trigger. `0f` to `1f`.

The GetTriggerAxis method returns a float that represents how much the trigger is being squeezed. This can be useful for using the trigger axis to perform high fidelity tasks or only activating the trigger press once it has exceeded a given press threshold.

#### GetGripAxis/0

  > `public virtual float GetGripAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the amount of squeeze that is being applied to the grip. `0f` to `1f`.

The GetGripAxis method returns a float that represents how much the grip is being squeezed. This can be useful for using the grip axis to perform high fidelity tasks or only activating the grip press once it has exceeded a given press threshold.

#### GetHairTriggerDelta/0

  > `public virtual float GetHairTriggerDelta()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the difference in the trigger pressure from the hairline threshold start to current position.

The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.

#### GetHairGripDelta/0

  > `public virtual float GetHairGripDelta()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing the difference in the trigger pressure from the hairline threshold start to current position.

The GetHairTriggerDelta method returns a float representing the difference in how much the trigger is being pressed in relation to the hairline threshold start.

#### GetTouchpadSenseAxis/0

  > `public virtual float GetTouchpadSenseAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing how much the touch sensor is being touched.

The GetTouchpadSenseAxis method returns a float representing how much of the touch sensor is being touched.

#### GetTriggerSenseAxis/0

  > `public virtual float GetTriggerSenseAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing how much the touch sensor is being touched.

The GetTriggerSenseAxis method returns a float representing how much of the touch sensor is being touched.

#### GetMiddleFingerSenseAxis/0

  > `public virtual float GetMiddleFingerSenseAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing how much the touch sensor is being touched.

The GetMiddleFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.

#### GetRingFingerSenseAxis/0

  > `public virtual float GetRingFingerSenseAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing how much the touch sensor is being touched.

The GetRingFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.

#### GetPinkyFingerSenseAxis/0

  > `public virtual float GetPinkyFingerSenseAxis()`

 * Parameters
   * _none_
 * Returns
   * `float` - A float representing how much the touch sensor is being touched.

The GetPinkyFingerSenseAxis method returns a float representing how much of the touch sensor is being touched.

#### AnyButtonPressed/0

  > `public virtual bool AnyButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if any of the controller buttons are currently being pressed.

The AnyButtonPressed method returns true if any of the controller buttons are being pressed and this can be useful to determine if an action can be taken whilst the user is using the controller.

#### GetAxisState/2

  > `public virtual bool GetAxisState(Vector2AxisAlias axis, SDK_BaseController.ButtonPressTypes pressType)`

 * Parameters
   * `Vector2AxisAlias axis` - The axis to check on.
   * `SDK_BaseController.ButtonPressTypes pressType` - The button press type to check for.
 * Returns
   * `bool` - Returns `true` if the axis is being interacted with via the given press type.

The GetAxisState method takes a given Vector2Axis and returns a boolean whether that given axis is currently being touched or pressed.

#### IsButtonPressed/1

  > `public virtual bool IsButtonPressed(ButtonAlias button)`

 * Parameters
   * `ButtonAlias button` - The button to check if it's being pressed.
 * Returns
   * `bool` - Returns `true` if the button is being pressed.

The IsButtonPressed method takes a given button alias and returns a boolean whether that given button is currently being pressed or not.

#### SubscribeToButtonAliasEvent/3

  > `public virtual void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)`

 * Parameters
   * `ButtonAlias givenButton` - The Button Alias to register the event on.
   * `bool startEvent` - If this is `true` then the start event related to the button is used (e.g. `OnPress`). If this is `false` then the end event related to the button is used (e.g. `OnRelease`).
   * `ControllerInteractionEventHandler callbackMethod` - The method to subscribe to the event.
 * Returns
   * _none_

The SubscribeToButtonAliasEvent method makes it easier to subscribe to a button event on either the start or end action. Upon the event firing, the given callback method is executed.

#### UnsubscribeToButtonAliasEvent/3

  > `public virtual void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)`

 * Parameters
   * `ButtonAlias givenButton` - The Button Alias to unregister the event on.
   * `bool startEvent` - If this is `true` then the start event related to the button is used (e.g. `OnPress`). If this is `false` then the end event related to the button is used (e.g. `OnRelease`).
   * `ControllerInteractionEventHandler callbackMethod` - The method to unsubscribe from the event.
 * Returns
   * _none_

The UnsubscribeToButtonAliasEvent method makes it easier to unsubscribe to from button event on either the start or end action.

#### SubscribeToAxisAliasEvent/3

  > `public virtual void SubscribeToAxisAliasEvent(SDK_BaseController.ButtonTypes buttonType, AxisType axisType, ControllerInteractionEventHandler callbackMethod)`

 * Parameters
   * `SDK_BaseController.ButtonTypes buttonType` - The button to listen for axis changes on.
   * `AxisType axisType` - The type of axis change to listen for.
   * `ControllerInteractionEventHandler callbackMethod` - The method to subscribe to the event.
 * Returns
   * _none_

The SubscribeToAxisAliasEvent method makes it easier to subscribe to axis changes on a given button for a given axis type.

#### UnsubscribeToAxisAliasEvent/3

  > `public virtual void UnsubscribeToAxisAliasEvent(SDK_BaseController.ButtonTypes buttonType, AxisType axisType, ControllerInteractionEventHandler callbackMethod)`

 * Parameters
   * `SDK_BaseController.ButtonTypes buttonType` - The button to unregister for axis changes on.
   * `AxisType axisType` - The type of axis change to unregister on.
   * `ControllerInteractionEventHandler callbackMethod` - The method to unsubscribe from the event.
 * Returns
   * _none_

The UnsubscribeToAxisAliasEvent method makes it easier to unsubscribe from axis changes on a given button for a given axis type.

### Example

`VRTK/Examples/002_Controller_Events` shows how the events are utilised and listened to. The accompanying example script can be viewed in `VRTK/Examples/ExampleResources/Scripts/VRTK_ControllerEvents_ListenerExample.cs`.

---

## Interact Touch (VRTK_InteractTouch)

### Overview

Determines if a GameObject can initiate a touch with an Interactable Object.

**Required Components:**
 * `Rigidbody` - A Unity kinematic Rigidbody to determine when collisions happen between the Interact Touch GameObject and other valid colliders.

**Script Usage:**
 * Place the `VRTK_InteractTouch` script on the controller script alias GameObject of the controller to track (e.g. Right Controller Script Alias).

### Inspector Parameters

 * **Custom Collider Container:** An optional GameObject that contains the compound colliders to represent the touching object. If this is empty then the collider will be auto generated at runtime to match the SDK default controller.

### Class Events

 * `ControllerStartTouchInteractableObject` - Emitted when the touch of a valid object has started.
 * `ControllerTouchInteractableObject` - Emitted when a valid object is touched.
 * `ControllerStartUntouchInteractableObject` - Emitted when the untouch of a valid object has started.
 * `ControllerUntouchInteractableObject` - Emitted when a valid object is no longer being touched.
 * `ControllerRigidbodyActivated` - Emitted when the controller rigidbody is activated.
 * `ControllerRigidbodyDeactivated` - Emitted when the controller rigidbody is deactivated.

### Unity Events

Adding the `VRTK_InteractTouch_UnityEvents` component to `VRTK_InteractTouch` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerReference controllerReference` - The reference to the controller doing the interaction.
 * `GameObject target` - The GameObject of the Interactable Object that is being interacted with.

### Class Methods

#### ForceTouch/1

  > `public virtual void ForceTouch(GameObject obj)`

 * Parameters
   * `GameObject obj` - The GameObject to attempt to force touch.
 * Returns
   * _none_

The ForceTouch method will attempt to force the Interact Touch onto the given GameObject.

#### GetTouchedObject/0

  > `public virtual GameObject GetTouchedObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject of what is currently being touched by this Interact Touch.

The GetTouchedObject method returns the current GameObject being touched by the Interact Touch.

#### IsObjectInteractable/1

  > `public virtual bool IsObjectInteractable(GameObject obj)`

 * Parameters
   * `GameObject obj` - The GameObject to check to see if it's a valid Interactable Object.
 * Returns
   * `bool` - Returns `true` if the given GameObjectis a valid Interactable Object.

The IsObjectInteractable method is used to check if a given GameObject is a valid Interactable Object.

#### ToggleControllerRigidBody/2

  > `public virtual void ToggleControllerRigidBody(bool state, bool forceToggle = false)`

 * Parameters
   * `bool state` - The state of whether the rigidbody is on or off. `true` toggles the rigidbody on and `false` turns it off.
   * `bool forceToggle` - Determines if the rigidbody has been forced into it's new state by another script. This can be used to override other non-force settings. Defaults to `false`
 * Returns
   * _none_

The ToggleControllerRigidBody method toggles the Interact Touch rigidbody's ability to detect collisions. If it is true then the controller rigidbody will collide with other collidable GameObjects.

#### IsRigidBodyActive/0

  > `public virtual bool IsRigidBodyActive()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the rigidbody on the Interact Touch is currently active and able to affect other scene rigidbodies.

The IsRigidBodyActive method checks to see if the rigidbody on the Interact Touch is active and can affect other rigidbodies in the scene.

#### IsRigidBodyForcedActive/0

  > `public virtual bool IsRigidBodyForcedActive()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the rigidbody is active and has been forced into the active state.

The IsRigidBodyForcedActive method checks to see if the rigidbody on the Interact Touch has been forced into the active state.

#### ForceStopTouching/0

  > `public virtual void ForceStopTouching()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceStopTouching method will stop the Interact Touch from touching an Interactable Object even if the Interact Touch is physically touching the Interactable Object.

#### ControllerColliders/0

  > `public virtual Collider[] ControllerColliders()`

 * Parameters
   * _none_
 * Returns
   * `Collider[]` - An array of colliders that are associated with the controller.

The ControllerColliders method retrieves all of the associated colliders on the Interact Touch.

#### GetControllerType/0

  > `public virtual SDK_BaseController.ControllerType GetControllerType()`

 * Parameters
   * _none_
 * Returns
   * `SDK_BaseController.ControllerType` - The type of controller that the Interact Touch is attached to.

The GetControllerType method is a shortcut to retrieve the current controller type the Interact Touch is attached to.

### Example

`VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the highlighting of objects that have the `VRTK_InteractableObject` script added to them to show the ability to highlight interactable objects when they are touched by the controllers.

---

## Interact Near Touch (VRTK_InteractNearTouch)

### Overview

Determines if a GameObject can initiate a near touch with an Interactable Object.

**Required Components:**
 * `VRTK_InteractTouch` - The touch component to determine the actual interacting GameObject that will deal with the near touch interaction. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.

**Script Usage:**
 * Place the `VRTK_InteractNearTouch` script on either:
   * The Interact Touch GameObject.
   * Any other scene GameObject and provide a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter of this script.

### Inspector Parameters

 * **Collider Radius:** The radius of the auto generated collider if a `Custom Collider Container` is not supplied.
 * **Custom Collider Container:** An optional GameObject that contains the compound colliders to represent the near touching object. If this is empty then the collider will be auto generated at runtime.
 * **Interact Touch:** The Interact Touch script to associate the near touches with. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.

### Class Events

 * `ControllerNearTouchInteractableObject` - Emitted when a valid object is near touched.
 * `ControllerNearUntouchInteractableObject` - Emitted when a valid object is no longer being near touched.

### Unity Events

Adding the `VRTK_InteractNearTouch_UnityEvents` component to `VRTK_InteractNearTouch` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### GetNearTouchedObjects/0

  > `public virtual List<GameObject> GetNearTouchedObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<GameObject>` - A list of GameObjects that are being near touched.

The GetNearTouchedObjects method returns all of the GameObjects that are currently being near touched.

#### ForceNearTouch/1

  > `public virtual void ForceNearTouch(GameObject obj)`

 * Parameters
   * `GameObject obj` - The GameObject to attempt to force near touch.
 * Returns
   * _none_

The ForceNearTouch method will attempt to force the controller to near touch the given GameObject.

#### ForceStopNearTouching/1

  > `public virtual void ForceStopNearTouching(GameObject obj = null)`

 * Parameters
   * `GameObject obj` - An optional GameObject to only include in the force stop. If this is null then all near touched GameObjects will be force stopped.
 * Returns
   * _none_

The ForceStopNearTouching method will stop the Interact Touch GameObject from near touching an Interactable Object even if the Interact Touch GameObject is physically touching the Interactable Object still.

#### GetNearTouchedObjects/0

  > `public virtual List<GameObject> GetNearTouchedObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<GameObject>` - A list of GameObjects that are being near touched.

The GetNearTouchedObjects method returns all of the GameObjects that are currently being near touched.

---

## Interact Grab (VRTK_InteractGrab)

### Overview

Determines if the Interact Touch can initiate a grab with the touched Interactable Object.

**Required Components:**
 * `VRTK_InteractTouch` - The touch component to determine when a valid touch has taken place to denote a grab can occur. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller Events` parameter.

**Script Usage:**
 * Place the `VRTK_InteractGrab` script on either:
   * The GameObject with the Interact Touch and Controller Events scripts.
   * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller Events` parameter and a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter of this script.

### Inspector Parameters

 * **Grab Button:** The button used to grab/release a touched Interactable Object.
 * **Grab Precognition:** An amount of time between when the grab button is pressed to when the controller is touching an Interactable Object to grab it.
 * **Throw Multiplier:** An amount to multiply the velocity of any Interactable Object being thrown.
 * **Create Rigid Body When Not Touching:** If this is checked and the Interact Touch is not touching an Interactable Object when the grab button is pressed then a Rigidbody is added to the interacting object to allow it to push other Rigidbody objects around.
 * **Controller Attach Point:** The rigidbody point on the controller model to snap the grabbed Interactable Object to. If blank it will be set to the SDK default.
 * **Controller Events:** The Controller Events to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Interact Touch:** The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.

### Class Events

 * `GrabButtonPressed` - Emitted when the grab button is pressed.
 * `GrabButtonReleased` - Emitted when the grab button is released.
 * `ControllerStartGrabInteractableObject` - Emitted when a grab of a valid object is started.
 * `ControllerGrabInteractableObject` - Emitted when a valid object is grabbed.
 * `ControllerStartUngrabInteractableObject` - Emitted when a ungrab of a valid object is started.
 * `ControllerUngrabInteractableObject` - Emitted when a valid object is released from being grabbed.

### Unity Events

Adding the `VRTK_InteractGrab_UnityEvents` component to `VRTK_InteractGrab` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### IsGrabButtonPressed/0

  > `public virtual bool IsGrabButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the grab alias button is being held down.

The IsGrabButtonPressed method determines whether the current grab alias button is being pressed down.

#### ForceRelease/1

  > `public virtual void ForceRelease(bool applyGrabbingObjectVelocity = false)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If this is true then upon releasing the Interactable Object any velocity on the Interact Touch GameObject will be applied to the Interactable Object to essentiall throw it. Defaults to `false`.
 * Returns
   * _none_

The ForceRelease method will force the Interact Grab to stop grabbing the currently grabbed Interactable Object.

#### AttemptGrab/0

  > `public virtual void AttemptGrab()`

 * Parameters
   * _none_
 * Returns
   * _none_

The AttemptGrab method will attempt to grab the currently touched Interactable Object without needing to press the grab button on the controller.

#### GetGrabbedObject/0

  > `public virtual GameObject GetGrabbedObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The game object of what is currently being grabbed by this controller.

The GetGrabbedObject method returns the current Interactable Object being grabbed by the this Interact Grab.

### Example

`VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the grabbing of interactable objects that have the `VRTK_InteractableObject` script attached to them. The objects can be picked up and thrown around.

`VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` demonstrates that each controller can grab and use objects independently and objects can also be toggled to their use state simultaneously.

`VRTK/Examples/014_Controller_SnappingObjectsOnGrab` demonstrates the different mechanisms for snapping a grabbed object to the controller.

---

## Interact Use (VRTK_InteractUse)

### Overview

Determines if the Interact Touch can initiate a use interaction with the touched Interactable Object.

**Required Components:**
 * `VRTK_InteractTouch` - The touch component to determine when a valid touch has taken place to denote a use interaction can occur. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller Events` parameter.
 * `VRTK_InteractGrab` - The grab component to determine when a valid grab has taken place. This must be applied on the same GameObject as this script if one is not provided via the `Interact Grab` parameter.

**Script Usage:**
 * Place the `VRTK_InteractUse` script on either:
   * The GameObject with the Interact Touch and Controller Events scripts.
   * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller Events` parameter and a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter of this script.

### Inspector Parameters

 * **Use Button:** The button used to use/unuse a touched Interactable Object.
 * **Controller Events:** The Controller Events to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Interact Touch:** The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Interact Grab:** The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.

### Class Events

 * `UseButtonPressed` - Emitted when the use toggle alias button is pressed.
 * `UseButtonReleased` - Emitted when the use toggle alias button is released.
 * `ControllerStartUseInteractableObject` - Emitted when a use of a valid object is started.
 * `ControllerUseInteractableObject` - Emitted when a valid object starts being used.
 * `ControllerStartUnuseInteractableObject` - Emitted when a unuse of a valid object is started.
 * `ControllerUnuseInteractableObject` - Emitted when a valid object stops being used.

### Unity Events

Adding the `VRTK_InteractUse_UnityEvents` component to `VRTK_InteractUse` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### IsUseButtonPressed/0

  > `public virtual bool IsUseButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the use alias button is being held down.

The IsUsebuttonPressed method determines whether the current use alias button is being pressed down.

#### GetUsingObject/0

  > `public virtual GameObject GetUsingObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject of what is currently being used by this Interact Use.

The GetUsingObject method returns the current GameObject being used by the Interact Use.

#### ForceStopUsing/0

  > `public virtual void ForceStopUsing()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceStopUsing method will force the Interact Use to stop using the currently touched Interactable Object and will also stop the Interactable Object's using action.

#### ForceResetUsing/0

  > `public virtual void ForceResetUsing()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceResetUsing will force the Interact Use to stop using the currently touched Interactable Object but the Interactable Object will continue with it's existing using action.

#### AttemptUse/0

  > `public virtual void AttemptUse()`

 * Parameters
   * _none_
 * Returns
   * _none_

The AttemptUse method will attempt to use the currently touched Interactable Object without needing to press the use button on the controller.

### Example

`VRTK/Examples/006_Controller_UsingADoor` simulates using a door object to open and close it. It also has a cube on the floor that can be grabbed to show how interactable objects can be usable or grabbable.

`VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that objects can be grabbed with one button and used with another (e.g. firing a gun).

---

## Controller Tracked Collider (VRTK_ControllerTrackedCollider)
 > extends VRTK_SDKControllerReady

### Overview

Provides a controller collider collection that follows the controller rigidbody via the physics system.

**Required Components:**
 * `VRTK_InteractTouch` - An Interact Touch script to determine which controller rigidbody to follow.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied in the same object hierarchy as the Interact Touch script if one is not provided via the `Controller Events` parameter.

**Script Usage:**
 * Place the `VRTK_ControllerTrackedCollider` script on any active scene GameObject except the Script Alias objects.
 * Assign the controller to track by applying an Interact Touch to the relevant Script Alias and then providing that reference to the `Interact Touch` parameter on this script.

### Inspector Parameters

 * **Interact Touch:** The Interact Touch script to relate the tracked collider to.
 * **Max Resnap Distance:** The maximum distance the collider object can be from the controller before it automatically snaps back to the same position.
 * **Activation Button:** The button to press to activate the colliders on the tracked collider set. If `Undefined` then it will always be active.
 * **Controller Events:** An optional Controller Events to use for listening to the button events. If this is left blank then it will attempt to be retrieved from the same controller as the `Interact Touch` parameter.

### Class Methods

#### ToggleColliders/1

  > `public virtual void ToggleColliders(bool state)`

 * Parameters
   * `bool state` - If `true` then the tracked colliders will be able to affect other Rigidbodies.
 * Returns
   * _none_

The ToggleColliders method toggles the collision state of the tracked colliders.

#### TrackedColliders/0

  > `public virtual Collider[] TrackedColliders()`

 * Parameters
   * _none_
 * Returns
   * `Collider[]` - A Collider array of the tracked colliders.

The TrackedColliders method returns an array of the tracked colliders.

---

## Controller Highlighter (VRTK_ControllerHighlighter)

### Overview

Enables highlighting of controller elements.

**Optional Components:**
 * `VRTK_BaseHighlighter` - The highlighter to use when highligting the controller. If one is not already injected in the `Controller Highlighter` parameter then the component on the same GameObject will be used.

**Script Usage:**
 * Place the `VRTK_ControllerHighlighter` script on either:
   * The controller script alias GameObject of the controller to affect (e.g. Right Controller Script Alias).
   * Any other scene GameObject and provide the controller script alias GameObject to the `Controller Alias` parameter of this script.
 * The Model Element Paths will be auto populated at runtime based on the SDK Setup Model Alias being used (except if a custom Model Alias for the SDK Setup is provided).
 * The Highlighter used by the Controller Highlighter will be selected in the following order:
   * The provided Base Highlighter in the `Controller Highlighter` parameter.
   * If the above is not provided, then the first active Base Highlighter found on the actual controller GameObject will be used.
   * If the above is not found, then a Material Color Swap Highlighter will be created on the actual controller GameObject at runtime.

### Inspector Parameters

 * **Transition Duration:** The amount of time to take to transition to the set highlight colour.
 * **Highlight Controller:** The colour to set the entire controller highlight colour to.
 * **Highlight Body:** The colour to set the body highlight colour to.
 * **Highlight Trigger:** The colour to set the trigger highlight colour to.
 * **Highlight Grip:** The colour to set the grip highlight colour to.
 * **Highlight Touchpad:** The colour to set the touchpad highlight colour to.
 * **Highlight Button One:** The colour to set the button one highlight colour to.
 * **Highlight Button Two:** The colour to set the button two highlight colour to.
 * **Highlight System Menu:** The colour to set the system menu highlight colour to.
 * **Highlight Start Menu:** The colour to set the start menu highlight colour to.
 * **Model Element Paths:** A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.
 * **Element Highlighter Overrides:** A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.
 * **Controller Alias:** An optional GameObject to specify which controller to apply the script methods to. If this is left blank then this script is required to be placed on a controller script alias GameObject and it will use the Actual Controller GameObject linked to the controller script alias.
 * **Model Container:** An optional GameObject to specifiy where the controller models are. If this is left blank then the controller Model Alias object will be used.
 * **Controller Highlighter:** An optional Highlighter to use when highlighting the controller element. If this is left blank, then the first active highlighter on the same GameObject will be used, if one isn't found then a Material Color Swap Highlighter will be created at runtime.

### Class Methods

#### ConfigureControllerPaths/0

  > `public virtual void ConfigureControllerPaths()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ConfigureControllerPaths method is used to set up the model element paths.

#### PopulateHighlighters/0

  > `public virtual void PopulateHighlighters()`

 * Parameters
   * _none_
 * Returns
   * _none_

The PopulateHighlighters method sets up the highlighters on the controller model.

#### HighlightController/2

  > `public virtual void HighlightController(Color color, float fadeDuration = 0f)`

 * Parameters
   * `Color color` - The Color to highlight the controller to.
   * `float fadeDuration` - The duration in seconds to fade from the initial color to the target color.
 * Returns
   * _none_

The HighlightController method attempts to highlight all sub models of the controller.

#### UnhighlightController/0

  > `public virtual void UnhighlightController()`

 * Parameters
   * _none_
 * Returns
   * _none_

The UnhighlightController method attempts to remove the highlight from all sub models of the controller.

#### HighlightElement/3

  > `public virtual void HighlightElement(SDK_BaseController.ControllerElements elementType, Color color, float fadeDuration = 0f)`

 * Parameters
   * `SDK_BaseController.ControllerElements elementType` - The element type on the controller.
   * `Color color` - The Color to highlight the controller element to.
   * `float fadeDuration` - The duration in seconds to fade from the initial color to the target color.
 * Returns
   * _none_

The HighlightElement method attempts to highlight a specific controller element.

#### UnhighlightElement/1

  > `public virtual void UnhighlightElement(SDK_BaseController.ControllerElements elementType)`

 * Parameters
   * `SDK_BaseController.ControllerElements elementType` - The element type on the controller.
 * Returns
   * _none_

The UnhighlightElement method attempts to remove the highlight from the specific controller element.

### Example

`VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.

---

## Object Auto Grab (VRTK_ObjectAutoGrab)

### Overview

Attempt to automatically grab a specified Interactable Object.

**Required Components:**
 * `VRTK_InteractTouch` - The touch component to determine when a valid touch has taken place to denote a use interaction can occur. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.
 * `VRTK_InteractGrab` - The grab component to determine when a valid grab has taken place. This must be applied on the same GameObject as this script if one is not provided via the `Interact Grab` parameter.

**Script Usage:**
 * Place the `VRTK_ObjectAutoGrab` script on either:
   * The GameObject that contains the Interact Touch and Interact Grab scripts.
   * Any other scene GameObject and provide a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter and a valid `VRTK_InteractGrab` component to the `Interact Grab` parameter of this script.
* Assign the Interactable Object to auto grab to the `Object To Grab` parameter on this script.
* If this Interactable Object is a prefab then the `Object Is Prefab` parameter on this script must be checked.

### Inspector Parameters

 * **Object To Grab:** The Interactable Object that will be grabbed by the Interact Grab.
 * **Object Is Prefab:** If the `Object To Grab` is a prefab then this needs to be checked, if the `Object To Grab` already exists in the scene then this needs to be unchecked.
 * **Clone Grabbed Object:** If this is checked then the `Object To Grab` will be cloned into a new Interactable Object and grabbed by the Interact Grab leaving the existing Interactable Object in the scene. This is required if the same Interactable Object is to be grabbed to multiple instances of Interact Grab. It is also required to clone a grabbed Interactable Object if it is a prefab as it needs to exist within the scene to be grabbed.
 * **Always Clone On Enable:** If `Clone Grabbed Object` is checked and this is checked, then whenever this script is disabled and re-enabled, it will always create a new clone of the Interactable Object to grab. If this is unchecked then the original cloned Interactable Object will attempt to be grabbed again. If the original cloned object no longer exists then a new clone will be created.
 * **Attempt Secondary Grab:** If this is checked then the `Object To Grab` will attempt to be secondary grabbed as well as primary grabbed.
 * **Interact Touch:** The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Interact Grab:** The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Secondary Interact Touch:** The secondary controller Interact Touch to listen for touches on. If this field is left blank then it will be looked up on the opposite controller script alias at runtime.
 * **Secondary Interact Grab:** The secondary controller Interact Grab to listen for grab actions on. If this field is left blank then it will be looked up on the opposite controller script alias at runtime.

### Class Events

 * `ObjectAutoGrabCompleted` - Emitted when the object auto grab has completed successfully.

### Unity Events

Adding the `VRTK_ObjectAutoGrab_UnityEvents` component to `VRTK_ObjectAutoGrab` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### ClearPreviousClone/0

  > `public virtual void ClearPreviousClone()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ClearPreviousClone method resets the previous cloned Interactable Object to null to ensure when the script is re-enabled that a new cloned Interactable Object is created, rather than the original clone being grabbed again.

### Example

`VRTK/Examples/026_Controller_ForceHoldObject` shows how to automatically grab a sword to each controller and also prevents the swords from being dropped so they are permanently attached to the user's controllers.

---

# Interactables (VRTK/Source/Scripts/Interactions/Interactables)

A collection of scripts that provide the ability denote objects as being interactable and providing functionality when an object is interected with.

 * [Interactable Object](#interactable-object-vrtk_interactableobject)
 * [Interactable Listener](#interactable-listener-vrtk_interactablelistener)
 * [Interact Haptics](#interact-haptics-vrtk_interacthaptics)
 * [Interact Object Appearance](#interact-object-appearance-vrtk_interactobjectappearance)
 * [Interact Object Highlighter](#interact-object-highlighter-vrtk_interactobjecthighlighter)
 * [Object Touch Auto Interact](#object-touch-auto-interact-vrtk_objecttouchautointeract)
 * [Ignore Interact Touch Colliders](#ignore-interact-touch-colliders-vrtk_ignoreinteracttouchcolliders)

---

## Interactable Object (VRTK_InteractableObject)

### Overview

Determines if the GameObject can be interacted with.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects.

**Optional Components:**
 * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System (not required for Climbable Grab Attach Types).
 * `VRTK_BaseGrabAttach` - A Grab Attach mechanic for determining how the Interactable Object is grabbed by the primary interacting object.
 * `VRTK_BaseGrabAction` - A Grab Action mechanic for determining how to manipulate the Interactable Object when grabbed by the secondary interacting object.

**Script Usage:**
 * Place the `VRTK_InteractableObject` script onto the GameObject that is to be interactable.
 * Alternatively, select the GameObject and use the `Window -> VRTK -> Setup Interactable Object` panel to set up quickly.
 * The optional Highlighter used by the Interactable Object will be selected in the following order:
   * The provided Base Highlighter in the `Object Highlighter` parameter.
   * If the above is not provided, then the first active Base Highlighter found on the Interactable Object GameObject will be used.
   * If the above is not found, then a Material Color Swap Highlighter will be created on the Interactable Object GameObject at runtime.

**Script Dependencies:**
 * Interactions
   * To near touch an Interactable Object the Interact NearTouch script is required on a controller Script Alias GameObject.
   * To touch an Interactable Object the Interact NearTouch script is required on a controller Script Alias GameObject.
   * To grab an Interactable Object the Interact Grab script is required on a controller Script Alias GameObject.
   * To use an Interactable Object the Interact Use script is required on a controller Script Alias GameObject.
 * Highlighting
   * To highlight an Interactable Object on a given interaction then a valid Interact Object Highlighter script must be associated with the Interactable Object.
 * Appearance
   * To affect the appearance of an Interactable Object then a valid Interact Object Appearance script must be associated with the Interactable Object.

### Inspector Parameters

 * **Disable When Idle:** If this is checked then the Interactable Object component will be disabled when the Interactable Object is not being interacted with.
 * **Allowed Near Touch Controllers:** Determines which controller can initiate a near touch action.
 * **Allowed Touch Controllers:** Determines which controller can initiate a touch action.
 * **Ignored Colliders:** An array of colliders on the GameObject to ignore when being touched.
 * **Is Grabbable:** Determines if the Interactable Object can be grabbed.
 * **Hold Button To Grab:** If this is checked then the grab button on the controller needs to be continually held down to keep grabbing. If this is unchecked the grab button toggles the grab action with one button press to grab and another to release.
 * **Stay Grabbed On Teleport:** If this is checked then the Interactable Object will stay grabbed to the controller when a teleport occurs. If it is unchecked then the Interactable Object will be released when a teleport occurs.
 * **Valid Drop:** Determines in what situation the Interactable Object can be dropped by the controller grab button.
 * **Grab Override Button:** Setting to a button will ensure the override button is used to grab this specific Interactable Object. Setting to `Undefined` will mean the `Grab Button` on the Interact Grab script will grab the object.
 * **Allowed Grab Controllers:** Determines which controller can initiate a grab action.
 * **Grab Attach Mechanic Script:** This determines how the grabbed Interactable Object will be attached to the controller when it is grabbed. If one isn't provided then the first Grab Attach script on the GameObject will be used, if one is not found and the object is grabbable then a Fixed Joint Grab Attach script will be created at runtime.
 * **Secondary Grab Action Script:** The script to utilise when processing the secondary controller action on a secondary grab attempt. If one isn't provided then the first Secondary Controller Grab Action script on the GameObject will be used, if one is not found then no action will be taken on secondary grab.
 * **Is Usable:** Determines if the Interactable Object can be used.
 * **Hold Button To Use:** If this is checked then the use button on the controller needs to be continually held down to keep using. If this is unchecked the the use button toggles the use action with one button press to start using and another to stop using.
 * **Use Only If Grabbed:** If this is checked the Interactable Object can be used only if it is currently being grabbed.
 * **Pointer Activates Use Action:** If this is checked then when a Pointer collides with the Interactable Object it will activate it's use action. If the the `Hold Button To Use` parameter is unchecked then whilst the Pointer is collising with the Interactable Object it will run the `Using` method. If `Hold Button To Use` is unchecked then the `Using` method will be run when the Pointer is deactivated. The Pointer will not emit the `Destination Set` event if it is affecting an Interactable Object with this setting checked as this prevents unwanted teleporting from happening when using an Interactable Object with a pointer.
 * **Use Override Button:** Setting to a button will ensure the override button is used to use this specific Interactable Object. Setting to `Undefined` will mean the `Use Button` on the Interact Use script will use the object.
 * **Allowed Use Controllers:** Determines which controller can initiate a use action.

### Class Variables

 * `public enum InteractionType` - The interaction type.
   * `None` - No interaction is affecting the object.
   * `NearTouch` - The near touch interaction is affecting the object.
   * `NearUntouch` - The near untouch interaction stopped affecting the object
   * `Touch` - The touch interaction is affecting the object.
   * `Untouch` - The untouch interaction stopped affecting the object
   * `Grab` - The grab interaction is affecting the object.
   * `Ungrab` - The ungrab interaction stopped affecting the object
   * `Use` - The use interaction is affecting the object.
   * `Unuse` - The unuse interaction stopped affecting the object
 * `public enum AllowedController` - Allowed controller type.
   * `Both` - Both controllers are allowed to interact.
   * `LeftOnly` - Only the left controller is allowed to interact.
   * `RightOnly` - Only the right controller is allowed to interact.
 * `public enum ValidDropTypes` - The types of valid situations that the object can be released from grab.
   * `NoDrop` - The object cannot be dropped via the controller.
   * `DropAnywhere` - The object can be dropped anywhere in the scene via the controller.
   * `DropValidSnapDropZone` - The object can only be dropped when it is hovering over a valid snap drop zone.
 * `public int usingState` - The current using state of the Interactable Object. `0` not being used, `1` being used. Default: `0`
 * `public bool isKinematic` - isKinematic is a pass through to the `isKinematic` getter/setter on the Interactable Object's Rigidbody component.

### Class Events

 * `InteractableObjectEnabled` - Emitted when the Interactable Object script is enabled;
 * `InteractableObjectDisabled` - Emitted when the Interactable Object script is disabled;
 * `InteractableObjectNearTouched` - Emitted when another interacting object near touches the current Interactable Object.
 * `InteractableObjectNearUntouched` - Emitted when the other interacting object stops near touching the current Interactable Object.
 * `InteractableObjectTouched` - Emitted when another interacting object touches the current Interactable Object.
 * `InteractableObjectUntouched` - Emitted when the other interacting object stops touching the current Interactable Object.
 * `InteractableObjectGrabbed` - Emitted when another interacting object grabs the current Interactable Object.
 * `InteractableObjectUngrabbed` - Emitted when the other interacting object stops grabbing the current Interactable Object.
 * `InteractableObjectUsed` - Emitted when another interacting object uses the current Interactable Object.
 * `InteractableObjectUnused` - Emitted when the other interacting object stops using the current Interactable Object.
 * `InteractableObjectEnteredSnapDropZone` - Emitted when the Interactable Object enters a Snap Drop Zone.
 * `InteractableObjectExitedSnapDropZone` - Emitted when the Interactable Object exists a Snap Drop Zone.
 * `InteractableObjectSnappedToDropZone` - Emitted when the Interactable Object gets snapped to a Snap Drop Zone.
 * `InteractableObjectUnsnappedFromDropZone` - Emitted when the Interactable Object gets unsnapped from a Snap Drop Zone.

### Unity Events

Adding the `VRTK_InteractableObject_UnityEvents` component to `VRTK_InteractableObject` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject interactingObject` - The GameObject that is initiating the interaction (e.g. a controller).

### Class Methods

#### IsNearTouched/0

  > `public virtual bool IsNearTouched()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently being near touched.

The IsNearTouched method is used to determine if the Interactable Object is currently being near touched.

#### IsTouched/0

  > `public virtual bool IsTouched()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently being touched.

The IsTouched method is used to determine if the Interactable Object is currently being touched.

#### IsGrabbed/1

  > `public virtual bool IsGrabbed(GameObject grabbedBy = null)`

 * Parameters
   * `GameObject grabbedBy` - An optional GameObject to check if the Interactable Object is grabbed by that specific GameObject. Defaults to `null`
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently being grabbed.

The IsGrabbed method is used to determine if the Interactable Object is currently being grabbed.

#### IsUsing/1

  > `public virtual bool IsUsing(GameObject usedBy = null)`

 * Parameters
   * `GameObject usedBy` - An optional GameObject to check if the Interactable Object is used by that specific GameObject. Defaults to `null`
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently being used.

The IsUsing method is used to determine if the Interactable Object is currently being used.

#### StartNearTouching/1

  > `public virtual void StartNearTouching(VRTK_InteractNearTouch currentNearTouchingObject = null)`

 * Parameters
   * `VRTK_InteractNearTouch currentNearTouchingObject` - The interacting object that is currently nearly touching this Interactable Object.
 * Returns
   * _none_

The StartNearTouching method is called automatically when the Interactable Object is initially nearly touched.

#### StopNearTouching/1

  > `public virtual void StopNearTouching(VRTK_InteractNearTouch previousNearTouchingObject = null)`

 * Parameters
   * `VRTK_InteractNearTouch previousNearTouchingObject` - The interacting object that was previously nearly touching this Interactable Object.
 * Returns
   * _none_

The StopNearTouching method is called automatically when the Interactable Object has stopped being nearly touched.

#### StartTouching/1

  > `public virtual void StartTouching(VRTK_InteractTouch currentTouchingObject = null)`

 * Parameters
   * `VRTK_InteractTouch currentTouchingObject` - The interacting object that is currently touching this Interactable Object.
 * Returns
   * _none_

The StartTouching method is called automatically when the Interactable Object is touched initially.

#### StopTouching/1

  > `public virtual void StopTouching(VRTK_InteractTouch previousTouchingObject = null)`

 * Parameters
   * `VRTK_InteractTouch previousTouchingObject` - The interacting object that was previously touching this Interactable Object.
 * Returns
   * _none_

The StopTouching method is called automatically when the Interactable Object has stopped being touched.

#### Grabbed/1

  > `public virtual void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)`

 * Parameters
   * `VRTK_InteractGrab currentGrabbingObject` - The interacting object that is currently grabbing this Interactable Object.
 * Returns
   * _none_

The Grabbed method is called automatically when the Interactable Object is grabbed initially.

#### Ungrabbed/1

  > `public virtual void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)`

 * Parameters
   * `VRTK_InteractGrab previousGrabbingObject` - The interacting object that was previously grabbing this Interactable Object.
 * Returns
   * _none_

The Ungrabbed method is called automatically when the Interactable Object has stopped being grabbed.

#### StartUsing/1

  > `public virtual void StartUsing(VRTK_InteractUse currentUsingObject = null)`

 * Parameters
   * `VRTK_InteractUse currentUsingObject` - The interacting object that is currently using this Interactable Object.
 * Returns
   * _none_

The StartUsing method is called automatically when the Interactable Object is used initially.

#### StopUsing/2

  > `public virtual void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)`

 * Parameters
   * `VRTK_InteractUse previousUsingObject` - The interacting object that was previously using this Interactable Object.
   * `bool resetUsingObjectState` - Resets the using object state to reset it's using action.
 * Returns
   * _none_

The StopUsing method is called automatically when the Interactable Object has stopped being used.

#### PauseCollisions/1

  > `public virtual void PauseCollisions(float delay)`

 * Parameters
   * `float delay` - The time in seconds to pause the collisions for.
 * Returns
   * _none_

The PauseCollisions method temporarily pauses all collisions on the Interactable Object at grab time by removing the Interactable Object's Rigidbody's ability to detect collisions.

#### ZeroVelocity/0

  > `public virtual void ZeroVelocity()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ZeroVelocity method resets the velocity and angular velocity to zero on the Rigidbody attached to the Interactable Object.

#### SaveCurrentState/0

  > `public virtual void SaveCurrentState()`

 * Parameters
   * _none_
 * Returns
   * _none_

The SaveCurrentState method stores the existing Interactable Object parent and the Rigidbody kinematic setting.

#### GetNearTouchingObjects/0

  > `public virtual List<GameObject> GetNearTouchingObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<GameObject>` - A list of GameObject of that are currently nearly touching the current Interactable Object.

The GetNearTouchingObjects method is used to return the collecetion of valid GameObjects that are currently nearly touching this Interactable Object.

#### GetTouchingObjects/0

  > `public virtual List<GameObject> GetTouchingObjects()`

 * Parameters
   * _none_
 * Returns
   * `List<GameObject>` - A list of GameObject of that are currently touching the current Interactable Object.

The GetTouchingObjects method is used to return the collecetion of valid GameObjects that are currently touching this Interactable Object.

#### GetGrabbingObject/0

  > `public virtual GameObject GetGrabbingObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject of what is grabbing the current Interactable Object.

The GetGrabbingObject method is used to return the GameObject that is currently grabbing this Interactable Object.

#### GetSecondaryGrabbingObject/0

  > `public virtual GameObject GetSecondaryGrabbingObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject of the secondary influencing object of the current grabbed Interactable Object.

The GetSecondaryGrabbingObject method is used to return the GameObject that is currently being used to influence this Interactable Object whilst it is being grabbed by a secondary influencing.

#### GetUsingObject/0

  > `public virtual GameObject GetUsingObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject of what is using the current Interactable Object.

The GetUsingObject method is used to return the GameObject that is currently using this Interactable Object.

#### GetUsingScript/0

  > `public virtual VRTK_InteractUse GetUsingScript()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractUse` - The Interact Use script of the interacting object that is using the current Interactable Object.

The GetUsingScript method is used to return the Interact Use component that is currently using this Interactable Object.

#### IsValidInteractableController/2

  > `public virtual bool IsValidInteractableController(GameObject actualController, AllowedController controllerCheck)`

 * Parameters
   * `GameObject actualController` - The GameObject of the controller that is being checked.
   * `AllowedController controllerCheck` - The value of which controller is allowed to interact with this object.
 * Returns
   * `bool` - Returns `true` if the interacting controller is allowed to grab the Interactable Object.

The IsValidInteractableController method is used to check to see if a controller is allowed to perform an interaction with this Interactable Object as sometimes controllers are prohibited from grabbing or using an Interactable Object depedning on the use case.

#### ForceStopInteracting/0

  > `public virtual void ForceStopInteracting()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceStopInteracting method forces the Interactable Object to no longer be interacted with and will cause an interacting object to drop the Interactable Object and stop touching it.

#### ForceStopSecondaryGrabInteraction/0

  > `public virtual void ForceStopSecondaryGrabInteraction()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceStopSecondaryGrabInteraction method forces the Interactable Object to no longer be influenced by the second controller grabbing it.

#### RegisterTeleporters/0

  > `public virtual void RegisterTeleporters()`

 * Parameters
   * _none_
 * Returns
   * _none_

The RegisterTeleporters method is used to find all GameObjects that have a teleporter script and register the Interactable Object on the `OnTeleported` event.

#### UnregisterTeleporters/0

  > `public virtual void UnregisterTeleporters()`

 * Parameters
   * _none_
 * Returns
   * _none_

The UnregisterTeleporters method is used to unregister all teleporter events that are active on this Interactable Object.

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
   * `bool state` - The state of whether the Interactable Object is fixed in or removed from the Snap Drop Zone. `true` denotes the Interactable Object is snapped to the Snap Drop Zone and `false` denotes it has been removed from the Snap Drop Zone.
 * Returns
   * _none_

The ToggleSnapDropZone method is used to set the state of whether the Interactable Object is in a Snap Drop Zone or not.

#### IsInSnapDropZone/0

  > `public virtual bool IsInSnapDropZone()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently snapped in a Snap Drop Zone, returns `false` if it is not.

The IsInSnapDropZone method determines whether the Interactable Object is currently snapped to a Snap Drop Zone.

#### SetSnapDropZoneHover/2

  > `public virtual void SetSnapDropZoneHover(VRTK_SnapDropZone snapDropZone, bool state)`

 * Parameters
   * `VRTK_SnapDropZone snapDropZone` - The Snap Drop Zone that is being interacted with.
   * `bool state` - The state of whether the Interactable Object is being hovered or not.
 * Returns
   * _none_

The SetSnapDropZoneHover method sets whether the Interactable Object is currently being hovered over a valid Snap Drop Zone.

#### GetStoredSnapDropZone/0

  > `public virtual VRTK_SnapDropZone GetStoredSnapDropZone()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_SnapDropZone` - The SnapDropZone that the Interactable Object is currently snapped to.

The GetStoredSnapDropZone method returns the Snap Drop Zone that the Interactable Object is currently snapped to.

#### IsHoveredOverSnapDropZone/0

  > `public virtual bool IsHoveredOverSnapDropZone()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object is currently hovering over a Snap Drop Zone.

The IsHoveredOverSnapDropZone method returns whether the Interactable Object is currently hovering over a Snap Drop Zone.

#### IsDroppable/0

  > `public virtual bool IsDroppable()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object can currently be dropped, returns `false` if it is not currently possible to drop.

The IsDroppable method returns whether the Interactable Object can be dropped or not in it's current situation.

#### IsSwappable/0

  > `public virtual bool IsSwappable()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object can be grabbed by a secondary interacting object whilst already being grabbed and the Interactable Object will swap controllers. Returns `false` if the Interactable Object cannot be swapped.

The IsSwappable method returns whether the Interactable Object can be grabbed with one interacting object and then swapped to another interacting object by grabbing with the secondary grab action.

#### PerformSecondaryAction/0

  > `public virtual bool PerformSecondaryAction()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Interactable Object has a Secondary Grab Action, returns `false` if it has no Secondary Grab Action or is swappable.

The PerformSecondaryAction method returns whether the Interactable Object has a Secondary Grab Action that can be performed when grabbing the object with a secondary interacting object.

#### ResetIgnoredColliders/0

  > `public virtual void ResetIgnoredColliders()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetIgnoredColliders method is used to clear any stored ignored colliders in case the `Ignored Colliders` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.

#### SubscribeToInteractionEvent/2

  > `public virtual void SubscribeToInteractionEvent(InteractionType givenType, InteractableObjectEventHandler methodCallback)`

 * Parameters
   * `InteractionType givenType` - The Interaction Type to register the events for.
   * `InteractableObjectEventHandler methodCallback` - The method to execute when the Interaction Type is initiated.
 * Returns
   * _none_

The SubscribeToInteractionEvent method subscribes a given method callback for the given Interaction Type.

#### UnsubscribeFromInteractionEvent/2

  > `public virtual void UnsubscribeFromInteractionEvent(InteractionType givenType, InteractableObjectEventHandler methodCallback)`

 * Parameters
   * `InteractionType givenType` - The Interaction Type that the previous event subscription was under.
   * `InteractableObjectEventHandler methodCallback` - The method that was being executed when the Interaction Type was initiated.
 * Returns
   * _none_

The UnsubscribeFromInteractionEvent method unsubscribes a previous event subscription for the given Interaction Type.

#### GetPrimaryAttachPoint/0

  > `public virtual Transform GetPrimaryAttachPoint()`

 * Parameters
   * _none_
 * Returns
   * `Transform` - A Transform that denotes where the primary grabbing object is grabbing the Interactable Object at.

The GetPrimaryAttachPoint returns the Transform that determines where the primary grabbing object is grabbing the Interactable Object at.

#### GetSecondaryAttachPoint/0

  > `public virtual Transform GetSecondaryAttachPoint()`

 * Parameters
   * _none_
 * Returns
   * `Transform` - A Transform that denotes where the secondary grabbing object is grabbing the Interactable Object at.

The GetSecondaryAttachPoint returns the Transform that determines where the secondary grabbing object is grabbing the Interactable Object at.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` uses the `VRTK_InteractTouch` and `VRTK_InteractGrab` scripts on the controllers to show how an interactable object can be grabbed and snapped to the controller and thrown around the game world.

`VRTK/Examples/013_Controller_UsingAndGrabbingMultipleObjects` shows multiple objects that can be grabbed by holding the buttons or grabbed by toggling the button click and also has objects that can have their Using state toggled to show how multiple items can be turned on at the same time.

---

## Interactable Listener (VRTK_InteractableListener)

### Overview

Provides a base that classes which require to subscribe to the interaction events of an Interactable Object can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides interaction event listener functionality, therefore this script should not be directly used.

---

## Interact Haptics (VRTK_InteractHaptics)
 > extends [VRTK_InteractableListener](#interactable-listener-vrtk_interactablelistener)

### Overview

Provides controller haptics upon interaction with the specified Interactable Object.

**Required Components:**
 * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Object To Affect` parameter.

**Script Usage:**
 * Place the `VRTK_InteractHaptics` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Object To Affect` parameter of this script.

### Inspector Parameters

 * **Clip On Near Touch:** Denotes the audio clip to use to rumble the controller on near touch.
 * **Strength On Near Touch:** Denotes how strong the rumble in the controller will be on near touch.
 * **Duration On Near Touch:** Denotes how long the rumble in the controller will last on near touch.
 * **Interval On Near Touch:** Denotes interval betweens rumble in the controller on near touch.
 * **Cancel On Near Untouch:** If this is checked then the rumble will be cancelled when the controller is no longer near touching.
 * **Clip On Touch:** Denotes the audio clip to use to rumble the controller on touch.
 * **Strength On Touch:** Denotes how strong the rumble in the controller will be on touch.
 * **Duration On Touch:** Denotes how long the rumble in the controller will last on touch.
 * **Interval On Touch:** Denotes interval betweens rumble in the controller on touch.
 * **Cancel On Untouch:** If this is checked then the rumble will be cancelled when the controller is no longer touching.
 * **Clip On Grab:** Denotes the audio clip to use to rumble the controller on grab.
 * **Strength On Grab:** Denotes how strong the rumble in the controller will be on grab.
 * **Duration On Grab:** Denotes how long the rumble in the controller will last on grab.
 * **Interval On Grab:** Denotes interval betweens rumble in the controller on grab.
 * **Cancel On Ungrab:** If this is checked then the rumble will be cancelled when the controller is no longer grabbing.
 * **Clip On Use:** Denotes the audio clip to use to rumble the controller on use.
 * **Strength On Use:** Denotes how strong the rumble in the controller will be on use.
 * **Duration On Use:** Denotes how long the rumble in the controller will last on use.
 * **Interval On Use:** Denotes interval betweens rumble in the controller on use.
 * **Cancel On Unuse:** If this is checked then the rumble will be cancelled when the controller is no longer using.
 * **Object To Affect:** The Interactable Object to initiate the haptics from. If this is left blank, then the Interactable Object will need to be on the current or a parent GameObject.

### Class Events

 * `InteractHapticsNearTouched` - Emitted when the haptics are from a near touch.
 * `InteractHapticsTouched` - Emitted when the haptics are from a touch.
 * `InteractHapticsGrabbed` - Emitted when the haptics are from a grab.
 * `InteractHapticsUsed` - Emitted when the haptics are from a use.

### Unity Events

Adding the `VRTK_InteractHaptics_UnityEvents` component to `VRTK_InteractHaptics` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerReference controllerReference` - The reference to the controller to perform haptics on.

### Class Methods

#### CancelHaptics/1

  > `public virtual void CancelHaptics(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` -
 * Returns
   * _none_

The CancelHaptics method cancels any existing haptic feedback on the given controller.

#### HapticsOnNearTouch/1

  > `public virtual void HapticsOnNearTouch(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to activate the haptic feedback on.
 * Returns
   * _none_

The HapticsOnNearTouch method triggers the haptic feedback on the given controller for the settings associated with near touch.

#### HapticsOnTouch/1

  > `public virtual void HapticsOnTouch(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to activate the haptic feedback on.
 * Returns
   * _none_

The HapticsOnTouch method triggers the haptic feedback on the given controller for the settings associated with touch.

#### HapticsOnGrab/1

  > `public virtual void HapticsOnGrab(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to activate the haptic feedback on.
 * Returns
   * _none_

The HapticsOnGrab method triggers the haptic feedback on the given controller for the settings associated with grab.

#### HapticsOnUse/1

  > `public virtual void HapticsOnUse(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to activate the haptic feedback on.
 * Returns
   * _none_

The HapticsOnUse method triggers the haptic feedback on the given controller for the settings associated with use.

---

## Interact Object Appearance (VRTK_InteractObjectAppearance)
 > extends [VRTK_InteractableListener](#interactable-listener-vrtk_interactablelistener)

### Overview

Determine whether the `Object To Affect` should be visible or hidden by default or on interaction (near touch, touch, grab, use).

**Required Components:**
 * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Object To Monitor` parameter.

**Script Usage:**
 * Place the `VRTK_InteractObjectAppearance` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Object To Monitor` parameter of this script.
 * Optionally provide a GameObject to the `Object To Affect` parameter to determine which GameObject to affect the appearance of.

### Inspector Parameters

 * **Object To Affect:** The GameObject to affect the appearance of. If this is null then then the interacting object will be used (usually the controller).
 * **Game Object Active By Default:** If this is checked then the `Object To Affect` will be an active GameObject when the script is enabled. If it's unchecked then it will be disabled. This only takes effect if `Affect Interacting Object` is unticked.
 * **Renderer Visible By Default:** If this is checked then the `Object To Affect` will have visible renderers when the script is enabled. If it's unchecked then it will have it's renderers disabled. This only takes effect if `Affect Interacting Object` is unticked.
 * **Game Object Active On Near Touch:** If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is near touched. If it's unchecked then it will be disabled on near touch.
 * **Renderer Visible On Near Touch:** If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is near touched. If it's unchecked then it will have it's renderers disabled on near touch.
 * **Near Touch Appearance Delay:** The amount of time to wait before the near touch appearance settings are applied after the near touch event.
 * **Near Untouch Appearance Delay:** The amount of time to wait before the previous appearance settings are applied after the near untouch event.
 * **Valid Near Touch Interacting Object:** Determines what type of interacting object will affect the appearance of the `Object To Affect` after the near touch and near untouch event.
 * **Game Object Active On Touch:** If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is touched. If it's unchecked then it will be disabled on touch.
 * **Renderer Visible On Touch:** If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is touched. If it's unchecked then it will have it's renderers disabled on touch.
 * **Touch Appearance Delay:** The amount of time to wait before the touch appearance settings are applied after the touch event.
 * **Untouch Appearance Delay:** The amount of time to wait before the previous appearance settings are applied after the untouch event.
 * **Valid Touch Interacting Object:** Determines what type of interacting object will affect the appearance of the `Object To Affect` after the touch/untouch event.
 * **Game Object Active On Grab:** If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is grabbed. If it's unchecked then it will be disabled on grab.
 * **Renderer Visible On Grab:** If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is grabbed. If it's unchecked then it will have it's renderers disabled on grab.
 * **Grab Appearance Delay:** The amount of time to wait before the grab appearance settings are applied after the grab event.
 * **Ungrab Appearance Delay:** The amount of time to wait before the previous appearance settings are applied after the ungrab event.
 * **Valid Grab Interacting Object:** Determines what type of interacting object will affect the appearance of the `Object To Affect` after the grab/ungrab event.
 * **Game Object Active On Use:** If this is checked then the `Object To Affect` will be an active GameObject when the `Object To Monitor` is used. If it's unchecked then it will be disabled on use.
 * **Renderer Visible On Use:** If this is checked then the `Object To Affect` will have visible renderers when the `Object To Monitor` is used. If it's unchecked then it will have it's renderers disabled on use.
 * **Use Appearance Delay:** The amount of time to wait before the use appearance settings are applied after the use event.
 * **Unuse Appearance Delay:** The amount of time to wait before the previous appearance settings are applied after the unuse event.
 * **Valid Use Interacting Object:** Determines what type of interacting object will affect the appearance of the `Object To Affect` after the use/unuse event.

### Class Variables

 * `public enum ValidInteractingObject` - The valid interacting object.
   * `Anything` - Any GameObject is considered a valid interacting object.
   * `EitherController` - Only a game controller is considered a valid interacting objcet.
   * `NeitherController` - Any GameObject except a game controller is considered a valid interacting object.
   * `LeftControllerOnly` - Only the left game controller is considered a valid interacting objcet.
   * `RightControllerOnly` - Only the right game controller is considered a valid interacting objcet.

### Class Events

 * `GameObjectEnabled` - Emitted when the GameObject on the `Object To Affect` is enabled.
 * `GameObjectDisabled` - Emitted when the GameObject on the `Object To Affect` is disabled.
 * `RenderersEnabled` - Emitted when the Renderers on the `Object To Affect` are enabled.
 * `RenderersDisabled` - Emitted when the Renderers on the `Object To Affect` are disabled.

### Unity Events

Adding the `VRTK_InteractObjectAppearance_UnityEvents` component to `VRTK_InteractObjectAppearance` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject affectingObject` - The GameObject that is being affected.
 * `VRTK_InteractableObject monitoringObject` - The Interactable Object that is being monitored.
 * `VRTK_InteractableObject.InteractionType interactionType` - The type of interaction initiating the event.

### Example

`VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that the controller can be hidden when touching, grabbing and using an object.

---

## Interact Object Highlighter (VRTK_InteractObjectHighlighter)
 > extends [VRTK_InteractableListener](#interactable-listener-vrtk_interactablelistener)

### Overview

Enable highlighting of an Interactable Object base on interaction type.

**Required Components:**
 * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Object To Monitor` parameter.

**Optional Components:**
 * `VRTK_BaseHighlighter` - The highlighter to use when highligting the Object. If one is not already injected in the `Object Highlighter` parameter then the component on the same GameObject will be used.

**Script Usage:**
 * Place the `VRTK_InteractObjectHighlighter` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Object To Affect` parameter of this script.

### Inspector Parameters

 * **Near Touch Highlight:** The colour to highlight the object on the near touch interaction.
 * **Touch Highlight:** The colour to highlight the object on the touch interaction.
 * **Grab Highlight:** The colour to highlight the object on the grab interaction.
 * **Use Highlight:** The colour to highlight the object on the use interaction.
 * **Object To Highlight:** The GameObject to highlight.
 * **Object Highlighter:** An optional Highlighter to use when highlighting the specified Object. If this is left blank, then the first active highlighter on the same GameObject will be used, if one isn't found then a Material Color Swap Highlighter will be created at runtime.

### Class Events

 * `InteractObjectHighlighterHighlighted` - Emitted when the object is highlighted
 * `InteractObjectHighlighterUnhighlighted` - Emitted when the object is unhighlighted

### Unity Events

Adding the `VRTK_InteractObjectHighlighter_UnityEvents` component to `VRTK_InteractObjectHighlighter` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_InteractableObject.InteractionType interactionType` - The type of interaction occuring on the object to monitor.
 * `Color highlightColor` - The colour being provided to highlight the affected object with.
 * `GameObject affectingObject` - The GameObject is initiating the highlight via an interaction.
 * `VRTK_InteractableObject objectToMonitor` - The Interactable Object that is being interacted with.
 * `GameObject affectedObject` - The GameObject that is being highlighted.

### Class Methods

#### ResetHighlighter/0

  > `public virtual void ResetHighlighter()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetHighlighter method is used to reset the currently attached highlighter.

#### Highlight/1

  > `public virtual void Highlight(Color highlightColor)`

 * Parameters
   * `Color highlightColor` - The colour to apply to the highlighter.
 * Returns
   * _none_

The Highlight method turns on the highlighter with the given Color.

#### Unhighlight/0

  > `public virtual void Unhighlight()`

 * Parameters
   * _none_
 * Returns
   * _none_

The Unhighlight method turns off the highlighter.

#### GetCurrentHighlightColor/0

  > `public virtual Color GetCurrentHighlightColor()`

 * Parameters
   * _none_
 * Returns
   * `Color` - The Color that the Interactable Object is being highlighted to.

The GetCurrentHighlightColor returns the colour that the Interactable Object is currently being highlighted to.

---

## Object Touch Auto Interact (VRTK_ObjectTouchAutoInteract)
 > extends [VRTK_InteractableListener](#interactable-listener-vrtk_interactablelistener)

### Overview

Allows for Interact Grab or Interact Use interactions to automatically happen upon touching an Interactable Object.

**Required Components:**
 * `VRTK_InteractableObject` - The Interactable Object component to detect interactions on. This must be applied on the same GameObject as this script if one is not provided via the `Interactable Object` parameter.

**Script Usage:**
 * Place the `VRTK_ObjectTouchAutoInteract` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and provide a valid `VRTK_InteractableObject` component to the `Interactable Object` parameter of this script.

### Inspector Parameters

 * **Grab On Touch When:** Determines when a grab on touch should occur.
 * **Regrab Delay:** After being ungrabbed, another auto grab on touch can only occur after this time.
 * **Continuous Grab Check:** If this is checked then the grab on touch check will happen every frame and not only on the first touch of the Interactable Object.
 * **Use On Touch When:** Determines when a use on touch should occur.
 * **Reuse Delay:** After being unused, another auto use on touch can only occur after this time.
 * **Continuous Use Check:** If this is checked then the use on touch check will happen every frame and not only on the first touch of the Interactable Object.
 * **Interactable Object:** The Interactable Object that the auto interaction will occur on. If this is blank then the script must be on the same GameObject as the Interactable Object script.

### Class Variables

 * `public enum AutoInteractions` - Situation when auto interaction can occur.
   * `Never` - Auto interaction can never occur on touch.
   * `NoButtonHeld` - Auto interaction will occur on touch even if the specified interaction button is not held down.
   * `ButtonHeld` - Auto interaction will only occur on touch if the specified interaction button is held down.

---

## Ignore Interact Touch Colliders (VRTK_IgnoreInteractTouchColliders)
 > extends VRTK_SDKControllerReady

### Overview

Ignores the collisions between the given Interact Touch colliders and the colliders on the GameObject this script is attached to.

**Required Components:**
 * `Collider` - Unity Colliders on the current GameObject or child GameObjects to ignore collisions from the given Interact Touch colliders.

**Script Usage:**
 * Place the `VRTK_IgnoreInteractTouchColliders` script on the GameObject with colliders to ignore collisions from the given Interact Touch colliders.
 * Increase the size of the `Interact Touch To Ignore` element list.
 * Add the appropriate GameObjects that have the `VRTK_InteractTouch` script attached to use when ignoring collisions with the colliders on GameObject the script is attached to.

### Inspector Parameters

 * **Interact Touch To Ignore:** The Interact Touch scripts to ignore collisions with.
 * **Skip Ignore:** A collection of GameObjects to not include when ignoring collisions with the provided Interact Touch colliders.

---

# Grab Attach Mechanics (VRTK/Source/Scripts/Interactions/GrabAttachMechanics)

A collection of scripts that are used to provide different mechanics to apply when grabbing an interactable object.

 * [Base Grab Attach](#base-grab-attach-vrtk_basegrabattach)
 * [Base Joint Grab Attach](#base-joint-grab-attach-vrtk_basejointgrabattach)
 * [Fixed Joint Grab Attach](#fixed-joint-grab-attach-vrtk_fixedjointgrabattach)
 * [Spring Joint Grab Attach](#spring-joint-grab-attach-vrtk_springjointgrabattach)
 * [Custom Joint Grab Attach](#custom-joint-grab-attach-vrtk_customjointgrabattach)
 * [Child Of Controller Grab Attach](#child-of-controller-grab-attach-vrtk_childofcontrollergrabattach)
 * [Track Object Grab Attach](#track-object-grab-attach-vrtk_trackobjectgrabattach)
 * [Rotator Track Grab Attach](#rotator-track-grab-attach-vrtk_rotatortrackgrabattach)
 * [Climbable Grab Attach](#climbable-grab-attach-vrtk_climbablegrabattach)
 * [Control Animation Grab Attach](#control-animation-grab-attach-vrtk_controlanimationgrabattach)
 * [Move Transform Grab Attach](#move-transform-grab-attach-vrtk_movetransformgrabattach)
 * [Rotate Transform Grab Attach](#rotate-transform-grab-attach-vrtk_rotatetransformgrabattach)

---

## Base Grab Attach (VRTK_BaseGrabAttach)

### Overview

Provides a base that all grab attach mechanics can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides grab attach functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Precision Grab:** If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.
 * **Right Snap Handle:** A Transform provided as an empty GameObject which must be the child of the Interactable Object being grabbed and serves as an orientation point to rotate and position the grabbed Interactable Object in relation to the right handed Interact Grab. If no Right Snap Handle is provided but a Left Snap Handle is provided, then the Left Snap Handle will be used in place. If no Snap Handle is provided then the Interactable Object will be grabbed at its central point. Not required for `Precision Grab`.
 * **Left Snap Handle:** A Transform provided as an empty GameObject which must be the child of the Interactable Object being grabbed and serves as an orientation point to rotate and position the grabbed Interactable Object in relation to the left handed Interact Grab. If no Left Snap Handle is provided but a Right Snap Handle is provided, then the Right Snap Handle will be used in place. If no Snap Handle is provided then the Interactable Object will be grabbed at its central point. Not required for `Precision Grab`.
 * **Throw Velocity With Attach Distance:** If checked then when the Interactable Object is thrown, the distance between the Interactable Object's attach point and the Interact Grab's attach point will be used to calculate a faster throwing velocity.
 * **Throw Multiplier:** An amount to multiply the velocity of the given Interactable Object when it is thrown. This can also be used in conjunction with the Interact Grab Throw Multiplier to have certain Interactable Objects be thrown even further than normal (or thrown a shorter distance if a number below 1 is entered).
 * **On Grab Collision Delay:** The amount of time to delay collisions affecting the Interactable Object when it is first grabbed. This is useful if the Interactable Object could get stuck inside another GameObject when it is being grabbed.

### Class Methods

#### IsTracked/0

  > `public virtual bool IsTracked()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Is true if the mechanic is of type tracked.

The IsTracked method determines if the grab attach mechanic is a track object type.

#### IsClimbable/0

  > `public virtual bool IsClimbable()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Is true if the mechanic is of type climbable.

The IsClimbable method determines if the grab attach mechanic is a climbable object type.

#### IsKinematic/0

  > `public virtual bool IsKinematic()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Is true if the mechanic is of type kinematic.

The IsKinematic method determines if the grab attach mechanic is a kinematic object type.

#### ValidGrab/1

  > `public virtual bool ValidGrab(Rigidbody checkAttachPoint)`

 * Parameters
   * `Rigidbody checkAttachPoint` - The rigidbody attach point to check.
 * Returns
   * `bool` - Always returns `true` for the base check.

The ValidGrab method determines if the grab attempt is valid.

#### SetTrackPoint/1

  > `public virtual void SetTrackPoint(Transform givenTrackPoint)`

 * Parameters
   * `Transform givenTrackPoint` - The track point to set on the grabbed Interactable Object.
 * Returns
   * _none_

The SetTrackPoint method sets the point on the grabbed Interactable Object where the grab is happening.

#### SetInitialAttachPoint/1

  > `public virtual void SetInitialAttachPoint(Transform givenInitialAttachPoint)`

 * Parameters
   * `Transform givenInitialAttachPoint` - The point where the initial grab took place.
 * Returns
   * _none_

The SetInitialAttachPoint method sets the point on the grabbed Interactable Object where the initial grab happened.

#### StartGrab/3

  > `public virtual bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.

#### StopGrab/1

  > `public virtual void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### CreateTrackPoint/4

  > `public virtual Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

 * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The GameObject that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The GameObject that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
 * Returns
   * `Transform` - The Transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public virtual void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the Interactable Object.

#### ProcessFixedUpdate/0

  > `public virtual void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the Interactable Object.

#### ResetState/0

  > `public virtual void ResetState()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetState method re-initializes the grab attach.

---

## Base Joint Grab Attach (VRTK_BaseJointGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Provides a base that all joint based grab attach mechanics can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides joint based grab attach functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Destroy Immediately On Throw:** Determines whether the joint should be destroyed immediately on release or whether to wait till the end of the frame before being destroyed.

### Class Methods

#### ValidGrab/1

  > `public override bool ValidGrab(Rigidbody checkAttachPoint)`

 * Parameters
   * `Rigidbody checkAttachPoint` - The rigidbody attach point to check.
 * Returns
   * `bool` - Returns `true` if there is no current grab happening, or the grab is initiated by another grabbing object.

The ValidGrab method determines if the grab attempt is valid.

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed. It is also responsible for creating the joint on the grabbed object.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state. It is also responsible for removing the joint from the grabbed object.

---

## Fixed Joint Grab Attach (VRTK_FixedJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

Attaches the grabbed Interactable Object to the grabbing object via a Fixed Joint.

  > The Interactable Object will be attached to the grabbing object via a Fixed Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.

**Script Usage:**
 * Place the `VRTK_FixedJointGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Break Force:** Maximum force the Joint can withstand before breaking. Setting to `infinity` ensures the Joint is unbreakable.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates this grab attach mechanic all of the grabbable objects in the scene.

---

## Spring Joint Grab Attach (VRTK_SpringJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

Attaches the grabbed Interactable Object to the grabbing object via a Spring Joint.

  > The Interactable Object will be attached to the grabbing object via a Spring Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.

**Script Usage:**
 * Place the `VRTK_SpringJointGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Break Force:** Maximum force the Joint can withstand before breaking. Setting to `infinity` ensures the Joint is unbreakable.
 * **Strength:** The strength of the spring.
 * **Damper:** The amount of dampening to apply to the spring.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Drawer object in the scene.

---

## Custom Joint Grab Attach (VRTK_CustomJointGrabAttach)
 > extends [VRTK_BaseJointGrabAttach](#base-joint-grab-attach-vrtk_basejointgrabattach)

### Overview

Attaches the grabbed Interactable Object to the grabbing object via a custom Joint.

  > The Interactable Object will be attached to the grabbing object via a custom Joint and the Joint can be broken upon colliding the Interactable Object with other colliders.

**Script Usage:**
 * Place the `VRTK_CustomJointGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
 * Create a `Joint` component suitable for attaching the grabbed Interactable Object to the grabbing object with and provide it to the `Custom Joint` parameter.

### Inspector Parameters

 * **Custom Joint:** The joint to use for the grab attach joint.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Lamp object in the scene.

---

## Child Of Controller Grab Attach (VRTK_ChildOfControllerGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Sets the grabbed Interactable Object to be a child of the grabbing object.

  > The Interactable Object will have 1:1 tracking of the grabbing object, however it will also have reduced collision detection and will be able to pass through other colliders.

**Script Usage:**
 * Place the `VRTK_ChildOfControllerGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Class Methods

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed. It is also responsible for creating the joint on the grabbed object.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

### Example

`VRTK/Examples/023_Controller_ChildOfControllerOnGrab` uses this grab attach mechanic for the bow and the arrow.

---

## Track Object Grab Attach (VRTK_TrackObjectGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Applies velocity to the grabbed Interactable Object to ensure it tracks the position of the grabbing object.

  > The Interactable Object follows the grabbing object based on velocity being applied and therefore fully interacts with all other scene Colliders but not at a true 1:1 tracking.

**Script Usage:**
 * Place the `VRTK_TrackObjectGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Detach Distance:** The maximum distance the grabbing object is away from the Interactable Object before it is automatically dropped.
 * **Velocity Limit:** The maximum amount of velocity magnitude that can be applied to the Interactable Object. Lowering this can prevent physics glitches if Interactable Objects are moving too fast.
 * **Angular Velocity Limit:** The maximum amount of angular velocity magnitude that can be applied to the Interactable Object. Lowering this can prevent physics glitches if Interactable Objects are moving too fast.
 * **Max Distance Delta:** The maximum difference in distance to the tracked position.

### Class Methods

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed Interactable Object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### CreateTrackPoint/4

  > `public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

 * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The GameObject that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The GameObject that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
 * Returns
   * `Transform` - The Transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the Interactable Object. It is responsible for checking if the tracked object has exceeded it's detach distance.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the Interactable Object. It applies velocity to the object to ensure it is tracking the grabbing object.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Chest handle and Fire Extinguisher body.

---

## Rotator Track Grab Attach (VRTK_RotatorTrackGrabAttach)
 > extends [VRTK_TrackObjectGrabAttach](#track-object-grab-attach-vrtk_trackobjectgrabattach)

### Overview

Applies a rotational force to the grabbed Interactable Object.

  > The Interactable Object is not attached to the grabbing object but rather has a rotational force applied based on the rotation of the grabbing object.

**Script Usage:**
 * Place the `VRTK_RotatorTrackGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Class Methods

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed Interactable Object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method is run in every FixedUpdate method on the Interactable Object. It applies a force to the grabbed Interactable Object to move it in the direction of the grabbing object.

### Example

`VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Wheel and Door objects in the scene.

---

## Climbable Grab Attach (VRTK_ClimbableGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Marks the Interactable Object as being climbable.

  > The Interactable Object will not be grabbed to the controller, instead in conjunction with the `VRTK_PlayerClimb` script will enable the PlayArea to be moved around as if it was climbing.

**Script Usage:**
 * Place the `VRTK_ClimbableGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Use Object Rotation:** Will respect the grabbed climbing object's rotation if it changes dynamically

### Example

`VRTK/Examples/037_CameraRig_ClimbingFalling` uses this grab attach mechanic for each item that is climbable in the scene.

---

## Control Animation Grab Attach (VRTK_ControlAnimationGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Scrubs through the given animation based on the distance from the grabbing object to the original grabbing point.

**Script Usage:**
 * Place the `VRTK_ControlAnimationGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
   * Create and apply an animation via:
     * `Animation Timeline` parameter takes a legacy `Animation` component to use as the timeline to scrub through. The animation must be marked as `legacy` via the inspector in debug mode.
     * `Animator Timeline` parameter takes an Animator component to use as the timeline to scrub through.

### Inspector Parameters

 * **Detach Distance:** The maximum distance the grabbing object is away from the Interactable Object before it is automatically released.
 * **Animation Timeline:** An Animation with the timeline to scrub through on grab. If this is set then the `Animator Timeline` will be ignored if it is also set.
 * **Animator Timeline:** An Animator with the timeline to scrub through on grab.
 * **Max Frames:** The maximum amount of frames in the timeline.
 * **Distance Multiplier:** An amount to multiply the distance by to determine the scrubbed frame to be on.
 * **Rewind On Release:** If this is checked then the animation will rewind to the start on ungrab.
 * **Rewind Speed Multplier:** The speed in which the animation rewind will be multiplied by.

### Class Events

 * `AnimationFrameAtStart` - Emitted when the Animation Frame is at the start.
 * `AnimationFrameAtEnd` - Emitted when the Animation Frame is at the end.
 * `AnimationFrameChanged` - Emitted when the Animation Frame has changed.

### Unity Events

Adding the `VRTK_ControlAnimationGrabAttach_UnityEvents` component to `VRTK_ControlAnimationGrabAttach` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject interactingObject` - The GameObject that is performing the interaction (e.g. a controller).
 * `float currentFrame` - The current frame the animation is on.

### Class Methods

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### CreateTrackPoint/4

  > `public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

 * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The GameObject that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The GameObject that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
 * Returns
   * `Transform` - The Transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the Interactable Object.

#### SetFrame/1

  > `public virtual void SetFrame(float frame)`

 * Parameters
   * `float frame` - The frame to scrub to.
 * Returns
   * _none_

The SetFrame method scrubs to the specific frame of the Animator timeline.

#### RewindAnimation/0

  > `public virtual void RewindAnimation()`

 * Parameters
   * _none_
 * Returns
   * _none_

The RewindAnimation method will force the animation to rewind to the start frame.

---

## Move Transform Grab Attach (VRTK_MoveTransformGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Moves the Transform of the Interactable Object towards the interacting object within specified limits.

  > To allow unrestricted movement, set the axis limit minimum to `-infinity` and the axis limit maximum to `infinity`.

**Script Usage:**
 * Place the `VRTK_MoveTransformGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Detach Distance:** The maximum distance the grabbing object is away from the Interactable Object before it is automatically released.
 * **Tracking Speed:** The speed in which to track the grabbed Interactable Object to the interacting object.
 * **Force Kinematic On Grab:** If this is checked then it will force the rigidbody on the Interactable Object to be `Kinematic` when the grab occurs.
 * **Release Deceleration Damper:** The damper in which to slow the Interactable Object down when released to simulate continued momentum. The higher the number, the faster the Interactable Object will come to a complete stop on release.
 * **Reset To Orign On Release Speed:** The speed in which the Interactable Object returns to it's origin position when released. If the `Reset To Orign On Release Speed` is `0f` then the position will not be reset.
 * **X Axis Limits:** The minimum and maximum limits the Interactable Object can be moved along the x axis.
 * **Y Axis Limits:** The minimum and maximum limits the Interactable Object can be moved along the y axis.
 * **Z Axis Limits:** The minimum and maximum limits the Interactable Object can be moved along the z axis.
 * **Min Max Threshold:** The threshold the position value needs to be within to register a min or max position value.
 * **Min Max Normalized Threshold:** The threshold the normalized position value needs to be within to register a min or max normalized position value.

### Class Variables

 * `public Vector3 localOrigin` - The default local position of the Interactable Object.

### Class Events

 * `TransformPositionChanged` - Emitted when the Transform position has changed.
 * `XAxisMinLimitReached` - Emitted when the Transform position has reached the X Axis Min Limit.
 * `XAxisMinLimitExited` - Emitted when the Transform position has exited the X Axis Min Limit.
 * `XAxisMaxLimitReached` - Emitted when the Transform position has reached the X Axis Max Limit.
 * `XAxisMaxLimitExited` - Emitted when the Transform position has exited the X Axis Max Limit.
 * `YAxisMinLimitReached` - Emitted when the Transform position has reached the Y Axis Min Limit.
 * `YAxisMinLimitExited` - Emitted when the Transform position has exited the Y Axis Min Limit.
 * `YAxisMaxLimitReached` - Emitted when the Transform position has reached the Y Axis Max Limit.
 * `YAxisMaxLimitExited` - Emitted when the Transform position has exited the Y Axis Max Limit.
 * `ZAxisMinLimitReached` - Emitted when the Transform position has reached the Z Axis Min Limit.
 * `ZAxisMinLimitExited` - Emitted when the Transform position has exited the Z Axis Min Limit.
 * `ZAxisMaxLimitReached` - Emitted when the Transform position has reached the Z Axis Max Limit.
 * `ZAxisMaxLimitExited` - Emitted when the Transform position has exited the Z Axis Max Limit.

### Unity Events

Adding the `VRTK_MoveTransformGrabAttach_UnityEvents` component to `VRTK_MoveTransformGrabAttach` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject interactingObject` - The GameObject that is performing the interaction (e.g. a controller).
 * `Vector3 position` - The current position in relation to the axis limits from the origin position.
 * `Vector3 normalizedPosition` - The normalized position (between `0f` and `1f`) of the Interactable Object in relation to the axis limits.
 * `Vector3 currentDirection` - The direction vector that the Interactable Object is currently moving across the axes in.
 * `Vector3 originDirection` - The direction vector that the Interactable Object is currently moving across the axes in in relation to the origin position.

### Class Methods

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### CreateTrackPoint/4

  > `public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)`

 * Parameters
   * `Transform controllerPoint` - The point on the controller where the grab was initiated.
   * `GameObject currentGrabbedObject` - The GameObject that is currently being grabbed.
   * `GameObject currentGrabbingObject` - The GameObject that is currently doing the grabbing.
   * `ref bool customTrackPoint` - A reference to whether the created track point is an auto generated custom object.
 * Returns
   * `Transform` - The Transform of the created track point.

The CreateTrackPoint method sets up the point of grab to track on the grabbed object.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the Interactable Object.

#### GetPosition/0

  > `public virtual Vector3 GetPosition()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A Vector3 containing the current Transform position in relation to the axis limits.

The GetPosition method returns a Vector3 of the Transform position in relation to the axis limits.

#### GetNormalizedPosition/0

  > `public virtual Vector3 GetNormalizedPosition()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A normalized Vector3 of the Transform position in relation to the axis limits.

The GetNormalizedPosition method returns a Vector3 of the Transform position normalized between `0f` and `1f` in relation to the axis limits.;

#### GetCurrentDirection/0

  > `public virtual Vector3 GetCurrentDirection()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A Vector3 of the direction the Transform is moving across the relevant axis in.

The GetCurrentDirection method returns a Vector3 of the current positive/negative axis direction that the Transform is moving in.

#### GetDirectionFromOrigin/0

  > `public virtual Vector3 GetDirectionFromOrigin()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A Vector3 of the direction the Transform is moving across the relevant axis in relation to the original position.

The GetDirectionFromOrigin method returns a Vector3 of the direction across the axis from the original position.

#### SetCurrentPosition/2

  > `public virtual void SetCurrentPosition(Vector3 newPosition, float speed)`

 * Parameters
   * `Vector3 newPosition` - The position to move the Interactable Object to.
   * `float speed` - The speed in which to move the Interactable Object.
 * Returns
   * _none_

The SetCurrentPosition method sets the position of the Interactable Object to the given new position at the appropriate speed.

#### ResetPosition/0

  > `public virtual void ResetPosition()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetPosition method will move the Interactable Object back to the origin position.

#### GetWorldLimits/0

  > `public virtual Limits2D[] GetWorldLimits()`

 * Parameters
   * _none_
 * Returns
   * `Limits2D[]` - An array of axis limits in world space.

The GetWorldLimits method returns an array of minimum and maximum axis limits for the Interactable Object in world space.

---

## Rotate Transform Grab Attach (VRTK_RotateTransformGrabAttach)
 > extends [VRTK_BaseGrabAttach](#base-grab-attach-vrtk_basegrabattach)

### Overview

Rotates the Transform of the Interactable Object around a specified transform local axis within the given limits.

  > To allow unrestricted movement, set the angle limits minimum to `-infinity` and the angle limits maximum to `infinity`.

**Script Usage:**
 * Place the `VRTK_RotateTransformGrabAttach` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.

### Inspector Parameters

 * **Detach Distance:** The maximum distance the grabbing object is away from the Interactable Object before it is automatically dropped.
 * **Origin Deadzone:** The distance between grabbing object and the centre of Interactable Object that is considered to be non grabbable. If the grabbing object is within the `Origin Deadzone` distance then it will be automatically ungrabbed.
 * **Rotate Around:** The local axis in which to rotate the object around.
 * **Rotation Action:** Determines how the rotation of the object is calculated based on the action of the grabbing object.
 * **Rotation Friction:** The amount of friction to apply when rotating, simulates a tougher rotation.
 * **Release Deceleration Damper:** The damper in which to slow the Interactable Object's rotation down when released to simulate continued momentum. The higher the number, the faster the Interactable Object will come to a complete stop on release.
 * **Reset To Orign On Release Speed:** The speed in which the Interactable Object returns to it's origin rotation when released. If the `Reset To Orign On Release Speed` is `0f` then the rotation will not be reset.
 * **Angle Limits:** The negative and positive limits the axis can be rotated to.
 * **Min Max Threshold:** The threshold the rotation value needs to be within to register a min or max rotation value.
 * **Min Max Normalized Threshold:** The threshold the normalized rotation value needs to be within to register a min or max normalized rotation value.

### Class Variables

 * `public enum RotationAxis` - The local axis for rotation.
   * `xAxis` - The local X Axis of the transform.
   * `yAxis` - The local Y Axis of the transform.
   * `zAxis` - The local Z Axis of the transform.
 * `public enum RotationType` - The way in which rotation from the grabbing object is applied.
   * `FollowAttachPoint` - The angle between the Interactable Object origin and the grabbing object attach point.
   * `FollowLongitudinalAxis` - The angular velocity across the grabbing object's longitudinal axis (the roll axis).
   * `FollowLateralAxis` - The angular velocity across the grabbing object's lateral axis (the pitch axis).
   * `FollowPerpendicularAxis` - The angular velocity across the grabbing object's perpendicular axis (the yaw axis).
 * `public Quaternion originRotation` - The default local rotation of the Interactable Object.

### Class Events

 * `AngleChanged` - Emitted when the angle changes.
 * `MinAngleReached` - Emitted when the angle reaches the minimum angle.
 * `MinAngleExited` - Emitted when the angle exits the minimum angle state.
 * `MaxAngleReached` - Emitted when the angle reaches the maximum angle.
 * `MaxAngleExited` - Emitted when the angle exits the maximum angle state.

### Unity Events

Adding the `VRTK_RotateTransformGrabAttach_UnityEvents` component to `VRTK_RotateTransformGrabAttach` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject interactingObject` - The GameObject that is performing the interaction (e.g. a controller).
 * `float currentAngle` - The current angle the Interactable Object is rotated to.
 * `float normalizedAngle` - The normalized angle (between `0f` and `1f`) the Interactable Object is rotated to.
 * `Vector3 rotationSpeed` - The speed in which the rotation is occuring.

### Class Methods

#### StartGrab/3

  > `public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)`

 * Parameters
   * `GameObject grabbingObject` - The GameObject that is doing the grabbing.
   * `GameObject givenGrabbedObject` - The GameObject that is being grabbed.
   * `Rigidbody givenControllerAttachPoint` - The point on the grabbing object that the grabbed object should be attached to after grab occurs.
 * Returns
   * `bool` - Returns `true` if the grab is successful, `false` if the grab is unsuccessful.

The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.

#### StopGrab/1

  > `public override void StopGrab(bool applyGrabbingObjectVelocity)`

 * Parameters
   * `bool applyGrabbingObjectVelocity` - If `true` will apply the current velocity of the grabbing object to the grabbed object on release.
 * Returns
   * _none_

The StopGrab method ends the grab of the current Interactable Object and cleans up the state.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method is run in every Update method on the Interactable Object.

#### SetRotation/2

  > `public virtual void SetRotation(float newAngle, float transitionTime = 0f)`

 * Parameters
   * `float newAngle` - The angle to rotate to through the current rotation axis.
   * `float transitionTime` - The time in which the entire rotation operation will take place.
 * Returns
   * _none_

The SetRotation method sets the rotation on the Interactable Object to the given angle over the desired time.

#### ResetRotation/1

  > `public virtual void ResetRotation(bool ignoreTransition = false)`

 * Parameters
   * `bool ignoreTransition` - If this is `true` then the `Reset To Origin On Release Speed` will be ignored and the reset will occur instantly.
 * Returns
   * _none_

The ResetRotation method will rotate the Interactable Object back to the origin rotation.

#### GetAngle/0

  > `public virtual float GetAngle()`

 * Parameters
   * _none_
 * Returns
   * `float` - The current rotated angle.

The GetAngle method returns the current angle the Interactable Object is rotated to.

#### GetNormalizedAngle/0

  > `public virtual float GetNormalizedAngle()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized rotated angle. Will return `0f` if either limit is set to `infinity`.

The GetNormalizedAngle returns the normalized current angle between the minimum and maximum angle limits.

#### GetRotationSpeed/0

  > `public virtual Vector3 GetRotationSpeed()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A Vector3 containing the speed each axis is rotating in.

The GetRotationSpeed returns the current speed in which the Interactable Object is rotating.

---

# Secondary Controller Grab Actions (VRTK/Source/Scripts/Interactions/SecondaryControllerGrabActions)

A collection of scripts that are used to provide different actions when a secondary controller grabs a grabbed object.

 * [Base Grab Action](#base-grab-action-vrtk_basegrabaction)
 * [Swap Controller Grab Action](#swap-controller-grab-action-vrtk_swapcontrollergrabaction)
 * [Axis Scale Grab Action](#axis-scale-grab-action-vrtk_axisscalegrabaction)
 * [Control Direction Grab Action](#control-direction-grab-action-vrtk_controldirectiongrabaction)

---

## Base Grab Action (VRTK_BaseGrabAction)

### Overview

Provides a base that all secondary controller grab attach can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides secondary controller grab action functionality, therefore this script should not be directly used.

### Class Methods

#### Initialise/5

  > `public virtual void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

 * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary grabbing object.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary grabbing object.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary grabbing object.
   * `Transform primaryGrabPoint` - The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.
   * `Transform secondaryGrabPoint` - The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.
 * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.

#### ResetAction/0

  > `public virtual void ResetAction()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetAction method is used to reset the secondary action when the Interactable Object is no longer grabbed by a secondary Interact Grab.

#### IsActionable/0

  > `public virtual bool IsActionable()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the secondary grab action does perform an action on secondary grab.

The IsActionable method is used to determine if the secondary grab action performs an action on grab.

#### IsSwappable/0

  > `public virtual bool IsSwappable()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the grab action allows swapping to another grabbing object.

The IsSwappable method is used to determine if the secondary grab action allows to swab the grab state to another grabbing Interactable Object.

#### ProcessUpdate/0

  > `public virtual void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.

#### ProcessFixedUpdate/0

  > `public virtual void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.

#### OnDropAction/0

  > `public virtual void OnDropAction()`

 * Parameters
   * _none_
 * Returns
   * _none_

The OnDropAction method is executed when the current grabbed Interactable Object is dropped and can be used up to clean up any secondary grab actions.

---

## Swap Controller Grab Action (VRTK_SwapControllerGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

Swaps the grabbed Interactable Object to the new grabbing object.

**Script Usage:**
 * Place the `VRTK_SwapControllerGrabAction` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.

### Example

`VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the ability to swap objects between controllers on grab.

---

## Axis Scale Grab Action (VRTK_AxisScaleGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

Scales the grabbed Interactable Object along the given axes based on the position of the secondary grabbing Interact Grab.

**Script Usage:**
 * Place the `VRTK_AxisScaleGrabAction` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.

### Inspector Parameters

 * **Ungrab Distance:** The distance the secondary grabbing object must move away from the original grab position before the secondary grabbing object auto ungrabs the Interactable Object.
 * **Lock Axis:** Locks the specified checked axes so they won't be scaled
 * **Uniform Scaling:** If checked all the axes will be scaled together (unless locked)

### Class Methods

#### Initialise/5

  > `public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

 * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary grabbing object.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary grabbing object.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary grabbing object.
   * `Transform primaryGrabPoint` - The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.
   * `Transform secondaryGrabPoint` - The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.
 * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab and performs the scaling action.

### Example

`VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.

---

## Control Direction Grab Action (VRTK_ControlDirectionGrabAction)
 > extends [VRTK_BaseGrabAction](#base-grab-action-vrtk_basegrabaction)

### Overview

Controls the facing direction of the grabbed Interactable Object to rotate in the direction of the secondary grabbing object.

  > Rotation will only occur correctly if the Interactable Object `forward` is correctly aligned to the world `z-axis` and the `up` is correctly aligned to the world `y-axis`. It is also not possible to control the direction of an Interactable Object that uses the Joint based grab mechanics.

**Script Usage:**
 * Place the `VRTK_ControlDirectionGrabAction` script on either:
   * The GameObject of the Interactable Object to detect interactions on.
   * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.

### Inspector Parameters

 * **Ungrab Distance:** The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.
 * **Release Snap Speed:** The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.
 * **Lock Z Rotation:** Prevent the secondary controller rotating the grabbed object through it's z-axis.

### Class Methods

#### Initialise/5

  > `public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)`

 * Parameters
   * `VRTK_InteractableObject currentGrabbdObject` - The Interactable Object script for the object currently being grabbed by the primary grabbing object.
   * `VRTK_InteractGrab currentPrimaryGrabbingObject` - The Interact Grab script for the object that is associated with the primary grabbing object.
   * `VRTK_InteractGrab currentSecondaryGrabbingObject` - The Interact Grab script for the object that is associated with the secondary grabbing object.
   * `Transform primaryGrabPoint` - The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.
   * `Transform secondaryGrabPoint` - The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.
 * Returns
   * _none_

The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.

#### ResetAction/0

  > `public override void ResetAction()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetAction method is used to reset the secondary action when the Interactable Object is no longer grabbed by a secondary Interact Grab.

#### OnDropAction/0

  > `public override void OnDropAction()`

 * Parameters
   * _none_
 * Returns
   * _none_

The OnDropAction method is executed when the current grabbed Interactable Object is dropped and can be used up to clean up any secondary grab actions.

#### ProcessUpdate/0

  > `public override void ProcessUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.

#### ProcessFixedUpdate/0

  > `public override void ProcessFixedUpdate()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab and influences the rotation of the Interactable Object.

### Example

`VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and control their direction with the second controller.

---

# Controllables (VRTK/Source/Scripts/Interactions/Controllables)

Contains scripts that form the basis of interactable 3D controls that are either Physics based or artificially simulated.

 * [Base Controllable](#base-controllable-vrtk_basecontrollable)

---

## Base Controllable (VRTK_BaseControllable)

### Overview

Provides a base that all Controllables can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides controllable functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Operate Axis:** The local axis in which the Controllable will operate through.
 * **Ignore Collisions With:** A collection of GameObjects to ignore collision events with.
 * **Exclude Collider Check On:** A collection of GameObjects to exclude when determining if a default collider should be created.
 * **Equality Fidelity:** The amount of fidelity when comparing the position of the control with the previous position. Determines if it's equal above a certain decimal place threshold.

### Class Variables

 * `public enum OperatingAxis` - The local axis that the Controllable will be operated through.
   * `xAxis` - The local x axis.
   * `yAxis` - The local y axis.
   * `zAxis` - The local z axis.

### Class Events

 * `ValueChanged` - Emitted when the Controllable value has changed.
 * `RestingPointReached` - Emitted when the Controllable value has reached the resting point.
 * `MinLimitReached` - Emitted when the Controllable value has reached the minimum limit.
 * `MinLimitExited` - Emitted when the Controllable value has exited the minimum limit.
 * `MaxLimitReached` - Emitted when the Controllable value has reached the maximum limit.
 * `MaxLimitExited` - Emitted when the Controllable value has exited the maximum limit.

### Unity Events

Adding the `VRTK_BaseControllable_UnityEvents` component to `VRTK_BaseControllable` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `Collider interactingCollider` - The Collider that is initiating the interaction.
 * `VRTK_InteractTouch interactingTouchScript` - The optional Interact Touch script that is initiating the interaction.
 * `float value` - The current value being reported by the controllable.
 * `float normalizedValue` - The normalized value being reported by the controllable.

### Class Methods

#### AtMinLimit/0

  > `public virtual bool AtMinLimit()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Controllable is at it's minimum limit.

The AtMinLimit method returns whether the Controllable is currently at it's minimum limit.

#### AtMaxLimit/0

  > `public virtual bool AtMaxLimit()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the Controllable is at it's maximum limit.

The AtMaxLimit method returns whether the Controllable is currently at it's maximum limit.

#### GetOriginalLocalPosition/0

  > `public virtual Vector3 GetOriginalLocalPosition()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - A Vector3 of the original local position.

The GetOriginalLocalPosition method returns the original local position of the control.

#### GetOriginalLocalRotation/0

  > `public virtual Quaternion GetOriginalLocalRotation()`

 * Parameters
   * _none_
 * Returns
   * `Quaternion` - A quaternion of the original local rotation.

The GetOriginalLocalRotation method returns the original local rotation of the control.

#### GetControlColliders/0

  > `public virtual Collider[] GetControlColliders()`

 * Parameters
   * _none_
 * Returns
   * `Collider[]` - The Colliders array associated with the control.

The GetControlColliders method returns the Colliders array associated with the control.

#### GetInteractingCollider/0

  > `public virtual Collider GetInteractingCollider()`

 * Parameters
   * _none_
 * Returns
   * `Collider` - The Collider currently interacting with the control.

The GetInteractingCollider method returns the Collider of the GameObject currently interacting with the control.

#### GetInteractingTouch/0

  > `public virtual VRTK_InteractTouch GetInteractingTouch()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractTouch` - The Interact Touch script currently interacting with the control.

The GetInteractingTouch method returns the Interact Touch script of the GameObject currently interacting with the control.

---

# Physics Controllables (VRTK/Source/Scripts/Interactions/Controllables/Physics)

A collection of scripts that provide physics based controls that mimiic real life objects.

 * [Base Physics Controllable](#base-physics-controllable-vrtk_basephysicscontrollable)
 * [Physics Pusher](#physics-pusher-vrtk_physicspusher)
 * [Physics Rotator](#physics-rotator-vrtk_physicsrotator)
 * [Physics Slider](#physics-slider-vrtk_physicsslider)

---

## Base Physics Controllable (VRTK_BasePhysicsControllable)
 > extends [VRTK_BaseControllable](#base-controllable-vrtk_basecontrollable)

### Overview

Provides a base that all physics based Controllables can inherit from.

**Script Usage:**
  > This is an abstract class that is to be inherited to a concrete class that provides physics based controllable functionality, therefore this script should not be directly used.

### Inspector Parameters

 * **Connected To:** The Rigidbody that the Controllable is connected to.

### Class Methods

#### GetControlRigidbody/0

  > `public virtual Rigidbody GetControlRigidbody()`

 * Parameters
   * _none_
 * Returns
   * `Rigidbody` - The Rigidbody associated with the control.

The GetControlRigidbody method returns the rigidbody associated with the control.

#### GetControlActivatorContainer/0

  > `public virtual GameObject GetControlActivatorContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject that contains the Controller Rigidbody Activator associated with the control.

The GetControlActivatorContainer method returns the GameObject that contains the Controller Rigidbody Activator associated with the control.

---

## Physics Pusher (VRTK_PhysicsPusher)
 > extends [VRTK_BasePhysicsControllable](#base-physics-controllable-vrtk_basephysicscontrollable)

### Overview

A physics based pushable pusher.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
 * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System. Will be automatically added at runtime.

**Optional Components:**
 * `VRTK_ControllerRigidbodyActivator` - A Controller Rigidbody Activator to automatically enable the controller rigidbody upon touching the pusher.

**Script Usage:**
 * Create a pusher container GameObject and set the GameObject that is to become the pusher as a child of the newly created container GameObject.
 * Place the `VRTK_PhysicsPusher` script onto the GameObject that is to become the pusher.

  > The Physics Pusher script must not be on a root level GameObject. Any runtime world positioning of the pusher must be set on the parent container GameObject.

### Inspector Parameters

 * **Pressed Distance:** The local space distance along the `Operate Axis` until the pusher reaches the pressed position.
 * **Stay Pressed:** If this is checked then the pusher will stay in the pressed position when it reaches the maximum position.
 * **Min Max Limit Threshold:** The threshold in which the pusher's current normalized position along the `Operate Axis` has to be within the minimum and maximum limits of the pusher.
 * **Resting Position:** The normalized position of the pusher between the original position and the pressed position that will be considered as the resting position for the pusher.
 * **Resting Position Threshold:** The normalized value that the pusher can be from the `Resting Position` before the pusher is considered to be resting when not being interacted with.
 * **Position Target:** The normalized position of the pusher between the original position and the pressed position. `0f` will set the pusher position to the original position, `1f` will set the pusher position to the pressed position.
 * **Target Force:** The amount of force to apply to push the pusher towards the intended target position.

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual position of the pusher.

The GetValue method returns the current position value of the pusher.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized position of the pusher.

The GetNormalizedValue method returns the current position value of the pusher normalized between `0f` and `1f`.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the pusher is currently at the resting position.

The IsResting method returns whether the pusher is currently at it's resting position.

#### GetControlJoint/0

  > `public virtual ConfigurableJoint GetControlJoint()`

 * Parameters
   * _none_
 * Returns
   * `ConfigurableJoint` - The joint associated with the control.

The GetControlJoint method returns the joint associated with the control.

---

## Physics Rotator (VRTK_PhysicsRotator)
 > extends [VRTK_BasePhysicsControllable](#base-physics-controllable-vrtk_basephysicscontrollable)

### Overview

A physics based rotatable object.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
 * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System. Will be automatically added at runtime.

**Optional Components:**
 * `VRTK_ControllerRigidbodyActivator` - A Controller Rigidbody Activator to automatically enable the controller rigidbody when near the rotator.

**Script Usage:**
 * Create a rotator container GameObject and set the GameObject that is to become the rotator as a child of the newly created container GameObject.
 * Place the `VRTK_PhysicsRotator` script onto the GameObject that is to become the rotatable object and ensure the Transform rotation is `0, 0, 0`.
 * Create a nested GameObject under the rotator GameObject and position it where the hinge should operate.
 * Apply the nested hinge GameObject to the `Hinge Point` parameter on the Physics Rotator script.

  > The rotator GameObject must not be at the root level and needs to have the Transform rotation set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the rotator must be set on the parent container GameObject.

### Inspector Parameters

 * **Hinge Point:** A Transform that denotes the position where the rotator hinge will be created.
 * **Angle Limits:** The minimum and maximum angle the rotator can rotate to.
 * **Min Max Threshold Angle:** The angle at which the rotator rotation can be within the minimum or maximum angle before the minimum or maximum angles are considered reached.
 * **Resting Angle:** The angle at which will be considered as the resting position of the rotator.
 * **Force Resting Angle Threshold:** The threshold angle from the `Resting Angle` that the current angle of the rotator needs to be within to snap the rotator back to the `Resting Angle`.
 * **Angle Target:** The target angle to rotate the rotator to.
 * **Is Locked:** If this is checked then the rotator Rigidbody will have all rotations frozen.
 * **Step Value Range:** The minimum and the maximum step values for the rotator to register along the `Operate Axis`.
 * **Step Size:** The increments the rotator value will change in between the `Step Value Range`.
 * **Use Step As Value:** If this is checked then the value for the rotator will be the step value and not the absolute rotation of the rotator Transform.
 * **Snap To Step:** If this is checked then the rotator will snap to the angle of the nearest step along the value range.
 * **Snap Force:** The speed in which the rotator will snap to the relevant angle along the `Operate Axis`
 * **Grab Mechanic:** The type of Interactable Object grab mechanic to use when operating the rotator.
 * **Precision Grab:** If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.
 * **Detach Distance:** The maximum distance the grabbing object is away from the rotator before it is automatically released.
 * **Use Friction Overrides:** If this is checked then the `Grabbed Friction` value will be used as the Rigidbody drag value when the rotator is grabbed and the `Released Friction` value will be used as the Rigidbody drag value when the door is released.
 * **Grabbed Friction:** The Rigidbody drag value when the rotator is grabbed.
 * **Released Friction:** The Rigidbody drag value when the rotator is released.
 * **Only Interact With:** A collection of GameObjects that will be used as the valid collisions to determine if the rotator can be interacted with.

### Class Variables

 * `public enum GrabMechanic` - Type of Grab Mechanic
   * `TrackObject` - The Track Object Grab Mechanic
   * `RotatorTrack` - The Rotator Track Grab Mechanic

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual rotation of the rotator.

The GetValue method returns the current rotation value of the rotator.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized rotation of the rotator.

The GetNormalizedValue method returns the current rotation value of the rotator normalized between `0f` and `1f`.

#### GetStepValue/1

  > `public virtual float GetStepValue(float currentValue)`

 * Parameters
   * `float currentValue` - The current angle value of the rotator to get the Step Value for.
 * Returns
   * `float` - The current Step Value based on the rotator angle.

The GetStepValue method returns the current angle of the rotator based on the step value range.

#### SetAngleTargetWithStepValue/1

  > `public virtual void SetAngleTargetWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Angle Target` parameter to.
 * Returns
   * _none_

The SetAngleTargetWithStepValue sets the `Angle Target` parameter but uses a value within the `Step Value Range`.

#### SetRestingAngleWithStepValue/1

  > `public virtual void SetRestingAngleWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Resting Angle` parameter to.
 * Returns
   * _none_

The SetRestingAngleWithStepValue sets the `Resting Angle` parameter but uses a value within the `Step Value Range`.

#### GetAngleFromStepValue/1

  > `public virtual float GetAngleFromStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value to check the angle for.
 * Returns
   * `float` - The angle the rotator would be at based on the given step value.

The GetAngleFromStepValue returns the angle the rotator would be at based on the given step value.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the rotator is at the resting angle or within the resting angle threshold.

The IsResting method returns whether the rotator is at the resting angle or within the resting angle threshold.

#### GetControlJoint/0

  > `public virtual HingeJoint GetControlJoint()`

 * Parameters
   * _none_
 * Returns
   * `HingeJoint` - The joint associated with the control.

The GetControlJoint method returns the joint associated with the control.

#### GetControlInteractableObject/0

  > `public virtual VRTK_InteractableObject GetControlInteractableObject()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractableObject` - The Interactable Object associated with the control.

The GetControlInteractableObject method returns the Interactable Object associated with the control.

---

## Physics Slider (VRTK_PhysicsSlider)
 > extends [VRTK_BasePhysicsControllable](#base-physics-controllable-vrtk_basephysicscontrollable)

### Overview

A physics based slider.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
 * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System. Will be automatically added at runtime.

**Optional Components:**
 * `VRTK_ControllerRigidbodyActivator` - A Controller Rigidbody Activator to automatically enable the controller rigidbody when near the slider.

**Script Usage:**
 * Create a slider container GameObject and set the GameObject that is to become the slider as a child of the container.
 * Place the `VRTK_PhysicsSlider` script onto the GameObject that is to become the slider.

  > The slider GameObject must not be at the root level and needs to have it's Transform position set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the slider must be set on the parent GameObject.

### Inspector Parameters

 * **Maximum Length:** The maximum length that the slider can be moved from the origin position across the `Operate Axis`. A negative value will allow it to move the opposite way.
 * **Min Max Threshold:** The normalized position the slider can be within the minimum or maximum slider positions before the minimum or maximum positions are considered reached.
 * **Position Target:** The target position to move the slider towards given in a normalized value of `0f` (start point) to `1f` (end point).
 * **Resting Position:** The position the slider when it is at the default resting point given in a normalized value of `0f` (start point) to `1f` (end point).
 * **Force Resting Position Threshold:** The normalized threshold value the slider has to be within the `Resting Position` before the slider is forced back to the `Resting Position` if it is not grabbed.
 * **Step Value Range:** The minimum and the maximum step values for the slider to register along the `Operate Axis`.
 * **Step Size:** The increments the slider value will change in between the `Step Value Range`.
 * **Use Step As Value:** If this is checked then the value for the slider will be the step value and not the absolute position of the slider Transform.
 * **Snap To Step:** If this is checked then the slider will snap to the position of the nearest step along the value range.
 * **Snap Force:** The speed in which the slider will snap to the relevant point along the `Operate Axis`
 * **Precision Grab:** If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.
 * **Detach Distance:** The maximum distance the grabbing object is away from the slider before it is automatically released.
 * **Release Friction:** The amount of friction to the slider Rigidbody when it is released.
 * **Only Interact With:** A collection of GameObjects that will be used as the valid collisions to determine if the door can be interacted with.

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual position of the button.

The GetValue method returns the current position value of the slider.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized position of the button.

The GetNormalizedValue method returns the current position value of the slider normalized between `0f` and `1f`.

#### GetStepValue/1

  > `public virtual float GetStepValue(float currentValue)`

 * Parameters
   * `float currentValue` - The current position value of the slider to get the Step Value for.
 * Returns
   * `float` - The current Step Value based on the slider position.

The GetStepValue method returns the current position of the slider based on the step value range.

#### SetPositionTargetWithStepValue/1

  > `public virtual void SetPositionTargetWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Position Target` parameter to.
 * Returns
   * _none_

The SetTargetPositionWithStepValue sets the `Position Target` parameter but uses a value within the `Step Value Range`.

#### SetRestingPositionWithStepValue/1

  > `public virtual void SetRestingPositionWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Resting Position` parameter to.
 * Returns
   * _none_

The SetRestingPositionWithStepValue sets the `Resting Position` parameter but uses a value within the `Step Value Range`.

#### GetPositionFromStepValue/1

  > `public virtual float GetPositionFromStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value to check the position for.
 * Returns
   * `float` - The position the slider would be at based on the given step value.

The GetPositionFromStepValue returns the position the slider would be at based on the given step value.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the slider is at the resting position or within the resting position threshold.

The IsResting method returns whether the slider is currently in a resting state at the resting position or within the resting position threshold and not grabbed.

#### GetControlJoint/0

  > `public virtual ConfigurableJoint GetControlJoint()`

 * Parameters
   * _none_
 * Returns
   * `ConfigurableJoint` - The joint associated with the control.

The GetControlJoint method returns the joint associated with the control.

#### GetControlInteractableObject/0

  > `public virtual VRTK_InteractableObject GetControlInteractableObject()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractableObject` - The Interactable Object associated with the control.

The GetControlInteractableObject method returns the Interactable Object associated with the control.

---

# Artificial Controllables (VRTK/Source/Scripts/Interactions/Controllables/Artificial)

A collection of scripts that provide artificial simulated controls that mimiic real life objects.

 * [Artificial Pusher](#artificial-pusher-vrtk_artificialpusher)
 * [Artificial Rotator](#artificial-rotator-vrtk_artificialrotator)
 * [Artificial Slider](#artificial-slider-vrtk_artificialslider)

---

## Artificial Pusher (VRTK_ArtificialPusher)
 > extends [VRTK_BaseControllable](#base-controllable-vrtk_basecontrollable)

### Overview

An artificially simulated pushable pusher.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.

**Script Usage:**
 * Place the `VRTK_ArtificialPusher` script onto the GameObject that is to become the pusher.

### Inspector Parameters

 * **Pressed Distance:** The distance along the `Operate Axis` until the pusher reaches the pressed position.
 * **Min Max Limit Threshold:** The threshold in which the pusher's current normalized position along the `Operate Axis` has to be within the minimum and maximum limits of the pusher.
 * **Resting Position:** The normalized position of the pusher between the original position and the pressed position that will be considered as the resting position for the pusher.
 * **Resting Position Threshold:** The normalized value that the pusher can be from the `Resting Position` before the pusher is considered to be resting when not being interacted with.
 * **Press Speed:** The speed in which the pusher moves towards to the `Pressed Distance` position.
 * **Return Speed:** The speed in which the pusher will return to the `Target Position` of the pusher.

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual position of the pusher.

The GetValue method returns the current position value of the pusher.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized position of the pusher.

The GetNormalizedValue method returns the current position value of the pusher normalized between `0f` and `1f`.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the pusher is currently at the resting position.

The IsResting method returns whether the pusher is currently at it's resting position.

#### SetStayPressed/1

  > `public virtual void SetStayPressed(bool state)`

 * Parameters
   * `bool state` - The state to set the `Stay Pressed` parameter to.
 * Returns
   * _none_

The SetStayPressed method sets the `Stay Pressed` parameter to the given state and if the state is false and the pusher is currently pressed then it is reset to the original position.

#### SetPositionTarget/1

  > `public virtual void SetPositionTarget(float normalizedTarget)`

 * Parameters
   * `float normalizedTarget` - The `Position Target` to set the pusher to between `0f` and `1f`.
 * Returns
   * _none_

The SetPositionTarget method sets the `Position Target` parameter to the given normalized value.

---

## Artificial Rotator (VRTK_ArtificialRotator)
 > extends [VRTK_BaseControllable](#base-controllable-vrtk_basecontrollable)

### Overview

A artificially simulated openable rotator.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.

**Script Usage:**
 * Create a rotator container GameObject and set the GameObject that is to become the rotator as a child of the newly created container GameObject.
 * Place the `VRTK_ArtificialRotator` script onto the GameObject that is to become the rotatable object and ensure the Transform rotation is `0, 0, 0`.
 * Create a nested GameObject under the rotator GameObject and position it where the hinge should operate.
 * Apply the nested hinge GameObject to the `Hinge Point` parameter on the Artificial Rotator script.

  > The rotator GameObject must not be at the root level and needs to have the Transform rotation set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the rotator must be set on the parent container GameObject.
  > The Artificial Rotator script GameObject will become the child of a runtime created GameObject that determines the rotational offset for the rotator.

### Inspector Parameters

 * **Hinge Point:** A Transform that denotes the position where the rotator will rotate around.
 * **Angle Limits:** The minimum and maximum angle the rotator can rotate to.
 * **Min Max Threshold Angle:** The angle at which the rotator rotation can be within the minimum or maximum angle before the minimum or maximum angles are considered reached.
 * **Resting Angle:** The angle at which will be considered as the resting position of the rotator.
 * **Force Resting Angle Threshold:** The threshold angle from the `Resting Angle` that the current angle of the rotator needs to be within to snap the rotator back to the `Resting Angle`.
 * **Is Locked:** If this is checked then the rotator Rigidbody will have all rotations frozen.
 * **Step Value Range:** The minimum and the maximum step values for the rotator to register along the `Operate Axis`.
 * **Step Size:** The increments the rotator value will change in between the `Step Value Range`.
 * **Use Step As Value:** If this is checked then the value for the rotator will be the step value and not the absolute rotation of the rotator Transform.
 * **Snap To Step:** If this is checked then the rotator will snap to the angle of the nearest step along the value range.
 * **Snap Force:** The speed in which the rotator will snap to the relevant angle along the `Operate Axis`
 * **Precision Grab:** If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.
 * **Detach Distance:** The maximum distance the grabbing object is away from the rotator before it is automatically released.
 * **Rotation Action:** Determines how the rotation of the object is calculated based on the action of the grabbing object.
 * **Grabbed Friction:** The simulated friction when the rotator is grabbed.
 * **Released Friction:** The simulated friction when the rotator is released.
 * **Only Interact With:** A collection of GameObjects that will be used as the valid collisions to determine if the rotator can be interacted with.

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual rotation of the rotator.

The GetValue method returns the current rotation value of the rotator.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized rotation of the rotator.

The GetNormalizedValue method returns the current rotation value of the rotator normalized between `0f` and `1f`.

#### GetContainer/0

  > `public virtual GameObject GetContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject container of the rotator control.

The GetContainer method returns the GameObject that is generated to hold the rotator control.

#### GetStepValue/1

  > `public virtual float GetStepValue(float currentValue)`

 * Parameters
   * `float currentValue` - The current angle value of the rotator to get the Step Value for.
 * Returns
   * `float` - The current Step Value based on the rotator angle.

The GetStepValue method returns the current angle of the rotator based on the step value range.

#### SetAngleTargetWithStepValue/1

  > `public virtual void SetAngleTargetWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Angle Target` parameter to.
 * Returns
   * _none_

The SetAngleTargetWithStepValue sets the `Angle Target` parameter but uses a value within the `Step Value Range`.

#### SetRestingAngleWithStepValue/1

  > `public virtual void SetRestingAngleWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Resting Angle` parameter to.
 * Returns
   * _none_

The SetRestingAngleWithStepValue sets the `Resting Angle` parameter but uses a value within the `Step Value Range`.

#### GetAngleFromStepValue/1

  > `public virtual float GetAngleFromStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value to check the angle for.
 * Returns
   * `float` - The angle the rotator would be at based on the given step value.

The GetAngleFromStepValue returns the angle the rotator would be at based on the given step value.

#### SetAngleTarget/1

  > `public virtual void SetAngleTarget(float newAngle)`

 * Parameters
   * `float newAngle` - The angle in which to rotate the rotator to.
 * Returns
   * _none_

The SetAngleTarget method sets a target angle to rotate the rotator to.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the rotator is at the resting angle or within the resting angle threshold.

The IsResting method returns whether the rotator is at the resting angle or within the resting angle threshold.

#### GetControlInteractableObject/0

  > `public virtual VRTK_InteractableObject GetControlInteractableObject()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractableObject` - The Interactable Object associated with the control.

The GetControlInteractableObject method returns the Interactable Object associated with the control.

---

## Artificial Slider (VRTK_ArtificialSlider)
 > extends [VRTK_BaseControllable](#base-controllable-vrtk_basecontrollable)

### Overview

A artificially simulated slider.

**Required Components:**
 * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.

**Script Usage:**
 * Create a slider container GameObject and set the GameObject that is to become the slider as a child of the container.
 * Place the `VRTK_ArtificialSlider` script onto the GameObject that is to become the slider.

  > The slider GameObject must not be at the root level and needs to have it's Transform position set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the slider must be set on the parent GameObject.

### Inspector Parameters

 * **Maximum Length:** The maximum length that the slider can be moved from the origin position across the `Operate Axis`. A negative value will allow it to move the opposite way.
 * **Min Max Threshold:** The normalized position the slider can be within the minimum or maximum slider positions before the minimum or maximum positions are considered reached.
 * **Resting Position:** The position the slider when it is at the default resting point given in a normalized value of `0f` (start point) to `1f` (end point).
 * **Force Resting Position Threshold:** The normalized threshold value the slider has to be within the `Resting Position` before the slider is forced back to the `Resting Position` if it is not grabbed.
 * **Step Value Range:** The minimum and the maximum step values for the slider to register along the `Operate Axis`.
 * **Step Size:** The increments the slider value will change in between the `Step Value Range`.
 * **Use Step As Value:** If this is checked then the value for the slider will be the step value and not the absolute position of the slider Transform.
 * **Snap To Step:** If this is checked then the slider will snap to the position of the nearest step along the value range.
 * **Snap Force:** The speed in which the slider will snap to the relevant point along the `Operate Axis`
 * **Tracking Speed:** The speed in which to track the grabbed slider to the interacting object.
 * **Precision Grab:** If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.
 * **Detach Distance:** The maximum distance the grabbing object is away from the slider before it is automatically released.
 * **Release Friction:** The amount of friction to the slider Rigidbody when it is released.
 * **Only Interact With:** A collection of GameObjects that will be used as the valid collisions to determine if the door can be interacted with.

### Class Methods

#### GetValue/0

  > `public override float GetValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The actual position of the button.

The GetValue method returns the current position value of the slider.

#### GetNormalizedValue/0

  > `public override float GetNormalizedValue()`

 * Parameters
   * _none_
 * Returns
   * `float` - The normalized position of the button.

The GetNormalizedValue method returns the current position value of the slider normalized between `0f` and `1f`.

#### GetStepValue/1

  > `public virtual float GetStepValue(float currentValue)`

 * Parameters
   * `float currentValue` - The current position value of the slider to get the Step Value for.
 * Returns
   * `float` - The current Step Value based on the slider position.

The GetStepValue method returns the current position of the slider based on the step value range.

#### SetPositionTarget/2

  > `public virtual void SetPositionTarget(float newPositionTarget, float speed)`

 * Parameters
   * `float newPositionTarget` - The new position target value.
   * `float speed` - The speed to move to the new position target.
 * Returns
   * _none_

The SetPositionTarget method allows the setting of the `Position Target` parameter at runtime.

#### SetPositionTargetWithStepValue/2

  > `public virtual void SetPositionTargetWithStepValue(float givenStepValue, float speed)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Position Target` parameter to.
   * `float speed` - The speed to move to the new position target.
 * Returns
   * _none_

The SetPositionTargetWithStepValue sets the `Position Target` parameter but uses a value within the `Step Value Range`.

#### SetRestingPositionWithStepValue/1

  > `public virtual void SetRestingPositionWithStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value within the `Step Value Range` to set the `Resting Position` parameter to.
 * Returns
   * _none_

The SetRestingPositionWithStepValue sets the `Resting Position` parameter but uses a value within the `Step Value Range`.

#### GetPositionFromStepValue/1

  > `public virtual float GetPositionFromStepValue(float givenStepValue)`

 * Parameters
   * `float givenStepValue` - The step value to check the position for.
 * Returns
   * `float` - The position the slider would be at based on the given step value.

The GetPositionFromStepValue returns the position the slider would be at based on the given step value.

#### IsResting/0

  > `public override bool IsResting()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the slider is at the resting position or within the resting position threshold.

The IsResting method returns whether the slider is at the resting position or within the resting position threshold.

#### GetControlInteractableObject/0

  > `public virtual VRTK_InteractableObject GetControlInteractableObject()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_InteractableObject` - The Interactable Object associated with the control.

The GetControlInteractableObject method returns the Interactable Object associated with the control.

---

# Presence (VRTK/Source/Scripts/Presence)

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

Denotes when the HMD is colliding with valid geometry.

**Script Usage:**
 * Place the `VRTK_HeadsetCollision` script on any active scene GameObject.

### Inspector Parameters

 * **Ignore Trigger Colliders:** If this is checked then the headset collision will ignore colliders set to `Is Trigger = true`.
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

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `Collider collider` - The Collider of the game object the headset has collided with.
 * `Transform currentTransform` - The current Transform of the object that the Headset Collision Fade script is attached to (Camera).

### Class Methods

#### IsColliding/0

  > `public virtual bool IsColliding()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the headset is currently colliding with a valid game object.

The IsColliding method is used to determine if the headset is currently colliding with a valid game object and returns true if it is and false if it is not colliding with anything or an invalid game object.

#### GetHeadsetColliderContainer/0

  > `public virtual GameObject GetHeadsetColliderContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The auto generated headset collider GameObject.

The GetHeadsetColliderContainer method returns the auto generated GameObject that contains the headset collider.

### Example

`VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.

---

## Headset Fade (VRTK_HeadsetFade)

### Overview

Provides the ability to change the colour of the headset view to a specified colour over a given duration.

**Script Usage:**
 * Place the `VRTK_HeadsetFade` script on any active scene GameObject.

### Class Events

 * `HeadsetFadeStart` - Emitted when the user's headset begins to fade to a given colour.
 * `HeadsetFadeComplete` - Emitted when the user's headset has completed the fade and is now fully at the given colour.
 * `HeadsetUnfadeStart` - Emitted when the user's headset begins to unfade back to a transparent colour.
 * `HeadsetUnfadeComplete` - Emitted when the user's headset has completed unfading and is now fully transparent again.

### Unity Events

Adding the `VRTK_HeadsetFade_UnityEvents` component to `VRTK_HeadsetFade` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `float timeTillComplete` - A float that is the duration for the fade/unfade process has remaining.
 * `Transform currentTransform` - The current Transform of the object that the Headset Fade script is attached to (Camera).

### Class Methods

#### IsFaded/0

  > `public virtual bool IsFaded()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the headset is currently fading or faded.

The IsFaded method returns true if the headset is currently fading or has completely faded and returns false if it is completely unfaded.

#### IsTransitioning/0

  > `public virtual bool IsTransitioning()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the headset is currently in the process of fading or unfading.

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

Initiates a fade of the headset view when a headset collision event is detected.

**Required Components:**
 * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the HMD has collided with valid geometry.
 * `VRTK_HeadsetFade` - A Headset Fade script to alter the visible colour on the HMD view.

**Script Usage:**
 * Place the `VRTK_HeadsetCollisionFade` script on any active scene GameObject.

### Inspector Parameters

 * **Time Till Fade:** The amount of time to wait until a fade occurs.
 * **Blink Transition Speed:** The fade blink speed on collision.
 * **Fade Color:** The colour to fade the headset to on collision.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision Fade.
 * **Headset Collision:** The VRTK Headset Collision script to use when determining headset collisions. If this is left blank then the script will need to be applied to the same GameObject.
 * **Headset Fade:** The VRTK Headset Fade script to use when fading the headset. If this is left blank then the script will need to be applied to the same GameObject.

### Example

`VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.

---

## Headset Controller Aware (VRTK_HeadsetControllerAware)

### Overview

Determines whether the HMD is in line of sight to the controllers or if the headset is directly looking at one of the controllers.

**Script Usage:**
 * Place the `VRTK_HeadsetControllerAware` script on any active scene GameObject.

### Inspector Parameters

 * **Track Left Controller:** If this is checked then the left controller will be checked if items obscure it's path from the headset.
 * **Track Right Controller:** If this is checked then the right controller will be checked if items obscure it's path from the headset.
 * **Controller Glance Radius:** The radius of the accepted distance from the controller origin point to determine if the controller is being looked at.
 * **Custom Right Controller Origin:** A custom transform to provide the world space position of the right controller.
 * **Custom Left Controller Origin:** A custom transform to provide the world space position of the left controller.
 * **Custom Raycast:** A custom raycaster to use when raycasting to find controllers.

### Class Events

 * `ControllerObscured` - Emitted when the controller is obscured by another object.
 * `ControllerUnobscured` - Emitted when the controller is no longer obscured by an object.
 * `ControllerGlanceEnter` - Emitted when the controller is seen by the headset view.
 * `ControllerGlanceExit` - Emitted when the controller is no longer seen by the headset view.

### Unity Events

Adding the `VRTK_HeadsetControllerAware_UnityEvents` component to `VRTK_HeadsetControllerAware` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `RaycastHit raycastHit` - The Raycast Hit struct of item that is obscuring the path to the controller.
 * `VRTK_ControllerReference controllerReference` - The reference to the controller that is being or has been obscured or being or has been glanced.

### Class Methods

#### LeftControllerObscured/0

  > `public virtual bool LeftControllerObscured()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the path between the headset and the controller is obscured.

The LeftControllerObscured method returns the state of if the left controller is being obscured from the path of the headset.

#### RightControllerObscured/0

  > `public virtual bool RightControllerObscured()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the path between the headset and the controller is obscured.

The RightControllerObscured method returns the state of if the right controller is being obscured from the path of the headset.

#### LeftControllerGlanced/0

  > `public virtual bool LeftControllerGlanced()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the headset can currently see the controller within the given radius threshold.

the LeftControllerGlanced method returns the state of if the headset is currently looking at the left controller or not.

#### RightControllerGlanced/0

  > `public virtual bool RightControllerGlanced()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the headset can currently see the controller within the given radius threshold.

the RightControllerGlanced method returns the state of if the headset is currently looking at the right controller or not.

### Example

`VRTK/Examples/029_Controller_Tooltips` displays tooltips that have been added to the controllers and are only visible when the controller is being looked at.

---

## Hip Tracking (VRTK_HipTracking)

### Overview

Attempts to provide the relative position of a hip without the need for additional hardware sensors.

**Script Usage:**
 * Place the `VRTK_HipTracking` script on any active scene GameObject and this GameObject will then track to the estimated hip position.

### Inspector Parameters

 * **Head Offset:** Distance underneath Player Head for hips to reside.
 * **Head Override:** Optional Transform to use as the Head Object for calculating hip position. If none is given one will try to be found in the scene.
 * **Reference Up:** Optional Transform to use for calculating which way is 'Up' relative to the player for hip positioning.

---

## Body Physics (VRTK_BodyPhysics)
 > extends [VRTK_DestinationMarker](#destination-marker-vrtk_destinationmarker)

### Overview

Allows the play area to be affected by physics and detect collisions with other valid geometry.

**Optional Components:**
 * `VRTK_BasicTeleport` - A Teleporter script to use when snapping the play area to the nearest floor when releasing from grab.

**Script Usage:**
 * Place the `VRTK_BodyPhysics` script on any active scene GameObject.

### Inspector Parameters

 * **Enable Body Collisions:** If checked then the body collider and rigidbody will be used to check for rigidbody collisions.
 * **Ignore Grabbed Collisions:** If this is checked then any items that are grabbed with the controller will not collide with the body collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.
 * **Ignore Collisions With:** An array of GameObjects that will not collide with the body collider.
 * **Headset Y Offset:** The collider which is created for the user is set at a height from the user's headset position. If the collider is required to be lower to allow for room between the play area collider and the headset then this offset value will shorten the height of the generated collider.
 * **Movement Threshold:** The amount of movement of the headset between the headset's current position and the current standing position to determine if the user is walking in play space and to ignore the body physics collisions if the movement delta is above this threshold.
 * **Play Area Movement Threshold:** The amount of movement of the play area between the play area's current position and the previous position to determine if the user is moving play space.
 * **Standing History Samples:** The maximum number of samples to collect of headset position before determining if the current standing position within the play space has changed.
 * **Lean Y Threshold:** The `y` distance between the headset and the object being leaned over, if object being leaned over is taller than this threshold then the current standing position won't be updated.
 * **Step Up Y Offset:** The maximum height to consider when checking if an object can be stepped upon to.
 * **Step Thickness Multiplier:** The width/depth of the foot collider in relation to the radius of the body collider.
 * **Step Drop Threshold:** The distance between the current play area Y position and the new stepped up Y position to consider a valid step up. A higher number can help with juddering on slopes or small increases in collider heights.
 * **Custom Raycast:** A custom raycaster to use when raycasting to find floors.
 * **Fall Restriction:** A check to see if the drop to nearest floor should take place. If the selected restrictor is still over the current floor then the drop to nearest floor will not occur. Works well for being able to lean over ledges and look down. Only works for falling down not teleporting up.
 * **Gravity Fall Y Threshold:** When the `y` distance between the floor and the headset exceeds this distance and `Enable Body Collisions` is true then the rigidbody gravity will be used instead of teleport to drop to nearest floor.
 * **Blink Y Threshold:** The `y` distance between the floor and the headset that must change before a fade transition is initiated. If the new user location is at a higher distance than the threshold then the headset blink transition will activate on teleport. If the new user location is within the threshold then no blink transition will happen, which is useful for walking up slopes, meshes and terrains to prevent constant blinking.
 * **Floor Height Tolerance:** The amount the `y` position needs to change by between the current floor `y` position and the previous floor `y` position before a change in floor height is considered to have occurred. A higher value here will mean that a `Drop To Floor` will be less likely to happen if the `y` of the floor beneath the user hasn't changed as much as the given threshold.
 * **Fall Check Precision:** The amount of rounding on the play area Y position to be applied when checking if falling is occuring.
 * **Teleporter:** The VRTK Teleport script to use when snapping to floor. If this is left blank then a Teleport script will need to be applied to the same GameObject.
 * **Custom Play Area Rigidbody:** A custom Rigidbody to apply to the play area. If one is not provided, then if an existing rigidbody is found on the play area GameObject it will be used, otherwise a default one will be created.
 * **Custom Body Collider Container:** A GameObject to represent a custom body collider container. It should contain a collider component that will be used for detecting body collisions. If one isn't provided then it will be auto generated.
 * **Custom Foot Collider Container:** A GameObject to represent a custom foot collider container. It should contain a collider component that will be used for detecting step collisions. If one isn't provided then it will be auto generated.

### Class Variables

 * `public enum FallingRestrictors` - Options for testing if a play space fall is valid
   * `NoRestriction` - Always drop to nearest floor when the headset is no longer over the current standing object.
   * `LeftController` - Don't drop to nearest floor  if the Left Controller is still over the current standing object even if the headset isn't.
   * `RightController` - Don't drop to nearest floor  if the Right Controller is still over the current standing object even if the headset isn't.
   * `EitherController` - Don't drop to nearest floor  if Either Controller is still over the current standing object even if the headset isn't.
   * `BothControllers` - Don't drop to nearest floor only if Both Controllers are still over the current standing object even if the headset isn't.
   * `AlwaysRestrict` - Never drop to nearest floor when the headset is no longer over the current standing object.

### Class Events

 * `StartFalling` - Emitted when a fall begins.
 * `StopFalling` - Emitted when a fall ends.
 * `StartMoving` - Emitted when movement in the play area begins.
 * `StopMoving` - Emitted when movement in the play area ends.
 * `StartColliding` - Emitted when the body collider starts colliding with another game object.
 * `StopColliding` - Emitted when the body collider stops colliding with another game object.
 * `StartLeaning` - Emitted when the body collider starts leaning over another game object.
 * `StopLeaning` - Emitted when the body collider stops leaning over another game object.
 * `StartTouchingGround` - Emitted when the body collider starts touching the ground.
 * `StopTouchingGround` - Emitted when the body collider stops touching the ground.

### Unity Events

Adding the `VRTK_BodyPhysics_UnityEvents` component to `VRTK_BodyPhysics` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject target` - The target GameObject the event is dealing with.
 * `Collider collider` - An optional collider that the body physics is colliding with.

### Class Methods

#### ArePhysicsEnabled/0

  > `public virtual bool ArePhysicsEnabled()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the body physics will interact with other scene physics objects and `false` if the body physics will ignore other scene physics objects.

The ArePhysicsEnabled method determines whether the body physics are set to interact with other scene physics objects.

#### ApplyBodyVelocity/3

  > `public virtual void ApplyBodyVelocity(Vector3 velocity, bool forcePhysicsOn = false, bool applyMomentum = false)`

 * Parameters
   * `Vector3 velocity` - The velocity to apply.
   * `bool forcePhysicsOn` - If `true` will toggle the body collision physics back on if enable body collisions is true.
   * `bool applyMomentum` - If `true` then the existing momentum of the play area will be applied as a force to the resulting velocity.
 * Returns
   * _none_

The ApplyBodyVelocity method applies a given velocity to the rigidbody attached to the body physics.

#### ToggleOnGround/1

  > `public virtual void ToggleOnGround(bool state)`

 * Parameters
   * `bool state` - If `true` then body physics are set to being on the ground.
 * Returns
   * _none_

The ToggleOnGround method sets whether the body is considered on the ground or not.

#### TogglePreventSnapToFloor/1

  > `public virtual void TogglePreventSnapToFloor(bool state)`

 * Parameters
   * `bool state` - If `true` the the snap to floor mechanic will not execute.
 * Returns
   * _none_

The PreventSnapToFloor method sets whether the snap to floor mechanic should be used.

#### ForceSnapToFloor/0

  > `public virtual void ForceSnapToFloor()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceSnapToFloor method disables the prevent snap to floor and forces the snap to nearest floor action.

#### IsFalling/0

  > `public virtual bool IsFalling()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the body is currently falling via gravity or via teleport.

The IsFalling method returns the falling state of the body.

#### IsMoving/0

  > `public virtual bool IsMoving()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the user is currently walking around their play area space.

The IsMoving method returns the moving within play area state of the body.

#### IsLeaning/0

  > `public virtual bool IsLeaning()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the user is considered to be leaning over an object.

The IsLeaning method returns the leaning state of the user.

#### OnGround/0

  > `public virtual bool OnGround()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the play area is on the ground and false if the play area is in the air.

The OnGround method returns whether the user is currently standing on the ground or not.

#### GetVelocity/0

  > `public virtual Vector3 GetVelocity()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - The velocity of the body physics rigidbody.

The GetVelocity method returns the velocity of the body physics rigidbody.

#### GetAngularVelocity/0

  > `public virtual Vector3 GetAngularVelocity()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - The angular velocity of the body physics rigidbody.

The GetAngularVelocity method returns the angular velocity of the body physics rigidbody.

#### ResetVelocities/0

  > `public virtual void ResetVelocities()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetVelocities method sets the rigidbody velocity and angular velocity to zero to stop the Play Area rigidbody from continuing to move if it has a velocity already.

#### ResetFalling/0

  > `public virtual void ResetFalling()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetFalling method force stops any falling states and conditions that might be set on this object.

#### GetBodyColliderContainer/0

  > `public virtual GameObject GetBodyColliderContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The auto generated body collider GameObject.
   * `GameObject` -

The GetBodyColliderContainer method returns the auto generated GameObject that contains the body colliders.

#### GetFootColliderContainer/0

  > `public virtual GameObject GetFootColliderContainer()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The auto generated foot collider GameObject.
   * `GameObject` -

The GetFootColliderContainer method returns the auto generated GameObject that contains the foot colliders.

#### GetCurrentCollidingObject/0

  > `public virtual GameObject GetCurrentCollidingObject()`

 * Parameters
   * _none_
 * Returns
   * `GameObject` - The GameObject that is colliding with the body physics colliders.

The GetCurrentCollidingObject method returns the object that the body physics colliders are currently colliding with.

#### ResetIgnoredCollisions/0

  > `public virtual void ResetIgnoredCollisions()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ResetIgnoredCollisions method is used to clear any stored ignored colliders in case the `Ignore Collisions On` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.

#### SweepCollision/2

  > `public virtual bool SweepCollision(Vector3 direction, float maxDistance)`

 * Parameters
   * `Vector3 direction` - The direction to test for the potential collision.
   * `float maxDistance` - The maximum distance to check for a potential collision.
 * Returns
   * `bool` - Returns `true` if a collision will occur on the given direction over the given maxium distance. Returns `false` if there is no collision about to happen.

The SweepCollision method tests to see if a collision will occur with the body collider in a given direction and distance.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad but the user cannot pass through the objects as they are collidable and the rigidbody physics won't allow the intersection to occur.

---

## Position Rewind (VRTK_PositionRewind)

### Overview

Attempts to rewind the position of the play area to a last know valid position upon the headset collision event.

**Required Components:**
 * `VRTK_BodyPhysics` - A Body Physics script to manage the collisions of the body presence within the scene.
 * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the headset is colliding with valid geometry.

**Script Usage:**
 * Place the `VRTK_PositionRewind` script on any active scene GameObject.

### Inspector Parameters

 * **Collision Detector:** The colliders to determine if a collision has occured for the rewind to be actioned.
 * **Ignore Trigger Colliders:** If this is checked then the collision detector will ignore colliders set to `Is Trigger = true`.
 * **Rewind Delay:** The amount of time from original headset collision until the rewind to the last good known position takes place.
 * **Pushback Distance:** The additional distance to push the play area back upon rewind to prevent being right next to the wall again.
 * **Crouch Threshold:** The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.
 * **Crouch Rewind Threshold:** The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.
 * **Target List Policy:** A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Position Rewind.
 * **Body Physics:** The VRTK Body Physics script to use for the collisions and rigidbodies. If this is left blank then the first Body Physics script found in the scene will be used.
 * **Headset Collision:** The VRTK Headset Collision script to use to determine if the headset is colliding. If this is left blank then the script will need to be applied to the same GameObject.

### Class Variables

 * `public enum CollisionDetectors` - Valid collision detectors.
   * `HeadsetOnly` - Listen for collisions on the headset collider only.
   * `BodyOnly` - Listen for collisions on the body physics collider only.
   * `HeadsetAndBody` - Listen for collisions on both the headset collider and body physics collider.

### Class Events

 * `PositionRewindToSafe` - Emitted when the draggable item is successfully dropped.

### Unity Events

Adding the `VRTK_PositionRewind_UnityEvents` component to `VRTK_PositionRewind` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `Vector3 collidedPosition` - The position of the play area when it collded.
 * `Vector3 resetPosition` - The position of the play area when it has been rewinded to a safe position.

### Class Methods

#### SetLastGoodPosition/0

  > `public virtual void SetLastGoodPosition()`

 * Parameters
   * _none_
 * Returns
   * _none_

The SetLastGoodPosition method stores the current valid play area and headset position.

#### RewindPosition/0

  > `public virtual void RewindPosition()`

 * Parameters
   * _none_
 * Returns
   * _none_

The RewindPosition method resets the play area position to the last known good position of the play area.

### Example

`VRTK/Examples/017_CameraRig_TouchpadWalking` has the position rewind script to reset the user's position if they walk into objects.

---

# UI (VRTK/Source/Scripts/UI)

A collection of scripts that provide the ability to utilise and interact with Unity UI elements.

 * [UI Canvas](#ui-canvas-vrtk_uicanvas)
 * [UI Pointer](#ui-pointer-vrtk_uipointer)
 * [UI Draggable Item](#ui-draggable-item-vrtk_uidraggableitem)
 * [UI Drop Zone](#ui-drop-zone-vrtk_uidropzone)

---

## UI Canvas (VRTK_UICanvas)

### Overview

Denotes a Unity World UI Canvas can be interacted with a UIPointer script.

**Script Usage:**
 * Place the `VRTK_UICanvas` script on the Unity World UI Canvas to allow UIPointer interactions with.

**Script Dependencies:**
 * A UI Pointer attached to another GameObject (e.g. controller script alias) to interact with the UICanvas script.

### Inspector Parameters

 * **Click On Pointer Collision:** Determines if a UI Click action should happen when a UI Pointer game object collides with this canvas.
 * **Auto Activate Within Distance:** Determines if a UI Pointer will be auto activated if a UI Pointer game object comes within the given distance of this canvas. If a value of `0` is given then no auto activation will occur.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` uses the `VRTK_UICanvas` script on two of the canvases to show how the UI Pointer can interact with them.

---

## UI Pointer (VRTK_UIPointer)

### Overview

Provides the ability to interact with UICanvas elements and the contained Unity UI elements within.

**Optional Components:**
 * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller` parameter.

**Script Usage:**
 * Place the `VRTK_UIPointer` script on either:
   * The controller script alias GameObject of the controller to emit the UIPointer from (e.g. Right Controller Script Alias).
   * Any other scene GameObject and provide a valid `Transform` component to the `Pointer Origin Transform` parameter of this script. This does not have to be a controller and can be any GameObject that will emit the UIPointer.

**Script Dependencies:**
 * A UI Canvas attached to a Unity World UI Canvas.

### Inspector Parameters

 * **Activation Button:** The button used to activate/deactivate the UI raycast for the pointer.
 * **Activation Mode:** Determines when the UI pointer should be active.
 * **Selection Button:** The button used to execute the select action at the pointer's target position.
 * **Click Method:** Determines when the UI Click event action should happen.
 * **Attempt Click On Deactivate:** Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`
 * **Click After Hover Duration:** The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.
 * **Maximum Length:** The maximum length the UI Raycast will reach.
 * **Attached To:** An optional GameObject that determines what the pointer is to be attached to. If this is left blank then the GameObject the script is on will be used.
 * **Controller Events:** The Controller Events that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.
 * **Custom Origin:** A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.

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

 * `ActivationButtonPressed` - Emitted when the UI activation button is pressed.
 * `ActivationButtonReleased` - Emitted when the UI activation button is released.
 * `SelectionButtonPressed` - Emitted when the UI selection button is pressed.
 * `SelectionButtonReleased` - Emitted when the UI selection button is released.
 * `UIPointerElementEnter` - Emitted when the UI Pointer is colliding with a valid UI element.
 * `UIPointerElementExit` - Emitted when the UI Pointer is no longer colliding with any valid UI elements.
 * `UIPointerElementClick` - Emitted when the UI Pointer has clicked the currently collided UI element.
 * `UIPointerElementDragStart` - Emitted when the UI Pointer begins dragging a valid UI element.
 * `UIPointerElementDragEnd` - Emitted when the UI Pointer stops dragging a valid UI element.

### Unity Events

Adding the `VRTK_UIPointer_UnityEvents` component to `VRTK_UIPointer` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_ControllerReference controllerReference` - The reference to the controller that was used.
 * `bool isActive` - The state of whether the UI Pointer is currently active or not.
 * `GameObject currentTarget` - The current UI element that the pointer is colliding with.
 * `GameObject previousTarget` - The previous UI element that the pointer was colliding with.
 * `RaycastResult raycastResult` - The raw raycast result of the UI ray collision.

### Class Methods

#### GetPointerLength/1

  > `public static float GetPointerLength(int pointerId)`

 * Parameters
   * `int pointerId` - The pointer ID for the UI Pointer to recieve the length for.
 * Returns
   * `float` - The maximum length the UI Pointer will cast to.

The GetPointerLength method retrieves the maximum UI Pointer length for the given pointer ID.

#### SetEventSystem/1

  > `public virtual VRTK_VRInputModule SetEventSystem(EventSystem eventSystem)`

 * Parameters
   * `EventSystem eventSystem` - The global Unity event system to be used by the UI pointers.
 * Returns
   * `VRTK_VRInputModule` - A custom input module that is used to detect input from VR pointers.

The SetEventSystem method is used to set up the global Unity event system for the UI pointer. It also handles disabling the existing Standalone Input Module that exists on the EventSystem and adds a custom VRTK Event System VR Input component that is required for interacting with the UI with VR inputs.

#### RemoveEventSystem/0

  > `public virtual void RemoveEventSystem()`

 * Parameters
   * _none_
 * Returns
   * _none_

The RemoveEventSystem resets the Unity EventSystem back to the original state before the VRTK_VRInputModule was swapped for it.

#### PointerActive/0

  > `public virtual bool PointerActive()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the ui pointer should be currently active.

The PointerActive method determines if the ui pointer beam should be active based on whether the pointer alias is being held and whether the Hold Button To Use parameter is checked.

#### IsActivationButtonPressed/0

  > `public virtual bool IsActivationButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the activation button is active.

The IsActivationButtonPressed method is used to determine if the configured activation button is currently in the active state.

#### IsSelectionButtonPressed/0

  > `public virtual bool IsSelectionButtonPressed()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns `true` if the selection button is active.

The IsSelectionButtonPressed method is used to determine if the configured selection button is currently in the active state.

#### ValidClick/2

  > `public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)`

 * Parameters
   * `bool checkLastClick` - If this is true then the last frame's state of the UI Click button is also checked to see if a valid click has happened.
   * `bool lastClickState` - This determines what the last frame's state of the UI Click button should be in for it to be a valid click.
 * Returns
   * `bool` - Returns `true` if the UI Click button is in a valid state to action a click, returns `false` if it is not in a valid state.

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

Denotes a Unity UI Element as being draggable on the UI Canvas.

  > If a UI Draggable item is set to `Restrict To Drop Zone = true` then the UI Draggable item must be a child of an element that has the VRTK_UIDropZone script applied to it to ensure it starts in a valid drop zone.

**Script Usage:**
 * Place the `VRTK_UIDraggableItem` script on the Unity UI element that is to be dragged.

### Inspector Parameters

 * **Restrict To Drop Zone:** If checked then the UI element can only be dropped in valid a VRTK_UIDropZone object and must start as a child of a VRTK_UIDropZone object. If unchecked then the UI element can be dropped anywhere on the canvas.
 * **Restrict To Original Canvas:** If checked then the UI element can only be dropped on the original parent canvas. If unchecked the UI element can be dropped on any valid VRTK_UICanvas.
 * **Forward Offset:** The offset to bring the UI element forward when it is being dragged.

### Class Variables

 * `public GameObject validDropZone` - The current valid drop zone the dragged element is hovering over.

### Class Events

 * `DraggableItemDropped` - Emitted when the draggable item is successfully dropped.
 * `DraggableItemReset` - Emitted when the draggable item is reset.

### Unity Events

Adding the `VRTK_UIDraggableItem_UnityEvents` component to `VRTK_UIDraggableItem` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `GameObject target` - The target the item is dragged onto.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI elements that are draggable

---

## UI Drop Zone (VRTK_UIDropZone)
 > extends MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

### Overview

Specifies a Unity UI Element as being a valid drop zone location for a UI Draggable element.

  > It's appropriate to use a Panel UI element as a drop zone with a layout group applied so new children dropped into the drop zone automatically align.

**Script Usage:**
 * Place the `VRTK_UIDropZone` script on the Unity UI element that is to become the drop zone.

### Example

`VRTK/Examples/034_Controls_InteractingWithUnityUI` demonstrates a collection of UI Drop Zones.

---

# Utilities (VRTK/Source/Scripts/Utilities)

A collection of scripts that provide useful functionality to aid the creation process.

 * [SDK Manager](#sdk-manager-scriptingdefinesymbolpredicateinfo)
 * [SDK Setup](#sdk-setup-vrtk_sdksetup)
 * [SDK Info](#sdk-info-vrtk_sdkinfo)
 * [Device Finder](#device-finder-vrtk_devicefinder)
 * [Shared Methods](#shared-methods-vrtk_sharedmethods)
 * [Policy List](#policy-list-vrtk_policylist)
 * [Custom Raycast](#custom-raycast-vrtk_customraycast)
 * [Nav Mesh Data](#nav-mesh-data-vrtk_navmeshdata)
 * [Adaptive Quality](#adaptive-quality-vrtk_adaptivequality)
 * [Object Follow](#object-follow-vrtk_objectfollow)
 * [Rigidbody Follow](#rigidbody-follow-vrtk_rigidbodyfollow)
 * [Transform Follow](#transform-follow-vrtk_transformfollow)
 * [SDK Object Alias](#sdk-object-alias-vrtk_sdkobjectalias)
 * [SDK Transform Modify](#sdk-transform-modify-vrtk_sdktransformmodify)
 * [SDK Object State](#sdk-object-state-vrtk_sdkobjectstate)
 * [SDK Input Override](#sdk-input-override-vrtk_sdkinputoverride)
 * [Velocity Estimator](#velocity-estimator-vrtk_velocityestimator)

---

## SDK Manager (ScriptingDefineSymbolPredicateInfo)

### Overview

A helper class that simply holds references to both the SDK_ScriptingDefineSymbolPredicateAttribute and the method info of the method the attribute is defined on.

### Inspector Parameters

 * **Auto Manage Script Defines:** Determines whether the scripting define symbols required by the installed SDKs are automatically added to and removed from the player settings.
 * **Script Alias Left Controller:** A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.
 * **Script Alias Right Controller:** A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.
 * **Auto Manage VR Settings:** Determines whether the VR settings of the Player Settings are automatically adjusted to allow for all the used SDKs in the SDK Setups list below.
 * **Auto Load Setup:** Determines whether the SDK Setups list below is used whenever the SDK Manager is enabled. The first loadable Setup is then loaded.
 * **Setups:** The list of SDK Setups to choose from.
 * **Exclude Target Groups:** The list of Build Target Groups to exclude.

### Class Variables

 * `public readonly SDK_ScriptingDefineSymbolPredicateAttribute attribute` - The predicate attribute.
 * `public readonly MethodInfo methodInfo` - The method info of the method the attribute is defined on.
 * `public static ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> AvailableScriptingDefineSymbolPredicateInfos { get private set }` - All found scripting define symbol predicate attributes with associated method info.
 * `public static readonly Dictionary<Type, Type> SDKFallbackTypesByBaseType` - Specifies the fallback SDK types for every base SDK type. Default: `new Dictionary<Type, Type>`
 * `public static ReadOnlyCollection<VRTK_SDKInfo> AvailableSystemSDKInfos { get private set }` - All available system SDK infos.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> AvailableBoundariesSDKInfos { get private set }` - All available boundaries SDK infos.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> AvailableHeadsetSDKInfos { get private set }` - All available headset SDK infos.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> AvailableControllerSDKInfos { get private set }` - All available controller SDK infos.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> InstalledSystemSDKInfos { get private set }` - All installed system SDK infos. This is a subset of `AvailableSystemSDKInfos`. It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists that uses the same symbol and whose associated method returns true.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> InstalledBoundariesSDKInfos { get private set }` - All installed boundaries SDK infos. This is a subset of `AvailableBoundariesSDKInfos`. It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists that uses the same symbol and whose associated method returns true.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> InstalledHeadsetSDKInfos { get private set }` - All installed headset SDK infos. This is a subset of `AvailableHeadsetSDKInfos`. It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists that uses the same symbol and whose associated method returns true.
 * `public static ReadOnlyCollection<VRTK_SDKInfo> InstalledControllerSDKInfos { get private set }` - All installed controller SDK infos. This is a subset of `AvailableControllerSDKInfos`. It contains only those available SDK infos for which an SDK_ScriptingDefineSymbolPredicateAttribute exists that uses the same symbol and whose associated method returns true.
 * `public static VRTK_SDKManager instance` - The singleton instance to access the SDK Manager variables from.
 * `public List<SDK_ScriptingDefineSymbolPredicateAttribute> activeScriptingDefineSymbolsWithoutSDKClasses` - The active (i.e. to be added to the PlayerSettings) scripting define symbol predicate attributes that have no associated SDK classes. Default: `new List<SDK_ScriptingDefineSymbolPredicateAttribute>()`
 * `public VRTK_SDKSetup loadedSetup` - The loaded SDK Setup. `null` if no setup is currently loaded.
 * `public ReadOnlyCollection<Behaviour> behavioursToToggleOnLoadedSetupChange { get private set }` - All behaviours that need toggling whenever `loadedSetup` changes.

### Class Events

 * `LoadedSetupChanged` - The event invoked whenever the loaded SDK Setup changes.

### Unity Events

Adding the `VRTK_SDKManager_UnityEvents` component to `VRTK_SDKManager` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Event Payload

 * `VRTK_SDKSetup previousSetup` - The previous loaded Setup. `null` if no previous Setup was loaded.
 * `VRTK_SDKSetup currentSetup` - The current loaded Setup. `null` if no Setup is loaded anymore. See `errorMessage` to check whether this is `null` because of an error.
 * `string errorMessage` - Explains why loading a list of Setups wasn't successful if `currentSetup` is `null` and an error occurred. `null` if no error occurred.

### Class Methods

#### ScriptingDefineSymbolPredicateInfo/2

  > `public ScriptingDefineSymbolPredicateInfo(SDK_ScriptingDefineSymbolPredicateAttribute attribute, MethodInfo methodInfo)`

 * Parameters
   * `SDK_ScriptingDefineSymbolPredicateAttribute attribute` - The predicate attribute.
   * `MethodInfo methodInfo` - The method info of the method the attribute is defined on.
 * Returns
   * _none_

Event Payload. Constructs a new instance with the specified predicate attribute and associated method info.

#### ManageScriptingDefineSymbols/2

  > `public bool ManageScriptingDefineSymbols(bool ignoreAutoManageScriptDefines, bool ignoreIsActiveAndEnabled)`

 * Parameters
   * `bool ignoreAutoManageScriptDefines` - Whether to ignore `autoManageScriptDefines` while deciding to manage.
   * `bool ignoreIsActiveAndEnabled` - Whether to ignore `Behaviour.isActiveAndEnabled` while deciding to manage.
 * Returns
   * `bool` - Whether the PlayerSettings' scripting define symbols were changed.

Manages (i.e. adds and removes) the scripting define symbols of the PlayerSettings for the currently set SDK infos. This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used in a type that is also compiled for a standalone build.

#### ManageVRSettings/1

  > `public void ManageVRSettings(bool force)`

 * Parameters
   * `bool force` - Whether to ignore `autoManageVRSettings` while deciding to manage.
 * Returns
   * _none_

Manages (i.e. adds and removes) the VR SDKs of the PlayerSettings for the currently set SDK infos. This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used in a type that is also compiled for a standalone build.

#### AddBehaviourToToggleOnLoadedSetupChange/1

  > `public void AddBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)`

 * Parameters
   * `Behaviour behaviour` - The behaviour to add.
 * Returns
   * _none_

Adds a behaviour to the list of behaviours to toggle when `loadedSetup` changes.

#### RemoveBehaviourToToggleOnLoadedSetupChange/1

  > `public void RemoveBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)`

 * Parameters
   * `Behaviour behaviour` - The behaviour to remove.
 * Returns
   * _none_

Removes a behaviour of the list of behaviours to toggle when `loadedSetup` changes.

#### TryLoadSDKSetupFromList/1

  > `public void TryLoadSDKSetupFromList(bool tryUseLastLoadedSetup = true)`

 * Parameters
   * _none_
 * Returns
   * _none_

Tries to load a valid VRTK_SDKSetup from setups.

#### TryLoadSDKSetup/3

  > `public void TryLoadSDKSetup(int startIndex, bool tryToReinitialize, params VRTK_SDKSetup[] sdkSetups)`

 * Parameters
   * `int startIndex` - The index of the VRTK_SDKSetup to start the loading with.
   * `bool tryToReinitialize` - Whether or not to retry initializing and using the currently set but unusable VR Device.
   * `params VRTK_SDKSetup[] sdkSetups` - The list to try to load a VRTK_SDKSetup from.
 * Returns
   * _none_

Tries to load a valid VRTK_SDKSetup from a list. The first loadable VRTK_SDKSetup in the list will be loaded. Will fall back to disable VR if none of the provided Setups is useable.

#### SetLoadedSDKSetupToPopulateObjectReferences/1

  > `public void SetLoadedSDKSetupToPopulateObjectReferences(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup to set as the loaded SDK.
 * Returns
   * _none_

Sets a given VRTK_SDKSetup as the loaded SDK Setup to be able to use it when populating object references in the SDK Setup. This method should only be called when not playing as it's only for populating the object references. This method is only available in the editor, so usage of the method needs to be surrounded by `#if UNITY_EDITOR` and `#endif` when used in a type that is also compiled for a standalone build.

#### UnloadSDKSetup/1

  > `public void UnloadSDKSetup(bool disableVR = false)`

 * Parameters
   * `bool disableVR` - Whether to disable VR altogether after unloading the SDK Setup.
 * Returns
   * _none_

Unloads the currently loaded VRTK_SDKSetup, if there is one.

---

## SDK Setup (VRTK_SDKSetup)

### Overview

The SDK Setup describes a list of SDKs and game objects to use.

### Inspector Parameters

 * **Auto Populate Object References:** Determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.
 * **Actual Boundaries:** A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.
 * **Actual Headset:** A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.
 * **Actual Left Controller:** A reference to the GameObject that contains the SDK Left Hand Controller.
 * **Actual Right Controller:** A reference to the GameObject that contains the SDK Right Hand Controller.
 * **Model Alias Left Controller:** A reference to the GameObject that models for the Left Hand Controller.
 * **Model Alias Right Controller:** A reference to the GameObject that models for the Right Hand Controller.

### Class Variables

 * `public VRTK_SDKInfo systemSDKInfo` - The info of the SDK to use to deal with all system actions. By setting this to `null` the fallback SDK will be used.
 * `public VRTK_SDKInfo boundariesSDKInfo` - The info of the SDK to use to utilize room scale boundaries. By setting this to `null` the fallback SDK will be used.
 * `public VRTK_SDKInfo headsetSDKInfo` - The info of the SDK to use to utilize the VR headset. By setting this to `null` the fallback SDK will be used.
 * `public VRTK_SDKInfo controllerSDKInfo` - The info of the SDK to use to utilize the input devices. By setting this to `null` the fallback SDK will be used.
 * `public SDK_BaseSystem systemSDK` - The selected system SDK.
 * `public SDK_BaseBoundaries boundariesSDK` - The selected boundaries SDK.
 * `public SDK_BaseHeadset headsetSDK` - The selected headset SDK.
 * `public SDK_BaseController controllerSDK` - The selected controller SDK.
 * `public string[] usedVRDeviceNames` - The VR device names used by the currently selected SDKs.
 * `public bool isValid` - Whether it's possible to use the Setup. See `GetSimplifiedErrorDescriptions` for more info.

### Unity Events

Adding the `VRTK_SDKSetup_UnityEvents` component to `VRTK_SDKSetup` object allows access to `UnityEvents` that will react identically to the Class Events.

 * All C# delegate events are mapped to a Unity Event with the `On` prefix. e.g. `MyEvent` -> `OnMyEvent`.

### Class Methods

#### PopulateObjectReferences/1

  > `public void PopulateObjectReferences(bool force)`

 * Parameters
   * `bool force` - Whether to ignore `autoPopulateObjectReferences` while deciding to populate.
 * Returns
   * _none_

Populates the object references by using the currently set SDKs.

#### GetSimplifiedErrorDescriptions/0

  > `public string[] GetSimplifiedErrorDescriptions()`

 * Parameters
   * _none_
 * Returns
   * `string[]` - An array of all the error descriptions. Returns an empty array if no errors are found.

Checks the setup for errors and creates an array of error descriptions. The returned error descriptions handle the following cases for the current SDK infos:  * Its type doesn't exist anymore.  * It's a fallback SDK.  * It doesn't have its scripting define symbols added.  * It's missing its vendor SDK.Additionally the current SDK infos are checked whether they use multiple VR Devices.

---

## SDK Info (VRTK_SDKInfo)
 > extends ISerializationCallbackReceiver

### Overview

Holds all the info necessary to describe an SDK.

### Class Variables

 * `public Type type { get private set }` - The type of the SDK.
 * `public string originalTypeNameWhenFallbackIsUsed { get private set }` - The name of the type of which this SDK info was created from. This is only used if said type wasn't found.
 * `public SDK_DescriptionAttribute description { get private set }` - The description of the SDK.

### Class Methods

#### Create<BaseType, FallbackType, ActualType>/0

  > `public static VRTK_SDKInfo[] Create<BaseType, FallbackType, ActualType>() where BaseType : SDK_Base where FallbackType : BaseType where ActualType : BaseType`

 * Type Params
   * `BaseType` - The SDK base type. Must be a subclass of SDK_Base.
   * `FallbackType` - The SDK type to fall back on if problems occur. Must be a subclass of `BaseType`.
   * `ActualType` - The SDK type to use. Must be a subclass of `BaseType`.
 * Parameters
   * _none_
 * Returns
   * `VRTK_SDKInfo[]` - Multiple newly created instances.

Creates new SDK infos for a type that is known at compile time.

#### Create<BaseType, FallbackType>/1

  > `public static VRTK_SDKInfo[] Create<BaseType, FallbackType>(Type actualType) where BaseType : SDK_Base where FallbackType : BaseType`

 * Type Params
   * `BaseType` - The SDK base type. Must be a subclass of SDK_Base.
   * `FallbackType` - The SDK type to fall back on if problems occur. Must be a subclass of `BaseType.
 * Parameters
   * `Type actualType` - The SDK type to use. Must be a subclass of `BaseType.
 * Returns
   * `VRTK_SDKInfo[]` - Multiple newly created instances.

Creates new SDK infos for a type.

#### VRTK_SDKInfo/1

  > `public VRTK_SDKInfo(VRTK_SDKInfo infoToCopy)`

 * Parameters
   * `VRTK_SDKInfo infoToCopy` - The SDK info to copy.
 * Returns
   * _none_

Creates a new SDK info by copying an existing one.

---

## Device Finder (VRTK_DeviceFinder)

### Overview

The Device Finder offers a collection of static methods that can be called to find common game devices such as the headset or controllers, or used to determine key information about the connected devices.

### Class Variables

 * `public enum Devices` - Possible devices.
   * `Headset` - The headset.
   * `LeftController` - The left hand controller.
   * `RightController` - The right hand controller.
 * `public enum Headsets` - Possible headsets
   * `Unknown` - An unknown headset.
   * `OculusRift` - A summary of all Oculus Rift headset versions.
   * `OculusRiftCV1` - A specific version of the Oculus Rift headset, the Consumer Version 1.
   * `Vive` - A summary of all HTC Vive headset versions.
   * `ViveMV` - A specific version of the HTC Vive headset, the first consumer version.
   * `ViveDVT` - A specific version of the HTC Vive headset, the first consumer version.
   * `OculusRiftES07` - A specific version of the Oculus Rift headset, the rare ES07.

### Class Methods

#### GetCurrentControllerType/1

  > `public static SDK_BaseController.ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `SDK_BaseController.ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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

  > `public static Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the origin for.
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

#### GetControllerReferenceLeftHand/0

  > `public static VRTK_ControllerReference GetControllerReferenceLeftHand()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerReference` - The Controller Reference for the left hand controller.

The GetControllerReferenceLeftHand returns a Controller Reference for the left hand controller.

#### GetControllerReferenceRightHand/0

  > `public static VRTK_ControllerReference GetControllerReferenceRightHand()`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerReference` - The Controller Reference for the right hand controller.

The GetControllerReferenceRightHand returns a Controller Reference for the right hand controller.

#### GetControllerReferenceForHand/1

  > `public static VRTK_ControllerReference GetControllerReferenceForHand(SDK_BaseController.ControllerHand hand)`

 * Parameters
   * _none_
 * Returns
   * `VRTK_ControllerReference` - The Controller Reference for the given hand controller.

The GetControllerReferenceForHand returns a Controller Reference for the given hand controller.

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

#### GetOppositeHand/1

  > `public static SDK_BaseController.ControllerHand GetOppositeHand(SDK_BaseController.ControllerHand currentHand)`

 * Parameters
   * `SDK_BaseController.ControllerHand currentHand` - The current hand.
 * Returns
   * `SDK_BaseController.ControllerHand` - The opposite hand.

The GetOppositeHand method returns the other hand type from the current hand given.

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

#### GetModelAliasControllerHand/1

  > `public static SDK_BaseController.ControllerHand GetModelAliasControllerHand(GameObject givenObject)`

 * Parameters
   * `GameObject givenObject` - The GameObject that may represent a model alias.
 * Returns
   * `SDK_BaseController.ControllerHand` - The enum of the ControllerHand that the given GameObject may represent.

The GetModelAliasControllerHand method will return the hand that the given model alias GameObject is for.

#### GetControllerVelocity/1

  > `public static Vector3 GetControllerVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller.
 * Returns
   * `Vector3` - A 3 dimensional vector containing the current real world physical controller velocity.

The GetControllerVelocity method is used for getting the current velocity of the physical game controller. This can be useful to determine the speed at which the controller is being swung or the direction it is being moved in.

#### GetControllerAngularVelocity/1

  > `public static Vector3 GetControllerAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller.
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

#### GetHeadsetTypeAsString/0

  > `public static string GetHeadsetTypeAsString()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetTypeAsString method returns a string representing the type of headset connected.

#### GetHeadsetType/0

  > `public static SDK_BaseHeadset.HeadsetType GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `SDK_BaseHeadset.HeadsetType` - The Headset type that is connected.

The GetHeadsetType method returns the type of headset currently connected.

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

#### ColliderExclude/2

  > `public static Collider[] ColliderExclude(Collider[] setA, Collider[] setB)`

 * Parameters
   * `Collider[] setA` - The array that contains all of the relevant colliders.
   * `Collider[] setB` - The array that contains the colliders to remove from setA.
 * Returns
   * `Collider[]` - A Collider array that is a subset of setA that doesn't contain the colliders from setB.

The ColliderExclude method reduces the colliders in the setA array by those matched in the setB array.

#### GetCollidersInGameObjects/3

  > `public static Collider[] GetCollidersInGameObjects(GameObject[] gameObjects, bool searchChildren, bool includeInactive)`

 * Parameters
   * `GameObject[] gameObjects` - An array of GameObjects to get the colliders for.
   * `bool searchChildren` - If this is `true` then the given GameObjects will also have their child GameObjects searched for colliders.
   * `bool includeInactive` - If this is `true` then the inactive GameObjects in the array will also be checked for Colliders. Only relevant if `searchChildren` is `true`.
 * Returns
   * `Collider[]` - An array of Colliders that are found in the given GameObject array.

The GetCollidersInGameObjects method iterates through a GameObject array and returns all of the unique found colliders for all GameObejcts.

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

#### RoundFloat/3

  > `public static float RoundFloat(float givenFloat, int decimalPlaces, bool rawFidelity = false)`

 * Parameters
   * `float givenFloat` - The float to round.
   * `int decimalPlaces` - The number of decimal places to round to.
   * `bool rawFidelity` - If this is true then the decimal places must be given in the decimal multiplier, e.g. 10 for 1dp, 100 for 2dp, etc.
 * Returns
   * `float` - The rounded float.

The RoundFloat method is used to round a given float to the given decimal places.

#### IsEditTime/0

  > `public static bool IsEditTime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if Unity is in the Unity Editor and not in play mode.

The IsEditTime method determines if the state of Unity is in the Unity Editor and the scene is not in play mode.

#### Mod/2

  > `public static float Mod(float a, float b)`

 * Parameters
   * `float a` - The dividend value.
   * `float b` - The divisor value.
 * Returns
   * `float` - The remainder value.

The Mod method is used to find the remainder of the sum a/b.

#### FindEvenInactiveGameObject<T>/1

  > `public static GameObject FindEvenInactiveGameObject<T>(string gameObjectName = null) where T : Component`

 * Type Params
   * `T` - The component type that needs to be on an ancestor of the wanted GameObject. Must be a subclass of `Component`.
 * Parameters
   * `string gameObjectName` - The name of the wanted GameObject. If it contains a '/' character, this method traverses the hierarchy like a path name, beginning on the game object that has a component of type `T`.
 * Returns
   * `GameObject` - The GameObject with name `gameObjectName` and an ancestor that has a `T`. If no such GameObject is found then `null` is returned.

Finds the first GameObject with a given name and an ancestor that has a specific component. This method returns active as well as inactive GameObjects in the scene. It doesn't return assets. For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.

#### FindEvenInactiveComponents<T>/0

  > `public static T[] FindEvenInactiveComponents<T>() where T : Component`

 * Type Params
   * `T` - The component type to search for. Must be a subclass of `Component`.
 * Parameters
   * _none_
 * Returns
   * `T[]` - All the found components. If no component is found an empty array is returned.

Finds all components of a given type. This method returns components from active as well as inactive GameObjects in the scene. It doesn't return assets. For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.

#### FindEvenInactiveComponent<T>/0

  > `public static T FindEvenInactiveComponent<T>() where T : Component`

 * Type Params
   * `T` - The component type to search for. Must be a subclass of `Component`.
 * Parameters
   * _none_
 * Returns
   * `T` - The found component. If no component is found `null` is returned.

Finds the first component of a given type. This method returns components from active as well as inactive GameObjects in the scene. It doesn't return assets. For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.

#### GenerateVRTKObjectName/2

  > `public static string GenerateVRTKObjectName(bool autoGen, params object[] replacements)`

 * Parameters
   * `bool autoGen` - An additiona [AUTOGEN] prefix will be added if this is true.
   * `params object[] replacements` - A collection of parameters to add to the generated name.
 * Returns
   * `string` - The generated name string.

The GenerateVRTKObjectName method is used to create a standard name string for any VRTK generated object.

#### GetGPUTimeLastFrame/0

  > `public static float GetGPUTimeLastFrame()`

 * Parameters
   * _none_
 * Returns
   * `float` - The total GPU time utilized last frame as measured by the VR subsystem.

The GetGPUTimeLastFrame retrieves the time spent by the GPU last frame, in seconds, as reported by the VR SDK.

#### Vector2ShallowCompare/3

  > `public static bool Vector2ShallowCompare(Vector2 vectorA, Vector2 vectorB, int compareFidelity)`

 * Parameters
   * `Vector2 vectorA` - The Vector2 to compare against.
   * `Vector2 vectorB` - The Vector2 to compare with
   * `int compareFidelity` - The number of decimal places to use when doing the comparison on the float elements within the Vector2.
 * Returns
   * `bool` - Returns `true` if the given Vector2 objects match based on the given fidelity.

The Vector2ShallowCompare method compares two given Vector2 objects based on the given fidelity, which is the equivalent of comparing rounded Vector2 elements to determine if the Vector2 elements are equal.

#### Vector3ShallowCompare/3

  > `public static bool Vector3ShallowCompare(Vector3 vectorA, Vector3 vectorB, float threshold)`

 * Parameters
   * `Vector3 vectorA` - The Vector3 to compare against.
   * `Vector3 vectorB` - The Vector3 to compare with
   * `float threshold` - The distance in which the two Vector3 objects can be within to be considered true
 * Returns
   * `bool` - Returns `true` if the given Vector3 objects are within the given threshold distance.

The Vector3ShallowCompare method compares two given Vector3 objects based on the given threshold, which is the equavelent of checking the distance between two Vector3 objects are above the threshold distance.

#### NumberPercent/2

  > `public static float NumberPercent(float value, float percent)`

 * Parameters
   * `float value` - The value to determine the percentage from
   * `float percent` - The percentage to find within the given value.
 * Returns
   * `float` - A float containing the percentage value based on the given input.

The NumberPercent method is used to determine the percentage of a given value.

#### SetGlobalScale/2

  > `public static void SetGlobalScale(this Transform transform, Vector3 globalScale)`

 * Parameters
   * `this Transform transform` - The reference to the transform to scale.
   * `Vector3 globalScale` - A Vector3 of a global scale to apply to the given transform.
 * Returns
   * _none_

The SetGlobalScale method is used to set a transform scale based on a global scale instead of a local scale.

#### VectorHeading/2

  > `public static Vector3 VectorHeading(Vector3 originPosition, Vector3 targetPosition)`

 * Parameters
   * `Vector3 originPosition` - The point to use as the originating position for the heading calculation.
   * `Vector3 targetPosition` - The point to use as the target position for the heading calculation.
 * Returns
   * `Vector3` - A Vector3 containing the heading changes of the target position in relation to the origin position.

The VectorHeading method calculates the current heading of the target position in relation to the origin position.

#### VectorDirection/2

  > `public static Vector3 VectorDirection(Vector3 originPosition, Vector3 targetPosition)`

 * Parameters
   * `Vector3 originPosition` - The point to use as the originating position for the direction calculation.
   * `Vector3 targetPosition` - The point to use as the target position for the direction calculation.
 * Returns
   * `Vector3` - A Vector3 containing the direction of the target position in relation to the origin position.

The VectorDirection method calculates the direction the target position is in relation to the origin position.

#### DividerToMultiplier/1

  > `public static float DividerToMultiplier(float value)`

 * Parameters
   * `float value` - The number to convert into the multplier value.
 * Returns
   * `float` - The calculated number that can replace the divider number in a multiplication sum.

The DividerToMultiplier method takes a number to be used in a division and converts it to be used for multiplication. (e.g. 2 / 2 becomes 2 * 0.5)

#### NormalizeValue/4

  > `public static float NormalizeValue(float value, float minValue, float maxValue, float threshold = 0f)`

 * Parameters
   * `float value` - The actual value to normalize.
   * `float minValue` - The minimum value the actual value can be.
   * `float maxValue` - The maximum value the actual value can be.
   * `float threshold` - The threshold to force to the minimum or maximum value if the normalized value is within the threhold limits.
 * Returns
   * `float` -

The NormalizeValue method takes a given value between a specified range and returns the normalized value between 0f and 1f.

#### AxisDirection/2

  > `public static Vector3 AxisDirection(int axisIndex, Transform givenTransform = null)`

 * Parameters
   * `int axisIndex` - The axis index of the axis. `0 = x` `1 = y` `2 = z`
   * `Transform givenTransform` - An optional Transform to get the Axis Direction for. If this is `null` then the World directions will be used.
 * Returns
   * `Vector3` - The direction Vector3 based on the given axis index.

The AxisDirection method returns the relevant direction Vector3 based on the axis index in relation to x,y,z.

#### AddListValue<TValue>/3

  > `public static bool AddListValue<TValue>(List<TValue> list, TValue value, bool preventDuplicates = false)`

 * Type Params
   * `TValue` - The datatype for the list value.
 * Parameters
   * `List<TValue> list` - The list to retrieve the value from.
   * `TValue value` - The value to attempt to add to the list.
   * `bool preventDuplicates` - If this is `false` then the value provided will always be appended to the list. If this is `true` the value provided will only be added to the list if it doesn't already exist.
 * Returns
   * `bool` - Returns `true` if the given value was successfully added to the list. Returns `false` if the given value already existed in the list and `preventDuplicates` is `true`.

The AddListValue method adds the given value to the given list. If `preventDuplicates` is `true` then the given value will only be added if it doesn't already exist in the given list.

#### GetDictionaryValue<TKey, TValue>/4

  > `public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue), bool setMissingKey = false)`

 * Type Params
   * `TKey` - The datatype for the dictionary key.
   * `TValue` - The datatype for the dictionary value.
 * Parameters
   * `Dictionary<TKey, TValue> dictionary` - The dictionary to retrieve the value from.
   * `TKey key` - The key to retrieve the value for.
   * `TValue defaultValue` - The value to utilise when either setting the missing key (if `setMissingKey` is `true`) or the default value to return when no key is found (if `setMissingKey` is `false`).
   * `bool setMissingKey` - If this is `true` and the given key is not present, then the dictionary value for the given key will be set to the `defaultValue` parameter. If this is `false` and the given key is not present then the `defaultValue` parameter will be returned as the value.
 * Returns
   * `TValue` - The found value for the given key in the given dictionary, or the default value if no key is found.

The GetDictionaryValue method attempts to retrieve a value from a given dictionary for the given key. It removes the need for a double dictionary lookup to ensure the key is valid and has the option of also setting the missing key value to ensure the dictionary entry is valid.

#### GetDictionaryValue<TKey, TValue>/5

  > `public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, out bool keyExists, TValue defaultValue = default(TValue), bool setMissingKey = false)`

 * Type Params
   * `TKey` - The datatype for the dictionary key.
   * `TValue` - The datatype for the dictionary value.
 * Parameters
   * `Dictionary<TKey, TValue> dictionary` - The dictionary to retrieve the value from.
   * `TKey key` - The key to retrieve the value for.
   * `out bool keyExists` - Sets the given parameter to `true` if the key exists in the given dictionary or sets to `false` if the key didn't existing in the given dictionary.
   * `TValue defaultValue` - The value to utilise when either setting the missing key (if `setMissingKey` is `true`) or the default value to return when no key is found (if `setMissingKey` is `false`).
   * `bool setMissingKey` - If this is `true` and the given key is not present, then the dictionary value for the given key will be set to the `defaultValue` parameter. If this is `false` and the given key is not present then the `defaultValue` parameter will be returned as the value.
 * Returns
   * `TValue` - The found value for the given key in the given dictionary, or the default value if no key is found.

The GetDictionaryValue method attempts to retrieve a value from a given dictionary for the given key. It removes the need for a double dictionary lookup to ensure the key is valid and has the option of also setting the missing key value to ensure the dictionary entry is valid.

#### AddDictionaryValue<TKey, TValue>/4

  > `public static bool AddDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool overwriteExisting = false)`

 * Type Params
   * `TKey` - The datatype for the dictionary key.
   * `TValue` - The datatype for the dictionary value.
 * Parameters
   * `Dictionary<TKey, TValue> dictionary` - The dictionary to set the value for.
   * `TKey key` - The key to set the value for.
   * `TValue value` - The value to set at the given key in the given dictionary.
   * `bool overwriteExisting` - If this is `true` then the value for the given key will always be set to the provided value. If this is `false` then the value for the given key will only be set if the given key is not found in the given dictionary.
 * Returns
   * `bool` - Returns `true` if the given value was successfully added to the dictionary at the given key. Returns `false` if the given key already existed in the dictionary and `overwriteExisting` is `false`.

The AddDictionaryValue method attempts to add a value for the given key in the given dictionary if the key does not already exist. If `overwriteExisting` is `true` then it always set the value even if they key exists.

#### GetTypeUnknownAssembly/1

  > `public static Type GetTypeUnknownAssembly(string typeName)`

 * Parameters
   * `string typeName` - The name of the type to get.
 * Returns
   * `Type` - The Type, or null if none is found.

The GetTypeUnknownAssembly method is used to find a Type without knowing the exact assembly it is in.

#### GetEyeTextureResolutionScale/0

  > `public static float GetEyeTextureResolutionScale()`

 * Parameters
   * _none_
 * Returns
   * `float` - Returns a float with the render scale for the resolution.

The GetEyeTextureResolutionScale method returns the render scale for the resolution.

#### SetEyeTextureResolutionScale/1

  > `public static void SetEyeTextureResolutionScale(float value)`

 * Parameters
   * `float value` - The value to set the render scale to.
 * Returns
   * _none_

The SetEyeTextureResolutionScale method sets the render scale for the resolution.

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
 * **Identifiers:** A list of identifiers to check for against the given check type (either tag or script).

### Class Variables

 * `public enum OperationTypes` - The operation to apply on the list of identifiers.
   * `Ignore` - Will ignore any game objects that contain either a tag or script component that is included in the identifiers list.
   * `Include` - Will only include game objects that contain either a tag or script component that is included in the identifiers list.
 * `public enum CheckTypes` - The types of element that can be checked against.
   * `Tag = 1` - The tag applied to the game object.
   * `Script = 2` - A script component added to the game object.
   * `Layer = 4` - A layer applied to the game object.

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

## Custom Raycast (VRTK_CustomRaycast)

### Overview

A Custom Raycast allows to specify custom options for a Physics.Raycast.

A number of other scripts can utilise a Custom Raycast to further customise the raycasts that the scripts use internally.

For example, the VRTK_BodyPhysics script can be set to ignore trigger colliders when casting to see if it should teleport up or down to the nearest floor.

### Inspector Parameters

 * **Layers To Ignore:** The layers to ignore when raycasting.
 * **Trigger Interaction:** Determines whether the ray will interact with trigger colliders.

### Class Methods

#### Raycast/6

  > `public static bool Raycast(VRTK_CustomRaycast customCast, Ray ray, out RaycastHit hitData, LayerMask ignoreLayers, float length = Mathf.Infinity, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)`

 * Parameters
   * `VRTK_CustomRaycast customCast` - The optional object with customised cast parameters.
   * `Ray ray` - The Ray to cast with.
   * `out RaycastHit hitData` - The raycast hit data.
   * `LayerMask ignoreLayers` - A layermask of layers to ignore from the raycast.
   * `float length` - The maximum length of the raycast.
   * `QueryTriggerInteraction affectTriggers` - Determines the trigger interaction level of the cast.
 * Returns
   * `bool` - Returns true if the raycast successfully collides with a valid object.

The Raycast method is used to generate a raycast either from the given CustomRaycast object or a default Physics.Raycast.

#### Linecast/6

  > `public static bool Linecast(VRTK_CustomRaycast customCast, Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData, LayerMask ignoreLayers, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)`

 * Parameters
   * `VRTK_CustomRaycast customCast` - The optional object with customised cast parameters.
   * `Vector3 startPosition` - The world position to start the linecast from.
   * `Vector3 endPosition` - The world position to end the linecast at.
   * `out RaycastHit hitData` - The linecast hit data.
   * `LayerMask ignoreLayers` - A layermask of layers to ignore from the linecast.
   * `QueryTriggerInteraction affectTriggers` - Determines the trigger interaction level of the cast.
 * Returns
   * `bool` - Returns true if the linecast successfully collides with a valid object.

The Linecast method is used to generate a linecast either from the given CustomRaycast object or a default Physics.Linecast.

#### CapsuleCast/9

  > `public static bool CapsuleCast(VRTK_CustomRaycast customCast, Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, out RaycastHit hitData, LayerMask ignoreLayers, QueryTriggerInteraction affectTriggers = QueryTriggerInteraction.UseGlobal)`

 * Parameters
   * `VRTK_CustomRaycast customCast` - The optional object with customised cast parameters.
   * `Vector3 point1` - The center of the sphere at the start of the capsule.
   * `Vector3 point2` - The center of the sphere at the end of the capsule.
   * `float radius` - The radius of the capsule.
   * `Vector3 direction` - The direction into which to sweep the capsule.
   * `float maxDistance` - The max length of the sweep.
   * `out RaycastHit hitData` - The linecast hit data.
   * `LayerMask ignoreLayers` - A layermask of layers to ignore from the linecast.
   * `QueryTriggerInteraction affectTriggers` - Determines the trigger interaction level of the cast.
 * Returns
   * `bool` - Returns true if the linecast successfully collides with a valid object.

The CapsuleCast method is used to generate a linecast either from the given CustomRaycast object or a default Physics.Linecast.

#### CustomRaycast/3

  > `public virtual bool CustomRaycast(Ray ray, out RaycastHit hitData, float length = Mathf.Infinity)`

 * Parameters
   * `Ray ray` - The Ray to cast with.
   * `out RaycastHit hitData` - The raycast hit data.
   * `float length` - The maximum length of the raycast.
 * Returns
   * `bool` - Returns true if the raycast successfully collides with a valid object.

The CustomRaycast method is used to generate a raycast based on the options defined in the CustomRaycast object.

#### CustomLinecast/3

  > `public virtual bool CustomLinecast(Vector3 startPosition, Vector3 endPosition, out RaycastHit hitData)`

 * Parameters
   * `Vector3 startPosition` - The world position to start the linecast from.
   * `Vector3 endPosition` - The world position to end the linecast at.
   * `out RaycastHit hitData` - The linecast hit data.
 * Returns
   * `bool` - Returns true if the line successfully collides with a valid object.

The CustomLinecast method is used to generate a linecast based on the options defined in the CustomRaycast object.

#### CustomCapsuleCast/6

  > `public virtual bool CustomCapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, out RaycastHit hitData)`

 * Parameters
   * `Vector3 point1` - The center of the sphere at the start of the capsule.
   * `Vector3 point2` - The center of the sphere at the end of the capsule.
   * `float radius` - The radius of the capsule.
   * `Vector3 direction` - The direction into which to sweep the capsule.
   * `float maxDistance` - The max length of the sweep.
   * `out RaycastHit hitData` - The capsulecast hit data.
 * Returns
   * `bool` - Returns true if the capsule successfully collides with a valid object.

The CustomCapsuleCast method is used to generate a capsulecast based on the options defined in the CustomRaycast object.

---

## Nav Mesh Data (VRTK_NavMeshData)

### Overview

The Nav Mesh Data script allows custom nav mesh information to be provided to the teleporter script.

### Inspector Parameters

 * **Distance Limit:** The max distance given point can be outside the nav mesh to be considered valid.
 * **Valid Areas:** The parts of the navmesh that are considered valid

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
 * **Render Scale Limits:** The minimum and maximum allowed render scale.
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

## Object Follow (VRTK_ObjectFollow)

### Overview

Abstract class that allows to change one game object's properties to follow another game object.

### Inspector Parameters

 * **Game Object To Follow:** The game object to follow. The followed property values will be taken from this one.
 * **Game Object To Change:** The game object to change the property values of. If left empty the game object this script is attached to will be changed.
 * **Follows Position:** Whether to follow the position of the given game object.
 * **Smooths Position:** Whether to smooth the position when following `gameObjectToFollow`.
 * **Max Allowed Per Frame Distance Difference:** The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.
 * **Follows Rotation:** Whether to follow the rotation of the given game object.
 * **Smooths Rotation:** Whether to smooth the rotation when following `gameObjectToFollow`.
 * **Max Allowed Per Frame Angle Difference:** The maximum allowed angle between the unsmoothed source and the smoothed target per frame to use for smoothing.
 * **Follows Scale:** Whether to follow the scale of the given game object.
 * **Smooths Scale:** Whether to smooth the scale when following `gameObjectToFollow`.
 * **Max Allowed Per Frame Size Difference:** The maximum allowed size between the unsmoothed source and the smoothed target per frame to use for smoothing.

### Class Variables

 * `public Vector3 targetPosition { get private set }` - The position that results by following `gameObjectToFollow`.
 * `public Quaternion targetRotation { get private set }` - The rotation that results by following `gameObjectToFollow`.
 * `public Vector3 targetScale { get private set }` - The scale that results by following `gameObjectToFollow`.

### Class Methods

#### Follow/0

  > `public virtual void Follow()`

 * Parameters
   * _none_
 * Returns
   * _none_

Follow `gameObjectToFollow` using the current settings.

---

## Rigidbody Follow (VRTK_RigidbodyFollow)
 > extends [VRTK_ObjectFollow](#object-follow-vrtk_objectfollow)

### Overview

Changes one GameObject's rigidbody to follow another GameObject's rigidbody.

### Inspector Parameters

 * **Movement Option:** Specifies how to position and rotate the rigidbody.
 * **Track Max Distance:** The maximum distance the tracked `Game Object To Change` Rigidbody can be from the `Game Object To Follow` Rigidbody before the position is forcibly set to match the position.

### Class Variables

 * `public enum MovementOption` - Specifies how to position and rotate the rigidbody.
   * `Set` - Use Rigidbody.position and Rigidbody.rotation.
   * `Move` - Use Rigidbody.MovePosition and Rigidbody.MoveRotation.
   * `Add` - Use Rigidbody.AddForce(Vector3) and Rigidbody.AddTorque(Vector3).
   * `Track` - Use velocity and angular velocity with MoveTowards.

### Class Methods

#### Follow/0

  > `public override void Follow()`

 * Parameters
   * _none_
 * Returns
   * _none_

Follow `gameObjectToFollow` using the current settings.

---

## Transform Follow (VRTK_TransformFollow)
 > extends [VRTK_ObjectFollow](#object-follow-vrtk_objectfollow)

### Overview

Changes one GameObject's transform to follow another GameObject's transform.

### Class Variables

 * `public enum FollowMoment` - The moment at which to follow.
   * `OnFixedUpdate` - Follow in the FixedUpdate method.
   * `OnUpdate` - Follow in the Update method.
   * `OnLateUpdate` - Follow in the LateUpdate method.
   * `OnPreRender` - Follow in the OnPreRender method. (This script doesn't have to be attached to a camera).
   * `OnPreCull` - Follow in the OnPreCull method. (This script doesn't have to be attached to a camera).

### Class Methods

#### Follow/0

  > `public override void Follow()`

 * Parameters
   * _none_
 * Returns
   * _none_

Follow `gameObjectToFollow` using the current settings.

---

## SDK Object Alias (VRTK_SDKObjectAlias)

### Overview

The GameObject that the SDK Object Alias script is applied to will become a child of the selected SDK Object.

### Inspector Parameters

 * **Sdk Object:** The specific SDK Object to child this GameObject to.

### Class Variables

 * `public enum SDKObject` - Valid SDK Objects
   * `Boundary` - The main camera rig/play area object that defines the player boundary.
   * `Headset` - The main headset camera defines the player head.

---

## SDK Transform Modify (VRTK_SDKTransformModify)
 > extends VRTK_SDKControllerReady

### Overview

The SDK Transform Modify can be used to change a transform orientation at runtime based on the currently used SDK or SDK controller.

### Inspector Parameters

 * **Loaded SDK Setup:** An optional SDK Setup to use to determine when to modify the transform.
 * **Controller Type:** An optional SDK controller type to use to determine when to modify the transform.
 * **Position:** The new local position to change the transform to.
 * **Rotation:** The new local rotation in eular angles to change the transform to.
 * **Scale:** The new local scale to change the transform to.
 * **Target:** The target Transform to modify on enable. If this is left blank then the Transform the script is attached to will be used.
 * **Reset On Disable:** If this is checked then the target Transform will be reset to the original orientation when this script is disabled.
 * **Sdk Overrides:** A collection of SDK Transform overrides to change the given target Transform for each specified SDK.

### Class Methods

#### UpdateTransform/1

  > `public virtual void UpdateTransform(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - An optional reference to the controller to update the transform with.
 * Returns
   * _none_

The UpdateTransform method updates the Transform data on the current GameObject for the specified settings.

#### SetOrigins/0

  > `public virtual void SetOrigins()`

 * Parameters
   * _none_
 * Returns
   * _none_

The SetOrigins method sets the original position, rotation, scale of the target Transform.

---

## SDK Object State (VRTK_SDKObjectState)
 > extends VRTK_SDKControllerReady

### Overview

The SDK Object State script can be used to set the enable/active state of a GameObject or Component based on SDK information.

The state can be determined by:
 * The current loaded SDK setup.
 * The current attached Headset type.
 * The current attached Controller type.

### Inspector Parameters

 * **Target:** The GameObject or Component that is the target of the enable/disable action. If this is left blank then the GameObject that the script is attached to will be used as the `Target`.
 * **Object State:** The state to set the `Target` to when this script is enabled. Checking this box will enable/activate the `Target`, unchecking will disable/deactivate the `Target`.
 * **Loaded SDK Setup:** If the currently loaded SDK Setup matches the one provided here then the `Target` state will be set to the desired `Object State`.
 * **Headset Type:** If the attached headset type matches the selected headset then the `Target` state will be set to the desired `Object State`.
 * **Controller Type:** If the current controller type matches the selected controller type then the `Target` state will be set to the desired `Object State`.

### Class Methods

#### SetStateByControllerReference/1

  > `public virtual void SetStateByControllerReference(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - A controller reference to check for the controller type of.
 * Returns
   * _none_

The SetStateByControllerReference method sets the object state based on the controller type of the given controller reference.

---

## SDK Input Override (VRTK_SDKInputOverride)
 > extends VRTK_SDKControllerReady

### Overview

Provides the ability to switch button mappings based on the current SDK or controller type

**Script Usage:**
 * Place the `VRTK_SDKInputOverride` script on any active scene GameObject.
 * Customise the input button for each script type for each SDK controller type.

### Inspector Parameters

 * **Loaded SDK Setup:** An optional SDK Setup to use to determine when to modify the transform.
 * **Controller Type:** An optional SDK controller type to use to determine when to modify the transform.
 * **Override Button:** The button to override to.
 * **Override Axis:** The Vector2 axis to override to.
 * **Interact Grab Script:** The Interact Grab script to override the controls on.
 * **Interact Grab Overrides:** The list of overrides.
 * **Interact Use Script:** The Interact Use script to override the controls on.
 * **Interact Use Overrides:** The list of overrides.
 * **Pointer Script:** The Pointer script to override the controls on.
 * **Pointer Activation Overrides:** The list of overrides for the activation button.
 * **Pointer Selection Overrides:** The list of overrides for the selection button.
 * **Ui Pointer Script:** The UI Pointer script to override the controls on.
 * **Ui Pointer Activation Overrides:** The list of overrides for the activation button.
 * **Ui Pointer Selection Overrides:** The list of overrides for the selection button.
 * **Pointer Direction Indicator Script:** The Pointer Direction Indicator script to override the controls on.
 * **Direction Indicator Coordinate Overrides:** The list of overrides for the coordinate axis.
 * **Touchpad Control Script:** The Touchpad Control script to override the controls on.
 * **Touchpad Control Coordinate Overrides:** The list of overrides for the Touchpad Control coordinate axis.
 * **Touchpad Control Activation Overrides:** The list of overrides for the activation button.
 * **Touchpad Control Modifier Overrides:** The list of overrides for the modifier button.
 * **Button Control Script:** The ButtonControl script to override the controls on.
 * **Button Control Forward Overrides:** The list of overrides for the forward button.
 * **Button Control Backward Overrides:** The list of overrides for the backward button.
 * **Button Control Left Overrides:** The list of overrides for the left button.
 * **Button Control Right Overrides:** The list of overrides for the right button.
 * **Slingshot Jump Script:** The SlingshotJump script to override the controls on.
 * **Slingshot Jump Activation Overrides:** The list of overrides for the activation button.
 * **Slingshot Jump Cancel Overrides:** The list of overrides for the cancel button.
 * **Move In Place Script:** The MoveInPlace script to override the controls on.
 * **Move In Place Engage Overrides:** The list of overrides for the engage button.
 * **Step Multiplier Script:** The Step Multiplier script to override the controls on.
 * **Step Multiplier Activation Overrides:** The list of overrides for the activation button.

### Class Methods

#### ForceManage/0

  > `public virtual void ForceManage()`

 * Parameters
   * _none_
 * Returns
   * _none_

The ForceManage method forces the inputs to be updated even without an SDK change event occuring.

---

## Velocity Estimator (VRTK_VelocityEstimator)

### Overview

The Velocity Estimator is used to calculate an estimated velocity on a moving object that is moving outside of Unity physics.

Objects that have rigidbodies and use Unity physics to move around will automatically provide accurate velocity and angular velocity information.

Any object that is moved around using absolute positions or moving the transform position will not be able to provide accurate velocity or angular velocity information.
When the Velocity Estimator script is applied to any GameObject it will provide a best case estimation of what the object's velocity and angular velocity is based on a given number of position and rotation samples.
The more samples used, the higher the precision but the script will be more demanding on processing time.

### Inspector Parameters

 * **Auto Start Sampling:** Begin the sampling routine when the script is enabled.
 * **Velocity Average Frames:** The number of frames to average when calculating velocity.
 * **Angular Velocity Average Frames:** The number of frames to average when calculating angular velocity.

### Class Methods

#### StartEstimation/0

  > `public virtual void StartEstimation()`

 * Parameters
   * _none_
 * Returns
   * _none_

The StartEstimation method begins logging samples of position and rotation for the GameObject.

#### EndEstimation/0

  > `public virtual void EndEstimation()`

 * Parameters
   * _none_
 * Returns
   * _none_

The EndEstimation method stops logging samples of position and rotation for the GameObject.

#### GetVelocityEstimate/0

  > `public virtual Vector3 GetVelocityEstimate()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - The velocity estimate vector of the GameObject

The GetVelocityEstimate method returns the current velocity estimate.

#### GetAngularVelocityEstimate/0

  > `public virtual Vector3 GetAngularVelocityEstimate()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - The angular velocity estimate vector of the GameObject

The GetAngularVelocityEstimate method returns the current angular velocity estimate.

#### GetAccelerationEstimate/0

  > `public virtual Vector3 GetAccelerationEstimate()`

 * Parameters
   * _none_
 * Returns
   * `Vector3` - The acceleration estimate vector of the GameObject

The GetAccelerationEstimate method returns the current acceleration estimate.

---

# Base SDK (VRTK/Source/SDK/Base)

The base scripts used to determine the interface for interacting with a Unity VR SDK.

 * [SDK Base](#sdk-base-sdk_base)
 * [SDK Description](#sdk-description-sdk_descriptionattribute)
 * [SDK Scripting Define Symbol Predicate](#sdk-scripting-define-symbol-predicate-sdk_scriptingdefinesymbolpredicateattribute)
 * [Base System](#base-system-sdk_basesystem)
 * [Base Headset](#base-headset-sdk_baseheadset)
 * [Base Controller](#base-controller-sdk_basecontroller)
 * [Base Boundaries](#base-boundaries-sdk_baseboundaries)

---

## SDK Base (SDK_Base)
 > extends ScriptableObject

### Overview

Abstract superclass that defines that a particular class is an SDK.

This is an abstract class to mark all different SDK endpoints with. This is used to allow for type safety when talking about 'an SDK' instead of one of the different endpoints (System, Boundaries, Headset, Controller).

### Class Methods

#### OnBeforeSetupLoad/1

  > `public virtual void OnBeforeSetupLoad(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just before loading the VRTK_SDKSetup that's using this SDK.

#### OnAfterSetupLoad/1

  > `public virtual void OnAfterSetupLoad(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just after loading the VRTK_SDKSetup that's using this SDK.

#### OnBeforeSetupUnload/1

  > `public virtual void OnBeforeSetupUnload(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just before unloading the VRTK_SDKSetup that's using this SDK.

#### OnAfterSetupUnload/1

  > `public virtual void OnAfterSetupUnload(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just after unloading the VRTK_SDKSetup that's using this SDK.

---

## SDK Description (SDK_DescriptionAttribute)
 > extends Attribute

### Overview

Describes a class that represents an SDK. Only allowed on classes that inherit from SDK_Base.

### Class Variables

 * `public readonly string prettyName` - The pretty name of the SDK. Uniquely identifies the SDK.
 * `public readonly string symbol` - The scripting define symbol needed for the SDK. Needs to be the same as `SDK_ScriptingDefineSymbolPredicateAttribute.symbol` to add and remove the scripting define symbol automatically using VRTK_SDKManager.
 * `public readonly string vrDeviceName` - The name of the VR Device to load.
 * `public readonly int index` - The index of this attribute, in case there are multiple on the same target.
 * `public BuildTargetGroup buildTargetGroup` - The build target group this SDK is for.
 * `public bool describesFallbackSDK` - Whether this description describes a fallback SDK.

### Class Methods

#### SDK_DescriptionAttribute/5

  > `public SDK_DescriptionAttribute(string prettyName, string symbol, string vrDeviceName, string buildTargetGroupName, int index = 0)`

 * Parameters
   * `string prettyName` - The pretty name of the SDK. Uniquely identifies the SDK. `null` and `string.Empty` aren't allowed.
   * `string symbol` - The scripting define symbol needed for the SDK. Needs to be the same as `SDK_ScriptingDefineSymbolPredicateAttribute.symbol` to add and remove the scripting define symbol automatically using VRTK_SDKManager. `null` and `string.Empty` are allowed.
   * `string vrDeviceName` - The name of the VR Device to load. Set to `null` or `string.Empty` if no VR Device is needed.
   * `string buildTargetGroupName` - The name of a constant of `BuildTargetGroup`. `BuildTargetGroup.Unknown`, `null` and `string.Empty` are not allowed.
   * `int index` - The index of this attribute, in case there are multiple on the same target.
 * Returns
   * _none_

Creates a new attribute.

#### SDK_DescriptionAttribute/2

  > `public SDK_DescriptionAttribute(Type typeToCopyExistingDescriptionFrom, int index = 0)`

 * Parameters
   * `Type typeToCopyExistingDescriptionFrom` - The type to copy the existing SDK_DescriptionAttribute from. `null` is not allowed.
   * `int index` - The index of the description to copy from the the existing SDK_DescriptionAttribute.
 * Returns
   * _none_

Creates a new attribute by copying from another attribute on a given type.

---

## SDK Scripting Define Symbol Predicate (SDK_ScriptingDefineSymbolPredicateAttribute)
 > extends Attribute, ISerializationCallbackReceiver

### Overview

Specifies a method to be used as a predicate to allow VRTK_SDKManager to automatically add and remove scripting define symbols. Only allowed on static methods that take no arguments and return a `bool`.

### Class Variables

 * `public const string RemovableSymbolPrefix` - The prefix of scripting define symbols that must be used to be able to automatically remove the symbols. Default: `"VRTK_DEFINE_"`
 * `public string symbol` - The scripting define symbol to conditionally add or remove.
 * `public BuildTargetGroup buildTargetGroup` - The build target group to use when conditionally adding or removing symbol.

### Class Methods

#### SDK_ScriptingDefineSymbolPredicateAttribute/2

  > `public SDK_ScriptingDefineSymbolPredicateAttribute(string symbol, string buildTargetGroupName)`

 * Parameters
   * `string symbol` - The scripting define symbol to conditionally add or remove. Needs to start with `RemovableSymbolPrefix` to be able to automatically remove the symbol. `null` and `string.Empty` are not allowed.
   * `string buildTargetGroupName` - The name of a constant of `BuildTargetGroup`. `BuildTargetGroup.Unknown`, `null` and `string.Empty` are not allowed.
 * Returns
   * _none_

Creates a new attribute.

#### SDK_ScriptingDefineSymbolPredicateAttribute/1

  > `public SDK_ScriptingDefineSymbolPredicateAttribute(SDK_ScriptingDefineSymbolPredicateAttribute attributeToCopy)`

 * Parameters
   * `SDK_ScriptingDefineSymbolPredicateAttribute attributeToCopy` - The attribute to copy.
 * Returns
   * _none_

Creates a new attribute by copying an existing one.

---

## Base System (SDK_BaseSystem)
 > extends [SDK_Base](#sdk-base-sdk_base)

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
 > extends [SDK_Base](#sdk-base-sdk_base)

### Overview

The Base Headset SDK script provides a bridge to SDK methods that deal with the VR Headset.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Variables

 * `public enum HeadsetType` - The connected headset type
   * `Undefined` - The headset connected is unknown.
   * `Simulator` - The headset associated with the simulator.
   * `HTCVive` - The HTC Vive headset.
   * `OculusRiftDK1` - The Oculus Rift DK1 headset.
   * `OculusRiftDK2` - The Oculus Rift DK2 headset.
   * `OculusRift` - The Oculus Rift headset.
   * `OculusGearVR` - The Oculus GearVR headset.
   * `GoogleDaydream` - The Google Daydream headset.
   * `GoogleCardboard` - The Google Cardboard headset.
   * `HyperealVR` - The HyperealVR headset.
   * `WindowsMixedReality` - The Windows Mixed Reality headset.

### Class Methods

#### ProcessUpdate/1

  > `public abstract void ProcessUpdate(Dictionary<string, object> options);`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public abstract void ProcessFixedUpdate(Dictionary<string, object> options);`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public abstract string GetHeadsetType();`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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
 > extends [SDK_Base](#sdk-base-sdk_base)

### Overview

The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.

This is an abstract class to implement the interface required by all implemented SDKs.

### Class Variables

 * `public enum ButtonTypes` - Types of buttons on a controller
   * `ButtonOne` - Button One on the controller.
   * `ButtonTwo` - Button Two on the controller.
   * `Grip` - Grip on the controller.
   * `GripHairline` - Grip Hairline on the controller.
   * `StartMenu` - Start Menu on the controller.
   * `Trigger` - Trigger on the controller.
   * `TriggerHairline` - Trigger Hairline on the controller.
   * `Touchpad` - Touchpad on the controller.
   * `TouchpadTwo` - Touchpad Two on the controller.
   * `MiddleFinger` - Middle Finger on the controller.
   * `RingFinger` - Ring Finger on the controller.
   * `PinkyFinger` - Pinky Finger on the controller.
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
 * `public enum ControllerType` - SDK Controller types.
   * `Undefined` - No controller type.
   * `Custom` - A custom controller type.
   * `Simulator_Hand` - The Simulator default hand controller.
   * `SteamVR_ViveWand` - The HTC Vive wand controller for SteamVR.
   * `SteamVR_OculusTouch` - The Oculus Touch controller for SteamVR.
   * `Oculus_OculusTouch` - The Oculus Touch controller for Oculus Utilities.
   * `Daydream_Controller` - The Daydream controller for Google Daydream SDK.
   * `Ximmerse_Flip` - The Flip controller for Ximmerse SDK.
   * `SteamVR_ValveKnuckles` - The Valve Knuckles controller for SteamVR.
   * `Oculus_OculusGamepad` - The Oculus Gamepad for Oculus Utilities.
   * `Oculus_OculusRemote` - The Oculus Remote for Oculus Utilities.
   * `Oculus_GearVRHMD` - The Oculus GearVR HMD controls for Oculus Utilities.
   * `Oculus_GearVRController` - The Oculus GearVR controller for Oculus Utilities.
   * `WindowsMR_MotionController` - The Windows Mixed Reality Motion Controller for Windows Mixed Reality.
   * `SteamVR_WindowsMRController` - The Windows Mixed Reality Motion Controller for SteamVR.

### Class Methods

#### ProcessUpdate/2

  > `public abstract void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public abstract void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public abstract ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public abstract Transform GetControllerOrigin(VRTK_ControllerReference controllerReference);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public abstract bool WaitForControllerModel(ControllerHand hand);`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

#### GetControllerModelHand/1

  > `public virtual ControllerHand GetControllerModelHand(GameObject controllerModel)`

 * Parameters
   * `GameObject controllerModel` - The controller model GameObject to get the hand for.
 * Returns
   * `ControllerHand` - The hand enum for which the given controller model is for.

The GetControllerModelHand method returns the hand for the given controller model GameObject.

#### GetControllerRenderModel/1

  > `public abstract GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public abstract void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public abstract bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public abstract SDK_ControllerHapticModifiers GetHapticModifiers();`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public abstract Vector3 GetVelocity(VRTK_ControllerReference controllerReference);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public abstract Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference);`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public abstract bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity);`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public abstract Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public abstract float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public abstract float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public abstract bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference);`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Base Boundaries (SDK_BaseBoundaries)
 > extends [SDK_Base](#sdk-base-sdk_base)

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

#### GetPlayAreaVertices/0

  > `public abstract Vector3[] GetPlayAreaVertices();`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public abstract float GetPlayAreaBorderThickness();`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public abstract bool IsPlayAreaSizeCalibrated();`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public abstract bool GetDrawAtRuntime();`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public abstract void SetDrawAtRuntime(bool value);`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# Fallback SDK (VRTK/Source/SDK/Fallback)

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

The Fallback Headset SDK script provides a fallback collection of methods that return null or default headset values.

This is the fallback class that will just return default values.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

The Fallback Controller SDK script provides a fallback collection of methods that return null or default headset values.

This is the fallback class that will just return default values.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Fallback Boundaries (SDK_FallbackBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The Fallback Boundaries SDK script provides a fallback collection of methods that return null or default headset values.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# Unity SDK (VRTK/Source/SDK/Unity)

The scripts used to utilise the built in UnityEngine.VR SDK.

 * [Unity System](#unity-system-sdk_unitysystem)
 * [Unity Headset](#unity-headset-sdk_unityheadset)
 * [Unity Controller](#unity-controller-sdk_unitycontroller)
 * [Unity Boundaries](#unity-boundaries-sdk_unityboundaries)
 * [Unity SDK Controller Tracker](#unity-sdk-controller-tracker-sdk_unitycontrollertracker)
 * [Unity SDK Headset Tracker](#unity-sdk-headset-tracker-sdk_unityheadsettracker)

---

## Unity System (SDK_UnitySystem)
 > extends [SDK_BaseSystem](#base-system-sdk_basesystem)

### Overview

The Unity System SDK script provides a bridge to the Unity SDK.

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

## Unity Headset (SDK_UnityHeadset)
 > extends [SDK_BaseHeadset](#base-headset-sdk_baseheadset)

### Overview

The Unity Headset SDK script provides a bridge to the base Unity headset support.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

## Unity Controller (SDK_UnityController)
 > extends [SDK_BaseController](#base-controller-sdk_basecontroller)

### Overview

The Unity Controller SDK script provides a bridge  to the base Unity input device support.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Unity Boundaries (SDK_UnityBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The Unity Boundaries SDK script provides a bridge to a default Unity play area.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

## Unity SDK Controller Tracker (SDK_UnityControllerTracker)

### Overview

The Controller Tracker enables the GameObject to track it's position/rotation to the available connected VR Controller via the `UnityEngine.VR` library.

The Unity Controller Tracker is attached to the `[UnityBase_CameraRig]` prefab on the child `LeftHandAnchor` and `RightHandAnchor` to enable controller tracking.

### Inspector Parameters

 * **Node Type:** The Unity VRNode to track.
 * **Index:** The unique index to assign to the controller.
 * **Trigger Axis Name:** The Unity Input name for the trigger axis.
 * **Grip Axis Name:** The Unity Input name for the grip axis.
 * **Touchpad Horizontal Axis Name:** The Unity Input name for the touchpad horizontal axis.
 * **Touchpad Vertical Axis Name:** The Unity Input name for the touchpad vertical axis.

---

## Unity SDK Headset Tracker (SDK_UnityHeadsetTracker)

### Overview

The Headset Tracker enables the GameObject to track it's position/rotation to the available connected VR HMD via the `UnityEngine.VR` library.

The Unity Headset Tracker is attached to the `[UnityBase_CameraRig]` prefab on the child `Head` HMD tracking.

---

# Simulator SDK (VRTK/Source/SDK/Simulator)

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

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Simulator Boundaries (SDK_SimBoundaries)
 > extends [SDK_BaseBoundaries](#base-boundaries-sdk_baseboundaries)

### Overview

The Sim Boundaries SDK script provides dummy functions for the play area boundaries.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# SteamVR SDK (VRTK/Source/SDK/SteamVR)

The scripts used to utilise the SteamVR Unity Plugin SDK.

 * [SteamVR Defines](#steamvr-defines-sdk_steamvrdefines)
 * [SteamVR System](#steamvr-system-sdk_steamvrsystem)
 * [SteamVR Headset](#steamvr-headset-sdk_steamvrheadset)
 * [SteamVR Controller](#steamvr-controller-sdk_steamvrcontroller)
 * [SteamVR Boundaries](#steamvr-boundaries-sdk_steamvrboundaries)

---

## SteamVR Defines (SDK_SteamVRDefines)

### Overview

Handles all the scripting define symbols for the SteamVR SDK.

### Class Variables

 * `public const string ScriptingDefineSymbol` - The scripting define symbol for the SteamVR SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_STEAMVR"`

---

## SteamVR System (SDK_SteamVRSystem)

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

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

### Overview

The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### OnAfterSetupUnload/1

  > `public override void OnAfterSetupUnload(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just after unloading the VRTK_SDKSetup that's using this SDK.

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## SteamVR Boundaries (SDK_SteamVRBoundaries)

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# Oculus SDK (VRTK/Source/SDK/Oculus)

The scripts used to utilise the Oculus Utilities Unity Package SDK.

 * [Oculus Defines](#oculus-defines-sdk_oculusdefines)
 * [Oculus System](#oculus-system-sdk_oculussystem)
 * [Oculus Headset](#oculus-headset-sdk_oculusheadset)
 * [Oculus Controller](#oculus-controller-sdk_oculuscontroller)
 * [Oculus Boundaries](#oculus-boundaries-sdk_oculusboundaries)

---

## Oculus Defines (SDK_OculusDefines)

### Overview

Handles all the scripting define symbols for the Oculus and Avatar SDKs.

### Class Variables

 * `public const string ScriptingDefineSymbol` - The scripting define symbol for the Oculus SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUS"`
 * `public const string AvatarScriptingDefineSymbol` - The scripting define symbol for the Oculus Avatar SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_OCULUS_AVATAR"`

---

## Oculus System (SDK_OculusSystem)

### Overview

The Oculus System SDK script provides a bridge to the Oculus SDK.

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

## Oculus Headset (SDK_OculusHeadset)

### Overview

The Oculus Headset SDK script provides a bridge to the Oculus SDK.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

## Oculus Controller (SDK_OculusController)

### Overview

The Oculus Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### OnAfterSetupLoad/1

  > `public override void OnAfterSetupLoad(VRTK_SDKSetup setup)`

 * Parameters
   * `VRTK_SDKSetup setup` - The SDK Setup which is using this SDK.
 * Returns
   * _none_

This method is called just after loading the VRTK_SDKSetup that's using this SDK.

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Oculus Boundaries (SDK_OculusBoundaries)

### Overview

The Oculus Boundaries SDK script provides a bridge to the Oculus SDK play area.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

#### GetAvatar/0

  > `public virtual OvrAvatar GetAvatar()`

 * Parameters
   * _none_
 * Returns
   * `OvrAvatar` - The OvrAvatar script for managing the Oculus Avatar.

The GetAvatar method is used to retrieve the Oculus Avatar object if it exists in the scene. This method is only available if the Oculus Avatar package is installed.

---

# Daydream SDK (VRTK/Source/SDK/Daydream)

The scripts used to utilise the Google VR SDK for Unity.

 * [Daydream Defines](#daydream-defines-sdk_daydreamdefines)
 * [Daydream System](#daydream-system-sdk_daydreamsystem)
 * [Daydream Headset](#daydream-headset-sdk_daydreamheadset)
 * [Daydream Controller](#daydream-controller-sdk_daydreamcontroller)
 * [Daydream Boundaries](#daydream-boundaries-sdk_daydreamboundaries)

---

## Daydream Defines (SDK_DaydreamDefines)

### Overview

Handles all the scripting define symbols for the Daydream SDK.

### Class Variables

 * `public const string ScriptingDefineSymbol` - The scripting define symbol for the Daydream SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_DAYDREAM"`

---

## Daydream System (SDK_DaydreamSystem)

### Overview

The Daydream System SDK script provides dummy functions for system functions.

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

## Daydream Headset (SDK_DaydreamHeadset)

### Overview

The Daydream Headset SDK script provides dummy functions for the headset.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

## Daydream Controller (SDK_DaydreamController)

### Overview

The Daydream Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Daydream Boundaries (SDK_DaydreamBoundaries)

### Overview

The Daydream Boundaries SDK script provides dummy functions for the play area boundaries.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# Ximmerse SDK (VRTK/Source/SDK/Ximmerse)

The scripts used to utilise the Ximmerse SDK for Unity.

 * [Ximmerse Defines](#ximmerse-defines-sdk_ximmersedefines)
 * [Ximmerse System](#ximmerse-system-sdk_ximmersesystem)
 * [Ximmerse Headset](#ximmerse-headset-sdk_ximmerseheadset)
 * [Ximmerse Controller](#ximmerse-controller-sdk_ximmersecontroller)
 * [Ximmerse Boundaries](#ximmerse-boundaries-sdk_ximmerseboundaries)

---

## Ximmerse Defines (SDK_XimmerseDefines)

### Overview

Handles all the scripting define symbols for the Ximmerse SDK.

### Class Variables

 * `public const string ScriptingDefineSymbol` - The scripting define symbol for the Ximmerse SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_XIMMERSE"`

---

## Ximmerse System (SDK_XimmerseSystem)

### Overview

The Ximmerse System SDK script provides a bridge to the Ximmerse SDK.

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

## Ximmerse Headset (SDK_XimmerseHeadset)

### Overview

The Ximmerse Headset SDK script provides a bridge to the Ximmerse SDK.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

## Ximmerse Controller (SDK_XimmerseController)

### Overview

The Ximmerse Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## Ximmerse Boundaries (SDK_XimmerseBoundaries)

### Overview

The Ximmerse Boundaries SDK script provides a bridge to the Ximmerse SDK play area.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

# HyperealVR SDK (VRTK/Source/SDK/HyperealVR)

The scripts used to utilise the HyperealVR SDK for Unity.

 * [HyperealVR Defines](#hyperealvr-defines-sdk_hyperealvrdefines)
 * [HyperealVR System](#hyperealvr-system-sdk_hyperealvrsystem)
 * [HyperealVR Headset](#hyperealvr-headset-sdk_hyperealvrheadset)
 * [HyperealVR Controller](#hyperealvr-controller-sdk_hyperealvrcontroller)
 * [HyperealVR Boundaries](#hyperealvr-boundaries-sdk_hyperealvrboundaries)

---

## HyperealVR Defines (SDK_HyperealVRDefines)

### Overview

Handles all the scripting define symbols for the Hypereal SDK.

### Class Variables

 * `public const string ScriptingDefineSymbol` - The scripting define symbol for the Hypereal SDK. Default: `SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix +`

---

## HyperealVR System (SDK_HyperealVRSystem)

### Overview

The HyperealVR System SDK script provides a bridge to the HyperealVR SDK.

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

## HyperealVR Headset (SDK_HyperealVRHeadset)

### Overview

The HyperealVR Headset SDK script provides a bridge to the HyperealVR SDK.

### Class Methods

#### ProcessUpdate/1

  > `public override void ProcessUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/1

  > `public override void ProcessFixedUpdate(Dictionary<string, object> options)`

 * Parameters
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

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

#### GetHeadsetType/0

  > `public override string GetHeadsetType()`

 * Parameters
   * _none_
 * Returns
   * `string` - The string of the headset connected.

The GetHeadsetType method returns a string representing the type of headset connected.

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

## HyperealVR Controller (SDK_HyperealVRController)

### Overview

The HyperealVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.

### Class Methods

#### ProcessUpdate/2

  > `public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the update.
 * Returns
   * _none_

The ProcessUpdate method enables an SDK to run logic for every Unity Update

#### ProcessFixedUpdate/2

  > `public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference for the controller.
   * `Dictionary<string, object> options` - A dictionary of generic options that can be used to within the fixed update.
 * Returns
   * _none_

The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate

#### GetCurrentControllerType/1

  > `public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get type of.
 * Returns
   * `ControllerType` - The ControllerType based on the SDK and headset being used.

The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.

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
   * `GameObject` - The GameObject of the controller

The GetControllerByIndex method returns the GameObject of a controller with a specific index.

#### GetControllerOrigin/1

  > `public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The controller to retrieve the origin from.
 * Returns
   * `Transform` - A Transform containing the origin of the controller.

The GetControllerOrigin method returns the origin of the given controller.

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

#### WaitForControllerModel/1

  > `public override bool WaitForControllerModel(ControllerHand hand)`

 * Parameters
   * `ControllerHand hand` - The hand to determine if the controller model will be ready for.
 * Returns
   * `bool` - Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.

The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.

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

  > `public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The GameObject to check.
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

#### HapticPulse/2

  > `public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `float strength` - The intensity of the rumble of the controller motor. `0` to `1`.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.

#### HapticPulse/2

  > `public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to initiate the haptic pulse on.
   * `AudioClip clip` - The audio clip to use for the haptic pattern.
 * Returns
   * _none_

The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.

#### GetHapticModifiers/0

  > `public override SDK_ControllerHapticModifiers GetHapticModifiers()`

 * Parameters
   * _none_
 * Returns
   * `SDK_ControllerHapticModifiers` - An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.

The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.

#### GetVelocity/1

  > `public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current velocity of the tracked object.

The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.

#### GetAngularVelocity/1

  > `public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)`

 * Parameters
   * `VRTK_ControllerReference controllerReference` - The reference to the tracked object to check for.
 * Returns
   * `Vector3` - A Vector3 containing the current angular velocity of the tracked object.

The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.

#### IsTouchpadStatic/4

  > `public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)`

 * Parameters
   * `Vector2 currentAxisValues` -
   * `Vector2 previousAxisValues` -
   * `int compareFidelity` -
 * Returns
   * `bool` - Returns true if the touchpad is not currently being touched or moved.

The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.

#### GetButtonAxis/2

  > `public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button axis on.
 * Returns
   * `Vector2` - A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.

The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.

#### GetButtonSenseAxis/2

  > `public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the sense axis on.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the sense axis on.
 * Returns
   * `float` - The current sense axis value.

The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.

#### GetButtonHairlineDelta/2

  > `public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to get the hairline delta for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to get the hairline delta for.
 * Returns
   * `float` - The delta between the button presses.

The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.

#### GetControllerButtonState/3

  > `public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)`

 * Parameters
   * `ButtonTypes buttonType` - The type of button to check for the state of.
   * `ButtonPressTypes pressType` - The button state to check for.
   * `VRTK_ControllerReference controllerReference` - The reference to the controller to check the button state on.
 * Returns
   * `bool` - Returns true if the given button is in the state of the given press type on the given controller reference.

The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.

---

## HyperealVR Boundaries (SDK_HyperealVRBoundaries)

### Overview

The HyperealVR Boundaries SDK script provides a bridge to the HyperealVR SDK play area.

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

#### GetPlayAreaVertices/0

  > `public override Vector3[] GetPlayAreaVertices()`

 * Parameters
   * _none_
 * Returns
   * `Vector3[]` - A Vector3 array of the points in the scene that represent the play area boundaries.

The GetPlayAreaVertices method returns the points of the play area boundaries.

#### GetPlayAreaBorderThickness/0

  > `public override float GetPlayAreaBorderThickness()`

 * Parameters
   * _none_
 * Returns
   * `float` - The thickness of the drawn border.

The GetPlayAreaBorderThickness returns the thickness of the drawn border for the given play area.

#### IsPlayAreaSizeCalibrated/0

  > `public override bool IsPlayAreaSizeCalibrated()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the play area size has been auto calibrated and set by external sensors.

The IsPlayAreaSizeCalibrated method returns whether the given play area size has been auto calibrated by external sensors.

#### GetDrawAtRuntime/0

  > `public override bool GetDrawAtRuntime()`

 * Parameters
   * _none_
 * Returns
   * `bool` - Returns true if the drawn border is being displayed.

The GetDrawAtRuntime method returns whether the given play area drawn border is being displayed.

#### SetDrawAtRuntime/1

  > `public override void SetDrawAtRuntime(bool value)`

 * Parameters
   * `bool value` - The state of whether the drawn border should be displayed or not.
 * Returns
   * _none_

The SetDrawAtRuntime method sets whether the given play area drawn border should be displayed at runtime.

---

