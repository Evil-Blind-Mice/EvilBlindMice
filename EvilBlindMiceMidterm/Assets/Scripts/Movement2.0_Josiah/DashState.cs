using UnityEngine;

public class DashState : MovementState
{
    [SerializeField] MovementState defaultMovementState;
    [SerializeField] float dashDuration;
    float timer;
    int dashesStacked;

    public override void OnEnter(PlayerMovement _playerMovement, Rigidbody _body)
    {
        base.OnEnter(_playerMovement, _body);

        if (!PlayerStats.instance.TrySpendDash())
        {
            playerMovement.ChangeToState(defaultMovementState);
            return;
        }

        timer = dashDuration;
        dashesStacked = 0;
    }

    public override void OnUpdate(MoveInputStruct _input)
    {
        base.OnUpdate(_input);

        if (playerMovement.currentIntersection != null && playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up))
        {
            body.linearVelocity = Vector3.zero;
            return;
        }

        body.linearVelocity = Camera.main.transform.forward * PlayerStats.instance.GetDashForce() * (1 + dashesStacked * .5f);

        if (_input.jumpPressedThisFrame)
        {
            StackDash();
        }

        timer -= Time.deltaTime;
        if (timer <= 0) playerMovement.ChangeToState(defaultMovementState);
    }

    public override void OnExit()
    {
        body.linearVelocity = Vector3.zero;
        base.OnExit();
    }

    public override void OnIntersectionEnter(Intersection _intersection)
    {
        base.OnIntersectionEnter(_intersection);
        if (playerMovement.currentIntersection.IsDirectionAvailable(-playerMovement.gravityReference.up))
        {
            playerMovement.ChangeToState(defaultMovementState);
        }
    }

    void StackDash()
    {
        if (!PlayerStats.instance.TrySpendDash()) return;

        dashesStacked++;
        timer = dashDuration;
    }
}
