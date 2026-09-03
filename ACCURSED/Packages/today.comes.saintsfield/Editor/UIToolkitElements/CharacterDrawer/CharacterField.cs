using UnityEngine;
using UnityEngine.UIElements;

namespace SaintsField.Editor.UIToolkitElements.CharacterDrawer
{
    public class CharacterField: TextValueField<char>
    {
        public new static readonly string ussClassName = "saintsfield-character-field";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        private CharacterInput CharacterTextInput => (CharacterInput)textInputBase;

        public CharacterField(string label)
            : base(label, 1, new CharacterInput())
        {
            AddToClassList(ussClassName);
            labelElement.AddToClassList(labelUssClassName);
            textInputBase.AddToClassList(inputUssClassName);

            AddLabelDragger<char>();
        }

        protected override string ValueToString(char value)
        {
            return value == 0 ? "" : ((char)value).ToString();
        }

        protected override char StringToValue(string str)
        {
            return string.IsNullOrEmpty(str) ? '\0' : str[0];
        }

        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, char startValue)
        {
            CharacterTextInput.ApplyInputDeviceDelta(delta, speed, startValue);
        }

        private class CharacterInput: TextValueInput
        {
            private CharacterField ParentCharacterField => (CharacterField)parent;

            protected override string allowedCharacters => null;

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, char startValue)
            {
                float acceleration = Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
                char currentValue = StringToValue(text);
                char nextValue = (char)Mathf.Clamp(
                    currentValue + Mathf.RoundToInt(NiceDelta(delta, acceleration)),
                    char.MinValue,
                    char.MaxValue);

                if (ParentCharacterField.isDelayed)
                {
                    text = ValueToString(nextValue);
                }
                else
                {
                    ParentCharacterField.value = nextValue;
                }
            }

            private static float Acceleration(bool shiftPressed, bool altPressed)
            {
                return (shiftPressed ? 4f : 1f) * (altPressed ? 0.25f : 1f);
            }

            private static bool _useYSign;

            private static float NiceDelta(Vector2 deviceDelta, float acceleration)
            {
                deviceDelta.y = -deviceDelta.y;
                if (Mathf.Abs(Mathf.Abs(deviceDelta.x) - Mathf.Abs(deviceDelta.y)) /
                    Mathf.Max(Mathf.Abs(deviceDelta.x), Mathf.Abs(deviceDelta.y)) > 0.1f)
                {
                    _useYSign = Mathf.Abs(deviceDelta.x) <= Mathf.Abs(deviceDelta.y);
                }

                return (_useYSign ? Mathf.Sign(deviceDelta.y) : Mathf.Sign(deviceDelta.x)) * deviceDelta.magnitude * acceleration;
            }

            protected override string ValueToString(char value)
            {
                return ParentCharacterField.ValueToString(value);
            }

            protected override char StringToValue(string str)
            {
                return ParentCharacterField.StringToValue(str);
            }
        }
    }
}
