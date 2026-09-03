using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class AboveButtonHideIfAttribute: AboveButtonShowIfAttribute
    {
        public override bool IsShow => false;

        public AboveButtonHideIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public AboveButtonHideIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
