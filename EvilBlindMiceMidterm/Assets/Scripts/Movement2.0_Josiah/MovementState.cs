using UnityEngine;
public abstract class MovementState : MonoBehaviour
{
    // Variables

    protected PlayerMovement playerMovement;
    protected Rigidbody body;
    protected bool isCurrentState = false;
    protected bool insideIntersection = false;



    // Functions

    public virtual void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        playerMovement = _playerMovement;
        body = _body;
        isCurrentState = true;
    }

    public abstract void OnUpdate(MoveInputStruct _input);

    public virtual void OnExit()
    {
        isCurrentState = false;
    }

    public virtual void OnIntersectionEnter()
    {
        insideIntersection = true;
    }

    public virtual void OnIntersectionExit()
    {
        insideIntersection = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCurrentState) return;
        if (other.gameObject.tag == "Intersection")
            OnIntersectionEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isCurrentState) return;
        if (other.gameObject.tag == "Intersection")
            OnIntersectionExit();
    }
}
