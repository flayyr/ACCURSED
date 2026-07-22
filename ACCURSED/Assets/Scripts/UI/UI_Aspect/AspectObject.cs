using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class AspectObject : MonoBehaviour
{
    [SerializeField] private AspectSO aspectSO;

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
}
