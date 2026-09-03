// #if UNITY_2022_2_OR_NEWER || SAINTSFIELD_UI_TOOLKIT_DISABLE
using System;
using System.Diagnostics;
using SaintsField.Interfaces;
using SaintsField.Playa;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class TableAttribute: Attribute, IPlayaAttribute
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public bool HideAddButton;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public bool HideRemoveButton;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public bool DefaultCollapse;

        public TableAttribute(bool hideAddButton=false, bool hideRemoveButton=false, bool defaultCollapse=false)
        {
            // DefaultExpanded = defaultExpanded;
            HideAddButton = hideAddButton;
            HideRemoveButton = hideRemoveButton;
            DefaultCollapse = defaultCollapse;
        }
    }
}
// #endif
