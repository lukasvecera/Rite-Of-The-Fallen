using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CircleActivation : MonoBehaviour, IInteractible
{
    public string requiredQuest;
    public string questAfterCompletion;
    [Header("Co se má zapnout při rozsvícení (nech vypnuté ve scéně)")]
    [SerializeField] private GameObject[] toEnable;

    [Header("Počáteční stav oltáře")]
    [SerializeField] private bool startLit = false;

    [Header("UI ovládání (zapni jen u JEDNOHO oltáře!)")]
    [SerializeField] private bool isUIController = false;
    [SerializeField] private TMP_Text counterText;

    private bool isLit;
    private bool firedAllLitEvent = false;

    private static int s_totalAltars = 0;
    private static int s_litAltars = 0;
    private static TMP_Text s_counterText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        s_totalAltars = 0;
        s_litAltars = 0;
        s_counterText = null;
    }

void Update()
{
    if (!isUIController || counterText == null) return;

    bool shouldShow = requiredQuest == QuestManager.Instance.GetCurrentQuest();
    counterText.gameObject.SetActive(shouldShow);
}


    private void Awake()
    {
        s_totalAltars++;

        isLit = startLit;
        ApplyVisual(isLit);

        if (isLit) s_litAltars++;

        if (isUIController)
            s_counterText = counterText;
    }

    private void Start()
    {
        if (isUIController && counterText)
            counterText.gameObject.SetActive(requiredQuest == QuestManager.Instance.GetCurrentQuest());

        UpdateUI();
        TryFireAllLit();
    }

    private void OnDestroy()
    {
        s_totalAltars = Mathf.Max(0, s_totalAltars - 1);
        if (isLit) s_litAltars = Mathf.Max(0, s_litAltars - 1);

        if (isUIController && s_counterText == counterText)
            s_counterText = null;

        UpdateUI();
    }

    public void Interact()
    {
        if (requiredQuest == QuestManager.Instance.GetCurrentQuest()) {
        if (isLit) return;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.altarSound);
        isLit = true;
        s_litAltars++;

        ApplyVisual(true);
        UpdateUI();
        TryFireAllLit();
        } else {
            return;
        }
    }

    private void ApplyVisual(bool lit)
    {
        if (toEnable != null)
        {
            foreach (var go in toEnable)
                if (go) go.SetActive(lit);
        }
    }

    private static void UpdateUI()
    {
        if (!s_counterText) return;
        int remaining = Mathf.Max(0, s_totalAltars - s_litAltars);
        s_counterText.text = $"Remaining: {remaining}/{s_totalAltars}";
    }

    private void TryFireAllLit()
    {
        if (firedAllLitEvent) return;
        if (s_totalAltars > 0 && s_litAltars == s_totalAltars)
        {
            firedAllLitEvent = true;
            QuestManager.Instance.SetQuest(questAfterCompletion);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.questCompleteSound, 0.5f);
        }
    }
}
