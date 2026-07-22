using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace SaintsField.Playa
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class PlayaHideIfAttribute: HideIfAttribute
    {
        public PlayaHideIfAttribute(params object[] orCallbacks): base(orCallbacks)
        {
        }
    }
}
