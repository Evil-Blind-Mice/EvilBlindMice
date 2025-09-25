using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    PlayerStats stats;

    public int distanceToTravel = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool SpawnWallObstacle(GameObject _parent, int section, ChunkSectionSubSection _subSection)
    {
        if (GetCanSpawn(_parent, section))
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
            if (GetCanSpawn(_parent, _section))
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

    bool GetCanSpawn(GameObject _parent, int _section)
    {
        if (_parent.transform.parent.gameObject.GetComponent<ChunkV2>().iteration == 0)
        {
            return false;
        }

        if (_section < 0 || _section > 7)
        {
            return false;
        }

        int[] sectionBaseChance = { 8, 9, 10, 12, 13, 14, 15};

        stats = PlayerStats.instance;

        float chanceScalar = stats.distanceTraveled / distanceToTravel;

        Mathf.Clamp(chanceScalar, 0.0f, 5);

        float spawnChance = chanceScalar * sectionBaseChance[_section];

        int check = Random.Range(0, 200);

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
        bool hasSpawned = false;
        switch (_subSection)
        {
            case ChunkSectionSubSection.floor:
                if (_parent.GetComponentInChildren<Floor>() == null)
                {
                    hasSpawned = true;
                }
                break;
            case ChunkSectionSubSection.leftWall:
                if (_parent.GetComponentInChildren<LeftWall>() == null)
                {
                    hasSpawned = true;
                }
                break;
            case ChunkSectionSubSection.rightWall:
                if (_parent.GetComponentInChildren<RightWall>() == null)
                {
                    hasSpawned = true;
                }
                break;
            case ChunkSectionSubSection.cieling:
                if (_parent.GetComponentInChildren<Cieling>() == null)
                {
                    hasSpawned = true;
                }
                break;
            default:
                break;
        }
        return hasSpawned;
    }
}
