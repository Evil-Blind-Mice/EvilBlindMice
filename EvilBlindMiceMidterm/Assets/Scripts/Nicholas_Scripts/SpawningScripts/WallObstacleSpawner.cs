using UnityEngine;

public class WallObstacleSpawner : MonoBehaviour
{

    [SerializeField] ChunkSectionSubSection subsection = ChunkSectionSubSection.floor;
    [SerializeField] GameObject lazerWall;
    [SerializeField] GameObject parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChunkV2Section chunkSection = parent.GetComponent<ChunkV2Section>();

        switch (subsection)
        {
            case ChunkSectionSubSection.floor:
                if (SpawnManager.Instance.SpawnWallObstacle(parent,
                    chunkSection.SectionNumber, ChunkSectionSubSection.floor))
                {
                    lazerWall.SetActive(true);
                    gameObject.SetActive(false);
                }
                break;
            case ChunkSectionSubSection.leftWall:
                if (SpawnManager.Instance.SpawnWallObstacle(parent,
                    chunkSection.SectionNumber, ChunkSectionSubSection.leftWall))
                {
                    lazerWall.SetActive(true);
                    gameObject.SetActive(false);
                }
                break;
            case ChunkSectionSubSection.rightWall:
                if (SpawnManager.Instance.SpawnWallObstacle(parent.gameObject,
                    chunkSection.SectionNumber, ChunkSectionSubSection.rightWall))
                {
                    lazerWall.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
                break;
            case ChunkSectionSubSection.cieling:
                if (SpawnManager.Instance.SpawnWallObstacle(parent.gameObject,
                    chunkSection.SectionNumber, ChunkSectionSubSection.cieling))
                {
                    lazerWall.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
