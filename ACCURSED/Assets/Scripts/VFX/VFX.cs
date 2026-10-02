using System;
using UnityEngine;

[Serializable]
public class VFX : MonoBehaviour
{
    [SerializeField] float lifeTime = 1;

    private void Awake() {
        Invoke("DestroySelf", lifeTime);
    }

    void DestroySelf() {
        Destroy(gameObject);
    }
}
