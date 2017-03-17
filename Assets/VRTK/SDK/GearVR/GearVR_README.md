## VRTK Samsung Gear VR Support
_Ryan Dawson (@coderdawson), Giovanni Paolo Viganò (@gpvigano)_

Samsung Gear VR is a mobile, Android-based VR platform that is extremely limited when compared to other desktop-based VR platforms such as the HTC Vive and Oculus Rift. The Gear VR headset supports orientation tracking only; it does not support position tracking. For input, the Gear VR headset has a very simple touchpad on its right side; it does not support motion or pointer based controllers. The touchpad supports the detection of a tap/click and can also be used for some simple directional swiping. The headset also includes a back button. Most implementations on the Gear VR rely on gaze-based pointers and touchpad tapping for application input.

Currently, Samsung Gear VR requires a Samsung Galaxy S6, S6 Edge, S6 Edge+, S7, S7 Edge, or Note 5 handset. With a mobile device used as the Gear VR's main processing unit, the performance capabilities are also extremely limited when compared with other desktop-base VR platforms. Special care must be taken to ensure your application can run at 60 fps. It is recommended that your application target between 50 and 100 draw calls per frame and stay below 100,000 vertices. 

**BE WARNED**: Mobile VR development is not for the faint of heart. Performance issues can arise with the simplist of visuals if they are not properly optimized. On top of this, VRTK was developed primarily for desktop-based VR platforms and mobile VR specific optimizations may be required in order to fully utilize it such a platform. 

### Pre-requisites

