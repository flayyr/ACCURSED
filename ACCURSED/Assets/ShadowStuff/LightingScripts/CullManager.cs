using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface ICullable { void SetVisibility(bool visible); }

public class CullManager : MonoBehaviour
{
    public static CullManager instance;

    [SerializeField] Camera cam;
    [SerializeField] Vector2 worldMinBounds;
    [SerializeField] Vector2 worldMaxBounds;
    [SerializeField] Vector2 chunkSize;
    HashSet<ICullable>[,] litObjects;
    bool[,] chunksIsEnabled;

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

        litObjects = new HashSet<ICullable>[columnCount, rowCount];
        chunksIsEnabled = new bool[columnCount, rowCount];

        for (int i = 0; i<columnCount; i++)
        {
            for(int j = 0; j < rowCount; j++)
            {
                litObjects[i, j] = new HashSet<ICullable>();
            }
        }

        prevCamCoord = PositionToChunkCoord(cam.transform.position);
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                chunksIsEnabled[prevCamCoord.x + i, prevCamCoord.y + j] = true;
            }
        }
    }

    private void Update()
    {
        int2 camCoord = PositionToChunkCoord(cam.transform.position);
        int2 difference = camCoord - prevCamCoord;

        if (difference.x != 0 || difference.y != 0)
        {
            UpdateCulling(camCoord);
            prevCamCoord = camCoord;
        }
    }
    
    void UpdateCulling(int2 camCoord)
    {
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                bool targetEnabledStatus = math.abs(camCoord.x - i) <= 1 && math.abs(camCoord.y - j) <= 1;

                if (targetEnabledStatus != chunksIsEnabled[i,j])
                {
                    ObjectsSetEnabledAll(litObjects[i, j], targetEnabledStatus);
                    chunksIsEnabled[i, j] = targetEnabledStatus;
                }
            }
        }
    }

    public void AddObject(ICullable obj, Vector2 position)
    {
        int2 chunkCoord = PositionToChunkCoord(position);
        //if (chunkCoord.x > 0 && chunkCoord.x<columnCount && chunkCoord.y > 0 && chunkCoord.y<rowCount)
        //{
            litObjects[chunkCoord.x, chunkCoord.y].Add(obj);

            ObjectSetEnabled(obj, chunksIsEnabled[chunkCoord.x, chunkCoord.y]);
        //}
    }
    public void RemoveObject(ICullable obj, Vector2 position) {
        int2 chunkCoord = PositionToChunkCoord(position);
        litObjects[chunkCoord.x, chunkCoord.y].Remove(obj);
    }

    int2 PositionToChunkCoord(Vector2 position)
    {
        Vector2 relativePos = position - worldMinBounds;
        int2 chunkCoord = new int2(Mathf.FloorToInt(relativePos.x/chunkSize.x), Mathf.FloorToInt(relativePos.y / chunkSize.y));
        return chunkCoord;
    }

    

    void ObjectsSetEnabledAll(HashSet<ICullable> set, bool enabled)
    {
        //set.RemoveWhere(item => item != null); //commented out for possible performance issues
        foreach (ICullable obj in set)
        {
            ObjectSetEnabled(obj, enabled);
        }
    }

    void ObjectSetEnabled(ICullable obj, bool enabled)
    {
        obj.SetVisibility(enabled);
        //obj.enabled = enabled;
        //obj.gameObject.SetActive(enabled);
    }
}
