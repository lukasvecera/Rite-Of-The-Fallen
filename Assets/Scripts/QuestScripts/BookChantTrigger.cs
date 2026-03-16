using UnityEngine;
using TMPro;

public class BookChantTrigger : MonoBehaviour
{
    [Header("Podmínky")]
    public GameObject demon;
    public GameObject demonSpot;
    public ItemSO requiredBook;
    public string requiredQuest = "Exorcism";

    [Header("Volitelné – blízkost oltáře")]
    public Transform altar;
    public float maxDistanceToAltar = 2.5f;

    [Header("Ovládání")]
    public KeyCode chantKey = KeyCode.F;

    [Header("Minihra")]
    public TypingIncantationMinigame minigame;
    [TextArea] public string incantation = "IN NOMINE PATRIS";

    [Header("UI hint (volitelné)")]
    public TMP_Text hintLabel;

    [Header("Enemy")]
    public EnemyTracking enemyTracking;

    PlayerInventory inv;
    bool inProgress = false;

    void Awake()
    {
        inv = FindFirstObjectByType<PlayerInventory>();
        if (hintLabel) hintLabel.gameObject.SetActive(false);

        if (!enemyTracking && demon != null)
        {
            enemyTracking = demon.GetComponent<EnemyTracking>();
        }
    }

    void Update()
    {
        if (inProgress) return;
        if (!inv || !minigame) return;

        bool questOk = QuestManager.Instance.GetCurrentQuest() == requiredQuest;
        bool holdingBook = inv.GetSelectedItem() == requiredBook;
        bool nearAltar = altar ? Vector3.Distance(altar.position, GetPlayerPos()) <= maxDistanceToAltar : true;

        bool canChant = questOk && holdingBook && nearAltar;

        if (hintLabel) hintLabel.gameObject.SetActive(canChant);

        if (canChant && Input.GetKeyDown(chantKey))
        {
            StartChant();
        }
    }

    Vector3 GetPlayerPos()
    {
        var cam = Camera.main;
        return cam ? cam.transform.position : transform.position;
    }

    void StartChant()
    {
        inProgress = true;
        if (hintLabel) hintLabel.gameObject.SetActive(false);

        minigame.Begin(incantation, OnChantFinished);
    }

void OnChantFinished(bool success)
{
    if (success)
    {
        QuestManager.Instance.SetQuest("Imbrue the demon");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);

        if (enemyTracking != null)
        {
            if (enemyTracking.agent != null)
            {
                enemyTracking.agent.ResetPath();
                enemyTracking.agent.isStopped = true;
                enemyTracking.agent.updateRotation = false;
                enemyTracking.agent.updatePosition = false;
            }

            enemyTracking.enabled = false;
        }

        if (demon != null && demonSpot != null)
        {
            demon.transform.SetPositionAndRotation(
                demonSpot.transform.position,
                demonSpot.transform.rotation
            );
        }
    }

    inProgress = false;
}
}