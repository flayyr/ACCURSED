using MoreMountains.Feedbacks;
using System;
using UnityEditor.EditorTools;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float attackDamage = 1;
    public float stepAmount;
    public float knockbackPower;
    public float stunDuration;
    public float vitalityBuildUp = 1;
    [Range(0, 1)] public float parryLeniency = 0.5f;
}

[CreateAssetMenu(fileName = "ActionSO", menuName = "Actions/ActionSO")]
public class ActionSO : ScriptableObject
{
    [SerializeField] public string actionName;
    [SerializeField] public string actionAnimationState;
    [SerializeField] public string windAnimationState;
    [SerializeField] public float windDuration;

    [Header("Movement")]
    [SerializeField] public AnimationCurve HorizontalDisplacement;

    [Header("Attack Data")]
    [SerializeField] public AttackData attackData;

    [Header("Visual Feedback")]
    [Tooltip("Insert prefab of the vfx you want playing when winding (leave null for nothing)")]
    [SerializeField] public GameObject windFeedback;
    [Tooltip("Insert prefab of the vfx you want playing when attacking (leave null for nothing)")]
    [SerializeField] public GameObject attackFeedback;
    [Tooltip("Insert prefab of the vfx you want playing when successfully hitting an attack (leave null for nothing)")]
    [SerializeField] public GameObject attackHitFeedback;


    //only triggered for the player. Ideally id have it so that enemy actions don't have this
    public virtual void PlayerActionTrigger(ref PlayerReference playerRef)
    {

    }
}
