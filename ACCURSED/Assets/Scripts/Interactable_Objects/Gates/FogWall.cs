using UnityEngine;

public class FogWall : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch[] statues;
    private bool unlocked;

    private void Start() {
        unlocked = true;
        foreach (var statue in statues) {
            statue.OnStateChanged += CheckStatues;
        }
    }

    private void OnDestroy() {
        foreach (var statue in statues) {
            statue.OnStateChanged -= CheckStatues;
        }
    }

    private void CheckStatues() {
        foreach (var statue in statues) {
            if (!statue.IsActivated) {
                return;
            }
        }

        unlocked = true;
        OpenWall();
    }

    private void OpenWall() {
        Destroy(gameObject);
    }
}
