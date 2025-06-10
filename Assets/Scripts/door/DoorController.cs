using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public List<SuccubusHealth> succubiToDefeat;
    public string nextSceneName;

    private List<SuccubusHealth> activeSuccubi;
    private bool isOpen = false;
    private Collider2D doorCollider; // Kap�n�n collider'�n� tutmak i�in

    void Awake() // Start yerine Awake kullanmak daha iyi olabilir, collider'a erken eri�im i�in
    {
        doorCollider = GetComponent<Collider2D>();
        if (doorCollider == null)
        {
            Debug.LogError("DoorController: Kap� nesnesinde Collider2D bulunamad�!", this);
            enabled = false; // Script'i devre d��� b�rak
            return;
        }
    }

    void Start()
    {
        activeSuccubi = new List<SuccubusHealth>(succubiToDefeat);
        activeSuccubi.RemoveAll(succubus => succubus == null || succubus.IsDead);

        if (activeSuccubi.Count == 0)
        {
            Debug.Log("Ba�lang��ta hi� aktif succubus yok, kap� direkt a��k olmal�.");
            OpenDoor(); // Direkt kap�y� a� ve trigger yap
        }
        else
        {
            // Ba�lang��ta kap� kilitli ve fiziksel bir engel
            doorCollider.isTrigger = false;
            Debug.Log(activeSuccubi.Count + " succubus yenilmeyi bekliyor. Kap� K�L�TL� (Fiziksel Engel).");
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
            Debug.Log(diedSuccubus.name + " listeden ��kar�ld�. Kalan succubus: " + activeSuccubi.Count);
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
            doorCollider.isTrigger = true; // Kap�y� trigger yap, b�ylece i�inden ge�ilebilir
        }
        Debug.Log("T�m succubus'lar yenildi! Kap� A�ILDI (Art�k Trigger).");

        // Opsiyonel: Kap� a��lma animasyonu, ses efekti vs.
        // �rne�in, kap�n�n sprite'�n� de�i�tirebilirsiniz.
        // SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // if (sr != null && openDoorSprite != null) sr.sprite = openDoorSprite;
    }

    // Kap� F�Z�KSEL bir engelken (isTrigger = false) oyuncu �arpt���nda
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isOpen && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Kap� K�L�TL�! (Fiziksel Temas). �nce t�m succubus'lar� yenmelisin.");
            // Burada oyuncuya "Kap� kilitli" mesaj� g�sterebilirsiniz (UI ile) veya bir ses �alabilirsiniz.
        }
    }

    // Kap� A�IK ve TRIGGER oldu�unda (isTrigger = true) oyuncu i�inden ge�ti�inde
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player")) // Sadece kap� a��ksa ve Player tag'l� nesne ise
        {
            Debug.Log("Kap�dan ge�iliyor...");
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("NextSceneName tan�mlanmam��, kap�dan ge�ildi ama sahne de�i�medi.");
                // Belki kap�y� yok edebilir veya pasif hale getirebilirsiniz
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