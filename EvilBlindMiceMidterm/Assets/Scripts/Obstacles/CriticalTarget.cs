using UnityEngine;

public class CriticalTarget : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] MonoBehaviour[] scriptsToDisable;
    [SerializeField] GameObject[] objectsToDisable;
    [SerializeField] GameObject[] objectsToEnable;
    public void TakeDamage(int _amount)
    {
        HP -= _amount;
        if(HP <= 0)
        {
            foreach (MonoBehaviour _script in scriptsToDisable)
            {
                _script.enabled = false;
            }

            foreach(GameObject _object in objectsToDisable)
            {
                _object.SetActive(false);
            }

            foreach(GameObject _object in objectsToEnable)
            {
                _object.SetActive(true);
            }
        }
    }
}
