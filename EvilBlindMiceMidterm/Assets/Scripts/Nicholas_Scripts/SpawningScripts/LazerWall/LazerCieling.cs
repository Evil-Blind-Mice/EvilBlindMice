using UnityEngine;

public class LazerCieling : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.YouLose();
        }
    }
}
