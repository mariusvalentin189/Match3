using UnityEngine;
using TMPro;
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
    [SerializeField] GameObject levelFinishedWindow;
    [SerializeField] TMP_Text scoreText;
    bool isPaused;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishLevel(int score)
    {
        IsPaused = true;
        levelFinishedWindow.SetActive(true);
        scoreText.text = $"FINAL SCORE: \n {score.ToString()}";

    }
}
