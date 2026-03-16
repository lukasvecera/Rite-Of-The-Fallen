using UnityEngine;

public class ItemPickable : MonoBehaviour, IInteractible
{
    public ItemSO item;

public void Interact()
{
    var inv = FindFirstObjectByType<PlayerInventory>();
    if (!inv) return;

    bool ok = inv.AddItem(item);
    if (ok)
        Destroy(gameObject);
    else
        Debug.Log("Nelze sebrat: inventář je plný");
}
}
