// BookOpenController.cs
using UnityEngine;

public class BookOpenController : MonoBehaviour
{
    [Header("Refs")]
    public PlayerInventory inventory;
    public PlayerFreeze playerFreeze;
    public GameObject bookRoot;
    public Book book;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.R;


    public GameObject inventoryPanel;
    public GameObject crosshair;
    bool isOpen;

    void Start()
    {
        if (bookRoot) bookRoot.SetActive(false);
        isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (!isOpen) TryOpen();
            else Close();
        }
    }

void TryOpen()
{
    var sel = inventory ? inventory.GetSelectedItem() : null;
    if (sel == null || !sel.isBook) return;

    if (sel.bookContent != null && book != null)
    {
        book.bookPages = sel.bookContent.pages;
        book.currentPage = 0;
        RefreshBookImmediate();
    }
    AudioManager.Instance.PlaySFX(AudioManager.Instance.bookOpenSound);
    if (bookRoot) bookRoot.SetActive(true);
    if (playerFreeze) {
        playerFreeze.SetFrozen(true);
        crosshair.SetActive(false);
    }
    if (book) book.interactable = true;

    isOpen = true;
}

void RefreshBookImmediate()
{
    if (!book) return;

    if (book.LeftNext)  book.LeftNext.sprite  = book.background;
    if (book.RightNext) book.RightNext.sprite = 
        (book.bookPages != null && book.bookPages.Length > 0) ? book.bookPages[0] : book.background;

    if (book.Left)      book.Left.gameObject.SetActive(false);
    if (book.Right)     book.Right.gameObject.SetActive(false);
    if (book.Shadow)    book.Shadow.gameObject.SetActive(false);
    if (book.ShadowLTR) book.ShadowLTR.gameObject.SetActive(false);
}


    public void Close()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.bookCloseSound);
        if (bookRoot) bookRoot.SetActive(false);
        if (playerFreeze) {
            playerFreeze.SetFrozen(false);
            crosshair.SetActive(true);
            inventoryPanel.SetActive(true);
        }
        if (book) book.interactable = false;
        isOpen = false;
    }

    void ResetBookToFirstPage()
    {
        if (!book) return;
        book.currentPage = 0;

        if (book.LeftNext)  book.LeftNext.sprite  = book.background;
        if (book.RightNext) book.RightNext.sprite = (book.bookPages != null && book.bookPages.Length > 0)
            ? book.bookPages[0] : book.background;

        if (book.Left)      book.Left.gameObject.SetActive(false);
        if (book.Right)     book.Right.gameObject.SetActive(false);
        if (book.Shadow)    book.Shadow.gameObject.SetActive(false);
        if (book.ShadowLTR) book.ShadowLTR.gameObject.SetActive(false);
    }
}
