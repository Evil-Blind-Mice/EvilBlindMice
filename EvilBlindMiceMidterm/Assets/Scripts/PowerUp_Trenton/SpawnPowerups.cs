using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerups : MonoBehaviour
{
    [SerializeField] List<GameObject> powerups;
    [SerializeField] int indexToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indexToSpawn = Mathf.Clamp(indexToSpawn, 0, powerups.Count - 1);
        Instantiate(powerups[indexToSpawn], transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
