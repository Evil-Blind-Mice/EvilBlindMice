using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        PowerUpPickup.ResetAllEffects();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    public void Options()
    {
        GameManager.instance.menuActive.SetActive(false);
        GameManager.instance.StateOptions();
    }
    public void Upgrades()
    {
        GameManager.instance.menuActive.SetActive(false);
        GameManager.instance.StateUpgrades();
    }
    public void Back()
    {
        GameManager.instance.menuActive.SetActive(false);
        GameManager.instance.Back();
    }
}
