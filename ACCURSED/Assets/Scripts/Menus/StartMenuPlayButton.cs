using Unity.VectorGraphics;
using UnityEngine;

public class StartMenuPlayButton : MonoBehaviour
{
    [SerializeField] private GameObject tribute;
    private Transform tributeTransform;


    public void Execute()
    {
        tributeTransform = tribute.transform;
        RoomTransitionManager.Instance.BeginTransition("Altar", "Start", tributeTransform);
    }
}
