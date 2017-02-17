namespace VRTK
{
    using UnityEngine;
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VRTK_KeyLayoutCalculatorAttribute : Attribute
    {
        public VRTK_KeyLayoutCalculatorAttribute() {
            
        }

        public string name { get; set; }
        public string help { get; set; }
        public string[] helpList { get; set; }
    }
}
