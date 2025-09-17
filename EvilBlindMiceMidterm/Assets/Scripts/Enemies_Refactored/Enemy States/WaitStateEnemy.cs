using UnityEngine;

public class WaitStateEnemy : CustomState
{
    [Header("Line of Sight")]
    [SerializeField] Transform sightCheckPos;
    [SerializeField] float sightCheckDistance = 100;
    [Tooltip("Anything that could block line of sight, as well as the player layer")]
    [SerializeField] LayerMask canSeeLayers;

    [Header("References")]
    [SerializeField] EnemyStats stats;
    [SerializeField] CustomState playerFoundState;

    private void Start()
    {
        if (sightCheckPos == null) sightCheckPos = transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCurrentState) return;

        else if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            Debug.DrawRay(sightCheckPos.position, (GameManager.instance.player.transform.position - sightCheckPos.position) * sightCheckDistance);
            if (Physics.Raycast(sightCheckPos.position, GameManager.instance.player.transform.position - sightCheckPos.position, out hit, sightCheckDistance, canSeeLayers))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    machine.ChangeToState(playerFoundState);
                }
            }
        }
    }
}
