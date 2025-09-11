using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject Enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject enemies = Instantiate(Enemy, this.gameObject.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
