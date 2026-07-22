using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class BelowButtonEnableIfAttribute: BelowButtonDisableIfAttribute
    {
        public override bool IsDisable => false;

        public BelowButtonEnableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public BelowButtonEnableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
