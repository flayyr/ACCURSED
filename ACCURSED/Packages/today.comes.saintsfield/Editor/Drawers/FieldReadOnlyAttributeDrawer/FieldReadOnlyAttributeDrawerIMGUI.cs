using System.Collections.Generic;
using System.Reflection;
using SaintsField.Editor.Utils;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.FieldReadOnlyAttributeDrawer
{
    public partial class FieldReadOnlyAttributeDrawer
    {
        private sealed class InfoIMGUI
        {
            public string Error = "";
        }

        private static readonly Dictionary<string, InfoIMGUI> InfoCacheIMGUI = new Dictionary<string, InfoIMGUI>();

        private static InfoIMGUI EnsureKey(SerializedProperty property)
        {
            string key = SerializedUtils.GetUniqueId(property);
            if (InfoCacheIMGUI.TryGetValue(key, out InfoIMGUI infoCache))
            {
                return infoCache;
            }

            InfoCacheIMGUI[key] = infoCache = new InfoIMGUI();
            NoLongerInspectingWatch(property.serializedObject.targetObject, key, () => InfoCacheIMGUI.Remove(key));
            return infoCache;
        }

        protected override bool WillDrawAbove(SerializedProperty property, IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute,
            FieldInfo info,
            object parent)
        {
            return true;
        }

        protected override Rect DrawAboveImGui(Rect position, SerializedProperty property,
            GUIContent label, IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute, int index, FieldInfo info,
            object parent)
        {
            (string error, bool disabled) = IsDisabledIMGUI(property, info, parent);
            EnsureKey(property).Error = error;
            EditorGUI.BeginDisabledGroup(disabled);
            return position;
        }

        protected override void OnPropertyEndImGui(Rect labelFieldRect, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute,
            int saintsIndex, FieldInfo info, object parent)
        {
            EditorGUI.EndDisabledGroup();
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute,
            int index,
            FieldInfo info,
            object parent)
        {
            return EnsureKey(property).Error != "";
            // return true;
        }

        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, int index, IReadOnlyList<PropertyAttribute> allAttributes,
            FieldInfo info, object parent)
        {
            // EditorGUI.EndDisabledGroup();

            string error = EnsureKey(property).Error;
            if (error == "")
            {
                return position;
            }

            (Rect errorRect, Rect leftRect) = RectUtils.SplitHeightRect(position,
                ImGuiHelpBox.GetHeight(error, position.width, MessageType.Error));
            ImGuiHelpBox.Draw(errorRect, error, MessageType.Error);
            return leftRect;
        }

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label,
            float width,
            IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute, int index, FieldInfo info, object parent)
        {
            // Debug.Log("check extra height!");
            string error = EnsureKey(property).Error;
            if (error == "")
            {
                return 0;
            }

            // Debug.Log(HelpBox.GetHeight(_error));
            return ImGuiHelpBox.GetHeight(error, width, MessageType.Error);
        }

        private static (string error, bool disabled) IsDisabledIMGUI(SerializedProperty property, FieldInfo info, object parent)
        {
            FieldReadOnlyAttribute[] targetAttributes =
                SerializedUtils.GetAttributesAndDirectParent<FieldReadOnlyAttribute>(property).attributes;
            return FieldReadOnlyUtils.IsDisabled(
                targetAttributes,
                property,
                info,
                parent
            );
        }
    }
}
