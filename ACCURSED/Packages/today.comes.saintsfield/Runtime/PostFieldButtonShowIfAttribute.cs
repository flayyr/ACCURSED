using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class PostFieldButtonShowIfAttribute: DecButtonShowIfAttribute
    {
        public PostFieldButtonShowIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public PostFieldButtonShowIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
