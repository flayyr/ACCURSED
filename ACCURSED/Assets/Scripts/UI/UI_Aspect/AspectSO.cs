using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "AspectSO", menuName = "Scriptable Objects/Aspects", order = 1)]
public class AspectSO : InteractableItemSO
{
    public string locationName;
    //public string sceneName;
    //public string spawnID;
    //public Scene scene;

    public Vector3 position;
    public string sceneName;

    public override void Interact()
    {
        PersistentPlayer.Instance.gameObject.GetComponent<PlayerDeath>().SetRespawnAspect(this);
        AspectController.Instance.OpenMenu(this);
        if(EnemySpawnManager.Instance != null)
        {
            EnemySpawnManager.Instance.RespawnEnemies();
        }
    }

    public bool isEmpty()
    {
        if (locationName == string.Empty)
        {
            return true;
        }

        return false;
    }
}
