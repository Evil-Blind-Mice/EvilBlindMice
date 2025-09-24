using UnityEngine;

public class WallObstacleSpawner : MonoBehaviour
{

    [Range(0, 3)][SerializeField] int FLRC;
    [SerializeField] GameObject lazerWall;
    [SerializeField] GameObject parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChunkV2Section chunkSection = parent.GetComponent<ChunkV2Section>();

        switch (FLRC)
        {
            case 0:
                if (SpawnManager.Instance.SpawnWallObstacle(parent,
                    chunkSection.SectionNumber, ChunkSectionSubSection.floor))
                {
                    lazerWall.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 1:
                if (SpawnManager.Instance.SpawnWallObstacle(parent,
                    chunkSection.SectionNumber, ChunkSectionSubSection.leftWall))
                {
                    lazerWall.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 2:
                if (SpawnManager.Instance.SpawnWallObstacle(parent.gameObject,
                    chunkSection.SectionNumber, ChunkSectionSubSection.rightWall))
                {
                    lazerWall.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 3:
                if (SpawnManager.Instance.SpawnWallObstacle(parent.gameObject,
                    chunkSection.SectionNumber, ChunkSectionSubSection.cieling))
                {
                    lazerWall.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
        }
    }
}
