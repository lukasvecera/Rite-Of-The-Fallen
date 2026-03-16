using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject deathScreen;
    public GameObject areYouSureMainMenuPanel;

    private CharacterController cc;
    private Rigidbody rb;
    private PlayerMovement movement;


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();

        if (deathScreen != null)
            deathScreen.SetActive(false);

        if (areYouSureMainMenuPanel != null)
            areYouSureMainMenuPanel.SetActive(false);
    }

    public void Die()
    {
        if (deathScreen == null) return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.deathSound, 0.8f);
        Time.timeScale = 0f;

        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonSound, 0.3f);
        Time.timeScale = 1f;

        if (cc) cc.enabled = false;

        if (rb)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        if (cc) cc.enabled = true;

        if (movement != null)
        movement.ResetStaminaToMax();

        if (deathScreen != null)
            deathScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AreYouSureMainMenu()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonSound, 0.3f);
        deathScreen.SetActive(false);
        areYouSureMainMenuPanel.SetActive(true);
    }

    public void NoMainMenu() {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonSound, 0.3f);
        deathScreen.SetActive(true);
        areYouSureMainMenuPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonSound, 0.3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
