using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    [SerializeField] CharacterAnimator cAnim;
    [SerializeField] public GameObject vfxPrefab;

    private void OnEnable() {
        cAnim.OnSpawnVFX += SpawnVFX;
    }

    private void OnDisable() {
        cAnim.OnSpawnVFX -= SpawnVFX;
    }

    public void SetVFX(VFX vfx) {
        vfxPrefab = vfx.gameObject;
    }

    public void SpawnVFX() {
        if(vfxPrefab == null) {
            Debug.Log("NO VFX PREFAB!");
            return;
        }
        Transform vfxTransform = Instantiate(vfxPrefab).transform;
        vfxTransform.position = transform.position;
        vfxTransform.rotation = transform.rotation;
        vfxTransform.parent = transform;
    }
}
