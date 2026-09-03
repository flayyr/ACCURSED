using System;
using SaintsField.Playa;

namespace SaintsField.Samples.Scripts.SaintsEditor.Testing
{
    public class ShowNullable : SaintsMonoBehaviour
    {
        [Serializable]
        public enum MyEnum
        {
            First,
            Second,
        }

        [ShowInInspector] private MyEnum? _showEnum;

        [ShowInInspector] private int? _showInt;
    }
}
