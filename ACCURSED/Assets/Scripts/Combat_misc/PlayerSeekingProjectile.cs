using UnityEngine;

public class PlayerSeekingProjectile : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float despawnRange = 50;
    [SerializeField] bool constantSeek;

    Transform playerTransform;
    Vector2 dir;
    private void Start() {
        playerTransform = PersistentPlayer.Instance.transform;
        UpdateDir();
    }

    void Update()
    {
        if (constantSeek) {
            UpdateDir();
        }

        transform.position += (Vector3)dir * speed * Time.deltaTime;

        if(((Vector2)(playerTransform.position - transform.position)).magnitude > despawnRange) {
            Destroy(transform.root.gameObject);
        }
    }

    private void UpdateDir() {
        dir = (playerTransform.position - transform.position).normalized;
    }
}
