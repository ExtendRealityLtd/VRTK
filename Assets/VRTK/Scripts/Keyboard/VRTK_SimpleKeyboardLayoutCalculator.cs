// Simple Keyboard Layout Calculator|Keyboard|81042
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// A keyboard layout calculator that lays out keys in evenly spaced rows on a split keyboard
    /// </summary>
    /// <remarks>
    /// The simple keyboard layout calculator lays out keys row by row with such that
    /// keys are evenly sized within their row. If you use the same row count/weight in
    /// each row then the simple keyboard layout will lay out keys in an evenly spaced grid.
    /// 
    /// This layout renders as a split keyboard with separate left and right key areas.
    /// 
    /// Special key weights can be applied to make some keys span multiple columns of the grid.
    /// </remarks>
    [VRTK_KeyLayoutCalculator(name = "Simple Keyboard Layout", requireSource = true, helpList = new string[]
    {
        "Uses a KeyboardLayout",
        "Keys are evenly sized",
        "Split (left/right) keyboard"
    })]
    public class VRTK_SimpleKeyboardLayoutCalculator : VRTK_SimpleKeypadLayoutCalculator
    {
        [Tooltip("Size of the space between the two halves of the keyboard")]
        public float areaSpacing = 0f;

        protected override void CalculateKeyset(BKeyset keyset, Vector2[] containerSizes)
        {
            BKeyArea leftArea = keyset.KeyArea("Left");
            BKeyArea rightArea = keyset.KeyArea("Right");

            if (containerSizes.Length > 1)
            {
                if (containerSizes.Length > 2)
                {
                    Debug.LogWarning(typeof(VRTK_SimpleKeyboardLayoutCalculator) + " expects a maximum of 2 containers, but " + name + " has " + containerSizes.Length);
                }

                // For two containers, ignore the area spacing and render each area in its own container
                leftArea.container = 0;
                leftArea.rect = new Rect(Vector2.zero, containerSizes[0]);
                rightArea.container = 1;
                rightArea.rect = new Rect(Vector2.zero, containerSizes[1]);
            }
            else
            {
                // For a single container, split the container in half between the two areas
                Vector2 containerSize = containerSizes[0];
                float areaWidth = containerSize.x / 2f - areaSpacing / 2f;
                leftArea.container = 0;
                leftArea.rect = new Rect(
                    Vector2.zero,
                    new Vector2(areaWidth, containerSize.y)
                );
                rightArea.container = 0;
                rightArea.rect = new Rect(
                    new Vector2(areaWidth + areaSpacing, 0),
                    new Vector2(areaWidth, containerSize.y)
                );
            }

            // TODO: Implicit space+done row
            BRow[] rows = keyset.Rows();
            float perRowSpacing = (rowSpacing * (float)(rows.Length - 1)) / (float)rows.Length;
            float leftRowHeight = leftArea.rect.size.y / (float)rows.Length - perRowSpacing;
            float rightRowHeight = rightArea.rect.size.y / (float)rows.Length - perRowSpacing;
            float leftY = 0;
            float rightY = 0;
            foreach (BRow row in rows)
            {
                CalculateAreaRowKeys(
                    area: leftArea,
                    row: row,
                    keys: row.LeftKeys(),
                    y: leftY,
                    keyHeight: leftRowHeight
                );
                CalculateAreaRowKeys(
                    area: rightArea,
                    row: row,
                    keys: row.RightKeys(),
                    y: rightY,
                    keyHeight: rightRowHeight
                );

                leftY += leftRowHeight + rowSpacing;
                rightY += rightRowHeight + rowSpacing;
            }
        }
    }
}
