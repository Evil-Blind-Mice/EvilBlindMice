using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PowerupChance
{
    public GameObject powerup;
    [Range(0, 100)] public int percent;
}

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] GameObject parent;

    [Header("Spawn Settings")]
    [SerializeField] List<PowerupChance> powerupsToSpawn;
    [SerializeField] int numberToSpawn;
    [SerializeField] float spawnRate;
    [SerializeField] bool noDuplicates;

    [Header("Spawn Positions")]
    [SerializeField] Transform[] spawnAnchors;
    [SerializeField] float spawnRadius;
    [SerializeField] float castHeight;
    [SerializeField] float surfaceGap;

    [Header("Surface Snapping")]
    [SerializeField] LayerMask surface;
    [SerializeField] bool alignToSurface;

    int spawnCount;
    List<int> availableAnchorIndices;
    HashSet<int> usedPowerupIndices = new();

    void Awake()
    {
        RebuildAvailableAnchors();
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
            RebuildAvailableAnchors();
    }

    void Update()
    {
        if (spawnAnchors == null || spawnAnchors.Length == 0) return;
        if (powerupsToSpawn == null || powerupsToSpawn.Count == 0) return;
        if (spawnCount >= numberToSpawn) return;

        if (parent.GetComponent<ChunkV2Section>().SectionNumber == 3 || parent.GetComponent<ChunkV2Section>().SectionNumber == 6)
            SpawnPowerup();
    }

    void RebuildAvailableAnchors()
    {
        if (availableAnchorIndices == null)
            availableAnchorIndices = new List<int>();
        else
            availableAnchorIndices.Clear();

        if (spawnAnchors == null) return;
        for (int anchorIndex = 0;  anchorIndex < spawnAnchors.Length; anchorIndex++)
            if (spawnAnchors[anchorIndex] != null)
                availableAnchorIndices.Add(anchorIndex);
    }

    void SpawnPowerup()
    {
        // Pick anchor
        int anchorIndex = PickAnchorIndex();
        if (anchorIndex < 0) return;

        Transform anchor = spawnAnchors[anchorIndex];

        // Scatter on anchor's local plane and raycast along -Up
        Vector3 localScatter = anchor.right * Random.Range(-spawnRadius, spawnRadius) + anchor.forward * Random.Range(-spawnRadius, spawnRadius);
        Vector3 castStart = anchor.position + localScatter + anchor.up * castHeight;
        Vector3 castDir = -anchor.up;

        // No surface
        if (!Physics.Raycast(castStart, castDir, out RaycastHit hitInfo, castHeight * 4, surface, QueryTriggerInteraction.Ignore)) return;

        // Roll which powerup to spawn
        int chosenPowerupIndex = PickByPercent(noDuplicates);
        if (chosenPowerupIndex < 0) return;

        GameObject powerup = powerupsToSpawn[chosenPowerupIndex].powerup;
        if (!powerup) return;

        // Place and orient the powerup
        Vector3 surfaceNormal = hitInfo.normal;
        Vector3 spawnPosition = hitInfo.point + surfaceNormal * surfaceGap;

        float powerupRotation = powerup.transform.localEulerAngles.z;
        GameObject instance = Instantiate(powerup, spawnPosition, powerup.transform.rotation, parent.transform);

        if (alignToSurface)
        {
            Vector3 surfaceUp = surfaceNormal;
            Vector3 surfaceforward = Vector3.ProjectOnPlane(anchor.forward, surfaceUp).normalized;
            if (surfaceforward.sqrMagnitude < 1e-4f)
                surfaceforward = Vector3.ProjectOnPlane(anchor.right, surfaceUp).normalized;

            Quaternion alignRotation = Quaternion.FromToRotation(instance.transform.up, surfaceNormal);
            instance.transform.rotation = alignRotation * instance.transform.rotation;
            instance.transform.Rotate(surfaceNormal, powerupRotation, Space.World);
        }

        usedPowerupIndices.Add(chosenPowerupIndex);
        RetireAnchor(anchorIndex);
        spawnCount++;
    }

    int PickAnchorIndex()
    {
        if (availableAnchorIndices == null || availableAnchorIndices.Count == 0) return -1;
        int bagIndex = Random.Range(0, availableAnchorIndices.Count);
        return availableAnchorIndices[bagIndex];
    }

    void RetireAnchor(int _anchorIndex)
    {
        if (availableAnchorIndices == null) return;
        int listIndex = availableAnchorIndices.IndexOf(_anchorIndex);
        if (listIndex >= 0)
            availableAnchorIndices.RemoveAt(listIndex);
    }

    // Return index into powerupsToSpawn, or -1 for nothing
    int PickByPercent(bool _enforceUnique)
    {
        if (powerupsToSpawn == null || powerupsToSpawn.Count == 0) return -1;

        float randomRollPercent = Random.Range(0, 100);
        float cumulativeActivePercent = 0;

        for (int powerupIndex = 0; powerupIndex < powerupsToSpawn.Count; powerupIndex++)
        {
            if (_enforceUnique && usedPowerupIndices.Contains(powerupIndex)) continue;

            float entryPercent = Mathf.Clamp(powerupsToSpawn[powerupIndex].percent, 0, 100);
            if (entryPercent <= 0) continue;

            cumulativeActivePercent += entryPercent;

            if (randomRollPercent < cumulativeActivePercent) return powerupIndex;
        }

        return -1;
    }
}
