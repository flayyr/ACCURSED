using UnityEngine;

public class Footbridge : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch statue;
    [SerializeField] SpriteRenderer[] repairedSprites;
    [SerializeField] GameObject[] brokenSprites;

    private void Start()
    {
        statue.OnStateChanged += CheckRepaired;
    }

    private void OnDestroy()
    {
        statue.OnStateChanged -= CheckRepaired;
    }

    private void CheckRepaired()
    {
        if (statue.IsActivated)
        {
            foreach (var sprite in repairedSprites)
            {
                sprite.gameObject.SetActive(true);
            }
            foreach (var sprite in brokenSprites)
            {
                sprite.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var sprite in repairedSprites)
            {
                sprite.gameObject.SetActive(false);
            }
            foreach (var sprite in brokenSprites)
            {
                sprite.gameObject.SetActive(true);
            }
        }
    }
}
