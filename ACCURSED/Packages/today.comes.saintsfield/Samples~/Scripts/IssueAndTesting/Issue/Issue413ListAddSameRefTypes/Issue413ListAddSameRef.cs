using System;
using SaintsField.Playa;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Issue.Issue413ListAddSameRefTypes
{
    public partial class Issue413ListAddSameRef : SaintsMonoBehaviour
    {
        [Serializable]
        public enum MyEnum
        {
            OptionA,
            OptionB
        }

        public interface MyInterface
        {

        }

        [Serializable]
        public class MyClass: MyInterface
        {
            public MyEnum MyEnum;
            public int Number;

            public MyClass()
            {

            }

            public MyClass(MyEnum myEnum, int number)
            {
                MyEnum= myEnum;
                Number= number;
            }

            // Other irrelevant code here.
        }

        [SaintsSerialized, FieldDefaultExpand]
        public MyInterface[] interfaces;
    }
}
