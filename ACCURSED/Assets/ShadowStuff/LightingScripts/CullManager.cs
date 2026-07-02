using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CullManager : MonoBehaviour
{
    public static CullManager instance;

    [SerializeField] Camera cam;
    [SerializeField] Vector2 worldMinBounds;
    [SerializeField] Vector2 worldMaxBounds;
    [SerializeField] Vector2 chunkSize;
    HashSet<CustomDynamicLit>[,] litObjects;

    Vector2 worldSize;

    int rowCount;
    int columnCount;

    int2 prevCamCoord;

    private void Awake()
    {
        instance = this;

        worldSize = new Vector2(worldMaxBounds.x - worldMinBounds.x, worldMaxBounds.y - worldMinBounds.y);

        columnCount = Mathf.CeilToInt(worldSize.x / chunkSize.x);
        rowCount = Mathf.CeilToInt( worldSize.y / chunkSize.y);

        litObjects = new HashSet<CustomDynamicLit>[columnCount, rowCount];

        for (int i = 0; i<columnCount; i++)
        {
            for(int j = 0; j < rowCount; j++)
            {
                litObjects[i, j] = new HashSet<CustomDynamicLit>();
            }
        }

        prevCamCoord = PositionToChunkCoord(cam.transform.position);
    }

    private void Update()
    {
        int2 camCoord = PositionToChunkCoord(cam.transform.position);
        int2 difference = camCoord - prevCamCoord;
        if (difference.x != 0)
        {
            for (int i = -1; i <= 1; i++)
            {
                ObjectsSetEnabledAll(litObjects[prevCamCoord.x - difference.x, prevCamCoord.y + i], false);
                ObjectsSetEnabledAll(litObjects[camCoord.x + difference.x, camCoord.y + i], true);
            }
        }
        if (difference.y != 0)
        {
            for (int i = -1; i <= 1; i++)
            {
                ObjectsSetEnabledAll(litObjects[prevCamCoord.x +i, prevCamCoord.y - difference.y], false);
                ObjectsSetEnabledAll(litObjects[camCoord.x + i, camCoord.y + difference.y], true);
            }
        }
        prevCamCoord = camCoord;
    }

    public void AddObject(CustomDynamicLit obj, Vector2 position)
    {
        int2 chunkCoord = PositionToChunkCoord(position);
        if (chunkCoord.x > 0 && chunkCoord.x<columnCount && chunkCoord.y > 0 && chunkCoord.y<rowCount)
        {
            litObjects[chunkCoord.x, chunkCoord.y].Add(obj);

            int2 difference = chunkCoord - prevCamCoord;
            if(math.abs(difference.x)>1 || math.abs(difference.y) > 1)
            {
                ObjectSetEnabled(obj, false);
            }
        }
    }

    int2 PositionToChunkCoord(Vector2 position)
    {
        Vector2 relativePos = position - worldMinBounds;
        int2 chunkCoord = new int2(Mathf.FloorToInt(relativePos.x/chunkSize.x), Mathf.FloorToInt(relativePos.y / chunkSize.y));
        return chunkCoord;
    }

    void ObjectsSetEnabledAll(HashSet<CustomDynamicLit> set, bool enabled)
    {
        foreach(CustomDynamicLit obj in set)
        {
            ObjectSetEnabled(obj, enabled);
        }
    }

    void ObjectSetEnabled(CustomDynamicLit obj, bool enabled)
    {
        obj.SetVisibility(enabled);
        obj.enabled = enabled;
        obj.gameObject.SetActive(enabled);
    }
}
