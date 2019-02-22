# Changelog

# 3.3.0

## Bug Fixes

 * **Pointer**
   * update pointer position in fixed update (65f600e2513dedfb5a82d3f30e08c5032259940e)
   * ensure direction indicator is only set on touch (42c09f9e99aaa6524b85847f054749a1bf526494)
   * child straight pointer renderer to smoothing object (a0e10d8ba7cc42ba9da7f4042c7efe30ad788f00)
   * ignore trigger colliders with play area cursor (80e0d3823f40536571ad093474217f27b3b866a5)
   * set object interactor scale correctly on renderer (07a59e4ac803d6ff081ff165dac403e5ac3506ee)
 * **RadialMenu**
   * ensure button placement is calculated correctly (b0305b02ad8ded8c23972573a8375e2ccdbfe0ca)
   * buttons have colored background (65eaad5e1bfb70aad0049724a4c256ebb29f9044)
   * remove unused image (92303e139f804e368307d56a33d81d290b0dc6a2)
   * disable mouse interactions (921ed42080967acda26fafc0a697daeaed2a5a2b)
 * **UpdatePrompt**
   * unnecessary pagination (5113b1982f3d0a6058caf82b4aa013f2992956b5)
 * **Internal**
   * ensure controller ready registration is always done (49cb91bdfb80559377cb6957179ba75dbcb8994f)
   * add editor datatypes and attributes to VRTK namespace (fc797d0990175eb0e524b99e1252f25fd8f05c23)
   * move property extensions script to editor directory (1e457bd2e896461415e0cbb308bdee5681054580)
 * **Examples**
   * rebuild oculus camera rig without a prefab (80db756d9f0a653236e3ff6f0ce4516ea1581111)
   * automatically repair broken Oculus prefab instances (c6062d3984b10e3767747721aa47e72b2136ae29)
   * remove script define symbol check and provide popup (32f94d288a5b984b79e03a65584d04ee34b8d477)
   * display error if script define symbols have not been set (b2a56f07bd6a0144cf4faa3a4cc75bbf8440bddc)
   * ensure constructor scene is available in build settings (7975664bc6f22d36715443ab3083898a5ee3f574)
   * use final scene index for constructor scene (71e25ece1ac884729daedc35f3b99d220f53f6ac)
   * auto populate simulator objects in basic grabbing scene (9e46e63341ae36f66bb5203147921fe920b77628)
   * prevent crash on scene change with active arrow (9ef2795281c9bd1470778e2e7ec3f63a63367b20)
   * remove unused variable to resolve warning (c801dc0e4a205f794db6a95a9ebb62c202c4a0b7)
 * **SharedMethods**
   * don't search in unloaded scenes (2a212384b19e48d87ea92f1dfee80ec8de7c894a)
   * find components in loading scenes (59951a2fd536b2cc3840a4828e6e0955ba511eca)
   * change FindEvenInactive to search all loaded scenes (0918143202a35696ae9d54d62048cc7a77e178c3)
 * **DeviceFinder**
   * add oculus headset type for rare es07 (71ecaa534fb9d42dc695686a6a77ba2695c3c697)
 * **Presence**
   * provide custom boxcast to ensure correct settings used (ae58f80d288cd59ba1c9918499a0c72537654b6a)
   * add null checks on colliders before ignoring collisions (bfd1fb9a3f3676f25328ac058149c2245ac5f577)
   * account for play area rotation in collision sweep (36113d41a00c05d02d4e8da63dee76f72f7f1005)
 * **controllables**
   * no longer resets value on re-enable (aa44b02fab3658b32dc829e2d0c79d85951ca949)
 * **Pointers**
   * handle destroyed origin transform follow (009d747d37a400839946dcea7fbb186913b5c9e6)
   * stop direction indicator working when script disabled (67766239010d190d461415b1b2c88db3ddcd2a07)
   * clear cached attach point on sdk change (5d9dcb333ad3a8146ceaa5c5bf69334474168b9d)
   * ensure pointer id exists when checking ui pointer length (3de1e482660acff457a4de1405b76362f7dca3eb)
   * ensure controller reference set when controller found (d0ab75b3a2f4944651e79bf1649f6764d9de7748)
   * use appropriate tip for determining pointer origin (51fdf4ebec4763a1c5b6118ce9493aa33fe7c6ae)
   * ensure visible renderers list is cleared correctly (2ede03ff0afdc296787b3ac2c56426c8c82a5a60)
   * turn off renderer when either script is disabled (d7c37bfbd6ace942cd511dbaad2551c16248c7f4)
 * **SDKManager**
   * fall back to Simulator SDK Setup (9c9aec7d630547450562ca0bf31851ca03b4331b)
   * ensure SDK Setup callbacks are called (eb472121717b49260d537fce84fde0cb69e6104a)
 * **physics**
   * change collision detection mode for 2018.3 (17995f90356b8301cb8d06a907e2b0e98111a10b)
 * **Controls**
   * ensure drawer snap force handles rotation correctly (a493d9b13084ec60321c11db5032bee5a8de93ed)
   * prevent button anchor changing on enable (915fa2fb4dfdfc54d6fb8daaec010c3887adfb0a)
 * **UI**
   * check for error with null event camera (63a1581e122d1c5d8d379e8f1de71e7f771085fa)
   * prevent crash when no pointer is attached to UI Pointer (f70d9a250524fc0d0a389659774a85d4e02486ef)
 * **Highlighter**
   * copy shadow casting mode from object renderer (0d3203d7c9c315b47ac47cbe3a1e364db6fdf65a)
   * MBP highlighter not restoring material properties (077d8e7f9c2f9b8ca984fe3a3bc7894e961972b4)
 * **UnityXR**
   * add vive pro controller names (5924d8154faac31df22c964a273de487ad92f3c3)
   * fix UnityXR stack overflow (3ba8299e503b72d5805ec13c62b45a7cf226b2ef)
   * add more joystick names (61fc2f7592f7a5083ebd8aeec7a3f66ecd43e41d)
 * **Highlighters**
   * use whiteTexture in place of new Texture (93592181d01c280fd5a86c089393bf89f4985def)
 * **Location**
   * make GetRotation in DestinationPoint to use eulerAngles (92ae954f9cdea93fb3687be8a3f9f23336f94784)
 * **Tooltips**
   * Spelling mistake in touchpadtwo (ccfb78a1aa95a6fc783d9a9f69629841be107599)
 * **Utilities**
   * update VR namespace to be XR (55a9cd69c7b7472030f139725795bed38ac309b3)
   * ensure controller ready event happens on sdk switch (60111b7a113c0c97270718e7c9f371b6d3fc29ab)
   * use local space for velocity estimation samples (3e05056af78f8c0b575d951c151bf0a71b9513c8)
   * prevent crash when switching sdk setup (adb5b25f367e2dda42220646fb1062606183029e)
 * **Locomotion**
   * prevent null reference in teleporter (cca532f77f12516965673ae952043811532bc66c)
   * offset player position after rotation in object control (eeac01e1fabb2f34266e1109b8274230823e127c)
   * continuous rotation in DragWorld (0070959fd647972e6b31d7ece95c832c7053d72f)
   * correctly set other touchpad control enable state (3812bf6af3d3baa80395bf12ce88e4159b0b71c5)
   * ensure correct rotation with dash teleport (41cb92c0744b2d2ef36e36f2af145e3063ba8e8f)
   * correctly check controller state in move in place (3389032b0850e75665867f759f64652409dfa82d)
   * apply headset compensation to force teleport (6c206d4fc5d13476be64ff2c7e54af68d3df71aa)
   * set rotation before teleport (e2ae7a769cdeaf854deb64bb96a77d56d0a22001)
   * get correct body physics collider in object control (d4ac47293dd4ac0da832a76c524cc38e39b4d71f)
   * correct teleport to destination point with dash (5f4e36007bfe2837969037776d976ada61c03990)
 * **SnapDropZone**
   * Highlight Null Check (2a4061311ec1d34c7022c41c0f885ae8f49d6fb1)
   * ensure valid object check is done at correct point (7550c10f0fc9e360ba256d02ab33962968f2cb93)
   * prioritise joint removal on highlight object (99f19cddaee1e20ff32586abe9611d67cc418c12)
   * remove shadows from highlight object (e75f59af2d5abb64be3c2d62563c144d50ecc6d7)
   * allow grab remove from unsnappable joints (5f668899b4c667d72a69ee1edd41ba8c9aea1002)
   * prevent unwanted unsnap on leaving trigger collider (9624e4e7aa967c32f8c2e52c066964d7ff1fe8c0)
   * ensure highlight always active option is honoured (9e707645440de2034f2305f72c228d7ad497bbd8)
   * ensure force snap object state is saved correctly (28e929646ff9cc869c91a3eb28e8bbd477761471)
 * **TransformModify**
   * SteamVR Dashboard reset (e9831b26d1b16b44b17a78ce94fc38d61b7405e1)
 * **highlighter**
   * index out of range (ccc7ac1332c854e2b8ea2b444a0b88827fb7ed4f)
 * **Input**
   * ensure modifiers are not null and wait for controller ready (87af9706c70aba1b16865d087f7bec7a72e12a16)
 * **CustomRaycast**
   * set default raycast layer mask to correct setting (d33adc3eb0744004963dc76e49541e5791b78398)
 * **Simulator**
   * ensure mouse lock behaviour works correctly (81274d6fa8335a17c23d229aaa03c2c6b2b5fa68)
 * **Avatar**
   * reset correct animation coroutine on disable (ac89d7ea9e39d4031e2ba995f42f869d8a83e22f)
 * **SDK**
   * handle touch and classic controller plugged in - fixes #1866 (aa10e660d013480859bf15e3f0a1885e27572291)
   * use any joystick with button mappings (110d22c6610496db94a1b8402524fd6a9a997031)
   * use new hierarcyChanged event introduced in 2018.1 (19ef457ed97fa8af667cf398c8ce79c856f9b7a9)
   * tag the CameraRig camera as MainCamera (649a42aefe74c6d04bb12db74697c6cab7888295)
   * handle multiple Oculus devices (14e8dc04ccbacf0df83674bb4d99a8c55b04974e)
   * update Google SDK vrDeviceName (1a63a57bf6d786ab8144636330ff21f868c5f6e9)
   * exclude WindowsMR from unsupported versions of Unity (6d18b3bee60bb6da50c1eb1ef0752e7f4c16fd98)
   * exclude BuildTargetGroup outside Unity Editor (1854d9b9bbc02bc8d11785941cd171cff546c29b)
   * let SDKManager work with Unity versions prior to 2017.1 (72cc3aaf6f2c1051ee882bebe25537e4c3b9ddb6)
   * ensure deprecated event is only used in old unity version (c41db4b95294175b404c6e70363ce31acb8d1d4c)
   * prevent crash in daydream sdk if camera not found (95e078ec345bae97302e2acad650c109ec9af1ae)
   * ensure oculus touch uses correct controller for button press (d5adf01cae75955eb08595bbdc15c119518e7b49)
   * prevent null exception if no controller model found (10ad77636fee591e8d7aff3e524dfd91e8812353)
   * stop adding the VRTK version symbol unnecessarily (f6c5c5d9818b21c81514e1f833f5486d0204b43f)
   * ensure simulator position is correct (a42c6418f27b6e9f3cf3b0870f1052d1c8963d5e)
 * **Interaction**
   * allow controllables to be moved without breaking (bfdf00cd3139b4d21d331465ab3bdc078a8030e2)
   * ensure the collision detector is set on custom models (0f709d763e20d8bae1770efe803da1007ce19c06)
   * ensure interactable helpers get reference to object (a34deea0f2b114522cca867a0709d53a2717fa8e)
   * do not look for grab attach/action in children (ddc5a04990e98e56bd645500489b1fc3c532e8f2)
   * prevent object highlighter always being created (0d6124d5bc2160d1dd60c23fe290575ba4fba1df)
   * ensure override buttons are reset correctly (5c21268ef4e31c132a51f0e03f02e8389dd66fe3)
   * ensure valid grab attempt is called (604097237b5748b4039d4243e438221c5d621ac0)
   * ensure controller highlighter uses correct aliases (cd8f2d13f35ce35333e4ac231c9cee325853c9c3)
   * defer unregister of tracked controller events (04d22359849dbb855c936b1dd6d44168dba2e9a8)
   * ensure haptics are reset on interaction type change (b903a5988e1fff5de572439ca2d0298a1c1b6504)
   * set transform of near touch custom collider container (41211b44d80a6eec3ca5ed7960df10cfff521110)
   * unhighlight when highlight color is clear (e99b7ec53e2f41d8c45e22f89911cac550923963)
   * prevent null exception when getting touch colliders (cdd96ad976a5b20f662f6971c52d422bb4ba9bb7)
   * change event name to prevent overriding base method (b6330af7f059037167f1cacf8a1e56513796de16)
   * ensure controllers preserve the correct visibility (2fe201d9a7c5c3002ff321c6b7f683142dc9eab9)
   * remove valid controller check from secondary action (d8f4b0f2c23ac0f92e8a321f2ae8ff200404d099)
   * determine grab rotation based on quaternion not eular (1fc162155494045cdc46b53c61216431b7c6b97d)
   * prevent coroutine running if game object is disabled (db143c3d0d842c6813a9521be64fde0095f96f3f)
   * prevent double unuse event (188a8833ae8a12279cb279a47d15a8b3f1946577)
 * **Structure**
   * clone HashSet when iterating to prevent change errors (2cb5fa94979234c84c81767d2b825bedd1fa437c)
   * ensure script references are correctly set (07e6cc5de32acc032bc1e8ba6affb825d912f7d6)
 * **Controller**
   * work around crash in Unity 2018.2 #1852 (54cb97fab953e601c5bfa6c93a5820f851993a4a)
 * **GraphicRaycaster**
   * transfer BlockingMask across (f2990b397c45d7a287ab8a772f69e27544163643)
 * **UIPointer**
   * repetitive enter events (bc02cfae0218aa93a9fc740d36aece1cdcdd164f)
   * exit events per element (35b6e7d464aba77fa0d21773e0e1da11503a9629)

