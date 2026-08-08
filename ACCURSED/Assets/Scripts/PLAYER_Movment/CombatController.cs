using UnityEngine;

public enum CombatState { Idle, Winding, Attacking }
public class CombatController : MonoBehaviour
{
    AttackQueuer attackQueuer;
    CharacterAnimator cAnim;

}
