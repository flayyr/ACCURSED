using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "AspectSO", menuName = "Scriptable Objects/Aspects", order = 1)]
public class AspectSO : ScriptableObject
{
    public string locationName;
    //public string sceneName;
    //public string spawnID;
    //public Scene scene;

    public Vector3 position;
    public string sceneName;

    public bool isEmpty()
    {
        if (locationName == string.Empty)
        {
            return true;
        }

        return false;
    }
}
