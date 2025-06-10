using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("Bu HANG� seviye? (�rn: 1. seviye i�in 1, 2. seviye i�in 2)")]
    public int thisLevelNumber;

    // Oyuncu bu objeye temas etti�inde (veya kazanma ko�ulu sa�land���nda)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Sadece oyuncu tetiklesin
        {
            CompleteThisLevel();
        }
    }

    // Bu fonksiyonu kazanma ko�ulunuzun oldu�u yerden de �a��rabilirsiniz.
    public void CompleteThisLevel()
    {
        Debug.Log("Level " + thisLevelNumber + " completed!");

        // LevelSelectMenu script'indeki static fonksiyonu �a��rarak bir sonraki seviyenin kilidini a�
        UILevels.CompleteLevel(thisLevelNumber);
    }
}
