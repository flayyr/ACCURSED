using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TriggerFinishWind()
    {
        anim.SetTrigger("FinishWind");
    }

    public void SetStunned(bool stunned)
    {
        anim.SetBool("Stunned", stunned);
    }


    public void SetMoveState(int moveState)
    {
        anim.SetInteger("MoveState", moveState);
    }
}
