using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;

public class CharacterAnimatorNew : MonoBehaviour
{
    // Actions invoked to be used by other scripts
    public Action OnActionFinished;
    public Action OnDodgeCancellable;

    private CharacterData cData;

    [LayoutStart("Library Management", ELayout.FoldoutBox)]
    [SerializeField] SpriteLibrary[] spriteLibraries;
    [SerializeField] private int directionIndex = 2; //0=up, 1=right, 2=down, 3=left
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
        cData = GetComponent<CharacterData>();
    }

    private void Update() {
        directionIndex = dirToIndex(cData.facingDirection);
        SetSpriteLibraries(directionIndex);
        SetMoveState((int)cData.movementState);
    }

    private int dirToIndex(Vector2 dir) {
        if (dir == Vector2.up) {return 0;} 
        else if (dir == Vector2.right) {return 1;} 
        else if (dir == Vector2.down) {return 2;} 
        else if (dir == Vector2.left) {return 3;} 
        else {return -1;}
    }

    void SetSpriteLibraries(int index) {
        //For characters with multiple sprites, the library assets lists are split in chunks of 8, assigning each spriteLibrary sequentially
        for (int i = 0; i < spriteLibraries.Length; i++) {
            spriteLibraries[i].spriteLibraryAsset = libraryAssets[index + i * 4];
        }
    }

    public void SetAnimationState(string stateName) {
        anim.Play(stateName);
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

    public int Direction => directionIndex;

    #endregion

    #region Foot Step VFX
    /// <summary>
    /// Called from animator
    /// plays a shot of the footstep particles (changes depending on the ground we are on)
    /// </summary>
    public void Footstep() {
        ParticleSystem prefab = FindFootstepVFX(transform.position); // pivot is at the feet
        if (prefab == null) return;

        ParticleSystem vfx = GetPooledFootstepVFX(prefab);
        vfx.transform.position = transform.position;

        // StopEmitting keeps the last step's particles alive (world space), Play restarts the burst
        vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        vfx.Play(true);
    }

    // OverlapPoint returns the lowest-z collider on surfaceLayers, so the top surface wins (prefab or tilemap)
    private ParticleSystem FindFootstepVFX(Vector2 footPos) {
        Collider2D ground = Physics2D.OverlapPoint(footPos, surfaceLayers);
        if (ground == null) return defaultFootstepVFX;

        SurfaceData surface = ground.GetComponentInParent<SurfaceData>();
        if (surface != null && surface.FootstepVFX != null) return surface.FootstepVFX;

        if (tileSurfaceDataBase != null && ground.TryGetComponent(out Tilemap tilemap)) {
            Vector3Int cell = tilemap.WorldToCell(footPos);
            cell.z = 0; // ignore the tilemap's z offset
            if (tileSurfaceDataBase.TryGetFootstepVFX(tilemap.GetTile(cell), out ParticleSystem tileVFX))
                return tileVFX;
        }

        return defaultFootstepVFX;
    }

    // One instance per VFX prefab, created the first time that surface is stepped on, then reused
    private ParticleSystem GetPooledFootstepVFX(ParticleSystem prefab) {
        if (footstepVFXPool.TryGetValue(prefab, out ParticleSystem instance) && instance != null)
            return instance;

        instance = Instantiate(prefab, transform.position, prefab.transform.rotation, footstepVFXParent);

        foreach (ParticleSystem ps in instance.GetComponentsInChildren<ParticleSystem>(true)) {
            ParticleSystem.MainModule main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World; // particles stay where they were emitted
            main.stopAction = ParticleSystemStopAction.None;            // a Destroy stop action would break the pool
        }

        footstepVFXPool[prefab] = instance;
        return instance;
    }
    #endregion
}
