using UnityEngine;

public class PlayerActionManager : ActionManager
{
    PlayerData pData;

    protected override void Awake() {
        base.Awake();
        pData = GetComponent<PlayerData>();
    }

    protected override void EndWind() {
        base.EndWind();
        cData.currentAction.actionSO.PlayerActionTrigger(ref pData.playerRef);
    }
}
