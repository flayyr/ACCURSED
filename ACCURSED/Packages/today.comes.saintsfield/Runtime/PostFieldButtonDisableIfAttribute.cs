using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class PostFieldButtonDisableIfAttribute: DecButtonDisableIfAttribute
    {
        public PostFieldButtonDisableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public PostFieldButtonDisableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
