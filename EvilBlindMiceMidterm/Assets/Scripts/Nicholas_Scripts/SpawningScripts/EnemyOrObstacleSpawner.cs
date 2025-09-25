using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrObstacleSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToSpawn;
    [SerializeField] GameObject parent;
    [SerializeField] ChunkSectionSubSection subSection = ChunkSectionSubSection.floor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CheckCanSpawn());
    }

    IEnumerator CheckCanSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        if (SpawnManager.Instance.SpawnEnemyOrObstacle(parent, parent.GetComponent<ChunkV2Section>().SectionNumber, subSection))
        {
            Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Count - 1)], gameObject.transform.position, gameObject.transform.rotation, parent.transform);
        }
        
    }
}
