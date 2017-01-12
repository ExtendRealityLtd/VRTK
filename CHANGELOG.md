# Changelog

# 3.0.1

## Bug Fixes

 * **Pointer**
  * set pointer transform correctly (619535066c9ce981affec11788bad754ed9c7e28)
  * ensure ToggleBeam() turns off bezier pointer (44d55aa1c76117ddb459fd4d449cb633e66e797b)
 * **SDK**
  * support SteamVR Plugin version 1.2.0 (53dc577ee9e4e85ba555e82890422e4f1ced2111)
  * run controller init code using Awake (cbc397a8fbd0e1c23512c34d9f425edcf6a80bb3)

# 3.0.0

## Breaking Changes

  > The following commits introduce changes that break functionality if upgrading from version 2.2.1
  
 * **SDK**
  * add basic support for Oculus SDK and OVRCameraRig (c0fba69b64cb3c6d521a4240ab550b7afa16b1ab)
  * remove interface and impose contract with abstract class (2b511ac5facf15d7aa550f0c4bc0b680b46643e3)
  * add support for a wider range of controller options (b49ea9227a1908747947b57b50a39e0c340de3be)
  * require all sdks to adhere to specific interface (3f019ad3c82a566b3948c6cdd14a17ffefffee41)
  * split bridge into distinct areas of responsibility (2008568a9f786eaa0c5f5789f54bd9fc2393273b)
 * **PolicyList**
  * add support for layers and rename script (3ac78e4350c9fd5ad08d58aeb4d4332797b43718)
 * **Structure**
  * ensure vrtk scripts are decoupled from sdk rig setup (eca0f66ac5a9b86630d767b7fabf991a0c0f0ed1)
  * rename Utilities to VRTK_SharedMethods (b364b78b5aee976a2ade1af86a9eff27b74ef3f2)
  * rename Helper to Internal and move UnityEvents (62a343a2df2abbaaceabe3534618b762b33d35b7)
  * arrange scripts into sub directory sections (36c599cf3b90e9ce3e601e49fb6aa0a02047735a)
 * **Headset**
  * remove need for scripts on headset (7a5e58a64d7869af7045b1cca127bb35f11ae40e)
 * **TagOrClass**
  * remove tag_or_class string option (15cdf15d30654dbb79cf3ac6a391ee3fb0b7bf38)
 * **BodyPhysics**
  * add body physics script (68d6e501c95e6c5707db065a446e6ad72d9cde59)
 * **SharedMethods**
  * move isolated methods into more specific scripts (4733898a3d1b7daaba6b19ab12cd0576f00a1f44)
 * **Pointer**
  * rename world pointer to base pointer for consistency (15f5266d935a7560a3d19f318772f2c42b741e80)
  * separate play area cursor into own script (b487f4af576595b9ed87e16941f741ab105e6d33)
 * **Interaction**
  * combine secondary grab actions into one parameter (72924624f64774d8aca144b4bf93892303189642)
  * extract grab mechanics into their own scripts (f46d7b967222da50c9c9d1912f204c02eaff1dd4)
  * extract controller visibility to own script (aad88c428a2e82abc9dd1259ec4827d2af871d6c)
  * remove global touch highlight option (f755e23ea854546476324c895ae2c14eb4bd24e4)
  * extract haptics out of interactable object (36c0f9d30947e4d064337fe32f64c15013cd2d5a)
  * add ability to utilise secondary controller grab (7961fbe47f1287ef284a837ac11635445dbf947e)
  * replace isDroppable with valid drop options (0851315e0cecd3884e1b46bdc87c14d0aa12049c)
 * **UICanvas**
  * add ui canvas script for defining valid canvas targets (02fb99287c2375b15999239bcbeb965a59273274)
 * **HipTracking**
  * rename script to be inline with naming convention (615784666bc049fa7e2e8e9e09d81a2ae9e46731)
 * **MonoBehaviour**
  * rename Reset methods to not clash with Unity Reset (7cdae19c48912009414fa979e565014d9984757e)
  
  
*End of breaking changes*

