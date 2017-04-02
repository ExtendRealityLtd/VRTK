namespace VRTK.Examples.PanelMenu
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Demo GridLayoutGroup component that subscribes to PanelMenuItemController events.
    /// </summary>
    /// <example>
    /// See the demo scene for a complete example: [ 040_Controls_Panel_Menu ] 
    /// </example>
    public class PanelMenuUIGrid : MonoBehaviour
    {
        #region Variables

        public enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        private readonly Color colorDefault = Color.white;
        private readonly Color colorSelected = Color.green;
        private readonly float colorAlpha = 0.25f;

        private GridLayoutGroup gridLayoutGroup;
        private int selectedIndex = 0;

        #endregion Variables

        #region Unity Methods

        private void Start()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuUIGrid", "GridLayoutGroup", "the same"));
                return;
            }

            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeTop += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeTop);
            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeBottom += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeBottom);
            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeLeft += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeLeft);
            GetComponentInParent<PanelMenuItemController>().PanelMenuItemSwipeRight += new PanelMenuItemControllerEventHandler(OnPanelMenuItemSwipeRight);
            GetComponentInParent<PanelMenuItemController>().PanelMenuItemTriggerPressed += new PanelMenuItemControllerEventHandler(OnPanelMenuItemTriggerPressed);

            SetGridLayoutItemSelectedState(selectedIndex);
        }

        #endregion Unity Methods

        #region Interation

        public bool MoveSelectGridLayoutItem(Direction direction, GameObject interactableObject)
        {
            int newIndex = FindNextItemBasedOnMoveDirection(direction);
            if (newIndex != selectedIndex)
            {
                SetGridLayoutItemSelectedState(newIndex);
                selectedIndex = newIndex;
            }
            return true;
        }

        private int FindNextItemBasedOnMoveDirection(Direction direction)
        {
            float width = gridLayoutGroup.preferredWidth;
            float cellWidth = gridLayoutGroup.cellSize.x;
            float spacing = gridLayoutGroup.spacing.x;
            int cellsAccross = (int)Mathf.Floor(width / (cellWidth + (spacing / 2))); // quick / dirty
            int childCount = gridLayoutGroup.transform.childCount;

            switch (direction)
            {
                case Direction.Up:
                    int nextUp = selectedIndex - cellsAccross;
                    return (nextUp >= 0) ? nextUp : selectedIndex;
                case Direction.Down:
                    int nextDown = selectedIndex + cellsAccross;
                    return (nextDown < childCount) ? nextDown : selectedIndex;
                case Direction.Left:
                    int nextLeft = selectedIndex - 1;
                    return (nextLeft >= 0) ? nextLeft : selectedIndex;
                case Direction.Right:
                    int nextRight = selectedIndex + 1;
                    return (nextRight < childCount) ? nextRight : selectedIndex;
                default:
                    return selectedIndex;
            }
        }

        private void SetGridLayoutItemSelectedState(int index)
        {
            foreach (Transform childTransform in gridLayoutGroup.transform)
            {
                var child = childTransform.gameObject;
                if (child != null)
                {
                    Color color = colorDefault;
                    color.a = colorAlpha;
                    child.GetComponent<Image>().color = color;
                }
            }

            var selected = gridLayoutGroup.transform.GetChild(index);
            if (selected != null)
            {
                Color color = colorSelected;
                color.a = colorAlpha;
                selected.GetComponent<Image>().color = color;
            }
        }

        #endregion Interaction

        #region UI Events

        private void OnPanelMenuItemSwipeTop(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Up, e.interactableObject);
        }

        private void OnPanelMenuItemSwipeBottom(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Down, e.interactableObject);
        }

        private void OnPanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Left, e.interactableObject);
        }

        private void OnPanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Right, e.interactableObject);
        }

        private void OnPanelMenuItemTriggerPressed(object sender, PanelMenuItemControllerEventArgs e)
        {
            SendMessageToInteractableObject(e.interactableObject);
        }

        private void SendMessageToInteractableObject(GameObject interactableObject)
        {
            interactableObject.SendMessage("UpdateGridLayoutValue", selectedIndex);
        }

        #endregion UI Events
    }
}