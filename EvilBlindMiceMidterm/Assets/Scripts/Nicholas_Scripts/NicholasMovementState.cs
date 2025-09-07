using UnityEngine;
public abstract class NicholasMovementState : MonoBehaviour
{
    // Variables

    protected NicholasPlayerMovement playerMovement;
    protected Rigidbody body;



    // Functions

    public virtual void NicholasOnEnter(NicholasPlayerMovement _playerMovement, Rigidbody _body)
    {
        playerMovement = _playerMovement;
        body = _body;
    }
    
    public abstract void NicholasOnUpdate(NicholasMoveInputStruct _input);
   
    public abstract void NicholasOnExit();
}
