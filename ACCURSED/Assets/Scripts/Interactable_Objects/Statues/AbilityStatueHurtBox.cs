using UnityEngine;

public class AbilityStatueHurtBox : HurtBox
{
    [Header("Ability Statue")]
    [SerializeField] AbilityTriggeredSwitch abilityStatue;

    protected override void HurtByHitbox(HitBox hitBox) {
        var attackSO = hitBox.GetAction();
        if (attackSO.GetType() == typeof(RemembranceSO)) {
            abilityStatue.RemembranceHit();
        } else if (attackSO.GetType() == typeof(VestigeSO)) {
            abilityStatue.VestigeHit();
        }
    }

    protected override void ReceiveBuff(HitBox hitBox) {
        //blank so buff logic doesn't run
    }
}
