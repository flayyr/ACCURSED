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

    public virtual void Trigger(ref PlayerReference playerRef)
    {
        
    }
}
