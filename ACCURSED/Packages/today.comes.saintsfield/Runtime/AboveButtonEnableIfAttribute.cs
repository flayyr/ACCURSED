using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class AboveButtonEnableIfAttribute: AboveButtonDisableIfAttribute
    {
        public override bool IsDisable => false;

        public AboveButtonEnableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public AboveButtonEnableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
