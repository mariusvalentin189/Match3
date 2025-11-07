using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    private void Awake()
    {
        Application.targetFrameRate = 60; //fixed framerate
        Instance = this;

    }

    [SerializeField] int levelID;

    public bool IsPaused 
    {
        get { return isPaused; } 
        set 
        { 
            isPaused = value;
            if (isPaused)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        } 
    }
    [SerializeField] GameObject levelFinishedWindow;
    [SerializeField] TMP_Text scoreText;
    bool isPaused;

    public void FinishLevel(int score)
    {
        IsPaused = true;
        levelFinishedWindow.SetActive(true);
        int previousScore = 0;
        previousScore = PlayerPrefs.GetInt("LevelScore" + levelID);
        if (previousScore < score)
        {
            scoreText.text = $"FINAL SCORE: \n{score.ToString()} \nHIGH SCORE!";
            PlayerPrefs.SetInt("LevelScore" + levelID, score);
        }
        else scoreText.text = $"FINAL SCORE: \n{score.ToString()}";

        PlayerPrefs.SetInt("LevelCompleted" + levelID, 1);

    }

    public void RestartLevel()
    {
        IsPaused=false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        IsPaused = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void MainMenu()
    {
        IsPaused = false;
        SceneManager.LoadSceneAsync(0); //Menu scene always 0
    }
}
