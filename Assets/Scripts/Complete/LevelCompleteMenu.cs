using UnityEngine;
using UnityEngine.UI;
public class LevelCompleteMenu : MonoBehaviour
{
    public static LevelCompleteMenu Instance;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        Instance = this;

        nextLevelButton.onClick.AddListener(LoadNextLevel);
        retryButton.onClick.AddListener(RetryLevel);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        menuPanel.SetActive(false);
    }

    public void ShowMenu()
    {
        Time.timeScale = 0f; // Oyun zamanýný durdur
        menuPanel.SetActive(true);
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;
       
    }

    private void RetryLevel()
    {
        Time.timeScale = 1f;
        
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
       
    }
}
