using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource; // Bu hala potansiyel bir sorun, aþaðýya bakýn

    private void Awake()
    {
        // ÖNCE: Bu objenin root olduðundan emin ol
        if (transform.parent != null)
        {
            Debug.LogWarning("SoundManager root deðildi, root yapýlýyor."); // Test için log
            transform.SetParent(null); // Ebeveyni kaldýr, root yap
        }

        // Sonra Singleton ve DontDestroyOnLoad mantýðý
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Artýk bu obje kesinlikle root olduðu için sorunsuz çalýþmalý

            // AudioSource'larý al (SoundManager root olduktan sonra)
            // SoundManager üzerindeki AudioSource
            soundSource = GetComponent<AudioSource>();
            if (soundSource == null)
            {
                Debug.LogError("SoundManager üzerinde AudioSource bulunamadý!");
            }

            // SoundManager'ýn ÝLK ÇOCUÐU üzerindeki AudioSource
            if (transform.childCount > 0)
            {
                musicSource = transform.GetChild(0).GetComponent<AudioSource>();
                if (musicSource == null)
                {
                    Debug.LogError("SoundManager'ýn ilk çocuðunda AudioSource bulunamadý!");
                }
            }
            else
            {
                Debug.LogError("SoundManager'ýn müzik için çocuðu bulunamadý!");
            }

            // Sadece ilk oluþturulduðunda ses seviyelerini ayarla
            // Eðer ses seviyeleri PlayerPrefs'ten okunuyorsa, 
            // bu baþlangýç deðerleri aslýnda PlayerPrefs'teki deðerlerle üzerine yazýlabilir.
            // Bu mantýðý gözden geçirmek isteyebilirsiniz.
            // Belki de PlayerPrefs'te deðer yoksa varsayýlanlarý ayarlamak daha mantýklýdýr.
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1f); // Varsayýlan olarak tam ses
            }
            if (!PlayerPrefs.HasKey("soundVolume"))
            {
                PlayerPrefs.SetFloat("soundVolume", 1f); // Varsayýlan olarak tam ses
            }

            ChangeMusicVolume(0); // Bu, mevcut PlayerPrefs deðerini okuyup uygulayacak
            ChangeSoundVolume(0); // Bu, mevcut PlayerPrefs deðerini okuyup uygulayacak
        }
        else if (instance != this) // 'instance != null' kontrolüne gerek yok, zaten yukarýda kontrol edildi
        {
            Destroy(gameObject);
        }
    }

    // ... (diðer metodlar ayný kalabilir)
    public void PlaySound(AudioClip _sound)
    {
        if (soundSource != null && _sound != null) // Ekstra güvenlik kontrolü
            soundSource.PlayOneShot(_sound);
        else
            Debug.LogWarning("PlaySound çaðrýldý ama soundSource veya _sound null.");
    }

    public void ChangeSoundVolume(float _change)
    {
        ChangeSourceVolume(1, "soundVolume", _change, soundSource);
    }
    public void ChangeMusicVolume(float _change)
    {
        ChangeSourceVolume(0.3f, "musicVolume", _change, musicSource); // baseVolume 0.3f, dikkat!
    }

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        if (source == null)
        {
            Debug.LogError(volumeName + " için AudioSource null, ses ayarlanamýyor.");
            return;
        }

        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1); // Varsayýlan 1 (tam ses)
        currentVolume += change;

        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }
}