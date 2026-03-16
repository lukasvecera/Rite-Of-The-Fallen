using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class ItemBind
    {
        public ItemSO item;
        public GameObject worldPrefab;
    }

    [SerializeField] TMP_Text fullMsg;
    [SerializeField] float fullMsgTime = 1.5f;

    [Header("UI – název vybraného předmětu")]
    [SerializeField] TMP_Text selectedItemLabel;

    [Header("Inventory")]
    public List<ItemSO> inventoryList = new List<ItemSO>();
    public int selectedItem = 0;

    [SerializeField] [Range(1, 30)] int maxSlots = 8;

    [Header("UI (volitelné, pokud je máš)")]
    [SerializeField] Image[] inventorySlotImage = new Image[9];
    [SerializeField] Image[] inventoryBackgroundImage = new Image[9];
    [SerializeField] Sprite prazdnySlotImage;

    [Header("Drop / ovládání")]
    [SerializeField] GameObject throwObject_gameobject;
    [SerializeField] KeyCode throwItemKey = KeyCode.G;

    [Header("Bindings (ItemSO -> world prefab)")]
    [SerializeField] ItemBind[] binds;

    Dictionary<ItemSO, GameObject> worldByItem = new Dictionary<ItemSO, GameObject>();

    public bool animationIsPlaying = false;

    IEnumerator ShowFullMsg()
    {
        if (!fullMsg) yield break;
        fullMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(fullMsgTime);
        fullMsg.gameObject.SetActive(false);
    }

    void UpdateSelectedLabel()
    {
        if (!selectedItemLabel) return;

        if (inventoryList.Count == 0 || inventoryList[selectedItem] == null)
            selectedItemLabel.text = "";
        else
            selectedItemLabel.text = inventoryList[selectedItem].displayName;
    }

    void Start()
    {
        worldByItem.Clear();

        foreach (var b in binds)
        {
            if (b == null || b.item == null || b.worldPrefab == null)
                continue;

            worldByItem[b.item] = b.worldPrefab;
        }

        NewItemSelected();
    }

    void Update()
    {
        if (inventoryList.Count == 0) return;

        var current = inventoryList[selectedItem];

        if (Input.GetKeyDown(throwItemKey) && !animationIsPlaying)
        {
            if (worldByItem.TryGetValue(current, out var worldPrefab) && worldPrefab)
            {
                var pos = throwObject_gameobject
                    ? throwObject_gameobject.transform.position
                    : transform.position + transform.forward * 1f;

                Instantiate(worldPrefab, pos, Quaternion.identity);
            }

            inventoryList.RemoveAt(selectedItem);
            selectedItem = Mathf.Clamp(selectedItem, 0, Mathf.Max(0, inventoryList.Count - 1));
            NewItemSelected();
            UpdateSelectedLabel();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelect(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelect(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelect(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) TrySelect(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) TrySelect(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) TrySelect(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) TrySelect(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) TrySelect(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) TrySelect(8);

        for (int i = 0; i < inventorySlotImage.Length; i++)
        {
            inventorySlotImage[i].sprite =
                (i < inventoryList.Count && inventoryList[i] != null)
                ? inventoryList[i].item_sprite
                : prazdnySlotImage;
        }

        for (int i = 0; i < inventoryBackgroundImage.Length; i++)
        {
            inventoryBackgroundImage[i].color =
                (i == selectedItem)
                ? new Color32(145, 255, 126, 255)
                : new Color32(219, 219, 219, 255);
        }
    }

    void TrySelect(int idx)
    {
        if (inventoryList.Count > idx && !animationIsPlaying)
        {
            selectedItem = idx;
            NewItemSelected();
        }
    }

    public ItemSO GetSelectedItem()
    {
        if (inventoryList.Count == 0) return null;
        return inventoryList[selectedItem];
    }

    public bool AddItem(ItemSO item)
    {
        if (!item) return false;

        if (inventoryList.Count >= maxSlots)
        {
            StopAllCoroutines();
            StartCoroutine(ShowFullMsg());
            return false;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupItemSound);
        inventoryList.Add(item);

        if (inventoryList.Count == 1)
        {
            selectedItem = 0;
            NewItemSelected();
        }

        UpdateSelectedLabel();
        return true;
    }

    public bool ConsumeSelectedIf(ItemSO expected)
    {
        if (inventoryList.Count == 0) return false;
        var current = inventoryList[selectedItem];
        if (current != expected) return false;

        inventoryList.RemoveAt(selectedItem);
        selectedItem = Mathf.Clamp(selectedItem, 0, Mathf.Max(0, inventoryList.Count - 1));

        if (inventoryList.Count == 0)
        {
            for (int i = 0; i < inventorySlotImage.Length; i++)
            {
                if (inventorySlotImage[i])
                {
                    inventorySlotImage[i].sprite = prazdnySlotImage;
                    inventorySlotImage[i].enabled = prazdnySlotImage != null;
                }

                if (inventoryBackgroundImage[i])
                    inventoryBackgroundImage[i].enabled = true;
            }

            UpdateSelectedLabel();
            return true;
        }

        NewItemSelected();
        UpdateSelectedLabel();
        return true;
    }

    void NewItemSelected()
    {
        UpdateSelectedLabel();
    }
}