## Features

 * **Avatar**
   * allow custom model to be provided (53850d0c35be20d0a4477ce1195b88b46922e621)
   * add near touch overrides to avatar hand (8b8b53f9b538ee3031cba1491389e82f0b7ffa27)
   * add basic avatar hand prefab with controller script (315911df42c1c128027efd6bcbc9e8c809209acf)
 * **DesktopCamera**
   * render desktop-only camera while in VR (58f2d5780b138aaca2fd8b16d351f85480dc613a)
 * **Examples**
   * consolidate example scenes and provide descriptions (63339816f4f0018980d5c1a40fb0c5d9c2e5dbe2)
   * add nock and arrow fire sound to bow and arrow scripts (f429777dcaae9ba2abbbcd47a4fc6cb78c771e3f)
 * **Input**
   * ability to switch script input button based on sdk type (77afbe48de930a870a6daa03e2880d83f44d115a)
 * **Interaction**
   * grip sense axis acts as button press (fce1315c9a57311591b4fd06da6f27a31771437d)
   * remove highlighting duty from interactable obejct (81cff04d05b24bba65394c44cce35eef42ed8075)
   * expose max distance delta parameter on track grab (f1ecedec86a5fdab6b856fe800e191b025c86aba)
   * add tracked controller colliders (dc68df32ffa1442df50824fa7f4be1415894ce6b)
   * support legacy animation in animation grab mechanic (0db2ffbf7749ff9e6c86f244e3921b92dfa76981)
   * add artificial slider controllable (feb920d15c0f75c36c5897d1c67201b9c9af097d)
   * add physics slider controllable (d04727cedf4bdced3c004fbac15772968b69b377)
   * add artificial rotator controllable (c7323a3fc8111834925f61279a4e6ae3aca6c064)
   * add physics rotator controllable (6f8af9862c7054cd6fe8991cba84bb3e35949c1c)
   * add artificial pusher controllable (bba545002ab66a992cbd550f562c263626b0b984)
   * add physics pusher controllable (c86351e2a8d3c9cf4eadcb03a30a1f47aefd159d)
   * add rotation set for rotate transform grab mechanic (4ab59a7719418d8a5a1942b58fb6444157d8fedb)
   * allow different rotation actions for rotate grab (438af0e4b875a320ce6af711f026ce88be556518)
   * rotate transform grab mechanic (a24ae46b301fae29477079fc7b2f22d6062d6a5a)
   * move transform grab mechanic (d4901ba60795d7b2ac1eb1a0cfa6eff123359e52)
   * control animation grab mechanic (0a5a7290970726492cc2d891798bae6d3d4ef819)
   * provide custom highlighter to controller highlighter (93fd9a53d941b8401ad5dec67988f3f4f59075b3)
   * provide custom highlighter to interactable object (40812fc6bf4faed447cd6879830c1f5163658922)
   * add near touch to interact object appearance (57ea62d19412cc1f6ce46ed32c50ac7f06b81310)
   * ability to cancel haptics on interaction end (a2e7a7a5408ee974c2bbd5e5b5bd3f0d8de2d220)
   * add near touch to interact haptics (48ed9056bfc426da402291fbdda3579a39ce90e2)
   * extract interactable object highlight to new script (c24633e72aa85ea877482a69eac583e180e2a046)
   * add near touch interactions (2fd29a8d8ece216208a9ef19d18cd1a08ab49eb7)
   * add interact object appearance script (8ca62334c0d2c771a5a5f6a26a09298734daf12d)
   * emit event when controller model is available (dc8a6cf9eb06097ac0e8c5416d310508a42002b2)
   * determine if object is hovered over snap drop zone (ff5abbe566cc2b4d52f194f5eb7a1da33dbb7149)
 * **Interactions**
   * option to automatically secondary grab object (da9f5f8e81e569dcd1b7d5889fe4faa9d8ce1790)
 * **Locomotion**
   * allow alternative axis for touchpad control (c4bf5d8afb5e17ac69f858c508240365e4e09ed7)
   * option to add skybox texture to tunnel overlay cage (f1e9901ced7d09496a78f90b6f54afd47bf59140)
   * ability to move, rotate, scale play area by dragging (17e39d292d268a6c187ceb1840c80f3884cd6e93)
   * apply play area parent position to teleporter (4e9c6050e84f763f0efb9436694cc3444ecd75f4)
   * add tunnel overlay camera effect for comfort options (3027e7f7457b6106b9981665169a3197bcfbd2ed)
   * add unity event helper for slingshot jump (822e4f30d8f4c85e6351a723efe9f81365cd132c)
   * add step multiplier locomotion method (1ac9bcc4836ed1e7c5ae94dc6b9952e128ca60f0)
   * jumping mechanic using controller slingshot (adcfe3c5be283d55c069287daaae98b6b07a31ba)
   * allow finer customisation of nav mesh limits (261da50809ee6903182282bac6a386327f74818b)
 * **Pointer**
   * add direction indicator touchpad deadzone (5ed3ccf661a819d3816d08b5e7fcfc8167335aeb)
   * allow direction indicator to be visible with cursor (fd3ad03eb17bd82e3fda777c601404f3e8a234c2)
 * **Pointers**
   * allow different axis to control direction indicator (649c5bd06924854fa0f85f4385096b6ff5e6a0fa)
   * add max length option to ui pointer (545b39d69645c6bb9f4a0550788d9994c8992430)
   * auto create pointer renderer if not set (a22b4565e2ff40cc630055e2207e0b287950a342)
   * match play area cursor rotation to direction indicator (c648dad74ffd9115139e7bdfae1f14066e72c1be)
 * **Presence**
   * add policy list to headset collision fade (3c2b5cc4735ea10108d5d6f7cf364340b533906d)
   * add always restrict falling option for body physics (35952630bc206909cc3ebe6ea912f1728b3d9f6e)
   * allow to add custom rigidbody for body physics (5fe228141054aa5ca6f665692a635bb67aee9a02)
 * **RadialMenu**
   * add dead zone to radial menu (a01a51a2f15dac2dfe26d5bd7da79e0a613b6acd)
 * **SDK**
   * add native Unity Windows Mixed Reality support (94c6cb5b54b1ed77482886c7468154ea68932a0c)
   * add touchpad two touch state (8e10a588849f29524ae8dcfa0198bd746962b259)
   * add WindowsMR support for SteamVR (1c040205617fe47e8bace8c55a7904bd5363f621)
   * utilise underlying sdk to retrieve headset type (8f61ffb664ed0e8773f8d4b4279b1ebc853e03a8)
   * add option to exclude target build platforms (7910cbe23de059e6722ce1b9b70636ab8f6404f8)
   * allow custom colour for simulator hands (5f57109574676ec9768e12025a0e39d81227ef6c)
   * add hand axis guides to simulator (c52dc182abc6832f9a7bc9a47a359e5e4dff5f72)
   * add new headset types and move to base headset sdk (d26535d6a67c973430297a76252fd3c1fc2d1154)
   * ability to synchronise the play area transform on sdk switch (aea4afd6ec20f97223f0c1ed91e140b39f0e12e2)
   * add actual controller ready events (6dfbff1636f9c9a4fe945d2d4a399dea991dd1fb)
   * support mouse button in Unity SDK (5d21083277b7a2ca04bf1b67031eeeb2aa8ca4c3)
   * add Google Cardboard support to Unity SDK (e07829c3ae6c3f0624a613d7b336201cf8e9b848)
   * implement internal Unity VR support (7b6f341e92441cc6bb8e0759b1aaf5b43494cd7c)
   * add HyperealVR SDK support (abc1dd3b953e6476946d0d6243728045a4ea33f4)
   * support GearVR input (da0bcf4d5e096a168eb2e4240af57f438350f275)
   * allow Android support for Oculus SDK (eb01c511b2dd0a2b008838bd2a6d1ba9c9cd0fa8)
   * add sense axis for capacitive touch buttons (0ac6b148c23e9d3b61711e270ca57d93fbfbe68b)
 * **SDKManager**
   * allow delayed assignment of behaviour toggles (5634f84624b8ab8f9bf0de5f9b6f3b7726178de6)
   * simplify single SDK Setup configurations (8fdf63ce4251318cad5c0c1acadef05801a07666)
 * **Simulator**
   * add toggle hotkey for mouse lock (22d8b655cfa83cdefdeaef63e7510ebe091138aa)
   * deprecate VRTK Simulator (91dbc1c187d048f1f419c75edfc8283353827451)
 * **SnapDropZone**
   * cloning snap drop zones (4766da071fbf9ebbd9764b05637aafaa731645e2)
   * add valid highlight color option (0fbef0789e6aaf6b28474f47c00ef22889bbe2af)
   * use interaction events for snapping (d02fcbd4acd7c6c28900461184a01c019813410c)
 * **Structure**
   * add ObsoleteInspector attribute (6f296ed062c18f2223ac7a32852b124f1fa0e782)
   * provide custom data types to convey better meaning (ef35e1913b031a9929307f32d631a875255e2be9)
   * remove deprecated classes and methods (508f2b2eef613153ff58d1a52313bca94db4c74e)
 * **Utilities**
   * add track object option to rigidbody follow (a911bfe38cdf42d233661ff716b71aac8ddcca6e)
   * ability to change object state based on loaded sdk (b352322c8601d0a698d1d3b7ca9547116dcdca6a)

