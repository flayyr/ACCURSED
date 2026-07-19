using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "AspectSO", menuName = "Scriptable Objects/Aspects", order = 1)]
public class Aspect : ScriptableObject
{
    public string locationName;
    public GameObject aspectObj;
    //public Scene scene;
}
