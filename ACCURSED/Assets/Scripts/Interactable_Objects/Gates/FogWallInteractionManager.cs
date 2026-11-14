using UnityEngine;

public class FogWallInteractionManager : InteractableObjectManager
{
    [Header("Fog Wall")]
    [SerializeField] GateTrigger gateTrigger;
    [SerializeField] Triggerable wallObject;

    private void Start() {
        OnPromptTriggered += OpenWall;
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
        if (gateTrigger.isActivated() && !wallObject.isActivated()) {
            if (isInsideTrigger && !GlobalUIController.Instance.CheckIfOtherUIOpen() && !promptOpen) {
                OpenPrompt();
            }
        }
    }

    private void OpenWall() {
        wallObject.Trigger(true);
    }
}
