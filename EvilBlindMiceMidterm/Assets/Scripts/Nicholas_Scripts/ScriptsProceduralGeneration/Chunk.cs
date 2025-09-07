using System.Collections.Generic;
using UnityEngine;
using static PossiblePaths;

public class Chunk : MonoBehaviour
{
    public List<ProceduralConnectionPoint> connectionPoints;
    public List<GameObject> childChunkList = new List<GameObject>();
    public List<LSRPaths> possiblePaths;

    public int GetNextIndexFromPath(LSRPaths chosenPath)
    {
        for (int i = 0; i < possiblePaths.Count; i++) 
        {
            bool isPath = (possiblePaths[i] == chosenPath);
            if (isPath)
            {
                return i;
            }
        }
        return -1;
    }

    public List<int> GetOppositeIndeciesFromPath(LSRPaths chosenPath)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < possiblePaths.Count; i++)
        {
            bool isntPath = (possiblePaths[i] != chosenPath);
            if (isntPath)
            {
                result.Add(i);
            }
        }

        return result;
    }
}
