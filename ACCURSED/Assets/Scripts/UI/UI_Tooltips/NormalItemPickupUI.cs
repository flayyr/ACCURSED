using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System;

// This class manages the specific Prefab for normal item pickups
public class NormalItemPickupUI : MonoBehaviour
{
    [SerializeField] private ItemPickupSO item;
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private GameObject imgUI;
    [SerializeField] private GameObject nameUI;
    [SerializeField] private GameObject quantityUI;

    public NormalItemPickup manager;


    public bool inQueue = true;

    /*public NormalItemPickupUI(ItemPickupSO item)
    {
        this.item = item;
        this.imgUI.GetComponent<Image>().sprite = item.itemSpr;
        this.nameUI.GetComponent <TextMeshProUGUI>().text = item.itemName;
        quantityUI.GetComponent<TextMeshProUGUI>().text = (item.itemQuantity != 0) ? "x" + item.itemQuantity : "x1";
    }*/

    // Monobehaviors can't use constructors apparently so here's an initialize method instead
    public void Initialize(ItemPickupSO item)
    {
        this.item = item;

        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;

        quantityUI.GetComponent<TextMeshProUGUI>().text = item.itemQuantity > 0
            ? $"x{item.itemQuantity}"
            : "x1";

        manager = ToolTipManager.Instance.GetComponent<NormalItemPickup>();

        StartCoroutine(UITransitions.Instance.FadeTransition(canvas, 0f, 1f, 0.08f));
    }

    //exit animation
    public void Confirmed()
    {
        StartCoroutine(ConfirmedRoutine());
        //Debug.Log("Confirmed");
    }

    private IEnumerator ConfirmedRoutine()
    {
        //Debug.Log("ConfirmedRoutine");
        yield return StartCoroutine(UITransitions.Instance.FadeTransformYTransition(canvas, 1f, 0f, 0f, -160f, 0.3f));
        Destroy(gameObject);
    }

    private void Awake()
    {
        // Item information is assigned in Initialize().
    }

    /*
    private void Awake()
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;
        manager = ToolTipManager.Instance.GetComponent<NormalItemPickup>();

        quantityUI.GetComponent<TextMeshProUGUI>().text = (item.itemQuantity != 0) ? $"x{item.itemQuantity}" : "x1";

    }
    */

    // lerp instance's Y position based on queue position
    private void ManageStack()
    {
        if (item != null && inQueue)
        {
            RectTransform rect = GetComponent<RectTransform>();
            float defY = 0f;

            int queuePos = Array.IndexOf(ToolTipManager.Instance.GetComponent<NormalItemPickup>().itemPickupQueue.ToArray(), item);
            float targetY = defY + queuePos * 200f;

            Vector2 pos = rect.anchoredPosition;

            pos.y = Mathf.Lerp(pos.y,targetY,Time.deltaTime * 10f);

            rect.anchoredPosition = pos;
        }
    }

    private void Update()
    {
        ManageStack();
    }

}
