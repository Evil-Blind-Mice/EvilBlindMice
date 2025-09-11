using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ObjectSpawner : MonoBehaviour
{

    // VARIABLES
    public enum SpawnType { onStart, external }
    [Header("Settings")]
    public SpawnType spawnType;

    [Header("Possible Objects")]
    [SerializeField] ObjectSpawnSettings[] spawnList;

    List<int> runtimeIDList;



    // RUNTIME

    void Start()
    {
        if (spawnType == SpawnType.onStart)
            Spawn();
    }

    void Spawn()
    {
        int randInt = Random.Range(0, runtimeIDList.Count);
        ObjectSpawnSettings toSpawn = spawnList[runtimeIDList[randInt]];
        Vector3 localOffset = toSpawn.spawnOffset.x * transform.right + toSpawn.spawnOffset.y * transform.up + toSpawn.spawnOffset.z * transform.forward;
        Instantiate(toSpawn.prefab, transform.position + localOffset, transform.rotation);
    }



    // EDITOR

    void InitializeLists()
    {
        runtimeIDList = new List<int>();

        for(int spawnIndex = 0; spawnIndex < spawnList.Length; spawnIndex++)
        {
            for (int count = spawnList[spawnIndex].probability; count > 0; count--)
                runtimeIDList.Add(spawnIndex);
        }
    }

    private void OnValidate()
    {
        InitializeLists();
    }
}

[System.Serializable]
public struct ObjectSpawnSettings
{
    public GameObject prefab;
    public int probability;
    public Vector3 spawnOffset;
}
