namespace VRTK.Examples.PanelMenu
{
    using UnityEngine;

    /// <summary>
    /// Demo component for Panel Menu example scene.
    /// </summary>
    /// <example>
    /// See the demo scene for a complete example: [ 040_Controls_Panel_Menu ]
    /// </example>
    public class PanelMenuDemoFlyingSaucer : MonoBehaviour
    {

        private readonly Color[] colors =
        {
        Color.black,
        Color.blue,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.magenta,
        Color.red,
        Color.white
    };

        public void UpdateGridLayoutValue(int selectedIndex)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().materials[0].color = colors[selectedIndex];
        }
    }
}