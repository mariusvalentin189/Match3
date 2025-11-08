using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] LevelButton[] levelsButtons;
    private void Start()
    {
        CheckCompletedLevels();
    }
    public void StartLevel(int id)
    {
        SceneManager.LoadSceneAsync(id);
    }

    void CheckCompletedLevels()
    {
        for (int i = 0; i < levelsButtons.Length; i++)
        {
            int completedState = 0;
            completedState = PlayerPrefs.GetInt("LevelCompleted" + i);
            if (completedState == 1)
            {
                int score = PlayerPrefs.GetInt("LevelScore" + i);

                int starsCount = 0;
                starsCount = PlayerPrefs.GetInt("LevelStars" + i);

                levelsButtons[i].UnlockLevel(score, starsCount);
            }
            else
            {
                levelsButtons[i].UnlockLevel(0, 0);
                break;
            }
        }
    }
}
