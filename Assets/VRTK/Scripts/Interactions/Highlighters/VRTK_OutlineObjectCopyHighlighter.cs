// Outline Object Copy|Highlighters|40030
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Outline Object Copy Highlighter works by making a copy of a mesh and adding an outline shader to it and toggling the appearance of the highlighted object.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the outline highlighting on the green sphere when the controller touches it.
    ///
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the outline highlighting if the left controller collides with the green box.
    /// </example>
    public class VRTK_OutlineObjectCopyHighlighter : VRTK_BaseHighlighter
    {
        [Tooltip("The thickness of the outline effect")]
        public float thickness = 1f;
        [Tooltip("The GameObjects to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.")]
        public GameObject[] customOutlineModels;
        [Tooltip("A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.")]
        public string[] customOutlineModelPaths;
        [Tooltip("If the mesh has multiple sub-meshes to highlight then this should be checked, otherwise only the first mesh will be highlighted.")]
        public bool enableSubmeshHighlight = false;

        protected Material stencilOutline;
        protected GameObject[] highlightModels;
        protected string[] copyComponents = new string[] { "UnityEngine.MeshFilter", "UnityEngine.MeshRenderer" };

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'thickness', float&gt;` - Same as `thickness` inspector parameter.\r     * `&lt;'customOutlineModels', GameObject[]&gt;` - Same as `customOutlineModels` inspector parameter.\r     * `&lt;'customOutlineModelPaths', string[]&gt;` - Same as `customOutlineModelPaths` inspector parameter.</param>
        public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
        {
            usesClonedObject = true;

            if (stencilOutline == null)
            {
                stencilOutline = Instantiate((Material)Resources.Load("OutlineBasic"));
            }
            SetOptions(options);
            ResetHighlighter();
        }

        /// <summary>
        /// The ResetHighlighter method creates the additional model to use as the outline highlighted object.
        /// </summary>
        public override void ResetHighlighter()
        {
            DeleteExistingHighlightModels();
            //First try and use the paths if they have been set
            ResetHighlighterWithCustomModelPaths();
            //If the custom models have been set then use these to override any set paths.
            ResetHighlighterWithCustomModels();
            //if no highlights set then try falling back
            ResetHighlightersWithCurrentGameObject();
        }

        /// <summary>
        /// The Highlight method initiates the outline object to be enabled and display the outline colour.
        /// </summary>
        /// <param name="color">The colour to outline with.</param>
        /// <param name="duration">Not used.</param>
        public override void Highlight(Color? color, float duration = 0f)
        {
            if (highlightModels != null && highlightModels.Length > 0)
            {
                stencilOutline.SetFloat("_Thickness", thickness);
                stencilOutline.SetColor("_OutlineColor", (Color)color);

                for (int i = 0; i < highlightModels.Length; i++)
                {
                    if (highlightModels[i])
                    {
                        highlightModels[i].SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// The Unhighlight method hides the outline object and removes the outline colour.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="duration">Not used.</param>
        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            if (highlightModels != null)
            {
                for (int i = 0; i < highlightModels.Length; i++)
                {
                    if (highlightModels[i])
                    {
                        highlightModels[i].SetActive(false);
                    }
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (customOutlineModels == null)
            {
                customOutlineModels = new GameObject[0];
            }

            if (customOutlineModelPaths == null)
            {
                customOutlineModelPaths = new string[0];
            }
        }

        protected virtual void OnDestroy()
        {
            if (highlightModels != null)
            {
                for (int i = 0; i < highlightModels.Length; i++)
                {
                    if (highlightModels[i])
                    {
                        Destroy(highlightModels[i]);
                    }
                }
            }
            Destroy(stencilOutline);
        }

        protected virtual void ResetHighlighterWithCustomModels()
        {
            if (customOutlineModels != null && customOutlineModels.Length > 0)
            {
                highlightModels = new GameObject[customOutlineModels.Length];
                for (int i = 0; i < customOutlineModels.Length; i++)
                {
                    highlightModels[i] = CreateHighlightModel(customOutlineModels[i], "");
                }
            }
        }

        protected virtual void ResetHighlighterWithCustomModelPaths()
        {
            if (customOutlineModelPaths != null && customOutlineModelPaths.Length > 0)
            {
                highlightModels = new GameObject[customOutlineModels.Length];
                for (int i = 0; i < customOutlineModelPaths.Length; i++)
                {
                    highlightModels[i] = CreateHighlightModel(null, customOutlineModelPaths[i]);
                }
            }
        }

        protected virtual void ResetHighlightersWithCurrentGameObject()
        {
            if (highlightModels == null || highlightModels.Length == 0)
            {
                highlightModels = new GameObject[1];
                highlightModels[0] = CreateHighlightModel(null, "");
            }
        }

        protected virtual void SetOptions(Dictionary<string, object> options = null)
        {
            var tmpThickness = GetOption<float>(options, "thickness");
            if (tmpThickness > 0f)
            {
                thickness = tmpThickness;
            }

            var tmpCustomModels = GetOption<GameObject[]>(options, "customOutlineModels");
            if (tmpCustomModels != null)
            {
                customOutlineModels = tmpCustomModels;
            }

            var tmpCustomModelPaths = GetOption<string[]>(options, "customOutlineModelPaths");
            if (tmpCustomModelPaths != null)
            {
                customOutlineModelPaths = tmpCustomModelPaths;
            }
        }

        protected virtual void DeleteExistingHighlightModels()
        {
            var existingHighlighterObjects = GetComponentsInChildren<VRTK_PlayerObject>(true);
            for (int i = 0; i < existingHighlighterObjects.Length; i++)
            {
                if (existingHighlighterObjects[i].objectType == VRTK_PlayerObject.ObjectTypes.Highlighter)
                {
                    Destroy(existingHighlighterObjects[i].gameObject);
                }
            }
        }

        protected virtual GameObject CreateHighlightModel(GameObject givenOutlineModel, string givenOutlineModelPath)
        {
            if (givenOutlineModel != null)
            {
                givenOutlineModel = (givenOutlineModel.GetComponent<Renderer>() ? givenOutlineModel : givenOutlineModel.GetComponentInChildren<Renderer>().gameObject);
            }
            else if (givenOutlineModelPath != "")
            {
                var getChildModel = transform.FindChild(givenOutlineModelPath);
                givenOutlineModel = (getChildModel ? getChildModel.gameObject : null);
            }

            GameObject copyModel = givenOutlineModel;
            if (copyModel == null)
            {
                copyModel = (GetComponent<Renderer>() ? gameObject : GetComponentInChildren<Renderer>().gameObject);
            }

            if (copyModel == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_OutlineObjectCopyHighlighter", "Renderer", "the same or child", " to add the highlighter to"));
                return null;
            }

            GameObject highlightModel = new GameObject(name + "_HighlightModel");
            highlightModel.transform.SetParent(copyModel.transform.parent, false);
            highlightModel.transform.localPosition = copyModel.transform.localPosition;
            highlightModel.transform.localRotation = copyModel.transform.localRotation;
            highlightModel.transform.localScale = copyModel.transform.localScale;
            highlightModel.transform.SetParent(transform);

            foreach (var component in copyModel.GetComponents<Component>())
            {
                if (Array.IndexOf(copyComponents, component.GetType().ToString()) >= 0)
                {
                    VRTK_SharedMethods.CloneComponent(component, highlightModel);
                }
            }

            var copyMesh = copyModel.GetComponent<MeshFilter>();
            var highlightMesh = highlightModel.GetComponent<MeshFilter>();
            if (highlightMesh)
            {
                if (enableSubmeshHighlight)
                {
                    List<CombineInstance> combine = new List<CombineInstance>();

                    for (int i = 0; i < copyMesh.mesh.subMeshCount; i++)
                    {
                        CombineInstance ci = new CombineInstance();
                        ci.mesh = copyMesh.mesh;
                        ci.subMeshIndex = i;
                        ci.transform = copyMesh.transform.localToWorldMatrix;
                        combine.Add(ci);
                    }

                    highlightMesh.mesh = new Mesh();
                    highlightMesh.mesh.CombineMeshes(combine.ToArray(), true, false);
                }
                else
                {
                    highlightMesh.mesh = copyMesh.mesh;
                }

                highlightModel.GetComponent<Renderer>().material = stencilOutline;
            }
            highlightModel.SetActive(false);

            VRTK_PlayerObject.SetPlayerObject(highlightModel, VRTK_PlayerObject.ObjectTypes.Highlighter);

            return highlightModel;
        }
    }
}
