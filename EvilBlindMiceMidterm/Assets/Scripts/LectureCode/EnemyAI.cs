using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static PlayerController;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPosition;
    [SerializeField] Transform headPosition;

    [SerializeField] int shieldHealth;
    [SerializeField] int health;
    [SerializeField] int FOV;
    [SerializeField] int animTransSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shield;
    [SerializeField] float shootRate;

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audBreak;
    [Range(0, 1)][SerializeField] float audBreakVol;
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 1)][SerializeField] float audShootVol;

    Color originalColor;

    public int faceTargetSpeed;

    [HideInInspector] public float shootTimer;

    float angleToPlayer;

    public bool playerInTrigger;

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

    void SetAnimationLocotion()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTransSpeed));
    }

    public bool CanSeePlayer()
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
        aud.PlayOneShot(audShoot[UnityEngine.Random.Range(0, audShoot.Length)], audShootVol);
        anim.SetTrigger("Shoot");
    }

    public void CreateBullet()
    {
        Instantiate(bullet, shootPosition.position, shootPosition.rotation);

    }

    public void TakeDamage(int _amount)
    {
        if (isBlue)
        {
            shieldHealth -= _amount;
            aud.PlayOneShot(audBreak[UnityEngine.Random.Range(0, audBreak.Length)], audBreakVol);
        }

        if (!isBlue)
        {
            if (health > 0)
            {
                health -= _amount;
                aud.PlayOneShot(audHurt[UnityEngine.Random.Range(0, audHurt.Length)], audHurtVol);
                StartCoroutine(FlashRed());
            }
        }

        if (health <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    public void EnemyShield()
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