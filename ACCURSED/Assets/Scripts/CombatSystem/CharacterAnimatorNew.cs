using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterAnimatorNew : MonoBehaviour
{
    // Actions invoked to be used by other scripts
    public Action OnActionFinished;
    public Action OnDodgeCancellable;

    [LayoutStart("Library Management", ELayout.FoldoutBox)]
    [SerializeField] SpriteLibrary[] spriteLibraries;
    [SerializeField] private int direction = 2; //0=up, 1=right, 2=down, 3=left
    [SerializeField] private Transform attackRotator;
    [SerializeField] private List<SpriteLibraryAsset> libraryAssets = new List<SpriteLibraryAsset>();

    [LayoutStart("Visual Effects", ELayout.FoldoutBox)]
    [SerializeField] VFXSpawner vfxSpawner;

    [LayoutStart(".Foot Steps", ELayout.FoldoutBox)]
    [Tooltip("All footstep particles created are pooled under this")]
    [SerializeField] Transform footstepVFXParent;
    [Tooltip("Maps tiles to footstep VFX")]
    [SerializeField] TileSurfaceDataBaseSO tileSurfaceDataBase;
    [Tooltip("Played when no prefab surface or mapped tile is under the feet")]
    [SerializeField] ParticleSystem defaultFootstepVFX;
    [Tooltip("Layers holding ground colliders (SurfaceData prefabs + walkable tilemaps). Must not include the player's own colliders")]
    [SerializeField] LayerMask surfaceLayers;
    [Tooltip("Dictionary holding the references to the particle system instances (prefab-instance) for quick finding")]
    [SerializeField] readonly Dictionary<ParticleSystem, ParticleSystem> footstepVFXPool = new Dictionary<ParticleSystem, ParticleSystem>();

    Animator anim;
    ActionSO currentAction;//set by CharacterManager when an action starts, used for VFX

    void Start() {
        anim = GetComponent<Animator>();
        if (vfxSpawner == null)
            vfxSpawner = GetComponentInChildren<VFXSpawner>();
    }

    void SetSpriteLibraries(int index) {
        //For characters with multiple sprites, the library assets lists are split in chunks of 8, assigning each spriteLibrary sequentially
        for (int i = 0; i < spriteLibraries.Length; i++) {
            spriteLibraries[i].spriteLibraryAsset = libraryAssets[index + i * 4];
        }
    }

    //Called by animation event when an action animation finishes.
    public void OnActionAnimationFinish() {
        //invokes function in CharacterManager
        OnActionFinished?.Invoke();
    }
    public void OnMakeActionCancellable() {
        //invokes function in CharacterManager
        OnDodgeCancellable?.Invoke();
    }
    //Called by animation event, spawns the current actionSO's windFeedback
    public void WindVFX() {
        vfxSpawner.SpawnWindVFX(currentAction);
    }
    //Called by animation event, spawns the current actionSO's attackFeedback
    public void AttackVFX() {
        vfxSpawner.SpawnAttackVFX(currentAction);
    }

    #region Set Animator Properties

    public void SetWind(bool wind) {
        anim.SetBool("Winding", wind);
    }

    public void SetStunned(bool stunned) {
        anim.SetBool("Stunned", stunned);
    }

    public void SetStagger(bool stagger) {
        anim.SetBool("Stagger", stagger);
    }

    public void SetDead(bool dead) {
        anim.SetBool("Dead", dead);
    }

    public void SetResting(bool resting) {
        if (resting) {
            anim.SetTrigger("StartRest");
        } else {
            anim.SetTrigger("EndRest");
        }
    }

    public void SetDashing() {
        anim.SetTrigger("Dashing");
    }

    public void SetMoveState(int moveState) {
        anim.SetInteger("MoveState", moveState);
    }

    public void SetCurrentAction(ActionSO action) {
        currentAction = action;
    }

    public int Direction => direction;

    #endregion
}
