using UnityEngine;

namespace SaintsField.Samples.Scripts
{
    public class InputDisableIfExample : MonoBehaviour
    {
        public bool disableMe;

        [AboveButton(":Debug.Log")]
        [BelowButton(":Debug.Log")]
        [PostFieldButton(":Debug.Log", "L")]
        [PrefixToggle(nameof(useHp))]
        [InputDisableIf(nameof(disableMe))]
        public int myField;

        [Space(20)]

        // Example: useful with `InputEnableIf`, `InputDisableIf` to control field edit
        [FieldHideIf(true)] public bool useHp;
        // only allow to edit if it's checked
        [PrefixToggle(nameof(useHp)), InputEnableIf(nameof(useHp)), Range(0, 100)] public int hpValue;

    }
}
