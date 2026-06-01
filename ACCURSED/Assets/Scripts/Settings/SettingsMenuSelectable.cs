using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenuSelectable : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Selection Visual")]
    public GameObject selectedPrefab;

    [Tooltip("Optional. If empty, visual spawns on this object's RectTransform.")]
    public RectTransform visualTargetOverride;

    public Vector2 visualSize = new Vector2(80f, 40f);
    public Vector2 visualOffset = Vector2.zero;

    protected SettingsMenuNavigator navigator;

    private GameObject visualInstance;
    private bool isSelected;

    public void SetNavigator(SettingsMenuNavigator newNavigator)
    {
        navigator = newNavigator;
    }

    public virtual void Activate()
    {
        
    }

    public virtual void SetSelected(bool selected)
    {
        if (isSelected == selected)
            return;

        isSelected = selected;

        if (isSelected)
            SpawnSelectedVisual();
        else
            RemoveSelectedVisual();
    }

    protected virtual RectTransform GetVisualTarget()
    {
        if (visualTargetOverride != null)
            return visualTargetOverride;

        return GetComponent<RectTransform>();
    }

    private void SpawnSelectedVisual()
    {
        RemoveSelectedVisual();

        if (selectedPrefab == null)
            return;

        RectTransform target = GetVisualTarget();

        if (target == null)
        {
            Debug.LogWarning("Selectable needs a RectTransform target: " + gameObject.name);
            return;
        }

        visualInstance = Instantiate(selectedPrefab, target);

        RectTransform visualRect = visualInstance.GetComponent<RectTransform>();

        if (visualRect == null)
        {
            Debug.LogWarning("Selected prefab needs a RectTransform: " + selectedPrefab.name);
            Destroy(visualInstance);
            visualInstance = null;
            return;
        }

        visualRect.anchorMin = new Vector2(0.5f, 0.5f);
        visualRect.anchorMax = new Vector2(0.5f, 0.5f);
        visualRect.pivot = new Vector2(0.5f, 0.5f);

        visualRect.anchoredPosition = visualOffset;
        visualRect.sizeDelta = visualSize;
        visualRect.localScale = Vector3.one;
        visualRect.localRotation = Quaternion.identity;

        visualRect.SetAsLastSibling();
    }

    private void RemoveSelectedVisual()
    {
        if (visualInstance != null)
        {
            Destroy(visualInstance);
            visualInstance = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (navigator != null)
            navigator.SelectOptionByMouse(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Activate();
    }

    protected virtual void OnDisable()
    {
        RemoveSelectedVisual();
    }
}