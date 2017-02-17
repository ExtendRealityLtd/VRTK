namespace VRTK
{
    using UnityEngine;
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VRTK_KeyLayoutRendererAttribute : Attribute
    {
        public VRTK_KeyLayoutRendererAttribute()
        {

        }

        public string name { get; set; }
        public string help { get; set; }
    }
}
