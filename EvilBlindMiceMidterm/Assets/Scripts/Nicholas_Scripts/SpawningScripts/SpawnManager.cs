using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    PlayerStats stats;

    public int distanceToTravel = 100;

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        stats = PlayerStats.instance;
    }

    public bool SpawnWallObstacle(GameObject _parent, int section, ChunkSectionSubSection _subSection)
    {
        if (GetCanSpawn(section))
        {
            switch (_subSection)
            {
                case ChunkSectionSubSection.floor:
                    _parent.GetComponentInChildren<Floor>().hasWallObstacleSpawned = true;
                    return true;
                case ChunkSectionSubSection.leftWall:
                    _parent.GetComponentInChildren<LeftWall>().hasWallObstacleSpawned = true;
                    return true;
                case ChunkSectionSubSection.rightWall:
                    _parent.GetComponentInChildren<RightWall>().hasWallObstacleSpawned = true;
                    return true;
                case ChunkSectionSubSection.cieling:
                    _parent.GetComponentInChildren<Cieling>().hasWallObstacleSpawned = true;
                    return true;
                default:
                    return false;

            }
        }
        else
        {
            return false;
        }
    }

    public bool SpawnEnemyOrObstacle(GameObject _parent, int _section, ChunkSectionSubSection _subSection)
    {
        if (!HasWallObstacleSpawned(_parent, _subSection))
        {
            if (GetCanSpawn(_section))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    bool GetCanSpawn(int _section)
    {
        if (_section < 0 || _section > 8)
        {
            return false;
        }

        int[] sectionBaseChance = { 8, 9, 10, 12, 13, 14, 15, 15 };

        float chanceScalar = stats.distanceTraveled / distanceToTravel;

        Mathf.Clamp(chanceScalar, 0.0f, 10);

        float spawnChance = chanceScalar * sectionBaseChance[_section];

        int check = Random.Range(0, 240);

        if (spawnChance <= check)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool HasWallObstacleSpawned(GameObject _parent, ChunkSectionSubSection _subSection)
    {
        switch (_subSection)
        {
            case ChunkSectionSubSection.floor:
                return _parent.GetComponentInChildren<Floor>().hasWallObstacleSpawned;
            case ChunkSectionSubSection.leftWall:
                return _parent.GetComponentInChildren<LeftWall>().hasWallObstacleSpawned;
            case ChunkSectionSubSection.rightWall:
                return _parent.GetComponentInChildren<RightWall>().hasWallObstacleSpawned;
            case ChunkSectionSubSection.cieling:
                return _parent.GetComponentInChildren<Cieling>().hasWallObstacleSpawned;
            default:
                return false;
        }
    }
}
