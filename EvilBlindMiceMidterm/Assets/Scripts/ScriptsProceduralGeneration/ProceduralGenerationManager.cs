using System.Collections.Generic;
using UnityEngine;
using static ProceduralConnectionPoint;
using static NESWDirections;
using static Chunk;
using static PossiblePaths;
using Unity.VisualScripting;

public class ProceduralGenerationManager : MonoBehaviour
{
    //Create singleton pattern
    public static ProceduralGenerationManager Instance { get; private set; }

    //Preset starting chunk, chunks to generate, and starting position of the first chunk
    [SerializeField] GameObject startChunk;
    public List<GameObject> prefabChunks;
    public Vector3 startingPosition;

    //Variables to handle chunk memory
    GameObject currentChunk;
    List<List<GameObject>> generationList;
    public bool nextCheckpointFlagged = false;
    public LSRPaths nextPathChoice;

    //Variable to change the number of procedurally generated elements
    public int chunksLevelsToGenerate;

    //Set up singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateInitialChunks();
    }

    //Function to generate the level
    void GenerateInitialChunks()
    {
        //create the list of lists of chunks
        generationList = new List<List<GameObject>>();
        Quaternion prefabRotation;
        Vector3 prefabLocation;

        for (int i = 0; i <= chunksLevelsToGenerate; i++)
        {
            generationList.Add(new List<GameObject>());
        }

        //Generate the first chunk and set the first connection points
        currentChunk = Instantiate(startChunk, startingPosition, Quaternion.identity);
        generationList[0].Add(currentChunk);

        //Generate the rest of the chunks
        for (int i = 0; i < chunksLevelsToGenerate; i++)
        {
            for (int j = 0; j < generationList[i].Count; j++) 
            {
                int numberOfConnections = generationList[i][j].GetComponentInChildren<Chunk>().connectionPoints.Count;
                for (int k = 0; k < numberOfConnections; k++)
                {
                    prefabRotation = PrefabDirection(generationList[i][j].GetComponentInChildren<Chunk>().connectionPoints[k].direction);
                    prefabLocation = generationList[i][j].GetComponentInChildren<Chunk>().connectionPoints[k].connectionPoint;

                    GameObject nextChunkPrefab = prefabChunks[Random.Range(0, prefabChunks.Count)];
                    GameObject nextChunkObject = Instantiate(nextChunkPrefab, prefabLocation, prefabRotation);
                    generationList[i][j].GetComponentInChildren<Chunk>().childChunkList.Add(nextChunkObject);
                    generationList[i + 1].Add(nextChunkObject);
                }
            }
        }
    }

    void NextGeneration()
    {
        GameObject tempChunk = currentChunk.GetComponentInChildren<Chunk>().childChunkList[currentChunk.GetComponentInChildren<Chunk>().GetNextIndexFromPath(nextPathChoice)];
        List<int> unusedPathIndecies;
        unusedPathIndecies = currentChunk.GetComponentInChildren<Chunk>().GetOppositeIndeciesFromPath(nextPathChoice);
        List<GameObject> tempList;

        //Creating variables to reduce variables created through loop
        Quaternion nextChunkRotation;
        Vector3 nextChunkLocation;

        tempList = generationList[0];
        generationList.RemoveAt(0);
        generationList.Add(new List<GameObject>());

        //Destroy Unused Paths
        if (unusedPathIndecies.Count > 0)
        {
            for (int i = unusedPathIndecies.Count - 1; i >= 0; i--)
            {
                int indexToDestroy = unusedPathIndecies[i];
                if (indexToDestroy != -1)
                {
                    GameObject pathDegenerate = currentChunk.GetComponentInChildren<Chunk>().childChunkList[indexToDestroy];
                    DestroyPaths(pathDegenerate);
                }
            }
        }

        currentChunk = tempChunk;
        Destroy(tempList[0]);

        //Generate new objects
        for (int i = 0; i < generationList[chunksLevelsToGenerate - 1].Count; i++)
        {
            int numberOfConnections = generationList[chunksLevelsToGenerate - 1][i].GetComponentInChildren<Chunk>().connectionPoints.Count;
            for (int j = 0; j < numberOfConnections; j++)
            {
                nextChunkRotation = PrefabDirection(generationList[chunksLevelsToGenerate - 1][i].GetComponentInChildren<Chunk>().connectionPoints[j].direction);
                nextChunkLocation = generationList[chunksLevelsToGenerate - 1][i].GetComponentInChildren<Chunk>().connectionPoints[j].connectionPoint;

                GameObject nextChunkPrefab = prefabChunks[Random.Range(0, prefabChunks.Count)];
                GameObject nextChunkObject = Instantiate(nextChunkPrefab, nextChunkLocation, nextChunkRotation);
                generationList[chunksLevelsToGenerate - 1][i].GetComponentInChildren<Chunk>().childChunkList.Add(nextChunkObject);
                generationList[chunksLevelsToGenerate].Add(nextChunkObject);
            }
        }
    }

    //Check for checkpoint flag, then run next generation
    void Update()
    {
        if (nextCheckpointFlagged)
        {
            NextGeneration();
            nextCheckpointFlagged = false;
        }
    }

    //Create quaternions based on direction
    Quaternion PrefabDirection(FacingDirection connectionDirection)
    {
        if (connectionDirection == FacingDirection.North)
        {
            return Quaternion.Euler(0, 270, 0);
        }
        else if (connectionDirection == FacingDirection.East)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        else if (connectionDirection == FacingDirection.South)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        else if (connectionDirection == FacingDirection.West)
        {
            return Quaternion.Euler(0, 180, 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    //Function to handle destroying paths
    void DestroyPaths(GameObject baseObject)
    {
        if (baseObject == null)
        {
            return;
        }

        List<GameObject> childList = baseObject.GetComponentInChildren<Chunk>().childChunkList;

        for (int i = childList.Count - 1; i >= 0; i--)
        {
            DestroyPaths(childList[i]);
        }

        for (int i = 0; i < generationList.Count; i++)
        {
            generationList[i].Remove(baseObject);
        }

        Destroy(baseObject);
    }
}
