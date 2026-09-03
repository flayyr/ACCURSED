using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class BelowButtonDisableIfAttribute: DecButtonDisableIfAttribute
    {
        public BelowButtonDisableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public BelowButtonDisableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
