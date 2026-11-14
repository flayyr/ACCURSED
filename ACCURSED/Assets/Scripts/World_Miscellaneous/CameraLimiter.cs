using UnityEngine;

public class CameraLimiter : MonoBehaviour
{
    [SerializeField] bool maxYBound;
    [SerializeField] bool minYBound;
    [SerializeField] bool maxXBound;
    [SerializeField] bool minXBound;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            CameraFollowWithMapEdges.Instance.SetCameraLimits(
                minXBound ? transform.position.x : float.MinValue,
                maxXBound ? transform.position.x : float.MaxValue,
                minYBound ? transform.position.y : float.MinValue,
                maxYBound ? transform.position.y : float.MaxValue
                );
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            CameraFollowWithMapEdges.Instance.ResetCameraLimits();
        }
    }
}
