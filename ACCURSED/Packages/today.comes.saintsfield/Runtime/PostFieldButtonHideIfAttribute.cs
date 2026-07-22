using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class PostFieldButtonHideIfAttribute: PostFieldButtonShowIfAttribute
    {
        public override bool IsShow => false;

        public PostFieldButtonHideIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public PostFieldButtonHideIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
