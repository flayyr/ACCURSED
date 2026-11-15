using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] string SceneName;
    [SerializeField] Vector2 targetPlayerPos;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            PersistentPlayer.controllerInstance.SetState(PlayerControlState.Disabled);
            RoomTransitionManager.Instance.BeginTransition(SceneName, SetPlayerPosAfterTransition, SetPlayerStateAfterFade);
        }
    }

    private void SetPlayerPosAfterTransition() {
        PersistentPlayer.Instance.transform.position = targetPlayerPos;
    }

    private void SetPlayerStateAfterFade() {
        PersistentPlayer.controllerInstance.SetState(PlayerControlState.Normal);
    }
}
