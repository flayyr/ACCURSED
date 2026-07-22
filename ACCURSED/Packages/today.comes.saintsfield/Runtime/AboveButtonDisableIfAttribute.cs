using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class AboveButtonDisableIfAttribute: DecButtonDisableIfAttribute
    {
        public AboveButtonDisableIfAttribute(EMode eMode, params object[] andCallbacks): base(eMode, andCallbacks)
        {
        }

        public AboveButtonDisableIfAttribute(params object[] andCallbacks): base(andCallbacks)
        {
        }
    }
}
