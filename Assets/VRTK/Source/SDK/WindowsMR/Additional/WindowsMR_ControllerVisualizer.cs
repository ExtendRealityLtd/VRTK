namespace VRTK
{
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR_WIN
    using System;
    using System.Runtime.InteropServices;
#endif

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    using GLTF;
    using System.Collections;
    using UnityEngine.XR.WSA.Input;
    using HoloToolkit.Unity;

#if !UNITY_EDITOR
using Windows.Foundation;
using Windows.Storage.Streams;
#endif
#endif

    public class WindowsMR_ControllerVisualizer : MonoBehaviour
    {
#if VRTK_DEFINE_SDK_WINDOWSMR

        [Tooltip("This setting will be used to determine if the model should be animated.")]
        [SerializeField]
        private bool animateControllerModel = true;

        [Tooltip("This setting will be used to determine if the model should always be the alternate. If false, the platform controller models will be prefered, only if they can't be loaded will the alternate be used. Otherwise, it will always use the alternate model.")]
        [SerializeField]
        private bool alwaysUseAlternateModel = false;

        [Tooltip("Use a model with the tip in the positive Z direction and the front face in the positive Y direction. To override the platform controller model set AlwaysUseAlternateModel to true; otherwise this will be the default if the model can't be found.")]
        [SerializeField]
        protected GameObject alternateController;

        [Tooltip("Use this to override the indicator used to show the user's touch location on the touchpad. Default is a sphere.")]
        [SerializeField]
        protected GameObject touchpadTouchedOverride;

        [Tooltip("This material will be used on the loaded glTF controller model. This does not affect the above overrides.")]
        [SerializeField]
        protected UnityEngine.Material GLTFMaterial;

        private WindowsMR_TrackedObject trackedObject;

        private WindowsMR_ControllerInfo controllerInfo;

        public delegate void OnControllerModelLoadedDelegate(InteractionSourceHandedness handedness);
        public event OnControllerModelLoadedDelegate OnControllerModelLoaded;

#if UNITY_EDITOR_WIN
        [DllImport("MotionControllerModel")]
        private static extern bool TryGetMotionControllerModel([In] uint controllerId, [Out] out uint outputSize, [Out] out IntPtr outputBuffer);
#endif
        private void Update()
        {
            if (trackedObject != null)
            {
                UpdateControllerButtonStates();
            }
        }

        public void LoadControllerModel(InteractionSource source, WindowsMR_TrackedObject trackedObject)
        {
            StartCoroutine(Co_LoadControllerModel(source));
            this.trackedObject = trackedObject;
        }

        private IEnumerator Co_LoadControllerModel(InteractionSource source)
        {
            if (alwaysUseAlternateModel)
            {
                if (alternateController == null)
                {
                    Debug.LogWarning("Always use the alternate model is set on " + name + ", but the alternate controller model was not specified.");
                    yield return LoadSourceControllerModel(source);
                }
                else
                {
                    LoadAlternateControllerModel(source);
                }
            }
            else
            {
                yield return LoadSourceControllerModel(source);
            }
        }

        private IEnumerator LoadSourceControllerModel(InteractionSource source)
        {
            byte[] fileBytes;
            GameObject controllerModelGameObject;

            if (GLTFMaterial == null)
            {
                Debug.Log("If using glTF, please specify a material on " + name + ".");
                yield break;
            }

#if !UNITY_EDITOR 
            // This API returns the appropriate glTF file according to the motion controller you're currently using, if supported.
            IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = source.TryGetRenderableModelAsync();

            if (modelTask == null)
            {
                Debug.Log("Model task is null; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }

            while (modelTask.Status == AsyncStatus.Started)
            {
                yield return null;
            }

            IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

            if (modelStream == null)
            {
                Debug.Log("Model stream is null; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }

            if (modelStream.Size == 0)
            {
                Debug.Log("Model stream is empty; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }

            fileBytes = new byte[modelStream.Size];

            using (DataReader reader = new DataReader(modelStream))
            {
                DataReaderLoadOperation loadModelOp = reader.LoadAsync((uint)modelStream.Size);

                while (loadModelOp.Status == AsyncStatus.Started)
                {
                    yield return null;
                }

                reader.ReadBytes(fileBytes);
            }
#else
            IntPtr controllerModel = new IntPtr();
            uint outputSize = 0;

            if (TryGetMotionControllerModel(source.id, out outputSize, out controllerModel))
            {
                fileBytes = new byte[Convert.ToInt32(outputSize)];

                Marshal.Copy(controllerModel, fileBytes, 0, Convert.ToInt32(outputSize));
            }
            else
            {
                Debug.Log("Unable to load controller models; loading alternate.");
                LoadAlternateControllerModel(source);
                yield break;
            }
#endif

            controllerModelGameObject = new GameObject();
            controllerModelGameObject.name = "glTFController";
            GLTFComponentStreamingAssets gltfScript = controllerModelGameObject.AddComponent<GLTFComponentStreamingAssets>();
            gltfScript.ColorMaterial = GLTFMaterial;
            gltfScript.NoColorMaterial = GLTFMaterial;
            gltfScript.GLTFData = fileBytes;

            yield return gltfScript.LoadModel();

            FinishControllerSetup(controllerModelGameObject, source.handedness, source.id);

            OnControllerModelLoaded(source.handedness);
        }

        private void LoadAlternateControllerModel(InteractionSource source)
        {
            GameObject controllerModelGameObject;
            if (alternateController != null)
            {
                controllerModelGameObject = Instantiate(alternateController);
            }
            else
            {
                //loadingControllers.Remove(source.id);
                return;
            }

            FinishControllerSetup(controllerModelGameObject, source.handedness, source.id);
        }

        private void FinishControllerSetup(GameObject controllerModelGameObject, InteractionSourceHandedness handedness, uint id)
        {
            string handednessString = handedness.ToString();
            var parentGameObject = new GameObject
            {
                name = handednessString + "ControllerModel"
            };

            parentGameObject.transform.parent = transform;
            parentGameObject.transform.localPosition = Vector3.zero;
            parentGameObject.transform.localRotation = Quaternion.identity;
            controllerModelGameObject.transform.parent = parentGameObject.transform;
            controllerModelGameObject.transform.localPosition = Vector3.zero;
            controllerModelGameObject.transform.localRotation = Quaternion.identity;

            controllerInfo = new WindowsMR_ControllerInfo() { ControllerParent = parentGameObject };
            if (animateControllerModel)
            {
                controllerInfo.LoadInfo(controllerModelGameObject.GetComponentsInChildren<Transform>(), this);
            }
        }

        private void UpdateControllerButtonStates()
        {
            if (animateControllerModel && controllerInfo != null)
            {
                controllerInfo.AnimateSelect(trackedObject.GetPressAmount(InteractionSourcePressType.Select));

                controllerInfo.AnimateGrasp(trackedObject.GetPress(InteractionSourcePressType.Grasp));
                
                controllerInfo.AnimateMenu(trackedObject.GetPress(InteractionSourcePressType.Menu));
                
                controllerInfo.AnimateThumbstick(trackedObject.GetPress(InteractionSourcePressType.Thumbstick), trackedObject.GetAxis(InteractionSourcePressType.Thumbstick));
                
                controllerInfo.AnimateTouchpad(trackedObject.GetPress(InteractionSourcePressType.Touchpad), trackedObject.GetTouch(InteractionSourcePressType.Touchpad), trackedObject.GetAxis(InteractionSourcePressType.Touchpad));
            }
        }
        
        public GameObject SpawnTouchpadVisualizer(Transform parentTransform)
        {
            GameObject touchVisualizer;
            if (touchpadTouchedOverride != null)
            {
                touchVisualizer = Instantiate(touchpadTouchedOverride);
            }
            else
            {
                touchVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                touchVisualizer.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
                touchVisualizer.GetComponent<Renderer>().sharedMaterial = GLTFMaterial;
            }

            Destroy(touchVisualizer.GetComponent<Collider>());
            touchVisualizer.transform.parent = parentTransform;
            touchVisualizer.transform.localPosition = Vector3.zero;
            touchVisualizer.transform.localRotation = Quaternion.identity;
            touchVisualizer.SetActive(false);
            return touchVisualizer;
        }

        public string GetPathToButton(SDK_BaseController.ControllerElements element)
        {
            if (controllerInfo == null)
            {
                return null;
            }

            return controllerInfo.GetPathToVisualizedButton(element);
        }
#endif
    }
}