using UnityEngine;

public class PaperInteraction : MonoBehaviour, IInteractible
{
    public bool wasInteracted = false;
    public GameObject paperUI;
    public GameObject crosshair;
    public GameObject inventoryPanel;
    public PlayerFreeze playerFreeze;
    public string newQuest;
    public bool questChanged;

    public void Interact() {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.paperSound, 0.9f);
        if (!questChanged) {
            QuestManager.Instance.SetQuest(newQuest);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);
        }
        if (!wasInteracted) {
            paperUI.SetActive(true);
            questChanged = true;
            wasInteracted = true;
            playerFreeze.SetFrozen(true);
            crosshair.SetActive(false);
            inventoryPanel.SetActive(false);
        } else {
            paperUI.SetActive(false);
            playerFreeze.SetFrozen(false);
            crosshair.SetActive(true);
            inventoryPanel.SetActive(true);
            wasInteracted = false;
        }
    }

}
