using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform hitPosition;
    [SerializeField] Transform hitPosition2;
    [SerializeField] Transform headPosition;

    [SerializeField] int shieldHealth;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject melee;
    [SerializeField] float hitRate;


    Color originalColor;

    float hitTimer;

    float angleToPlayer;

    bool playerInTrigger;

    bool isBlue;

    Vector3 playerDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = model.material.color;
        GameManager.instance.UpdateGameGoal(1);
    }


    // Update is called once per frame
    void Update()
    {
        if (shieldHealth > 0)
        {
            EnemyShield();
        }

        hitTimer += Time.deltaTime;

        if (playerInTrigger && CanSeePlayer())
        {

        }

    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.instance.player.transform.position - headPosition.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }

                if (hitTimer >= hitRate)
                {
                    MeleeHit();
                }

                return true;
            }
        }
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player"))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Player"))
            playerInTrigger = false;
    }

    void MeleeHit()
    {
        hitTimer = 0;
        Instantiate(melee, hitPosition.position, transform.rotation);
        Instantiate(melee, hitPosition2.position, transform.rotation);
    }


    public void TakeDamage(int _amount)
    {
        if (shieldHealth > 0)
        {
            shieldHealth -= _amount;
        }

        if (shieldHealth <= 0)
        {
            if (health > 0)
            {
                health -= _amount;
                StartCoroutine(FlashRed());
            }
        }

        if (health <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    void EnemyShield()
    {
        if (shieldHealth > 0)
        {
            model.material.color = Color.cyan;
            isBlue = true;
        }

        if (shieldHealth <= 0)
        {
            model.material.color = originalColor;
            isBlue = false;
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

}