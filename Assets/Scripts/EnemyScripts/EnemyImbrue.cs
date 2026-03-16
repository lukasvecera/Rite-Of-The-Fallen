using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyImbrue : MonoBehaviour, IInteractible
{
    public ItemSO requiredGoblet;
    PlayerInventory inv;

    void Awake()
    {
        inv = FindFirstObjectByType<PlayerInventory>();
    }

    public void Interact()
    {
        if (inv != null && inv.GetSelectedItem() == requiredGoblet)
        {
            SceneManager.LoadScene(2);
        }
    }
}
