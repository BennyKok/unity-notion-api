using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BennyKok.NotionAPI
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class EditorButtonAttribute : PropertyAttribute
    {
        public readonly string methodName;
        public readonly string label;

        public EditorButtonAttribute(string label, string methodName)
        {
            this.methodName = methodName;
            this.label = label;
        }
    }
}