# 3.2.1

## Bug Fixes

 * **ControllerTooptips**
   * enable tips if no HeadsetAware script is found (eebe8ea66ebb2f88d37008c14393db4e325eebd3)
 * **Interaction**
   * correctly set attach point when switching sdk (877c3286fd042ed2be3910d92d0d73ae6af76f82)
 * **Internal**
   * emit tracked controller index change events (a740146297b948a25cbc5f50f29f110729117df2)
 * **Presence**
   * calculate collider height based on global positions (5497c6cc2f42abb1305bff2ff9b68cd08d89ff95)
 * **SDK**
   * SteamVR SDK Setup unusable in build (5f24681f4c4bea15c1eae34743cb9d89e3057c25)
   * support SteamVR plugin 1.2.2 (bbdd9f92460111278a05267cb2b049ef76e89e8e)
 * **SDKManager**
   * deprecate persist on load setting (63adb7ed32367bfaa3a68b34b22cec0e8eb1e8da)
   * handle null Behaviour (d8ead58936179595005613da8267faf1c62eb2bf)
 * **SDKSetupSwitcher**
   * recognize SDKSetup camera (21ae3d3dcd2f6aa576df428efa8e6bc30cb1d243)
 * **UpdatePrompt**
   * long changelog error (dbb3f7a599c2ec28fb00aba0ba1a12dbc9e13480)

# 3.2.0

## Breaking Changes

  > The following commits introduce changes that break functionality if upgrading from version 3.1.0

 * **SDKManager**
   * runtime SDK switching using SDK Setups (f544015f353fa6cf3e9ab57571addef825d80076)
 * **Interaction**
   * leave usage when disabling InteractUse (60bd7b57c4ccaae8341f60c8349e654b5adae8e1)
   * update interact touch parameter to make sense (5fde9ab885c83d1c2a3f9890e9df3353a75011e2)

