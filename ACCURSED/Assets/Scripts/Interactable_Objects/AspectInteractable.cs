using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Aspect Interactable")]
public class AspectInteractable : InteractableItemSO
{
    public override void Interact()
    {
        AspectController.Instance.OpenMenu();
    }
}
