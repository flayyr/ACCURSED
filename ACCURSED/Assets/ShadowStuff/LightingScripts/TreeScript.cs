using UnityEngine;

[RequireComponent(typeof(CustomDynamicLit))]
public class TreeScript: MonoBehaviour
{
    [SerializeField] Sprite leafSprite;

    [HideInInspector] public CustomDynamicLit leavesDynamicLit;

    CustomDynamicLit treeDynamicLit;

    private void Awake()
    {
        treeDynamicLit = GetComponent<CustomDynamicLit>();
        GameObject leavesPrefab = Resources.Load<GameObject>("LeavesPrefab");

        CustomDynamicLit prefabDynamicLit = leavesPrefab.GetComponent<CustomDynamicLit>();
        prefabDynamicLit.CopyValues(treeDynamicLit);
        prefabDynamicLit.useWind = true;
        prefabDynamicLit.gameObject.GetComponent<SpriteRenderer>().sprite = leafSprite;

        leavesDynamicLit = Instantiate(leavesPrefab, transform).GetComponent<CustomDynamicLit>();
        leavesDynamicLit.transform.localPosition = Vector3.zero;
    }
}