## Bug Fixes

 * **ObjectFollow**
   * runtime usage (again) (06b948b0585cf74761036d656d815fc7f0e59ada)
   * runtime usage (0b5d97b4f7ca876c6b4824f1270d6bdec1cb98e3)
 * **SDK**
   * allow setup at runtime (adc796e52e5510cf86b97e8135e8ce36eb0e52be)
   * add null reference check (d3fa259d37894f2ccd82cff082944ee8bd175652)
   * support Utilities SDK version 1.12.0 (95c33ffecd83ae9dc86952276caf0cbefb4cb6e5)
   * prevent increasing haptics duration on Oculus Touch (d69d37fc6e79395bbcc1d26d8df3a0b46405b4dd)
   * fix angular and velocity calculations (cb3c49f520d9ac23499820d81959256a46d7143c)
   * ensure simulator controllers are set up on scene change (2323c7b0fb1e53ffac2a1db6b1ec0cc0fddbc620)
   * prevent null exception on simulator when switching scene (6c6d0e5d2baa41f935c84aeccf9013ede1460b4d)
   * provide grip axis for oculus touch on steamvr (f6264adf923edff57f9bb13400baebc74cf2911d)
   * ensure object references are populated on enable (46c39396a21591bda2425c2cb15f38be0ff8b93a)
   * prevent crash when switching scene with simulator (32a589c37f5b82fbae9996a7822dd5537bdf5d24)
   * provide alternative way of getting unknown assembly types (fa0849a0dff65da3b88045247f49524254c1bbd6)
   * prevent crash on sdk setup when actual headset is null (e6b92f470725ec05c5645c316daad86d29f6ecce)
   * prevent crash using simulator prefab (fa6026e20f8abe952c58d139a8c2e0f9ab6b2af2)
   * only add SteamVR_UpdatePoses for Unity 5.6 (43638c8b0d7f0132cb8708cd6049be9ccd3fa877)
   * ensure simulator returns correct transform for headset (20a5267ceffe6cc961ac08b50db4e0c345f30221)
   * remove erroneous parameter from boundary methods (77a667db8de78f338d64ef0036dcfc5cb3aa78c1)
   * ensure SteamVR boundary mesh is generated correctly (0c3b73e51fc1d6296a0eb46726571cb44f26d363)
   * controller null reference exception (bb5ce86122268632e18aa1d5fc1b9669889a2d14)
   * name based controller lookup (bb77e483b57ed56482160a57ddb2e566e3a7d890)
   * ensure SteamVR works by default in Unity 5.6 (0b3b18535515f6208f780bacf955d3dd87810637)
   * ensure simulator returns valid play area vertices array (3fc07e685032d06041463fb9abd98097764f8b00)
   * find inactive game objects and components (ff6885cde8f0282d810dfebae63ce1c5f70682ae)
   * SteamVR defines (3522e8127c2184b8dbaecab65e51526dbfdebcc8)
   * SteamVR defines (eb96658992c870aa0ae5c9beec63ecb3f6f6296c)
 * **Pointer**
   * prevent crash when closing steam menu (2b5280c44ad582e2acee5ce125cc8fee93aec719)
   * provide default boundary data if none available (9c2f1aceed446ed3562bb64f3aad40e7fca5d9c0)
   * ensure material change is done correctly (0676113a1d819b1d4b4c4ca8438a0595dbe30c3a)
   * reset destination hit info on activate (cf205d275045f78fecf9fa3571cb787da3574d72)
   * trigger enter/exit events when controller not required (923c595500025c5934a12b5bfea39877b64d2d72)
   * defer controller setup to Update method (b744aded347733cb7fbd0a48e9e133f46eb453b8)
   * reduce BEAM_ADJUST_OFFSET distance in renderer (56b63546790111fd329d644ba101ad036d7d96a3)
   * prevent null exception when pointer has no renderer (4e483f5512fdd517b6951c8f7d9a17b03f540111)
   * ensure object interactor is set by global scale (ffa49fe9035c0d9ae7d01db31eee19cf7ae5159d)
 * **SDKSetup**
   * changes because of auto populate not serializing (66af4fbcd1ee8ec168c9edb2107ca6eaa5516c8a)
 * **PlayerObject**
   * update existing object type (2a584ecb8760a64653aad6f9e46d027ade6700d5)
 * **RoomExtender**
   * prevent error on drawing gizmo in editor (a5c7a1cf35328c5c30d0a89727d26c10683e21e1)
 * **UpdatePrompt**
   * prevent null path to version file (bad7ff9e4b05002d9ca2450d595a62f21f0466d0)
   * allow to move VRTK installation (e6fa7f6b8b5c792f9a260817fbef1dfb7149b19b)
 * **EventSystem**
   * screen space overlay desktop camera (again) (de123363cfc7842aeb9d2147f541e0573c250ae2)
   * screen space overlay desktop camera (613b7119d03372fc630b08c3d03afd5cecdc5804)
 * **setupWindow**
   * increase window height (71d62bc8a134dca873e9b1052b44eaaf3d7b459d)
 * **Internal**
   * ensure tracked controller index is set correctly (e4e5b58d8f72c42a6ca9f2085471db1f89bbf86f)
 * **Examples**
   * prevent spawning multiple arrows when holding bow (c14d36f96e6430736c8bac608ee1b9e3967e5158)
   * remove auto populate from custom controller model scene (0dfa6b9d10c946ee827cc03b65dc1b561a5caf3c)
 * **Climbing**
   * reset body collision and false fall end triggers (a3c2ac075b6b1f364a9ce5d77580f01dece2cfef)
   * scaled throw from climb (357c427891c657a6403f00e75b879e1e84e2726e)
 * **ScreenFade**
   * ensure alpha does not get locked at 1f (7c841b61b191c86c9956afaaade69c1ae81bb4fc)
   * ensure screen fade reaches full opacity (dcd69452ec2f2daade5654510263910c7ef978a1)
 * **SharedMethods**
   * standalone build errors (fd652983523f18cac7931b5ff5423052c733ba43)
 * **DeviceFinder**
   * headset type recognition (90fb4979663ee3d1dc2dc6acabff79521fa23632)
   * make sure to use correct headset transform (9731aa595763482a6975c9c3221fc7a43380d898)
 * **Presence**
   * reduce GetComponent calls in body physics update (57e9451343918d7bfd6bc4b0960b2951c99d132c)
   * remove console warning in position rewind (df4ad1aaccb730e416820fe59f55b5b5daef166c)
   * prevent crashes when switching scenes with body physics (b94d453234f0d9bee6ac79cbb027714ae37ddec4)
   * ensure position rewind does not go through walls (ebcd1392a22661cd60f56b3b2d4b8bab88537279)
   * ensure is moving isn't always toggled on in body phsyics (8add2f367ba03e2ac23fda9f2eebdc642a95bdbe)
   * prevent position rewind when body physics collisions off (08827b4bec9ecc76771999fc8696b087fa986dd7)
   * apply new collision tracker to PlayArea for body physics (02005cdab47d1e8fd5807b4227f1d5220371671e)
   * use lower precision on checking y position for falling (3073686aa2075e6e7e63b7b6d3bd2a9f8aa2dda7)
   * set crouch rewind threshold to stop fall through floor (9e6f305587bedcf493610cb44146ce02f3c93734)
 * **RoomExtender_PlayAreaGizmo**
   * null play area (1dd32c6dea2762a0298f60a32ae52c3320df979f)
 * **ControllerEvents**
   * prevent events emitting if object not active (1f8ab766294b7af6c494f26a801afc5d34cffa52)
 * **UICanvas**
   * RectTransform in 5.5.2p2 and 5.6 (0aee3b2e2329a681ae7f95c00315159556033e4d)
   * ghost in layout group (478ffbf893e37b2bc0a40678a8f304d6a4685151)
   * unusable in Unity 5.6 (1d2b1ef8e866c22a85897dbf6599c7f9e94433d0)
 * **SDKManager**
   * SDK Setup load/unload crashes on scene changes (f302adc5e5049f7d7eb3093b45d68441d017a06e)
   * SDK Setup unloading (841432c2a2baf0a8e3c2bbda54f2565b6e34279c)
   * honour script enable states (cf48f330fca346a1cc3fa3025c0537ce63bba7d8)
   * allow `-vrmode none` to load Simulator SDK Setup (04d49b792cd0e8e8bc05ae37864d792d9906f027)
   * standalone build errors (35f7eadf49b096621f05016ffa492a34603e8d7c)
   * duplicated errors on SDK changes undo (98d877967827e8d24e74a6b6f3c9cdb5f8c7ce67)
   * ignore enabled state for symbol management (689fc7d8848290b43be7a59adad6f618ae77dc20)
   * undo and redo (b2c0fa80399faa41831a9d5626b7e14dfa749aa1)
   * populate object references (47f44629b551eba84a9d8566030bc970cb58e956)
   * handle scene reload (b0927caef6bcf6b62d5172422e4424c802860665)
   * move method to editor only (5d58cfea3ace07bab3c891eaa11b0d70c1f91033)
   * enable auto populate button at all times (dd4152d389b8daac058fac69d87b052b1a5f9da8)
 * **TrackedController**
   * ensure to set public index parameter (8d5e856bfbb2b17177626571d787a4d7d40924c5)
 * **Controls**
   * rebuild grabbable slider object to work with snapping (e10be5a027f58dc3c0d654b18d75bd13a1a0a00e)
   * prevent error in panel menu on disable (f0da6cd76c7d09e2db237457ad85f3ea3d115588)
 * **Tooltips**
   * make retry init variables public (28a83932adb038a49df6ec80cb7f49a09c79668a)
   * force initialise tooltips when controller ready (b177373abbbe9b524999e2e310dd86b4638769de)
   * initialise tips when controller is enabled (f3d1ba3b50d0dc445820b4e9596516a02aa67a6d)
 * **DOCUMENTATION**
   * provide missing text for empty returns (7ab57f4e0893d403cd30868844dbd5dc224a7429)
 * **AdaptiveQuality**
   * index out of bounds when using Oculus Utilities (fc5b1465a74c685f2103a09d24adb05f77b2b76e)
 * **FPSViewer**
   * ensure correct SDK camera is set as canvas camera (7c26a2ac89dac375c72a28e745349c1b81190bf4)
 * **Utilities**
   * new vive keyword for headset discovery in device finder (90765ed6dffc6b331ca8e8671b2dd05dd2e75aea)
   * prevent sdk change event unregistering on disable (ee141c00b977fe88bf756c3463414d3cf3acbd68)
   * ensure sdk manager reference is gloabl for object alias (256e60136ae489b363d71d8665655ae493284a1e)
   * prevent error with sdk object alias on destroy (96334ddcadaa3050dffbc50c8434971010d2a3ff)
   * allow for trigger colliders to be ignored by casts (93ff8dcb5b9666b3cd410bba00ac8b5432fa46d6)
   * ensure custom raycast doesn't force infinity length (b5c0d454ef323e9681e873c8c49eb86ec83efa6a)
 * **GETTING_STARTED.md**
   * update oculus utilities download link (553db2845609586d6fdfe093b9737fabfc810bf6)
 * **Shaders**
   * prevent unity auto updating shader code (cc287dc6cea4fdd37e6f19fe4407a576e2f2bc2b)
 * **Locomotion**
   * prevent current destination point being set to null (021e92c2ffd9d61c5653e1cecec46d1fc6d3d27c)
   * replace `timeSinceLevelLoad` to prevent stuck timer (3f4d2da80b7815be4d0e73b77daef3c547c31fd6)
   * prevent exception with missing player rewind (c690c32dbd02fb2fa84f99dd80e09460bba0a35c)
   * typo in destination point locked cursor game object (1b60bac736d9fdf74d799710b44ac99fb30cb95f)
   * ensure rewind on climb collision resets correctly (2c9a61cc9d7bcc7f01e066d4e0d8d235e1d6fffc)
   * ensure snap to nearest floor is called with climbing (deec4eb1565ba9777eed9e9cfcc5b107d8e0a496)
   * move in place left controller only check (f5708a0ab1696f9bec351cdc60e8819722484f90)
   * emit dash teleport events at the correct time (98f6ef1ab17d403586bbfae878aefca81c15ea11)
   * remove teleport exclusions from example scene (fc51ae54c5b8b11bdbd9c6b7d551c4a58bdb47e6)
   * ensure play area collider is found in children (e719f44ff7112a947bc84c082237187409229035)
   * support multiple terrains with teleport (a7ce7ff248232ae120a3cce0ad21066add0ed175)
   * use correct axis check in touchpad control (1ebc2c96031a718809a9b0235b21fc50f906e85e)
   * always force small blink on distance blink delay (98e52acfa44acbd3cd70f31e72bfe81881569f31)
   * make SteamVR the default for the mesh teleport example (16f33e429f362c60fa71e18a7d13de49d3e3b7ee)
   * ensure current destination point is reset to null (5841ec358f2fea065423151dff47cdc3ed08fb66)
   * prevent unexpected change in direction in MoveInPlace (cca33dedf41f2b6a9a87ed73985bf5b55b59c165)
   * allow terrain Y not to be 0 for teleporting to work (03630c4f0937eecc8f68f592eaf7806c67edd5ce)
 * **SnapDropZone**
   * allow multiple valid objects to hover at same time (1bbe2b44b9330ee23a2231bb735c11567593c1fd)
   * prevent enter zone event firing multiple times (f2869bed8726079914ccdc1399f624b4d27e4f3f)
   * force unsnap if snapped object leaves collider (20573ef2fa3f89cdd4cd7c3d254f2f845446f25b)
   * do not require grabbed state on unhighlight (a202eed30435151be7c08869beab70675fc5f772)
   * clean up on destroy of snapped object (3b707f94629fcbace42dd7e6245cd3f67997ebb5)
   * prevent default snap at edit time (fb896bca9342bbb777bbc4b53f042f07f9278eb5)
   * allow for child colliders to activate drop zone (0bce6c3ada7413a193a2bf340355413d34428413)
   * prevent highlight turning off when set to always on (e765255b08bb22fc4bad33cb40438c046d9c5dc8)
 * **ControllerReference**
   * prevent getting reference in OnEnable (9a98d0997b2880a9a9590a5fcab7316490ec8635)
 * **Editor**
   * ensure script icons use editor texture (420d8445107e60a54f4311274430354fba5a9a77)
 * **Shader**
   * prevent shader upgrade code error in Unity 5.6 (4ddc71bdf896f7df7f9f4ec03ab2838a465692c7)
 * **OculusAvatar**
   * only set up once (4f678f0bfba239cea4c9b566fda2381ee5611180)
   * define symbol (14b4196043f4c1b340ad15785746884ca4c41619)
 * **Simulator**
   * reset unwanted rotations in simulator rig prefab (0246fb4047c62d96a14efdf5353f25745db6fb1f)
   * add simple neck model. (431d641cc26cd09c6cf0bef80b2eeb39c831b9bd)
   * missing headset fade implementations (bbf0a0366a76133681280eae74c8eaeb6b7891b2)
   * support headset fading (2a12be02417858390a0e3069634b8c932579c13a)
   * typo (85c3bec1f97a6afa65fbb1047ff6aa167e7f4c6e)
   * null headset in Editor (d344279cac90cabec01f434c06caa00a92bccddd)
   * render model null (594592167fd33462371d6994a0f8a272f5cf12dd)
   * rebuild simulator prefab to work on Unity 5.4 (6df8af6b0f9929ad179c5dc3c4d7ab4ad8c4f88b)
   * fix view rotation jump when returning from editor (ef0a387c61970f83e27218e5d18b3de8e12cbc95)
 * **Haptics**
   * ensure haptics cancel correctly when called again (3fe6e15f2ad4faeeb50304df0192f1e29c9f7274)
 * **Logger**
   * prevent custom logger being destroyed on awake (230f0ec97c441af8adf62da2e6f0067b8a41b72f)
   * usage when serialization runs (3d9e7bf63a90376ed3dd6055ea5760288bc1c45a)
   * saved to scene in Editor (a0256d9386583af61c312f5d6da28d73c50c66d3)
   * prevent crash on parameter mismatch (38c1c2f1a1455704e27843186cf746600e602a2e)
 * **Interaction**
   * prevent secondary joint break causing force release (c43ea26c0fb44a543284b62988a0955149e02d07)
   * reset collision state when clearing ignored colliders (17b012e0b8d77f18faa6892a83b7b72284bf6685)
   * prevent secondary grab when isGrabbable is false (059cb3e548c021a40b915692e489b8d1dbfbad3a)
   * recognize touchpad axis movement on oculus touch (b213073daec55c4a98f947d5fab11416ec492908)
   * prevent error when exiting after haptics (c6994c29c924e1c5199fd3958be18c301b96bf90)
   * ensure to wake rigidbody when removed from drop zone (90e829d04bf55251f8080457e0507668b0c63c91)
   * ensure touch rigidbody settings are always applied (c133fe0de2604d2e65dc4ffdd476e2c2dcdc79a2)
   * ensure joint break force is set correct on drop zones (505e1d61fa7f7510db7ffd6828dcda40432d3b9f)
   * stop disable code running if object starts disabled (43d295319e1208c79b909ec35d68635a4a43de8e)
   * ensure touch listeners are registered on interact use (80520da1c0821cdfc5239652686828e192cea73c)
   * remove rigidbody requirement from snap drop zones (54fb4124cf7ae395e9bf33b94b07af9a0b7348fe)
   * update interact touch parameter to make sense (5fde9ab885c83d1c2a3f9890e9df3353a75011e2)
   * force secondary controller release on primary drop (88ea7255913ff612921c4b31b17ea37668f88dcb)
   * allow grab on force snapped item after ungrab (96239c0278cd84e399246e29a7461fc944bc8874)
   * prevent ungrab event being emitted if not grabbed (b4b81713803f98b81291b94321087067af8e8272)
   * remove register touch listeners from update (2e4cb130ba35a2f220ffdc2e78103bfd704a4ab9)
   * check if touched collider is child of touched object (fccddb6565d6f7b4ae28d8c56535c2d4505a3275)
   * prevent crash if no ignored colliders are present (bfa93820e708164d2668111963cfcf4171ece72b)
   * remove erroneous Debug.Log (00c2a1f1d07bf6ffcf2f3eb31550eaee2db4aaf6)
   * pass relevant VRTK_InteractX to interactable object (61f2f070ef75363e5dac86a397649caeecdab8a7)
   * prevent null reference exceptions (d797104cdd0d32dbcf74e0533fc6e82541013746)
   * leave usage when disabling InteractUse (60bd7b57c4ccaae8341f60c8349e654b5adae8e1)
 * **Structure**
   * replace FindChild with Find (3a63a78466362edd08f4ae21171177362551a6bb)
   * remove underscores from enum entries (78b233583e28dcd0672d1e2ec348eb95d8f8e7cb)
 * **UIPointer**
   * ensure toggle does not execute if on existing state (f108ac281672fd5bed12bfff8b59552ca7845dbe)
   * ensure pointer events are cleaned with invalid state (e846e2dfa1a348878d866e4197e23085e6bd6fe2)
   * ensure the controller is not null before accessing (610974486af572c9c4345e9618d94a735c91fdd7)
 * **SceneChanger**
   * wrong SDK usage and errors (5eba4a4ae06b0925ee5b812974e735b0dab966ea)
 * **CurveGenerator**
   * ensure unity does not override 5.5 code (2bf3dc089d669f4e8cf83e1bc7e3fb7daa365393)
   * prevent unity auto updating code (899c2d33d173974b33507d727720023a36def802)

