// Outline Object Copy|Highlighters|40040
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Multiple Outline Object Copy Highlighter works by making a copy of a mesh and adding an outline shader to it and toggling the appearance of the highlighted object.
    /// </summary>
    public class VRTK_MultipleOutlineObjectCopyHighlighter : VRTK_BaseHighlighter
    {
        [Tooltip("The thickness of the outline effect")]
        public float thickness = 1f;
        [Tooltip("The GameObjects to use as the model to outline. If at least one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.")]
        public GameObject[] customOutlineModels;

        private Material stencilOutline;
        private GameObject[] highlightModels;
        private string[] copyComponents = new string[] { "UnityEngine.MeshFilter", "UnityEngine.MeshRenderer" };

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'thickness', float&gt;` - Same as `thickness` inspector parameter.\r     * `&lt;'customOutlineModel', GameObject&gt;` - Same as `customOutlineModel` inspector parameter.\r     * `&lt;'customOutlineModelPath', string&gt;` - Same as `customOutlineModelPath` inspector parameter.</param>
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
            CreateHighlightModel();
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

                foreach (var model in highlightModels)
                {
                    model.SetActive(true);
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
            if (highlightModels != null && highlightModels.Length > 0)
            {
                foreach (var model in highlightModels)
                    model.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (highlightModels != null)
            {
                foreach (var model in highlightModels)
                {
                    Destroy(model);
                }
            }

            Destroy(stencilOutline);
        }

        private void SetOptions(Dictionary<string, object> options = null)
        {
            var tmpThickness = GetOption<float>(options, "thickness");
            if (tmpThickness > 0f)
            {
                thickness = tmpThickness;
            }

            var tmpCustomModel = GetOption<GameObject[]>(options, "customOutlineModels");
            if (tmpCustomModel != null)
            {
                customOutlineModels = tmpCustomModel;
            }
        }

        private void DeleteExistingHighlightModels()
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

        private void CreateHighlightModel()
        {
            if (customOutlineModels != null && customOutlineModels.Length > 0)
            {
                List<GameObject> modelsToAdd = new List<GameObject>();
                foreach (var model in customOutlineModels)
                {
                    GameObject copyModel = (model.GetComponent<Renderer>() ? model : model.GetComponentInChildren<Renderer>().gameObject);
                    if (copyModel == null)
                    {
                        copyModel = (GetComponent<Renderer>() ? gameObject : GetComponentInChildren<Renderer>().gameObject);
                    }

                    if (copyModel == null)
                    {
                        Debug.LogError("No Renderer has been found on the model to add highlighting to");
                        return;
                    }

                    var highlightModel = new GameObject(name + "_HighlightModel");
                    highlightModel.transform.SetParent(transform);
                    highlightModel.transform.position = copyModel.transform.position;
                    highlightModel.transform.rotation = copyModel.transform.rotation;
                    highlightModel.transform.localScale = Vector3.one;

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
                        highlightModel.GetComponent<MeshFilter>().mesh = copyMesh.mesh;
                        highlightModel.GetComponent<Renderer>().material = stencilOutline;
                    }
                    highlightModel.SetActive(false);
                    modelsToAdd.Add(highlightModel);
                }

                highlightModels = modelsToAdd.ToArray();
                foreach (var model in highlightModels)
                {
                    VRTK_PlayerObject.SetPlayerObject(model, VRTK_PlayerObject.ObjectTypes.Highlighter);
                }

            }

        }
    }
}
