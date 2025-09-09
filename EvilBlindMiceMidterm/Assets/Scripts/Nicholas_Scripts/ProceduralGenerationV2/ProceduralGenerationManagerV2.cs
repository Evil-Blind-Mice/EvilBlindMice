using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationManagerV2 : MonoBehaviour
{
    public static ProceduralGenerationManagerV2 Instance { get; private set; }

    [HideInInspector] GameObject firstSpawn;
    [HideInInspector] public ChunkV2 currentChunk;

    [SerializeField] GameObject initialChunkPrefab;
    [SerializeField] List<GameObject> availableChunkPool;
    [SerializeField] Vector3 generationStartingPosition = new(0, 0, 0);
    [SerializeField] Quaternion generationStartingRotation = new(0, 0, 0, 0);
    public int generationIterations = 1;

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

    private void Start()
    {
        GenerateFirstChunks();
    }

    //Generate the starting chunk
    private void GenerateFirstChunks()
    {
        firstSpawn = Instantiate(initialChunkPrefab, generationStartingPosition, generationStartingRotation);
        currentChunk = firstSpawn.GetComponent<ChunkV2>();
        currentChunk.SetInfo(0, availableChunkPool, generationIterations);

        for (int i = 0; i < generationIterations; i++)
        {
            GenerateTailChunks();
        }
    }

    //handle creation and destruction
    public void GenerateNextChunkSet(ChunkV2 _chunkToSave, int _destroyDelay = 0)
    {

        DestroyUnnessesaryChunks(_chunkToSave);
        UpdateFirstChunk(_chunkToSave, _destroyDelay);
        UpdateChunkIterations();
        FillInTheGaps();
        GenerateTailChunks();
    }

    //generate the next generation of chunks
    void GenerateTailChunks()
    {
        List<ChunkV2> tailChunks = new();
        FindTailChunks(currentChunk, ref tailChunks);

        for (int i = 0;i < tailChunks.Count; i++)
        {
            tailChunks[i].CreateChildChunks();
        }
    }

    void DestroyUnnessesaryChunks(ChunkV2 childToSave)
    {
        currentChunk.DestroyChildChunks(childToSave);
    }

    void UpdateFirstChunk(ChunkV2 newCurrentChunk, int _destroyDelay)
    {
        Destroy(currentChunk, _destroyDelay);
        currentChunk = newCurrentChunk;
    }

    void FillInTheGaps()
    {
        for (int i = 0; i < currentChunk.GetChildList().Count; i++)
        {
            currentChunk.GetChildList()[i].AttemptCreateMissingChildren();
        }
    }

    void UpdateChunkIterations()
    {
        currentChunk.UpdateIterations();
    }

    void FindTailChunks(ChunkV2 _startingChunk, ref List<ChunkV2> listOfTailChunks)
    {
        if (_startingChunk.GetChildList().Count == 0)
        {
            listOfTailChunks.Add(_startingChunk);
            return;
        }

        for (int i = 0; i < _startingChunk.GetChildList().Count; i++)
        {
            FindTailChunks(_startingChunk.GetChildList()[i], ref listOfTailChunks);
        }
    }
}