## Features

 * **Controller**
   * add audio haptics (3b50262cec9a8401b7f5b8080095405e01fa0b48)
 * **ControllerEvents**
   * provide alternative events for obsolete aliases (ea7ba809181d1d443ffa8ce1abd942d956bfa91e)
 * **ControllerReference**
   * add new way of referencing controllers (d564410ab1417aa8ef1e00390379e64e2240f249)
 * **ControllerTooltips**
   * provide option to auto hide tooltips (cca0b972a2c2b7ef1b4ad1f22decba5a7ca2ac16)
 * **Controls**
   * add Released event to Button control (505b594ba5a3980ba2dd436f14203ab7dd7034c8)
 * **DestinationMarker**
   * add hover event (47aea93cd9250b219f68e6df34342c5ee8a91114)
 * **DestinationPoint**
   * allow rotation of point to affect destination (09f6aaeac5d7ac2b9ae0f2514c560952bdfb1b7d)
   * emit on enter and exit destination marker event (0a0ef5313bdb9f75fa0517bf55e57683eb7ee0d3)
 * **Haptics**
   * extract haptics out of ControllerActions (529196fbe661639bfd1f10827917d625b8165130)
 * **Interaction**
   * move methods from SharedMethods to specific scripts (39e63cbf57030471c2b1d672f8d4fe2a8c0b7a1b)
   * deprecate menu toggle button alias (3d12ca0306fae54b80ebd4f87b2d19f740cbf69e)
   * allow dependencies to be injected (9c7df874a337ada717c02d2d688e6b8cf7cd5d34)
   * add snap zone events Mirror the events from `SnapDropZone` to the `InteractableObject` because at times its more useful to observe these actions from the point of view of the object being manipulated. An example might be an object that changes shape depending if it's hovering over or snapped to a drop zone. (c7a95afc97883686713d2c5c9fb846315c389315)
   * add object touch auto interact script (c7867b1b2beb7958766f737acee7166b4b426ebb)
   * move global grab/use buttons to grab/use scripts (ea3cb926b4d4423a6413aba155c6efb2e1cf934d)
   * add lower threshold for controller axis events (7ff336c8637d995f4877613e89601204125c75f9)
   * allow sub colliders to be ignored on interactables (362084c21162e1d118058b8442495d2837648820)
   * allow custom idle checks (4919ca4a56397783ada4f3c30ad9ea427bc9296b)
 * **Locomotion**
   * rename force teleport to teleport and add new method (9a8d97f300c880fca7ab29d58da89c05746f4f1a)
   * hide direction indicator on destination point (902eb3c6b93e1b6c86def532cf96f10d5f5d72a9)
   * add option to ignore snap to floor on teleporter (713b47c4ee3658b6de9240becc5fcd390144ab27)
   * allow force destination in force teleport method (1bd992653a6b5c585962aaa68d90fb53f8371cbb)
   * add method to force teleport. (2065a7f9e3c202b480dac80c1416d2b2d39a584d)
   * add specific controller direction for move in place (a37493799aa990ddec841842c91fd497aa7816d9)
   * allow move in place to not require button press (97d06c75c0ab8809a1f6738515ab6f99f9f93286)
   * add optional location offset for destination point (061af40ed41e2c2ce99e3460b482d4112f2b4571)
   * add option for custom blink fade colour on teleport (3e6e4ecb4a6fd837846d1492bf61fc84bcefe087)
 * **Logger**
   * add custom logger to better handle log events (2aa1f1e0fe49271fde135d21979ba3e540468ab3)
 * **Pointer**
   * move direction indicator into base renderer (f0b9edb2cc6a02367ad0ba912ee007eb98dcc570)
   * valid/invalid location objects for direction indicator (f4fa3313a518bd1e70a9d33b2f2a8ae0ae61c6b1)
   * allow valid/invalid color to change direction indicator (1f0c66a9a86996ce4d65771af7c59dfe123ece50)
   * emit event when pointer state is valid or invalid (56ae2afc84141a91b3c98d20495fa818fe1d9cc8)
   * add ability to hide play area cursor on invalid location (d12d7d0b9b86f51c7322e51eb8351743e283f0a2)
   * allow bezier pointer to utilise line renderer (106ece36bfbbae805a9afa874ae00c7a41ef94b5)
   * add variable downward length on bezier pointer (468d6db5b37a847f27f724cf0730069c4c9d2361)
   * allow custom valid/invalid play area cursor object (f00bee011ae903940918ed1fe74835a1aa4db0f9)
   * add customisable play area cursor game object (8016231c8effe3b4bf775cf0e2ade65cd7eea7db)
   * add pointer direction indicator prefab (da93d45a3e042fe123679f782a8026e96057fdae)
   * add method to reset pointer objects at runtime (178631091316a5ebb286d87a0f87ea8dccd9d470)
   * add selection on hover parameter (63083a2029dcc951ecfc04cb7d07ab4b8c992b8e)
   * add selection delay parameter (4791c03f23dbb180f055c42c50a4f40e328825e8)
   * expose policy list as public on destination marker (8296110651597fab972f8771bb9164d332119c2b)
 * **Presence**
   * allow custom body/feed colliders for body physics (d7dd7f38a05ccb5a5dd47ed90f65c7dc3115f0e1)
   * provide collision sweep detection for body physics (9cf962a4111e42139d1e7a19ec32670c93b8bec5)
   * option to ignore trigger colliders for head collisions (f2e7fa6629ec9174cc68e4795e2924112b77504a)
   * option to ignore game object collisions to body physics (440ad283f1dd31e11f4b0f8b5d791fe4d79146d0)
   * add policy list to position rewind (d4445f2505fe169e0454cbe2a11d56511efb7c9b)
   * option to use body colliders when doing player rewind (adb3ffe4ca7dc53ff57b41fb6b5c305216e75e1c)
   * add time till fade parameter on headset collision fade (0d13a4e72d5125bddfdce25d3e5fcf019feb693d)
   * add methods to get body physics rigidbody velocities (23d980f7fa621596f170d9aac09c2b09532e4008)
   * add step height support to body physics (d774c5a73924bb4dabefe8b9552b752e3460bce3)
   * add custom raycast option to headset controller aware (603e6e71dafef83315192a74cfad57139d9b81ed)
 * **SDK**
   * add methods to get/set boundary draw status (fa1fd3c8d7917de3d733be5ab829341b66ce8479)
   * allow additional tracked controller usage (ea345efdd668d52646d09f626190af223e11c3c6)
   * add FixedUpdate routine for controller and headset (a5e9925741c2e4b7781474050a06481e219e2d76)
   * remove force tracking origin on Oculus SDK camera rig (cd7fea9e15c27e1903a12a72034e4d96994c89cc)
   * allow for dynamic SDK discovery (025605fa2c8c27ee4a675dc5fd37fc28ba275443)
   * add Ximmerse SDK as a new SDK option (1a67a95a4379b42f1c426f8d485ecd5f5cbadd98)
 * **SDKBridge**
   * cache invalidation (9f2090b914d67fa24eb516c003eb5e0d5979b8d9)
 * **SDKManager**
   * load the previously used SDK Setup (1b179291038b41443dbfaeaa3ec416edc8a0f1d1)
   * runtime SDK switching using SDK Setups (f544015f353fa6cf3e9ab57571addef825d80076)
   * allow disabling non-SDK symbols (5163fb7e73dcf4e478e540f2ee557629fc92df0f)
   * disable inspector fields when auto-populating (2d8c97f37e6747805438a28201d75ffd0140c56a)
   * clear scripting define symbols (ca7504ba2dd9e1f7cfa15980314de0560d24183c)
 * **Simulator**
   * add sprint and new distance grab mode (9aa6e493c7716c6577c4ce56d318720db8f38a50)
   * lock mouse by default and hints background (7e2ce97451900920c06ef015963201ec54193ba8)
   * allow the simulator to lock the cursor when in use (3871cb628ede59c3d85c91b674f2891f64692af5)
   * add mouse input mode that requires a button press (7b7995b64fb1c71efc15958b5b82da5443391d97)
   * add F1 control hint toggle (4a1439b157b70d517e29208451fd60599a8b7fcc)
   * add context sensitive key mapping control hints (bce93dbc1f4d7a4d3e1ecbcd0c35b7e46767262e)
   * allow WASD movement key bindings to be changed (3d91db42d0d58ee4e2095d3e793d9f8502f6da88)
 * **SnapDropZone**
   * allow cloning snapped object on unsnap (c009276e88f913faecfdc6e7b2ec4a55496b1e97)
   * add getter method for drop zone object (3079a08b40a8e751a40a676346687a68f6b76b97)
   * add default snapped object option (64644a669955ac8720e1030c4a4b35249242500a)
 * **StraightPointerRenderer**
   * add maximum cursor scale (74366077692418d2490b4884ef36cb2f5519701e)
 * **Structure**
   * add events for all current actions (75debee8c453e3766dcc1dc52b46cad237b96d62)
   * provide getter methods for auto gen objects (b366d691f5a4da3729939732762a9af0fae6616f)
   * add components to component menu (b28da7e4f66b20d377cb3c1b5d16240a8d9f9987)
   * add context appropriate icons to VRTK scripts (d6fe3abfab1ce402082f8b2f561d1086931a0f89)
   * add standard method for naming generated objects (9bd7f880acc4fa20db1ec50151a7fbd2e6d8d625)
 * **SupportInfo**
   * add VR settings (effff8a36f7c579a8b829e222374ac8864f4b4d6)
   * add window to list and copy info for support (447ef380af0557ce42a3f456996c2c0b5b314ea1)
 * **Tooltips**
   * add option to make text always face the headset (87934efa481db63baa0e840fb40ec15000cc9d56)
 * **UIPointer**
   * include RaycastResult in UI Pointer event (3af3dec59063d6562c1a6460307763ffc9ae866d)
 * **UpdatePrompt**
   * manual update checking and improvements (d7cc00eaacfa9bf5dec9d6637ce385dca42b8920)
 * **Utilities**
   * simplify headset checking in device finder (21a6d705dbea68b4164e44b43005daff4e33d6f1)
   * ability to modify transform orientation based on sdk (a55dd41872a8933ccc4ed9dc2b974a4a3b8d1393)
   * add sdk object alias script for following SDK objects (d9c9c573e80a74519982c0c1b56920f74d51c1e4)
   * add OnPreCull option for tranform follow (b697195e44f59bee161045de8a672f72ce163ddc)
   * add custom raycast script for customising rays (61e1fe7f8c61b90a4ff6776e31c8970244ab6a83)
 * **Version**
   * add versioned symbols for VRTK (871202742ba67d4aedfe823d2a41a5d03971c6f6)
 * **setupWindow**
   * support multi selection (4728c3a5d7774059b2c6611ff6daac2f176a6117)

