using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject upgradesPanel;
    [SerializeField] GameObject mainMenuItems;

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Options()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StateOptions();
            return;
        }

        if (optionsPanel)
        {
            optionsPanel.SetActive(true);
            mainMenuItems.SetActive(false);
        }
    }

    public void Upgrades()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StateUpgrades();
            return;
        }
        
        if (upgradesPanel)
        {
            upgradesPanel.SetActive(true);
            mainMenuItems.SetActive(false);
        }
    }

    public void Back()
    {
        if (GameManager.instance != null)
        { 
            GameManager.instance.Back();
            return;
        }

        if (optionsPanel)
        {
            optionsPanel.SetActive(false);
            mainMenuItems.SetActive(true);
        }
            
        if (upgradesPanel)
        {
            upgradesPanel.SetActive(false);
            mainMenuItems.SetActive(true);
        }
    }

    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
