using System.Threading.Tasks;
#if SAINTSFIELD_UNITASK && !SAINTSFIELD_UNITASK_DISABLE
using Cysharp.Threading.Tasks;
#endif
using SaintsField.Playa;
using UnityEngine;

namespace SaintsField.Samples.Scripts.SaintsEditor.Testing
{
    public class AsyncButton : SaintsMonoBehaviour
    {
        [Button]
        private async Task AsyncVoid()
        {
            Debug.Log("Async start");
            await Task.Delay(1000);
            Debug.Log("Async end");
        }

        [Button]
        private async Task<int> AsyncWithInt()
        {
            Debug.Log("Async start");
            await Task.Delay(1000);
            Debug.Log("Async end");
            return 100;
        }

#if SAINTSFIELD_UNITASK && !SAINTSFIELD_UNITASK_DISABLE
        [ShowInInspector] private bool _uniTaskUntil;

        [Button]
        private async UniTask AsyncUniTaskBase()
        {
            Debug.Log("Async start");
            // await UniTask.Yield();
            await UniTask.WaitUntil(() => _uniTaskUntil);
            // throw new Exception("xx");
            Debug.Log("Async end");
        }

        [Button]
        private async UniTask<string> AsyncUniTaskValue()
        {
            Debug.Log("Async start");
            // await UniTask.Yield();
            await UniTask.WaitUntil(() => _uniTaskUntil);
            // throw new Exception("xx");
            return "fine";
        }
#endif
    }
}
