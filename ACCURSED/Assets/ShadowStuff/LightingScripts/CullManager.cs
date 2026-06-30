using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CullManager : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] readonly Vector2 worldMinBounds;
    [SerializeField] readonly Vector2 worldMaxBounds;
    [SerializeField] readonly Vector2 chunkSize;
    HashSet<CustomDynamicLit>[,] litObjects;

    Vector2 worldSize;

    int rowCount;
    int columnCount;

    int2 prevCamCoord;

    private void Awake()
    {
        worldSize = new Vector2(worldMaxBounds.x - worldMinBounds.x, worldMaxBounds.y - worldMinBounds.y);
        rowCount = Mathf.FloorToInt( worldSize.y / chunkSize.y);
        columnCount = Mathf.FloorToInt(worldSize.x / chunkSize.x);

        //may break if chunksize is divided by the bounds exactly
        litObjects = new HashSet<CustomDynamicLit>[columnCount+1, rowCount+1];

        for (int i = 0; i<rowCount; i++)
        {
            for(int j = 0; j < columnCount; j++)
            {
                litObjects[i, j] = new HashSet<CustomDynamicLit>();
            }
        }
    }

    private void Update()
    {
        int2 camCoord = PositionToChunkCoord(cam.transform.position);
        int2 difference = camCoord - prevCamCoord;
        if (difference.x != 0)
        {
            for (int i = -1; i <= 1; i++)
            {
                ObjectsSetEnabled(litObjects[prevCamCoord.x - difference.x, prevCamCoord.y + i], false);
            }
        }
        if (difference.y != 0)
        {
            for (int i = -1; i <= 1; i++)
            {
                ObjectsSetEnabled(litObjects[prevCamCoord.x +i, prevCamCoord.y - difference.y], false);
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
        }
    }

    int2 PositionToChunkCoord(Vector2 position)
    {
        Vector2 relativePos = position - worldMinBounds;
        int2 chunkCoord = new int2(Mathf.FloorToInt(relativePos.x/chunkSize.x), Mathf.FloorToInt(relativePos.y / chunkSize.y));
        return chunkCoord;
    }

    void ObjectsSetEnabled(HashSet<CustomDynamicLit> set, bool enabled)
    {
        foreach(CustomDynamicLit obj in set)
        {
            obj.enabled = enabled;
        }
    }
}
