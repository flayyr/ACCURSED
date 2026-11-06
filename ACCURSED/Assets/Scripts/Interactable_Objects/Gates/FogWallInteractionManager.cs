using UnityEngine;

public class FogWallInteractionManager : InteractableObjectManager
{
    [SerializeField] Gate fogWall;

    private void Start() {
        OnPromptTriggered += fogWall.OpenWall;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision) {
        base.OnTriggerExit2D(collision);
    }

    protected override void OnTriggerStay2D(Collider2D collision) {
        base.OnTriggerStay2D(collision);
    }

    protected override void Update() {
        if (fogWall.isUnlocked()) {
            if (isInsideTrigger && !GlobalUIController.Instance.CheckIfOtherUIOpen() && !promptOpen) {
                OpenPrompt();
            }
        }
    }
}
