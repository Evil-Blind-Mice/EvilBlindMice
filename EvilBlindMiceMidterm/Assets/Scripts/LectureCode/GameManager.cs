using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{    
    public static GameManager instance { get; private set; }

    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuUpgrades;
    [SerializeField] TMP_Text distanceTraveledText;

    public Image playerHealthBar;
    public GameObject playerDamageFlash;
    public GameObject playerHealingFlash;
    public GameObject playerSpeedBoostFlash;
    public TMP_Text qLeft;
    public TMP_Text eRight;
    public TMP_Text weaponCurrentAmmo, weaponMaxAmmo;



    public GameObject player;
    public PlayerStats playerScript;
    public PlayerShooting playerAttackScript;

    public bool isPaused;

    int gameGoalCount;

    float timeScaleOriginal;

    TMP_Text lastText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
        timeScaleOriginal = Time.timeScale;

        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerStats>();
        playerAttackScript = player.GetComponent<PlayerShooting>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
         UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        if (playerHealthBar != null && PlayerStats.instance != null)
        {
            float currentHealth = PlayerStats.instance.GetHealth();
            float maxHealth = Mathf.Max(1, PlayerStats.instance.GetMaxHealth());
            playerHealthBar.fillAmount = currentHealth / maxHealth;
        }

        if(distanceTraveledText != null && PlayerStats.instance != null)
            distanceTraveledText.text = PlayerStats.instance.GetDistanceTraveled().ToString("F0");

        if(playerAttackScript != null && playerAttackScript.HasWeapon)
        {
            if (weaponCurrentAmmo)
                weaponCurrentAmmo.text = playerAttackScript.WeaponCurrentAmmo.ToString("F0");

            if (weaponMaxAmmo)
                weaponMaxAmmo.text = playerAttackScript.WeaponMaxAmmo.ToString("F0");
        }
    }

    public void FlashDamage()
    {
        if (playerDamageFlash)
            StartCoroutine(Flash(playerDamageFlash, .7f));
    }
    public void FlashHeal()
    {
        if (playerHealingFlash)
            StartCoroutine(Flash(playerHealingFlash, .7f));
    }

    public void FlashSpeedBoost(float _duration)
    {
        if (playerSpeedBoostFlash)
            StartCoroutine(Flash(playerSpeedBoostFlash, _duration));
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGameGoal(int _amount)
    {
        gameGoalCount += _amount;

        if (gameGoalCount <= 0)
        {
            // you won!!!
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void StateOptions()
    {
        Time.timeScale = 0;
        menuActive = menuOptions;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(true);
    }
    public void StateUpgrades()
    {
        Time.timeScale = 0;
        menuActive = menuUpgrades;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(true);
    }
    public void Back()
    {
        Time.timeScale = 0;
        menuActive = menuPause;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(true);
    }

    public void IntersectionDirectionPromptLeft()
    {
            StartCoroutine(Flash(qLeft.gameObject, 1.2f)); 
    }
    public void IntersectionDirectionPromptRight()
    {
        StartCoroutine(Flash(eRight.gameObject, 1.2f));
    }
    IEnumerator Flash(GameObject _go, float _seconds)
    {
        _go.SetActive(true);
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, _seconds));
        _go.SetActive(false);
    }
}
