using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerups : MonoBehaviour
{
    [SerializeField] List<GameObject> powerups;
    [SerializeField] int indexToSpawn;
    GameObject powerUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indexToSpawn = Mathf.Clamp(indexToSpawn, 0, powerups.Count - 1);
        powerUp = Instantiate(powerups[indexToSpawn], transform.position, Quaternion.identity);
        powerUp.transform.localScale = new Vector3(3, 3, 3);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
