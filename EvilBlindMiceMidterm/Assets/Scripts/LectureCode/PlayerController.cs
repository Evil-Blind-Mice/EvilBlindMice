using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour, IDamage, IHeal
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] public int health;
    [SerializeField] public int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int speedBoostMultiplier = 1;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    float shootTimer;

    int jumpCount;
    int originalHealth;


    [HideInInspector] public int hasTripped = 0;

    bool isSprinting;
    public bool isInvincible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHealth = health;
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        Movement();
        Sprint();
    }

    void Movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                        (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDirection * (speed * speedBoostMultiplier) * Time.deltaTime);

        Jump();

        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
            Shoot();
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
    }

    public void TakeDamage(int _amount)
    {
        if (isInvincible) return;

        health -= _amount;
        UpdatePlayerUI();
        StartCoroutine(FlashDamage());

        if (health <= 0)
        {
            // Hey I'm dead!
            GameManager.instance.YouLose();
        }
    }

    public void Heal(int _amount)
    {
        health = Mathf.Clamp(health + _amount, 0, originalHealth);
        UpdatePlayerUI();
        StartCoroutine(FlashHeal());
    }

    public void SetSpeedBoostMultiplier(int _multiplier) { speedBoostMultiplier = _multiplier; }

    public void UpdatePlayerUI()
    {
        //GameManager.instance.playerHealthBar.fillAmount = (float)health / (float)originalHealth;
    }

    IEnumerator FlashDamage()
    {
        GameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageFlash.SetActive(false);
    }

    IEnumerator FlashHeal()
    {
        GameManager.instance.playerHealingFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerHealingFlash.SetActive(false);
    }
}