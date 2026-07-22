using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.ButtonDrawers
{
    // this is a workaround... this ensures the decorated attribute's order won't get above these
    // this is a hack of the GetAttributes cache that secretly changed SaintsField attribute to top
    [CustomPropertyDrawer(typeof(DecButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonHideIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonHideIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonHideIfAttribute), true)]

    [CustomPropertyDrawer(typeof(DecButtonDisableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonDisableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonEnableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonDisableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonEnableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonDisableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonEnableIfAttribute), true)]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.WrapperPriority)]
#endif
    public class ConditionWrapperDrawer: SaintsPropertyDrawer
    {

    }
}
