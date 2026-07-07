using UnityEngine;

public abstract class BirdBaseState
{
    public abstract void EnterState(BirdController bird);

    public abstract void UpdateState(BirdController bird);

    public abstract void ExitState(BirdController bird);
}
