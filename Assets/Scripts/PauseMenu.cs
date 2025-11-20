using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    private void Awake()
    {
        Application.targetFrameRate = 60; //fixed framerate
        Instance = this;

    }
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingsPanel.activeSelf)
            {
                IsPaused = !isPaused;
                pausePanel.SetActive(!pausePanel.activeSelf);
            }
            else
            {
                SettingsButtons.Instance.Back();
                settingsPanel.SetActive(false);
                pausePanel.SetActive(true);
            }
        }
    }
    [SerializeField] int levelID;
    [SerializeField] GameObject levelFinishedWindow;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image[] starsImages;
    [SerializeField] Sprite starSprite;
    [Header("Panels")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;

    [Header("Scores")]
    [SerializeField] int oneStarScore;
    [SerializeField] int twoStarScore;
    [SerializeField] int threeStarScore;

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

            //Save stars only for the highest score
            int stars = 0;
            if (score >= oneStarScore)
            {
                starsImages[0].sprite = starSprite;
                stars = 1;
                if (score >= twoStarScore)
                {
                    stars = 2;
                    starsImages[1].sprite = starSprite;

                    if (score >= threeStarScore)
                    {
                        stars = 3;
                        starsImages[2].sprite = starSprite;
                    }
                }

                PlayerPrefs.SetInt("LevelStars" + levelID, stars);
            }

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
    public void Settings()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResumeGame()
    {
        IsPaused = false;
        pausePanel.SetActive(false);
    }
}
