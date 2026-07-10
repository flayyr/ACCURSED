using UnityEngine;

public class BirdStateTakeOff : BirdBaseState
{
    public override void EnterState(BirdController bird)
    {
        bird.playingAnimation = true;

        bird.SetFlyDirection();

        if (bird.animator != null)
        {
            bird.animator.Play(bird.takeOffAnimationName, 0, 0f);
        }
    }

    public override void UpdateState(BirdController bird)
    {

    }

    public override void AnimationEnd(BirdController bird)
    {
        //base.AnimationEnd(bird);

        bird.playingAnimation = false;

        bird.SwitchState(bird.FlyingState);
    }

    public override void ExitState(BirdController bird)
    {
        bird.playingAnimation = false;
    }
}
