// TimeManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in eklendi
using TMPro;
using System.IO;
using System.Collections.Generic; // List i�in eklendi
using System.Linq; // LINQ sorgular� i�in eklendi (iste�e ba�l� ama kullan��l�)

public class TimeManager : MonoBehaviour
{
    [Header("UI Elemanlar� (TextMeshPro)")]
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestTimeText;

    [Header("Oyuncu Ayarlar�")]
    public string playerTag = "Player";

    private float currentTime = 0f;
    private bool timerRunning = false;
    private AllBestTimesData allBestTimesData; // T�m seviyelerin en iyi s�relerini tutar
    private float currentLevelBestTime = float.MaxValue; // Mevcut seviyenin en iyi s�resi
    private string currentLevelName;
    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "allBestTimesData.json"); 
        currentLevelName = SceneManager.GetActiveScene().name; // Mevcut sahnenin ad�n� al
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
        // Mevcut seviye i�in kayd� bul veya olu�tur
        LevelBestTime levelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);

        if (levelEntry != null)
        {
            // Kay�t varsa g�ncelle
            levelEntry.bestTime = currentLevelBestTime;
        }
        else
        {
            // Kay�t yoksa yeni olu�tur ve listeye ekle
            allBestTimesData.levelTimes.Add(new LevelBestTime(currentLevelName, currentLevelBestTime));
        }

        // T�m veriyi JSON'a kaydet
        string json = JsonUtility.ToJson(allBestTimesData, true);
        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("En iyi s�reler kaydedilemedi: " + e.Message);
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
                if (allBestTimesData == null || allBestTimesData.levelTimes == null) // Dosya bozuk olabilir veya i�eri�i yanl��
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

        // Mevcut seviyenin en iyi s�resini bul
        LevelBestTime currentLevelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);
        if (currentLevelEntry != null)
        {
            currentLevelBestTime = currentLevelEntry.bestTime;
            
        }
        else
        {
            currentLevelBestTime = float.MaxValue; // Bu seviye i�in hen�z kay�t yok
     
        }
    }

    void UpdateCurrentTimeUI()
    {
        if (currentTimeText != null)
        {
            currentTimeText.text = $"S�re: {FormatTime(currentTime)}";
        }
    }

    void UpdateBestTimeUI()
    {
        if (bestTimeText != null)
        {
            if (currentLevelBestTime == float.MaxValue)
            {
                bestTimeText.text = $"En �yi S�re : --:--:---";
            }
            else
            {
                bestTimeText.text = $"En �yi S�re : {FormatTime(currentLevelBestTime)}";
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

    // �ste�e ba�l�: Sadece mevcut seviyenin en iyi s�resini s�f�rlamak i�in
    public void ResetCurrentLevelBestTime()
    {
        LevelBestTime levelEntry = allBestTimesData.levelTimes.FirstOrDefault(lt => lt.levelName == currentLevelName);
        if (levelEntry != null)
        {
            levelEntry.bestTime = float.MaxValue; // Kayd� s�f�rla
            currentLevelBestTime = float.MaxValue;
            SaveCurrentLevelBestTime(); // De�i�ikli�i kaydet
            UpdateBestTimeUI();
        }
        else
        {
        }
    }

    // �ste�e ba�l�: T�M seviyelerin en iyi s�relerini silmek i�in
    public void ResetAllBestTimes()
    {
        allBestTimesData = new AllBestTimesData();
        currentLevelBestTime = float.MaxValue; // Mevcut seviyenin de g�stergesini s�f�rla
        // Dosyay� silmek veya bo� bir AllBestTimesData kaydetmek
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath); // Dosyay� silmek daha temiz olabilir
            }
            // Veya bo� bir nesne kaydedebilirsiniz:
            // string json = JsonUtility.ToJson(allBestTimesData, true);
            // File.WriteAllText(saveFilePath, json);

        }
        catch (System.Exception )
        {
        }
        UpdateBestTimeUI();
    }
}