## Bug Fixes

 * **Pointer**
  * prevent crash when using interact with objects parameter (132bb687dc3621c03e3eae80c27e617766e90b0f)
  * prevent bezier pointer clipping (1c6b2437b984a24d0f6fdc7fab241b519b656747)
  * unparent simple pointer holder to fix scaling issue (7e4cd637bc18949e8e7ced79c7aae8143c37bda9)
  * ensure controller is valid before removing event handlers (fdd760c1e6f89caeec5b3e47c0b3941212f41fed)
  * ensure controller set up check on base is done in start (df95931a60d780587bfea38086e92133abac958f)
  * ensure object using state is set when use with pointer (ec15c1887e3c5efb993c013abd52c45070d3f0d3)
  * reset saved beam length if not grabbing (356b09de0ef2414d2b142feae5a0e3831e30c1bc)
  * rescale the pointer cursor also if no collision (96912f6bd2e87bec3f335f0f1cd870c75b6447d4)
  * update custom cursor prefab (1a96d935fa787ef6e9f051d60eed1e6eec981b1a)
  * ensure custom pointer objects retain set material (0aafda0782ed8fdd4265a0a84996d4d53d95deb5)
  * prevent flicker on activate in different position (242ac6b70ef4e5b249a3e34f1b246b4fdc3f4a23)  
 * **Assets**
  * add white background to pig outline (89ad6ca734a02161ea3466ef643fed820bf4e2fb)
 * **VR Simulator**
  * fix readme and rig (3d8da8e340627472244c40d36869e3da451ff6ab)
 * **PlayerObject**
  * ensure player object check includes inactive objects (452c0376b54a53eafa946bcb22a95bd4b339333f)
 * **ControllerActions**
  * check cachedElements is set (ccd54a27a8d3d9cb5539a93b7ba79fb774f427a9)
  * remove redundant private variable (fff5ec7560d784bc8c5491a523773773c1042ecd)
 * **RoomExtender**
  * ensure gizmo adheres to coding standards (b6e7b7241ef58f63d77d1a8407916c124b2fb895)
 * **RadialMenu**
  * apply correct rotation offset to menu (f11bbc578edaf197df8c69a548cb46add85e70d0)
  * add option to choose whether to auto generate buttons (0c3678d812545afbc56a2221e4746a349cacfd36)
  * only change the axis if the touchpad is being touched (b155c6e81e8e84325a10782b699aa127504de315)
 * **SpringLever**
  * rename script to conform with naming convention (58e93a8463735f7d0ad00cb52b940d014340d79f)
 * **Examples**
  * fix a warning on Unity 5.5 and above (5b6f950036707dec3af39c4583f3af2fcd864cb6)
 * **Caching**
  * force fetch controller manager if current is disabled (ecc6bf217542e83c65f8b7c7b9fb1729bf6e7f59)
  * get specific ControllerManager for left/right controller (d817dc4ef09bd114ac707de78b54cd6d7a3edc64)
  * reset controller cache when tracked device changes (c8eac61416fb7510d966f0a87467a803e3a928e4)
 * **Headset**
  * detect collision end on collider disable and destroy (49bc483cb1b75efe436cc60d54c1c6f4c694cc1a)
 * **Presence**
  * adjust physics fall to correct for local rotation (69c7775d169b9df40752c58811c548bdad6bab93)
  * prevent collisions when swapping grab hand (7034181e4823145dde08d444eac948bf9a4e896c)
  * prevent crash when ignoring inactive colliders (2b9460038c56db1184f02b4247387ab72db9ec01)
 * **InteractGrab**
  * scaled player throw velocity matches world size (3db485515e1f67332007d150c910e16d08672bc2)
 * **Animation**
  * remove legacy animations from animation controllers (be6b0d0466ce5c1c85532597a44f43cb8d13c98a)
 * **UICanvas**
  * calculate collider depth based on scale and pivot (05dfdb352b96b68c61d3dabe36d18a670bc5b5da)
  * ensure pre-existing collider/rigidbody is not deleted (49ccf2746c43318cb5cfcb9d80aa018fbb63d3f6)
 * **UnityEvents**
  * unregister listeners in OnDisable - resolves #642 (e79f4fca3ce3ebae7e9f732ea4c7e511a93d2a8d)
 * **Tracking**
  * replace double quotes with single quotes in tooltip (2dc8d715c5a3878ddd5420d431b3e76491b08f50)
 * **Teleport**
  * register destination markers on child components (fd9a285d904354dc16080e64e8435f2a93a75003)
  * use the nav mesh check distance value in SamplePosition (2e4616ad2ea15192a5c00673c0109e1c22bfa938)
 * **Highlighter**
  * ensure faderRoutines is set before acting with it (910870e1cc2067e09935c6950b5a10c23da5390f)
  * ensure outline highlighter uses correct transform (a383e3820f00d01e368f1cd4ab5278f60643c1b3)
  * clean up pre-existing outline clone objects (5efa25424e51b3e749916d4e89237f6c6c7cac12)
 * **ConsoleViewerCanvas**
  * add VRTK_UICanvas to make prefab work (386025f966c4d12528cd1304cf68fab8c930210a)
 * **AdaptiveQuality**
  * extract SteamVR call in editor to SDK Bridge (5d227f82b6f992be35065920c5c4f80c7d1c7bb4)
  * remove sdk error when using debug visualisation (ea76402213939c81fcb2ed38c280cb65d263679f)
  * prevent flickering on Unity 5.4.3 (1274078a648920b46c1f2e8fa2cd86417497282b)
  * remove check for VR camera (7b796c72be0bd3234561a98a5133cfa16aae97ae)
  * show the correct render scale level in the debug visualization when inactive (1c41cbb60bdacbdae8d6e00ede0b02efd867bb39)
  * prevent flickering on Unity 5.4.2 (109b4f154fe67e3eb9e5dc5c56a4fb85aa837cf2)
  * ensure adaptive quality works on CameraRig (8d974b4a8baff349a5cb9ebc55c030246ccbb476)
  * only allow for Unity 5.4 and above (b483c1fd68bcf0322a007e46ad1135b628acf896)
 * **FPSViewer**
  * set canvas plane distance on start (9f11d0ac1cf6f9941998ba7039293be663ead0db)
 * **SnapDropZone**
  * ensure object position is right on moving transition (ec3e21b02dae790cd1b584b5701f6665c4abcdc3)
  * check parent objects for interactable object as well (4cb85afd91f74ed32f39e3141844f0005665e269)
  * use highlight container for position (f1486af0001380cfd60e2c0c8fb3e5f9696271f2)
 * **Editor**
  * allow multiple object editing on interactable object (baadc90c943d5e5c49d9cddd776af636ae6ff890)
 * **Shader**
  * apply correct render order to outline shader (336cf787a75ebea05f4e0193e583a9a97e7ed2ca)
 * **Simulator**
  * inconsistent vector addition Fixed inconsistency in vector addition caused by adding a vector in world space (the camera forward)to a vector in local space (the camera rig's basis vectors).  Doing the math in the same space gives the intended result. (5d49dfd83dc134f678e147a5b7cded7c0bc208ef)
 * **SDK**
  * prevent SteamVR SDK reporting a button two press (748902df7d6c8adf3cc19e687b3a956cc5d76428)
  * prevent error on end scene with simulator SDK (c76fa012151a597f3a6a899704e456fa93e7d86a)
  * rename vr simulator prefab to more meaningful name (e22380425fdfb86429111b2779690a099fc7e663)
  * add debug log error message if fallback sdk is being used (c4ff4248b8e4f37955b6b0f2f8ecec42c5a00fd1)
  * catch errors when sdk scripting define symbol is not set (8fe0a208ae9e87ca8fbc0f47e432bcada0e5f34f)
  * add debug error if simulator sdk used but prefab not found (f89c450c00dad16b5aed1a84ccda8c3842420d7e)
  * prevent crash when requesting non-existant vive render model (c379f2b9c9fc4e098f563a4bac21f969ba7d3584)
 * **Interaction**
  * ensure collision checks are done in physics loop (2c22050ef363a2f3060849c3508a472c30ee223c)
  * improve z locked rotations for 2 hand direction (130cb351ccfd95c17624a9b3072cb5196c8e3ccf)
  * use local rotation when inverting tracked snap handle (b2112202dd2db3f78cbffe859b03abd357071cad)
  * use magnitude instead of distance for uniform scale (5ca58f34f1fab2ee36e69215a5270ea13aa5fa71)
  * prevent controller becoming visible after use (7c2b04dd8ce38f8b63e3c2f9738d255169b0e624)
  * ensure using object is reset on interact use on stop (ab26f989a6506be990346313d5f890cdef202f9e)
  * calculate throw velocity from centre of collision (6dbc7124b1c6728af3205005398b69af0010f978)
  * ensure haptics enabled on correct interaction type (ea9df0e237a8ca0ebcf94a112620dc208242fd84)
  * ensure object is dropped when primary grab released (c1ef337b6584f8110845c8e1dc162e5435dda3b7)
  * remove redundant GetComponent/GetComponentInParent (5ddd1ef7df4b192393e5bb1f380177f0cc10d45f)
  * reactivate collisions when an object is ungrabbed (276b65347d3eeb1868a5bfb00979eb73497f6348)
  * ensure track object snap handle uses same orientation (6511eeef705b56580a6e7075d23e24bb54ad3a76)
  * ensure using state is set correctly after touch/grab (3dd56cb3537bf802512e36b8d7d4141fe38ff757)
  * prevent button overrides being reset incorrectly (52b89dc5b7ae11e24b632684c3e6d21b5ede79fb)
  * prevent error on stop using if controller is null (21219d2e5fe1331f115925c98253c601da73884a)
  * use relative instead of world force when snapping (49600e15bc32f17cb73834bbaf11773ed1c6de42)
  * ensure highlighter exists before highlighting (4217b75fc5cff8d7bd9a2ee40523f75c81d3f97c)
  * objectHighlighter not initialized in ToggleHighlight (fc276f35cb9cb207cbfc6c8d39d794c3634b5adb)
  * scan also inactive objects in color highlighter (500321c22e3fe6b697a8f3b0dd76857ee8c30369)
  * ensure multi-collider objects register touch properly (8651dd9af4c61b7140b9385e6349bdd25f6a685b)
  * ensure trigger collision check is done every frame (4f3b68c4ebb60808252306ede74475e1d938e335)
 * **Structure**
  * remove erroneous semi colons (dbf0734d35827ad89751d30de77648ca7cbcf167)
 * **Controller**
  * ensure controller colliders scale (0dfaac8a0ae3bd0be5c7c5886cd7f81ad12de624)
 * **UIPointer**
  * UI not responding after application loses focus (1a3328f78b8f86763bcefad9ae6269cf1dd70f31)
  * ensure event input pointers list is managed correctly (23b4dbdc9f8410f720ebcdf66945789a42f5eab4)
  * remove UI Pointer artifacts from invalid canvas (92cafdc815fdbcecf73773c3fa790fe6bb310284)
 * **CONTRIBUTING**
  * change 72 lines to 72 characters (8fc526bb79357c96e6e760636de87460cc30e800)
 * **MonoBehaviour**
  * rename Reset methods to not clash with Unity Reset (7cdae19c48912009414fa979e565014d9984757e)
 * **HipTracking**
  * format script to adhere to coding conventions (f44c89fb3cb8cca060b788467a61501fb2d77be3)
  * rename script to be inline with naming convention (615784666bc049fa7e2e8e9e09d81a2ae9e46731)

## Features

 * **AdaptiveQuality**
  * add a refresh button to the inspector (bd25e5154b18b663222e98f5ee3f5adaad01d1de)
  * adjust render target resolution (65d2cc38a80efc9fd012e33f6a12186b9aac49b0)
 * **Assets**
  * add svg version of logos and update main logo png (8b3678a16b239bb4dc358c8b81c843a13f42fa29)
 * **BodyPhysics**
  * add body physics script (68d6e501c95e6c5707db065a446e6ad72d9cde59)
 * **Climbing**
  * climb release can height adjust or physics fall (714d37dcb710cdc269a28f6839219eef068abfd7)
 * **Controller**
  * add prefab to auto enable rigidbody on controller (3b89119e73eaae8eb871900490db4443b2d041be)
  * add headset controller awareness (b9a94413eb8e7c3e006f49dd3afbdc7737180ca3)
 * **Controls**
  * add new Panel Menu 2D control (ee2031507bfa196fa2f0d72a1f5890caaaaf3c7c)
 * **Headset**
  * remove need for scripts on headset (7a5e58a64d7869af7045b1cca127bb35f11ae40e)
 * **Highlighter**
  * specify whether highlighter uses cloned object (cb9a29d838804e35e8edc16a609345e9af93e816)
  * add custom material option to color swap highlighter (7d69cc0ecdb8bb7df4d3390ae323327daa3a970c)
  * add new generic Reset method for highlighters (191e798e8a4a8c266ebd248665859dfda66eed4b)
 * **Interaction**
  * add z axis lock for direction control grab action (eb89a13f5460c5a71cf928b5f18f3dd0d2e59ee2)
  * allow interactable objects to be disabled (d2002e0eed66e5e12a5217d19bc61edb1cb4696d)
  * expose interactable object highlighter reset (cb9031e0f413c40a5614089336379335b23c1d46)
  * add option to apply point distance on throw (f98c098c48f322f764d960a70a8081b76465e4a4)
  * combine secondary grab actions into one parameter (72924624f64774d8aca144b4bf93892303189642)
  * extract grab mechanics into their own scripts (f46d7b967222da50c9c9d1912f204c02eaff1dd4)
  * extract controller visibility to own script (aad88c428a2e82abc9dd1259ec4827d2af871d6c)
  * remove global touch highlight option (f755e23ea854546476324c895ae2c14eb4bd24e4)
  * extract haptics out of interactable object (36c0f9d30947e4d064337fe32f64c15013cd2d5a)
  * uniform scaling for secondary grab action (cdb6946424e41ddbbb1d4f07d45b6fda622a6516)
  * add grab to pointer tip (a81a720a1065e89e3602c899e8c7aa2c3c911a27)
  * add secondary grab action to control direction (9c0a46540148ca76c91fcdf4cb5ce183da32348d)
  * add custom secondary grab action to scale objects (aeebca88ece89fce338b960c2f93b1d7e40e0823)
  * add ability to utilise secondary controller grab (7961fbe47f1287ef284a837ac11635445dbf947e)
  * alter velocity of thrown object based on distance (695c46358762cc62e5ba62b5dea737fd2b85c754)
  * replace isDroppable with valid drop options (0851315e0cecd3884e1b46bdc87c14d0aa12049c)
  * support moving knobs (d06d8b03ec068ecc2f2564b9c01da9e11ba8c80a)
  * add multi interactable object gun example (c1b65bb95651d8444bdfe3093a5932a66b4cea2b)
  * add fire extinguisher interactable object example (5fef690da9de49fd55d138f22e5d317055ce5375)
  * add option to interact without grab (872008dc2661c1b030492c245abac399bc8828ff)
  * support moving drawers (496106173b6d655fda6152b412261d63f080ca1c)
  * lever attach to game object (2783df17a6bbc251fa2154ce18ab3555617e549b)
 * **Locomotion**
  * add locomotion based on arm / head movement (3ffcb20c8d20007e70a6750fee323f475dbd95bb)
 * **MoveInPlace**
  * add controller rotation control method (3c30dadbee03ddc99316cdfc050ed73c790ae4d0)
 * **Pointer**
  * allow custom transform origin on pointers (460c3be5ddd941c1f182d449878a8b1995d97151)
  * check headset out of bounds on play area cursor (e860d834407f4fe36510de88876f105870b15b21)
  * options to reorient and rescale the pointer cursor (fee13fec8ec912398a29bdb5d93ae3a525897029)
  * allow pointers to interact with interactable objects (ae31ddd0e40a0284da88edcfd26d8e2485118c54)
  * separate play area cursor into own script (b487f4af576595b9ed87e16941f741ab105e6d33)
  * pass RaycastHit struct on destination marker events (c694cd6eaf67205520943d62b97d45e3a8ba9288)
 * **PolicyList**
  * add support for layers and rename script (3ac78e4350c9fd5ad08d58aeb4d4332797b43718)
 * **PositionRewind**
  * add ability to reset user position on collision (7a3ae1ab85fd09fc80000a32bc8d98028fdc5d6f)
 * **SDK**
  * add simulator (e52ca5fc97489e120b30201de04cda9363768430)
  * add basic support for Oculus Avatar package (c6fd89ccb0d6ad9aa5f10f67ecffd2b4ad94f507)
  * add basic support for Oculus SDK and OVRCameraRig (c0fba69b64cb3c6d521a4240ab550b7afa16b1ab)
  * add button two on controller (4d53a6e9c3cdfaaa6ffe0c5b39090e17d470531e)
  * add method to auto generate controller pointer origins (c923f1403a598f4ab36e277069d9af997b9bc224)
  * require all sdks to adhere to specific interface (3f019ad3c82a566b3948c6cdd14a17ffefffee41)
  * split bridge into distinct areas of responsibility (2008568a9f786eaa0c5f5789f54bd9fc2393273b)
  * allow controller element paths to be hand specific (0514640b4fc2dbb1356c57181a82dcbbb77a9107)
 * **SDKManager**
  * option to disable auto script define management (ef95e9f161523ece7721cb2869467ab858a9c78c)
  * require scene save on auto populate linked objects (0df00659f1a66576a0120cf9204752d4f2a3b77a)
 * **SnapDropZone**
  * add prefab to allow for set drop zones for objects (b7eb7e585ff85c537d2b839834e281dd04d631c7)
 * **Structure**
  * support Unity 5.5 (7580e39f448d6ae0f3100e3d68e0cbee95988eb4)
  * ensure vrtk scripts are decoupled from sdk rig setup (eca0f66ac5a9b86630d767b7fabf991a0c0f0ed1)
  * rename Utilities to VRTK_SharedMethods (b364b78b5aee976a2ade1af86a9eff27b74ef3f2)
  * arrange scripts into sub directory sections (36c599cf3b90e9ce3e601e49fb6aa0a02047735a)
 * **Teleport**
  * disable teleport when controller obscured from view (0efefd3f56bf9056e72f8dd53d1896e043cf19ef)
 * **Tooltips**
  * add method to update tooltip text at runtime (5fb8d15659e6c09e8f0d8d9d9f0b70118413fa1b)
 * **Tracking**
  * add head-rotation independent hips (c549d25fc9572d880c181504b006124b4bc31e28)
 * **UICanvas**
  * add ability to drag and drop UI elements on canvas (ff57860188bc299147254ea770cb08f1e3732aba)
  * add ui canvas script for defining valid canvas targets (02fb99287c2375b15999239bcbeb965a59273274)
 * **UIPointer**
  * add events for click, drag start and drag end (45a39c38f0684b887c80029deb300c63434e3954)
  * automatically activate UI ray within given distance (6856b8f8ccde753d9923de03708c81e84ed440bd)
  * add click ui elements with pointer collision (6d625e928e1ebeef59ea1532559f75c306b0b489)
  * add ability to choose click on button down or up (300e237d5c05d57f71e85a78f84aa43e5ee36c48)
 * **Utilities**
  * allow tag or script check to use include string (cb0f1e4fb39cec03dbeefc9d49a48b8659a5a97d)
 * **Layers**
  * expose layers to ignore for all raycasts (39cf2fede029445a983acd85a6b40bb5e2c7c552)
 * **VRTK Window Menu**
  * add setup helper (2ce450f4526595fc8b0c7bbbd7ee3d0b334017d4)
 * **TagOrClass**
  * remove tag_or_class string option (15cdf15d30654dbb79cf3ac6a391ee3fb0b7bf38)

## Performance Improvements
 * **ControllerEvents**
  * don't create garbage on vector comparison (a491404cf6ba1e58fbd9c5a28e67a4907badcbf6)
 * **AdaptiveQuality**
  * increase render scale faster if possible (773e5f23c5dd5543b4f18676a838c3b5e4eb5293)
  * change render scale less often (c7d22bb6c9b01ec21123d046a2e580d0198d855b)
  * increase render scale later (aae9c903e0267c84aaf850f4e8fc026367b0e4be)

# 2.2.1

## Bug Fixes

 * **Editor**
  * prevent crash on build due to UnityEditor (ff74eaeb11761ff61a91b7dd2f33a631b741fdae)

# 2.2.0

## Bug Fixes

 * **Colliders**
  * ensure colliders scale properly (25c192a9bbf0d9f18818593ab5f9ffe504e23ea0)
 * **Components**
  * simplify getting components and child components (338508a87f238ca13176fff6c06eac1283685e73)
 * **ConsoleViewer**
  * include in VRTK namespace (6f45bf2a6a51275eeaa04ac6c5dc3fbf3992698d)
  * remove unnecessary struct (278c4c9ab134a44d6489941d1e1973afed527b7b)
 * **Controller**
  * ensure controller colliders scale (ec40a450085fdca1b976a40cf190518d169ffcb5)
  * ensure controller elements are cached initially (ed83890b70201ba49c96fccdf9106c08ea26ef4c)
  * ensure cached tracked object index does not exist (cd02b4bc6c40adc652c4fcb0e0bf160aa3f2acd9)
  * prevent error on cache controller tracked object (5cdd30789e5e84e9212ef122e22aea4bb27018f7)
  * ensure touchpadTouched is correctly reset to false (00d5e84d3a276a641c6c1e499a9e9baa33258783)
  * calculate velocity correctly on play area rotation (b3b9f75b52b22671a5081f9989997293ff0a557a)
  * button only disables when controller not highlighted (c31f78e6bb7b2f94137536dc6cd33232a6439a50)
  * prevent crash if model has no material colour (88dcf8e8cc84feabe0e9b4858463fe155ea5f55c)
 * **Controls**
  * replace lever to use rotator track (d7591066cccf518afc1337ae4a11cbcb971de324)
 * **DOCUMENTATION**
  * add missing controls content handler docs (63ed7b63712adcae57ba8159442ac86ced3d8fb9)
  * ensure nested lists in tooltips generate correct markdown (bd9090168a095ce1521a8b35057efa45a13e80de)
  * ensure multiline tooltips are included (a39d935e59eb545d9d316000eabacd8e5b6dd4b2)
 * **Editor**
  * prevent interactable object rumble values from switching (0ee0baa168571a228bd49b0c10c1f93fd39e1c80)
  * ensure custom properties are changeable and savable (ebf7b0e0db680a2e027b7916f3551dc9b5dc1d1f)
 * **Interaction**
  * replace force drop enumerator for list index access (f27da1c927d086087baa3580ad756f49bcc31b61)
  * stop crash with interactable object switching scene (f9c5ca2e9321fec8ce68296de4a530a391459cdb)
  * track every touching object instead of last one (16293dddbc7b39400f2f24259780ef795a47353e)
  * remove limiting max angular velocity on throw (7339f353d585a45cb5c7df3e73cd4a4fb9735838)
  * ensure undroppable objects cannot be dropped (0eeb7a3bbe1f39b2890dd9aeb3d7a3ec3aa2f438)
  * ensure unity event helper doesn't crash (79ff3dcbab83562cacee2f2a24ac223501b28fd3)
  * use game object instance id as ref for highlighting (5c38bbf32611bf636fb65c91564bb6e654a944e5)
  * detachment check from grab point (45f8fd59c0e4b36138417ca0f7fd6eccdff2bbed)
  * improve simulator (e405a52357c92f999ea09601436688e7f6766cd6)
  * only instance materials if highlight is used (d1c53e8e6d65982b796b36146f91f3f48ae552a3)
  * improve door gizmos (549c8cf59d77aa216a980558e199adccc72e1e88)
  * make tracked objects throw the same as other objects (75d20e536336dec1fbfbaf24e4063011dc9a167f)
  * ensure touch highlight works with multiple renders (276b4d164fadb9d164c759c8429b815da31014b4)
  * ensure autograb always happens on enable (823c4d9bbaf85f0a1779b0403aadaf296201d7e2)
  * ensure pointer use activation honours valid hand (37a6c9b2ef07640cc67fde6bd1581efbf3efb134)
  * ensure object is released on disable (c553ff3a9cf74cbfebdaf3b7c69cf599cddf1525)
  * make force stop interactions wait for end of frame (6a94c07a48d4fe42ad8f3583edc4fbc2cdb11965)
  * remove unused code in 3D button (af60a54f9d461170c441785db571fc73ee94cd6b)
 * **Pointer**
  * prevent bezier pointer joint clipping on down cast (9c248dd9faa7aa72a48518f796233a769d940f92)
  * set destination correctly for constant beam (6d1cd8feb06a5f124b6f2ae07a8a8023a943d0a7)
  * switch material back to unlit/transparent (861660905204922b4baf109cd9468adde4e338d7)
  * enable ZWrite on pointer shader to fix UI issues (56e8c1c48c1f2a9e180ee1582218b75d8246ed5a)
 * **Presence**
  * ensure player collider height is set correctly (087b5679bafb9affd839f6247db9c19b328d86c4)
  * ensure controller colliders do not affect user collider (d02adb472caab0ff45fa709a0bc5381a391cc621)
 * **Teleport**
  * ensure position change over tiny distance change (de6358c5d8b6e6065fca7c61894669505da2a465)
  * add required components to dash teleport example scene (41a03554ef15903ae32790092fd3c2d36518cd47)
  * add floor height tolerance parameter (5e47d291f42b634691b0547be01236cb17ce1666)
  * prevent constant teleporting to nearest floor (0fd5d9e7201d62c45188893df58f5c9f332d18af)
 * **Tooltips**
  * ensure tips are hidden when controller is hidden (94ea57780bb3b1810b65a882b3e3e4e3f9c02f22)
 * **UIPointer**
  * prevent central button flickering on hover (be26979423ef1eb35d426549331b238fab2d934c)
  * ensure blocking objects prevent ui ray (284a5adc03a49e2544bb56475514bac79a45bcd4)

## Features

 * **AdaptiveQuality**
  * add Adaptive Quality (0a607b7029ec57be53d0d60795459b591dcd6a67)
 * **Caching**
  * add object cache to reduce expensive find calls (d2e1d558a3497934f8dbdbb868bd03587614cc7c)
 * **Controller**
  * add event for action of toggling visibility (574f2a4f43146c6b70c18d0df604413183425c78)
  * add event for controller enable and disable state (36e2ae7fc62af490244f9af59307efb6536e7d73)
  * add trigger click threshold parameter (a5d31da0d11128d180b04fbd6861d8772ba88041)
 * **Controls**
  * add spring lever (7d9180721812fdade075a560c35510ba73b71913)
 * **Editor**
  * add interactable object custom inspector (9d61fad65a85f133a9eb51b4f6f418952cb260af)
 * **Events**
  * add unity event for dash teleport (d2ecf7c9a18bda86ebc76bc30a55012993b37f66)
  * add unity events helper scripts for delegate events (b5f9b10e219d617296af53f1403a06e8e51bb042)
 * **Headset**
  * decouple collision and fade into separate scripts (07760291531045198e3f4cd86cc5b3f32aa2b663)
 * **Highlighters**
  * add outline copy highlighter (8dee20322ebefda037205d2d58646e43758d24aa)
  * add highlighter composition scripts (5b850bba845622f7092013588540b5e940f2cae3)
 * **Interaction**
  * add variable to know when rigidbody is auto created (0c4d5d5523ada7ec13a719fd7e527ce2ebe5879a)
  * support chests without handle (3ae8fde5ddab5c8f4b05df802c05a1a18b0b7c90)
  * add helper for interaction unity events (7a9ab9bd75b39e0b9a23d9f06498f519926f79d2)
  * add stay grabbed on teleport option (a6aeb523fe1e5161c673677e7ce4e508eefbcadd)
  * add custom grab/use override buttons (cbda41565bf65e9bb221594de6b4ef403a81de6f)
 * **Locomotion**
  * add ability for touchpad walking to follow controller (8e27f8f5ce6680fac06f4c81945618a72c5cb701)
  * add option to only touchpad walk on button press (6fa5d5bfb89d69cc7ec01aea532b78871d3d8c58)
  * decouple touchpad walking from player presence (5a18a66c48871cd8b0d3aad826c754d014f8e642)
 * **Pointer**
  * rotate bezier pointer cursor to hit surface (d0551fa80d35ff42629278e7ebbfbb0848237ffa)
  * add bezier beam height limit based on controller angle (a1759a9000ceb16232e472fcb71297991bc88291)
  * update default pointer material (38948815241950cc37f078dab1d6b7d89d0ebda9)
 * **PolicyList**
  * add ability to have multiple tag/script ignore checks (5f99aed4df38d8700de40c6d215bc59a38fe6045)
 * **Presence**
  * replace collider with capsule collider (b436c75e4690112e273c7cd513c8ac17722aa88f)
 * **Structure**
  * rename to VRTK brand (dbe23f28d4d57f223d1a5e682382d10913507d37)
  * decouple SteamVR dependency from toolkit (2e72bd498a3cc4fd42d60f58708b159efc4e06a5)
 * **Teleport**
  * add play space falling restrictions on height adjust (01fa673d365a43e52f665c4e3d1838965261a814)
  * extract teleport disable on headset collision (7f27b6c52b021c70218d17ba0a2cf07b9e371ece)
  * add dash teleport (e05371ae5747b9fe2167d1986cb3b5fcc8927d85)
 * **UIPointer**
  * add option to do click when pointer is deactivated (6cce633f845606c45faf66f41fa80bdd9b38dca6)
 * **UnityEvents**
  * add sender object to unity event payload (beb35bbf3ee7ce06e6089ff2350449d1f991fdc1)

## Performance Improvements

 * **Controller**
  * cache tracked objects for controllers (adc4b5baf16af837d30658a3bba8078f39ed39c0)

## 2.1.0

### Bug Fixes

 * **Controller**
  * ensure rigidbody on not touching works on auto drop (69bf349da59d9c584dd48e098aac158c247bd0d6)
  * enhance AutoGrab script (ba26ec513ddcbd054e924517f73089e1d16395ba)
  * ensure events are not triggered on destroy (cd04fe610b57c393d6c1241adcceb9bba9924beb)
  * ensure controller device index is valid (c9eeec953635a58db72f7c85f7f7efa429f60afd)
  * check device index is valid in OnDisable (7e5fd6e951cebd46ff21b55c70c49adbaa8e53b3)
  * ensure touchpad press status is set correctly (9bf7506fa65df13685abd91617c37b0d95a25a33)
 * **ControllerEvents**
  * ensure all events are released when disabled (a86447b90ac1e62bb20134b2438bca4536556cc1)
 * **DOCUMENTATION**
  * add copy for the IsRigidBodyActive method (7a3ea70c80fc112b4d68dc0ec44906e8e228a0ea)
  * amend spelling error (726a1a86c7dfa1aebd0216bdeb8e1a78b1465da9)
  * label climbing example correctly (0a8876b64056a07566f85bf29caa8660e8d5af11)
  * fix incorrect example description (6214ce35551c377c7a5b5e3745dcdfb71fb99d46)
 * **Headset**
  * set method accessibility to correct default (e5d9ef4fc81726496dae66ba01c85a9ca791b164)
 * **Interaction**
  * prevent controller rigidbody from being destroyed (ffe142fc086c585e4d4b643866867f5fdecbf959)
  * prevent custom rigidbody gameobject instantiation (81c1de47bef980aa061f72e6ef8010ba3011c59d)
  * correct wrong door references (7e568a7363d83d68334610b9131c1088eaf3881c)
  * make controller rigidbody unmovable (c50ee15ae1ccf2fe05031d21473b8a811fac6718)
  * ensure controller collider is created correctly (6c6c232f760785e97955319da2989af1eaf19b3c)
  * remove unused triggerOnStaticObjects variable (0618f37505a7683bb4cc137e1462618e30ef6d10)
  * allow pause collisions on tracked objects (b5e24a23b3aed20bb50281d707f2f193cfcc2c5f)
  * ensure controller rigid body is created correctly (915024d23fccccad608a3df96a4ca98e6abc0cf7)
  * ensure grab state is reset for tracked objects (8a376ee2b3eb78726fccea41b12c3d802c37c1df)
  * wait for attach point before auto grabbing (fec9717c1d8ea2db7617629f0c74cebc82dc1ede)
  * allow touch highlight to affect all object materials (d79239f4baec9428ffbf3b0c728be3242ab2cf81)
  * make force of drawer more reasonable (702a8dd60fc86ce0b02c156a94198d178b2a6964)
  * ensure precognition timer is reset on grab (b43b66b44e7696fbb7fb0c1c0ba34b24d38a5263)
  * prevent controller swap breaking use functionality (0cc5e0632ce57cfd96ba4c64b052934cbeade3c2)
  * ensure custom rigidbody is not amended for touch (e60ab2e398119a920052451cfdebc130d2fd2e37)
  * ensure dropping objects on disable works (29a974134ae642e3f1d3b5270ee261c0111bc955)
  * react to enablement state of interactable_object (3715d98c56e2a1552b64e222a9631574b397e0bd)
  * prevent infinite touch haptic loop (57f2eedb52d9ebeef78517f923379a8baab7d6a2)
  * ensure touched objects check ancestors in triggers (f0e45700767d3a44fe948f060d1a87dc987f95eb)
  * ensure ui hover events are correctly managed (4bc32c71546bfe4a13acf6da5e021f953fe30255)
  * update example scenes back to use child of controller (af23691a853af65912c91dd4d4034d2beb81a64e)
  * add dependencies for interact use (d6b7509a47ca7ee4f4fbea01a153ebe4c174ec62)
  * ensure tracked object stays grabbed on teleport (10b8588c70afc3ffdcb2bc642dedc980dd859ff5)
 * **Pointer**
  * remove unrequired eventsRegistered variable (1c8bc23cd35ee465e2b3eba649b836f74aa29db9)
  * ensure pointer is cleaned up correctly on disable (64d320799635af15c98ecf1c2e05b3e204e04e7a)
  * ensure activate delay only happens on destination set (9d01206701cb52a7c7e474950baaa07acb5a54ce)
  * ensure play area cursor exists before updating (74f8a09c78330a0ed365f1b5ed12f3c7166c9b9e)
  * ensure bezier pointer down beam is infinite (fcc6eb2a33fb0059e0fc38031f053f354db71814)
 * **Presence**
  * fix crash on object grab if no collider is present (bcf03d6c4d85407d47adfdeb45bebfd3ddd66e90)
 * **RadialMenu**
  * ensure events are de-registered on disable (cbb9e6363b068fba8b50314d355aa2a0127594fd)
  * ensure button press is registered correctly (f9cc5f6152785cc20722187d0320132b99b0b790)
 * **Structure**
  * remove incorrectly commited code (49b5c3dd9c79dee50f5dd9d2ac1104e03a9278ec)
 * **Teleport**
  * ensure nav mesh check uses destination position (f5f47dc99bc7c2af562b18dfe8caebd39e19f9db)
  * reapply height offset on teleport (0b01d8c3becafb143a640a660e92e715303fb3d5)
  * ensure teleporting sets y position correctly (a180e6a3310501395af482ec37d17b95ca40dd31)
 * **Tooltip**
  * allow for line break character (18e4be0253bf1304338feb817567a8127081cca5)
 * **UIPointer**
  * replace CompareTag with tag equals check (5adee239b71cd5662c1d6897cbcf68b4106900cb)
  * get pointer id from specified controller (db20e794252d20bda48085b05565edefa5f654a8)
 * **errors**
  * ensure methods are exited after caught errors (411784367e4b907558d556f4de39797e2e8829a2)
 * **example**
  * prevent menu cube from detaching on activation (036b32388da6665ce2e0078c1d760f696d6206ce)
 * **timing**
  * update timers to use Time.time for accuracy (123b73c097002d72ddd519b57f8826c4e2357bdc)
 * **utility**
  * remove unrequired variable (253f0602ecc0e2c3f1d6cddea4d6e2b05d24f5e6)

### Features

 * **Climbing**
  * add new headset safe zone offset (f65f03fff1dfb68e4b31048a89f8473195749240)
  * add ability to use controllers to climb objects (063e35b1e6773370e076bc1bd93e32284e882cb7)
 * **Controller**
  * add undefined button alias (e463661924432d651ed2d9128804e735342f0517)
  * add more trigger states (28cd899af61fd92b6fbaa02f023eb075df88a84c)
  * add method for getting trigger axis value (fab96aa15cb49f55f3f9b088947262220d695ba5)
  * add example scene showing controller appearance (e0b2c46784ef7bd749d210f0463d5c1dbff8b52f)
  * add ability to highlight controller parts (1c7c93b8349b7b1d12510792d4cce8f43d1eb988)
  * add ability to set controller model opacity (1dce8839fe716e919ba83f0349033dcaf07c2c95)
 * **DeviceFinder**
  * add ability to get hand of controller (616814375d229f3e9f245d78cef9eb61bab91f79)
 * **Interaction**
  * add custom controller colliders for touch (113746f036a12ac2a78b10df58d1c88c032a7886)
  * service pack for 3D controls (ae4f02f10339c2d6ef1072917bababa671f5f1c5)
  * override controller hide + use only if grabbed (bdfd1b695fc1229c0867b170747ed72c1c1848b3)
  * service pack for 3D controls (15bf1d2b3314fb7070a69094b4f616383ceded03)
  * add door 3D control (a8f25147ab35e1af8dfa6abf5f69d4702b01da23)
  * make 3D controls expose UnityEvents (0696a7acc9bb61a495b2ad15bc49c2b57a08e91f)
  * enhance button control (fa007f9787dd8ea605313069e3ad3d0a9fe4f016)
  * enhanced drawer control (d014ac61e18bdb64b67faec64605f81fd5e4b794)
  * add ui click alias on controller events (7212b12a151926a342aa2006e3e19b13d55c7c0f)
  * add ui keyboard to example scene (64d362f3c884ed1369ce8cdd6c4cdb02b93636ed)
  * add rotator track grab attach mechanic (745275eaf70f19780325c20457b66b1d0fe160fd)
  * add drawer control (62afdb1a988f974b076db412a8b8d72cd9037e5e)
 * **Pointer**
  * allow for pointer toggle with button presses (72ec2f70d055d5ce32d535da1f83cb0cdfe1bc39)
  * Customized compound teleport (c4f5eced0c6d03645c17d12e64f5aaafef888106)
  * add ability to manually toggle pointer beam (ef2106f7035e87cfa5f8ca777b7949b5bd0568eb)
  * add seperate button for destination set (c1c874dd26e4d8861c458d12137c18b4ad44ecd2)
  * allow exclusions for play area cursor collision (924c77cf55a2929a08566608993ce44b36c8e518)
  * add custom cursor option for the simple pointer (19ce6f584383afeb715f31c2dce0e1009faad0e3)
 * **RadialMenu**
  * allow anchoring to interactable GameObjects (9238bf2c662519b79e07d1e575ac483263fbdd5e)
  * add hover events (dced6ae37b3de67d12cb7f9bca7800b6726df5c1)
 * **Testing**
  * add movement simulator (bcf2acdbab53b244166d5ea92be215ac82af4736)
 * **Tooltip**
  * hide controller tooltip if no text provided (0895b3eece47ec63a59e02f5cffa86d00d6d54cd)
 * **Tooltips**
  * add ability to toggle specific button tips (7847c5c13291d520320945fe46842876c268e183)
 * **UI**
  * add ability to ignore certain world canvases (a648e1c1f9accb101dc73dc2a58d43abe1eb1610)
 * **UIPointer**
  * add ability to scroll views with touchpad axis (da0c2d2cf930c2f15d354cdbfe1dad54a865b8c0)
  * emit events when pointer enters/exits valid target (224a832b00dad2c7a2b7c2a47c55bb73fbf5f5ed)
  * add activation mode setting (ec1cdb112e141f5acf372ae368ff8ceb90601c23)
  * add hold button to activate option (7475582bdf14a16faf1d0b2b94d1e9a34917d3f5)
 * **utility**
  * add prefab to display console logs (4e1b8fdbd2a831986e036ebdebf76deb3477fd5a)

## 2.0.0

### Bug Fixes

 * **ControllerEvents**
  * fix order of controller events (5c7173ffba073cd30b0de5be121809cb20559bf1)
 * **Controls**
  * remove warnings from unused variables (ccddc9520f5d4095923bf3acd67462b86744e34f)
 * **DeviceFinder**
  * fix for compile warning in Unity 5.4+ (6b6f40433dbbf87980e2edcdee8d9210a4975e4d)
 * **Interaction**
  * ensure touch state is reset on disable (40fa57792b310b233aad4ce334809bde45346448)
  * prevent continuous rumble on touch (083d02658544a7b4f8537c5ef61ca19d4384de41)
  * fix issue with child on controller grab (2d18b8a3ed4f8272cf3753d78887d5e16fc3180b)
  * fix issue with overlapping colliders on touch (11d5b2aadfa55cdf7f106d63a8703468e1810be8)
  * improve object tracking on track object mechanic (cc161a3039b94cc5c1e7866a2efd3cac2c1a12f4)
  * force stop interacting on object when disabled (7820b531b081384161f0e69d14614e12b467cb43)
  * ensure haptic pulse duration is a float (a219347da7e6939645a2a5a25b4ecc49ae8787f7)
  * ensure press to grab/release works on quick grab (da38086ce5f09841b03a7e3f8868ceb842fba2d3)
  * ensure touched object is always set after quick grab (f30ac8c10cf91dc9e9dce84a6b2ad557cad8c386)
  * remove physics glitch on rotation when grabbing (996fd2512316085cdefcbce8e0f4ce4a36347072)
  * fix syntax error with spring precison grab (13a3966e72e58dfe6e3178e20a151810db9c5576)
  * set precise spring anchor (cce8dd1a6bb73d4854c196b2ab2c7b248cc03da3)
  * save previous object state on grab (76142bb947dd874907c9576af3206f79283f5b03)
 * **Pointer**
  * correctly plot play area cursor based on offset (dcb81566c5384f66c7f75ab27bba524d323b78c6)
  * clean up pointer artifacts on destroy (dc6109664c8b537250834a49c7e2f625f046e80f)
  * prevent pointer events when script is disabled (87935f2c2cece749053aa0e544ccfac479506db8)
 * **Teleport**
  * fix issue with examples due to teleport code change (5c1302e3abf0e9445954edd27f48a8da02516316)
 * **spelling**
  * fix spelling error with destination marker event (b025a410f59756894be6511aa241c842f62ae1ad)

### Features

 * **CameraRig**
  * support default SteamVR prefab (aa3dcf0d90e97dd3f97b0a1de175278e0f847954)
 * **Controller**
  * add methods to get touchpad axis and angle (7187fede2c576e0a6bb794c7aeb1742a990d87f0)
  * add ability to get controller velocity (283155cfffc00a4431f248ed0c49d90038a1abd9)
 * **Headset**
  * add option to ignore certain objects for headset fade (3e63b72903378eba742519964beafc169e42d244)
 * **InteractableObject**
  * add a rigid body if missing (eeae2e9ad1d81666f4bbe9a7a3d1e699070a47a1)
 * **Interaction**
  * add chest UI control (10f5184690d2367505ddfc16e2ba3863b8be3788)
  * add custom controller model example (29e088034bd8e59a0d669a4bc924e7442af9382d)
  * allow custom rigidbody colliders for controller (f553a946307a7c5a9e82b2bf54f3f8b3ae4a4098)
  * add radial menu (27fd5f5c3d3b76bf31136db169b04d131f6252b8)
  * prevent object swapping between controllers (69a37e6a893ff9f2ef4de03fad468df9e3f4e833)
  * add additional slider auto-detect mode (b3283e19324ab5aae010799f4c4d513ccb3eff58)
  * add option to prevent certain controller interaction (e043d4ccf55df66e601ab234298dd0d54bd5ea03)
  * UI controls (7dbc7e4980848ad865014a648900976a4dde704f)
  * add haptic feedback coroutine (b246103a0de9f0768cbd85683fd1ef42dc86c235)
  * add ability to get grabbing object (ecbc4717f509352c3f262710241a012eaeda14fe)
  * add RequireComponents to grab and touch (b2afbda88c0d4248e5cf8e72b76da567a24249b1)
 * **Pointer**
  * add option for custom pointer material (c64d8e06d4d8eca9a68de20e46e43ee0a26a4348)
  * allow pointers to be placed on any object (c03b2e27bbc0d524934199112b4063fe1dffe5e8)
  * add raycast ignore layer checks (6177e13bc3fd79656ad77cdd73a6e9a83198a5c2)
  * offset playAreaCursor to hint at relative player position (4b3adf93093484f91107427def725b9eff9c6283)
 * **Presence**
  * add toggle to reset position on headset collision (04e571f43b501e65b79e484c135cb85216dc3eda)
 * **RoomExtender**
  * adds toggle, gizmo and controller example #189 (2f6c4ec2d6e0581dec8b820fdf01c09669967347)
  * add a script to move camera rig (041b05a361b8ec686ef427e7c9544490c04e224e)
 * **Teleport**
  * add ability to use navmesh for limits (b1329eb7f6bb78a08cd210cb92046623b86e70f3)
  * include sender in teleport event arguments (60a37a006d61d1c5a791aac5c4501b7d8119aa93)
  * add teleport events into HeightAdjustTeleport (352507f82063e8b8531982772524e014a1567714)
  * add events to teleport (b1826784273b4869b22970c470455e24f3f96644)
 * **Tooltips**
  * add UI object tooltips and on controllers (f75a435da4d1887a824357a983312d8c358e94ad)
 * **UI**
  * add ability to interact with unity ui (5fa29d7f159b00d9e719455f16d1d4f0bfc6b6b1)

## 1.1.0

### Bug Fixes

 * **Pointer**
  * ensure play area cursor collider matches headset (66295ab2670cafbb8df7ab31783b99a0470665e8)
 * **README**
  * provide correct sub reddit link (54422f964ef7adf7e777940f49559009a72c7ca2)

### Features

 * **Controller**
  * disable walking per controller (52578c3c67161e010320b8b357628fc27e07b25f)
  * add angle of touch on touchpad ControllerClickedEventArgs now contains touchpadAngle which is the angle in degrees of the touch on the touchpad around it's center, with 0 being up, 90 being right, 180 being down and 270 being left. (0e5ccab5da13478990055ceec7e4750278435c4f)
 * **Interaction**
  * add haptic feedback on object interactions (304263eb5017acb47c9327e0d4ed9914beb0257b)
  * add delay time for collision pausing on grab (34989d7e7590634be7af9487340ed7a7810bbe32)
  * add throw multiplier to objects (84dbd020bf289b681f89233e30df35cc173c7a5c)
  * add ability to auto grab an object (3731f999b232d63d3c57394b1638d78ba2f9ef8f)
  * prevent object dropping once grabbed (5232ed3fa3e88ca16ff8793909654e3e4c102c57)
 * **LightSaber**
  * add color pulse on blade when active (24b81339b46db2cdcdbe15d6051e7ccfc979d002)
 * **Teleport**
  * add example of teleporting without using pointers (f94d7899ed97e9263b204b7f12c60f6de3362e2b)

## 1.0.1

### Bug Fixes

 * **Interaction**
  * store highlight colors with a dictionary (5c54691c6fd1aea7e52b4ae43ea418de9b892d55)
 * **README**
  * fix faq video links (8f8a729863d2577b34e141d47e3b9baea3900d8b)

### Features

 * **Interaction**
  * add handle snap grab mechanic (d5a5585397073968347a68f9e64fd78b4987e808)

## 1.0.0

### Bug Fixes

 * **Archery**
  * add parameter for max pull distance (d76c11c413232e2c30fdf9210ad3ed3461b2e0cc)
 * **CameraRig**
  * get controllers for events from controller manager (44e331b434e9993c0954f06d2f74b454f47fa2e8)
  * add player presence to touchpad walking example (8b95b91ad1980664278b6b6e00f503b793001ede)
  * add movement deadzone to prevent walking glitch (924eb63ca8696c7a799f07752615db0bf1d25bab)
 * **Controller**
  * store connected device by object instead of index (5fe071bbb6b296357b80ac8a11ecc699688e3e5e)
  * support skinned mesh renderers in toggle model (55fded58023f090ca5c9516e950130fffab9a157)
  * add prefix to objects spawned to controller (dc0b0ed243b2476a16c8a55fc11a9397be9effbc)
 * **Controllers**
  * add listeners to controllers when they are turned on (c13d6e3176fad5d4d39cd2f8a9c4f3cbc35d93e4)
 * **Headset**
  * show warning if fade script is not present (7f46ea98c0b8bc5b2a9d6715cb6c060802c38007)
 * **Interaction**
  * prevent error when highlight on touch (e229ca8a88f961ccbf158b29d1bae787fbc5f4f7)
  * prevent crash when using object (2eb7978b244219801ebf2dd604510ce84f40bfc7)
  * prevent touch highlight when grabbing or using (4624e8f158de8c5d8ad6996961a8825448e57977)
  * add toggle for pause collisions on grab (694227f478f30379411ace3d977b23ccdf47b068)
  * prevent interacted object collision intersection (4c0823ccf085a01c05917a89f25a007ddefeec68)
  * prevent spring joint when other controller attached (f8e1dacb243220dd1c67d50113107ccab75fed93)
  * ensure lightsaber active state is set (9f4793275feee9e143574caaf89efe0c2db1a8f1)
  * prevent crash on interaction button press (de40491974793a1a9a9b63a0f80b79086667f16f)
 * **Pointer**
  * ensure pointer components are hidden by default (9faf7327dd78ba6ff763c6edf23cf52ff83c69c8)
  * ensure camera rig rotation affects pointer rotation (27c07f6da240b4bcec10e1eb7336edbec1ec8d03)
 * **Teleport**
  * prevent adjusting height to controller or grabbed object (406225f047b760dc690d5e1b173effdb1398f01d)
  * ensure headset compensation works correctly (cf46dc3559dbde46e20d359d0ebe65134143370b)
  * ensure headset eye object is found (c8e6a97bfb5247a696d14cf9b08dbf675228f255)
  * ensure the headset offset is taken into consideration (5a5fd72ca43b30ca727323ccdd1f976e8dc148b7)
 * **Teleporter**
  * prevent falling through higher block on start (39c27291e029feef737b66c6fdfdfd8e4a13db57)
 * **spelling**
  * fix error with attach and detach (d4df42dc06bbbfea2219e865dbbc173d485d90b2)

### Features

 * **CameraRig**
  * extract user rigidbody out of touchpad walking (aee1f7e5db3ac6012420d9e7020847f9777d913d)
  * add ability to move camera rig with touchpad (3582c87173378ef8c1169d4d0ce63c42a9c0965b)
 * **Controller**
  * move haptic feedback to controller actions (e8e3bfe66a0d4b5b9ae2d4db8e37f7cf476fecd4)
  * add method to trigger controller vibration (2dbcff7bf3d833379c63ed59d86de6a681c59f5e)
  * add button type axis change events (07ae21f38f444f8b46f642aacaf123f943618801)
  * add custom button alias events (c7f12ccf25c4c2251403044d22785c45714687c6)
  * add button event listener script (78c7d7885dcfacef15189db4f84c3dd54a4a607f)
 * **ControllerInteract**
  * add ability to hide controller model (3252952e90c8e0fa2c44dab208152e760aa6ca8e)
 * **Device**
  * add basic headset and controller tracking (6af96d408c484fe189db5a6cb48cf1f5e50cdee3)
 * **Gun**
  * update gun script to allow for varying shape bullets (76e4e3f4179c6e78eea09b5047b08530b061d70c)
 * **Headset**
  * add headset collision fading (ec78fec241f60577b1a9583bd6476643badeafcb)
 * **Interaction**
  * add archery demo scene to show grab mechanic (08a2cc0c24c830f00c5150f170c34492ad12ce4f)
  * add events to interactable objects (d86dbf9e8503d7eb1cc3ebdf150364d0bb768bb3)
  * add child of controller grab mechanic (a98843145ed31b7384f28c4746216b4197a4ca69)
  * add rigidbody to controller option (aa31355238a9fa57de348ba1f62b4ba6dee7bae1)
  * add grab precognition for catching objects easier (d792617ebf24b740bcfe8039d9a468bee7d4dcfd)
  * provide delay when hiding controllers (ce85bcfb2232328b0dfd5c1df83aab063760a260)
  * add position offset on grab items (057e3abbea72399a494d3ae94cf027b414f45aa1)
  * add new grab mechanics and attach types. (93fa933fada732ff19746f3896659804e8872723)
  * add spring joint on grabbing objects with joint (c4a1e2603439dfd9e63cac9bffa1b97fcd6b9bad)
  * add ability to toggle use state with world pointer (227c58b220bdd446502c4f51c563521985f9fdba)
  * add varying grab snap mechanics (b025c9a9e6980d11650481f8e45c8ad617f740f3)
  * add example to show touchpad axis control (0cd7b601e6d714f0bd3ad0296d5dd9e12ac98ae7)
  * add ability to snap object rotation on grab (e17496db4131781d76711fd89a6d41b30660acf3)
  * add ability to not need to hold buttons to interact (2ec93f2d8f4114124d7cf1a497a6a7dcf99d042d)
  * add ability to trigger actions on touch events (f281de5b7bd84bddabe93ef4dd52d36318451915)
  * example scene to show grabbed object being used (70094984df173df8dde96bf5fa1f5128b37ad94c)
  * add ability to use an interactable object (8fe401344d44e69a5e766f44b131b66ecc4c03cf)
  * emit events when controller interacts with objects (5babf82ca3b06388b8b129c6dbddb7d28bee6d23)
  * add ability to interact and grab objects (41a004513d64b7e29b98dbed5be11cac2a0b3851)
 * **Movement**
  * add ability to change play area Y on mesh colliders (39e91b82314b98f79ac2ce56f918705004279487)
 * **ObjectGrab**
  * improve fidelity of object throw velocity (ce16a9e3c85a7cb2723d61e8ce5b5f69714f0b60)
 * **Pointer**
  * add rotation to tracer objects in bezier pointer (b47f2c6b8ae6cc02cc054ea828f1e8209d5c9522)
  * add ability to provide custom game objects for beam (8baae717f8fd4f71587bae661bdf7535c586f880)
  * add pointer activation delay option (a1dba98d1d43faade90ac4367794df3075e052a1)
  * add option to have beam always on (f690eb229231563cccbec1e852c439f9028bd9f3)
  * add ability to provide custom play area cursor size (27aaec18f47dec2c1daab2c0e934b7dff53829fc)
  * add option to disable teleport on pointer (821bb5e1522bfb1ce615545723a3e0908cd28fc2)
  * add play area cursor option to all pointers (87c6ebb32744bba9aa0e1568613dcae4cfc13e08)
  * add bezier curve pointer (35340b83b743f9ccb9b874fb4540f4bd4c0bdea7)
  * separate pointer events into abstract class (569cad45958b358c9289c2a41d8511ead5a76d84)
  * add new event for giving pointer end position (619f9a5a4b7481d32ce8b916bbc75386a2113457)
  * add simple pointer for highlighting game items (728a99e362283af39afbc835a6ef0ceb69807a40)
 * **Teleport**
  * add distance blink delay option (70b4b75deb9b3fdf3d7c1bc2377b6904d7f0241b)
  * add ability to traverse mesh colliders (ad0f27450efa6bfb1f591f2628fe8f31920f4f72)
  * add ability to traverse terrain objects (eed0829ad47ffbb3a2ca4619c116911ba36b6cb2)
  * add teleportation to different object heights (e3a562898fe37cde264579ad6be9593b7f1b2148)
  * add basic teleport script for moving in game world (2ef13192f164d2219e72b36d6f57ba248e35785a)
 * **Teleporter**
  * disable teleport on headset collision (6a624bf4de765a2821b83b37e12e40093fe9c64c)
  * add ability to exclude objects as valid locations (a36dd7fc8ee1cd8fd703cb4ef24cec7e91915904)
 * **utility**
  * add new fps canvas view to display fps on headset (f161a309a859b0fb9790d878b374bab31f8b080d)