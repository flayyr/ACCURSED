using System;
using System.Reflection;
using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.PrefixToggleDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.WrapperPriority)]
#endif
    [CustomPropertyDrawer(typeof(PrefixToggleAttribute), true)]
    public partial class PrefixToggleAttributeDrawer: SaintsPropertyDrawer
    {
        public readonly struct Payload
        {
            public readonly object Parent;

            public readonly bool IsField;
            public readonly FieldInfo FieldInfo;
            public readonly PropertyInfo PropertyInfo;

            public Payload(MemberInfo memberInfo, object parent)
            {
                Parent = parent;

                switch (memberInfo)
                {
                    case FieldInfo fi:
                        IsField = true;
                        FieldInfo = fi;
                        PropertyInfo = null;
                        break;
                    case PropertyInfo pi:
                        IsField = false;
                        FieldInfo = null;
                        PropertyInfo = pi;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(memberInfo), memberInfo, $"{memberInfo.MemberType}");
                }
            }

            public void SetValue(bool on)
            {
                if (IsField)
                {
                    FieldInfo.SetValue(Parent, on);
                }
                else
                {
                    PropertyInfo.SetValue(Parent, on);
                }
            }
        }
    }
}