## Performance Improvements

 * **UpdatePrompt**
   * only check for updates every 6 hours (cc521ce86c8af198899b14a3e2d5a9512746b8c5)

# 3.1.0

  > A number of items have been deprecated in 3.1.0, any VRTK script that is reporting a deprecation warning can be ignored, however any 3rd party scripts throwing the deprecation warning should be fixed.

## Bug Fixes

 * **AdaptiveQuality**
   * ensure debug visualization moves with headset (228cf637af837e4c8e7b68dca14ee84e08924d0e)
 * **Assets**
   * ensure assets are serialized as text (1bb6396d6dc1f324961010b75aa0df33ee30271e)
 * **Climbing**
   * ensure play area momentum is kept when ungrabbing (cb60385da353111934a9481aa19e0046b3ac0f19)
   * update example scene to conform with guidelines (ceaad50d48ad465429eda8486d1f27603c450456)
   * jitter on second hand grab climb state (#928) (612aacb2292b95b31e75a4a261d43a0339f6d041)
   * add moving and rotating to VRTK_PlayerClimb (06eff77f8e236f0440abf8a81cba22e9514499be)
 * **ControllerActions**
   * ensure inactive renderer visibility is toggled (f2461d0e8b83c5c235e476c12ee83d5f58e12803)
 * **ControllerEvents**
   * ensure touchpad axis reset to zero on untouch (de2e410906c10df3dbd79c4949d0f82a809a2f7b)
   * ensure axis events take place on small changes (64461f829035b128097588dc440427800cd42007)
 * **Controls**
   * ensure door force doesn't get incorrectly applied (c8b6639b39ccbb01230fd0717566d2261b4dabe5)
   * use relative force to snap door shut (e70ba6492470c924229998cd701264956acd2d12)
   * rotateTowards working on all platforms (92b5f14fc62374e0424b853336b279a4691266ef)
 * **GETTING_STARTED.md**
   * add web links to document footer (9e9f0851c8585b5e0b9b291e36739d5efc0ef852)
 * **Highlighters**
   * fix syntax error with use of CombineMeshes (608b00746cfee1d71f36fd2a634ac90769aa4e56)
   * explicitly check for null arrays in outline (8739232c68150e646d6b6bfe467485da2bb6e8ed)
   * ensure custom outline arrays are never null (1fb9016accb2dcf84c58065de4dc7a554d330b70)
   * provide option to unhighlight on object disable (43fe9d47188ec09ae06d4d942cada22d8366f158)
   * ensure highlighter is turned off on disable (ad9f6b97bfc7db61e58ff189886e8cf6ff5eeeb1)
   * Use local scale of 1,1,1 to match parent size (e33373906700ab75ec41a09e74c418dbdc7a1eac)
   * ensure cloned object parent is set first (6bbe13e5188a9132ab74e9c6c72fe0b39c5f00c1)
 * **Interaction**
   * ensure touchpad axis only reset on touchpad untouch (79a19724465e420bf8a19550e934678acb4fbb4d)
   * only set force dropped when stop grabbing (866c93c053b46abe7ce549ed205665642a4a66c3)
   * rebuild slider control to use joints (6201641108e81077351283fce01a41b567e552fe)
   * provide more natural door control (3c375b87eed9f4b093652ba37ced1a679db540ba)
   * ensure drawer control operates correctly when rotated (994c9eab984964e22444f909acd3893ffbd4fdab)
   * scale of the outline highlighter cloned object (a63536ca06f844238cc83ee526b9c813f814e0a6)
   * prevent rotator track applying velocity on release (3b724e5eca083326e63d4c4556cfb2c27dd83cfc)
   * do not set auto grabbed object to kinematic (6c6f9f89a54c7e33353d6ac89d777f1e813aa626)
   * ensure auto grab works correctly with prefabs (40ec5d04ca41901daba800023ef4a739e957656a)
   * set error if helper script not on interactable object (89c7de9e543b35f34ca27f11cbaefea9792a1e42)
   * parent of the outline highlighter cloned object (af45d441efe47fc534db5ee14297d07d24423b48)
   * prevent pointer use constantly calling start using (4edb5110bfc6b326be6ea7551ae06a439ea84ba0)
   * ensure a valid using object is present before haptics (7dd35a95237bf97ac5c812f487b39e311cd67ca3)
 * **Internal**
   * remove null coalescing operator usage (??) (a3d898338fef3fc572d960dcbea0840194fb27d5)
 * **Locomotion**
   * check for null marker in destination point on disable (ad5ba0a940cffa063a06f7d1a6eedfa0df4f9309)
   * force controller listener setup on destination points (09be398dafb42ad714f266b50bc3283bca7512cc)
   * prevent MoveInPlace control work whilst falling (6cec273cb5bab42b7f797984af1e27c302266e9b)
   * enable MoveInPlace engage button change at runtime (9897a62a9814d7a09970b930503e83c8e26b2613)
   * prevent touchpad control of play area when falling (5b5a1742752973f2da4b2ea17e7e5a79f95fe157)
   * ensure listener coroutine is not run if disabled (3e3a11c61f345722efee881b5ce21ddd6dbda61b)
 * **Pointer**
   * prevent flickering when activating an always on pointer (40ea5a08261c1e33544e726ca2e83cb51ec2d7fa)
   * always on visibility (7e1d316d2b71c1e14f7e8bf5509bd362bb726bd0)
   * ensure visibility and interaction states are toggled (1cdf73e7b3489e850c7327beece28626a6205f7c)
   * ensure required include is used for unity 5.5 (8d796fa9424e865db2928122bd0fd243ab87393c)
   * ensure straight pointer elements can be toggled (beb2fe123167d3871e10af07c16af197f46f4e8e)
   * ensure bezier pointer tracer does not flicker on enable (fbca304175fb5a576e558564de6fc52df72622bb)
   * ensure enum comments match actual enum names (dd31f991b37cc68202929cca8b186b18b527ba6c)
   * update interactor collider in FixedUpdate (caf41abebdeccb6bb2d3b8320763fb97f4ebddc7)
   * ensure pointer interaction collider is sized correctly (e981b46eaaf7ec87c913a7e7f0d8ec18e8fbe3fc)
 * **PositionRewind**
   * reset play area velocity on rewind. (dcb8ea47af3e4a351391ec7858409e16ce747b89)
 * **RadialMenu**
   * remove override button settings from example scene (b1fb11c4e3d08c3140997144beeda8017c2bf56c)
   * ensure button unity events have been initalised (a1fb48915922f8e2eb207da4f400bc95d8495ace)
 * **SDK**
   * support SteamVR plugin 1.2.1 (f51d06c72e4aa5000a9858bfaad12dd80386974e)
   * ensure custom Daydream code only executes when SDK selected (fbf44fbe8089469140b07a931b791f71f88fdd71)
   * ensure simulator hand rotation is on top level controller (e6e063fd17664593db28f830c24e35146888f119)
   * ensure consistent button mapping on oculus touch for steamvr (b94b98126c36411b7371a242a9b654eb4e83718d)
   * retrieval of controllers in simulator (38c859c0cdab3db08571c6082e8b334019d777c1)
   * ensure SteamVR returns false for button two touch (1f92c136a849d40f29c2afae3a0fded3c8a70537)
 * **SnapDropZone**
   * save interactable object state before force snap (cf7c7a6c3fbb5c907a2e6bc4f3c107618c31b98c)
 * **TransformFollow**
   * update scenes to use new transform follow script (378e0f0adb7fc305b00e3e5bfef6b17f49df4809)
 * **UI**
   * allow non-VR UI usage (c253b1f5b8a9db4ff78e80b626ccb5a67874032d)
   * handle ending a drag over no canvas (aec9aec6db5cd86c8b7a8ce100605544bfd8da66)
 * **UIPointer**
   * clear existing pointer enter on invalid hover (ac0b8c79a835bd0d28f2bbe4228d351498cf8cbd)
 * **UnityEvents**
   * ensure all unity event helpers initialise event. (22cde734f99a6e9ef5ab4c5c7769f973a53173bf)
 * **objectSetup**
   * force button overrides to undefined. (cf433e079f0814514354bb4716294a1ad80b3abd)

## Features

 * **Controls**
   * implement c# delegates in place of unity events (ff78129480de0cdfe9cd3aadde3b9ecde01ac398)
 * **Editor**
   * add quick select sdk option (5f83638703c8bc07184736da6006bf428053e77d)
 * **Highlighters**
   * add multiple object support in outline highlighter (8b15670440b9020bfc1cbcc38fa6bf8af3afe7e9)
   * add highlighter using MPB for material swap (7e9fc87579a3b73e0752f7630a6bfe13e72a88d6)
 * **Interaction**
   * add friction/spring options to lever/spring lever (28c3754e010fd1b73de70ff12639595d3d682888)
   * add wheel 3d control (fadc29d58eb4aea68bb349203621a61d9fddb3e6)
   * allow for velocity limits on track object grab (bed58670e45b8288bfa44fd1e176518f06134a07)
   * ensure correct order of events (bd47157df3375bf2eb098050041d9bbbdd2afed2)
 * **Locomotion**
   * abstract touchpad control to object control (8ac0b637c260eebba056730f8846fbc9c30ed50e)
   * add destination point prefab (d76ab68499e29ecba111597fc4e22b63a3540a7a)
   * add axis threshold to snap rotate touchpad control (105c9c1c8c84ed88296cf4389cd626055a714256)
   * add unity events helper for touchpad control (e9f73e53cc4957e7c04a25f649ec0bf2b44855fc)
   * use events with touchpad control instead of scripts (97ea3f1542305a1a1f84944819599dce1c5cfcec)
   * add warp touchpad control action (4b5665d2b32cf8f7a7e93f0bd74842bcb15b36eb)
   * add snap rotation touchpad control action (c2669b0c0ec6a71e01daa1b9c45ab95255e627f0)
   * add rotation touchpad control action (8b0a80b20555979b15d7b48ae38219c385c28cee)
   * add new generic touchpad control script (31fc04b5a06c22b71f9dd21f3e25cdc94d67386d)
   * add falling deceleration to MoveInPlace (f659edd3d0c8385ea01bf1d315e3acb54a11d4fd)
   * add deceleration to MoveInPlace (7a1df514a6f514e13c793bb92de7b338eb22d24a)
   * add improved touchpad movement script (5b902d493fb0f1ccf983e14c53da046cd0039ef3)
   * add speed multiplier button to touchpad walking (a57af77675291ceee5f4eaf682c70e7deabbbdc3)
 * **Pointer**
   * create new decoupled pointer script (e5ca6ea29bf2f9a8f852d47a4f32ddc2ca2ad0e4)
   * allow overriding of base pointer controller events (d0bc44dad580804cdf8a10a1ad933dce6c166286)
 * **SDK**
   * implements the Daydream SDK for VRTK (6c23fd179a6059ff5d01381be3488ab531141076)
   * allow configuration of controller simulator keys (5ff229df376fd7f1ebbfda92539c9f5d227243c6)
   * simulator supporting touch events (40817c18a5f384f4542470b574e98fe0746851bc)
   * add support for start button (ad631ebf116716170263f764c7b7db750bcbcbcc)
 * **UIPointer**
   * remove dependency on global button alias (3f9f4095fae5a28122b3c70a240e790333ecc322)
   * allow for auto click after pointer hover timer (94c6ee8c0b38484e3a72404d071b8c24181327f6)
 * **headset_velocity**
   * add methods to get velocities for headset (899e49c4273b7239189a89585340e2da735aa179)
 * **smoothing**
   * add generic smoothing and pointer smoothing settings (23e87e7940c8aedbe9360394229f730679bc7fcf)

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