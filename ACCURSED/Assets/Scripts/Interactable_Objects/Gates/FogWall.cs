using UnityEngine;

public class FogWall : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch[] statues;
    private bool open;

    private void Start() {
        foreach (var statue in statues) {
            statue.OnStateChanged += CheckStatues;
        }
        CheckStatues();
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

        open = true;
        OpenWall();
    }

    private void OpenWall() {
        Destroy(gameObject);
    }
}
