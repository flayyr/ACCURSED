using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class FieldDisableIfAttribute: FieldReadOnlyAttribute
    {
        public FieldDisableIfAttribute(EMode editorMode, params object[] by) : base(editorMode, by)
        {
        }

        public FieldDisableIfAttribute(params object[] by) : base(by)
        {
        }
    }
}
