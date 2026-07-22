using System;

namespace SaintsField.Samples.Scripts.SaintsEditor.Testing
{
    [InfoBox("This is a class message", EMessageType.None)]
    [BelowInfoBox("$" + nameof(dynamicContent))]
    public class ClassInfoBoxExample : SaintsMonoBehaviour
    {
        public string dynamicContent;

        [Serializable]
        [InfoBox("This is a struct message")]
        public struct MyStruct
        {
            public string structString;
        }

        public MyStruct myStruct;
    }
}
