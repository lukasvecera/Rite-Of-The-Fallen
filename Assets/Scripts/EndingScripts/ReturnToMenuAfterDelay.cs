using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuAfterDelay : MonoBehaviour
{
    [Header("Menu Return Settings")]
    [SerializeField] private float delay = 18f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(ReturnToMenuRoutine());

    }

    private System.Collections.IEnumerator ReturnToMenuRoutine()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(0);
    }
}
