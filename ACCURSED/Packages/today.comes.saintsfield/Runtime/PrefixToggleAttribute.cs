using System;
using System.Diagnostics;
using SaintsField.Interfaces;
using SaintsField.Utils;
using UnityEngine;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field |  AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class PrefixToggleAttribute: PropertyAttribute, ISaintsAttribute
    {
        public SaintsAttributeType AttributeType => SaintsAttributeType.Other;
        public string GroupBy => "";

        public readonly string TargetName;
        public readonly string ShowIf;

        public PrefixToggleAttribute(string name, string showIf="")
        {
            TargetName = RuntimeUtil.ParseCallback(name).content;
            ShowIf = showIf == null
                ? ""
                : RuntimeUtil.ParseCallback(showIf).content;
        }
    }
}
