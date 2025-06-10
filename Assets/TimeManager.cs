// TimeManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için eklendi
using TMPro;
using System.IO;
using System.Collections.Generic; // List için eklendi
using System.Linq; // LINQ sorgularý için eklendi (isteðe baðlý ama kullanýþlý)

public class TimeManager : MonoBehaviour
{
    [Header("UI Elemanlarý (TextMeshPro)")]
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestTimeText;

    [Header("Oyuncu Ayarlarý")]
    public string playerTag = "Player";

    private float currentTime = 0f;
    private bool timerRunning = false;
    private AllBestTimesData allBestTimesData; // Tüm seviyelerin en iyi sürelerini tutar
    private float currentLevelBestTime = float.MaxValue; // Mevcut seviyenin en iyi süresi
    private string currentLevelName;
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "allBestTimesData.json"); 
        currentLevelName = SceneManager.GetActiveScene().name; // Mevcut sahnenin adýný al
        LoadAllBestTimes();
    }

    void Start()
    {
        StartTimer();
        UpdateBestTimeUI();
    }

    void Update()
    {
        if (timerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateCurrentTimeUI();
        }
    }

    public void StartTimer()
    {
        currentTime = 0f;
        timerRunning = true;
        UpdateCurrentTimeUI();
    }

    public void PlayerCrossedFinishLine()
    {
        if (!timerRunning) return;

        timerRunning = false;

        if (currentTime < currentLevelBestTime)
        {
            currentLevelBestTime = currentTime;
            SaveCurrentLevelBestTime();
        }
        UpdateBestTimeUI();
    }

    void SaveCurrentLevelBestTime()
    {
        // Mevcut seviye için kaydý bul veya oluþtur
        LevelBestTime levelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);

        if (levelEntry != null)
        {
            // Kayýt varsa güncelle
            levelEntry.bestTime = currentLevelBestTime;
        }
        else
        {
            // Kayýt yoksa yeni oluþtur ve listeye ekle
            allBestTimesData.levelTimes.Add(new LevelBestTime(currentLevelName, currentLevelBestTime));
        }

        // Tüm veriyi JSON'a kaydet
        string json = JsonUtility.ToJson(allBestTimesData, true);
        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("En iyi süreler kaydedilemedi: " + e.Message);
        }
    }

    void LoadAllBestTimes()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                allBestTimesData = JsonUtility.FromJson<AllBestTimesData>(json);
                if (allBestTimesData == null || allBestTimesData.levelTimes == null) // Dosya bozuk olabilir veya içeriði yanlýþ
                {
                    allBestTimesData = new AllBestTimesData();
                }
                
            }
            catch (System.Exception )
            {
                
                allBestTimesData = new AllBestTimesData();
            }
        }
        else
        {
         
            allBestTimesData = new AllBestTimesData();
        }

        // Mevcut seviyenin en iyi süresini bul
        LevelBestTime currentLevelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);
        if (currentLevelEntry != null)
        {
            currentLevelBestTime = currentLevelEntry.bestTime;
            
        }
        else
        {
            currentLevelBestTime = float.MaxValue; // Bu seviye için henüz kayýt yok
     
        }
    }

    void UpdateCurrentTimeUI()
    {
        if (currentTimeText != null)
        {
            currentTimeText.text = $"Süre: {FormatTime(currentTime)}";
        }
    }

    void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
        {
            if (currentLevelBestTime == float.MaxValue)
            {
                bestTimeText.text = $"En Ýyi Süre : --:--:---";
            }
            else
            {
                bestTimeText.text = $"En Ýyi Süre : {FormatTime(currentLevelBestTime)}";
            }
        }
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 1000) % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // Ýsteðe baðlý: Sadece mevcut seviyenin en iyi süresini sýfýrlamak için
    public void ResetCurrentLevelBestTime()
    {
        LevelBestTime levelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);
        if (levelEntry != null)
        {
            levelEntry.bestTime = float.MaxValue; // Kaydý sýfýrla
            currentLevelBestTime = float.MaxValue;
            SaveCurrentLevelBestTime(); // Deðiþikliði kaydet
            UpdateBestTimeUI();
        }
        else
        {
        }
    }

    // Ýsteðe baðlý: TÜM seviyelerin en iyi sürelerini silmek için
    public void ResetAllBestTimes()
    {
        allBestTimesData = new AllBestTimesData();
        currentLevelBestTime = float.MaxValue; // Mevcut seviyenin de göstergesini sýfýrla
        // Dosyayý silmek veya boþ bir AllBestTimesData kaydetmek
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath); // Dosyayý silmek daha temiz olabilir
            }
            // Veya boþ bir nesne kaydedebilirsiniz:
            // string json = JsonUtility.ToJson(allBestTimesData, true);
            // File.WriteAllText(saveFilePath, json);

        }
        catch (System.Exception )
        {
        }
        UpdateBestTimeUI();
    }
}