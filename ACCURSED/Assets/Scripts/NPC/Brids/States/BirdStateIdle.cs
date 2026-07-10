using UnityEngine;

public class BirdStateIdle : BirdBaseState
{
    public override void EnterState(BirdController bird)
    {
        // play animation first at the start
        bird.ResetIdleTimer();
        bird.PlayRandomAnimation();
    }

    public override void UpdateState(BirdController bird)
    {
        // take off if detects player
        if (bird.playerDetected)
        {
            bird.SwitchState(bird.TakeOffState);
            return;
        }

        // do nothing if animation is playing
        if (bird.playingAnimation)
            return;

        bird.idleTimer -= Time.deltaTime;

        // play random animation when idle timer stops
        if (bird.idleTimer <= 0f)
        {
            bird.ResetIdleTimer();
            bird.PlayRandomAnimation();
        }
    }

    public override void ExitState(BirdController bird)
    {
        // reset 
        bird.idleTimer = 0f;
        bird.playingAnimation = false;
    }
}
