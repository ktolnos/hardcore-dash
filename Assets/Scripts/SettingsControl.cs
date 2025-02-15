using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioMixer audioMixer;
    private Slider masterVolume;
    private Slider musicVolume;
    private Slider SFXVolume;
    
    void Start()
    {
        var tmp = pauseMenu.GetComponentsInChildren<Slider>();
        foreach (var item in tmp){
            if(item.name == "Master Slider"){
                masterVolume = item;
            }
            if(item.name == "Music Slider"){
                musicVolume = item;
            }
            if (item.name == "SFX Slider"){
                SFXVolume = item;
            }
        }
        masterVolume.value = PlayerPrefs.GetFloat("masterVolume");
        musicVolume.value = PlayerPrefs.GetFloat("musicVolume");
        SFXVolume.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false); 
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }
        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("masterVolume"));
        audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("musicVolume"));
        audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFXVolume"));
    }
}
