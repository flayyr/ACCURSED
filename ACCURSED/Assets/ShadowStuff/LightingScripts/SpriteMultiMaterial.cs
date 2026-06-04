using System.Collections.Generic;
using UnityEngine;

public class SpriteMultiMaterial : MonoBehaviour
{
    public List<Material> materials = new();

    void Awake()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.SetSharedMaterials(materials);
        }
    }
}
