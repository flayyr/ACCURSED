using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface ICullable { 
    void SetVisibility(bool visible);
    int2 GetSize();
}

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

    int cullDistThreshold = 1;

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
        Queue<int2> toDisableCoords = new Queue<int2>();
        Queue<int2> toEnableCoords = new Queue<int2>();

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                bool targetEnabledStatus = math.abs(camCoord.x - i) <= cullDistThreshold && math.abs(camCoord.y - j) <= cullDistThreshold;

                if (targetEnabledStatus != chunksIsEnabled[i,j])
                {
                    if (targetEnabledStatus)
                        toEnableCoords.Enqueue(new int2(i,j));
                    else
                        toDisableCoords.Enqueue(new int2(i,j));
                }
            }
        }

        //enable after disabling for large objects that occupy multiple coords
        while (toDisableCoords.Count > 0) {
            int2 curr = toDisableCoords.Dequeue();
            ObjectsSetEnabledAll(litObjects[curr.x, curr.y], false);
            chunksIsEnabled[curr.x, curr.y] = false;
        }
        while (toEnableCoords.Count > 0) {
            int2 curr = toEnableCoords.Dequeue();
            ObjectsSetEnabledAll(litObjects[curr.x, curr.y], true);
            chunksIsEnabled[curr.x, curr.y] = true;
        }
    }

    public void AddObject(ICullable obj, Vector2 position)
    {
        int2 chunkCoord = PositionToChunkCoord(position);
        int2 chunkSize = obj.GetSize() - new int2(1,1);
        if(chunkSize.x<0 || chunkSize.y < 0) {
            Debug.LogError("culling chunk size not set to be equal or greater than 1!");
        }

        bool shouldBeEnabled = false;
        for (int i = -chunkSize.x; i <= chunkSize.x; i++) {
            for (int j = -chunkSize.y; j <= chunkSize.y; j++) {
                int2 currCoord = new int2(chunkCoord.x + i, chunkCoord.y + j);

                if (currCoord.x < 0 || currCoord.y < 0 || currCoord.x>=columnCount || currCoord.y>=rowCount)
                    continue;

                litObjects[currCoord.x, currCoord.y].Add(obj);
                if (chunksIsEnabled[currCoord.x, currCoord.y]) {
                    shouldBeEnabled = true;
                }
            }
        }

        ObjectSetEnabled(obj, shouldBeEnabled);
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
    }

    public void SetCullDist(int dist) {
        cullDistThreshold = dist;
    }
    public void ResetCullDist() {
        cullDistThreshold = 1;
    }
}
