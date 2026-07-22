using System.Collections.Generic;
using System.Reflection;
using SaintsField.Editor.Utils;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.InputReadOnlyDrawer
{
    public partial class InputReadOnlyAttributeDrawer
    {
        private class InfoIMGUI
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

        protected override float DrawPreLabelImGui(Rect position, SerializedProperty property, ISaintsAttribute saintsAttribute, FieldInfo info,
            object parent)
        {
            (string error, bool disabled) = IsDisabledIMGUI(property, info, parent);
            EnsureKey(property).Error = error;
            EditorGUI.BeginDisabledGroup(disabled);
            return -1;
        }

        protected override bool DrawPostFieldImGui(Rect position, Rect fullRect, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, int index, IReadOnlyList<PropertyAttribute> allAttributes, FieldInfo info, object parent)
        {
            EditorGUI.EndDisabledGroup();
            return true;
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

        private static (string error, bool disabled) IsDisabledIMGUI(SerializedProperty property, FieldInfo info,
            object parent)
        {
            InputReadOnlyAttribute[] targetAttributes =
                SerializedUtils.GetAttributesAndDirectParent<InputReadOnlyAttribute>(property).attributes;
            return InputReadOnlyUtils.IsDisabled(
                targetAttributes,
                property,
                info,
                parent
            );
        }
    }
}
