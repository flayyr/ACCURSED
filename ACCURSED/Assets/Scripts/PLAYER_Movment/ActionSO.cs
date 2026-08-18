using System;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public struct AttackData
{
    public float attackDamage;
    public float stepAmount;
    public float knockbackPower;
}

[CreateAssetMenu(fileName = "ActionSO", menuName = "Actions/ActionSO")]
public class ActionSO : ScriptableObject
{
    [SerializeField] public string actionName;
    [SerializeField] public AnimatorController animatorController;
    [SerializeField] public string windAnimationState;
    [SerializeField] public string actionAnimationState;
    [SerializeField] public float windDuration;

    [Header("Attack Data")]
    [SerializeField] public AttackData attackData;


    //only triggered for the player. Ideally id have it so that enemy actions don't have this
    public virtual void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        
    }
}
