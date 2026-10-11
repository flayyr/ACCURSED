using System;
using System.Collections.Generic;
using UnityEngine;

public interface IResetable {
    public void Reset();
}

public class ResetObjectsManager : MonoBehaviour
{
    public static ResetObjectsManager Instance;

    List<IResetable> resetableObjs = new List<IResetable>();

    private void Awake() {
        Instance = this;
    }

    public void AddResetable(IResetable obj) {
        resetableObjs.Add(obj);
    }

    public void RemoveResetable(IResetable obj) {
        resetableObjs.Remove(obj);
    }

    public void ResetAll() {
        foreach (IResetable resetable in resetableObjs) {
            resetable.Reset();
        }
    }
}
