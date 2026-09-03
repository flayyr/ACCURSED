using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NormalItemPickupUI : MonoBehaviour
{
    [Header("Runtime Item")]
    [SerializeField] private ItemPickupSO item;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private GameObject imgUI;
    [SerializeField] private GameObject nameUI;
    [SerializeField] private GameObject quantityUI;

    public NormalItemPickup manager;

    public bool inQueue = true;

    public void Initialize(ItemPickupSO item, NormalItemPickup manager)
    {
        this.item = item;
        this.manager = manager;


        imgUI.GetComponent<Image>().sprite = item.itemSpr;

        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;

        quantityUI.GetComponent<TextMeshProUGUI>().text = item.itemQuantity > 0
            ? $"x{item.itemQuantity}"
            : "x1";

        StartCoroutine(UITransitions.Instance.FadeTransition(
            canvas,
            0f,
            1f,
            0.08f
        ));
    }


    public void Confirmed()
    {
        StartCoroutine(ConfirmedRoutine());
    }


    private IEnumerator ConfirmedRoutine()
    {
        yield return StartCoroutine(UITransitions.Instance.FadeTransformYTransition(
            canvas,
            1f,
            0f,
            0f,
            -160f,
            0.3f
        ));

        Destroy(gameObject);
    }

    private void Update()
    {
        ManageStack();
    }

    private void ManageStack()
    {
        if (!inQueue)
            return;

        if (manager == null)
            return;

        int queuePos = manager.GetQueuePosition(this);

        if (queuePos < 0)
            return;

        RectTransform rect = GetComponent<RectTransform>();

        float targetY = queuePos * 200f;

        Vector2 pos = rect.anchoredPosition;
        
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);

        rect.anchoredPosition = pos;
    }
}