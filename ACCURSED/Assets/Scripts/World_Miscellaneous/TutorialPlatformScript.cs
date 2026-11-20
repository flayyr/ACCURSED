using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class TutorialPlatformScript : MonoBehaviour
{
    [SerializeField] float lowerYLevel;
    [SerializeField] Transform playerSnapPosition;
    [Space]
    [SerializeField] float loweringSpeed;
    [SerializeField] float fadeDuration;
    [SerializeField] float cameraSize;
    [Space]
    [SerializeField] EnemyDeath[] enemies;
    [SerializeField] AspectObject aspect;
    [Space]
    [SerializeField] ScreenFadeOverlay screenFadePrefab;
    [SerializeField] Collider2D platformCollider;
    [SerializeField] MMF_Player loweringFeedback;
    [SerializeField] MMF_Player endFeedback;
    int numEnemies;

    bool lowered = false;

    private void Awake()
    {
        numEnemies = enemies.Length;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnCharacterDead += DecrementEnemies;
        }
    }

    private void Start() {
        if(SavesManager.Instance.TryLoadData("TutorialPlatformIsLowered", out lowered)) {
            if (lowered) {
                StartLowered();
            }
        }
    }

    void DecrementEnemies(CharacterDeath enemy)
    {
        numEnemies--;
        enemy.OnCharacterDead -= DecrementEnemies;

        if(numEnemies <= 0 && !lowered)
        {
            StartCoroutine(LowerPlatform());
        }
    }

    void StartLowered() {
        PlayerController playerController = PersistentPlayer.Instance.GetComponent<PlayerController>();

        Vector3 moveAmount = new Vector3(0f, lowerYLevel - transform.localPosition.y, 0f);
        transform.position += moveAmount;
        playerController.transform.position += moveAmount;
        aspect.Move(moveAmount);
        platformCollider.enabled = false;

        foreach (CharacterDeath enemy in enemies) {
            enemy.Die();
        }

        lowered = true;
    }

    IEnumerator LowerPlatform()
    {
        PlayerController playerController = PersistentPlayer.Instance.GetComponent<PlayerController>();
        //fade to black
        ScreenFadeOverlay screenFade = Instantiate(screenFadePrefab);
        yield return screenFade.FadeToBlack(fadeDuration);

        //teleport player to position and fade from black
        CameraFollowWithMapEdges.Instance.SetTarget(playerSnapPosition);
        CameraFollowWithMapEdges.Instance.SetCameraSize(cameraSize);
        CullManager.instance.SetCullDist(3);

        playerController.SetState(PlayerControlState.Disabled);


        yield return new WaitForSeconds(fadeDuration);
        yield return screenFade.FadeFromBlack(fadeDuration);

        //lower platform and all objects
        loweringFeedback.PlayFeedbacks();

        while(transform.localPosition.y > lowerYLevel)
        {
            Vector3 moveAmount = -Vector3.up * loweringSpeed * Time.deltaTime;
            transform.position += moveAmount;
            playerController.transform.position += moveAmount;
            aspect.Move(moveAmount);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = new Vector3(transform.localPosition.x, lowerYLevel, transform.localPosition.z);
        loweringFeedback.StopFeedbacks();
        endFeedback.PlayFeedbacks();

        yield return screenFade.FadeToBlack(fadeDuration);

        CameraFollowWithMapEdges.Instance.ResetTargetToPlayer();
        CameraFollowWithMapEdges.Instance.ResetCameraSize();
        CullManager.instance.ResetCullDist();

        yield return new WaitForSeconds(fadeDuration);
        yield return screenFade.FadeFromBlack(fadeDuration);

        playerController.SetState(PlayerControlState.Normal);
        platformCollider.enabled = false;

        Destroy(screenFade.gameObject);

        lowered = true;
        SavesManager.Instance.AddData("TutorialPlatformIsLowered", lowered);
    }

    private void OnDisable()
    {
        aspect.ResetPosition();
    }


}
