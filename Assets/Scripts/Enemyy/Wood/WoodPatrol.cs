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
    [SerializeField] private Transform player; // Bu scriptte aktif olarak kullanýlmýyor ama referans olarak kalabilir.

    private int currentWaypointIndex = 0;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isFacingRight; // Sprite'ýn saða bakýp bakmadýðýný takip eder
    private bool isPlayerDetected = false;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            // Baþlangýçta hangi yöne bakacaðýný belirle
            DetermineInitialFacingDirection();
            // Karakterin pozisyonunu ilk waypoint'in Y seviyesine ayarlayabiliriz (isteðe baðlý)
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
            // Baþlangýçta hareket animasyonunu ayarla
            anim.SetBool("MoveTrigger", !isIdle && startImmediately && waypoints.Length > 0);
        }
    }

    private void Update()
    {
        CheckPlayerDetection();

        // Eðer oyuncu tespit edildiyse ve hareket ediyorsa, hareketi durdur
        if (isPlayerDetected)
        {
            if (anim != null && anim.GetBool("MoveTrigger"))
            {
                anim.SetBool("MoveTrigger", false); // Hareket animasyonunu durdur
            }
            // Oyuncu tespit edildiðinde yapýlacak ek davranýþlar buraya eklenebilir (örn: saldýrý)
            return; // Oyuncu tespit edildiyse devriye mantýðýný çalýþtýrma
        }

        // Waypoint yoksa veya animatör yoksa bir þey yapma
        if (waypoints.Length == 0) return;

        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                if (anim != null) anim.SetBool("MoveTrigger", true); // Bekleme bitti, harekete geç
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
        // Karakterin baktýðý yöne göre algýlama kutusunun merkezini hesapla
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    isFacingRight ? detectionOffset.x : -detectionOffset.x,
                                    detectionOffset.y
                                );

        // Belirlenen alanda oyuncu var mý diye kontrol et
        bool detectedThisFrame = Physics2D.OverlapBox(
            detectionCenter,
            detectionSize,
            0f, // Açý
            playerLayer);

        // Algýlama durumu deðiþtiyse
        if (detectedThisFrame && !isPlayerDetected)
        {
            isPlayerDetected = true;
           
            if (anim != null) anim.SetBool("MoveTrigger", false); // Oyuncu algýlandýðýnda hareket animasyonunu durdur
            // Burada oyuncu ilk algýlandýðýnda yapýlacaklarý ekleyebilirsiniz (örn: Uyarý sesi, animasyon)
        }
        else if (!detectedThisFrame && isPlayerDetected)
        {
            isPlayerDetected = false;
            Debug.Log(gameObject.name + " lost player.");
            // Oyuncu algýlama alanýndan çýktýðýnda ve bekleme durumunda deðilse tekrar hareket et
            if (anim != null && !isIdle) anim.SetBool("MoveTrigger", true);
        }
    }

    private void DetermineInitialFacingDirection()
    {
        if (waypoints.Length == 0 || waypoints[currentWaypointIndex] == null) return;

        // Ýlk waypoint'e olan X eksenindeki yönü al
        float directionToWaypoint = waypoints[currentWaypointIndex].position.x - transform.position.x;

        // Eðer waypoint karakterin saðýndaysa saða, solundaysa sola bak
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

        // Hedefe doðru hareket et
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Sprite yönünü güncellemek için hedefle mevcut pozisyon arasýndaki farka bak
        // Bu, karakterin waypoint'e ulaþtýðýnda ve geri dönerken yönünü hemen deðiþtirmesini saðlar
        float moveDirection = targetPosition.x - transform.position.x;

        // Sadece anlamlý bir hareket varsa yönü güncelle
        if (Mathf.Abs(moveDirection) > 0.01f) // Küçük bir eþik deðer
        {
            bool shouldFaceRight = moveDirection > 0;
            if (isFacingRight != shouldFaceRight)
            {
                isFacingRight = shouldFaceRight;
                UpdateSpriteDirection();
            }
        }

        // Waypoint'e yeterince yaklaþýldý mý kontrol et
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), targetPosition) < 0.1f)
        {
            // Bir sonraki waypoint'e geç ve bekleme moduna gir
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isIdle = true;
            idleTimer = 0f;
            if (anim != null) anim.SetBool("MoveTrigger", false); // Waypoint'e ulaþýldý, hareket animasyonunu durdur

            // Yeni waypoint'e doðru bakmasýný saðla (bekleme bittikten sonra doðru yöne hareket etmesi için)
            // Eðer hemen dönmesini istemiyorsanýz bu satýrý yorum satýrý yapýn veya StartIdle gibi bir fonksiyona taþýyýn.
            DetermineInitialFacingDirection();
        }
    }

    private void UpdateSpriteDirection()
    {
        // Sprite'ýn X ölçeðini ters çevirerek yönünü deðiþtir
        // Karakterin sprite'ýnýn baþlangýçta sola baktýðýný varsayarsak (scale.x pozitifken sola bakýyorsa):
        // Saða bakmasý için scale.x negatif olmalý.
        // Sola bakmasý için scale.x pozitif olmalý.
        // Eðer sprite'ýnýz baþlangýçta saða bakýyorsa (scale.x pozitifken saða bakýyorsa):
        // isFacingRight ? 1 : -1 þeklinde kullanýn.
        // Benim kodum genellikle sprite'ýn varsayýlan olarak saða baktýðý (scale.x = 1) ve
        // sola dönmesi için scale.x = -1 olmasý gerektiði varsayýmýyla yazýlýr.
        // Ancak senin verdiðin kodda scale.x = Mathf.Abs(scale.x) * (isFacingRight ? -1 : 1);
        // Bu, scale.x pozitifken sprite'ýn Sola baktýðý anlamýna gelir. (isFacingRight true ise -1 ile çarpýlýr)
        // Bu mantýðý koruyorum:
        Vector3 scale = transform.localScale;
        if (isFacingRight)
        {
            scale.x = -Mathf.Abs(scale.x); // Saða bakýyorsa scale.x negatif
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);  // Sola bakýyorsa scale.x pozitif
        }
        transform.localScale = scale;
    }

    // Editörde algýlama alanýný ve waypoint yolunu çizmek için
    private void OnDrawGizmosSelected()
    {
        // Algýlama alaný (oyuncu algýlandýðýnda kýrmýzý, yoksa sarý)
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    (transform.localScale.x > 0 ? -detectionOffset.x : detectionOffset.x), // Sprite yönüne göre düzeltme
                                    detectionOffset.y
                                );
        // Eðer UpdateSpriteDirection'daki mantýða göre isFacingRight kullanmak daha doðruysa:
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
            Vector3 previousWaypointPosition = new Vector2(transform.position.x, transform.position.y); // Baþlangýç noktasý olarak karakterin mevcut konumu

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // Tüm waypoint'leri karakterin mevcut Y seviyesinde çiz
                    Vector2 wpPos = new Vector2(waypoints[i].position.x, transform.position.y);
                    Gizmos.DrawSphere(wpPos, 0.2f);
                    if (i == 0) // Ýlk waypoint ise, karakterden ilk waypoint'e çizgi çiz
                    {
                        Gizmos.DrawLine(previousWaypointPosition, wpPos);
                    }
                    if (i > 0 && waypoints[i - 1] != null) // Önceki waypoint'ten mevcut waypoint'e
                    {
                        Vector2 prevWpActualPos = new Vector2(waypoints[i - 1].position.x, transform.position.y);
                        Gizmos.DrawLine(prevWpActualPos, wpPos);
                    }
                    // Son waypoint'ten ilk waypoint'e döngüsel bir çizgi çizmek isterseniz:
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