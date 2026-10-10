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
    [Space]
    [SerializeField] EnemyDeath[] enemies;
    [SerializeField] AspectObject aspect;
    [Space]
    [SerializeField] ScreenFadeOverlay screenFadePrefab;
    [SerializeField] Collider2D platformCollider;
    [SerializeField] MMF_Player loweringFeedback;
    int numEnemies;

    private void Awake()
    {
        numEnemies = enemies.Length;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].OnCharacterDead += DecrementEnemies;
        }
    }

    void DecrementEnemies(CharacterDeath enemy)
    {
        numEnemies--;
        enemy.OnCharacterDead -= DecrementEnemies;

        if(numEnemies <= 0)
        {
            StartCoroutine(LowerPlatform());
        }
    }

    IEnumerator LowerPlatform()
    {
        PlayerController playerController = PersistentPlayer.Instance.GetComponent<PlayerController>();
        //fade to black
        ScreenFadeOverlay screenFade = Instantiate(screenFadePrefab);
        yield return screenFade.FadeToBlack(fadeDuration);

        //teleport player to position and fade from black
        playerController.transform.position = playerSnapPosition.position;
        playerController.SetState(PlayerControlState.Disabled);
        yield return new WaitForSeconds(fadeDuration);
        yield return screenFade.FadeFromBlack(fadeDuration);
        Destroy(screenFade.gameObject);

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

        playerController.SetState(PlayerControlState.Normal);
        platformCollider.enabled = false;
        loweringFeedback.StopFeedbacks();
    }

    private void OnDisable()
    {
        aspect.ResetPosition();
    }


}
