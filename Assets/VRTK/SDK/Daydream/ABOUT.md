## Daydream SDK for VRTK
_Jonathan Linowes (@linojon)_

The Daydream SDK for VRTK integrates the Daydream platform with VRTK. It can use Unity's built-in VR supported Camera with Daydream support. 

Daydream controllers are signficantly limited compared with positionally tracked hand controllers like HTC Vive Lighthouse and Oculus Rift Touch.
The Daydream controller hardware provides 3 DOF (x y z axis rotation) and accellerometer. However, some (limited) 3-space positioning can be simulated. The Google VR (Gvr) SDK provides an Arm Model that simulates transforms for shoulder, elbow, wrist and pointer joints. Your application can still use these with VRTK, just supply an object with the GvrArmModelOffsets component to the VRTK_LinkedObjects, VRTK_SimplerPointer or other appropriate component. 

Further, this VRTK Daydream SDK provides a DaydreamReach component to facilitate reaching out to touch and grab. It uses the touchpad to push the controller position out in front of user along the current orientation plane or pointer axis.

### Pre-requisites

For first time users, see the Unity docs [Getting started with Android development](https://docs.unity3d.com/Manual/android-GettingStarted.html) and Google documentation [Get Started Guides for Android Daydream](https://developers.google.com/vr/unity/get-started-controller). In particular, 

* Use a version of Unity that natively support Google VR with Daydream (as of Jan 30, 2017, 5.4.2f2-GVR13 [Daydream technical preview](https://unity3d.com/partners/google/daydream#section-download). Note Unity 5.6.0b4 build is not stable with thie SDK). Install with "Android Build Support"
*  If this is your first time developing Android applications in Unity, follow Unity's [Android SDK setup](https://docs.unity3d.com/Manual/android-sdksetup.html)
* Download the [Google VR SDK for Unity](https://developers.google.com/vr/unity/download#google-vr-sdk-for-unity)
* You'll need [Daydream hardware](https://developers.google.com/vr/daydream/hardware) to run your app, including a Daydream-ready phone and Daydream headset (Note, I use a Nexus 6P running Nougat, although not offically Daydream ready, it does work with some complaining)
* A second Android phone with the [Controller Emulator](https://developers.google.com/vr/daydream/controller-emulator)

### Setup

Instructions for setting up your Unity project for Daydream with VRTK. Uses the integrated support for Google Daydream VR camera object.

* Open a new or existing project in Unity (5.4.2f2-GVR13 or other version with Daydream integration)
* Import asset package GoogleVRForUnity you downloaded from Google
* Build Settings: 
	* Target platform: Android
* Player settings:
	* Virtual Reality Supported > Daydream
	* API Level: Nougat
	* Bundle Identifier and other settings you'd use to run on Android
* In Hierarchy, create empty, named "DaydreamCameraRig"
	* Move or create a Camera as child of DaydreamCameraRig, reset its transform (position 0,0,0)
	* Add GvrControllerPointer prefab from Assets/GoogleVR/Prefabs/UI
	* Add GvrControllerMain prefab from Assets/GoogleVR/Prefabs/Controller/
	* Add GvrViewerMain prefab (enables view in editor play mode)
* Disable Daydream's native pointer tools
	* To the Camera object, disable or remove GvrPointerPhysicsRaycaster component, if present
	* To the GvrControllerPointer/Laser, disable or delete


### Setup VRTK Components
* In Hierarchy, create an Empty named "[VRTK]"
* Add component VRTK_SDKManager
* Add a child Empty named "RightController"
* Note, Daydream supports only one controller, LeftController will not be used. If present, can be disabled or deleted.
* SDK Selection
	* In Inspector, choose Quick Select SDK: Daydream
	* that should populate the four SDKs, 
	* In Player Settings, ensure Scripting Define Symbols: VRTK_SDK_DAYDREAM
* Linked Objects: 
	* Click "Auto Populate Linked Objects", that should set:
	* Actual Boundaries: DaydreamCameraRig
	* Actual Headset: DaydreamCameraRig/Camera
	* Actual Left Controller: empty
	* Actual Right Controller: DaydreamCameraRig/GvrControllerPointer/Controller
* Controler Aliases:
	* Model Alias Left Controller: empty
	* Model Alias Right Controller: DaydreamCameraRig/GvrControllerPoints/Controller
	* Script Alias Left Controller: empty
	* Script Alias Right Controller: [VRTK]/RightController


## Tips

* Note, we're using the name "RightController" to follow VRTK convention. However, it's a Daydream device setting whether the controller is rendered as held in the player's right or left hand. But normally players will have just one controller.

* I recommend enabling Use Accelerometer in GrvControllerMain for more natural hand tracking.

* You can change the Controller's GvrArmModelOffsets Joint to Elbow for more exaggerated (amplified) motions

* Optionally, in Hierarchy add the following prefabs,
	* GvrFPSCanvas (to see the frames per second rate)
	* DemoInputManager (to see the controller connection status)


### VRTK Controller Components
Depending on which components you're using on the [VRTK]/RightController, you might want to make the following adjustments.

* VRTK_ControllerEvents: suggested Action Alias Buttons:
	* Pointer Toggle Button: Touchpad_Touch
	* Pointer Set Button: Touchpad_Touch
	* Grab Toggle Button: Touchpad_Press
	* Use Toggle Button: Touchpad_Press
	* UI Click Button: Touchpad_Press
	* Menu Toggle Button: Button_One_Press

* VRTK_SimplePointer: 
	* PointOriginTransform: drag in the GvrControllerPointer/Laser object, which has the Point joint (instead of Controller which has the Wrist joint) or other object using Pointer joint 
	* (Note: Pointer Tilt Angle is set in GvrControllerMain's GvrArmModel component)

### VRTK Event Support

The following shows the events supported directly by the VRTK Daydream SDK. Other events, such as Trigger_Press, if needed should be set in the controller Action Alias Buttons.

**Supported**
* Touchpad_Touch
* Touchpad_Press
* Button_One_Press

**Ignored**
* Trigger_Hairline
* Trigger_Touch
* Trigger_Press
* Trigger_Click
* Grip_Hairline
* Grip_Touch
* Grip_Press
* Grip_Click
* Button_One_Touch
* Button_Two_Touch
* Button_One_Press
* Start_Menu_Press


### VRTK SDK Support

The following shows the API methods supported directly by the VRTK Daydream SDK. 

**Supported**

* GetControllerDefaultColliderPath
* GetControllerElementPath
* GetControllerIndex
* GetControllerByIndex
* GetControllerOrigin
* GenerateControllerPointerOrigin
* GetControllerRightHand
* GetControllerModel
* GetControllerRenderModel
* GetVelocityOnIndex
* GetAngularVelocityOnIndex
* GetTouchpadAxisOnIndex
* IsTouchpadPressedOnIndex
* IsTouchpadPressedDownOnIndex
* IsTouchpadPressedUpOnIndex
* IsTouchpadTouchedOnIndex
* IsTouchpadTouchedDownOnIndex
* IsTouchpadTouchedUpOnIndex
* IsButtonOnePressedOnIndex
* IsButtonOnePressedDownOnIndex
* IsButtonOnePressedUpOnIndex


**Ignored**

Uses Fallback methods, usually returning false or null.

* GetControllerLeftHand
* IsControllerLeftHand
* SetControllerRenderModelWheel
* HapticPulseOnIndex
* GetHapticModifiers
* GetTriggerAxisOnIndex
* GetGripAxisOnIndex
* GetTriggerHairlineDeltaOnIndex
* GetGripHairlineDeltaOnIndex
* IsTriggerPressedOnIndex
* IsTriggerPressedDownOnIndex
* IsTriggerPressedUpOnIndex
* IsTriggerTouchedOnIndex
* IsTriggerTouchedDownOnIndex
* IsTriggerTouchedUpOnIndex
* IsHairTriggerDownOnIndex
* IsHairTriggerUpOnIndex
* IsGripPressedOnIndex
* IsGripPressedDownOnIndex
* IsGripPressedUpOnIndex
* IsGripTouchedOnIndex
* IsGripTouchedDownOnIndex
* IsGripTouchedUpOnIndex
* IsHairGripDownOnIndex
* IsHairGripUpOnIndex
* IsButtonOneTouchedOnIndex
* IsButtonOneTouchedDownOnIndex
* IsButtonOneTouchedUpOnIndex
* IsButtonTwoPressedOnIndex
* IsButtonTwoPressedDownOnIndex
* IsButtonTwoPressedUpOnIndex
* IsButtonTwoTouchedOnIndex
* IsButtonTwoTouchedDownOnIndex
* IsButtonTwoTouchedUpOnIndex
* IsStartMenuPressedOnIndex
* IsStartMenuPressedDownOnIndex
* IsStartMenuPressedUpOnIndex
* IsStartMenuTouchedOnIndex
* IsStartMenuTouchedDownOnIndex
* IsStartMenuTouchedUpOnIndex

### Mobile Considerations

Please consider that VRTK was originally developed for desktop VR in mind. Google Daydream is for Android mobile devices. I do not know to what extent, if any, VRTK has been optimized with mobile VR in mind. I appreciate any feedback, issues, and suggestions to improve this SDK or make VRTK great for mobile.

