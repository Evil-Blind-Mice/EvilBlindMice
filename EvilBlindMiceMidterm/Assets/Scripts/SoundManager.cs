using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey("gameVolume"))
        {
            PlayerPrefs.SetFloat("gameVolume", 0.5f);
            Load();
        }
        else
        {
            Load();
        }
        volumeSider.interactable = true;
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSider.value;
        Save();
    }
    private void Load()
    {
        volumeSider.value = PlayerPrefs.GetFloat("gameVolume");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("gameVolume", volumeSider.value);
    }

}
