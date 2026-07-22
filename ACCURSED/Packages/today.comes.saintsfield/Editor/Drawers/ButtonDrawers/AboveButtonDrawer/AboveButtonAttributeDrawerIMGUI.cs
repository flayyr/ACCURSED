using System.Collections.Generic;
using System.Reflection;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.ButtonDrawers.AboveButtonDrawer
{
    public partial class AboveButtonAttributeDrawer
    {
        protected override float GetAboveExtraHeight(SerializedProperty property, GUIContent label,
            float width, IReadOnlyList<PropertyAttribute> allAttributes,
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


        protected override bool WillDrawAbove(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute,
            FieldInfo info,
            object parent)
        {
            return GetConditionsIMGUI(property, saintsAttribute, allAttributes, info, parent).show;
        }

        protected override Rect DrawAboveImGui(Rect position, SerializedProperty property, GUIContent label,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute, int index,
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
