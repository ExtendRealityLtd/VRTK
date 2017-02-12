namespace VRTK
{
    using UnityEngine;
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VRTK_KeyLayoutSourceAttribute : Attribute
    {
        public VRTK_KeyLayoutSourceAttribute()
        {

        }

        public string name { get; set; }
        public string help { get; set; }
    }
}
