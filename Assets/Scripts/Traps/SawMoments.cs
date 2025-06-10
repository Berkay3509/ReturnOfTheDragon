using UnityEngine;

public class SawMovements : MonoBehaviour
{
    // Hareket için deðiþkenler
    public float moveSpeed = 2f; // Testerenin yukarý aþaðý hareket hýzý
    public float moveDistance = 2f; // Testerenin hareket mesafesi (zincir boyunca ne kadar gidip geleceði)
    public Vector3 moveAxis = Vector3.up; // Hareket ekseni (yukarý-aþaðý için Vector3.up kullanýyoruz)
    private Vector3 startPosition; // Testerenin baþlangýç pozisyonu

    // Dönüþ için deðiþkenler
    public float rotationSpeed = 360f; // Testerenin dönüþ hýzý (derece/saniye)

    void Start()
    {
        // Testerenin baþlangýç pozisyonunu kaydet
        startPosition = transform.position;
    }

    void Update()
    {
        // Yukarý aþaðý hareket (PingPong ile sürekli gidip gelme)
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        Vector3 offset = moveAxis * movement;
        transform.position = startPosition + offset;

        // Testereyi kendi etrafýnda döndür
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}