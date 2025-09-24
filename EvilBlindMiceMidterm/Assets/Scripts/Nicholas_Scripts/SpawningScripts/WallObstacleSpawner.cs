using UnityEngine;

public class WallObstacleSpawner : MonoBehaviour
{

    [Range(0, 3)][SerializeField] int FLRC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        switch (FLRC)
        {
            case 0:
                if (SpawnManager.Instance.SpawnWallObstacle(gameObject.transform.parent.gameObject, 
                    gameObject.transform.parent.gameObject.GetComponent<ChunkV2Section>().SectionNumber, ChunkSectionSubSection.floor))
                {
                    gameObject.transform.parent.gameObject.GetComponentInChildren<LazerFloor>().transform.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 1:
                if (SpawnManager.Instance.SpawnWallObstacle(gameObject.transform.parent.gameObject,
                    gameObject.transform.parent.gameObject.GetComponent<ChunkV2Section>().SectionNumber, ChunkSectionSubSection.leftWall))
                {
                    gameObject.transform.parent.gameObject.GetComponentInChildren<LazerLeftWall>().transform.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 2:
                if (SpawnManager.Instance.SpawnWallObstacle(gameObject.transform.parent.gameObject,
                    gameObject.transform.parent.gameObject.GetComponent<ChunkV2Section>().SectionNumber, ChunkSectionSubSection.rightWall))
                {
                    gameObject.transform.parent.gameObject.GetComponentInChildren<LazerRightWall>().transform.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
            case 3:
                if (SpawnManager.Instance.SpawnWallObstacle(gameObject.transform.parent.gameObject,
                    gameObject.transform.parent.gameObject.GetComponent<ChunkV2Section>().SectionNumber, ChunkSectionSubSection.cieling))
                {
                    gameObject.transform.parent.gameObject.GetComponentInChildren<LazerCieling>().transform.gameObject.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                break;
        }
    }
}
