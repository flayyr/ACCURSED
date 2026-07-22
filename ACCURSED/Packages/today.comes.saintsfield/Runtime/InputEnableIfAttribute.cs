using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class InputEnableIfAttribute: InputReadOnlyAttribute
    {
        public InputEnableIfAttribute(EMode editorMode, params object[] by) : base(editorMode, by)
        {
        }

        public InputEnableIfAttribute(params object[] by) : base(by)
        {
        }
    }
}
