using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameName;
    public Canvas confirmMenu;
    public Canvas controlsMenu;

    [SerializeField] private AudioManagerMenu audioManagerMenu;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        confirmMenu.enabled = false;
        controlsMenu.enabled = false;
    }

    public void PlayGame()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        SceneManager.LoadScene(1);
    }

    public void ClickControls()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        controlsMenu.enabled = true;
        mainMenu.SetActive(false);
        gameName.SetActive(false);
    }

    public void ClickBack()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        controlsMenu.enabled = false;
        mainMenu.SetActive(true);
        gameName.SetActive(true);
    }

    public void QuitGame()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        confirmMenu.enabled = true;
        mainMenu.SetActive(false);
    }

    public void ClickYes()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        Application.Quit();
    }

    public void ClickNo()
    {
        if (audioManagerMenu != null)
            audioManagerMenu.PlaySFX(audioManagerMenu.click);

        confirmMenu.enabled = false;
        mainMenu.SetActive(true);
    }
}
