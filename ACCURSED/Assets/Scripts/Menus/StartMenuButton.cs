using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public enum ButtonType
    {
        Play,
        Saves,
        Settings,
        Credits,
        Quit
    }

    [Header("Button Type")]
    public ButtonType buttonType;

    [Header("Selection Visualization")]
    public float selectedScale = 1.2f;
    public float scaleSpeed = 12f;

    [Header("Arrows")]
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    public Vector3 leftArrowOffset = new Vector3(-120f, 0f, 0f);
    public Vector3 rightArrowOffset = new Vector3(120f, 0f, 0f);

    private Vector3 normalScale;
    private GameObject leftArrowInstance;
    private GameObject rightArrowInstance;
    private bool rowSelected;

    private StartMenuManager manager;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    public void Initialize(StartMenuManager newManager)
    {
        manager = newManager;
    }

    private void LateUpdate()
    {
        Vector3 targetScale = rowSelected ? normalScale * selectedScale : normalScale;
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }

    public void SetSelected(bool selected)
    {
        if (rowSelected == selected)
            return;

        rowSelected = selected;

        if (rowSelected)
        {
            SpawnArrows();
        }
        else
        {
            RemoveArrows();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager == null)
            return;

        manager.SelectButton(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null)
            return;

        manager.ActivateButton(this);
    }

    private void SpawnArrows()
    {
        RemoveArrows();

        RectTransform buttonRect = GetComponent<RectTransform>();

        if (buttonRect == null)
        {
            Debug.LogWarning("Button has no RectTransform: " + gameObject.name);
            return;
        }

        if (leftArrowPrefab != null)
        {
            leftArrowInstance = Instantiate(leftArrowPrefab, transform);
            SetupArrowAsButtonChild(leftArrowInstance, buttonRect, true);
        }

        if (rightArrowPrefab != null)
        {
            rightArrowInstance = Instantiate(rightArrowPrefab, transform);
            SetupArrowAsButtonChild(rightArrowInstance, buttonRect, false);
        }
    }

    private void SetupArrowAsButtonChild(GameObject arrowObject, RectTransform buttonRect, bool isLeftArrow)
    {
        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();

        if (arrowRect == null)
        {
            Debug.LogWarning("Arrow prefab needs a RectTransform.");
            return;
        }

        arrowObject.SetActive(true);

        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);

        float buttonHalfWidth = buttonRect.rect.width / 2f;

        Vector3 offset = isLeftArrow ? leftArrowOffset : rightArrowOffset;
        float baseX = isLeftArrow ? -buttonHalfWidth : buttonHalfWidth;

        arrowRect.anchoredPosition = new Vector2(
            baseX + offset.x,
            offset.y
        );

        arrowRect.localScale = Vector3.one;
        arrowRect.localRotation = Quaternion.identity;
        arrowRect.SetAsLastSibling();
    }

    private void RemoveArrows()
    {
        if (leftArrowInstance != null)
        {
            Destroy(leftArrowInstance);
            leftArrowInstance = null;
        }

        if (rightArrowInstance != null)
        {
            Destroy(rightArrowInstance);
            rightArrowInstance = null;
        }
    }

    private void OnDisable()
    {
        RemoveArrows();
    }
}