using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Scriptable Objects/TutorialSO")]
public class TutorialSO : ScriptableObject
{
    public string title;
    public string instructions;
    public Sprite image;
}
