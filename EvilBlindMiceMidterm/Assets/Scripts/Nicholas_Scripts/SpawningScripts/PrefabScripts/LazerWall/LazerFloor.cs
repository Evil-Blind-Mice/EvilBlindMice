using UnityEngine;

public class LazerFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.YouLose();
        }
    }
}
