# Examples

**Important Note**

> The example scenes no longer contain the `VRTK_SDKManager` as this is loaded in at runtime via the `VRTK_SDKManager_Constructor` scene. As the `VRTK_SDKManager` script is responsible for setting up the required Unity Scripting Define Symbols for installed SDKs it is required that the `VRTK_SDKManager_Constructor` scene is opened in the Unity Editor when first using the project or when installing a new supported SDK so it can set up the scripting define symbols. The example scenes will not work until the scripting define symbols have been set up correctly so please ensure that the `VRTK_SDKManager_Constructor` scene is loaded into the Unity Editor first.

This directory contains Unity3d scenes that demonstrate the scripts and prefabs being used in the game world to create desired functionality.

> *VRTK offers a VR Simulator that works without any third party SDK, but VR device support requires a supported VR SDK to be imported into the Unity project.*

The example scenes support all the VRTK supported VR SDKs. To make use of VR devices (besides the included VR Simulator) import the needed third party VR SDK into the project.

For further information about setting up a specific SDK and using VRTK in your own project, check out the
[GETTING_STARTED.md](/Assets/VRTK/Documentation/GETTING_STARTED.md) document.