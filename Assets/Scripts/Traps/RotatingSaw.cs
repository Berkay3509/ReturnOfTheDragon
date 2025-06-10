using UnityEngine;

public class RotatingSawTrap : MonoBehaviour
{
    public Transform pivotPoint;  // Alt noktadaki pivot (aðýrlýk)
    public float rotationSpeed = 50f;  // 360 derece dönme hýzý
    public float sawRotationSpeed = 200f; // Testerenin kendi ekseninde dönme hýzý
    public int damage = 10; // Verilecek hasar
    public float damageCooldown = 1f; // Hasar verme süresi (1 saniyede 1 kez)

    private float lastDamageTime = 0f;

    void Update()
    {
        if (pivotPoint != null)
        {
            // Pivot noktasýný merkez alarak 360 derece döndürme
            transform.RotateAround(pivotPoint.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        // Testerenin kendi ekseninde dönmesi
        transform.GetChild(0).Rotate(0, 0, sawRotationSpeed * Time.deltaTime);
    }

    // Oyuncu testereye temas ettiðinde hasar alýr
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastDamageTime = Time.time; // Hasar süresi sýfýrlanýyor
            }
        }
    }
}
