using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private TimeManager timeManager;

    void Start()
    {
        // TimeManager script'ini sahneden bul
        timeManager = FindObjectOfType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("Sahnedeki TimeManager bulunamadý!");
        }
    }

    // Bu script'in eklendiði GameObject'in bir Collider2D'si olmalý
    // ve "Is Trigger" özelliði iþaretli olmalý.
    // Oyuncunun da bir Rigidbody2D'si ve Collider2D'si olmalý.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Oyuncunun Tag'i TimeManager'da belirttiðimiz Tag ile eþleþiyorsa
        if (timeManager != null && other.CompareTag(timeManager.playerTag))
        {
            timeManager.PlayerCrossedFinishLine();
            // Ýsteðe baðlý: Bitiþ çizgisini devre dýþý býrakarak tekrar tetiklenmesini engelle
            // gameObject.SetActive(false); 
        }
    }
}