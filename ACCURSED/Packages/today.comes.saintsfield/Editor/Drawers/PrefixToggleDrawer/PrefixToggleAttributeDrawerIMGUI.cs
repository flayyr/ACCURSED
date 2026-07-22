using System;
using System.Collections.Generic;
using System.Reflection;
using SaintsField.Editor.Utils;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.PrefixToggleDrawer
{
    public partial class PrefixToggleAttributeDrawer
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

        protected override float DrawPreLabelImGui(Rect position, SerializedProperty property, ISaintsAttribute saintsAttribute, FieldInfo info,
            object parent)
        {
            if (SerializedUtils.PropertyPathIndex(property.propertyPath) >= 0)
            {
                return -1f;
            }

            const float toggleWidth = 18f;
            Rect toggleRect = new Rect(position)
            {
                width = toggleWidth,
                height = EditorGUIUtility.singleLineHeight,
            };

            InfoIMGUI cache = EnsureKey(property);
            PrefixToggleAttribute attr = (PrefixToggleAttribute)saintsAttribute;
            string targetStr = attr.TargetName;
            string showIf = attr.ShowIf;

            #region Show If
            if (!string.IsNullOrEmpty(showIf))
            {
                (string showError, MemberInfo _, object showResult) = Util.GetOf<object>(showIf, null, property, info, parent, null);
                cache.Error = showError;
                if (showError != "")
                {
                    return toggleWidth;
                }

                if (!ReflectUtils.Truly(showResult))
                {
                    cache.Error = "";
                    return 0f;
                }
            }
            #endregion

            #region Serialized Toggle
            (string error, SerializedProperty toggleProperty) = PrefixToggleUtils.GetToggleTarget(targetStr, property);
            if (error != "")
            {
                cache.Error = error;
                EditorGUI.Toggle(toggleRect, false);
                return toggleWidth;
            }

            if (toggleProperty != null)
            {
                cache.Error = "";
                using (EditorGUI.ChangeCheckScope changed = new EditorGUI.ChangeCheckScope())
                {
                    bool newValue = EditorGUI.Toggle(toggleRect, toggleProperty.boolValue);
                    if (changed.changed)
                    {
                        toggleProperty.boolValue = newValue;
                        toggleProperty.serializedObject.ApplyModifiedProperties();
                    }
                }

                return toggleWidth;
            }
            #endregion

            #region Non Serialized Toggle
            object p = SerializedUtils.GetFieldInfoAndDirectParent(property).parent;
            (string refError, MemberInfo memberInfo, object result) = Util.GetOf<object>(targetStr, null, property, info, p, null);
            cache.Error = refError;
            if (refError != "")
            {
                EditorGUI.Toggle(toggleRect, false);
                return toggleWidth;
            }

            bool current = ReflectUtils.Truly(result);
            using (EditorGUI.ChangeCheckScope changed = new EditorGUI.ChangeCheckScope())
            {
                bool newValue = EditorGUI.Toggle(toggleRect, current);
                if (changed.changed)
                {
                    try
                    {
                        new Payload(memberInfo, p).SetValue(newValue);
                    }
                    catch (Exception e)
                    {
#if SAINTSFIELD_DEBUG
                        Debug.LogWarning(e);
#endif
                        cache.Error = e.InnerException?.Message ?? e.Message;
                    }
                }
            }
            #endregion

            return toggleWidth;
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute, int index, FieldInfo info,
            object parent) => EnsureKey(property).Error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width,
            IReadOnlyList<PropertyAttribute> allAttributes, ISaintsAttribute saintsAttribute, int index, FieldInfo info,
            object parent)
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
