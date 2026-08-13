using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionSO", menuName = "Actions/ActionSO")]
public class ActionSO : ScriptableObject
{
    [SerializeField] public string actionName;
    [SerializeField] public AnimatorController animatorController;
    [SerializeField] public string windAnimationState;
    [SerializeField] public string actionAnimationState;
    [SerializeField] public float windDuration;

    [Header("Data")]
    [SerializeField] public float attackDamage = 1f;
    [SerializeField] public float stepAmount;
    [SerializeField] public float knockbackPower;

    //only triggered for the player. Ideally id have it so that enemy actions don't have this
    public virtual void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        
    }
}
