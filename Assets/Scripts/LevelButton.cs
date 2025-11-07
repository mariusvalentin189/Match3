using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] GameObject uiElements;
    [SerializeField] GameObject lockedIcon;
    [SerializeField] TMP_Text scoreText;
    public void UnlockLevel(int score)
    {
        uiElements.SetActive(true);
        scoreText.text = score.ToString();
        lockedIcon.SetActive(false);
    }
}
