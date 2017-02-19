namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Helper methods for editors scripts interacting with VRTK custom attributes
    /// </summary>
    public static class VRTK_AttributeUtilities
    {
        /// <summary>
        /// Finds all users of a class attribute
        /// </summary>
        /// <typeparam name="A">The VRTK class attribute</typeparam>
        /// <returns>A dictionary mapping attribute instance to class type</returns>
        public static Dictionary<A, Type> GetAttributeUsage<A>() where A : Attribute
        {
            Dictionary<A, Type> attributes = new Dictionary<A, Type>();

            string definedIn = typeof(A).Assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache)
                {
                    continue;
                }

                if (assembly.GetName().Name != definedIn)
                {
                    bool isReferenced = false;
                    foreach (AssemblyName refAssembly in assembly.GetReferencedAssemblies())
                    {
                        if (refAssembly.Name == definedIn)
                        {
                            isReferenced = true;
                            break;
                        }
                    }

                    if (!isReferenced)
                    {
                        continue;
                    }
                }

                foreach (Type type in assembly.GetTypes())
                {
                    Attribute attribute = GetAttribute<A>(type);
                    if (attribute != null)
                    {
                        attributes.Add((A)attribute, type);
                    }
                }
            }

            return attributes;
        }

        /// <summary>
        /// Returns the Attribute of Type A for the class of type, null if not defined
        /// </summary>
        /// <typeparam name="A">The Attribute type</typeparam>
        /// <param name="type">The class Type to return attributes from</param>
        /// <returns>The Attribute of type A</returns>
        public static A GetAttribute<A>(Type type) where A : Attribute
        {
            Attribute attribute = null;
            foreach (Attribute a in type.GetCustomAttributes(typeof(A), true))
            {
                attribute = a;
                break;
            }
            return (A)attribute;
        }
    }
}
