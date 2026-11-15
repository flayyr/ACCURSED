using System;
using System.Collections.Generic;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;

// Central animator bridge — all other character scripts call into here instead of touching Animator directly.
// Appends "_dir{1-8}" to every animation name before playing it, where direction is:
// 1=Up, 2=UpRight, 3=Right, 4=DownRight, 5=Down, 6=DownLeft, 7=Left, 8=UpLeft
public class CharacterAnimator : MonoBehaviour
{
    // Actions invoked to be used by other scripts
    public Action OnActionFinished;
    public Action OnDodgeCancellable;

    [LayoutStart("Library Management", ELayout.FoldoutBox)]
    [SerializeField] SpriteLibrary[] spriteLibraries;
    [SerializeField] private int direction = 5;
    [SerializeField] private bool eightDirections;
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

    void Start()
    {
        anim = GetComponent<Animator>();
        if (vfxSpawner == null)
            vfxSpawner = GetComponentInChildren<VFXSpawner>();
    }

    // Call this whenever the character's facing changes (CharacterMovement.UpdateRotation)
    public Vector2 SetFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero) return Vector2.zero;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        // Rotate attack rotator so that the attacks shift with a turning player
        attackRotator.eulerAngles = new Vector3(0, 0, angle + 90);

        // Shift by half a sector so sector boundaries fall between the 8 directions
        int sector = Mathf.FloorToInt(((angle + 22.5f) % 360f) / 45f);

        // sector 0=E→dir3, 1=NE→dir2, 2=N→dir1, 3=NW→dir8,
        //        4=W→dir7, 5=SW→dir6, 6=S→dir5, 7=SE→dir4
        if (eightDirections)
        {
            int[] sectorToDir = { 3, 2, 1, 8, 7, 6, 5, 4 };

            direction = sectorToDir[sector];

            SetSpriteLibraries(direction - 1);
        }
        else
        {
            int[] sectorToDir = { 3, 3, 1, 7, 7, 7, 5, 3 };

            direction = sectorToDir[sector];

            SetSpriteLibraries(direction - 1);
        }

        return dirToVector(direction);
    }

    void SetSpriteLibraries(int index)
    {
        //For characters with multiple sprites, the library assets lists are split in chunks of 8, assigning each spriteLibrary sequentially
        for (int i = 0; i < spriteLibraries.Length; i++)
        {
            spriteLibraries[i].spriteLibraryAsset = libraryAssets[index + i * 8];
        }
    }

    private Vector2 dirToVector(int dir)
    {
        switch (dir)
        {
            case 1:
                return Vector2.up;
            case 3:
                return Vector2.right;
            case 5:
                return Vector2.down;
            case 7:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }

    //Called by animation event when an action animation finishes.
    public void OnActionAnimationFinish()
    {
        //invokes function in CharacterManager
        OnActionFinished?.Invoke();
    }
    public void OnMakeActionCancellable()
    {
        //invokes function in CharacterManager
        OnDodgeCancellable?.Invoke();
    }
    //Called by animation event, spawns the current actionSO's windFeedback
    public void WindVFX()
    {
        vfxSpawner.SpawnWindVFX(currentAction);
    }
    //Called by animation event, spawns the current actionSO's attackFeedback
    public void AttackVFX()
    {
        vfxSpawner.SpawnAttackVFX(currentAction);
    }




    public void SwitchAnimationState(string baseName)
    {
        anim.Play(baseName);
    }

    public void GetCurrentState(string stateName)
    {
        int reformatedStateName = Animator.StringToHash("Base Layer." + stateName);
        print(anim.GetNextAnimatorStateInfo(0).fullPathHash == reformatedStateName);
    }

    // Mirrors Animator.GetCurrentAnimatorStateInfo(0).IsName, but auto-appends the direction suffix
    public bool IsCurrentState(string baseName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(baseName);
    }

    public float GetCurrentNormalizedTime()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


    #region Set Animator Properties

    public void SetWind(bool wind)
    {
        anim.SetBool("Winding", wind);
    }

    public void SetStunned(bool stunned)
    {
        anim.SetBool("Stunned", stunned);
    }

    public void SetStagger(bool stagger)
    {
        anim.SetBool("Stagger", stagger);
    }

    public void SetDead(bool dead)
    {
        anim.SetBool("Dead", dead);
    }

    public void SetResting(bool resting)
    {
        if (resting)
        {
            anim.SetTrigger("StartRest");
        }
        else
        {
            anim.SetTrigger("EndRest");
        }
    }

    public void SetDashing()
    {
        anim.SetTrigger("Dashing");
    }

    public void SetMoveState(int moveState)
    {
        anim.SetInteger("MoveState", moveState);
    }

    public void SetCurrentAction(ActionSO action)
    {
        currentAction = action;
    }

    public int Direction => direction;

    #endregion

    #region Foot Step VFX
    /// <summary>
    /// Called from animator
    /// plays a shot of the footstep particles (changes depending on the ground we are on)
    /// </summary>
    public void Footstep()
    {
        ParticleSystem prefab = FindFootstepVFX(transform.position); // pivot is at the feet
        if (prefab == null) return;

        ParticleSystem vfx = GetPooledFootstepVFX(prefab);
        vfx.transform.position = transform.position;

        // StopEmitting keeps the last step's particles alive (world space), Play restarts the burst
        vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        vfx.Play(true);
    }

    // OverlapPoint returns the lowest-z collider on surfaceLayers, so the top surface wins (prefab or tilemap)
    private ParticleSystem FindFootstepVFX(Vector2 footPos)
    {
        Collider2D ground = Physics2D.OverlapPoint(footPos, surfaceLayers);
        if (ground == null) return defaultFootstepVFX;

        SurfaceData surface = ground.GetComponentInParent<SurfaceData>();
        if (surface != null && surface.FootstepVFX != null) return surface.FootstepVFX;

        if (tileSurfaceDataBase != null && ground.TryGetComponent(out Tilemap tilemap))
        {
            Vector3Int cell = tilemap.WorldToCell(footPos);
            cell.z = 0; // ignore the tilemap's z offset
            if (tileSurfaceDataBase.TryGetFootstepVFX(tilemap.GetTile(cell), out ParticleSystem tileVFX))
                return tileVFX;
        }

        return defaultFootstepVFX;
    }

    // One instance per VFX prefab, created the first time that surface is stepped on, then reused
    private ParticleSystem GetPooledFootstepVFX(ParticleSystem prefab)
    {
        if (footstepVFXPool.TryGetValue(prefab, out ParticleSystem instance) && instance != null)
            return instance;

        instance = Instantiate(prefab, transform.position, prefab.transform.rotation, footstepVFXParent);

        foreach (ParticleSystem ps in instance.GetComponentsInChildren<ParticleSystem>(true))
        {
            ParticleSystem.MainModule main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World; // particles stay where they were emitted
            main.stopAction = ParticleSystemStopAction.None;            // a Destroy stop action would break the pool
        }

        footstepVFXPool[prefab] = instance;
        return instance;
    }
    #endregion
}
