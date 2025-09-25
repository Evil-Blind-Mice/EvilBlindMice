using UnityEngine;

public class CustomState : MonoBehaviour
{
    [Header("CustomState Script Variables")]
    [SerializeField] protected StateMachine machine;
    [SerializeField] protected GameObject[] objectsEnabledDuringState;
    protected bool isCurrentState = false;

    public virtual void OnEnter()
    {
        isCurrentState = true;
        foreach(GameObject obj in objectsEnabledDuringState)
        {
            obj.SetActive(true);
        }
    }

    public virtual void OnExit()
    {
        isCurrentState = false;
        foreach (GameObject obj in objectsEnabledDuringState)
        {
            obj.SetActive(false);
        }
    }

    public virtual void OnUpdate()
    {

    }
}
