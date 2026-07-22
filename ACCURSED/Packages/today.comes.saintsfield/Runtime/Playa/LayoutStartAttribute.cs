using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace SaintsField.Playa
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    public class LayoutStartAttribute: LayoutAttribute
    {
        public LayoutStartAttribute(string layoutBy, ELayout layout=0, float marginTop = -1f, float marginBottom = -1f,
            float paddingLeft = 0, float paddingRight = 0):
            base(layoutBy, layout, true, marginTop, marginBottom, paddingLeft, paddingRight)
        {
        }
    }
}
