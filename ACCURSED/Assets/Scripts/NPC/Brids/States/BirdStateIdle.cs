using UnityEngine;

public class BirdStateIdle : BirdBaseState
{
    public override void EnterState(BirdController bird)
    {
        // play animation first at the start
        bird.PlayRandomIdleAnimation();
        bird.ResetIdleTimer();
    }

    public override void UpdateState(BirdController bird)
    {
        // timer for when to play animation
        bird.idleTimer -= Time.deltaTime;

        if (bird.idleTimer < 0)
        {
            bird.PlayRandomIdleAnimation();
            bird.ResetIdleTimer();
        }
    }

    public override void ExitState(BirdController bird)
    {
        // reset timer
        bird.idleTimer = 0f;
    }
}
