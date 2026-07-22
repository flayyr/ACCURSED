using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class InputDisableIfAttribute: InputReadOnlyAttribute
    {
        public InputDisableIfAttribute(EMode editorMode, params object[] by) : base(editorMode, by)
        {
        }

        public InputDisableIfAttribute(params object[] by) : base(by)
        {
        }
    }
}
