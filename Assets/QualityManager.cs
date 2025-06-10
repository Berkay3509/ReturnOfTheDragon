using UnityEngine;

public class QualityManager : MonoBehaviour
{
    [Header("Frame Rate Targets")]
    [SerializeField] private int targetFPS_Low = 60;
    [SerializeField] private int targetFPS_Medium = 60; 
    [SerializeField] private int targetFPS_High = 60;

    // �rne�in, par�ac�k efektleri veya post-processing profilleri i�in referanslar
    // [SerializeField] private GameObject lowQualityParticles;
    // [SerializeField] private GameObject mediumQualityParticles;
    // [SerializeField] private GameObject highQualityParticles;
    // [SerializeField] private UnityEngine.Rendering.Volume postProcessingVolume;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_Low;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_Medium;
    // [SerializeField] private UnityEngine.Rendering.VolumeProfile ppProfile_High;


    void Start()
    {
        // Oyun ba�lad���nda mevcut kalite seviyesine g�re ayarlar� uygula
        ApplyQualitySettings();
    }

    public void SetQualityLevel(int levelIndex)
    {
        // Kalite seviyesini de�i�tir (�rne�in, Ayarlar men�s�nden �a�r�l�r)
        // Level index'leri Project Settings > Quality'deki s�raya g�redir.
        // Emin olmak i�in isimle kar��la�t�rma da yapabilirsiniz.
        QualitySettings.SetQualityLevel(levelIndex, true); // 'true' de�i�ikli�i hemen uygular
        Debug.Log("Quality level set to: " + QualitySettings.names[levelIndex]);
        ApplyQualitySettings();
    }

    void ApplyQualitySettings()
    {
        int currentLevel = QualitySettings.GetQualityLevel();
        string currentLevelName = QualitySettings.names[currentLevel];

        Debug.Log("Applying settings for quality level: " + currentLevelName);

        // FPS Ayarlama (VSync kapal�yken �al���r)
        if (currentLevelName == "MobileLow")
        {
            Application.targetFrameRate = targetFPS_Low;
            // lowQualityParticles?.SetActive(true);
            // mediumQualityParticles?.SetActive(false);
            // highQualityParticles?.SetActive(false);
            // if (postProcessingVolume != null) postProcessingVolume.profile = ppProfile_Low; // Veya kapat�l�r
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
            // Tan�mlanmayan bir seviye i�in varsay�lan (g�venlik �nlemi)
            Application.targetFrameRate = 45;
        }

        // Di�er oyun i�i g�rsel ayarlar� burada yapabilirsiniz
        // - Par�ac�k efekti yo�unlu�unu de�i�tirme
        // - Post-processing profilini de�i�tirme veya kapatma
        // - Arka plan katmanlar�n� a��p kapatma vb.
    }

    // Ayarlar men�s�nde kullanmak i�in public metodlar (�rnek)
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
        return 0; // Bulamazsa varsay�lan� d�nd�r
    }
}
