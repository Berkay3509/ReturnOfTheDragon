using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("Bu HANGÝ seviye? (Örn: 1. seviye için 1, 2. seviye için 2)")]
    public int thisLevelNumber;

    // Oyuncu bu objeye temas ettiðinde (veya kazanma koþulu saðlandýðýnda)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Sadece oyuncu tetiklesin
        {
            CompleteThisLevel();
        }
    }

    // Bu fonksiyonu kazanma koþulunuzun olduðu yerden de çaðýrabilirsiniz.
    public void CompleteThisLevel()
    {
        Debug.Log("Level " + thisLevelNumber + " completed!");

        // LevelSelectMenu script'indeki static fonksiyonu çaðýrarak bir sonraki seviyenin kilidini aç
        UILevels.CompleteLevel(thisLevelNumber);
    }
}
