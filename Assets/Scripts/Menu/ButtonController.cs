using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject lockIconObject;
    public int buttonValue;
    private bool isComplete;

    public void SetLockstate()
    {
        isComplete = true;

        if (isComplete)
        {
           
        }
    }
}
