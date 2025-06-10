using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]

public class SpecificEnemyDoorController : MonoBehaviour
{
    // Kap�y� a�mak i�in yenilmesi gereken d��manlar�n listesi
    public List<GameObject> enemiesToDefeat;

    // Kap� a��ld���nda gizlenecek veya yok edilecek nesne (Inspector'dan atanacak)
    public GameObject objectToDisableOnOpen;

    // Ge�ilecek bir sonraki sahnenin ad� (opsiyonel)
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
        // �zlenecek d��manlar� ba�lang��ta ayarla
        enemiesToMonitor = new List<GameObject>();
        if (enemiesToDefeat != null)
        {
            foreach (GameObject enemy in enemiesToDefeat)
            {
                if (enemy != null) // Sadece var olan d��manlar� ekle
                {
                    enemiesToMonitor.Add(enemy);
                }
            }
        }

        // Ba�lang��ta hi� izlenecek d��man yoksa kap�y� ve ilgili i�lemi hemen yap
        if (enemiesToMonitor.Count == 0)
        {
            OpenDoor();
        }
        else
        {
            // D��man varsa kap�y� kilitli (fiziksel engel) yap
            isOpen = false;
            doorCollider.isTrigger = false;
        }
    }

    void Update()
    {
        // Kap� zaten a��ksa veya hi� d��man yoktuysa kontrole gerek yok
        if (isOpen)
        {
            return;
        }

        // �zleme listesindeki yok edilmi� (null olmu�) d��manlar� listeden ��kar
        // Geriye do�ru d�ng� kurmak, eleman ��kar�rken sorun ya�anmas�n� engeller
        for (int i = enemiesToMonitor.Count - 1; i >= 0; i--)
        {
            if (enemiesToMonitor[i] == null)
            {
                enemiesToMonitor.RemoveAt(i);
            }
        }

        // �zlenecek d��man kalmad�ysa kap�y� a�
        if (enemiesToMonitor.Count == 0)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        // Kap�n�n zaten a��k olup olmad���n� bir kez daha kontrol etmeye gerek yok,
        // ��nk� Update i�indeki kontrol bunu sa�l�yor, ama ek bir g�venlik katman� olabilir.
        if (isOpen) return;

        isOpen = true;

        // Kap�n�n Collider'�n� trigger yap (ge�ilebilir hale getir)
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }

        // --- YEN� EKLENEN KISIM ---
        // Belirtilen ek nesneyi deaktif et (gizle) veya yok et
        if (objectToDisableOnOpen != null)
        {
            // Se�enek 1: Nesneyi deaktif et (gizle) - Genellikle tercih edilir
            objectToDisableOnOpen.SetActive(false);
        }
        // --- YEN� EKLENEN KISIM SONU ---
        // Opsiyonel: Kap� a��lma animasyonu, ses efekti vs. burada tetiklenebilir.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kap� kilitliyken Player �arparsa (opsiyonel geri bildirim)
        if (!isOpen && collision.gameObject.CompareTag("Player"))
        {
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kap� a��ksa ve Player i�inden ge�erse
        if (isOpen && other.CompareTag("Player"))
        {
            // Sonraki sahne tan�ml�ysa y�kle
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                // gameObject.SetActive(false); // Opsiyonel: Kap�n�n kendisini de gizle/kapat
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enemiesToDefeat == null || enemiesToDefeat.Count == 0) return;

        Gizmos.color = Color.red;
        Vector3 doorPosition = transform.position;

        // Kap�dan d��manlara �izgiler �iz
        foreach (GameObject enemy in enemiesToDefeat)
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(doorPosition, enemy.transform.position);
            }
        }

        // Kap�dan gizlenecek/yok edilecek nesneye �izgi �iz (mavi renkte)
        if (objectToDisableOnOpen != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(doorPosition, objectToDisableOnOpen.transform.position);
        }
    }
}
