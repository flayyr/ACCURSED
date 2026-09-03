using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class BelowButtonShowIfAttribute: DecButtonShowIfAttribute
    {
        public BelowButtonShowIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public BelowButtonShowIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
