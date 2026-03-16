using UnityEngine;

[CreateAssetMenu(fileName = "BookContent", menuName = "Items/Book Content")]
public class BookContentSO : ScriptableObject
{
    public string title;
    public Sprite[] pages;
}
