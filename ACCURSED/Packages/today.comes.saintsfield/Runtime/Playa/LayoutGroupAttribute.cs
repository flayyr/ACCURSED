using System;
using System.Diagnostics;

namespace SaintsField.Playa
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class LayoutGroupAttribute: LayoutStartAttribute
    {
        public LayoutGroupAttribute(string layoutBy, ELayout layout=0, float marginTop = -1f, float marginBottom = -1f,
            float paddingLeft = 0, float paddingRight = 0):
            base(layoutBy, layout, marginTop, marginBottom, paddingLeft, paddingRight)
        {
        }
    }
}
