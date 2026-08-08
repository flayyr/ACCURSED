using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSO", menuName = "Scriptable Objects/AttackSO")]
public class AttackSO : ScriptableObject
{
    [SerializeField] public string attackName;
    [SerializeField] public AnimatorController animatorController;
    [SerializeField] public string windAnimationState;
    [SerializeField] public string attackAnimationState;
    [SerializeField] public float windDuration;

    [Header("Data")]
    [SerializeField] public float moveDist;
    [SerializeField] public float knockback;

    [Header("Enemy")]
    [SerializeField] public float enemyWindMin;
    [SerializeField] public float enemyWindMax;
}
