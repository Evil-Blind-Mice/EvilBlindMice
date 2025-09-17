using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkV2 : MonoBehaviour
{
    int iteration = 0;
    [SerializeField] List<ChunkConnectionPoint> connectionPoints;
    [SerializeField] LayerMask floorLayer;
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
        if (_childToSave.gameObject == this.gameObject)
        {
            return;
        }

        for (int i = childChunkList.Count - 1; i >= 0; i--)
        {
            childChunkList[i].DestroyChildChunks(_childToSave);
        }

        if (childChunkList.Count == 0)
        {
            if (parent.gameObject != null)
            {
                parent.childChunkList.Remove(this);
            }
            Destroy(gameObject);
            return;
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
        childChunk.transform.localScale = connectionPoints[connectionPoint].transform.localScale;
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
            float increment = 12 * connectionPoints[i].transform.localScale.z + 2;
            int found = 0;
            for (int j = 0; j < 8; j++)
            {
                if (Physics.OverlapSphere(connectionPoints[i].transform.position + connectionPoints[i].transform.forward * increment, 6f * connectionPoints[i].transform.localScale.z).Length > 0)
                {
                    found++;
                    Debug.Log("Overlapped");
                }
                increment += 12f * connectionPoints[i].transform.localScale.z;
            }
            if (found == 0)
            {
                safeSpawnIndex.Add(i);
            }
        }
        return safeSpawnIndex;
    }
}
