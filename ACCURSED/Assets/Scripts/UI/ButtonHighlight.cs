using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject highlight;

    private bool isHighlighted;
    void Awake()
    {
        isHighlighted = false;
        highlight.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHighlighted && highlight != null)
        {
            ShowHighlight();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHighlighted && highlight != null)
        {
            HideHighlight();
        }
    }

    private void ShowHighlight()
    {
        highlight.SetActive(true);
        isHighlighted = true;
    }

    private void HideHighlight()
    {
        highlight.SetActive(false);
        isHighlighted = false;
    }
}
