// Base Keyboard Layout Calculator|Keyboard|81040
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using KLKeyset = VRTK_KeyboardLayout.Keyset;
    using KLRow = VRTK_KeyboardLayout.Row;
    using KLKey = VRTK_KeyboardLayout.Key;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;
    using RKeyArea = VRTK_RenderableKeyLayout.KeyArea;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// The Base Keyboard Layout Calculator is an abstract class that all keyboard layout calculators inherit
    /// </summary>
    /// <remarks>
    /// A keyboard layout calculator takes a `VRTK_KeyboardLayout` and generates a `VRTK_RenderableKeyLayout`
    /// with all key locations calculated.
    /// 
    /// How keys are laid out is up to the keyboard layout calculator, different styles of keyboard layouts may
    /// be implemented by different keyboard layout calculators.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseKeyboardLayoutCalculator : VRTK_BaseKeyLayoutCalculator
    {
        [Tooltip("Keyboard layout to render")]
        public VRTK_KeyboardLayout keyboardLayout;

        /// <summary>
        /// A high-level builder for renderable keyboard layouts
        /// </summary>
        protected class RKeyboardBuilder
        {
            private KeyboardLayout keyboardLayout;
            private RKeyLayout rKeyboard;
            private List<RKeyArea> areas = new List<RKeyArea>();

            public RKeyboardBuilder(KeyboardLayout keyboardLayout)
            {
                this.keyboardLayout = keyboardLayout;
                this.rKeyboard = new RKeyLayout();
            }

            /// <summary>
            /// Iterate over keysets automatically building the metadata for renderable keysets from keyboard layout keysets
            /// </summary>
            /// <returns>An iterator over keyset builders</returns>
            public IEnumerable<BKeyset> Keysets()
            {
                rKeyboard.keysets = new RKeyset[keyboardLayout.keysets.Length];
                for (int s = 0; s < keyboardLayout.keysets.Length; s++)
                {
                    KLKeyset keyset = keyboardLayout.keysets[s];
                    RKeyset rKeyset = new RKeyset();
                    rKeyset.name = keyset.name;

                    BKeyset bKeyset = new BKeyset(s, keyset, rKeyset);
                    yield return bKeyset;
                    bKeyset.Commit();

                    rKeyboard.keysets[s] = rKeyset;
                }
            }

            /// <summary>
            /// Return a renderable keyboard layout once building is finalized
            /// </summary>
            /// <returns></returns>
            public RKeyLayout Commit()
            {
                return rKeyboard;
            }
        }

        /// <summary>
        /// A high-level builder for renderable keysets
        /// </summary>
        protected class BKeyset
        {
            /// <summary>
            /// The keyset index
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The original KeyboardLayout.Keyset
            /// </summary>
            public readonly KLKeyset keyset;
            /// <summary>
            /// The RenderableKeyLayout.Keyset being built
            /// </summary>
            public readonly RKeyset rKeyset;

            private List<RKeyArea> rAreas = new List<RKeyArea>();
            private List<BKeyArea> bAreas = new List<BKeyArea>();

            public BKeyset(int index, KLKeyset keyset, RKeyset rKeyset)
            {
                this.index = index;
                this.keyset = keyset;
                this.rKeyset = rKeyset;
            }

            /// <summary>
            /// Create a RenderableKeyLayout.KeyArea in this keyset
            /// </summary>
            /// <param name="name">Name to use for this key area</param>
            /// <returns>High-level builder for this key area</returns>
            public BKeyArea KeyArea(string name)
            {
                RKeyArea rKeyArea = new RKeyArea();
                rKeyArea.name = name;
                rAreas.Add(rKeyArea);

                BKeyArea bKeyArea = new BKeyArea(rKeyArea);
                bAreas.Add(bKeyArea);

                return bKeyArea;
            }

            /// <summary>
            /// Return an array of high-level builders for the rows in this keyset
            /// </summary>
            /// <returns>High-level row builders</returns>
            public BRow[] Rows()
            {
                BRow[] rows = new BRow[keyset.rows.Length];
                for (int r = 0; r < keyset.rows.Length; r++)
                {
                    rows[r] = new BRow(r, keyset.rows[r]);
                }
                return rows;
            }

            /// <summary>
            /// Commit all key areas to their renderable key layout objects
            /// </summary>
            /// <remarks>
            /// This method is implicitly called in `CalculateKeyLayout` if you override the `CalculateKeyset`.
            /// </remarks>
            public void Commit()
            {
                foreach (BKeyArea bArea in bAreas)
                {
                    bArea.Commit();
                }

                this.rKeyset.areas = rAreas.ToArray();
            }
        }

        /// <summary>
        /// A high-level builder for renderable key areas
        /// </summary>
        protected class BKeyArea
        {
            /// <summary>
            /// The RenderableKeyLayout.KeyArea being built
            /// </summary>
            public readonly RKeyArea rKeyArea;

            private List<RKey> keys = new List<RKey>();

            public BKeyArea(RKeyArea rKeyArea)
            {
                this.rKeyArea = rKeyArea;
            }

            /// <summary>
            /// Add a key to this key area
            /// </summary>
            /// <param name="key">The renderable key to add</param>
            public void AddKey(RKey key)
            {
                keys.Add(key);
            }

            /// <summary>
            /// Commit all keys to the renderable key area
            /// </summary>
            /// <remarks>This is implicitly called by `BKeyset.Commit`</remarks>
            public void Commit()
            {
                this.rKeyArea.keys = keys.ToArray();
            }
        }
        
        /// <summary>
        /// A high-level builder meta class exposing KeyboardLayout.Row instances
        /// </summary>
        protected class BRow
        {
            /// <summary>
            /// The row index
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The original KeyboardLayout.Row
            /// </summary>
            public readonly KLRow row;

            public BRow(int index, KLRow row)
            {
                this.index = index;
                this.row = row;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys in this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] Keys()
            {
                BKey[] keys = new BKey[row.keys.Length];
                for (int k = 0; k < row.keys.Length; k++)
                {
                    keys[k] = new BKey(k, k, row.keys[k]);
                }
                return keys;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys on the left side of this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] LeftKeys()
            {
                BKey[] keys = new BKey[row.splitIndex];
                for (int k = 0; k < row.splitIndex; k++)
                {
                    keys[k] = new BKey(k, k, row.keys[k]);
                }
                return keys;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys on the right side of this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] RightKeys()
            {
                BKey[] keys = new BKey[row.keys.Length - row.splitIndex];
                for (int k = row.splitIndex; k < row.keys.Length; k++)
                {
                    keys[k - row.splitIndex] = new BKey(k, k - row.splitIndex, row.keys[k]);
                }
                return keys;
            }
        }

        /// <summary>
        /// A high-level builder meta class exposing KeyboardLayout.Key instances
        /// </summary>
        protected class BKey
        {
            /// <summary>
            /// The key index within the row
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The key index within the local half of the row
            /// </summary>
            public readonly int localIndex;
            /// <summary>
            /// The original KeyboardLayout.Key
            /// </summary>
            public readonly KLKey key;

            public BKey(int index, int localIndex, KLKey key)
            {
                this.index = index;
                this.localIndex = localIndex;
                this.key = key;
            }

            /// <summary>
            /// Create a RenderableKeyLayout.Key based off this key's metadata
            /// </summary>
            /// <returns>The renderable key</returns>
            public RKey Create()
            {
                RKey rKey = new RKey();

                return rKey;
            }
        }

        /// <summary>
        /// Return a Renderable Keyboard Layout builder for the keyboard layout attached to this component
        /// </summary>
        /// <returns></returns>
        protected RKeyboardBuilder BuildKeyboard()
        {
            return new RKeyboardBuilder(keyboardLayout);
        }

        /// <summary>
        /// Base Keyboard Layout Calculator implements this to provide the higher-level `CalculateKeyset` builder interface
        /// </summary>
        /// <param name="containerSize">The dimensions of the container to render into</param>
        /// <returns>The generated renderable key layout</returns>
        public override RKeyLayout CalculateKeyLayout(Vector2 containerSize)
        {
            if (keyboardLayout == null)
            {
                Debug.LogError("Keyboard Layout Calculator requires a Keyboard Layout");
                return null;
            }

            RKeyboardBuilder b = BuildKeyboard();

            foreach (BKeyset keyset in b.Keysets())
            {
                CalculateKeyset(keyset, containerSize);
            }

            return b.Commit();
        }

        /// <summary>
        /// Keyboard Layout Calculators can override CalculateKeyset to build a keyboard layout using a high level builder
        /// </summary>
        /// <param name="keyset">Keyboard layout keyset builder</param>
        /// <param name="containerSize">Dimensions of the container</param>
        protected virtual void CalculateKeyset(BKeyset keyset, Vector2 containerSize)
        {
            throw new System.NotImplementedException(typeof(VRTK_BaseKeyboardLayoutCalculator) + " subclasses must override either the CalculateKeyLayout or CalculateKeyset method");
        }
    }
}
