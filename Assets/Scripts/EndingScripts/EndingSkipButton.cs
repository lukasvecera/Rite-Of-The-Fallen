using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSkipButton : MonoBehaviour
{
public void SkipCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
