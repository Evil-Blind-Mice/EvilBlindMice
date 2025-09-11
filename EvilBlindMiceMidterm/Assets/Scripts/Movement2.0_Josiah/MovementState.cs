using UnityEngine;
public abstract class MovementState : MonoBehaviour
{
    // Variables

    protected PlayerMovement playerMovement;
    protected Rigidbody body;
    protected bool isCurrentState = false;
    protected bool insideIntersection = false;
    Vector3 positionLastFrame;


    // Functions

    public virtual void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        playerMovement = _playerMovement;
        body = _body;
        isCurrentState = true;

        if (playerMovement.currentIntersection != null) OnIntersectionEnter(playerMovement.currentIntersection);
    }

    public virtual void OnUpdate(MoveInputStruct _input)
    {
        PlayerStats.instance.AddDistanceTraveled(Vector3.Dot((body.transform.position - positionLastFrame), playerMovement.gravityReference.forward));
        positionLastFrame = body.transform.position;
    }

    public virtual void OnExit()
    {
        isCurrentState = false;
    }

    public virtual void OnIntersectionEnter(Intersection _intersection)
    {
        insideIntersection = true;
        playerMovement.currentIntersection = _intersection;
    }

    public virtual void OnInsideIntersection() { }

    public virtual void OnIntersectionExit(Intersection _intersection, Vector3 _exitPoint)
    {
        insideIntersection = false;
        playerMovement.currentIntersection = null;
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
