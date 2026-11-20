using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Vector2 anchorPos;
    [SerializeField] Vector2 parallaxStrength;

    Vector2 originPos;

    Transform cameraTransform;

    private void Start()
    {
        originPos = transform.position;
    }
    void Update()
    {
        if(cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        Vector2 offset = (Vector2)cameraTransform.position - anchorPos;
        transform.position = new Vector3(originPos.x + -parallaxStrength.x * offset.x, originPos.y + -parallaxStrength.y * offset.y, transform.position.z);
    }
}
