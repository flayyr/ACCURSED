using UnityEngine;

public abstract class EscMenuUIButtonSO : ScriptableObject
{
    public Sprite buttonImage;
    public string buttonText;

    public abstract void Execute();
}
