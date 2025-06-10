using UnityEngine;

public class RotatingSawTrap : MonoBehaviour
{
    public Transform pivotPoint;  // Alt noktadaki pivot (a��rl�k)
    public float rotationSpeed = 50f;  // 360 derece d�nme h�z�
    public float sawRotationSpeed = 200f; // Testerenin kendi ekseninde d�nme h�z�
    public int damage = 10; // Verilecek hasar
    public float damageCooldown = 1f; // Hasar verme s�resi (1 saniyede 1 kez)

    private float lastDamageTime = 0f;

    void Update()
    {
        if (pivotPoint != null)
        {
            // Pivot noktas�n� merkez alarak 360 derece d�nd�rme
            transform.RotateAround(pivotPoint.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        // Testerenin kendi ekseninde d�nmesi
        transform.GetChild(0).Rotate(0, 0, sawRotationSpeed * Time.deltaTime);
    }

    // Oyuncu testereye temas etti�inde hasar al�r
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
