using UnityEngine;

public class SawMovements : MonoBehaviour
{
    // Hareket i�in de�i�kenler
    public float moveSpeed = 2f; // Testerenin yukar� a�a�� hareket h�z�
    public float moveDistance = 2f; // Testerenin hareket mesafesi (zincir boyunca ne kadar gidip gelece�i)
    public Vector3 moveAxis = Vector3.up; // Hareket ekseni (yukar�-a�a�� i�in Vector3.up kullan�yoruz)
    private Vector3 startPosition; // Testerenin ba�lang�� pozisyonu

    // D�n�� i�in de�i�kenler
    public float rotationSpeed = 360f; // Testerenin d�n�� h�z� (derece/saniye)

    void Start()
    {
        // Testerenin ba�lang�� pozisyonunu kaydet
        startPosition = transform.position;
    }

    void Update()
    {
        // Yukar� a�a�� hareket (PingPong ile s�rekli gidip gelme)
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        Vector3 offset = moveAxis * movement;
        transform.position = startPosition + offset;

        // Testereyi kendi etraf�nda d�nd�r
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}