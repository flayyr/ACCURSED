using UnityEngine;

[RequireComponent(typeof(CustomDynamicLit))]
public class TreeScript: MonoBehaviour
{
    [SerializeField] Sprite leafSprite;

    [HideInInspector]public CustomDynamicLit leavesDynamicLit;

    CustomDynamicLit treeDynamicLit;

    private void Start()
    {
        treeDynamicLit = GetComponent<CustomDynamicLit>();
        GameObject leavesPrefab = Resources.Load<GameObject>("LeavesPrefab");

        leavesDynamicLit = leavesPrefab.GetComponent<CustomDynamicLit>();
        leavesDynamicLit.CopyValues(treeDynamicLit);
        leavesDynamicLit.useWind = true;
        leavesDynamicLit.gameObject.GetComponent<SpriteRenderer>().sprite = leafSprite;
        Instantiate(leavesPrefab, transform).transform.localPosition = Vector3.zero;
        //leavesDynamicLit.SetUp();
    }
}
