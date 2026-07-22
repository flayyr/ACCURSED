using System.Collections.Generic;
using System.Reflection;
using SaintsField.Editor.Utils;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SaintsField.Editor.Drawers.AdvancedDropdownDrawer
{
    public partial class AdvancedDropdownAttributeDrawer
    {
        private class InfoIMGUI
        {
            public string Error = "";
            public readonly IMGUIUtils.IMGUITicker Ticker = new IMGUIUtils.IMGUITicker();
            public AdvancedDropdownMetaInfo MetaInfo;
            public bool Clicked;
        }

        private static readonly Dictionary<string, InfoIMGUI> InfoCacheIMGUI = new Dictionary<string, InfoIMGUI>();

        private readonly Dictionary<string, Texture2D> _iconCache = new Dictionary<string, Texture2D>();

        ~AdvancedDropdownAttributeDrawer()
        {
            _iconCache.Clear();
        }

        private static InfoIMGUI EnsureKey(SerializedProperty property)
        {
            string key = SerializedUtils.GetUniqueId(property);
            if (InfoCacheIMGUI.TryGetValue(key, out InfoIMGUI infoCache))
            {
                return infoCache;
            }

            InfoCacheIMGUI[key] = infoCache = new InfoIMGUI();
            NoLongerInspectingWatch(property.serializedObject.targetObject, key, () =>
            {
                InfoCacheIMGUI.Remove(key);
            });
            return infoCache;
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
            InfoIMGUI cachedInfo = EnsureKey(property);
            AdvancedDropdownAttribute advancedDropdownAttribute = (AdvancedDropdownAttribute)saintsAttribute;
            cachedInfo.Ticker.Tick();
            if (cachedInfo.Ticker.TickWaiterResult.Exception != null)
            {
                cachedInfo.Error = cachedInfo.Ticker.TickWaiterResult.Exception?.InnerException?.Message
                                   ?? cachedInfo.Ticker.TickWaiterResult.Exception?.Message
                                   ?? "";
                cachedInfo.Clicked = false;
            }

            #region Dropdown

            Rect leftRect = EditorGUI.PrefixLabel(position, label);
            Rect labelRect = new Rect(position)
            {
                width = position.width - leftRect.width,
            };
            DrawOverrideRichText(labelRect, label, overrideRichTextChunks);

            AdvancedDropdownMetaInfo metaInfo = cachedInfo.MetaInfo;
            bool hasMetaInfo = metaInfo.SelectStacks != null;

            GUI.SetNextControlName(FieldControlName);
            string display = hasMetaInfo ? GetMetaStackDisplay(metaInfo) : "";
            // Debug.Assert(false, "Here");
            // ReSharper disable once InvertIf
            if (cachedInfo.Ticker.DropdownButton(leftRect, new GUIContent(display), EditorStyles.popup))
            {
                cachedInfo.Error = "";
                cachedInfo.Clicked = true;
                cachedInfo.Ticker.ResetResolved();
            }

            bool isRunning = cachedInfo.Ticker.IsRunning();
            if (!cachedInfo.Ticker.Resolved && !isRunning)
            {
                GetMetaInfoAsync(cachedInfo.Ticker,
                    asyncMetaInfo =>
                    {
                        cachedInfo.MetaInfo = asyncMetaInfo;
                        cachedInfo.Error = asyncMetaInfo.Error;
                        if (asyncMetaInfo.Error != "")
                        {
                            cachedInfo.Clicked = false;
                            return;
                        }

                        if (cachedInfo.Clicked)
                        {
                            cachedInfo.Clicked = false;
                            ShowDropdown(asyncMetaInfo);
                        }
                    },
                    property,
                    advancedDropdownAttribute,
                    info,
                    parent,
                    true);
            }

            #endregion

            return;

            void ShowDropdown(AdvancedDropdownMetaInfo dropdownMetaInfo)
            {
                if (dropdownMetaInfo.DropdownListValue == null)
                {
                    cachedInfo.Clicked = false;
                    return;
                }

                Vector2 size = AdvancedDropdownUtil.GetSizeIMGUI(dropdownMetaInfo.DropdownListValue, position.width);

                SaintsAdvancedDropdownIMGUI dropdown = new SaintsAdvancedDropdownIMGUI(
                    dropdownMetaInfo.DropdownListValue,
                    size,
                    position,
                    new AdvancedDropdownState(),
                    curItem =>
                    {
                        ReflectUtils.SetValue(property.propertyPath, property.serializedObject.targetObject, info, parent, curItem);
                        Util.SignPropertyValue(property, info, parent, curItem);
                        property.serializedObject.ApplyModifiedProperties();
                        TriggerChangedIMGUI(property, curItem);
                        cachedInfo.MetaInfo = default;
                        cachedInfo.Ticker.ResetResolved();
                    },
                    GetIcon);
                cachedInfo.Clicked = false;
                dropdown.Show(position);
                dropdown.BindWindowPosition();
            }
        }

        private Texture2D GetIcon(string icon)
        {
            if (_iconCache.TryGetValue(icon, out Texture2D result))
            {
                return result;
            }

            result = Util.LoadResource<Texture2D>(icon);
            if (result == null)
            {
                return null;
            }
            if (result.width == 1 && result.height == 1)
            {
                return null;
            }
            _iconCache[icon] = result;
            return result;
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute,
            int index,
            FieldInfo info,
            object parent) => EnsureKey(property).Error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width,
            IReadOnlyList<PropertyAttribute> allAttributes,
            ISaintsAttribute saintsAttribute, int index, FieldInfo info, object parent)
        {
            InfoIMGUI cachedInfo = EnsureKey(property);
            return cachedInfo.Error == "" ? 0 : ImGuiHelpBox.GetHeight(cachedInfo.Error, width, MessageType.Error);
        }

        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, int index, IReadOnlyList<PropertyAttribute> allAttributes,
            FieldInfo info, object parent)
        {
            InfoIMGUI cachedInfo = EnsureKey(property);
            return cachedInfo.Error == "" ? position : ImGuiHelpBox.Draw(position, cachedInfo.Error, MessageType.Error);
        }
    }
}
