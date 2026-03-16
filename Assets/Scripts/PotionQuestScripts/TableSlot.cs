using UnityEngine;

public class TableSlot : MonoBehaviour, IInteractible
{
    [Header("Reference")]
    public PotionTableController table;
    public Transform visualAnchor;

    [HideInInspector] public ItemSO currentItem;
    GameObject currentVisual;

    public bool IsEmpty => currentItem == null;

    public void Interact()
    {
        var inv = FindFirstObjectByType<PlayerInventory>();
        if (!inv) return;

        if (IsEmpty)
        {
            var sel = inv.GetSelectedItem();
            if (sel == null) return;
            if (table && !table.CanPlace(this, sel)) return;

            if (inv.ConsumeSelectedIf(sel))
            {
                currentItem = sel;
                SpawnVisualFor(sel);
                table?.NotifyChanged();
            }
            return;
        }

        if (inv.AddItem(currentItem))
        {
            currentItem = null;
            if (currentVisual) Destroy(currentVisual);
            table?.NotifyChanged();
        }
    }

void SpawnVisualFor(ItemSO item)
{
    if (!visualAnchor) visualAnchor = transform;
    if (currentVisual) Destroy(currentVisual);

    var prefab = item.tableVisualPrefab;
    if (prefab != null)
        currentVisual = Instantiate(prefab, visualAnchor.position, visualAnchor.rotation, visualAnchor);
    else
        Debug.LogWarning($"[{name}] Item {item.name} nemá nastavený tableVisualPrefab.");
}


}
