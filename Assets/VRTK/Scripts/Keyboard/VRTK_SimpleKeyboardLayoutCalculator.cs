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

            leftArea.rKeyArea.rect = new Rect(
                new Vector2(0, 0),
                new Vector2(containerSize.x / 2f, containerSize.y)
            );
            rightArea.rKeyArea.rect = new Rect(
                new Vector2(containerSize.x / 2f, 0),
                new Vector2(containerSize.x / 2f, containerSize.y)
            );

            // TODO: Implicit space+done row
            BRow[] rows = keyset.Rows();
            foreach (BRow row in rows)
            {
                CalculateAreaRowKeys(leftArea, row, row.LeftKeys(), rows.Length);
                CalculateAreaRowKeys(rightArea, row, row.RightKeys(), rows.Length);
            }
        }

        protected virtual void CalculateAreaRowKeys(BKeyArea area, BRow row, BKey[] keys, int rowCount)
        {
            float keyHeight = area.rKeyArea.rect.size.y / (int)rowCount;
            float keyWidth = area.rKeyArea.rect.size.x / (float)keys.Length;

            foreach (BKey key in keys)
            {
                RKey rKey = key.Create();
                rKey.rect = new Rect(
                    keyWidth * (float)key.localIndex,
                    keyHeight * (float)row.index,
                    keyWidth,
                    keyHeight
                );

                area.AddKey(rKey);
            }
        }
    }
}
