using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static PlayerController;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPosition;
    [SerializeField] Transform headPosition;

    [SerializeField] int shieldHealth;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shield;
    [SerializeField] float shootRate;

    Color originalColor;

    float shootTimer;

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
        EnemyShield();

        shootTimer += Time.deltaTime;

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

                if (playerInTrigger)
                {
                    FaceTarget();
                }

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

                return true;
            }
        }
        return false;
    }
    void FaceTarget()
    {
        Vector3 rot = transform.eulerAngles;
        rot.y = Quaternion.LookRotation(playerDirection).eulerAngles.y;

        Quaternion targetRot = Quaternion.Euler(rot);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * faceTargetSpeed);
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
        if (isBlue)
        {
            shieldHealth -= _amount;
        }

        if (!isBlue)
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
            shield.SetActive(true);
            isBlue = true;
        }

        if (shieldHealth <= 0)
        {
            shield.SetActive(false);
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