using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] Slider volumeSider;
    [SerializeField] Slider fovSlider;
    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        playerCamera = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey("gameVolume"))
        {
            PlayerPrefs.SetFloat("gameVolume", 0.5f);
            LoadVolume();
        }
        else
        {
            LoadVolume();
        }
        if (!PlayerPrefs.HasKey("FOV"))
        {
            PlayerPrefs.SetFloat("FOV", 60f);
            LoadFOV();
        }
        else
        {
            LoadFOV();
        }
        volumeSider.interactable = true;
        fovSlider.interactable = true;
    }

    private Camera FindWithTag(string v)
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
       
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSider.value;
        SaveVolume();
    }
    public void ChangeFOV()
    {
        playerCamera.fieldOfView = fovSlider.value;
        
        SaveFOV();

    }
    private void LoadVolume()
    {
        volumeSider.value = PlayerPrefs.GetFloat("gameVolume");
        AudioListener.volume = volumeSider.value;
    }
    private void LoadFOV()
    {
        fovSlider.value = PlayerPrefs.GetFloat("FOV");
        playerCamera.fieldOfView = fovSlider.value;
    }
    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("gameVolume", volumeSider.value);
    }
    private void SaveFOV()
    {
        PlayerPrefs.SetFloat("FOV", fovSlider.value);
    }

}
