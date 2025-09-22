using UnityEngine;

public class CriticalTarget : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] MonoBehaviour[] scriptsToDisable;
    [SerializeField] GameObject[] objectsToDestroy;
    public void TakeDamage(int _amount)
    {
        HP -= _amount;
        if(HP <= 0)
        {
            foreach (MonoBehaviour _script in scriptsToDisable)
            {
                _script.enabled = false;
            }

            foreach(GameObject _object in objectsToDestroy)
            {
                Destroy(_object);
            }
        }
    }
}
