// Simple Keyboard Layout Calculator|Keyboard|81041
namespace VRTK
{
    using UnityEngine;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// A keyboard layout calculator that lays out keys in evenly spaced rows
    /// </summary>
    /// <remarks>
    /// The simple keyboard layout calculator lays out keys row by row with such that
    /// keys are evenly sized within their row. If you use the same row count/weight in
    /// each row then the simple keyboard layout will lay out keys in an evenly spaced grid.
    /// 
    /// Special key weights can be applied to make some keys span multiple columns of the grid.
    /// </remarks>
    public class VRTK_SimpleKeyboardLayoutCalculator : VRTK_BaseKeyboardLayoutCalculator
    {
        [Tooltip("Size of the space between the two halves of the keyboard")]
        public float areaSpacing = 0f;
        [Tooltip("Size of the vertical space in between rows")]
        public float rowSpacing = 0f;
        [Tooltip("Side of the horizontal space in between keys")]
        public float keySpacing = 0f;

        protected override void CalculateKeyset(BKeyset keyset, Vector2 containerSize)
        {
            BKeyArea leftArea = keyset.KeyArea("Left");
            BKeyArea rightArea = keyset.KeyArea("Right");

            float areaWidth = containerSize.x / 2f - areaSpacing / 2f;
            leftArea.rKeyArea.rect = new Rect(
                new Vector2(0, 0),
                new Vector2(areaWidth, containerSize.y)
            );
            rightArea.rKeyArea.rect = new Rect(
                new Vector2(areaWidth + areaSpacing, 0),
                new Vector2(areaWidth, containerSize.y)
            );

            // TODO: Implicit space+done row
            BRow[] rows = keyset.Rows();
            float perRowSpacing = (rowSpacing * (float)(rows.Length - 1)) / (float)rows.Length;
            float rowHeight = containerSize.y / (float)rows.Length - perRowSpacing;
            float y = 0;
            foreach (BRow row in rows)
            {
                CalculateAreaRowKeys(
                    area: leftArea,
                    row: row,
                    keys: row.LeftKeys(),
                    y: y,
                    keyHeight: rowHeight
                );
                CalculateAreaRowKeys(
                    area: rightArea,
                    row: row,
                    keys: row.RightKeys(),
                    y: y,
                    keyHeight: rowHeight
                );

                y += rowHeight + rowSpacing;
            }
        }

        protected virtual void CalculateAreaRowKeys(BKeyArea area, BRow row, BKey[] keys, float y, float keyHeight)
        {
            // FUTURE: Add a BRrow.rowWeight getter that does this
            float rowWeight = 0;
            foreach (BKey key in keys)
            {
                rowWeight += Mathf.Max(key.key.weight, 1);
            }

            float perKeySpacing = (keySpacing * (float)(rowWeight - 1)) / (float)rowWeight;
            float keyWidth = (area.rKeyArea.rect.size.x / (float)rowWeight) - perKeySpacing;

            float x = 0;
            foreach (BKey key in keys)
            {
                float keyWeight = (float)Mathf.Max(key.key.weight, 1);
                float width = keyWidth * keyWeight + keySpacing * (keyWeight - 1f);

                RKey rKey = key.Create();
                rKey.rect = new Rect(
                    x,
                    y,
                    width,
                    keyHeight
                );

                area.AddKey(rKey);

                x += keyWidth + keySpacing;
            }
        }
    }
}
