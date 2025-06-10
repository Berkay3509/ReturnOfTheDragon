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
            Debug.LogError("Sahnedeki TimeManager bulunamad�!");
        }
    }

    // Bu script'in eklendi�i GameObject'in bir Collider2D'si olmal�
    // ve "Is Trigger" �zelli�i i�aretli olmal�.
    // Oyuncunun da bir Rigidbody2D'si ve Collider2D'si olmal�.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Oyuncunun Tag'i TimeManager'da belirtti�imiz Tag ile e�le�iyorsa
        if (timeManager != null && other.CompareTag(timeManager.playerTag))
        {
            timeManager.PlayerCrossedFinishLine();
            // �ste�e ba�l�: Biti� �izgisini devre d��� b�rakarak tekrar tetiklenmesini engelle
            // gameObject.SetActive(false); 
        }
    }
}