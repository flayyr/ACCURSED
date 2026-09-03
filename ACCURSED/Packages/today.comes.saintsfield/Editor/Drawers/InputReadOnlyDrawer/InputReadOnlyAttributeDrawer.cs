using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.InputReadOnlyDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.WrapperPriority)]
#endif
    [CustomPropertyDrawer(typeof(InputEnableIfAttribute), true)]
    [CustomPropertyDrawer(typeof(InputReadOnlyAttribute), true)]
    [CustomPropertyDrawer(typeof(InputDisableIfAttribute), true)]
    public partial class InputReadOnlyAttributeDrawer: SaintsPropertyDrawer
    {
    }
}
