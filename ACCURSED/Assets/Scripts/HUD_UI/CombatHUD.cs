using System.Collections;
using UnityEngine;

public class CombatHUD : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float idleTimeBeforeFade = 5f;
    [SerializeField] float fadeOutTime = 0.5f;
    [SerializeField] float fadeInTime = 0.1f;
    [Space]
    [SerializeField] PlayerStatistics playerStatistics;
    [SerializeField] PlayerAbilities playerAbilities;

    bool showing = true;

    float currentShowingTime = 0;

    private void Start()
    {
        canvasGroup.alpha = 1f;
        currentShowingTime = 0;

        if(playerStatistics != null)
        {
            playerStatistics.OnHealChargeUpdate += Show;
            playerStatistics.OnHealthUpdate += Show;
            playerStatistics.OnVitalityUpdate += Show;

            playerAbilities.OnAbilityUsed += Show;
        }
    }

    private void Update()
    {
        if (showing)
        {
            currentShowingTime += Time.deltaTime;
            if (currentShowingTime > idleTimeBeforeFade)
            {
                StartCoroutine(Fade(1, 0, fadeOutTime));
                showing = false;
            }
        }
    }

    public void Show()
    {
        if(showing)
            return;

        showing = true;
        currentShowingTime = 0;
        StartCoroutine(Fade(0, 1, fadeInTime));
    }

    IEnumerator Fade(float from, float to, float fadeTime)
    {
        float t = 0;
        while (t<fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, t/fadeTime);

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = to;
    }
}
