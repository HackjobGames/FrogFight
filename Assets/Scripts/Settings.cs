using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Dropdown textureDropdown;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider GameVolumeSlider;
    float currentMasterVolume;
    float currentMusicVolume;
    float currentGameVolume;
    Resolution[] resolutions;

    void Start()
    {
        LoadSettings();
    }

    public void LoadSettings(){
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + 
                    resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width 
                && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetMasterVolume(float volume) {
        audioMixer.SetFloat("Master", volume);
        currentMasterVolume = volume;
    }
    public void SetMusicVolume(float volume) {
        audioMixer.SetFloat("Music", volume);
        currentMusicVolume = volume;
    }
    public void SetGameVolume(float volume) {
        audioMixer.SetFloat("Game", volume);
        currentGameVolume = volume;
    }
    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, 
                resolution.height, Screen.fullScreen);
    }
    public void SetTextureQuality(int textureIndex) {
        QualitySettings.masterTextureLimit = textureIndex;
    }

    public void SaveSettings() {
        
        PlayerPrefs.SetInt("ResolutionPreference", 
                resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference", 
                textureDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", 
                Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MasterVolumePreference", 
                currentMasterVolume); 
        PlayerPrefs.SetFloat("MusicVolumePreference", 
                currentMusicVolume);
        PlayerPrefs.SetFloat("GameVolumePreference", 
                currentGameVolume);
    }

    public void LoadSettings(int currentResolutionIndex) {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = 
                        PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("TextureQualityPreference"))
            textureDropdown.value = 
                        PlayerPrefs.GetInt("TextureQualityPreference");
        else
            textureDropdown.value = 0;
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = 
            Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolumePreference");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolumePreference");
        GameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolumePreference");
    }

}
