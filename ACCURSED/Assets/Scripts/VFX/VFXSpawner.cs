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

        VFX vfx = vfxTransform.GetComponent<VFX>();
        if (vfx.doNotRotate)
        {
            vfx.SetDirection(GetDirection());
        }
        else
        {
            vfxTransform.rotation = transform.rotation;
        }
    }

    int GetDirection()//return 0-3 for up,right,down,left
    {
        float rot = transform.rotation.eulerAngles.z;

        Vector2 dir = -transform.up;
        if (Mathf.Abs(dir.x) > 0.3)//could be 0 instead of .3, its there in case we fuck up in the future
        {
            return Mathf.Sign(dir.x) == 1? 1 : 3;
        }
        return Mathf.Sign(dir.y)==1?0:2;
    }
}
