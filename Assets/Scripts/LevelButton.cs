using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] GameObject uiElements;
    [SerializeField] GameObject lockedIcon;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image[] starsImages;
    [SerializeField] Sprite starSprite;
    public void UnlockLevel(int score, int starsCount)
    {
        uiElements.SetActive(true);
        scoreText.text = score.ToString();
        lockedIcon.SetActive(false);

        if (score == 0) return;

        for (int i = 0; i < starsCount; i++)
        {
            starsImages[i].sprite = starSprite;
        }
    }
}
