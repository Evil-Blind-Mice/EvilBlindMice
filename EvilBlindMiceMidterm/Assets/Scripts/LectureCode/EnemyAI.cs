using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPosition;

    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;


    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color originalColor;

    float shootTimer;

    bool playerInTrigger;

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
        shootTimer += Time.deltaTime;
        playerDirection = GameManager.instance.player.transform.position - transform.position;

        if (playerInTrigger)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (shootTimer >= shootRate)
            {
                Shoot();
            }
        }
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

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPosition.position, transform.rotation);
    }

    public void TakeDamage(int _amount)
    {
        if (health > 0)
        {
            health -= _amount;
            StartCoroutine(FlashRed());
        }

        if (health <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }
}