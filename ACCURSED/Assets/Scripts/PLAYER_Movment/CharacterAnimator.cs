using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.U2D.Animation;

// Central animator bridge — all other character scripts call into here instead of touching Animator directly.
// Appends "_dir{1-8}" to every animation name before playing it, where direction is:
// 1=Up, 2=UpRight, 3=Right, 4=DownRight, 5=Down, 6=DownLeft, 7=Left, 8=UpLeft
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private int direction = 5;
    [SerializeField] private bool eightDirections;
    [SerializeField] private Transform attackRotator;
    [SerializeField] private List<SpriteLibraryAsset> libraryAssets = new List<SpriteLibraryAsset>();
    SpriteLibrary spriteLibrary;
    Animator anim;

    void Start()
    {
        spriteLibrary = GetComponentInChildren<SpriteLibrary>();
        anim = GetComponent<Animator>();
    }

    // Call this whenever the character's facing changes (CharacterMovement.UpdateRotation)
    public void SetFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero) return;

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

            spriteLibrary.spriteLibraryAsset = libraryAssets[direction - 1];
        }
        else
        {
            int[] sectorToDir = { 3, 3, 1, 7, 7, 7, 5, 3 };

            direction = sectorToDir[sector];

            spriteLibrary.spriteLibraryAsset = libraryAssets[direction - 1];
        }
    }

    public void Play(string baseName)
    {
        anim.Play(baseName);
    }

    public float Play(string baseName, AnimatorController animator)
    {
        if(anim.runtimeAnimatorController != animator)
            anim.runtimeAnimatorController = animator;

        anim.Play(baseName);

        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(baseName)).length;
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

    public int Direction => direction;
}
