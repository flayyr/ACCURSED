using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("tall grass walk sfx")]
    [field: SerializeField] public EventReference tallGrassWalkSFX { get; private set; }
    [field: Header("foliage walk sfx")]
    [field: SerializeField] public EventReference foliageWalkSFX { get; private set; }
    [field: Header("water walk sfx")]
    [field: SerializeField] public EventReference waterWalkSFX { get; private set; }
    [field: Header("better water walk sfx")]
    [field: SerializeField] public EventReference betterWaterWalkSFX { get; private set; }
    [field: Header("dirt walk sfx")]
    [field: SerializeField] public EventReference dirtWalkSFX { get; private set; }
    [field: Header("grass walk sfx")]
    [field: SerializeField] public EventReference grassWalkSFX { get; private set; }
    [field: Header("leaves walk sfx")]
    [field: SerializeField] public EventReference leavesWalkSFX { get; private set; }
    [field: Header("sand walk sfx")]
    [field: SerializeField] public EventReference sandWalkSFX { get; private set; }
    [field: Header("stone walk sfx")]
    [field: SerializeField] public EventReference stoneWalkSFX { get; private set; }
    [field: Header("snow walk sfx")]
    [field: SerializeField] public EventReference snowWalkSFX { get; private set; }
    [field: Header("wood walk sfx")]
    [field: SerializeField] public EventReference woodWalkSFX { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("more than one FMOD events instance in scene");
        }
        instance = this;
    }

}
