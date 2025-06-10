using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevels : MonoBehaviour
{
    public Button[] levelButtons; // Inspector'dan t�m seviye butonlar�n� buraya s�r�kleyin (1'den 9'a kadar)
    // Opsiyonel: Kilitli butonlar i�in bir kilit ikonu prefab'� veya sprite'�
    public GameObject lockIconPrefab; // E�er kullanacaksan�z, her buton i�in bir tane olu�turup pozisyonlay�n

    private const string MAX_LEVEL_REACHED_KEY = "MaxLevelReached";

    void Start()
    {
        // Oyun ilk kez a��l�yorsa veya kay�t yoksa, sadece 1. seviye a��k olsun.
        if (!PlayerPrefs.HasKey(MAX_LEVEL_REACHED_KEY))
        {
            PlayerPrefs.SetInt(MAX_LEVEL_REACHED_KEY, 1);
            PlayerPrefs.Save();
        }

        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        int maxLevelReached = PlayerPrefs.GetInt(MAX_LEVEL_REACHED_KEY, 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int buttonLevelNumber = i + 1; // �nemli: Buton dizisi 0'dan ba�lar

            if (buttonLevelNumber <= maxLevelReached)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    // Bu fonksiyon bir seviye tamamland���nda �a�r�lacak
    public static void CompleteLevel(int completedLevelNumber) // static yapt�k ki her yerden eri�ilebilsin
    {
        int nextLevel = completedLevelNumber + 1;
        int currentMaxLevel = PlayerPrefs.GetInt(MAX_LEVEL_REACHED_KEY, 1);

        if (nextLevel > currentMaxLevel)
        {
            PlayerPrefs.SetInt(MAX_LEVEL_REACHED_KEY, nextLevel);
            PlayerPrefs.Save();
            Debug.Log("Level " + nextLevel + " unlocked!");
        }
    }

    // --- MEVCUT GoToLevel FONKS�YONLARINIZ BURADA DEVAM EDECEK ---
    // Bu fonksiyonlar butonlar `interactable = false` oldu�unda zaten �a�r�lamayacak.
    public void GoToLevel1()
    {
        SceneManager.LoadScene(2); // Build index 3 -> Sizin Seviye 1'iniz
    }
    public void GoToLevel2()
    {
        SceneManager.LoadScene(3); // Build index 4 -> Sizin Seviye 2'niz
    }
    public void GoToLevel3()
    {
        SceneManager.LoadScene(4);
    }
    public void GoToLevel4()
    {
        SceneManager.LoadScene(5);
    }
    public void GoToLevel5()
    {
        SceneManager.LoadScene(6);
    }
    public void GoToLevel6()
    {
        SceneManager.LoadScene(7);
    }
    public void GoToLevel7()
    {
        SceneManager.LoadScene(8);
    }
    public void GoToLevel8()
    {
        SceneManager.LoadScene(9);
    }
    public void GoToLevel9()
    {
        SceneManager.LoadScene(10); // Tahmini bir sonraki build index
    }
    public void GoToHome()
    {
        SceneManager.LoadScene(0); // Ana men� build index'i
    }

    // Test i�in ilerlemeyi s�f�rlama (Geli�tirme a�amas�nda kullan��l�)
    [ContextMenu("Reset Level Progress")] // Inspector'da script'e sa� t�klay�nca ��kar
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(MAX_LEVEL_REACHED_KEY);
        PlayerPrefs.SetInt(MAX_LEVEL_REACHED_KEY, 1); // 1. seviyeyi tekrar a�
        PlayerPrefs.Save();
        UpdateLevelButtons(); // Butonlar� g�ncelle
        Debug.Log("Level progress reset. Only Level 1 is unlocked.");
    }
}
