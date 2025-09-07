using UnityEngine;
public abstract class NicholasMovementState : MonoBehaviour
{
    // Variables

    protected NicholasPlayerMovement playerMovement;
    protected Rigidbody body;



    // Functions

    public virtual void OnEnter(NicholasPlayerMovement _playerMovement, Rigidbody _body)
    {
        playerMovement = _playerMovement;
        body = _body;
    }
    
    public abstract void OnUpdate(MoveInputStruct _input);
   
    public abstract void OnExit();
}
