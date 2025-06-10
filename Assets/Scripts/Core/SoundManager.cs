using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource; // Bu hala potansiyel bir sorun, a�a��ya bak�n

    private void Awake()
    {
        // �NCE: Bu objenin root oldu�undan emin ol
        if (transform.parent != null)
        {
            Debug.LogWarning("SoundManager root de�ildi, root yap�l�yor."); // Test i�in log
            transform.SetParent(null); // Ebeveyni kald�r, root yap
        }

        // Sonra Singleton ve DontDestroyOnLoad mant���
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Art�k bu obje kesinlikle root oldu�u i�in sorunsuz �al��mal�

            // AudioSource'lar� al (SoundManager root olduktan sonra)
            // SoundManager �zerindeki AudioSource
            soundSource = GetComponent<AudioSource>();
            if (soundSource == null)
            {
                Debug.LogError("SoundManager �zerinde AudioSource bulunamad�!");
            }

            // SoundManager'�n �LK �OCU�U �zerindeki AudioSource
            if (transform.childCount > 0)
            {
                musicSource = transform.GetChild(0).GetComponent<AudioSource>();
                if (musicSource == null)
                {
                    Debug.LogError("SoundManager'�n ilk �ocu�unda AudioSource bulunamad�!");
                }
            }
            else
            {
                Debug.LogError("SoundManager'�n m�zik i�in �ocu�u bulunamad�!");
            }

            // Sadece ilk olu�turuldu�unda ses seviyelerini ayarla
            // E�er ses seviyeleri PlayerPrefs'ten okunuyorsa, 
            // bu ba�lang�� de�erleri asl�nda PlayerPrefs'teki de�erlerle �zerine yaz�labilir.
            // Bu mant��� g�zden ge�irmek isteyebilirsiniz.
            // Belki de PlayerPrefs'te de�er yoksa varsay�lanlar� ayarlamak daha mant�kl�d�r.
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1f); // Varsay�lan olarak tam ses
            }
            if (!PlayerPrefs.HasKey("soundVolume"))
            {
                PlayerPrefs.SetFloat("soundVolume", 1f); // Varsay�lan olarak tam ses
            }

            ChangeMusicVolume(0); // Bu, mevcut PlayerPrefs de�erini okuyup uygulayacak
            ChangeSoundVolume(0); // Bu, mevcut PlayerPrefs de�erini okuyup uygulayacak
        }
        else if (instance != this) // 'instance != null' kontrol�ne gerek yok, zaten yukar�da kontrol edildi
        {
            Destroy(gameObject);
        }
    }

    // ... (di�er metodlar ayn� kalabilir)
    public void PlaySound(AudioClip _sound)
    {
        if (soundSource != null && _sound != null) // Ekstra g�venlik kontrol�
            soundSource.PlayOneShot(_sound);
        else
            Debug.LogWarning("PlaySound �a�r�ld� ama soundSource veya _sound null.");
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
            Debug.LogError(volumeName + " i�in AudioSource null, ses ayarlanam�yor.");
            return;
        }

        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1); // Varsay�lan 1 (tam ses)
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