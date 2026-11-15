using UnityEngine;

public class GateTriggerable : Triggerable
{
    [SerializeField] Animator animator;
    [SerializeField] Collider2D gateCollider;
    public override void Trigger(bool activate) {
        base.Trigger(activate);

        animator.SetBool("Open", activate);
        gateCollider.enabled = !activate;
    }

    public override void InitialState(bool activate) {
        base.InitialState(activate);

        animator.Play(activate?"opened":"closed");

        Trigger(activate);
    }
}
