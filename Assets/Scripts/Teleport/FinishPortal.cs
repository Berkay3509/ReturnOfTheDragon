using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPortal : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    private bool allEnemiesDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && allEnemiesDead)
        {
            OpenLevelCompleteMenu();
        }
    }

    public void CheckEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allEnemiesDead = enemies.Length == 0;
    }

    private void OpenLevelCompleteMenu()
    {
        // Menüyü aç
        LevelCompleteMenu.Instance.ShowMenu();
    }
}
