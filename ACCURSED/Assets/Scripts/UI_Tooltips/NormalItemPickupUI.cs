using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// This class manages the specific Prefab for normal item pickups
public class NormalItemPickupUI : MonoBehaviour
{
    [SerializeField] ItemPickupSO item;
    [SerializeField] CanvasGroup canvas;

    [SerializeField] GameObject imgUI;
    [SerializeField] GameObject nameUI;
    [SerializeField] GameObject quantityUI;

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
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;
        quantityUI.GetComponent<TextMeshProUGUI>().text = item.itemQuantity > 0 ? $"x{item.itemQuantity}" : "x1";

        Debug.Log("Initialize");
        //StartCoroutine(FadeTransformTransition());
        StartCoroutine(UITransitions.Instance.FadeTransition(canvas, 0f, 1f, 0.08f));
    }

    //exit animation
    public void Confirmed()
    {
        StartCoroutine(ConfirmedRoutine());
        Debug.Log("Confirmed");
    }

    private IEnumerator ConfirmedRoutine()
    {
        Debug.Log("ConfirmedRoutine");
        yield return StartCoroutine(UITransitions.Instance.FadeTransformYTransition(canvas, 1f, 0f, 0f, -80f, 0.3f));
        Destroy(gameObject);
    }

    private void Awake()
    {
        imgUI.GetComponent<Image>().sprite = item.itemSpr;
        nameUI.GetComponent<TextMeshProUGUI>().text = item.itemName;

        quantityUI.GetComponent<TextMeshProUGUI>().text = (item.itemQuantity != 0) ? $"x{item.itemQuantity}" : "x1";

    }

}
