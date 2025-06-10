using UnityEngine;

public class RotatingSawDamage : MonoBehaviour
{
    public int damage = 10; // Testerenin verece�i hasar
    public float damageCooldown = 1f; // Hasar verme s�resi (1 saniyede 1 kez)

    private float lastDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastDamageTime = Time.time; // Hasar s�resi s�f�rlan�yor
            }
        }
    }
}
