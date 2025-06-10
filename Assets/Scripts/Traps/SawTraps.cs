using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public float swingSpeed = 2f; // Sallanma h�z�
    public float swingAmount = 45f; // Maksimum a��sal sapma
    public float rotationSpeed = 200f; // Testerenin kendi etraf�nda d�nme h�z�
    public int damage = 10; // Verilecek hasar
    public float damageCooldown = 1f; // Hasar verme s�resi (1 saniyede 1 kez)

    private float lastDamageTime = 0f;
    private float startRotation;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        // Sarka� Hareketi (Sa�a-Sola Sal�n�m)
        float angle = startRotation + Mathf.Sin(Time.time * swingSpeed) * swingAmount;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Oyuncu testerenin i�inde kald��� s�rece hasar vermeye devam eder
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
