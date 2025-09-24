using UnityEngine;

public class LazerRightWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.YouLose();
        }
    }
}
