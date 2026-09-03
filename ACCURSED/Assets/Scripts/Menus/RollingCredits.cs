using UnityEngine;
using UnityEngine.SceneManagement;

public class RollingCredits : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 50f;
    public float fastSpeed = 200f;

    [Header("End setting")]
    public float endYPosition = 1200f;
    //public string sceneToLoadWhenFinished = "StartMenu";

    [Header("Destination")]
    [SerializeField] private string startScreenName = "StartMenu";

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float currentSpeed = normalSpeed;

        if (Input.GetKey(KeyCode.Space))
            currentSpeed = fastSpeed;

        if (rectTransform.anchoredPosition.y < endYPosition)
        {
            rectTransform.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;
        }
        else
        {
            RoomTransitionManager manager = RoomTransitionManager.Instance;

            if (manager == null)
            {
                Debug.LogError("EscapeMenuHandler: No RoomTransitionManager exists. " +
                    "Add the RoomTransitionManager prefab to the scene.");
                return;
            }

            if (manager.IsTransitioning)
                return;
            
            manager.BeginTransition(startScreenName);
        }

        
        //SceneManager.LoadScene(sceneToLoadWhenFinished);
    }
}
