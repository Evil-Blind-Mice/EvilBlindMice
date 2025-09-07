using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{
    
    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();

#endif
    }
    public void Options()
    {
        RonnieGameManager.instance.menuActive.SetActive(false);
        RonnieGameManager.instance.StateOptions();
    }
    public void Upgrades()
    {

    }
    public void Back()
    {
        GameManager.instance.StatePause();
    }
}
