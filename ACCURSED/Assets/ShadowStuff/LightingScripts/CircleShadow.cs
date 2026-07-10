using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleShadow : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetVisibility();
    }

    void SetVisibility()
    {
        spriteRenderer.enabled = LightManager.instance.useCircleShadow;
    }
}
