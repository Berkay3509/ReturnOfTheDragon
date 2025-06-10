using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public List<SuccubusHealth> succubiToDefeat;
    public string nextSceneName;

    private List<SuccubusHealth> activeSuccubi;
    private bool isOpen = false;
    private Collider2D doorCollider; // Kapýnýn collider'ýný tutmak için

    void Awake() // Start yerine Awake kullanmak daha iyi olabilir, collider'a erken eriþim için
    {
        doorCollider = GetComponent<Collider2D>();
        if (doorCollider == null)
        {
            Debug.LogError("DoorController: Kapý nesnesinde Collider2D bulunamadý!", this);
            enabled = false; // Script'i devre dýþý býrak
            return;
        }
    }

    void Start()
    {
        activeSuccubi = new List<SuccubusHealth>(succubiToDefeat);
        activeSuccubi.RemoveAll(succubus => succubus == null || succubus.IsDead);

        if (activeSuccubi.Count == 0)
        {
            Debug.Log("Baþlangýçta hiç aktif succubus yok, kapý direkt açýk olmalý.");
            OpenDoor(); // Direkt kapýyý aç ve trigger yap
        }
        else
        {
            // Baþlangýçta kapý kilitli ve fiziksel bir engel
            doorCollider.isTrigger = false;
            Debug.Log(activeSuccubi.Count + " succubus yenilmeyi bekliyor. Kapý KÝLÝTLÝ (Fiziksel Engel).");
        }
    }

    void OnEnable()
    {
        SuccubusHealth.OnSuccubusDied += HandleSuccubusDied;
    }

    void OnDisable()
    {
        SuccubusHealth.OnSuccubusDied -= HandleSuccubusDied;
    }

    void HandleSuccubusDied(SuccubusHealth diedSuccubus)
    {
        if (activeSuccubi.Contains(diedSuccubus))
        {
            activeSuccubi.Remove(diedSuccubus);
            Debug.Log(diedSuccubus.name + " listeden çýkarýldý. Kalan succubus: " + activeSuccubi.Count);
            CheckSuccubi();
        }
    }

    void CheckSuccubi()
    {
        if (activeSuccubi.Count == 0 && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true; // Kapýyý trigger yap, böylece içinden geçilebilir
        }
        Debug.Log("Tüm succubus'lar yenildi! Kapý AÇILDI (Artýk Trigger).");

        // Opsiyonel: Kapý açýlma animasyonu, ses efekti vs.
        // Örneðin, kapýnýn sprite'ýný deðiþtirebilirsiniz.
        // SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // if (sr != null && openDoorSprite != null) sr.sprite = openDoorSprite;
    }

    // Kapý FÝZÝKSEL bir engelken (isTrigger = false) oyuncu çarptýðýnda
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isOpen && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Kapý KÝLÝTLÝ! (Fiziksel Temas). Önce tüm succubus'larý yenmelisin.");
            // Burada oyuncuya "Kapý kilitli" mesajý gösterebilirsiniz (UI ile) veya bir ses çalabilirsiniz.
        }
    }

    // Kapý AÇIK ve TRIGGER olduðunda (isTrigger = true) oyuncu içinden geçtiðinde
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player")) // Sadece kapý açýksa ve Player tag'lý nesne ise
        {
            Debug.Log("Kapýdan geçiliyor...");
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("NextSceneName tanýmlanmamýþ, kapýdan geçildi ama sahne deðiþmedi.");
                // Belki kapýyý yok edebilir veya pasif hale getirebilirsiniz
                // gameObject.SetActive(false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (succubiToDefeat == null || succubiToDefeat.Count == 0) return;
        Gizmos.color = Color.magenta;
        foreach (SuccubusHealth succubus in succubiToDefeat)
        {
            if (succubus != null)
            {
                Gizmos.DrawLine(transform.position, succubus.transform.position);
            }
        }
    }
}