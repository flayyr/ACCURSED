using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class BelowButtonHideIfAttribute: BelowButtonShowIfAttribute
    {
        public override bool IsShow => false;

        public BelowButtonHideIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public BelowButtonHideIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
