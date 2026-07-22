using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.FieldReadOnlyAttributeDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.WrapperPriority)]
#endif
    [CustomPropertyDrawer(typeof(FieldReadOnlyAttribute), true)]
    [CustomPropertyDrawer(typeof(FieldDisableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(FieldEnableIfAttribute), true)]
    public partial class FieldReadOnlyAttributeDrawer: SaintsPropertyDrawer
    {
    }
}
