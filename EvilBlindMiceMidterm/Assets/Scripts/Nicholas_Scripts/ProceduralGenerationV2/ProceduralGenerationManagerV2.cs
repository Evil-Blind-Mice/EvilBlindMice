using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] Vector3 generationStartingScale = new(1, 1, 1);
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
        firstSpawn.transform.localScale = generationStartingScale;
        currentChunk = firstSpawn.GetComponent<ChunkV2>();
        currentChunk.SetInfo(0, availableChunkPool, generationIterations);

        for (int i = 0; i < generationIterations; i++)
        {
            GenerateTailChunks();
        }
    }

    public void GenerateNextChunkSet(ChunkV2 _chunkToSave, float _destroyDelay = 0)
    {
        StartCoroutine(GenerateNextChunkSetCouroutine(_chunkToSave, _destroyDelay));
    }

    //handle creation and destruction
    private IEnumerator GenerateNextChunkSetCouroutine(ChunkV2 _chunkToSave, float _destroyDelay = 0)
    {
        if (_chunkToSave != null)
        {
            DestroyUnnessesaryChunks(_chunkToSave);
            UpdateFirstChunk(_chunkToSave, _destroyDelay);
        }
        yield return new WaitForSeconds(0.1f);
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

    void UpdateFirstChunk(ChunkV2 newCurrentChunk, float _destroyDelay)
    {
        Destroy(currentChunk.gameObject, _destroyDelay);
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
