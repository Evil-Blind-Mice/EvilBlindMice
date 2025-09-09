using System.Collections.Generic;
using UnityEngine;

public class ChunkV2 : MonoBehaviour
{
    int iteration = 0;
    [SerializeField] List<ChunkConnectionPoint> connectionPoints;
    List<ChunkV2> childChunkList = new();
    ChunkV2 parent;
    List<GameObject> chunkPool;
    int generationIterations;

    public void SetInfo(int _iteration, List<GameObject> _chunkPool, int _generationIterations, ChunkV2 _parent = null)
    {
        iteration = _iteration;
        chunkPool = _chunkPool;
        generationIterations = _generationIterations;
        parent = _parent;
    }

    public void CreateChildChunks()
    {
        
        List<int> safeSpawnIndex = CheckOverlap();
        for (int j = 0; j < safeSpawnIndex.Count; j++)
        {
            CreateChunk(safeSpawnIndex[j]);
        }
    }

    public void DestroyChildChunks(ChunkV2 _childToSave)
    {
        if (childChunkList.Count == 0)
        {
            parent.childChunkList.Remove(this);
            Destroy(gameObject);
            return;
        }

        if (_childToSave == this)
        {
            return;
        }

        for (int i = childChunkList.Count - 1; i >= 0; i--)
        {
            childChunkList[i].DestroyChildChunks(_childToSave);
        }
    }

    public void AttemptCreateMissingChildren()
    {
        List<int> safeSpawnIndex = CheckOverlap();
        for (int i = 0; i < safeSpawnIndex.Count; i++)
        {
            CreateChunk(safeSpawnIndex[i]);
        }

        if (iteration < generationIterations - 1)
        {
            for (int i = 0; i < childChunkList.Count; i++)
            {
                childChunkList[i].AttemptCreateMissingChildren();
            }
        }
    }

    void CreateChunk(int connectionPoint)
    {
        GameObject childChunk = Instantiate(chunkPool[Random.Range(0, chunkPool.Count)],
                connectionPoints[connectionPoint].position, connectionPoints[connectionPoint].rotation);
        childChunk.GetComponent<ChunkV2>().SetInfo(iteration + 1, chunkPool, generationIterations, this);
        connectionPoints[connectionPoint].SetChild(childChunk.GetComponent<ChunkV2>());
        childChunkList.Add(childChunk.GetComponent<ChunkV2>());
    }

    public void UpdateIterations()
    {
        for (int i = 0; i < childChunkList.Count; i++)
        {
            childChunkList[i].UpdateIterations();
        }

        iteration--;
    }

    public List<ChunkV2> GetChildList()
    {
        return childChunkList;
    }

    private List<int> CheckOverlap()
    {
        List<int> safeSpawnIndex = new();
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            RaycastHit hit;
            if (!Physics.Raycast(connectionPoints[i].position + (connectionPoints[i].forwardAxis * 1)
                + (connectionPoints[i].upAxis * 1), connectionPoints[i].rightAxis, out hit, 60))
            {
                safeSpawnIndex.Add(i);
            }
        }
        return safeSpawnIndex;
    }
}
