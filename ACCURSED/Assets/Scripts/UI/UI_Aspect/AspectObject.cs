using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class AspectObject : MonoBehaviour
{
    [SerializeField] private AspectSO aspectSO;

    Vector3 originalPosition;

#if UNITY_EDITOR
    private void Update()
    {
        if (aspectSO != null)
        {
            transform.position = aspectSO.position;
            aspectSO.sceneName = SceneManager.GetActiveScene().name;
        }
    }
#endif

    private void Awake()
    {
        originalPosition = aspectSO.position;
    }

    public void Move(Vector3 moveAmount)
    {
        aspectSO.position += moveAmount;
    }

    public void ResetPosition()
    {
        aspectSO.position = originalPosition;
    }
}
