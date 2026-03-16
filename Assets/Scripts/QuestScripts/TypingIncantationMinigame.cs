using UnityEngine;
using TMPro;
using System;

public class TypingIncantationMinigame : MonoBehaviour
{
    [Header("UI panel (CanvasGroup MinigamePanel)")]
    public CanvasGroup rootGroup;
    public TMP_Text incantationLabel;
    public TMP_InputField inputField;

    [Header("Volitelná správa hráče/kurzoru/UI")]
    public PlayerFreeze playerFreeze;
    public GameObject crosshair;
    public GameObject inventoryPanel;
    public bool manageCursor = true;

    [Header("Chování vyhodnocení")]
    public bool caseInsensitive = true;
    public bool trimWhitespace = true;

    Action<bool> onFinish;
    string target;
    bool isOpen;

    void Awake() { Show(false); }

    public void Begin(string incantation, Action<bool> onFinishCallback)
    {
        target = incantation;
        onFinish = onFinishCallback;

        if (incantationLabel) incantationLabel.text = incantation;
        if (inputField) { inputField.text = string.Empty; }

        if (playerFreeze) playerFreeze.SetFrozen(true);
        if (crosshair)      crosshair.SetActive(false);
        if (inventoryPanel) inventoryPanel.SetActive(false);
        if (manageCursor) { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }

        Show(true);
        if (inputField) inputField.ActivateInputField();

        isOpen = true;
    }

    void Update()
    {
        if (!isOpen) return;
        if (!rootGroup || rootGroup.alpha == 0f) return;
        if (!inputField) return;

        var typed = inputField.text;
        if (trimWhitespace) typed = typed.Trim();

        bool ok = caseInsensitive
            ? string.Equals(typed, target, StringComparison.OrdinalIgnoreCase)
            : typed == target;

        if (ok) Complete(true);
        if (Input.GetKeyDown(KeyCode.Escape)) Complete(false);
    }

    void Complete(bool success)
    {
        Show(false);

        if (playerFreeze) playerFreeze.SetFrozen(false);
        if (crosshair)      crosshair.SetActive(true);
        if (inventoryPanel) inventoryPanel.SetActive(true);
        if (manageCursor) { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }

        onFinish?.Invoke(success);
        onFinish = null;
        target = null;
        isOpen = false;
    }

    void Show(bool visible)
    {
        if (!rootGroup) return;
        rootGroup.alpha = visible ? 1f : 0f;
        rootGroup.interactable = visible;
        rootGroup.blocksRaycasts = visible;
    }
}
