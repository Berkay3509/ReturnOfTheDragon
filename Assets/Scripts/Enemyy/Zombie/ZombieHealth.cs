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
    private bool dead; // Bu zaten �l�m durumunu takip ediyor

    // Kap� Kontrolc�s�n�n eri�ebilmesi i�in public property
    public bool IsDead => dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components; // D��man sald�r�s� vb. scriptleri buraya atanabilir
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound; // Bu zaten vard�, iyi

    // Event: Bu succubus �ld���nde DoorController'� bilgilendirmek i�in
    public static event Action<ZombieHealth> OnZombieDied;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        // dead ba�lang��ta false olmal�, bu zaten C# bool'lar�n�n varsay�lan�
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead) return; // �l�yse veya �l�ms�zse hasar almas�n

        currentHealth -= _damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("StunedTrigger"); // Hasar alma animasyonu
            if (hurtSound != null)
                AudioSource.PlayClipAtPoint(hurtSound, transform.position); // Hasar sesi
            StartCoroutine(Invunerability());
        }
        else if (!dead) // Sadece bir kere �lmesini sa�lamak i�in !dead kontrol� �nemli
        {
            Die(); // �l�m i�lemini ayr� bir metoda ta��yal�m
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
        if (rb != null) // E�er Rigidbody varsa ve �l�mde sorun ��kar�yorsa
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Veya tamamen devre d��� b�rak�labilir
        }
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position); // �l�m sesi


        OnZombieDied?.Invoke(this);
    }
    public void AddHealth(float _value)
    {
        if (dead) return; // �lm��se can eklenmesin
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // K�rm�z� ve yar� saydam
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
        // Belki Destroy(gameObject) da burada �a�r�labilir
        Destroy(gameObject);
    }
}
