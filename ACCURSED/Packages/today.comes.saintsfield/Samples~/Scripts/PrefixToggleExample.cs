using UnityEngine;

namespace SaintsField.Samples.Scripts
{
    public class PrefixToggleExample : MonoBehaviour
    {
        // Example: put a bool field as a prefix of another field
        // hide the toggle field itself
        [FieldHideIf(true)] public bool myBool;
        // prefix it
        [PrefixToggle(nameof(myBool))] public int myValue;

        // Example: use a non-serialized field/property as a toggle prefix
        // this is a not serialized field
        private bool _nonSerBool;
        [PrefixToggle(nameof(_nonSerBool))] public GameObject myG;

        [Space(15)]
        // Example: useful with `InputEnableIf`, `InputDisableIf` to control field edit
        [FieldHideIf(true)] public bool useHp;
        // only allow to edit if it's checked
        [PrefixToggle(nameof(useHp)), InputEnableIf(nameof(useHp)), Range(0, 100)] public int hpValue;

        [Space(15)]
        // Example: you can use `ShowIf` to control if you want the toggle show/hide
        [Range(-10, 10)] public int range;
        public bool ShowIfPositive() => range > 0;

        [FieldHideIf(true)] public bool overrideIfPositive;
        [PrefixToggle(nameof(overrideIfPositive), showIf: nameof(ShowIfPositive))] public int v;

        [Space(15)]
        // Example: you can use it to mimic a toggle group
        [FieldHideIf(true), OnValueChanged(nameof(ChangedA))] public bool optionA = true;
        [PrefixToggle(nameof(optionA)), InputEnableIf(nameof(optionA))] public int valueA;
        [FieldHideIf(true), OnValueChanged(nameof(ChangedB))] public bool optionB;
        [PrefixToggle(nameof(optionB)), InputEnableIf(nameof(optionB))] public GameObject valueB;
        [FieldHideIf(true), OnValueChanged(nameof(ChangedC))] public bool optionC;
        [PrefixToggle(nameof(optionC)), InputEnableIf(nameof(optionC))] public Color valueC;

        private void ChangedA(bool on)
        {
            if(on)
            {
                optionB = optionC = false;
            }
        }

        private void ChangedB(bool on)
        {
            if(on)
            {
                optionA = optionC = false;
            }
        }

        private void ChangedC(bool on)
        {
            if(on)
            {
                optionA = optionB = false;
            }
        }
    }
}
