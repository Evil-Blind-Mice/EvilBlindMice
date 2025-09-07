using UnityEngine;
public abstract class MovementState : MonoBehaviour
{
    // Variables

    protected PlayerMovement playerMovement;
    protected Rigidbody body;



    // Functions

    public virtual void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        playerMovement = _playerMovement;
        body = _body;
    }
    
    public abstract void OnUpdate(MoveInputStruct _input);
   
    public abstract void OnExit();
}
