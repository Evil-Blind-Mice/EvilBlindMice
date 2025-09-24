using UnityEngine;

public class LazerLeftWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.YouLose();
        }
    }
}
