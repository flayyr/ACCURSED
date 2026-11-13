using UnityEngine;

public class DoorwayEntrance : InteractableObjectManager
{
    [Header("Entrance")]
    [SerializeField] float fadeStartY;
    [SerializeField] float fadeOverYDist;
    [SerializeField] float detectionDist;
    [SerializeField] string sceneName;
    PlayerManager playerManager;

    private void Start() {
        playerManager = PersistentPlayer.managerInstance;

        OnPromptTriggered += TransitionToScene;
    }

    protected override void Update() {
        base.Update();
        Vector2 toPlayer = playerManager.transform.position - transform.position;

        if (Mathf.Abs(toPlayer.x) < detectionDist) {
            float yOffset = playerManager.transform.position.y - fadeStartY;
            if(yOffset > 0)
                playerManager.SetSpriteOpacity((fadeOverYDist-yOffset)/fadeOverYDist);
        }
    }

    private void TransitionToScene() {
        RoomTransitionManager.Instance.BeginTransition(sceneName);
    }
}
