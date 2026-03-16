using UnityEngine;

public class PotionTableController : MonoBehaviour, IInteractible
{
    public string reuiredQuest;
    public string newQuest;

    [Header("Sloty na stole (pořadí zleva doprava)")]
    public TableSlot[] slots;

    [Header("Požadované pořadí lektvarů")]
    public ItemSO[] requiredOrder;

    [Header("Kalichy")]
    public ItemSO gobletEmpty;
    public ItemSO gobletFull;

    [Header("Blueprinty nad sloty (stejné pořadí jako slots)")]
    public GameObject[] potionBlueprints;


    void Start()
    {
        UpdateBlueprints();
    }

    public bool CanPlace(TableSlot slot, ItemSO item)
    {
        if (requiredOrder == null || requiredOrder.Length == 0) return true;
        foreach (var need in requiredOrder) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupItemSound);
            if (need == item) {
                return true;
            }
        }
        return false;
    }

    public void NotifyChanged()
    {
        UpdateBlueprints();
    }

    void UpdateBlueprints()
    {
        if (potionBlueprints == null || slots == null) return;

        for (int i = 0; i < potionBlueprints.Length; i++)
        {
            if (!potionBlueprints[i]) continue;

            bool hasItem = false;

            if (i < slots.Length && slots[i] != null)
            {
                hasItem = (slots[i].currentItem != null);
            }

            potionBlueprints[i].SetActive(!hasItem);
        }
    }

    public bool IsCompleteAndCorrect()
    {
        if (slots == null || requiredOrder == null) return false;
        if (slots.Length != requiredOrder.Length) return false;

        for (int i = 0; i < slots.Length; i++)
            if (slots[i].currentItem != requiredOrder[i])
                return false;

        return true;
    }

    public void Interact()
    {
        var inv = FindFirstObjectByType<PlayerInventory>();
        if (!inv) return;

        if (!IsCompleteAndCorrect()) return;

        var sel = inv.GetSelectedItem();
        if (sel != gobletEmpty) return;

        if (reuiredQuest != QuestManager.Instance.GetCurrentQuest()) return;

        if (inv.ConsumeSelectedIf(gobletEmpty))
        {
            inv.AddItem(gobletFull);
            QuestManager.Instance.SetQuest(newQuest);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.potionPouringSound);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);
        }
    }
}
