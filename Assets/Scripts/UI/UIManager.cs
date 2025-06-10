using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Buton kontrolü için gerekli

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject MainMenuScene;
    [SerializeField] private GameObject optionMenuScene;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Text pauseTitleText; // "PAUSE" yazýsý için Text bileþeni
    [SerializeField] private GameObject[] pauseMenuItems; // Diðer menü öðeleri (RESUME, VOLUME vb.)

    public GameObject optionMenu;

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        if (pauseButton == null)
            pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
    }

    public void ShowOption()
    {
        pauseMenuItems[0].SetActive(false);
        pauseMenuItems[1].SetActive(false);
        pauseMenuItems[2].SetActive(false);
        pauseMenuItems[4].SetActive(false);
        pauseMenuItems[5].SetActive(false);
        optionMenuScene.gameObject.SetActive(true);
       
    }

    public void X()
    {
        pauseScreen.SetActive(true);
        optionMenuScene.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        // "PAUSE" yazýsýný ve diðer menü öðelerini geri getir
        pauseTitleText.gameObject.SetActive(true);
        foreach (var item in pauseMenuItems)
        {
            item.SetActive(true);
        }
    }
    #region Game Over
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
        pauseButton.gameObject.SetActive(false); // Oyun bittiðinde buton kaybolsun
    }

    public void Restart()
    {
        MainMenuScene.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pauseButton.gameObject.SetActive(true); // Yeniden baþlatýnca buton geri gelsin
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        pauseButton.gameObject.SetActive(!status); // Menü açýkken buton kapalý, kapalýyken açýk

        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    // Butona týklayýnca menüyü açacak fonksiyon
    public void TogglePauseMenu()
    {
        bool shouldPause = !pauseScreen.activeInHierarchy;
        PauseGame(shouldPause);
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion

    public void SetQuality(int qual)
    {
        QualitySettings.SetQualityLevel(qual);
    }
}