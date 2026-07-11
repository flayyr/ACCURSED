using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BirdStateFlying : BirdBaseState
{
    public override void EnterState(BirdController bird)
    {
        bird.flyDirection = bird.GetCurrentFlyDirection();

        // flip the bird based on flying direction
        if (bird.flyDirection.x != 0)
        {
            Vector3 localScale = bird.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * Mathf.Sign(bird.flyDirection.x * -1);
            bird.transform.localScale = localScale;
        }

        bird.playingAnimation = true;

        if (bird.animator != null)
        {
            bird.animator.Play(bird.flyAnimationName, 0, 0f);
        }

        Debug.Log(bird.name + " entered Flying state.");
    }

    public override void UpdateState(BirdController bird)
    {
        bird.UpdateFlightFluctuation();

        bird.flyDirection = bird.GetCurrentFlyDirection();

        bird.birdTransform.position += (Vector3)(bird.flyDirection * bird.flySpeed * Time.deltaTime);
    }

    public override void ExitState(BirdController bird)
    {
        bird.playingAnimation = false;
    }
}