using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Issue.Issue416
{
    public class DictionaryOwner : MonoBehaviour
    {
        [SerializeField] private SaintsDictionary<SomeEnum, SaintsInterface<ISomeInterface>> _theDictionary;
        [SerializeField] private SaintsDictionary<SomeEnum, ISomeInterface> _direct;

        // public SaintsInterface<ISomeInterface> interf;
    }
}
