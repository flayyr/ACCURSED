using UnityEngine;

public class ArenaEnterTrigger : MonoBehaviour
{
    [SerializeField] Triggerable gateTrigger;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            gateTrigger.Trigger(false);
        }
    }
}
