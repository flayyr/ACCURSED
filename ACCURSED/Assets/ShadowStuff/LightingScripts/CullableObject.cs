using UnityEngine;
using System.Collections.Generic;

public class CullableObject : MonoBehaviour
{
    [SerializeField] List<Component> componentsToDisable;

    bool isEnabled = true;

    private void Awake()
    {
        componentsToDisable = new List<Component>();
    }

    public void AddComponentToDisable(Component component)
    {
        componentsToDisable.Add(component);
    }

    private void Update()
    {
        Vector2 position = transform.position;
        Vector2 boundsBotLeft = LightManager.instance.boundsBotLeft;
        Vector2 boundsTopRight = LightManager.instance.boundsTopRight;

        if (position.x < boundsBotLeft.x || position.x > boundsTopRight.x || position.y < boundsBotLeft.y || position.y > boundsTopRight.y)
        {
            if (isEnabled)
            {
                SetVisibility(false);
                isEnabled = false;
            }
            return;
        }
        if (!isEnabled)
        {
            SetVisibility(true);
            isEnabled = true;
        }
    }

    private void SetVisibility(bool isEnabled)
    {
        for (int i = 0; i < componentsToDisable.Count; i++)
        {
            ComponentSetEnabled(componentsToDisable[i],isEnabled);
        }
    }

    private void ComponentSetEnabled(Component comp, bool isEnabled)
    {
        if (comp == null) return;

        if (comp is Behaviour behaviour)
        {
            behaviour.enabled = false;
        }
        else if (comp is Renderer renderer)
        {
            renderer.enabled = false;
        }
        else if (comp is Collider collider)
        {
            collider.enabled = false;
        }
        else if (comp is Collider2D collider2D)
        {
            collider2D.enabled = false;
        }
    }
}
