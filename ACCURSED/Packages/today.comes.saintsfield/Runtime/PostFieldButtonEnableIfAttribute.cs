using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class PostFieldButtonEnableIfAttribute: PostFieldButtonDisableIfAttribute
    {
        public override bool IsDisable => false;

        public PostFieldButtonEnableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public PostFieldButtonEnableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
