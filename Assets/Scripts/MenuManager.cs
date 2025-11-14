using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                SettingsButtons.Instance.Back();
                settingsPanel.SetActive(false);
            }
        }
    }
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void Settings()
    {
        settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
