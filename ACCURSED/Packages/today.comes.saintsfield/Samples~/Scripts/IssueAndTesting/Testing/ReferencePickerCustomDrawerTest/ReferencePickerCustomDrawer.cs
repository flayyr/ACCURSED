using System;
using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest
{
    public class ReferencePickerCustomDrawer : MonoBehaviour
    {
        public interface IRefInterface
        {
            public int TheInt { get; }
        }

        [Serializable]
        public struct TypeWithCustomDrawer : IRefInterface
        {
            [field: SerializeField]
            public int TheInt { get; set; }
            public string myStruct;
        }

        [Serializable]
        public class TypeWithDefaultDrawer: IRefInterface
        {
            [field: SerializeField, Range(0, 10)]
            public int TheInt { get; set; }
        }

#if UNITY_2021_3_OR_NEWER
        [SerializeReference, ReferencePicker]
#endif
        public IRefInterface myInterface;

    }
}
