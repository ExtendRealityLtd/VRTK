namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PanelMenuSaucerGrid : MonoBehaviour
    {
        public GridLayoutGroup gridLayoutGroup;
        public MeshRenderer changeObject;
        public VRTK_PanelMenuItemController panelMenuController;
        public Color[] colours = new Color[0];

        protected int currentIndex = 0;
        protected readonly Color colorDefault = Color.white;
        protected readonly Color colorSelected = Color.green;
        protected readonly float colorAlpha = 0.25f;
        protected enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        protected virtual void OnEnable()
        {
            if (panelMenuController != null)
            {
                panelMenuController.PanelMenuItemSwipeTop += PanelMenuItemSwipeTop;
                panelMenuController.PanelMenuItemSwipeBottom += PanelMenuItemSwipeBottom;
                panelMenuController.PanelMenuItemSwipeLeft += PanelMenuItemSwipeLeft;
                panelMenuController.PanelMenuItemSwipeRight += PanelMenuItemSwipeRight;
                panelMenuController.PanelMenuItemTriggerPressed += PanelMenuItemTriggerPressed;
            }

            SetGridLayoutItemSelectedState(currentIndex);
        }

        protected virtual void PanelMenuItemTriggerPressed(object sender, PanelMenuItemControllerEventArgs e)
        {
            if (currentIndex < colours.Length && changeObject != null)
            {
                changeObject.material.color = colours[currentIndex];
            }
        }

        protected virtual void PanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Right);
        }

        protected virtual void PanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Left);
        }

        protected virtual void PanelMenuItemSwipeBottom(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Down);
        }

        protected virtual void PanelMenuItemSwipeTop(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Up);
        }

        protected virtual void SetGridLayoutItemSelectedState(int index)
        {
            foreach (Transform childTransform in gridLayoutGroup.transform)
            {
                GameObject child = childTransform.gameObject;
                if (child != null)
                {
                    Color color = colorDefault;
                    color.a = colorAlpha;
                    child.GetComponent<Image>().color = color;
                }
            }

            Transform selected = gridLayoutGroup.transform.GetChild(index);
            if (selected != null)
            {
                Color color = colorSelected;
                color.a = colorAlpha;
                selected.GetComponent<Image>().color = color;
            }
        }

        protected virtual bool MoveSelectGridLayoutItem(Direction direction)
        {
            int newIndex = FindNextItemBasedOnMoveDirection(direction);
            if (newIndex != currentIndex)
            {
                SetGridLayoutItemSelectedState(newIndex);
                currentIndex = newIndex;
            }
            return true;
        }

        protected virtual int FindNextItemBasedOnMoveDirection(Direction direction)
        {
            float width = gridLayoutGroup.preferredWidth;
            float cellWidth = gridLayoutGroup.cellSize.x;
            float spacing = gridLayoutGroup.spacing.x;
            int cellsAccross = (int)Mathf.Floor(width / (cellWidth + (spacing / 2))); // quick / dirty
            int childCount = gridLayoutGroup.transform.childCount;

            switch (direction)
            {
                case Direction.Up:
                    int nextUp = currentIndex - cellsAccross;
                    return (nextUp >= 0) ? nextUp : currentIndex;
                case Direction.Down:
                    int nextDown = currentIndex + cellsAccross;
                    return (nextDown < childCount) ? nextDown : currentIndex;
                case Direction.Left:
                    int nextLeft = currentIndex - 1;
                    return (nextLeft >= 0) ? nextLeft : currentIndex;
                case Direction.Right:
                    int nextRight = currentIndex + 1;
                    return (nextRight < childCount) ? nextRight : currentIndex;
                default:
                    return currentIndex;
            }
        }
    }
}