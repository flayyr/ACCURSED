using System.Collections;
using UnityEngine;

public class CombatHUD : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float idleTimeBeforeFade = 5f;
    [SerializeField] float fadeOutTime = 0.5f;
    [SerializeField] float fadeInTime = 0.1f;
    [Space]
    [SerializeField] AbilityUIDisplay vestigeUI;
    [SerializeField] AbilityUIDisplay remembranceUI;
    [SerializeField] HealthDisplay healthUI;
    [Space]
    PlayerStatistics playerStatistics;
    PlayerAbilities playerAbilities;

    bool showing = true;

    float currentShowingTime = 0;

    Coroutine currFade;

    private void Start()
    {
        canvasGroup.alpha = 1f;
        currentShowingTime = 0;

        playerStatistics = PersistentPlayer.Instance.gameObject.GetComponent<PlayerStatistics>();
        playerAbilities = PersistentPlayer.Instance.gameObject.GetComponent<PlayerAbilities>();

        if(playerStatistics != null)
        {
            playerStatistics.OnHealChargeUpdate += Show;
            playerStatistics.OnHealthUpdate += Show;
            playerStatistics.OnVitalityUpdate += Show;

            playerAbilities.OnAbilityUsed += Show;

            healthUI.Initialize(playerStatistics);
        }

        if(playerAbilities != null)
        {
            playerAbilities.InitializeUI(vestigeUI, remembranceUI);
        }
    }

    private void Update()
    {
        if (showing)
        {
            currentShowingTime += Time.deltaTime;
            if (currentShowingTime > idleTimeBeforeFade)
            {
                if (currFade != null)
                    StopCoroutine(currFade);
                currFade = StartCoroutine(Fade(1, 0, fadeOutTime));
                showing = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Show();
        }
    }

    public void Show()
    {
        currentShowingTime = 0;
        if(showing)
            return;

        showing = true;
        currentShowingTime = 0;
        if (currFade != null)
            StopCoroutine(currFade);
        currFade = StartCoroutine(Fade(0, 1, fadeInTime));
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
