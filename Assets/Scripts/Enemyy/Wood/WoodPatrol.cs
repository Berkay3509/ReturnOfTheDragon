using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPatrol : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Tooltip("Movement speed of the character.")]
    [SerializeField] private float speed = 2f;
    [Tooltip("Array of waypoints the character will patrol between.")]
    [SerializeField] private Transform[] waypoints;
    [Tooltip("Duration the character will idle at a waypoint.")]
    [SerializeField] private float idleDuration = 1f;
    [Tooltip("Should the character start moving immediately on Start?")]
    [SerializeField] private bool startImmediately = true;

    [Header("Player Detection")]
    [Tooltip("Size of the player detection box.")]
    [SerializeField] private Vector2 detectionSize = new Vector2(5f, 2f);
    [Tooltip("Offset of the player detection box relative to the character's position.")]
    [SerializeField] private Vector2 detectionOffset = new Vector2(1f, 0f);
    [Tooltip("Layer mask to specify what is considered the player.")]
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [Tooltip("Animator component for controlling animations.")]
    [SerializeField] private Animator anim;
    [Tooltip("Reference to the player's transform (optional, used by some detection logic).")]
    [SerializeField] private Transform player; // Bu scriptte aktif olarak kullan�lm�yor ama referans olarak kalabilir.

    private int currentWaypointIndex = 0;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isFacingRight; // Sprite'�n sa�a bak�p bakmad���n� takip eder
    private bool isPlayerDetected = false;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            // Ba�lang��ta hangi y�ne bakaca��n� belirle
            DetermineInitialFacingDirection();
            // Karakterin pozisyonunu ilk waypoint'in Y seviyesine ayarlayabiliriz (iste�e ba�l�)
            // transform.position = new Vector2(transform.position.x, waypoints[currentWaypointIndex].position.y);
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": No waypoints assigned for patrol.", this);
        }

        if (anim == null)
        {
            Debug.LogWarning(gameObject.name + ": Animator not assigned.", this);
        }
        else
        {
            // Ba�lang��ta hareket animasyonunu ayarla
            anim.SetBool("MoveTrigger", !isIdle && startImmediately && waypoints.Length > 0);
        }
    }

    private void Update()
    {
        CheckPlayerDetection();

        // E�er oyuncu tespit edildiyse ve hareket ediyorsa, hareketi durdur
        if (isPlayerDetected)
        {
            if (anim != null && anim.GetBool("MoveTrigger"))
            {
                anim.SetBool("MoveTrigger", false); // Hareket animasyonunu durdur
            }
            // Oyuncu tespit edildi�inde yap�lacak ek davran��lar buraya eklenebilir (�rn: sald�r�)
            return; // Oyuncu tespit edildiyse devriye mant���n� �al��t�rma
        }

        // Waypoint yoksa veya animat�r yoksa bir �ey yapma
        if (waypoints.Length == 0) return;

        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                if (anim != null) anim.SetBool("MoveTrigger", true); // Bekleme bitti, harekete ge�
            }
        }
        else
        {
            // Sadece oyuncu tespit edilmediyse hareket et
            MoveToWaypoint();
        }
    }

    private void CheckPlayerDetection()
    {
        // Karakterin bakt��� y�ne g�re alg�lama kutusunun merkezini hesapla
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    isFacingRight ? detectionOffset.x : -detectionOffset.x,
                                    detectionOffset.y
                                );

        // Belirlenen alanda oyuncu var m� diye kontrol et
        bool detectedThisFrame = Physics2D.OverlapBox(
            detectionCenter,
            detectionSize,
            0f, // A��
            playerLayer);

        // Alg�lama durumu de�i�tiyse
        if (detectedThisFrame && !isPlayerDetected)
        {
            isPlayerDetected = true;
           
            if (anim != null) anim.SetBool("MoveTrigger", false); // Oyuncu alg�land���nda hareket animasyonunu durdur
            // Burada oyuncu ilk alg�land���nda yap�lacaklar� ekleyebilirsiniz (�rn: Uyar� sesi, animasyon)
        }
        else if (!detectedThisFrame && isPlayerDetected)
        {
            isPlayerDetected = false;
            Debug.Log(gameObject.name + " lost player.");
            // Oyuncu alg�lama alan�ndan ��kt���nda ve bekleme durumunda de�ilse tekrar hareket et
            if (anim != null && !isIdle) anim.SetBool("MoveTrigger", true);
        }
    }

    private void DetermineInitialFacingDirection()
    {
        if (waypoints.Length == 0 || waypoints[currentWaypointIndex] == null) return;

        // �lk waypoint'e olan X eksenindeki y�n� al
        float directionToWaypoint = waypoints[currentWaypointIndex].position.x - transform.position.x;

        // E�er waypoint karakterin sa��ndaysa sa�a, solundaysa sola bak
        isFacingRight = directionToWaypoint > 0;
        UpdateSpriteDirection();
    }

    private void MoveToWaypoint()
    {
        if (waypoints.Length == 0 || waypoints[currentWaypointIndex] == null) return;

        // Hedef pozisyonu al, ancak mevcut Y pozisyonunu koru (dikey hareketi engelle)
        Vector2 targetPosition = new Vector2(
            waypoints[currentWaypointIndex].position.x,
            transform.position.y // Orijinal Y pozisyonunu koru
        );

        // Hedefe do�ru hareket et
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Sprite y�n�n� g�ncellemek i�in hedefle mevcut pozisyon aras�ndaki farka bak
        // Bu, karakterin waypoint'e ula�t���nda ve geri d�nerken y�n�n� hemen de�i�tirmesini sa�lar
        float moveDirection = targetPosition.x - transform.position.x;

        // Sadece anlaml� bir hareket varsa y�n� g�ncelle
        if (Mathf.Abs(moveDirection) > 0.01f) // K���k bir e�ik de�er
        {
            bool shouldFaceRight = moveDirection > 0;
            if (isFacingRight != shouldFaceRight)
            {
                isFacingRight = shouldFaceRight;
                UpdateSpriteDirection();
            }
        }

        // Waypoint'e yeterince yakla��ld� m� kontrol et
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), targetPosition) < 0.1f)
        {
            // Bir sonraki waypoint'e ge� ve bekleme moduna gir
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isIdle = true;
            idleTimer = 0f;
            if (anim != null) anim.SetBool("MoveTrigger", false); // Waypoint'e ula��ld�, hareket animasyonunu durdur

            // Yeni waypoint'e do�ru bakmas�n� sa�la (bekleme bittikten sonra do�ru y�ne hareket etmesi i�in)
            // E�er hemen d�nmesini istemiyorsan�z bu sat�r� yorum sat�r� yap�n veya StartIdle gibi bir fonksiyona ta��y�n.
            DetermineInitialFacingDirection();
        }
    }

    private void UpdateSpriteDirection()
    {
        // Sprite'�n X �l�e�ini ters �evirerek y�n�n� de�i�tir
        // Karakterin sprite'�n�n ba�lang��ta sola bakt���n� varsayarsak (scale.x pozitifken sola bak�yorsa):
        // Sa�a bakmas� i�in scale.x negatif olmal�.
        // Sola bakmas� i�in scale.x pozitif olmal�.
        // E�er sprite'�n�z ba�lang��ta sa�a bak�yorsa (scale.x pozitifken sa�a bak�yorsa):
        // isFacingRight ? 1 : -1 �eklinde kullan�n.
        // Benim kodum genellikle sprite'�n varsay�lan olarak sa�a bakt��� (scale.x = 1) ve
        // sola d�nmesi i�in scale.x = -1 olmas� gerekti�i varsay�m�yla yaz�l�r.
        // Ancak senin verdi�in kodda scale.x = Mathf.Abs(scale.x) * (isFacingRight ? -1 : 1);
        // Bu, scale.x pozitifken sprite'�n Sola bakt��� anlam�na gelir. (isFacingRight true ise -1 ile �arp�l�r)
        // Bu mant��� koruyorum:
        Vector3 scale = transform.localScale;
        if (isFacingRight)
        {
            scale.x = -Mathf.Abs(scale.x); // Sa�a bak�yorsa scale.x negatif
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);  // Sola bak�yorsa scale.x pozitif
        }
        transform.localScale = scale;
    }

    // Edit�rde alg�lama alan�n� ve waypoint yolunu �izmek i�in
    private void OnDrawGizmosSelected()
    {
        // Alg�lama alan� (oyuncu alg�land���nda k�rm�z�, yoksa sar�)
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    (transform.localScale.x > 0 ? -detectionOffset.x : detectionOffset.x), // Sprite y�n�ne g�re d�zeltme
                                    detectionOffset.y
                                );
        // E�er UpdateSpriteDirection'daki mant��a g�re isFacingRight kullanmak daha do�ruysa:
        // Vector2 detectionCenter = (Vector2)transform.position +
        //                         new Vector2(
        //                             isFacingRight ? detectionOffset.x : -detectionOffset.x,
        //                             detectionOffset.y
        //                         );
        Gizmos.DrawWireCube(detectionCenter, detectionSize);

        // Waypoint yolu (mavi)
        Gizmos.color = Color.blue;
        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 previousWaypointPosition = new Vector2(transform.position.x, transform.position.y); // Ba�lang�� noktas� olarak karakterin mevcut konumu

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // T�m waypoint'leri karakterin mevcut Y seviyesinde �iz
                    Vector2 wpPos = new Vector2(waypoints[i].position.x, transform.position.y);
                    Gizmos.DrawSphere(wpPos, 0.2f);
                    if (i == 0) // �lk waypoint ise, karakterden ilk waypoint'e �izgi �iz
                    {
                        Gizmos.DrawLine(previousWaypointPosition, wpPos);
                    }
                    if (i > 0 && waypoints[i - 1] != null) // �nceki waypoint'ten mevcut waypoint'e
                    {
                        Vector2 prevWpActualPos = new Vector2(waypoints[i - 1].position.x, transform.position.y);
                        Gizmos.DrawLine(prevWpActualPos, wpPos);
                    }
                    // Son waypoint'ten ilk waypoint'e d�ng�sel bir �izgi �izmek isterseniz:
                    if (i == waypoints.Length - 1 && waypoints[0] != null)
                    {
                        Vector2 firstWpPos = new Vector2(waypoints[0].position.x, transform.position.y);
                        Gizmos.DrawLine(wpPos, firstWpPos);
                    }
                }
            }
        }
    }
}