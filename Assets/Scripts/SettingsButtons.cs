using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{
    public static SettingsButtons Instance;
    private void Awake()
    {
        Instance = this;
    }
    [Header("Volume")]
    [SerializeField] Slider soundVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [Header("Resolution")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;
    List<Resolution> resolutions = new List<Resolution>();
    float soundVolume, musicVolume;
    bool fullScreen;
    int savedIndex;
    private void Start()
    {
        SettingsManager.LoadSettings();
        soundVolumeSlider.value = SettingsManager.soundVolume;
        musicVolumeSlider.value = SettingsManager.musicVolume;
        resolutions = Screen.resolutions
        .GroupBy(r => new { r.width, r.height })
        .Select(g => g.First())
        .ToList();
        resolutionDropdown.ClearOptions();
        List<string> resolutionsList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == SettingsManager.screenWidth && resolutions[i].height == SettingsManager.screenHeight)
                currentResolutionIndex = i;
            string res = resolutions[i].width + " x " + resolutions[i].height;
            resolutionsList.Add(res);
        }
        resolutionDropdown.AddOptions(resolutionsList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        fullScreenToggle.isOn = SettingsManager.fullScreen;
        savedIndex = currentResolutionIndex;
        AudioManager.instance.SetSoundVolume(soundVolume);
        AudioManager.instance.SetMusicVolume(musicVolume);
    }
    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        AudioManager.instance.SetSoundVolume(soundVolume);
        SettingsManager.soundVolume = soundVolume;
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        AudioManager.instance.SetMusicVolume(musicVolume);
        SettingsManager.musicVolume = musicVolume;
    }
    public void SetFullScreen(bool value)
    {
        fullScreen = value;
        SettingsManager.SetFullScreen(fullScreen);
    }
    public void SetResolution(int index)
    {
        savedIndex = index;
        SettingsManager.SetResolution(resolutions[savedIndex]);
    }
    public void Back()
    {
        SettingsManager.SaveSettings();
        soundVolumeSlider.value = SettingsManager.soundVolume;
        soundVolumeSlider.value = SettingsManager.soundVolume;
        fullScreenToggle.isOn = SettingsManager.fullScreen;
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
