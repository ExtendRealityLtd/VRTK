// Grid Keyboard Layout Calculator|Keyboard|81041
namespace VRTK
{
    using UnityEngine;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using KLKeyset = VRTK_KeyboardLayout.Keyset;
    using KLRow = VRTK_KeyboardLayout.Row;
    using KLKey = VRTK_KeyboardLayout.Key;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;
    using RKeyArea = VRTK_RenderableKeyLayout.KeyArea;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// A keyboard layout calculator that lays out keys in a plain grid pattern
    /// </summary>
    /// <remarks>
    /// The grid keyboard layout calculator lays out keys in a plain evenly spaced grid pattern.
    /// 
    /// Special key weights can be applied to make some keys span multiple columns of the grid.
    /// </remarks>
    public class VRTK_GridKeyboardLayoutCalculator : VRTK_BaseKeyboardLayoutCalculator
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
