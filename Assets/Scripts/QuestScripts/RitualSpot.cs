using UnityEngine;

[DisallowMultipleComponent]
public class RitualSpot : MonoBehaviour, IInteractible
{
    public string requiredQuest;
    public string newQuest;
    [Header("Co sem patří (musí přesně sedět)")]
    public ItemSO requiredItem;

    [Header("Kde se má vizuál zobrazit")]
    public Transform visualAnchor;

    [HideInInspector] public ItemSO currentItem;
    private GameObject currentVisual;

    public System.Action<RitualSpot> OnChanged;

    public bool IsFilled => currentItem != null;

    public void Interact()
    {
        var inv = FindFirstObjectByType<PlayerInventory>();
        if (!inv) return;

        if (!IsFilled)
        {
            var selected = inv.GetSelectedItem();
            if (selected != requiredItem) return;
            if (requiredQuest != QuestManager.Instance.GetCurrentQuest()) return;

            if (inv.ConsumeSelectedIf(selected))
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.bookCloseSound);
                currentItem = selected;
                SpawnVisual(selected);
                QuestManager.Instance.SetQuest(newQuest);
                OnChanged?.Invoke(this);
            }
            return;
        }

        if (IsFilled && inv.AddItem(currentItem))
        {
            currentItem = null;
            if (currentVisual) Destroy(currentVisual);
            OnChanged?.Invoke(this);
        }
    }

    private void SpawnVisual(ItemSO item)
    {
        if (!visualAnchor) visualAnchor = transform;
        if (currentVisual) Destroy(currentVisual);

        var prefab = item.tableVisualPrefab;
        if (!prefab) return;

        currentVisual = Instantiate(prefab, visualAnchor.position, visualAnchor.rotation, visualAnchor);

        foreach (var c in currentVisual.GetComponentsInChildren<Collider>(true)) Destroy(c);
        var rb   = currentVisual.GetComponentInChildren<Rigidbody>(true);      if (rb) Destroy(rb);
        var pick = currentVisual.GetComponentInChildren<ItemPickable>(true);   if (pick) Destroy(pick);
    }

    public void ClearVisualOnly()
    {
        if (currentVisual) Destroy(currentVisual);
    }
}
