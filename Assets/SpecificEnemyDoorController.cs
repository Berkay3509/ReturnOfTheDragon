using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]

public class SpecificEnemyDoorController : MonoBehaviour
{
    // Kapýyý açmak için yenilmesi gereken düþmanlarýn listesi
    public List<GameObject> enemiesToDefeat;

    // Kapý açýldýðýnda gizlenecek veya yok edilecek nesne (Inspector'dan atanacak)
    public GameObject objectToDisableOnOpen;

    // Geçilecek bir sonraki sahnenin adý (opsiyonel)
    public string nextSceneName;

    private List<GameObject> enemiesToMonitor;
    private bool isOpen = false;
    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Ýzlenecek düþmanlarý baþlangýçta ayarla
        enemiesToMonitor = new List<GameObject>();
        if (enemiesToDefeat != null)
        {
            foreach (GameObject enemy in enemiesToDefeat)
            {
                if (enemy != null) // Sadece var olan düþmanlarý ekle
                {
                    enemiesToMonitor.Add(enemy);
                }
            }
        }

        // Baþlangýçta hiç izlenecek düþman yoksa kapýyý ve ilgili iþlemi hemen yap
        if (enemiesToMonitor.Count == 0)
        {
            OpenDoor();
        }
        else
        {
            // Düþman varsa kapýyý kilitli (fiziksel engel) yap
            isOpen = false;
            doorCollider.isTrigger = false;
        }
    }

    void Update()
    {
        // Kapý zaten açýksa veya hiç düþman yoktuysa kontrole gerek yok
        if (isOpen)
        {
            return;
        }

        // Ýzleme listesindeki yok edilmiþ (null olmuþ) düþmanlarý listeden çýkar
        // Geriye doðru döngü kurmak, eleman çýkarýrken sorun yaþanmasýný engeller
        for (int i = enemiesToMonitor.Count - 1; i >= 0; i--)
        {
            if (enemiesToMonitor[i] == null)
            {
                enemiesToMonitor.RemoveAt(i);
            }
        }

        // Ýzlenecek düþman kalmadýysa kapýyý aç
        if (enemiesToMonitor.Count == 0)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        // Kapýnýn zaten açýk olup olmadýðýný bir kez daha kontrol etmeye gerek yok,
        // çünkü Update içindeki kontrol bunu saðlýyor, ama ek bir güvenlik katmaný olabilir.
        if (isOpen) return;

        isOpen = true;

        // Kapýnýn Collider'ýný trigger yap (geçilebilir hale getir)
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }

        // --- YENÝ EKLENEN KISIM ---
        // Belirtilen ek nesneyi deaktif et (gizle) veya yok et
        if (objectToDisableOnOpen != null)
        {
            // Seçenek 1: Nesneyi deaktif et (gizle) - Genellikle tercih edilir
            objectToDisableOnOpen.SetActive(false);
        }
        // --- YENÝ EKLENEN KISIM SONU ---
        // Opsiyonel: Kapý açýlma animasyonu, ses efekti vs. burada tetiklenebilir.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kapý kilitliyken Player çarparsa (opsiyonel geri bildirim)
        if (!isOpen && collision.gameObject.CompareTag("Player"))
        {
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kapý açýksa ve Player içinden geçerse
        if (isOpen && other.CompareTag("Player"))
        {
            // Sonraki sahne tanýmlýysa yükle
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                // gameObject.SetActive(false); // Opsiyonel: Kapýnýn kendisini de gizle/kapat
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enemiesToDefeat == null || enemiesToDefeat.Count == 0) return;

        Gizmos.color = Color.red;
        Vector3 doorPosition = transform.position;

        // Kapýdan düþmanlara çizgiler çiz
        foreach (GameObject enemy in enemiesToDefeat)
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(doorPosition, enemy.transform.position);
            }
        }

        // Kapýdan gizlenecek/yok edilecek nesneye çizgi çiz (mavi renkte)
        if (objectToDisableOnOpen != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(doorPosition, objectToDisableOnOpen.transform.position);
        }
    }
}
