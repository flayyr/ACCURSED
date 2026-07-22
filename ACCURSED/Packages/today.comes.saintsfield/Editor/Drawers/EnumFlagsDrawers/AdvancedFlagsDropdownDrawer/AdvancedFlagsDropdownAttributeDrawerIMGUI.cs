using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaintsField.DropdownBase;
using SaintsField.Editor.Drawers.AdvancedDropdownDrawer;
using SaintsField.Editor.Drawers.ExpandableDrawer;
using SaintsField.Editor.Utils;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SaintsField.Editor.Drawers.EnumFlagsDrawers.AdvancedFlagsDropdownDrawer
{
    public partial class AdvancedFlagsDropdownAttributeDrawer
    {
        private sealed class InfoIMGUI
        {
            public string Error = "";
        }

        private static readonly Dictionary<string, InfoIMGUI> InfoCacheIMGUI = new Dictionary<string, InfoIMGUI>();

        private static InfoIMGUI EnsureKey(SerializedProperty property)
        {
            string key = SerializedUtils.GetUniqueId(property);
            if (InfoCacheIMGUI.TryGetValue(key, out InfoIMGUI cache))
            {
                return cache;
            }

            InfoCacheIMGUI[key] = cache = new InfoIMGUI();
            NoLongerInspectingWatch(property.serializedObject.targetObject, key, () => InfoCacheIMGUI.Remove(key));
            return cache;
        }

        private static InfoIMGUI UpdateStatus(SerializedProperty property, FieldInfo info)
        {
            InfoIMGUI cache = EnsureKey(property);
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                cache.Error = $"Type {property.propertyType} is not a enum type";
                return cache;
            }

            EnumFlagsMetaInfo metaInfo = EnumFlagsUtil.GetMetaInfo(property, info);
            AdvancedDropdownMetaInfo dropdownMetaInfo = EnumFlagsUtil.GetDropdownMetaInfo(
                EnumFlagsUtil.GetSerializedPropertyEnumValue(metaInfo.EnumType, property),
                metaInfo.AllCheckedLong,
                metaInfo.BitValueToName);
            cache.Error = dropdownMetaInfo.Error;
            return cache;
        }

        protected override float GetFieldHeight(SerializedProperty property, GUIContent label,
            float width,
            int index,
            ISaintsAttribute saintsAttribute,
            FieldInfo info,
            bool hasLabelWidth, object parent)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void DrawField(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, IReadOnlyList<PropertyAttribute> allAttributes,
            FieldInfo info, object parent)
        {
            InfoIMGUI cache = UpdateStatus(property, info);
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label, GUIContent.none);
                return;
            }

            EnumFlagsMetaInfo metaInfo = EnumFlagsUtil.GetMetaInfo(property, info);
            long currentMask = EnumFlagsUtil.GetSerializedPropertyEnumValue(metaInfo.EnumType, property);
            AdvancedDropdownMetaInfo dropdownMetaInfo = EnumFlagsUtil.GetDropdownMetaInfo(
                currentMask,
                metaInfo.AllCheckedLong,
                metaInfo.BitValueToName);
            cache.Error = dropdownMetaInfo.Error;

            #region Dropdown

            Rect leftRect = EditorGUI.PrefixLabel(position, label);
            Rect labelRect = new Rect(position)
            {
                width = position.width - leftRect.width,
            };
            DrawOverrideRichText(labelRect, label, overrideRichTextChunks);

            GUI.SetNextControlName(FieldControlName);
            string display = GetSelectedNames(metaInfo.BitValueToName, currentMask);
            // Debug.Assert(false, "Here");
            // ReSharper disable once InvertIf
            if (EditorGUI.DropdownButton(leftRect, new GUIContent(display), FocusType.Keyboard))
            {
                Vector2 size = AdvancedDropdownUtil.GetSizeIMGUI(dropdownMetaInfo.DropdownListValue,
                    leftRect.width);
                SaintsAdvancedDropdownIMGUI dropdown = new SaintsAdvancedDropdownIMGUI(
                    dropdownMetaInfo.DropdownListValue,
                    size,
                    leftRect,
                    new AdvancedDropdownState(),
                    curItem =>
                    {
                        long selectedValue = (long)curItem;
                        long originValue = EnumFlagsUtil.GetSerializedPropertyEnumValue(metaInfo.EnumType, property);
                        if (originValue == ~0L)
                        {
                            originValue = metaInfo.AllCheckedLong;
                        }

                        long newMask;
                        if (selectedValue == 0)
                        {
                            newMask = 0;
                        }
                        else
                        {
                            newMask = EnumFlagsUtil.ToggleBit(originValue, selectedValue);
                            if ((newMask & metaInfo.AllCheckedLong) == metaInfo.AllCheckedLong)
                            {
                                newMask = ~0L;
                            }
                        }

                        EnumFlagsUtil.SetSerializedPropertyEnumValue(metaInfo.EnumType, property, newMask);
                        property.serializedObject.ApplyModifiedProperties();
                        TriggerChangedIMGUI(property, newMask);
                        ReflectUtils.SetValue(property.propertyPath, property.serializedObject.targetObject, info, parent,
                            System.Enum.ToObject(metaInfo.EnumType, newMask));
                        if(ExpandableIMGUIScoop.IsInScoop)
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    },
                    icon => Util.LoadResource<Texture2D>(icon));
                dropdown.Show(leftRect);
                dropdown.BindWindowPosition();
            }

            #endregion
        }

        private static int GetValueItemCounts(IDropdown dropdownList)
        {
            if (dropdownList.isSeparator)
            {
                return 0;
            }

            if(dropdownList.ChildCount() == 0)
            {
                return 1;
            }

            return dropdownList.children.Sum(child => GetValueItemCounts(child));
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute,
            int index,
            FieldInfo info,
            object parent) => UpdateStatus(property, info).Error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width,
            IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute, int index, FieldInfo info, object parent)
        {
            string error = EnsureKey(property).Error;
            return error == "" ? 0 : ImGuiHelpBox.GetHeight(error, width, MessageType.Error);
        }

        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, int index, IReadOnlyList<PropertyAttribute> allAttributes,
            FieldInfo info, object parent)
        {
            string error = EnsureKey(property).Error;
            return error == "" ? position : ImGuiHelpBox.Draw(position, error, MessageType.Error);
        }

    }
}
