#if UNITY_2021_3_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;

namespace SaintsField.Editor.UIToolkitElements.CharacterDrawer
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class CharacterElement: BindableElement, INotifyValueChanged<int>
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<CharacterElement, UxmlTraits> { }
#endif

        private readonly TextField _textField;

        // ReSharper disable once MemberCanBePrivate.Global
        public CharacterElement()
        {
            Add(_textField = new TextField(""));
            _textField.RegisterValueChangedCallback(evt =>
            {
                string newValue = evt.newValue;
                int newInt = string.IsNullOrEmpty(newValue) ? 0 : newValue[0];
                int acceptInt = Mathf.Clamp(newInt, char.MinValue, char.MaxValue);
                if (acceptInt != value)
                {
                    value = acceptInt;
                }
            });
        }

        public void SetValueWithoutNotify(int newValue)
        {
            string newStr = newValue == 0 ? "" : ((char)newValue).ToString();
            _textField.SetValueWithoutNotify(newStr);
        }

        public int value
        {
            get => string.IsNullOrEmpty(_textField.value) ? 0 : _textField.value[0];
            set
            {
                int preValue = string.IsNullOrEmpty(_textField.value) ? 0 : _textField.value[0];
                if (preValue == value)
                {
                    return;
                }

                SetValueWithoutNotify(value);
                using ChangeEvent<int> evt = ChangeEvent<int>.GetPooled(preValue, value);
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}
#endif
