using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerManager), typeof(PlayerInput), typeof(Rigidbody2D))]
public class DisableWhenInMenus : MonoBehaviour
{
    [SerializeField] private List<string> disabledScenes = new List<string>();

    private PlayerManager playerManager;
    private PlayerInput playerInput;
    private Rigidbody2D body;
    private readonly List<Renderer> renderers = new List<Renderer>();
    private readonly Dictionary<Renderer, bool> rendererWasHidden = new Dictionary<Renderer, bool>();
    private bool managerWasEnabled;
    private bool inputWasEnabled;
    private bool bodyWasSimulated;
    private bool initialized;
    private bool playerDisabled;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerInput = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        if (initialized)
            UpdatePlayerState();
    }

    private void Start()
    {
        initialized = true;
        UpdatePlayerState();
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        RestorePlayer();
    }

    private void LateUpdate()
    {
        if (playerDisabled)
            HideRenderers();
    }

    private void OnActiveSceneChanged(Scene previousScene, Scene nextScene)
    {
        if (initialized)
            UpdatePlayerState();
    }

    private void UpdatePlayerState()
    {
        if (disabledScenes.Contains(SceneManager.GetActiveScene().name))
            DisablePlayer();
        else
            RestorePlayer();
    }

    private void DisablePlayer()
    {
        if (playerDisabled)
            return;

        playerDisabled = true;
        managerWasEnabled = playerManager.enabled;
        inputWasEnabled = playerInput.enabled;
        bodyWasSimulated = body.simulated;
        playerInput.enabled = false;
        playerManager.MoveInput(Vector2.zero);
        if (managerWasEnabled)
        {
            // PlayerManager does not unsubscribe this handler in its OnDisable.
            SceneManager.sceneLoaded -= playerManager.ResetSpriteOpacity;
            playerManager.enabled = false;
        }

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.simulated = false;

        HideRenderers();
    }

    private void HideRenderers()
    {
        GetComponentsInChildren(true, renderers);
        foreach (Renderer playerRenderer in renderers)
        {
            if (!rendererWasHidden.ContainsKey(playerRenderer))
                rendererWasHidden.Add(playerRenderer, playerRenderer.forceRenderingOff);
            playerRenderer.forceRenderingOff = true;
        }
    }

    private void RestorePlayer()
    {
        if (!playerDisabled)
            return;

        playerDisabled = false;
        foreach (KeyValuePair<Renderer, bool> entry in rendererWasHidden)
        {
            if (entry.Key != null)
                entry.Key.forceRenderingOff = entry.Value;
        }
        rendererWasHidden.Clear();
        renderers.Clear();

        body.simulated = bodyWasSimulated;
        playerManager.enabled = managerWasEnabled;
        playerInput.enabled = inputWasEnabled;
    }
}
