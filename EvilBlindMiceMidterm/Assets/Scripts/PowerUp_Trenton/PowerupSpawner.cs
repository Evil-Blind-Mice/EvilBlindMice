using UnityEngine;
using System.Collections.Generic;

public class PowerupSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] List<GameObject> powerupsToSpawn;
    [SerializeField] int numberToSpawn;
    [SerializeField] int spawnRate;

    [Header("Spawn Positions")]
    [SerializeField] Transform[] spawnAnchors;
    [SerializeField] float spawnRadius;
    [SerializeField] float castHeight;
    [SerializeField] float surfaceGap;

    [Header("Surface Snapping")]
    [SerializeField] LayerMask surface;
    [SerializeField] bool alignToSurface;

    float spawnTimer;
    int spawnCount;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnCount < numberToSpawn && spawnTimer >= spawnRate)
        {
            SpawnPowerup();
            spawnTimer = 0;
        }
    }

    void SpawnPowerup()
    {
        GameObject powerup = powerupsToSpawn[Random.Range(0, powerupsToSpawn.Count)];
        Transform anchor = spawnAnchors[Random.Range(0, spawnAnchors.Length)];

        Vector3 localScatter = anchor.right * Random.Range(-spawnRadius, spawnRadius) + anchor.forward * Random.Range(-spawnRadius, spawnRadius);
        Vector3 castStart = anchor.position + localScatter + anchor.up * castHeight;
        Vector3 castDir = -anchor.up;

        if (Physics.Raycast(castStart, castDir, out RaycastHit hit, castHeight * 4, surface, QueryTriggerInteraction.Ignore))
        {
            Vector3 up = hit.normal;
            Vector3 position = hit.point + up * surfaceGap;

            float powerupRotation = powerup.transform.localEulerAngles.z;

            GameObject instance = Instantiate(powerup, position, powerup.transform.rotation);

            if (alignToSurface)
            {
                Quaternion align = Quaternion.FromToRotation(instance.transform.up, up);
                instance.transform.Rotate(up, powerupRotation, Space.World);
            }

            spawnCount++;
        }
    }
}
