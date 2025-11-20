using UnityEngine;

public class SettingsManager
{
    public static float soundVolume, musicVolume;
    public static int screenWidth, screenHeight;
    public static bool fullScreen;
    public static void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SoundVolume"))
            soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        else soundVolume = 1;
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        else musicVolume = 1;
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            int f = PlayerPrefs.GetInt("FullScreen");
            if (f == 0)
            {
                fullScreen = false;
                Screen.fullScreen = false;
            }
            else
            {
                fullScreen = true;
                Screen.fullScreen = true;
            }
        }
        else
        {
            Screen.fullScreen = true;
            fullScreen = true;
        }
        if (PlayerPrefs.HasKey("Width"))
            screenWidth = PlayerPrefs.GetInt("Width");
        else screenWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
        if (PlayerPrefs.HasKey("Height"))
            screenHeight = PlayerPrefs.GetInt("Height");
        else screenHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
    }
    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        if (fullScreen == false)
            PlayerPrefs.SetInt("FullScreen", 0);
        else PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.SetInt("Width", screenWidth);
        PlayerPrefs.SetInt("Height", screenHeight);

        PlayerPrefs.Save();
    }
    public static void SetResolution(Resolution res)
    {
        int width = res.width;
        int height = res.height;
        Screen.SetResolution(width, height, fullScreen);
        screenWidth = width;
        screenHeight = height;
    }
    public static void SetFullScreen(bool full)
    {
        Screen.fullScreen = full;
        fullScreen = full;
    }
}
