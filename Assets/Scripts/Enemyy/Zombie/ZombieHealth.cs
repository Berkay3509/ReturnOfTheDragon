using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead; // Bu zaten ölüm durumunu takip ediyor

    // Kapý Kontrolcüsünün eriþebilmesi için public property
    public bool IsDead => dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components; // Düþman saldýrýsý vb. scriptleri buraya atanabilir
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound; // Bu zaten vardý, iyi

    // Event: Bu succubus öldüðünde DoorController'ý bilgilendirmek için
    public static event Action<ZombieHealth> OnZombieDied;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        // dead baþlangýçta false olmalý, bu zaten C# bool'larýnýn varsayýlaný
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead) return; // Ölüyse veya ölümsüzse hasar almasýn

        currentHealth -= _damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("StunedTrigger"); // Hasar alma animasyonu
            if (hurtSound != null)
                AudioSource.PlayClipAtPoint(hurtSound, transform.position); // Hasar sesi
            StartCoroutine(Invunerability());
        }
        else if (!dead) // Sadece bir kere ölmesini saðlamak için !dead kontrolü önemli
        {
            Die(); // Ölüm iþlemini ayrý bir metoda taþýyalým
        }
    }

    private void Die()
    {
        if (dead) return;
        dead = true;
        anim.SetBool("DeathTrigger", true);
        anim.ResetTrigger("DeathTrigger");
        anim.SetTrigger("DeathTrigger");
        Destroy(gameObject, 2f);
        foreach (Behaviour component in components)
            component.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) // Eðer Rigidbody varsa ve ölümde sorun çýkarýyorsa
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Veya tamamen devre dýþý býrakýlabilir
        }
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position); // Ölüm sesi


        OnZombieDied?.Invoke(this);
    }
    public void AddHealth(float _value)
    {
        if (dead) return; // Ölmüþse can eklenmesin
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Kýrmýzý ve yarý saydam
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white; // Normal renk
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        invulnerable = false;
    }
    public void DisableAnimatorOnDeathEnd()
    {
        if (anim != null)
        {
            anim.enabled = false;
        }
        // Belki Destroy(gameObject) da burada çaðrýlabilir
        Destroy(gameObject);
    }
}
