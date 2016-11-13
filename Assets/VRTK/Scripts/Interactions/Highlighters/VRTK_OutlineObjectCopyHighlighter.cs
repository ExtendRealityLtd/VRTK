// Outline Object Copy|Highlighters|0030
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
        [Tooltip("The GameObject to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.")]
        public GameObject customOutlineModel;
        [Tooltip("A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.")]
        public string customOutlineModelPath = "";

        private Material stencilOutline;
        private GameObject highlightModel;
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
            if (highlightModel)
            {
                stencilOutline.SetFloat("_Thickness", thickness);
                stencilOutline.SetColor("_OutlineColor", (Color)color);

                highlightModel.SetActive(true);
            }
        }

        /// <summary>
        /// The Unhighlight method hides the outline object and removes the outline colour.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="duration">Not used.</param>
        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            if (highlightModel)
            {
                highlightModel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Destroy(highlightModel);
            Destroy(stencilOutline);
        }

        private void SetOptions(Dictionary<string, object> options = null)
        {
            var tmpThickness = GetOption<float>(options, "thickness");
            if (tmpThickness > 0f)
            {
                thickness = tmpThickness;
            }

            var tmpCustomModel = GetOption<GameObject>(options, "customOutlineModel");
            if (tmpCustomModel != null)
            {
                customOutlineModel = tmpCustomModel;
            }

            var tmpCustomModelPath = GetOption<string>(options, "customOutlineModelPath");
            if (tmpCustomModelPath != null)
            {
                customOutlineModelPath = tmpCustomModelPath;
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
            if (customOutlineModel != null)
            {
                customOutlineModel = (customOutlineModel.GetComponent<Renderer>() ? customOutlineModel : customOutlineModel.GetComponentInChildren<Renderer>().gameObject);
            }
            else if (customOutlineModelPath != "")
            {
                var getChildModel = transform.FindChild(customOutlineModelPath);
                customOutlineModel = (getChildModel ? getChildModel.gameObject : null);
            }

            GameObject copyModel = customOutlineModel;
            if (copyModel == null)
            {
                copyModel = (GetComponent<Renderer>() ? gameObject : GetComponentInChildren<Renderer>().gameObject);
            }

            if (copyModel == null)
            {
                Debug.LogError("No Renderer has been found on the model to add highlighting to");
                return;
            }

            highlightModel = new GameObject(name + "_HighlightModel");
            highlightModel.transform.position = copyModel.transform.position;
            highlightModel.transform.rotation = copyModel.transform.rotation;
            highlightModel.transform.localScale = copyModel.transform.localScale;
            highlightModel.transform.SetParent(transform);

            foreach (var component in copyModel.GetComponents<Component>())
            {
                if (Array.IndexOf(copyComponents, component.GetType().ToString()) >= 0)
                {
                    Utilities.CloneComponent(component, highlightModel);
                }
            }

            var copyMesh = copyModel.GetComponent<MeshFilter>();
            highlightModel.GetComponent<MeshFilter>().mesh = copyMesh.mesh;
            highlightModel.GetComponent<Renderer>().material = stencilOutline;
            highlightModel.SetActive(false);

            Utilities.SetPlayerObject(highlightModel, VRTK_PlayerObject.ObjectTypes.Highlighter);
        }
    }
}