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

    public Image playerHealthBar;
    public GameObject playerDamageFlash;
    public GameObject playerHealingFlash;

    public GameObject player;
    public PlayerController playerScript;

    public bool isPaused;

    int gameGoalCount;

    float timeScaleOriginal;



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

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
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

}
