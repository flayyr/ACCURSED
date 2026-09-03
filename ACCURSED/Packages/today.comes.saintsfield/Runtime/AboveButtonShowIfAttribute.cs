using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class AboveButtonShowIfAttribute: DecButtonShowIfAttribute
    {
        public AboveButtonShowIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public AboveButtonShowIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
