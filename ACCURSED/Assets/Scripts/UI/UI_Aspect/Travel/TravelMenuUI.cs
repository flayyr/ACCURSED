using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TravelMenuUI : MonoBehaviour
{
    public static TravelMenuUI Instance { get; private set; }

    [SerializeField] GameObject title;
    [SerializeField] List<GameObject> buttons = new List<GameObject>();

    [SerializeField] MajorRegion altar;
    [SerializeField] MajorRegion vita;
    [SerializeField] MajorRegion ritus;
    [SerializeField] MajorRegion mors;
    [SerializeField] MajorRegion nihil;

    private MajorRegion selectedRegion;

    private void Awake()
    {
        //change this
        TravelMenuController.currentRegion = altar;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //RefreshUI();
    }

    public void RefreshUI()
    {
        selectedRegion = TravelMenuController.currentRegion;

        if (selectedRegion == null) return;

        title.GetComponent<TextMeshProUGUI>().text = selectedRegion.regionName;

        int _i = 0;

        foreach (GameObject b in buttons)
        {
            Debug.Log($"Button {_i}: {selectedRegion.locations[_i]}");
            if (selectedRegion.locations[_i].isEmpty()) {
                b.SetActive(false);
            }
            else {
                b.SetActive(true);
                b.GetComponent<TravelAspect_Buttons>().Refresh(selectedRegion.locations[_i]);
            }
            _i++;
        }
    }
}
