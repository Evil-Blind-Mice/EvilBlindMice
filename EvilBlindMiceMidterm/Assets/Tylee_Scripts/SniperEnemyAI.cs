using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static PlayerController;

public class sniperEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPosition;
    [SerializeField] Transform headPosition;

    [SerializeField] int shieldHealth;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject shootLine;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] Material preparingShot;
    [SerializeField] Material firedShot;
    [SerializeField] Material reloadShot;

    Color originalColor;

    float shootTimer;

    float distanceFromPlayer;

    float angleToPlayer;

    bool playerInTrigger;

    bool hasShot;

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

        shootTimer += Time.deltaTime;

        if (shootTimer > shootRate)
        {
            shootLine.GetComponent<Renderer>().material = preparingShot;
            shootTimer = 0;
        }

        if (hasShot)
        {
            ReloadSniper();
        }

        if (playerInTrigger)
        {
            if (CanSeePlayer()) { }
            UpdateSniperLine();
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

                if (shootTimer >= shootRate)
                {
                    Snipe();
                    hasShot = true;
                }

                return true;
            }
        }
        return false;
    }

    IEnumerator ReloadSniper()
    {
        shootLine.GetComponent<Renderer>().material = reloadShot;
        yield return new WaitForSeconds(0.1f);
        shootLine.GetComponent<Renderer>().material = preparingShot;
    }

    void UpdateSniperLine()
    {

        gameObject.transform.GetChild(0).transform.localScale =
        new Vector3(0.1f, Vector3.Distance(GameManager.instance.player.transform.position, gameObject.transform.position) / 5, 0.1f);

        gameObject.transform.GetChild(0).transform.localPosition =
        new Vector3(0, 0.75f, (Vector3.Distance(GameManager.instance.player.transform.position, gameObject.transform.position) / 5) + 0.5f);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Player"))
            playerInTrigger = true;
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

    void Snipe()
    {
        shootLine.GetComponent<Renderer>().material = firedShot;
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
            model.material.color = Color.lightCyan;
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