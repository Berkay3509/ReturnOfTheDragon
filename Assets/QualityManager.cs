using UnityEngine;

public class QualityManager : MonoBehaviour
{
    [Header("Frame Rate Targets")]
    [SerializeField] private int targetFPS_Low = 60;
    [SerializeField] private int targetFPS_Medium = 60; 
    [SerializeField] private int targetFPS_High = 60;

    // Örneðin, parçacýk efektleri veya post-processing profilleri için referanslar
    // [SerializeField] private GameObject lowQualityParticles;
    // [SerializeField] private GameObject mediumQualityParticles;
    // [SerializeField] private GameObject highQualityParticles;
    // [SerializeField] private UnityEngine.Rendering.Volume postProcessingVolume;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_Low;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_Medium;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_High;


    void Start()
    {
        // Oyun baþladýðýnda mevcut kalite seviyesine göre ayarlarý uygula
        ApplyQualitySettings();
    }

    public void SetQualityLevel(int levelIndex)
    {
        // Kalite seviyesini deðiþtir (örneðin, Ayarlar menüsünden çaðrýlýr)
        // Level index'leri Project Settings > Quality'deki sýraya göredir.
        // Emin olmak için isimle karþýlaþtýrma da yapabilirsiniz.
        QualitySettings.SetQualityLevel(levelIndex, true); // 'true' deðiþikliði hemen uygular
        Debug.Log("Quality level set to: " + QualitySettings.names[levelIndex]);
        ApplyQualitySettings();
    }

    void ApplyQualitySettings()
    {
        int currentLevel = QualitySettings.GetQualityLevel();
        string currentLevelName = QualitySettings.names[currentLevel];

        Debug.Log("Applying settings for quality level: " + currentLevelName);

        // FPS Ayarlama (VSync kapalýyken çalýþýr)
        if (currentLevelName == "MobileLow")
        {
            Application.targetFrameRate = targetFPS_Low;
            // lowQualityParticles?.SetActive(true);
            // mediumQualityParticles?.SetActive(false);
            // highQualityParticles?.SetActive(false);
            // if (postProcessingVolume != null) postProcessingVolume.profile = ppProfile_Low; // Veya kapatýlýr
        }
        else if (currentLevelName == "MobileMedium")
        {
            Application.targetFrameRate = targetFPS_Medium;
            // lowQualityParticles?.SetActive(false);
            // mediumQualityParticles?.SetActive(true);
            // highQualityParticles?.SetActive(false);
            // if (postProcessingVolume != null) postProcessingVolume.profile = ppProfile_Medium;
        }
        else if (currentLevelName == "MobileHigh")
        {
            Application.targetFrameRate = targetFPS_High;
            // lowQualityParticles?.SetActive(false);
            // mediumQualityParticles?.SetActive(false);
            // highQualityParticles?.SetActive(true);
            // if (postProcessingVolume != null) postProcessingVolume.profile = ppProfile_High;
        }
        else
        {
            // Tanýmlanmayan bir seviye için varsayýlan (güvenlik önlemi)
            Application.targetFrameRate = 45;
        }

        // Diðer oyun içi görsel ayarlarý burada yapabilirsiniz
        // - Parçacýk efekti yoðunluðunu deðiþtirme
        // - Post-processing profilini deðiþtirme veya kapatma
        // - Arka plan katmanlarýný açýp kapatma vb.
    }

    // Ayarlar menüsünde kullanmak için public metodlar (örnek)
    public void SetLowQuality() => SetQualityLevel(GetQualityLevelIndexByName("MobileLow"));
    public void SetMediumQuality() => SetQualityLevel(GetQualityLevelIndexByName("MobileMedium"));
    public void SetHighQuality() => SetQualityLevel(GetQualityLevelIndexByName("MobileHigh"));

    private int GetQualityLevelIndexByName(string name)
    {
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            if (QualitySettings.names[i] == name)
            {
                return i;
            }
        }
        Debug.LogError("Quality level not found: " + name);
        return 0; // Bulamazsa varsayýlaný döndür
    }
}
