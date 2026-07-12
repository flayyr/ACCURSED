using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BirdStateFlying : BirdBaseState
{
    private float flyingTimer;
    private float fadeTimer;

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

        flyingTimer = 0f;
        fadeTimer = 0f;

        bird.InitializeFade();

        Debug.Log("Entered Flying state.");
    }

    public override void UpdateState(BirdController bird)
    {
        bird.UpdateFlightFluctuation();

        bird.flyDirection = bird.GetCurrentFlyDirection();

        bird.birdTransform.position += (Vector3)(bird.flyDirection * bird.flySpeed * Time.deltaTime);

        flyingTimer += Time.deltaTime;

        // Remain fully visible until the delay has passed.
        if (flyingTimer < bird.flyFadeDelay)
        {
            bird.SetFlightFade(1f);
            return;
        }

        fadeTimer += Time.deltaTime;

        float fadeAmount = 1f - Mathf.Clamp01(fadeTimer / bird.flyFadeDuration);

        bird.SetFlightFade(fadeAmount);

        if (fadeAmount <= 0f)
            bird.gameObject.SetActive(false);
    }

    public override void ExitState(BirdController bird)
    {
        bird.playingAnimation = false;
    }
}