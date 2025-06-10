using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevels : MonoBehaviour
{
    public Button[] levelButtons; // Inspector'dan tüm seviye butonlarýný buraya sürükleyin (1'den 9'a kadar)
    // Opsiyonel: Kilitli butonlar için bir kilit ikonu prefab'ý veya sprite'ý
    public GameObject lockIconPrefab; // Eðer kullanacaksanýz, her buton için bir tane oluþturup pozisyonlayýn

    private const string MAX_LEVEL_REACHED_KEY = "MaxLevelReached";

    void Start()
    {
        // Oyun ilk kez açýlýyorsa veya kayýt yoksa, sadece 1. seviye açýk olsun.
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
            int buttonLevelNumber = i + 1; // Önemli: Buton dizisi 0'dan baþlar

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

    // Bu fonksiyon bir seviye tamamlandýðýnda çaðrýlacak
    public static void CompleteLevel(int completedLevelNumber) // static yaptýk ki her yerden eriþilebilsin
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

    // --- MEVCUT GoToLevel FONKSÝYONLARINIZ BURADA DEVAM EDECEK ---
    // Bu fonksiyonlar butonlar `interactable = false` olduðunda zaten çaðrýlamayacak.
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
        SceneManager.LoadScene(0); // Ana menü build index'i
    }

    // Test için ilerlemeyi sýfýrlama (Geliþtirme aþamasýnda kullanýþlý)
    [ContextMenu("Reset Level Progress")] // Inspector'da script'e sað týklayýnca çýkar
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(MAX_LEVEL_REACHED_KEY);
        PlayerPrefs.SetInt(MAX_LEVEL_REACHED_KEY, 1); // 1. seviyeyi tekrar aç
        PlayerPrefs.Save();
        UpdateLevelButtons(); // Butonlarý güncelle
        Debug.Log("Level progress reset. Only Level 1 is unlocked.");
    }
}