Developing for Samsung Gear VR requires that your environment have both Unity and your development phone set up first. Follow the steps/links below to configure your development environment and phone.

 1. [Setup Unity for Android development](https://docs.unity3d.com/Manual/android-sdksetup.html)
 2. [Install the USB drivers for your phone.](https://developer.android.com/studio/run/oem-usb.html)
 3. Enable developer settings on your phone by following [this guide](https://developer.android.com/studio/run/device.html#device-developer-options), then enable USB debugging on the developer menu.

### Setup

For simple VR support, all that is required is to turn on Unity's `Virtual Reality Supported` project setting and place a GameObject with the Camera component on it in your scene and tag the GameObject with the `MainCamera` tag. However, in order to support features like teleporting you must place your camera under a parent GameObject so the camera can be moved. Follow the steps below to configure your scene for Gear VR deployment with VRTK feature support.
* Basic Setup
  * Create a GameObject, named `[CameraRig]`, positioned where the **feet** of the player should be located in your virtual world. This GameObject will be the parent for our camera.
    * **NOTE**: Ensure that the `CameraRig` Transform's Y value is set to the floor level (actually the "feet level").
    * **NOTE**: Since Gear VR is a non-position tracked platform it is important that the position of the `[CameraRig]` be at the position at which you would like the player to be in your world. A navigation technique can be used to move around, like teleporting or walking (beware of motion sickness).
  * Create a Camera GameObject (GameObject with a `Camera` component) as a child of the `CameraRig` GameObject previously created.
    * **NOTE**: This Camera GameObject must be named `Camera`, `Main Camera`, or `Headset`.
    * **NOTE**: Ensure that the Camera Transform's Y value is set to the user height (actually the "eyes level") with zero for both the X and Z value on the Transform. The Camera (local) position is actually the offset of eyes from avatar's feet.
  * Create a GameObject in your scene called `[VRTK]` and add the `VRTK_SDKManager` to it.
  * Select `Gear VR` from the `Quick select SDK` drop-down on the `VRTK_SDKManager` component inspector and wait for your scripts to be recompiled.
  * Click the `Auto Populate Linked Objects` button on the `VRTK_SDKManager` inspector to find the relevant Linked Objects.
    * You should have `Actual Boundaries` set to `[CameraRig]` and `Actual Headset` set to `Camera` (assuming that this is the name of your camera game object)
  * Create a game object named `Headset` as child of `[VRTK]`
    * Add to it a VRTK_Transform Follow component
      * Set `Game Object To Follow` to `Camera` (drag your camera game object to this field)
      * Set `Game Object To Change` to `Headset` or leave it empty (it will be automatically set)
    * Add a `VRTK_ControllerEvents` component
      * **NOTE**: Only some aliases are used by GearVR. `Touchpad Touch`, `Touchpad Press`, `Button One Touch`, `Button One Press` are routed to D-Pad touch, `Button Two Press` and `Button Two Touch` are routed to back button press (holding down the back button activates the system menu).
* Gaze Pointer Setup
  * Add a `VRTK_Pointer` component to the `Headset` game object.
    * Set `Controller` to the `VRTK_ControllerEvents` component of your camera GameObject (drag your camera game object to the `Controller` slot).
component.
    * Check `Activate On Enable` and uncheck `Hold Button To Activate` if you want the pointer always visible.
  * Add a `VRTK_StraightPointerRenderer` component to your `Headset` game object.
    * Set `Tracer Visibility` to `Always Off`
    * Set the `Pointer Renderer` object field of the `VRTK_Pointer` to this component (drag your camera game object to the `Pointer Renderer` slot)
* Gaze Pointer Highlighting/Grabbing
  * Add a `VRTK_InteractTouch` component to your `Headset` game object.
  * Add a `VRTK_InteractGrab` component to your `Headset` game object.
  * Check the `Interact With Objects` and `Grab To Pointer Tip` checkboxes on the `VRTK_Pointer` component.
  * If you are using also the teleport you can use this configuration:
     * in the `VRTK_Pointer` component
       * set `Selection Button` to `Button Two Press`
     * set `Pointer Set Button` to `Button Two Press`
     * `VRTK_ControllerEvents` component
  * Check each interactive object:
    * Set `Allowed Grab Controllers` to `Both` on the `VRTK_Interactable Object` component of each object.
    * **NOTE**: Probably setting `Precision Grab` on the `VRTK_Fixed Joint Grap Attach` component of each interactive object makes the interaction easier.
* UI Pointer setup
  * Set up the Gaze Pointer
  * Add a `VRTK_UI_Pointer` component to the `Headset` game object
    * Set `Activation Button` to `Undefined`
    * Set `Activation Mode` to `Always On`
    * Set `Selection Button` to `Touchpad touch`
  * In the `VRTK_ControllerEvents` component set `UI Click Button` to `Touchpad touch`
Deploying and running your project on your development device for Gear VR:
* switch your Unity project platform to Android
* copy special OSIG file additions (e.g. `oculussig_*`) to your project under `Assets/Plugins/Android/Assets/`
* make some changes to Unity Player Settings (Other Settings)
  * Rendering
    * uncheck *Auto Graphics API*
    * remove *OpenGLES2* from the *Graphics APIs* list
  * Identification
    * set *Bundle Identifier*
    * set *Minimum API Level* to **API level 18** or higher

Follow the [detailed instructions on the Oculus website](https://developer3.oculus.com/documentation/game-engines/latest/concepts/unity-build-android/) to get your application running on device.

You can also test a GearVR setup directly in the Editor, switching to Android platform and pressing Play:
* Move your camera in the Editor (or attach a proper script to move it with the keyboard/mouse)
* Left mouse button is routed to the D-Pad touch
* ESC key is routed to the D-Pad back button

### VRTK Gear VR Controller Support

The Oculus Mobile SDK maps the tap of the Gear VR D-Pad to Button.One and the back button to Button.Two in the [OVRInput class](https://developer3.oculus.com/documentation/game-engines/latest/concepts/unity-ovrinput/). Unity maps the inputs via the Input class with button names "Fire1" and "Cancel".

The closer analogy with the original htc Vive controller is D-Pad==Touchpad, Back==Menu (Back held down==System menu).
This is the suggested mapping, but, to make working with VRTK on the Gear VR a bit simpler, the inputs are mapped to allow compatibility with other VRTK platform button mappings so the tap of the touchpad works for several VRTK button aliases as follows:

**Touchpad Tap (Button One Mappings)**
* Touchpad_Touch
* Touchpad_Press
* Button_One_Touch
* Button_One_Press

**Back Button (Button Two Mappings)**
* Button_Two_Touch
* Button_Two_Press