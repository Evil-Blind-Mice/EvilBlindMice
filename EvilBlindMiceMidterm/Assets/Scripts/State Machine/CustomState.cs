using UnityEngine;

public class CustomState : MonoBehaviour
{
    [Header("CustomState Script Variables")]
    [SerializeField] protected StateMachine machine;
    protected bool isCurrentState = false;

    public virtual void OnEnter()
    {
        isCurrentState = true;
    }

    public virtual void OnExit()
    {
        isCurrentState = false;
    }

    public virtual void OnUpdate()
    {

    }
}
