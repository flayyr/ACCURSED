using UnityEngine;

[CreateAssetMenu(fileName = "ChargeableActionSO", menuName = "Actions/ChargeableAction")]
public class ChargeableActionSO : ActionSO
{
    [Header("Charge Attack")]
    public float chargeTime;
    [Tooltip("Charged action gets played instantly instead of this if charged")]
    public ActionSO chargedAction;
}
