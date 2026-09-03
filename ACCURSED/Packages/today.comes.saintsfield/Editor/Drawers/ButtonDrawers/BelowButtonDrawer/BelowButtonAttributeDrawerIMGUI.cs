using System.Collections.Generic;
using System.Reflection;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.ButtonDrawers.BelowButtonDrawer
{
    public partial class BelowButtonAttributeDrawer
    {
        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label,
            float width,
            IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute, int index, FieldInfo info, object parent)
        {
            bool show = GetConditionsIMGUI(property, saintsAttribute, allAttributes, info, parent).show;
            if (!show)
            {
                return 0f;
            }

            UpdateButtonLabelIMGUI(property, saintsAttribute, index, info, parent);
            return GetButtonHeightIMGUI() + GetResultHeightIMGUI(property, index, width);
        }


        protected override bool WillDrawBelow(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute,
            int index,
            FieldInfo info,
            object parent)
        {
            return GetConditionsIMGUI(property, saintsAttribute, allAttributes, info, parent).show;
        }

        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, int index, IReadOnlyList<PropertyAttribute> allAttributes,
            FieldInfo info, object parent)
        {
            (bool show, bool disable) = GetConditionsIMGUI(property, saintsAttribute, allAttributes, info, parent);
            if (!show)
            {
                return position;
            }

            using (new EditorGUI.DisabledScope(disable))
            {
                Rect leftRect = Draw(position, property, label, saintsAttribute, index, info, parent);
                return DrawResultIMGUI(leftRect, property, index);
            }
        }

    }
}
