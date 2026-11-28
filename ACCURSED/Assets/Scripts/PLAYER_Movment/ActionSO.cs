using MoreMountains.Feedbacks;
using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float attackDamage = 1;
    public float stepAmount;
    public float knockbackPower;
    public float stunDuration;
    public float staggerAmount;
    public float vitalityBuildUp = 1;
    [Range(-0.01f, 1)] public float parryLeniency = 0.5f;

    public BuffData buffData;
}

[Serializable]
public struct ActionKeyValuePair {
    public ActionSO key;
    public ActionSO value;
}

[Serializable]
public class BuffData {
    public BuffType type;
    public BuffTarget target;
    public float amount;
    public float duration;
}
public enum BuffType { None, DefenseUp, StairsSlow, RimeflowerFreeze }
public enum BuffTarget { Enemy, Player }

[CreateAssetMenu(fileName = "ActionSO", menuName = "Actions/ActionSO")]
public class ActionSO : ScriptableObject
{
    [SerializeField] public string actionName;
    [SerializeField] public string windAnimationState;//will need to rename this at some point, not done rn cus i dont want to reinput all the animation names just yet

    [Header("Winding")]
    [SerializeField] public bool useWind;
    [SerializeField] public float windDuration;

    [Header("Movement")]
    [SerializeField] public bool allowDirectionUpdate;
    [SerializeField] public AnimationCurve HorizontalDisplacement;

    [Header("Attack Data")]
    [SerializeField] public AttackData attackData;

    [Header("Chained Actions")]
    [SerializeField] public ActionKeyValuePair[] chainedActions;

    [Header("Visual Feedback")]
    [Tooltip("Insert prefab of the vfx you want playing when winding (leave null for nothing)")]
    [SerializeField] public GameObject windFeedback;
    [Tooltip("Insert prefab of the vfx you want playing when attacking (leave null for nothing)")]
    [SerializeField] public GameObject attackFeedback;
    [Tooltip("Insert prefab of the vfx you want playing when successfully hitting an attack (leave null for nothing)")]
    [SerializeField] public GameObject attackHitFeedback;

    public virtual ActionSO GetChainedAction(ActionSO previousAction) {
        foreach(ActionKeyValuePair pair in chainedActions) {
            if (pair.key == previousAction) {
                return pair.value;
            }
        }
        return this;
    }

    //only triggered for the player. Ideally id have it so that enemy actions don't have this
    public virtual void PlayerActionTrigger(ref PlayerReference playerRef)
    {

    }

    public virtual void GeneralActionTrigger(ref CharacterReference characterRef)
    {

    }

    //events that can be triggered by animation events. key can be used to set up multiple events
    public virtual void AnimationEventTrigger(ref CharacterReference characterRef, int key) {

    }
}
