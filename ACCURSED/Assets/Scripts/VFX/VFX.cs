using System;
using UnityEngine;

[Serializable]
public class VFX : MonoBehaviour
{
    [SerializeField] float lifeTime = 1;
    public bool doNotRotate;
    public bool animated;

    int direction;//0-3:up,right,down,left
    Animator animator;

    private void Awake() {
        Invoke("DestroySelf", lifeTime);
        if (animated)
        {
            animator = GetComponent<Animator>();
        }
    }

    void DestroySelf() {
        Destroy(gameObject);
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        if(animated)
            animator.SetInteger("Direction", dir);
    }
}
