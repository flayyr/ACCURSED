using UnityEngine;
using UnityEngine.UI;

public class LoadingSliderUI : MonoBehaviour
{
    [SerializeField] public Slider slider;
    private float startX = 800f;
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void UpdatePosition()
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x = startX + (slider.value * 1008f);

        rect.anchoredPosition = pos;
    }

    void Update()
    {
        UpdatePosition();
    }
}