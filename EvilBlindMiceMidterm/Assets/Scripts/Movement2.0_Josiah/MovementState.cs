using UnityEditor.Rendering;
using UnityEngine;
public abstract class MovementState : MonoBehaviour
{
    // Variables

    protected PlayerMovement playerMovement;
    protected Rigidbody body;
    protected bool isCurrentState = false;
    protected bool insideIntersection = false;
    protected Intersection currentIntersection;



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

    public virtual void OnIntersectionEnter(Intersection _intersection)
    {
        insideIntersection = true;
        currentIntersection = _intersection;
    }

    public virtual void OnInsideIntersection() { }

    public virtual void OnIntersectionExit(Intersection _intersection, Vector3 _exitPoint)
    {
        insideIntersection = false;
        currentIntersection = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCurrentState) return;
        if (other.gameObject.tag == "Intersection")
            OnIntersectionEnter(other.GetComponent<Intersection>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isCurrentState) return;
        if (other.gameObject.tag == "Intersection")
            OnIntersectionExit(other.GetComponent<Intersection>(), other.ClosestPoint(body.transform.position));
    }
}
