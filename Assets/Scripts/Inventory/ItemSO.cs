using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Properties")]
    public float cooldown;
    public Sprite item_sprite;

    public string displayName;
    public GameObject tableVisualPrefab;
    public bool isBook = false;
    public BookContentSO bookContent; 
}
