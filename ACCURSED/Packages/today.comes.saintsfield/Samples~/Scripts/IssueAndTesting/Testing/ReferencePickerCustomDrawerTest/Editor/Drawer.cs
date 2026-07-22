using SaintsField.Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest.Editor
{
    [CustomPropertyDrawer(typeof(ReferencePickerCustomDrawer.TypeWithCustomDrawer))]
    public class Drawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty myStruct = property.FindPropertyRelative(nameof(ReferencePickerCustomDrawer.TypeWithCustomDrawer.myStruct));
            float propertyHeight = myStruct == null
                ? EditorGUIUtility.singleLineHeight
                : EditorGUI.GetPropertyHeight(myStruct, true);

            return Mathf.Max(EditorGUIUtility.singleLineHeight * 2, propertyHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty myStruct = property.FindPropertyRelative(nameof(ReferencePickerCustomDrawer.TypeWithCustomDrawer.myStruct));
            Rect contentRect = EditorGUI.PrefixLabel(position, label);

            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            const float spacing = 4f;
            float halfWidth = (contentRect.width - spacing) / 2f;
            Rect helpBoxRect = new Rect(contentRect.x, contentRect.y, halfWidth, contentRect.height);
            Rect propertyRect = new Rect(contentRect.x + halfWidth + spacing, contentRect.y, halfWidth, contentRect.height);

            EditorGUI.HelpBox(helpBoxRect, "Custom Drawer!", MessageType.Info);

            if (myStruct == null)
            {
                EditorGUI.LabelField(propertyRect, "myStruct not found");
            }
            else
            {
                EditorGUI.PropertyField(propertyRect, myStruct, GUIContent.none, true);
            }

            EditorGUI.indentLevel = oldIndentLevel;
            EditorGUI.EndProperty();
        }

#if UNITY_2021_3_OR_NEWER && !SAINTSFIELD_UI_TOOLKIT_DISABLE
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.RowReverse,
                },
            };
            HelpBox info = new HelpBox(
                "Custom Drawer!", HelpBoxMessageType.Info)
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    minWidth = 0,
                },
            };
            root.Add(info);
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(ReferencePickerCustomDrawer.TypeWithCustomDrawer.myStruct)))
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    minWidth = 0,
                },
            });
            return root;
        }
#endif
    }
}
