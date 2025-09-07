using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get; private set;
    }
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] public GameObject menuOptions;
    [SerializeField] public GameObject menuUpgrades;

    //public Image playerHealthBar;
    //public GameObject playerDamageFlashPanel;
    //public playerController playerScript;
    public bool isPaused;
    //int gameGoalCount;
    float timeScaleOriginal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        timeScaleOriginal = Time.timeScale;
        //player = GameObject.FindWithTag("Player");
        //playerScript = player.GetComponent<playerController>();

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

    public void StateOptions()
    {       
        Time.timeScale = 0;
        menuActive = menuOptions;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(true);
    }

    //public void updateGameGoal(int _amount)
    //{
    //    gameGoalCount += amount;

    //    if (gameGoalCount <= 0)
    //    {
    //        //you won!!
    //        statePause();
    //        menuActive = menuWin;
    //        menuActive.SetActive(true);
    //    }
    //}
    //public void loseState()
    //{
    //    statePause();
    //    menuActive = menuLose;
    //    menuActive.SetActive(true);
    //}
}
